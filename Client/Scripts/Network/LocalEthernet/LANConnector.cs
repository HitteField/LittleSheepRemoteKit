using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LittleSheep
{
    
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

        private UdpClient broadcastfd;
        private UdpClient broadcastRecvfd;

        public UdpClient BroadcastRecvfd => broadcastRecvfd;
        public UdpClient Broadcastfd => broadcastfd;
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
        /// <summary>
        /// 本机在局域网内的全部IP
        /// </summary>
        public List<RemoteUser> locateUsers = new List<RemoteUser>();

        public Thread broadcastRecvThread;
        public Thread unicastRecvThread;

        public bool Init()
        {
            try
            {
                broadcastfd = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

                msgHandler.AddMsgListener("LANProbeRequestMsg", OnRecvLANProbeRequestMsg);
                msgHandler.AddMsgListener("LANProbeReplyMsg", OnRecvLANProbeReplyMsg);
                //msgHandler.AddMsgListener("LANConnectRequestMsg", OnRecvLANConnectRequestMsg);
                msgHandler.AddMsgListener("LANConnectReplyMsg", OnRecvLANConnectReplyMsg);

                broadcastRecvThread = new Thread(BroadcastReceive);
                unicastRecvThread = new Thread(UnicastReceive);
                broadcastRecvThread.IsBackground = true;
                unicastRecvThread.IsBackground = true;
                unicastRecvThread.SetApartmentState(ApartmentState.STA);
                broadcastRecvThread.Start();
                unicastRecvThread.Start();

                OpenToLan = false;
                HasInit = true;

                DebugKit.Log("LANConnector has inited");

                LANProbeRequest();

                return true;
            }
            catch (Exception ex)
            {
                broadcastfd = null;
                msgHandler.RemoveAllEventListener();
                msgHandler.RemoveAllMsgListener();
                DebugKit.Warning("[Initialization]" + ex.Message);
                HasInit = false;
                return false;
            }
        }

        public bool BroadcastMsg(MsgBase msg)
        {
            if (broadcastfd == null) return false;
            try
            {
                byte[] msgBytes = MsgBase.EncodeToSendBytes(msg);
                //获取全部本地局域网
                List<string> EthernetList = NetKit.GetLocalIpAddress(AddressFamily.InterNetwork);
                foreach (var ip in EthernetList)
                {
                    //locateUsers.Add(new RemoteUser(UserInformationCache.Default.UserName, new IPEndPoint(IPAddress.Parse(ip), 0)));

                    string[] parts = ip.Split('.');
                    int netTypeNum = Convert.ToInt32(parts[0]);
                    if (netTypeNum != 127) //本地回环
                    {
                        if (netTypeNum < 127) parts[1] = (255).ToString();  // A类IP
                        if (netTypeNum < 192) parts[2] = (255).ToString();  // B类IP
                        if (netTypeNum < 224) parts[3] = (255).ToString();  // C类IP
                    }
                    StringBuilder stringBuilder = new StringBuilder(parts[0]);
                    for (int i = 1; i <= 3; ++i)
                    {
                        stringBuilder.Append(".");
                        stringBuilder.Append(parts[i]);
                    }
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(stringBuilder.ToString()), 20714);
                    broadcastfd.Send(msgBytes, msgBytes.Length, endpoint);
                    DebugKit.Log("BroadcastMsg has been sent to " + stringBuilder.ToString());
                }

                /*IPEndPoint endpoint = new IPEndPoint(IPAddress.Broadcast, 20714);
                boardcastfd.Send(msgBytes, msgBytes.Length, endpoint);
                DebugKit.Log("BoardcastMsg has been sent to " + endpoint.ToString());*/

                return true;
            }
            catch(Exception ex)
            {
                DebugKit.Warning("[BroadcastMsg]" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 单播一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="remoteUser"></param>
        /// <returns></returns>
        public bool UnicastMsg(MsgBase msg, RemoteUser remoteUser)
        {
            UdpClient sender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            IPEndPoint endPoint = new IPEndPoint(remoteUser.endPoint.Address, 20713);
            byte[] bytes = MsgBase.EncodeToSendBytes(msg);
            int result = sender.Send(bytes, bytes.Length, endPoint);
            return result > 0;
        }

        public void BroadcastReceive()
        {
            broadcastRecvfd = new UdpClient(new IPEndPoint(IPAddress.Any, 20714));
            DebugKit.Log("broadcastRecvfd has bound to 20714");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 20714);
            while(true)
            {
                byte[] buf = broadcastRecvfd.Receive(ref endPoint);

                //DebugKit.Log("Has recved a boardcastMsg");

                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);

                object[] args = new object[1];
                args[0] = endPoint;

                msgHandler.FireMsg(msg.protoName, msg, args);

            }
        }

        public void UnicastReceive()
        {
            UdpClient unicastRecvfd = new UdpClient(new IPEndPoint(IPAddress.Any, 20713));
            DebugKit.Log("unicastRecvfd has bound to 20713");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 20713);
            while(true)
            {
                byte[] buf = unicastRecvfd.Receive(ref endPoint);

                //DebugKit.Log("Has recved a unicastMsg");

                //如果已经连接上了远程设备却又收到了来自第三者的单播报文
                if (ConnectionManager.Instance.HasConnected && ConnectionManager.Instance.remoteUser.endPoint.Address.ToString() != endPoint.Address.ToString())
                {
                    DebugKit.Log("收到了一条第三者的单播消息:" + endPoint.Address);
                    continue;
                }

                MsgBase msg = MsgBase.DecodeFromRecvBytes(buf);

                object[] args = new object[1];
                args[0] = endPoint;

                msgHandler.FireMsg(msg.protoName, msg, args);

            }
        }

        #region 具体连接步骤
        public bool LANProbeRequest()
        {
            //remoteUsers.Clear();
            //msgHandler.FireEvent(NetEvent.LANRemoteUserListReady);

            //Thread.Sleep(10);

            LANProbeRequestMsg probeRequestMsg = new LANProbeRequestMsg();
            return BroadcastMsg(probeRequestMsg);
        }

        public bool LANConnectRequest(RemoteUser remoteUser)
        {
            LANConnectRequestMsg connectRequestMsg = new LANConnectRequestMsg();
            connectRequestMsg.userName = UserInformationCache.Default.UserName;
            try
            {
                UnicastMsg(connectRequestMsg, remoteUser);
                return true;
            }
            catch (Exception ex)
            {
                DebugKit.Warning("[LANConnectRequest]" + ex.Message);
                return false;
            }
        }

        public bool LANConnectReply(RemoteUser remoteUser, bool isAgree)
        {
            LANConnectReplyMsg connectReplyMsg = new LANConnectReplyMsg();
            connectReplyMsg.userName = UserInformationCache.Default.UserName;
            connectReplyMsg.permission = isAgree;
            try
            {
                UnicastMsg(connectReplyMsg, remoteUser);

                if(isAgree)
                {
                    ConnectionManager.Instance.remoteUser = remoteUser;
                    ConnectionManager.Instance.msgHandler.FireEvent(NetEvent.LANAcceptConnectRequest, null);
                }

                return true;
            }
            catch (Exception ex)
            {
                DebugKit.Warning("[LANConnectReply]" + ex.Message);
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
            //如果已经连结到目标，则不再回复
            if (ConnectionManager.Instance.HasConnected) return;

            //LANProbeRequestMsg probeRequestMsg = (LANProbeRequestMsg)msg;
            LANProbeReplyMsg probeReplyMsg = new LANProbeReplyMsg();
            probeReplyMsg.userName = UserInformationCache.Default.UserName;
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
            //if(locateUsers.Contains(newUser))
            if (probeReplyMsg.userName == UserInformationCache.Default.UserName) 
            {
                //找到自己了属于是
                DebugKit.Log("Find yourself:" + newUser.ToString());
                return;
            }

            if (!remoteUsers.Contains(newUser))
            {
                remoteUsers.Add(newUser);
                DebugKit.Log("New Remote User:" + newUser.ToString());
            }
            msgHandler.FireEvent(NetEvent.LANRemoteUserListReady, null);
        }

        /// <summary>
        /// 收到连接请求报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        //void OnRecvLANConnectRequestMsg(MsgBase msg, object[] args)
        //{
        //    //收到了连接请求时，需要给对方一个回复：是否同意连接
        //}

        /// <summary>
        /// 收到连接请求回复报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANConnectReplyMsg(MsgBase msg, object[] args)
        {
            LANConnectReplyMsg connectReplyMsg = (LANConnectReplyMsg)msg;
            //DebugKit.Log("Recv LANConnectReplyMsg with result:" + connectReplyMsg.permission.ToString());

            //如果对方同意，便需要开始连接
            if (connectReplyMsg.permission == true)
            {
                ConnectionManager.Instance.remoteUser = new RemoteUser(connectReplyMsg.userName, (IPEndPoint)args[0]);
                ConnectionManager.Instance.msgHandler.FireEvent(NetEvent.LANRecvAcceptConnectRequest, null);
            }
            else DebugKit.MessageBoxShow("对方拒绝了你的连接请求", "太逊了");
        }
        #endregion
    }
}

namespace LittleSheep.XamlWindows
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 收到连接请求报文时
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void OnRecvLANConnectRequestMsg(MsgBase msg, object[] args)
        {
            //收到了连接请求时，需要给对方一个回复：是否同意连接
            LANConnectRequestMsg connectRequestMsg = (LANConnectRequestMsg)msg;
            IPEndPoint endPoint = (IPEndPoint)args[0];
            RemoteUser applicant = new RemoteUser(connectRequestMsg.userName, endPoint);
            App.Current.Dispatcher.Invoke(new Action(delegate
            {
                XamlWindows.RecvLANConnectRequestMsgWindow window = new XamlWindows.RecvLANConnectRequestMsgWindow();
                window.Title = "收到了新的连接请求";
                window.Owner = this;
                window.HintMsg.Text = string.Format("收到了来自{0}的局域网连接控制请求，是否同意？", applicant.ToString());
                window.remoteUser = applicant;
                window.Show();
            }));
        }

        void OnLANRemoteUserListReady(string err)
        {
            RemoteUserList.Dispatcher.Invoke(new Action(delegate
            {
                RemoteUserList.ItemsSource = LANConnector.Instance.remoteUsers;
            }));
        }

    }
}