using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    /// <summary>
    /// 网络事件枚举
    /// </summary>
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
        LANRemoteUserListReady = 4
    }
    /// <summary>
    /// 网络模块事件注册管理
    /// </summary>
    class NetMsgHandler
    {
        
        /// <summary>
        /// 事件的委托类型，无返回值的传参为string的方法
        /// </summary>
        /// <param name="err">参数</param>
        public delegate void EventListener(string err);
        /// <summary>
        /// 事件监听列表字典，<事件，事件的监听者>键值对
        /// </summary>
        private Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        /// <summary>
        /// 消息的委托类型，无返回值的传参为MsgBase的方法
        /// </summary>
        /// <param name="msgBase"></param>
        public delegate void MsgListener(MsgBase msgBase, object[] args);
        /// <summary>
        /// 消息监听列表字典，<消息名，消息的监听者>键值对
        /// </summary>
        private Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

        /// <summary>
        /// 添加事件监听（注册事件）
        /// </summary>
        /// <param name="netEvent">监听的网络事件</param>
        /// <param name="listener">监听者（注册此事件的方法）</param>
        public void AddEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                //如果已存在注册此事件的方法，就加上去（添加）
                eventListeners[netEvent] += listener;
            }
            else
            {
                //否则注册此事件（新建）
                eventListeners[netEvent] = listener;
            }
        }

        /// <summary>
        /// 删除事件监听（取消注册事件）
        /// </summary>
        /// <param name="netEvent">监听的网络事件</param>
        /// <param name="listener">要删除的监听者</param>
        public void RemoveEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
                if (eventListeners[netEvent] == null)
                {
                    //如果此事件已无监听者，就把此事件从监听列表中删掉
                    eventListeners.Remove(netEvent);
                }
            }
        }

        /// <summary>
        /// 删除所有事件监听
        /// </summary>
        public void RemoveAllEventListener()
        {
            eventListeners.Clear();
        }

        /// <summary>
        /// 是否存在事件监听
        /// </summary>
        /// <param name="netEvent">监听的网络事件</param>
        /// <returns></returns>
        public bool HasEventListener(NetEvent netEvent)
        {
            return eventListeners.ContainsKey(netEvent);
        }

        /// <summary>
        /// 添加消息监听（注册消息）
        /// </summary>
        /// <param name="msgName">监听的消息</param>
        /// <param name="listener">监听者（注册此消息的方法）</param>
        public void AddMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                //如果已存在注册此消息的方法，就加上去（添加）
                msgListeners[msgName] += listener;
            }
            else
            {
                //否则注册此消息（新建）
                msgListeners[msgName] = listener;
            }
        }

        /// <summary>
        /// 删除消息监听（取消注册消息）
        /// </summary>
        /// <param name="msgName">监听的消息</param>
        /// <param name="listener">要删除的监听者</param>
        public void RemoveMsgListener(string msgName, MsgListener listener)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
                if (msgListeners[msgName] == null)
                {
                    //如果此消息已无监听者，就把此消息从监听列表中删掉
                    msgListeners.Remove(msgName);
                }
            }
        }

        /// <summary>
        /// 删除所有消息监听
        /// </summary>
        public void RemoveAllMsgListener()
        {
            msgListeners.Clear();
        }

        /// <summary>
        /// 是否存在事件监听
        /// </summary>
        /// <param name="msgName">监听的消息</param>
        /// <returns></returns>
        public bool HasMsgListener(string msgName)
        {
            return msgListeners.ContainsKey(msgName);
        }

        /// <summary>
        /// 分发事件，在某事件发生的时候通知所有的监听者
        /// </summary>
        /// <param name="netEvent">发生的网络事件</param>
        /// <param name="err">给监听者们发送的传参</param>
        public void FireEvent(NetEvent netEvent, string err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        /// <summary>
        /// 分发消息，在某消息到来并被处理后通知所有监听者
        /// </summary>
        /// <param name="msgName">消息</param>
        /// <param name="msgBase">消息类</param>
        public void FireMsg(string msgName, MsgBase msgBase, object[] args)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase, args);
            }
        }

        /// <summary>
        /// 分发消息，在某消息到来并被处理后通知所有监听者
        /// </summary>
        /// <param name="msgName">消息</param>
        /// <param name="msgBase">消息类</param>
        public void FireMsg(string msgName, MsgBase msgBase)
        {
            if (msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase, null);
            }
        }
    }
}
