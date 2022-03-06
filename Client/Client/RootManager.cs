using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;

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
            FirewallOperation.NetFwAddPorts("LittleSheepBoardcast", 20713, System.Net.Sockets.ProtocolType.Udp);
            FirewallOperation.NetFwAddPorts("LittleSheepUnicast", 20714, System.Net.Sockets.ProtocolType.Udp);
        }

        public void GlobalDestruct()
        {
            FirewallOperation.NetFwDelApps(20713, System.Net.Sockets.ProtocolType.Udp);
            FirewallOperation.NetFwDelApps(20714, System.Net.Sockets.ProtocolType.Udp);

            UserInformationCache.Default.Save();
        }
    }
}
