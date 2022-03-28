using System.Collections;
using System.Collections.Generic;
using System;

namespace LittleSheep
{
    public class ByteArray
    {
        #region 常量
        //默认大小
        public const int DEFAULT_SIZE = 4096;
        #endregion

        #region 变量
        //初始大小
        private int initSize = 0;
        //缓冲区
        public byte[] bytes;
        //读取位置下标
        public int readIdx = 0;
        //写入位置下标
        public int writeIdx = 0;
        //容量
        private int capacity = 0;
        #endregion

        #region 属性
        //数据长度
        public int Length => writeIdx - readIdx;
        //剩余空间
        public int Remain => capacity - writeIdx;
        #endregion

        #region 方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defaultBytes">初始化的字节数组</param>
        public ByteArray(byte[] defaultBytes)
        {
            bytes = defaultBytes;
            capacity = initSize = defaultBytes.Length;
            readIdx = 0;
            writeIdx = defaultBytes.Length;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size">初始化的容量大小</param>
        public ByteArray(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIdx = writeIdx = 0;
        }


        //重载ToString函数
        public override string ToString()
        {
            return BitConverter.ToString(bytes, readIdx, Length);
        }
        //调试信息
        public string Debug()
        {
            return string.Format("readIdx({0}), writeIdx({1}), bytes({2})",
                readIdx,
                writeIdx,
                BitConverter.ToString(bytes, 0, bytes.Length));
        }

        /// <summary>
        /// 写入数据，把bs从offset开始的count个字节写入缓冲区
        /// </summary>
        /// <param name="bs">待写入字节数组</param>
        /// <param name="offset">写入的起始位置</param>
        /// <param name="count">起始位置开始想要写入的字节数</param>
        /// <returns>成功写入的字节数</returns>
        public int Write(byte[] bs, int offset, int count)
        {
            if (Remain < count)                    //如果剩余容量不足以存下待写入的数据
            {
                ReSize(Length + count);
            }
            Array.Copy(bs, offset, bytes, writeIdx, count);
            writeIdx += count;
            return count;                       //返回成功写入的字节数
        }

        /// <summary>
        /// 读入数据，把缓冲区中count个字节读到bs的offset处
        /// </summary>
        /// <param name="bs">待写入字节数组</param>
        /// <param name="offset">写入的起始位置</param>
        /// <param name="count">欲读取的字节数</param>
        /// <returns>成功读取的字节数</returns>
        public int Read(byte[] bs, int offset, int count)
        {
            count = Math.Min(count, Length);    //最多读取Length个字节
            Array.Copy(bytes, readIdx, bs, offset, count);
            readIdx += count;
            CheckAndMoveBytes();
            return count;
        }

        /// <summary>
        /// 从缓冲区的readIdxOffset处开始读取2字节的Int16值
        /// </summary>
        /// <param name="readIdxOffset">在缓冲区中的偏移量</param>
        /// <returns>读取的Int16的值</returns>
        public Int16 ReadInt16(int readIdxOffset = 0)
        {
            if (Length < 2) return 0;
            Int16 ret = (Int16)((bytes[readIdxOffset + 1] << 8) | bytes[readIdxOffset]);
            readIdx += 2;
            CheckAndMoveBytes();
            return ret;
        }

        /// <summary>
        /// 从缓冲区的readIdxOffset处开始读取4字节的Int16值
        /// </summary>
        /// <param name="readIdxOffset">在缓冲区中的偏移量</param>
        /// <returns>读取的Int32的值</returns>
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
        /// 在缓冲区容量不足的时候重新设置尺寸
        /// </summary>
        /// <param name="size">新的大小</param>
        public void ReSize(int size)
        {
            if (size < Length) return;          //不能比当前缓冲区内的内容大小要小
            if (size < initSize) return;        //不应比初始化时的大小要小

            int n = 1;
            while (n < size) n <<= 1;           //必须是2的幂次，找到大于等于size的最小的2的幂次保存在n中
            capacity = n;

            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIdx, newBytes, 0, Length);    //把原数组从readIdx处的Length个元素拷贝到newBytes的头部
            bytes = newBytes;
            writeIdx = Length;
            readIdx = 0;
        }

        /// <summary>
        /// 检查并移动数据到数组开头
        /// </summary>
        public void CheckAndMoveBytes()
        {
            if (Length < 8) MoveBytes();
        }

        /// <summary>
        /// 把数据移动到数组开头
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
