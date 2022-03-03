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
        /// 目标设备套接字
        /// </summary>
        Socket Clientfd
        {
            get;
        }
        
        /// <summary>
        /// 初始化LAN连接模块
        /// </summary>
        /// <returns>初始化结果</returns>
        bool Initialization();
        /// <summary>
        /// 广播一条消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>广播成功与否</returns>
        bool BoradcastMsg(MsgBase msg);
        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        /// <returns>发送成功与否</returns>
        bool SendMsg(Socket socket, MsgBase msg);
        /// <summary>
        /// 开始接收报文
        /// </summary>
        void StartReceive(int port);
        /// <summary>
        /// 关闭接收报文
        /// </summary>
        void HaltReceive();
    }
}
