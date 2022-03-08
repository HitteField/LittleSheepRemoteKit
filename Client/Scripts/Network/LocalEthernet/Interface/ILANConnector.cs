using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    /// <summary>
    /// 局域网连接模块
    /// </summary>
    interface ILANConnector
    {
        /// <summary>
        /// 接收广播套接字
        /// </summary>
        UdpClient BroadcastRecvfd { get; }
        /// <summary>
        /// 广播套接字
        /// </summary>
        UdpClient Broadcastfd { get; }


        /// <summary>
        /// 初始化LAN连接模块
        /// </summary>
        /// <returns>初始化结果</returns>
        bool Init();
        /// <summary>
        /// 广播一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>广播成功与否</returns>
        bool BroadcastMsg(MsgBase msg);
        /// <summary>
        /// 单播一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="remoteUser"></param>
        /// <returns>单播成功与否</returns>
        bool UnicastMsg(MsgBase msg, RemoteUser remoteUser);
        /// <summary>
        /// 收到广播消息
        /// </summary>
        /// <param name="ar"></param>
        void BroadcastReceive();
        /// <summary>
        /// 收到单播消息
        /// </summary>
        void UnicastReceive();

        #region 具体连接步骤
        /// <summary>
        /// 发送广播探查报文
        /// </summary>
        /// <returns></returns>
        bool LANProbeRequest();
        /// <summary>
        /// 发送单播连接请求报文
        /// </summary>
        /// <returns></returns>
        bool LANConnectRequest(RemoteUser remoteUser);
        /// <summary>
        /// 发送单播连接请求回复报文
        /// </summary>
        /// <param name="isAgree">是否同意</param>
        /// <returns></returns>
        bool LANConnectReply(RemoteUser remoteUser, bool isAgree);
        #endregion
    }
}
