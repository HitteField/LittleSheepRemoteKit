using System.Collections;
using System.Collections.Generic;
using System;

namespace LittleSheep
{
    public class ByteArray
    {
        #region ����
        //Ĭ�ϴ�С
        public const int DEFAULT_SIZE = 4096;
        #endregion

        #region ����
        //��ʼ��С
        private int initSize = 0;
        //������
        public byte[] bytes;
        //��ȡλ���±�
        public int readIdx = 0;
        //д��λ���±�
        public int writeIdx = 0;
        //����
        private int capacity = 0;
        #endregion

        #region ����
        //���ݳ���
        public int Length => writeIdx - readIdx;
        //ʣ��ռ�
        public int Remain => capacity - writeIdx;
        #endregion

        #region ����
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="defaultBytes">��ʼ�����ֽ�����</param>
        public ByteArray(byte[] defaultBytes)
        {
            bytes = defaultBytes;
            capacity = initSize = defaultBytes.Length;
            readIdx = 0;
            writeIdx = defaultBytes.Length;
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="size">��ʼ����������С</param>
        public ByteArray(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIdx = writeIdx = 0;
        }


        //����ToString����
        public override string ToString()
        {
            return BitConverter.ToString(bytes, readIdx, Length);
        }
        //������Ϣ
        public string Debug()
        {
            return string.Format("readIdx({0}), writeIdx({1}), bytes({2})",
                readIdx,
                writeIdx,
                BitConverter.ToString(bytes, 0, bytes.Length));
        }

        /// <summary>
        /// д�����ݣ���bs��offset��ʼ��count���ֽ�д�뻺����
        /// </summary>
        /// <param name="bs">��д���ֽ�����</param>
        /// <param name="offset">д�����ʼλ��</param>
        /// <param name="count">��ʼλ�ÿ�ʼ��Ҫд����ֽ���</param>
        /// <returns>�ɹ�д����ֽ���</returns>
        public int Write(byte[] bs, int offset, int count)
        {
            if (Remain < count)                    //���ʣ�����������Դ��´�д�������
            {
                ReSize(Length + count);
            }
            Array.Copy(bs, offset, bytes, writeIdx, count);
            writeIdx += count;
            return count;                       //���سɹ�д����ֽ���
        }

        /// <summary>
        /// �������ݣ��ѻ�������count���ֽڶ���bs��offset��
        /// </summary>
        /// <param name="bs">��д���ֽ�����</param>
        /// <param name="offset">д�����ʼλ��</param>
        /// <param name="count">����ȡ���ֽ���</param>
        /// <returns>�ɹ���ȡ���ֽ���</returns>
        public int Read(byte[] bs, int offset, int count)
        {
            count = Math.Min(count, Length);    //����ȡLength���ֽ�
            Array.Copy(bytes, readIdx, bs, offset, count);
            readIdx += count;
            CheckAndMoveBytes();
            return count;
        }

        /// <summary>
        /// �ӻ�������readIdxOffset����ʼ��ȡ2�ֽڵ�Int16ֵ
        /// </summary>
        /// <param name="readIdxOffset">�ڻ������е�ƫ����</param>
        /// <returns>��ȡ��Int16��ֵ</returns>
        public Int16 ReadInt16(int readIdxOffset = 0)
        {
            if (Length < 2) return 0;
            Int16 ret = (Int16)((bytes[readIdxOffset + 1] << 8) | bytes[readIdxOffset]);
            readIdx += 2;
            CheckAndMoveBytes();
            return ret;
        }

        /// <summary>
        /// �ӻ�������readIdxOffset����ʼ��ȡ4�ֽڵ�Int16ֵ
        /// </summary>
        /// <param name="readIdxOffset">�ڻ������е�ƫ����</param>
        /// <returns>��ȡ��Int32��ֵ</returns>
        public Int32 ReadInt32(int readIdxOffset = 0)
        {
            if (Length < 4) return 0;
            Int32 ret = (Int32)((bytes[readIdxOffset + 3] << 24) |
                (bytes[readIdxOffset + 2] << 16) |
                (bytes[readIdxOffset + 1] << 8) |
                bytes[readIdxOffset]);
            readIdx += 4;
            CheckAndMoveBytes();
            return ret;
        }


        /// <summary>
        /// �ڻ��������������ʱ���������óߴ�
        /// </summary>
        /// <param name="size">�µĴ�С</param>
        public void ReSize(int size)
        {
            if (size < Length) return;          //���ܱȵ�ǰ�������ڵ����ݴ�СҪС
            if (size < initSize) return;        //��Ӧ�ȳ�ʼ��ʱ�Ĵ�СҪС

            int n = 1;
            while (n < size) n <<= 1;           //������2���ݴΣ��ҵ����ڵ���size����С��2���ݴα�����n��
            capacity = n;

            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIdx, newBytes, 0, Length);    //��ԭ�����readIdx����Length��Ԫ�ؿ�����newBytes��ͷ��
            bytes = newBytes;
            writeIdx = Length;
            readIdx = 0;
        }

        /// <summary>
        /// ��鲢�ƶ����ݵ����鿪ͷ
        /// </summary>
        public void CheckAndMoveBytes()
        {
            if (Length < 8) MoveBytes();
        }

        /// <summary>
        /// �������ƶ������鿪ͷ
        /// </summary>
        public void MoveBytes()
        {
            Array.Copy(bytes, readIdx, bytes, 0, Length);
            writeIdx = Length;
            readIdx = 0;
        }

        #endregion
    }

}
