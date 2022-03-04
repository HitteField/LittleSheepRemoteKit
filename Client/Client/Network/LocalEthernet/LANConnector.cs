using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LittleSheep
{
    struct RemoteUser
    {
        public string userName;
        public IPEndPoint endPoint;
        public RemoteUser(string userName,IPEndPoint endPoint)
        {
            this.userName = userName;
            this.endPoint = endPoint;
        }
    }
    class LANConnector : ILANConnector
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly LANConnector instance = new LANConnector();
        }
        private LANConnector() { Initialization(); }
        public static LANConnector Instance { get { return Nested.instance; } }
        #endregion

        private UdpClient boardcastfd;
        private UdpClient boardcastRecvfd;

        public UdpClient BoardcastRecvfd => boardcastRecvfd;
        public UdpClient Boardcastfd => boardcastfd;
        public bool OpenToLan { get; set; }

        /// <summary>
        /// 网络事件管理器
        /// </summary>
        public NetMsgHandler msgHandler = new NetMsgHandler();

        /// <summary>
        /// 广播获取得到的局域网内的其他终端
        /// </summary>
        public List<RemoteUser> remoteUsers = new List<RemoteUser>();

        public Thread boardcastRecvThread;
        public Thread unicastRecvThread;

        public bool Initialization()
        {
            try
            {
                msgHandler.AddMsgListener("LANProbeRequestMsg", OnRecvLANProbeRequestMsg);
                msgHandler.AddMsgListener("LANProbeReplyMsg", OnRecvLANProbeReplyMsg);
                msgHandler.AddMsgListener("LANConnectRequestMsg", OnRecvLANConnectRequestMsg);
                msgHandler.AddMsgListener("LANConnectReplyMsg", OnRecvLANConnectReplyMsg);

                boardcastRecvThread = new Thread(BoardcastReceive);
                unicastRecvThread = new Thread(UnicastReceive);
                boardcastRecvThread.IsBackground = true;
                unicastRecvThread.IsBackground = true;
                boardcastRecvThread.Start();
                unicastRecvThread.Start();

                OpenToLan = false;

                return true;
            }
            catch (Exception ex)
            {
                msgHandler.RemoveAllEventListener();
                msgHandler.RemoveAllMsgListener();
                DebugKit.Warning(ex.Message);
                return false;
            }
        }

        public bool InitializationBoardcast()
        {
            try
            {
                boardcastfd = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
                
                //IPEndPoint endpoint = new IPEndPoint(IPAddress.Broadcast, 20714);
                
                return true;
            }
            catch(Exception ex)
            {
                boardcastfd = null;
                DebugKit.Warning(ex.Message);
                return false;
            }
        }

        public bool BoardcastMsg(MsgBase msg)
        {
            if (boardcastfd == null) return false;
            try
            {
                byte[] msgBytes = MsgBase.EncodeToSendBytes(msg);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Broadcast, 20714);
                boardcastfd.Send(msgBytes, msgBytes.Length, endpoint);
                return true;
            }
            catch(Exception ex)
            {
                DebugKit.Warning(ex.Message);
                return false;
            }
        }

        public void BoardcastReceive()
        {
            boardcastRecvfd = new UdpClient(new IPEndPoint(IPAddress.Any, 20714));
            DebugKit.Log("boardcastRecvfd has bound to 20714");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            while(true)
            {
                byte[] buf = boardcastRecvfd.Receive(ref endPoint);
                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);
                if(msg.protoName == "LANProbeRequestMsg")
                {
                    LANProbeRequestMsg probeRequestMsg = (LANProbeRequestMsg)msg;
                    probeRequestMsg.ip = endPoint.Address.ToString();
                    msg = (MsgBase)probeRequestMsg;
                }
                
                msgHandler.FireMsg(msg.protoName, msg);

            }
        }

        public void UnicastReceive()
        {
            UdpClient unicastRecvfd = new UdpClient(new IPEndPoint(IPAddress.Any, 20713));
            DebugKit.Log("unicastRecvfd has bound to 20713");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            while(true)
            {
                byte[] buf = boardcastRecvfd.Receive(ref endPoint);
                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);
                msgHandler.FireMsg(msg.protoName, msg);
                
                if(msg.protoName == "LANProbeReplyMsg")
                {
                    LANProbeReplyMsg probeReplyMsg = (LANProbeReplyMsg)msg;
                    remoteUsers.Add(new RemoteUser(probeReplyMsg.userName, endPoint));
                }

            }
        }

        #region 具体连接步骤
        public bool LANProbeRequest()
        {
            LANProbeRequestMsg probeRequestMsg = new LANProbeRequestMsg();
            return BoardcastMsg(probeRequestMsg);
        }

        public bool LANConnectRequest()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region 收到消息回调函数
        void OnRecvLANProbeRequestMsg(MsgBase msg)
        {
            if (OpenToLan == false) return;

            LANProbeRequestMsg probeRequestMsg = (LANProbeRequestMsg)msg;
            LANProbeReplyMsg probeReplyMsg = new LANProbeReplyMsg();
            probeReplyMsg.userName = UserInformation.Instance.Username;

            try
            {
                UdpClient sender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(probeRequestMsg.ip), 20713);
                byte[] bytes = MsgBase.EncodeToSendBytes(probeReplyMsg);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch(Exception ex)
            {
                DebugKit.Warning(ex.Message);
            }
        }

        void OnRecvLANProbeReplyMsg(MsgBase msg)
        {
            //Is there anything that needs to be done?
        }

        void OnRecvLANConnectRequestMsg(MsgBase msg)
        {

        }

        void OnRecvLANConnectReplyMsg(MsgBase msg)
        {

        }
        #endregion
    }
}
