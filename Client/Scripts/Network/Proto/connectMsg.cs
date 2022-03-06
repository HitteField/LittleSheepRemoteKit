using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    /// <summary>
    /// sid信息
    /// </summary>
    public class SidMsg : MsgBase
    {
        public SidMsg() { protoName = "SidMsg"; }
        public int sid = 0;
    }
}
