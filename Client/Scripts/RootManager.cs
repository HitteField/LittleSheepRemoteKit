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
            FirewallOperation.CreateRule(FirewallOperation.ProtocolType.UDP, "LittleSheepBoardcast", localPorts: 20714);
            FirewallOperation.CreateRule(FirewallOperation.ProtocolType.UDP, "LittleSheepUnicast", localPorts: 20713);
            FirewallOperation.CreateRule(FirewallOperation.ProtocolType.TCP, "LittleSheepFileModule", localPorts: 20718);

            ConnectionManager.Instance.Init();
        }

        public void GlobalDestruct()
        {
            //FirewallOperation.NetFwDelApps(20713, System.Net.Sockets.ProtocolType.Udp);
            //FirewallOperation.NetFwDelApps(20714, System.Net.Sockets.ProtocolType.Udp);

            FirewallOperation.DeleteRule("LittleSheepBoardcast");
            FirewallOperation.DeleteRule("LittleSheepUnicast");
            FirewallOperation.DeleteRule("LittleSheepFileModule");

            UserInformationCache.Default.Save();
        }
    }
}
