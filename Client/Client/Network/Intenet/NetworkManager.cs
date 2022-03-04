using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;

namespace LittleSheep
{
    public class NetManager
    {

        //套接字
        static Socket socket;
        //接收缓冲区
        static ByteArray readBuff;
        //写入队列
        static Queue<ByteArray> writeQueue;
        //网络事件管理器
        static NetMsgHandler msgHandler = new NetMsgHandler();
        

        /// <summary>
        /// 需要高频更新的一些内容
        /// 需要在某个while(true)里调用
        /// </summary>
        public static void Update()
        {
            if (socket == null || !socket.Connected) return;
            MsgUpdate();            //消息处理
            PingUpdate();           //心跳
        }

        //--------------------------------连接服务器--------------------------------------

        /// <summary>
        /// 是否正在连接
        /// </summary>
        static bool isConnecting = false;
        /// <summary>
        /// 是否正在关闭
        /// </summary>
        static bool isClosing = false;

        /// <summary>
        /// 初始化变量
        /// </summary>
        private static void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //初始化套接字
            readBuff = new ByteArray();                                             //初始化读缓冲区
            writeQueue = new Queue<ByteArray>();                                    //初始化写入队列
            msgList = new List<MsgBase>();                                          //初始化消息列表

            lastPingTime = TimeKit.GetTimeStamp();       //初始化心跳PING
            lastPongTime = TimeKit.GetTimeStamp();       //初始化心跳PONG
            msgCount = 0;                   //初始时消息列表当然为空
            isConnecting = false;           //初始化的时候还没开始连接
            isClosing = false;              //当然也没关闭

            //监听PONG协议（心跳机制）
            if (!msgHandler.HasMsgListener("MsgPong"))
            {
                msgHandler.AddMsgListener("MsgPong", OnMsgPong);
            }
        }

        /// <summary>
        /// 连接方式枚举
        /// </summary>
        public enum ConnectMethod
        {
            IPaddress = 1,
            DNS = 2
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="connectMethod">连接方式</param>
        /// <param name="str">连接字符串</param>
        /// <param name="port">端口</param>
        public static void Connect(ConnectMethod connectMethod, string str, int port)
        {
            string ip = "";
            if (connectMethod == ConnectMethod.IPaddress)
            {
                ip = str;
            }
            else if (connectMethod == ConnectMethod.DNS)
            {
                ip = Dns.GetHostAddresses(str)[0].ToString();
            }
            //状态判断
            if (socket != null && socket.Connected)
            {
                DebugKit.Warning("Conect fail, already connected.");
                return;
            }
            if (isConnecting)
            {
                DebugKit.Log("Is connecting, plz waiting.");
                return;
            }
            //初始化成员
            InitState();
            //不启用Nagle算法
            socket.NoDelay = true;

            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallback, socket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                DebugKit.Log("Socket Connect Succ.");
                msgHandler.FireEvent(NetEvent.ConnectSucc, string.Empty);      //分发“连接成功”事件
                isConnecting = false;                               //连接结束
                                                                    //连接完成，开始接收消息
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception ex)
            {
                DebugKit.Log("Socket Connect Fail with reason: " + ex.ToString());
                msgHandler.FireEvent(NetEvent.ConnectFail, ex.ToString());     //分发“连接失败”事件
                isConnecting = false;                               //连接结束
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void Close()
        {
            if (socket == null || !socket.Connected) return;
            if (isConnecting) return;
            //如果写入队列还有内容，就延迟关闭，设置isClosing为true意为正在关闭中
            //让发送事件的回调函数去处理关闭连接
            if (writeQueue.Count > 0) isClosing = true;
            else
            {
                socket.Close();
                msgHandler.FireEvent(NetEvent.Close, string.Empty);            //分发“关闭连接”事件
            }

        }

        /// <summary>
        /// 强制关闭连接
        /// </summary>
        public static void ForceShutdown()
        {
            if (socket == null || !socket.Connected) return;
            if (isConnecting) return;
            lock (writeQueue)
            {
                writeQueue.Clear();
            }
            socket.Close();

        }

        //--------------------------------发送数据--------------------------------------

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        public static void Send(MsgBase msg)
        {
            if (socket == null || !socket.Connected) return;
            if (isConnecting) return;
            if (isClosing) return;

            //数据编码
            byte[] sendBytes = MsgBase.EncodeToSendBytes(msg);

            //写入队列
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;                                          //写入队列的长度
            lock (writeQueue)
            {
                writeQueue.Enqueue(ba);                             //入队
                count = writeQueue.Count;
            }

            if (count == 1)          //count大于1的时候会在回调函数中递归处理
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket == null || !socket.Connected) return;
            int count = socket.EndSend(ar);     //成功发送的字节数

            //获取写入队列第一条数据
            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();        //取队首
            }

