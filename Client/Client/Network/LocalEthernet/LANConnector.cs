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
        public RemoteUser(string userName, IPEndPoint endPoint)
        {
            this.userName = userName;
            this.endPoint = endPoint;
        }

        public bool Equals(RemoteUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // 由于发送消息用的是随机端口，所以这里只判断IP是否相同
            if (userName != other.userName || endPoint.Address.ToString() != other.endPoint.Address.ToString()) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemoteUser)obj);
        }

        public override int GetHashCode()
        {
            return ((endPoint != null && userName != null) ? endPoint.Address.ToString().GetHashCode() + userName.GetHashCode() : 0);
        }

        public static bool operator ==(RemoteUser left, RemoteUser right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteUser left, RemoteUser right)
        {
            return !Equals(left, right);
        }

    }
    class LANConnector : ILANConnector
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly LANConnector instance = new LANConnector();
        }
        private LANConnector() { }
        public static LANConnector Instance { get { return Nested.instance; } }
        #endregion

        private UdpClient boardcastfd;
        private UdpClient boardcastRecvfd;

        public UdpClient BoardcastRecvfd => boardcastRecvfd;
        public UdpClient Boardcastfd => boardcastfd;
        public bool OpenToLan { get; set; }

        public bool HasInit = false;

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
                boardcastfd = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

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
                HasInit = true;

                DebugKit.Log("LANConnector has inited");

                return true;
            }
            catch (Exception ex)
            {
                boardcastfd = null;
                msgHandler.RemoveAllEventListener();
                msgHandler.RemoveAllMsgListener();
                DebugKit.Warning("[Initialization]" + ex.Message);
                HasInit = false;
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
                DebugKit.Log("BoardcastMsg has been sent");
                return true;
            }
            catch(Exception ex)
            {
                DebugKit.Warning("[BoardcastMsg]" + ex.Message);
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

                DebugKit.Log("Has recved a boardcastMsg");

                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);

                object[] args = new object[1];
                if(msg.protoName == "LANProbeRequestMsg")
                {
                    args[0] = endPoint;
                }

                msgHandler.FireMsg(msg.protoName, msg, args);

            }
        }

        public void UnicastReceive()
        {
            UdpClient unicastRecvfd = new UdpClient(new IPEndPoint(IPAddress.Any, 20713));
            DebugKit.Log("unicastRecvfd has bound to 20713");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            while(true)
            {
                byte[] buf = unicastRecvfd.Receive(ref endPoint);

                DebugKit.Log("Has recved a unicastMsg");

                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);

                object[] args = new object[1];
                if (msg.protoName == "LANProbeReplyMsg")
                {
                    args[0] = endPoint;
                }

                msgHandler.FireMsg(msg.protoName, msg, args);

            }
        }

        #region 具体连接步骤
        public bool LANProbeRequest()
        {
            LANProbeRequestMsg probeRequestMsg = new LANProbeRequestMsg();
            return BoardcastMsg(probeRequestMsg);
        }

        public bool LANConnectRequest(RemoteUser remoteUser)
        {
            LANConnectRequestMsg connectRequestMsg = new LANConnectRequestMsg();
            connectRequestMsg.userName = UserInformation.Instance.Username;
            try
            {
                UdpClient sender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
                IPEndPoint endPoint = new IPEndPoint(remoteUser.endPoint.Address, 20713);
                byte[] bytes = MsgBase.EncodeToSendBytes(connectRequestMsg);
                sender.Send(bytes, bytes.Length, endPoint);
                return true;
            }
            catch (Exception ex)
            {
                DebugKit.Warning("[LANConnectRequest]" + ex.Message);
                return false;
            }
        }

        #endregion


        #region 收到消息回调函数
        /// <summary>
        /// 收到广播探查报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANProbeRequestMsg(MsgBase msg, object[] args)
        {
            //如果对局域网开放，则需要给对方回复

            if (OpenToLan == false) return;

            //LANProbeRequestMsg probeRequestMsg = (LANProbeRequestMsg)msg;
            LANProbeReplyMsg probeReplyMsg = new LANProbeReplyMsg();
            probeReplyMsg.userName = UserInformation.Instance.Username;
            IPEndPoint targetEndPoint = (IPEndPoint)args[0];

            try
            {
                UdpClient sender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
                IPEndPoint endPoint = new IPEndPoint(targetEndPoint.Address, 20713);
                byte[] bytes = MsgBase.EncodeToSendBytes(probeReplyMsg);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch(Exception ex)
            {
                DebugKit.Warning("[OnRecvLANProbeRequestMsg]" + ex.Message);
            }
        }

        /// <summary>
        /// 收到探查回复报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANProbeReplyMsg(MsgBase msg, object[] args)
        {
            //收到了来自其他设备的探查回复，获取其用户名与EndPoint添加到[远程用户]列表中

            LANProbeReplyMsg probeReplyMsg = (LANProbeReplyMsg)msg;
            IPEndPoint endPoint = (IPEndPoint)args[0];
            RemoteUser newUser = new RemoteUser(probeReplyMsg.userName, endPoint);
            if (!remoteUsers.Contains(newUser))
            {
                remoteUsers.Add(newUser);
                DebugKit.Log("New Remote User:" + newUser.ToString());
            }
        }

        /// <summary>
        /// 收到连接请求报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANConnectRequestMsg(MsgBase msg, object[] args)
        {
            //收到了连接请求时，需要给对方一个回复：是否同意连接
        }

        /// <summary>
        /// 收到连接请求回复报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANConnectReplyMsg(MsgBase msg, object[] args)
        {

        }
        #endregion
    }
}
