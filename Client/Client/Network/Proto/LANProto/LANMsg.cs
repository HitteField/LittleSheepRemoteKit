using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    /// <summary>
    /// 局域网探查报文 广播
    /// </summary>
    class LANProbeRequestMsg : MsgBase
    {
        public LANProbeRequestMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public string ip = "";
    }

    /// <summary>
    /// 局域网探查回复报文 单播
    /// </summary>
    class LANProbeReplyMsg : MsgBase
    {
        public LANProbeReplyMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public string userName;
    }

    /// <summary>
    /// 局域网连接请求报文 单播
    /// </summary>
    class LANConnectRequestMsg : MsgBase
    {
        public LANConnectRequestMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public string userName;
    }

    /// <summary>
    /// 局域网连接请求回复报文 单播
    /// </summary>
    class LANConnectReplyMsg : MsgBase
    {
        public LANConnectReplyMsg() { protoName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        public bool permission = true;
    }
}
