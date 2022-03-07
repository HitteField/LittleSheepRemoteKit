using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    
    /// <summary>
    /// 文件传输请求报文
    /// </summary>
    public class FileSendRequestMsg:MsgBase
    {
        public FileSendRequestMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public string fileName = "";
        public string fileLength = "";
        public long fileTrueLength = 0;
    }

    /// <summary>
    /// 文件传输请求回复报文
    /// </summary>
    public class FileSendReplyMsg:MsgBase
    {
        public FileSendReplyMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public bool permission = false;
    }

    public class ShutConnectionMsg:MsgBase
    {
        public ShutConnectionMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
    }
}
