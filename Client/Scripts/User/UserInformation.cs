using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    class UserInformation
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly UserInformation instance = new UserInformation();
        }
        private UserInformation() {  }
        public static UserInformation Instance { get { return Nested.instance; } }
        #endregion
    }
}
