﻿using System;
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
        UdpClient BoardcastRecvfd { get; }
        /// <summary>
        /// 广播套接字
        /// </summary>
        UdpClient Boardcastfd { get; }


        /// <summary>
        /// 初始化LAN连接模块
        /// </summary>
        /// <returns>初始化结果</returns>
        bool Initialization();
        /// <summary>
        /// 初始化LAN广播模块
        /// </summary>
        /// <returns></returns>
        bool InitializationBoardcast();
        /// <summary>
        /// 广播一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>广播成功与否</returns>
        bool BoardcastMsg(MsgBase msg);
        /// <summary>
        /// 收到广播消息
        /// </summary>
        /// <param name="ar"></param>
        void BoardcastReceive();
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
        bool LANConnectRequest();
        #endregion
    }
}