            ba.readIdx += count;                //读下标加上成功发送的字节数
            if (ba.Length == 0)                    //当前消息全部发送完成
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();       //全都发送完成了就出队
                    if (writeQueue.Count > 0) ba = writeQueue.First();    //换下一个
                    else ba = null;
                }
            }

            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.Length, 0, SendCallback, socket);
            }
            else if (isClosing)                  //全都发完了后康康是否正在关闭
            {
                socket.Close();
                msgHandler.FireEvent(NetEvent.Close, string.Empty);        //分发事件
                isClosing = false;
            }

            //Debug.Log("Send finished.");
        }

        //--------------------------------接收数据--------------------------------------

        /// <summary>
        /// 消息列表
        /// </summary>
        static List<MsgBase> msgList = new List<MsgBase>();
        /// <summary>
        /// 消息列表长度
        /// </summary>
        static int msgCount = 0;
        /// <summary>
        /// 每一次Update处理的消息量最大值
        /// </summary>
        readonly static int MAX_MESSAGE_FIRE = 10;

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);      //成功接收到的字节数
                if (count == 0)  //收到了FIN信号
                {
                    Close();
                    return;
                }
                readBuff.writeIdx += count;
                //处理二进制消息
                OnReceiveData();
                //继续接收
                if (readBuff.Remain < 8)
                {
                    //容量不足时需要拓充
                    readBuff.MoveBytes();
                    readBuff.ReSize(readBuff.Length * 2);
                }
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception ex)
            {
                msgHandler.FireEvent(NetEvent.Close, string.Empty);            //分发“关闭连接”事件
                DebugKit.Warning("Socket Receive Fail with reason: " + ex.ToString());
            }
        }

        /// <summary>
        /// 读取并解析收到的消息，将消息放入消息列表中
        /// </summary>
        public static void OnReceiveData()
        {
            if (readBuff.Length <= 2) return;

            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.Length < bodyLength + 2) return;       //此消息没接收完全

            readBuff.readIdx += 2;
            //解析协议名
            int nameCount;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);

            if (protoName == "")
            {
                DebugKit.Warning("OnReceiveData MsgBase.DecodeName fail");
                return;
            }

            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();

            //添加到消息队列
            lock (msgList)
            {
                msgList.Add(msgBase);
            }
            msgCount++;

            //继续读取消息
            if (readBuff.Length > 2)
                OnReceiveData();
        }

        public static void MsgUpdate()
        {
            if (msgCount == 0) return;

            for (int i = 0; i < MAX_MESSAGE_FIRE; ++i)         //每次Update需要做最多MAX_MESSAGE_FIRE条消息的处理
            {
                MsgBase msgBase = null;
                lock (msgList)
                {
                    if (msgList.Count > 0)
                    {
                        msgBase = msgList[0];
                        msgList.RemoveAt(0);
                        msgCount--;
                    }
                }

                if (msgBase != null)
                {
                    msgHandler.FireMsg(msgBase.protoName, msgBase);
                }
                else break;             //没有新的消息了
            }
        }

        //--------------------------------心跳机制--------------------------------------

        /// <summary>
        /// 是否启用心跳机制
        /// </summary>
        public static bool isUsePing = true;
        /// <summary>
        /// 心跳间隔时间
        /// </summary>
        public static int pintInterval = 5;
        /// <summary>
        /// 上一次发送PING的时间
        /// </summary>
        static long lastPingTime = 0;
        /// <summary>
        /// 上一次收到PONG的时间
        /// </summary>
        static long lastPongTime = 0;

        private static void PingUpdate()
        {
            if (!isUsePing) return;

            if (TimeKit.GetTimeStamp() - lastPingTime > pintInterval)
            {
                MsgPing msgPing = new MsgPing();
                Send(msgPing);
                lastPingTime = TimeKit.GetTimeStamp();
            }

            if (TimeKit.GetTimeStamp() - lastPongTime > pintInterval * 3)
            {
                Close();
            }
        }

        private static void OnMsgPong(MsgBase msgBase)
        {
            lastPongTime = TimeKit.GetTimeStamp();
        }
    }
}
