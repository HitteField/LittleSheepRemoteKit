using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    public static class NetKit
    {
        /// <summary>
        /// 获取本机在所有的本地局域网中的IP地址
        /// </summary>
        /// <param name="netType">网络类型，v4或者v6</param>
        /// <returns>IP地址的集合</returns>
        public static List<string> GetLocalIpAddress(AddressFamily netType)
        {
            string hostName = Dns.GetHostName();                    //获取主机名称
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址

            List<string> IPList = new List<string>();

            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].AddressFamily == netType)
                {
                    IPList.Add(addresses[i].ToString());
                }
            }

            return IPList;
        }
    }
}
