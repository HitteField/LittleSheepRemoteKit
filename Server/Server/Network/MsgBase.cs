using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace LittleSheep
{
    /// <summary>
    /// 协议类基类
    /// </summary>
    public class MsgBase
    {
        /// <summary>
        /// 协议名
        /// </summary>
        public string protoName = "";

        /// <summary>
        /// 将协议体编码为Json
        /// </summary>
        /// <param name="msgBase">待编码的协议对象</param>
        /// <returns></returns>
        public static byte[] Encode(MsgBase msgBase)
        {
            string s = JsonConvert.SerializeObject(msgBase);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// 从json解码出协议体
        /// </summary>
        /// <param name="protoName">协议名</param>
        /// <param name="bytes">字节缓冲区</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            string fullProtoName = "LittleSheep." + protoName;
            MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType(fullProtoName));

            return msgBase;
        }

        /// <summary>
        /// 编码协议名（2字节长度(0:商，1:余数)+字符串）
        /// </summary>
        /// <param name="msgBase">协议类</param>
        /// <returns></returns>
        public static byte[] EncodeName(MsgBase msgBase)
        {
            //协议名和协议名的长度
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            Int16 len = (Int16)nameBytes.Length;

            byte[] bytes = new byte[2 + len];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);

            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// 解码协议名（2字节长度+字符串），count返回协议名长度+2
        /// </summary>
        /// <param name="bytes">缓冲区</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">协议名信息的字节数（协议名长度+2）</param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            if (offset + 2 > bytes.Length) return "";                           //数组越界
            Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);      //后面是高八位，故左移八位

            if (len <= 0) return "";                                            //协议名长度为0
            if (offset + 2 + len > bytes.Length) return "";                     //数组越界

            count = len + 2;                                                    //协议名长度加2（2为上面求len的两个字节）
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }
    }
}
