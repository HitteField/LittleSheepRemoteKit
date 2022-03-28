using NetFwTypeLib;
using System;
using System.Net.Sockets;

namespace LittleSheep
{
    /// <summary>
    /// 防火墙相关操作
    /// </summary>
    public static class FirewallOperation
    {
        /// <summary>
        /// 网络协议类型
        /// </summary>
        public enum ProtocolType
        {
            TCP = 6,
            UDP = 17,
            ANY = 256
        }

        [Obsolete]
        /// <summary>
        /// 添加防火墙例外端口
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="port">端口</param>
        /// <param name="protocol">协议</param>
        public static void NetFwAddPorts(string name, int port, ProtocolType protocol)
        {
            //创建firewall管理类的实例
            INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
            INetFwOpenPort objPort = (INetFwOpenPort)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwOpenPort"));

            objPort.Name = name;
            objPort.Port = port;

            if (protocol == ProtocolType.TCP)
            {
                objPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            }
            else if (protocol == ProtocolType.UDP)
            {
                objPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
            }

            objPort.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
            objPort.Enabled = true;

            bool exist = false;

            //加入到防火墙的管理策略
            foreach (INetFwOpenPort mPort in netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts)
            {
                if (objPort == mPort)
                {
                    exist = true;
                    break;
                }
            }
            if (!exist) netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Add(objPort);
        }

        [Obsolete]
        /// <summary>
        /// 删除防火墙例外端口
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="protocol">协议</param>
        public static void NetFwDelApps(int port, ProtocolType protocol)
        {
            INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));

            if (protocol == ProtocolType.TCP)
            {
                netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Remove(port, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
            }
            else if (protocol == ProtocolType.UDP)
            {
                netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Remove(port, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);
            }

        }

        
        /// <summary>
        /// 创建防火墙规则
        /// </summary>
        /// <param name="type">网络协议类型</param>
        /// <param name="ruleName">规则名</param>
        /// <param name="appPath">应用程序绝对路径</param>
        /// <param name="localAddresses">本地地址</param>
        /// <param name="localPorts">本地端口</param>
        /// <param name="remoteAddresses">远端地址</param>
        /// <param name="remotePorts">远端端口</param>
        /// <returns></returns>
        public static bool CreateRule(ProtocolType type, string ruleName, string appPath = null, string localAddresses = null, string localPorts = null, string remoteAddresses = null, string remotePorts = null)
        {
            //创建防火墙策略类的实例
            INetFwPolicy2 policy2 = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            //检查是否有同名规则
            foreach (INetFwRule item in policy2.Rules)
            {
                if (item.Name == ruleName)
                {
                    return true;
                }
            }
            //创建防火墙规则类的实例: 有关该接口的详细介绍：https://docs.microsoft.com/zh-cn/windows/win32/api/netfw/nn-netfw-inetfwrule
            INetFwRule rule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwRule"));
            //为规则添加名称
            rule.Name = ruleName;
            //为规则添加描述
            rule.Description = "LittleSheep端口需求";
            //选择入站规则还是出站规则，IN为入站，OUT为出站
            rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            //为规则添加协议类型
            rule.Protocol = (int)type;
            //为规则添加应用程序（注意这里是应用程序的绝对路径名）
            if(!string.IsNullOrEmpty(appPath))
            {
                rule.ApplicationName = appPath;
            }
            //为规则添加本地IP地址    
            if (!string.IsNullOrEmpty(localAddresses))
            {
                rule.LocalAddresses = localAddresses;
            }

            //为规则添加本地端口
            if(!string.IsNullOrEmpty(localPorts))
            {
                rule.LocalPorts = localPorts; // "1-29999, 30003-33332, 33334-55554, 55556-60004, 60008-65535";
            }

            //为规则添加远程IP地址
            //if (!string.IsNullOrEmpty(remoteAddresses))
            //{
            //    rule.RemoteAddresses = remoteAddresses;
            //}
            //为规则添加远程端口
            //rule.RemotePorts = remotePorts.ToString();

            //设置规则是阻止还是允许（ALLOW=允许，BLOCK=阻止）
            rule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            //分组 名
            //rule.Grouping = "小绵羊";
            //配置文件：公用网络，专用网络，域
            rule.InterfaceTypes = "All";
            //是否启用规则
            rule.Enabled = true;
            try
            {
                //添加规则到防火墙策略
                policy2.Rules.Add(rule);
            }
            catch (Exception e)
            {
                string error = $"防火墙添加规则出错：{ruleName} {e.Message}";
                DebugKit.Warning(error);
                throw new Exception(error);
            }
            return true;
        }

        public static bool DeleteRule(string ruleName)
        {
            INetFwPolicy2 policy2 = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            try
            {
                //根据规则名称移除规则
                policy2.Rules.Remove(ruleName);
            }
            catch (Exception e)
            {
                string error = $"防火墙删除规则出错：{ruleName} {e.Message}";
                DebugKit.Warning(error);
                throw new Exception(error);
            }
            return true;
        }
    }
}
