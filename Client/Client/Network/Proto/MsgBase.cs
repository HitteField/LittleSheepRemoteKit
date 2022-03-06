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
        /// 编码协议名（2字节长度+字符串）
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

        /// <summary>
        /// 将消息体直接封装成可以发送的JSON报文
        /// </summary>
        /// <param name="msg">消息体</param>
        /// <returns>编码完成的报文</returns>
        public static byte[] EncodeToSendBytes(MsgBase msg)
        {
            //数据编码
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);

            int len = nameBytes.Length + bodyBytes.Length;          //总长度
            byte[] sendBytes = new byte[2 + len];                   //前面还有2字节的消息总长度

            //组装
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);                           //拷贝协议名部分
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);        //拷贝协议体部分
            return sendBytes;
        }

        /// <summary>
        /// 从一个完整的报文中解码出消息体
        /// </summary>
        /// <param name="bytes">完整的报文</param>
        /// <returns>解码出的消息体</returns>
        public static MsgBase DecodeFromRecvBytes(byte[] bytes)
        {

            int readIdx = 0;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (bytes.Length < bodyLength + 2) return null;       //此消息没接收完全

            readIdx += 2;
            //解析协议名
            int nameCount;
            string msgProtoName = MsgBase.DecodeName(bytes, readIdx, out nameCount);

            if (msgProtoName == "")
            {
                DebugKit.Warning("OnReceiveData MsgBase.DecodeName fail");
                return null;
            }

            readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(msgProtoName, bytes, readIdx, bodyCount);
            return msgBase;
        }
    }
}
