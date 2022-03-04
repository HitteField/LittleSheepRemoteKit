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

        //�׽���
        static Socket socket;
        //���ջ�����
        static ByteArray readBuff;
        //д�����
        static Queue<ByteArray> writeQueue;
        //�����¼�������
        static NetMsgHandler msgHandler = new NetMsgHandler();
        

        /// <summary>
        /// ��Ҫ��Ƶ���µ�һЩ����
        /// ��Ҫ��ĳ��while(true)�����
        /// </summary>
        public static void Update()
        {
            if (socket == null || !socket.Connected) return;
            MsgUpdate();            //��Ϣ����
            PingUpdate();           //����
        }

        //--------------------------------���ӷ�����--------------------------------------

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        static bool isConnecting = false;
        /// <summary>
        /// �Ƿ����ڹر�
        /// </summary>
        static bool isClosing = false;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        private static void InitState()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //��ʼ���׽���
            readBuff = new ByteArray();                                             //��ʼ����������
            writeQueue = new Queue<ByteArray>();                                    //��ʼ��д�����
            msgList = new List<MsgBase>();                                          //��ʼ����Ϣ�б�

            lastPingTime = TimeKit.GetTimeStamp();       //��ʼ������PING
            lastPongTime = TimeKit.GetTimeStamp();       //��ʼ������PONG
            msgCount = 0;                   //��ʼʱ��Ϣ�б�ȻΪ��
            isConnecting = false;           //��ʼ����ʱ��û��ʼ����
            isClosing = false;              //��ȻҲû�ر�

            //����PONGЭ�飨�������ƣ�
            if (!msgHandler.HasMsgListener("MsgPong"))
            {
                msgHandler.AddMsgListener("MsgPong", OnMsgPong);
            }
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
        /// ���ӷ�����
        /// </summary>
        /// <param name="connectMethod">���ӷ�ʽ</param>
        /// <param name="str">�����ַ���</param>
        /// <param name="port">�˿�</param>
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
            //״̬�ж�
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
            //��ʼ����Ա
            InitState();
            //������Nagle�㷨
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
        public static void Close()
        {
            if (socket == null || !socket.Connected) return;
            if (isConnecting) return;
            //���д����л������ݣ����ӳٹرգ�����isClosingΪtrue��Ϊ���ڹر���
            //�÷����¼��Ļص�����ȥ����ر�����
            if (writeQueue.Count > 0) isClosing = true;
            else
            {
                socket.Close();
                msgHandler.FireEvent(NetEvent.Close, string.Empty);            //�ַ����ر����ӡ��¼�
            }

        }

        /// <summary>
        /// ǿ�ƹر�����
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

        //--------------------------------��������--------------------------------------

        /// <summary>
        /// ����һ����Ϣ
        /// </summary>
        /// <param name="msg">Ҫ���͵���Ϣ</param>
        public static void Send(MsgBase msg)
        {
            if (socket == null || !socket.Connected) return;
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
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
        }

        private static void SendCallback(IAsyncResult ar)
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
        static List<MsgBase> msgList = new List<MsgBase>();
        /// <summary>
        /// ��Ϣ�б���
        /// </summary>
        static int msgCount = 0;
        /// <summary>
        /// ÿһ��Update�������Ϣ�����ֵ
        /// </summary>
        readonly static int MAX_MESSAGE_FIRE = 10;

        private static void ReceiveCallback(IAsyncResult ar)
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
        public static void OnReceiveData()
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

        public static void MsgUpdate()
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
        public static bool isUsePing = true;
        /// <summary>
        /// �������ʱ��
        /// </summary>
        public static int pintInterval = 5;
        /// <summary>
        /// ��һ�η���PING��ʱ��
        /// </summary>
        static long lastPingTime = 0;
        /// <summary>
        /// ��һ���յ�PONG��ʱ��
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
