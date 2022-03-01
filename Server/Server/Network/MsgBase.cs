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
        /// ����Э������2�ֽڳ���(0:�̣�1:����)+�ַ�����
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
    }
}
