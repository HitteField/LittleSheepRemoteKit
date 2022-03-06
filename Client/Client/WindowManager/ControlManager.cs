using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep.Client.WindowManager
{
    class ControlManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly ControlManager instance = new ControlManager();
        }
        private ControlManager() { }
        public static ControlManager Instance { get { return Nested.instance; } }
        #endregion

        public 
    }
}
