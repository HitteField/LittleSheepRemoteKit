using System.Collections;
using System.Collections.Generic;

namespace LittleSheep
{
    /// <summary>
    /// ������Ping
    /// </summary>
    public class MsgPing : MsgBase
    {
        public MsgPing() { protoName = "MsgPing"; }

    }

    /// <summary>
    /// ������Pong
    /// </summary>
    public class MsgPong : MsgBase
    {
        public MsgPong() { protoName = "MsgPong"; }
    }
}