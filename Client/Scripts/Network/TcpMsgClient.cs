using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LittleSheep
{
    public class TcpMsgClient
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        TcpMsgClient()
        {

        }

        public TcpClient tcpClient;
        public ByteArray byteArray;
    }
}
