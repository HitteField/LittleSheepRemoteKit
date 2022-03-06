using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSheep
{
    public partial class EventHandler
    {
        public static void OnDisconnect(ClientState cs)
        {
            DebugKit.Log("Socket close:" + cs.remoteName);
        }

        public static void OnTimer()
        {
            CheckPing();
        }

        /// <summary>
        /// 检查是否有心跳包超时的客户端
        /// </summary>
        public static void CheckPing()
        {
            long timeNow = TimeKit.GetTimeStamp();
            //遍历所有用户
            foreach (ClientState s in NetManager.clients.Values)
            {
                if (timeNow - s.lastPingTime > NetManager.pingInterval * 3)
                {
                    NetManager.Close(s);
                    //因为删除后foreach不稳定，故直接return，下秒再处理其他的
                    return;
                }
            }
        }
    }
}