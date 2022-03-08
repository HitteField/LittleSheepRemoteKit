using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        ///-----------------------------------------------
        /// 考虑到CIDR的存在而不得不端出这么个乱七八糟的玩意
        ///-----------------------------------------------
        /// <summary>
        /// 获取本机在所有的合法局域网中的所有网络信息
        /// </summary>
        /// <returns>{IP地址,子网掩码,广播地址}的元组的列表</returns>
        public static List<Tuple<string,string,string>> GetLocalNetworkInf()
        {
            List<Tuple<string, string, string>> result = new List<Tuple<string, string, string>>();

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface bendi in interfaces)
            {
                IPInterfaceProperties ip = bendi.GetIPProperties();
                //获取Ip 掩码
                for (int i = 0; i < ip.UnicastAddresses.Count; i++)
                {
                    if (ip.UnicastAddresses[i].Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ip.UnicastAddresses[i].Address != null)
                        {
                            string[] ipParts = ip.UnicastAddresses[i].Address.ToString().Split('.');
                            if (ipParts[0] == "169" && ipParts[1] == "254") continue;           // DHCP保留地址
                            if (ipParts[0] == "127") continue;                                  // 本地回环
                            uint ipInt = (((((Convert.ToUInt32(ipParts[0]) << 8) + Convert.ToUInt32(ipParts[1])) << 8) + Convert.ToUInt32(ipParts[2])) << 8) + Convert.ToUInt32(ipParts[3]);
                            string[] maskParts = ip.UnicastAddresses[i].IPv4Mask.ToString().Split('.');
                            uint maskInt = (((((Convert.ToUInt32(maskParts[0]) << 8) + Convert.ToUInt32(maskParts[1])) << 8) + Convert.ToUInt32(maskParts[2])) << 8) + Convert.ToUInt32(maskParts[3]);

                            uint broadcastIpInt = ipInt | (~maskInt);
                            string[] broadcastIpParts = new string[4];
                            broadcastIpParts[3] = (broadcastIpInt & 0b11111111).ToString();
                            broadcastIpParts[2] = ((broadcastIpInt >> 8) & 0b11111111).ToString();
                            broadcastIpParts[1] = ((broadcastIpInt >> 16) & 0b11111111).ToString();
                            broadcastIpParts[0] = ((broadcastIpInt >> 24) & 0b11111111).ToString();
                            string broadcastIp = string.Join(".", broadcastIpParts);

                            result.Add(new Tuple<string, string, string>(ip.UnicastAddresses[i].Address.ToString(), ip.UnicastAddresses[i].IPv4Mask.ToString(), broadcastIp));
                        }
                    }
                }
            }

            return result;
        }
    }
}
