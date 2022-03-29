using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    public class controlMsg : MsgBase
    {
        public controlMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public byte[] inf = new byte[11];
    }
}
