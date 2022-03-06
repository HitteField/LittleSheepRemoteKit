using System.Collections;
using System.Collections.Generic;

namespace LittleSheep
{
    /// <summary>
    /// 心跳包Ping
    /// </summary>
    public class MsgPing : MsgBase
    {
        public MsgPing() { protoName = "MsgPing"; }

    }

    /// <summary>
    /// 心跳包Pong
    /// </summary>
    public class MsgPong : MsgBase
    {
        public MsgPong() { protoName = "MsgPong"; }
    }
}