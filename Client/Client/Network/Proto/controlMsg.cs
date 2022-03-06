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
        //TODO 完善控制信号消息的消息内容
    }
}
