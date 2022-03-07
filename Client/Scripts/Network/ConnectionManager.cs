using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    /// <summary>
    /// 总网络连接管理器
    /// </summary>
    class ConnectionManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly ConnectionManager instance = new ConnectionManager();
        }
        private ConnectionManager() { }
        public static ConnectionManager Instance { get { return Nested.instance; } }
        #endregion

        /// <summary>
        /// 局域网连接管理器
        /// </summary>
        LANConnector lanConnectorInstance = LANConnector.Instance;

    }
}
