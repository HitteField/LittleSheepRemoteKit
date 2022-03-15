using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace LittleSheep
{
    /// <summary>
    /// ����TCP����ר�����ڽ���Msg�Ĺ�����
    /// </summary>
    public class TcpMsgManager
    {

        //�Ի��׽���
        Socket clientfd;
        //���ջ�����
        ByteArray readBuff;
        //д�����
        Queue<ByteArray> writeQueue;
        //�����¼�������
        NetMsgHandler msgHandler = new NetMsgHandler();
        //�����Ƿ��ǽ��ն�
        bool isServer = false;
        //������ʹ�õĶ˿�
        int port = -1;
        
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="port"></param>
        public TcpMsgManager(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// ��Ҫ��Ƶ���µ�һЩ����
        /// ��Ҫ��ĳ��while(true)�����
        /// </summary>
        public void Update()
        {
            if (clientfd == null || !clientfd.Connected) return;
            MsgUpdate();            //��Ϣ����
            PingUpdate();           //����
        }

        //--------------------------------����--------------------------------------

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        bool isConnecting = false;
        /// <summary>
        /// �Ƿ����ڹر�
        /// </summary>
        bool isClosing = false;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        private void InitState()
        {
            if(!isServer)
            {
                clientfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //��ʼ���׽���                                 

                lastPingTime = TimeKit.GetTimeStamp();       //��ʼ������PING
                lastPongTime = TimeKit.GetTimeStamp();       //��ʼ������PONG

                //����PONGЭ�飨�������ƣ�
                if (!msgHandler.HasMsgListener("MsgPong"))
                {
                    msgHandler.AddMsgListener("MsgPong", OnMsgPong);
                }
            }

            readBuff = new ByteArray();                                             //��ʼ����������
            writeQueue = new Queue<ByteArray>();                                    //��ʼ��д�����
            msgList = new List<MsgBase>();                                          //��ʼ����Ϣ�б�
            msgCount = 0;                   //��ʼʱ��Ϣ�б�ȻΪ��
            isConnecting = false;           //��ʼ����ʱ��û��ʼ����
            isClosing = false;              //��ȻҲû�ر�
        }

        /// <summary>
        /// ���ô�TCP�������õĶ˿�
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// ���ӷ�ʽö��
        /// </summary>
        public enum ConnectMethod
        {
            IPaddress = 1,
            DNS = 2
        }

        /// <summary>
        /// ���������ͻ��˵���������
        /// </summary>
        /// <param name="remoteIp">Զ�˿ͻ��˵�IP</param>
        public void Accept(string remoteIp)
        {
            isUsePing = false;
            isServer = true;
            InitState();
            Thread acceptThread = new Thread(new ParameterizedThreadStart(AcceptThread));
            acceptThread.Start(remoteIp);
        }

        private void AcceptThread(object remoteIp)
        {
            isConnecting = true;
            try
            {
                Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEp = new IPEndPoint(IPAddress.Any, port);
                listenfd.Bind(ipEp);
                listenfd.Listen(1);

               while(true)
               {
                    clientfd = listenfd.Accept();
                    if (((IPEndPoint)clientfd.RemoteEndPoint).Address.ToString() != (string)remoteIp)
                    {
                        clientfd.Close();
                        clientfd.Dispose();
                    }
                    else break;
               }
                
                clientfd.NoDelay = true;

                DebugKit.Log($"TcpMsgManager has accepted remoteUser{(string)remoteIp}'s connection");
                msgHandler.FireEvent(NetEvent.ConnectSucc, string.Empty);     
                isConnecting = false;
                clientfd.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, clientfd);
            }
            catch (Exception ex)
            {
                DebugKit.Error($"[TcpMsgManager.AcceptThread]{ex.Message}");
                msgHandler.FireEvent(NetEvent.ConnectFail, ex.ToString());     
                isConnecting = false;
            }
        }

        /// <summary>
        /// ���ӷ�����
        /// </summary>
        /// <param name="connectMethod">���ӷ�ʽ</param>
        /// <param name="targetHoseName">�����ַ���</param>
        /// <param name="isUsePing">�Ƿ�ʹ��������</param>
        public void Connect(ConnectMethod connectMethod, string targetHoseName, bool isUsePing)
        {
            isServer = false;
            string ip = "";
            if (connectMethod == ConnectMethod.IPaddress)
            {
                ip = targetHoseName;
            }
            else if (connectMethod == ConnectMethod.DNS)
            {
                ip = Dns.GetHostAddresses(targetHoseName)[0].ToString();
            }
            //״̬�ж�
            if (clientfd != null && clientfd.Connected)
            {
                DebugKit.Warning("Conect fail, already connected.");
                return;
            }
            if (isConnecting)
            {
                DebugKit.Log("Is connecting, plz waiting.");
                return;
            }
            //��ʼ����Ա
            InitState();
            //������Nagle�㷨
            clientfd.NoDelay = true;
            this.isUsePing = isUsePing;
            isConnecting = true;
            clientfd.BeginConnect(ip, port, ConnectCallback, clientfd);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                DebugKit.Log("Socket Connect Succ.");
                msgHandler.FireEvent(NetEvent.ConnectSucc, string.Empty);      //�ַ������ӳɹ����¼�
                isConnecting = false;                               //���ӽ���
                                                                    //������ɣ���ʼ������Ϣ
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception ex)
            {
                DebugKit.Log("Socket Connect Fail with reason: " + ex.ToString());
                msgHandler.FireEvent(NetEvent.ConnectFail, ex.ToString());     //�ַ�������ʧ�ܡ��¼�
                isConnecting = false;                               //���ӽ���
            }
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        public void Close()
        {
            if (clientfd == null || !clientfd.Connected) return;
            if (isConnecting) return;
            //���д����л������ݣ����ӳٹرգ�����isClosingΪtrue��Ϊ���ڹر���
            //�÷����¼��Ļص�����ȥ����ر�����
            if (writeQueue.Count > 0) isClosing = true;
            else
            {
                clientfd.Close();
                msgHandler.FireEvent(NetEvent.Close, string.Empty);            //�ַ����ر����ӡ��¼�
            }

        }

        /// <summary>
        /// ǿ�ƹر�����
        /// </summary>
        public void ForceShutdown()
        {
            if (clientfd == null || !clientfd.Connected) return;
            if (isConnecting) return;
            lock (writeQueue)
            {
                writeQueue.Clear();
            }
            clientfd.Close();

        }

        //--------------------------------��������--------------------------------------

        /// <summary>
        /// ����һ����Ϣ
        /// </summary>
        /// <param name="msg">Ҫ���͵���Ϣ</param>
        public void Send(MsgBase msg)
        {
            if (clientfd == null || !clientfd.Connected) return;
            if (isConnecting) return;
            if (isClosing) return;

            //���ݱ���
            byte[] sendBytes = MsgBase.EncodeToSendBytes(msg);

            //д�����
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;                                          //д����еĳ���
            lock (writeQueue)
            {
                writeQueue.Enqueue(ba);                             //���
                count = writeQueue.Count;
            }

            if (count == 1)          //count����1��ʱ����ڻص������еݹ鴦��
            {
                clientfd.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, clientfd);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket == null || !socket.Connected) return;
            int count = socket.EndSend(ar);     //�ɹ����͵��ֽ���

            //��ȡд����е�һ������
            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();        //ȡ����
            }

            ba.readIdx += count;                //���±���ϳɹ����͵��ֽ���
            if (ba.Length == 0)                    //��ǰ��Ϣȫ���������
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();       //ȫ����������˾ͳ���
                    if (writeQueue.Count > 0) ba = writeQueue.First();    //����һ��
                    else ba = null;
                }
            }

            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.Length, 0, SendCallback, socket);
            }
            else if (isClosing)                  //ȫ�������˺󿵿��Ƿ����ڹر�
            {
                socket.Close();
                msgHandler.FireEvent(NetEvent.Close, string.Empty);        //�ַ��¼�
                isClosing = false;
            }

            //Debug.Log("Send finished.");
        }

        //--------------------------------��������--------------------------------------

        /// <summary>
        /// ��Ϣ�б�
        /// </summary>
        List<MsgBase> msgList = new List<MsgBase>();
        /// <summary>
        /// ��Ϣ�б���
        /// </summary>
        int msgCount = 0;
        /// <summary>
        /// ÿһ��Update�������Ϣ�����ֵ
        /// </summary>
        readonly int MAX_MESSAGE_FIRE = 10;

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);      //�ɹ����յ����ֽ���
                if (count == 0)  //�յ���FIN�ź�
                {
                    Close();
                    return;
                }
                readBuff.writeIdx += count;
                //�����������Ϣ
                OnReceiveData();
                //��������
                if (readBuff.Remain < 8)
                {
                    //��������ʱ��Ҫ�س�
                    readBuff.MoveBytes();
                    readBuff.ReSize(readBuff.Length * 2);
                }
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception ex)
            {
                msgHandler.FireEvent(NetEvent.Close, string.Empty);            //�ַ����ر����ӡ��¼�
                DebugKit.Warning("Socket Receive Fail with reason: " + ex.ToString());
            }
        }

        /// <summary>
        /// ��ȡ�������յ�����Ϣ������Ϣ������Ϣ�б���
        /// </summary>
        public void OnReceiveData()
        {
            if (readBuff.Length <= 2) return;

            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.Length < bodyLength + 2) return;       //����Ϣû������ȫ

            readBuff.readIdx += 2;
            //����Э����
            int nameCount;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);

            if (protoName == "")
            {
                DebugKit.Warning("OnReceiveData MsgBase.DecodeName fail");
                return;
            }

            readBuff.readIdx += nameCount;
            //����Э����
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();

            //��ӵ���Ϣ����
            lock (msgList)
            {
                msgList.Add(msgBase);
            }
            msgCount++;

            //������ȡ��Ϣ
            if (readBuff.Length > 2)
                OnReceiveData();
        }

        public void MsgUpdate()
        {
            if (msgCount == 0) return;

            for (int i = 0; i < MAX_MESSAGE_FIRE; ++i)         //ÿ��Update��Ҫ�����MAX_MESSAGE_FIRE����Ϣ�Ĵ���
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
                else break;             //û���µ���Ϣ��
            }
        }

        //--------------------------------��������--------------------------------------

        /// <summary>
        /// �Ƿ�������������
        /// </summary>
        public bool isUsePing = true;
        /// <summary>
        /// �������ʱ��
        /// </summary>
        public int pintInterval = 5;
        /// <summary>
        /// ��һ�η���PING��ʱ��
        /// </summary>
        long lastPingTime = 0;
        /// <summary>
        /// ��һ���յ�PONG��ʱ��
        /// </summary>
        long lastPongTime = 0;

        private void PingUpdate()
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

        private void OnMsgPong(MsgBase msgBase, object[] args)
        {
            lastPongTime = TimeKit.GetTimeStamp();
        }
    }
}
