using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace LittleSheep
{
    /// <summary>
    /// Э�������
    /// </summary>
    public class MsgBase
    {
        /// <summary>
        /// Э����
        /// </summary>
        public string protoName = "";

        /// <summary>
        /// ��Э�������ΪJson
        /// </summary>
        /// <param name="msgBase">�������Э�����</param>
        /// <returns></returns>
        public static byte[] Encode(MsgBase msgBase)
        {
            string s = JsonConvert.SerializeObject(msgBase);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// ��json�����Э����
        /// </summary>
        /// <param name="protoName">Э����</param>
        /// <param name="bytes">�ֽڻ�����</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="count">����</param>
        /// <returns></returns>
        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            string fullProtoName = "LittleSheep." + protoName;
            MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType(fullProtoName));

            return msgBase;
        }

        /// <summary>
        /// ����Э������2�ֽڳ���+�ַ�����
        /// </summary>
        /// <param name="msgBase">Э����</param>
        /// <returns></returns>
        public static byte[] EncodeName(MsgBase msgBase)
        {
            //Э������Э�����ĳ���
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            Int16 len = (Int16)nameBytes.Length;

            byte[] bytes = new byte[2 + len];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);

            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// ����Э������2�ֽڳ���+�ַ�������count����Э��������+2
        /// </summary>
        /// <param name="bytes">������</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="count">Э������Ϣ���ֽ�����Э��������+2��</param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            if (offset + 2 > bytes.Length) return "";                           //����Խ��
            Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);      //�����Ǹ߰�λ�������ư�λ

            if (len <= 0) return "";                                            //Э��������Ϊ0
            if (offset + 2 + len > bytes.Length) return "";                     //����Խ��

            count = len + 2;                                                    //Э�������ȼ�2��2Ϊ������len�������ֽڣ�
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }

        /// <summary>
        /// ����Ϣ��ֱ�ӷ�װ�ɿ��Է��͵�JSON����
        /// </summary>
        /// <param name="msg">��Ϣ��</param>
        /// <returns>������ɵı���</returns>
        public static byte[] EncodeToSendBytes(MsgBase msg)
        {
            //���ݱ���
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);

            int len = nameBytes.Length + bodyBytes.Length;          //�ܳ���
            byte[] sendBytes = new byte[2 + len];                   //ǰ�滹��2�ֽڵ���Ϣ�ܳ���

            //��װ
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);                           //����Э��������
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);        //����Э���岿��
            return sendBytes;
        }

        /// <summary>
        /// ��һ�������ı����н������Ϣ��
        /// </summary>
        /// <param name="bytes">�����ı���</param>
        /// <returns>���������Ϣ��</returns>
        public static MsgBase DecodeFromRecvBytes(byte[] bytes)
        {

            int readIdx = 0;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (bytes.Length < bodyLength + 2) return null;       //����Ϣû������ȫ

            readIdx += 2;
            //����Э����
            int nameCount;
            string msgProtoName = MsgBase.DecodeName(bytes, readIdx, out nameCount);

            if (msgProtoName == "")
            {
                DebugKit.Warning("OnReceiveData MsgBase.DecodeName fail");
                return null;
            }

            readIdx += nameCount;
            //����Э����
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(msgProtoName, bytes, readIdx, bodyCount);
            return msgBase;
        }
    }
}
