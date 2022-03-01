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
        /// <summary>
        /// 网络事件枚举
        /// </summary>
        public enum NetEvent
        {
            ConnectSucc = 1,
            ConnectFail = 2,
            Close = 3
        }

        //套接字
        static Socket socket;
        //接收缓冲区
        static ByteArray readBuff;
        //写入队列
        static Queue<ByteArray> writeQueue;

        /// <summary>
        /// 事件的委托类型，无返回值的传参为string的方法
        /// </summary>
        /// <param name="err">参数</param>
        public delegate void EventListener(string err);
        /// <summary>
        /// 事件监听列表字典，<事件，事件的监听者>键值对
        /// </summary>
        private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        /// <summary>
        /// 消息的委托类型，无返回值的传参为MsgBase的方法
        /// </summary>
        /// <param name="msgBase"></param>
        public delegate void MsgListener(MsgBase msgBase);
        /// <summary>
        /// 消息监听列表字典，<消息名，消息的监听者>键值对
        /// </summary>
        private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

        /// <summary>
        /// 添加事件监听（注册事件）
        /// </summary>
        /// <param name="netEvent">监听的网络事件</param>
        /// <param name="listener">监听者（注册此事件的方法）</param>
        public static void AddEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                //如果已存在注册此事件的方法，就加上去（添加）
                eventListeners[netEvent] += listener;
            }
            else
            {
                //否则注册此事件（新建）
                eventListeners[netEvent] = listener;
            }
        }

        /// <summary>
        /// 删除事件监听（取消注册事件）
        /// </summary>
        /// <param name="netEvent">监听的网络事件</param>
        /// <param name="listener">要删除的监听者</param>
        public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
                if (eventListeners[netEvent] == null)
                {
                    //如果此事件已无监听者，就把此事件从监听列表中删掉
                    eventListeners.Remove(netEvent);
                }
            }
        }

        /// <summary>
        /// 添加消息监听（注册消息）
        /// </summary>
        /// <param name="msgName">监听的消息</param>
        /// <param name="listener">监听者（注册此消息的方法）</param>
        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                //如果已存在注册此消息的方法，就加上去（添加）
                msgListeners[msgName] += listener;
            }
            else
            {
                //否则注册此消息（新建）
                msgListeners[msgName] = listener;
            }
        }

        /// <summary>
        /// 删除消息监听（取消注册消息）
        /// </summary>
        /// <param name="msgName">监听的消息</param>
        /// <param name="listener">要删除的监听者</param>
        public static void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
                if (msgListeners[msgName] == null)
                {
                    //如果此消息已无监听者，就把此消息从监听列表中删掉
                    msgListeners.Remove(msgName);
                }
            }
        }

        /// <summary>
        /// 分发事件，在某事件发生的时候通知所有的监听者
        /// </summary>
        /// <param name="netEvent">发生的网络事件</param>
        /// <param name="err">给监听者们发送的传参</param>
        private static void FireEvent(NetEvent netEvent, string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        /// <summary>
        /// 分发消息，在某消息到来并被处理后通知所有监听者
        /// </summary>
        /// <param name="msgName">消息</param>
        /// <param name="msgBase">消息类</param>
        private static void FireMsg(string msgName, MsgBase msgBase)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }

        /// <summary>
        /// 需要高频更新的一些内容
        /// 需要在某个Unity脚本的Update()方法里调用
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
            if (!msgListeners.ContainsKey("MsgPong"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
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
                FireEvent(NetEvent.ConnectSucc, string.Empty);      //分发“连接成功”事件
                isConnecting = false;                               //连接结束
                                                                    //连接完成，开始接收消息
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception ex)
            {
                DebugKit.Log("Socket Connect Fail with reason: " + ex.ToString());
                FireEvent(NetEvent.ConnectFail, ex.ToString());     //分发“连接失败”事件
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
                FireEvent(NetEvent.Close, string.Empty);            //分发“关闭连接”事件
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
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);

            int len = nameBytes.Length + bodyBytes.Length;          //总长度
            byte[] sendBytes = new byte[2 + len];                   //前面还有2字节的消息总长度

            //组装
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);                           //拷贝协议名部分
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);        //拷贝协议体部分

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
                FireEvent(NetEvent.Close, string.Empty);        //分发事件
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
                FireEvent(NetEvent.Close, string.Empty);            //分发“关闭连接”事件
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
                    FireMsg(msgBase.protoName, msgBase);
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
