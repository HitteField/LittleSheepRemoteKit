using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Reflection;

namespace LittleSheep
{
    class NetManager
    {
        /// <summary>
        /// 服务端监听套接字
        /// </summary>
        public static Socket listenfd;
        /// <summary>
        /// 客户端[套接字，状态信息]字典
        /// </summary>
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        /// <summary>
        /// 多路复用Select检查列表
        /// </summary>
        static List<Socket> checkRead = new List<Socket>();
        /// <summary>
        /// 心跳包时间间隔
        /// </summary>
        public static long pingInterval = 5;

        /// <summary>
        /// 开启服务端监听，传入监听端口
        /// </summary>
        /// <param name="listenPort">监听端口</param>
        public static void StartLoop(int listenPort)
        {
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPEndPoint ipEp = new IPEndPoint(IPAddress.Any, listenPort);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);

            DebugKit.Log("Server started up suuccessfully.");

            while (true)
            {
                ResetCheckRead();
                //只监听是否可读
                Socket.Select(checkRead, null, null, 1000);
                //先客户端Recv再服务端Accept，逆序检查可读对象
                for (int i = checkRead.Count - 1; i >= 0; --i)
                {
                    Socket s = checkRead[i];
                    if (s == listenfd)
                    {
                        ReadListenfd();
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
                //时间监视器，由于上面多路复用的阻塞时间为1000ms，故大约一秒调用一次这玩意
                //每处理一轮读取消息，调用一次Timer()
                Timer();
            }
        }

        /// <summary>
        /// 重置多路复用列表
        /// </summary>
        public static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (var s in clients.Values)
                checkRead.Add(s.socket);
        }

        /// <summary>
        /// 时间监视器
        /// </summary>
        static void Timer()
        {
            MethodInfo mi = typeof(EventHandler).GetMethod("OnTimer");
            object[] ob = { };
            mi.Invoke(null, ob);
        }

        /// <summary>
        /// 监听套接字监听到了新的连接
        /// </summary>
        public static void ReadListenfd()
        {
            try
            {
                //监听到新连接
                Socket clientfd = listenfd.Accept();
                DebugKit.Log("Accept new client: " + clientfd.RemoteEndPoint.ToString());
                //为其创建一个新的客户端状态信息并加入到字典中
                ClientState state = new ClientState();
                state.socket = clientfd;
                state.remoteName = clientfd.RemoteEndPoint.ToString();
                state.lastPingTime = TimeKit.GetTimeStamp();
                clients.Add(clientfd, state);

                //MsgVersionCheck msgVersionCheck = new MsgVersionCheck();
                //msgVersionCheck.version = CurrentVersion.curVersion;
                //Send(state, msgVersionCheck);

            }
            catch (Exception ex)
            {
                DebugKit.Warning("Accept fail: " + ex.ToString());
            }
        }

        /// <summary>
        /// 收到了来自客户端的消息
        /// </summary>
        /// <param name="clientfd">客户端信息</param>
        public static void ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            ByteArray readBuff = state.readBuff;
            //接收
            int count = 0;
            if (readBuff.Remain <= 0)
            {
                //尝试读取以拓展remain空间
                OnReceiveData(state);
                readBuff.MoveBytes();
                if (readBuff.Remain <= 0)
                {
                    DebugKit.Warning("Client " + state.remoteName + " receive fail, maybe msg length > buff capacity.");
                    //关闭套接字
                    Close(state);
                    return;
                }
            }

            try
            {
                //尝试接收消息
                count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0);
            }
            catch (Exception ex)
            {
                DebugKit.Error("Client " + state.remoteName + " receive socket exception " + ex.ToString());
                Close(state);
                return;
            }

            //收到FIN，客户端关闭
            if (count <= 0)
            {
                DebugKit.Log("Client " + state.remoteName + " socket close.");
                Close(state);
                return;
            }

            //可写位置往后挪count个字节
            readBuff.writeIdx += count;
            //处理接收到的信息
            OnReceiveData(state);
            readBuff.CheckAndMoveBytes();

        }

        /// <summary>
        /// 接收到数据后进行处理
        /// </summary>
        /// <param name="state">要处理的客户端状态信息</param>
        public static void OnReceiveData(ClientState state)
        {
            ByteArray readBuff = state.readBuff;                //byteArray结构
            byte[] bytes = readBuff.bytes;                      //缓冲区
            if (readBuff.Length <= 2) return;                   //长度不够，连消息有多长都拼不出来

            Int16 bodyLength = (Int16)((bytes[readBuff.readIdx + 1] << 8) | bytes[readBuff.readIdx]);
            if (readBuff.Length < bodyLength + 2) return;           //长度不够，说明不足以拼出一条信息

            //如果可以拼出一条信息，不能忘记上面已经处理了2字节的内容
            readBuff.readIdx += 2;

            //解析协议名
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out int nameCount);
            if (protoName == "")
            {
                DebugKit.Error("Client " + state.remoteName + " msg decode name fail.");
                Close(state);
            }
            //处理了nameCount字节的信息，向前推进readIdx
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);

            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();

            //分发消息
            //根据protoName获取MsgHandler类中的对应的静态方法（处理某个协议的静态方法 和 此协议的名字 相同）
            //然后把发送此协议的Client的State与协议对象本身作为此静态方法的参数传递，Invoke此静态方法
            MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
            object[] ob = { state, msgBase };
            if (protoName != "MsgPing") DebugKit.Log("Receive " + protoName + " from client " + state.remoteName + ".");
            if (mi != null) mi.Invoke(null, ob);
            else DebugKit.Warning("Receive unknow msg " + protoName + " from client " + state.remoteName + ".");

            //继续读取消息
            if (readBuff.Length > 2) OnReceiveData(state);
        }

        /// <summary>
        /// 关闭某个客户端连接
        /// </summary>
        /// <param name="state">要关闭的客户端状态信息</param>
        public static void Close(ClientState state)
        {
            //分发事件
            MethodInfo mi = typeof(EventHandler).GetMethod("OnDisconnect");
            object[] ob = { state };
            mi.Invoke(null, ob);
            //关闭套接字
            state.socket.Close();
            clients.Remove(state.socket);
        }

        /// <summary>
        /// 给目标客户端state发送一条消息msg
        /// </summary>
        /// <param name="state">目标客户端</param>
        /// <param name="msg">要发送的消息</param>
        public static void Send(ClientState state, MsgBase msg)
        {
            if (state == null) return;
            if (!state.socket.Connected) return;

            //数据编码
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[len + 2];
            //组装
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

            //发送
            try
            {
                state.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
                //Tool.DebugLog("Send a msg to client " + state.RemoteName);
            }
            catch (Exception ex)
            {
                DebugKit.Error("Fail sending msg on BeginSend() " + ex.ToString());
            }
        }



    }

}
