using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    class RootManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly RootManager instance = new RootManager();
        }
        private RootManager() {}
        public static RootManager Instance { get { return Nested.instance; } }
        #endregion

        public void GlobalInit()
        {
            //FirewallOperation.NetFwAddPorts("LittleSheepBoardcast", 20713, System.Net.Sockets.ProtocolType.Udp);
            //FirewallOperation.NetFwAddPorts("LittleSheepUnicast", 20714, System.Net.Sockets.ProtocolType.Udp);
            FirewallOperation.CreateRule(FirewallOperation.ProtocolType.UDP, "LittleSheepUDP", localPorts: "20713,20714,20716,20717");
            FirewallOperation.CreateRule(FirewallOperation.ProtocolType.TCP, "LittleSheepTCP", localPorts: "20712,20715,20718");

            ConnectionManager.Instance.Init();
            LANConnector.Instance.Init();
        }

        public void GlobalDestruct()
        {
            //FirewallOperation.NetFwDelApps(20713, System.Net.Sockets.ProtocolType.Udp);
            //FirewallOperation.NetFwDelApps(20714, System.Net.Sockets.ProtocolType.Udp);

            FirewallOperation.DeleteRule("LittleSheepUDP");
            FirewallOperation.DeleteRule("LittleSheepTCP");
            ConnectionManager.Instance.ShutConnection();
            UserInformationCache.Default.Save();
        }
    }
}
