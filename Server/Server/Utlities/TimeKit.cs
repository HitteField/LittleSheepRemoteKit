using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Net;

namespace LittleSheep
{
    public static class TimeKit
    { 
        /// <summary>
        /// ��ȡ��ǰʱ���
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}