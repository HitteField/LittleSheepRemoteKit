using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LittleSheep
{
    public static class FileOperationKit
    {
        /// <summary>
        /// 将文件大小按照1024的幂进行转化为字符串
        /// </summary>
        /// <param name="lengthOfFile"></param>
        /// <returns></returns>
        public static string ConvertLength(long lengthOfFile)
        {

            if (lengthOfFile < 1024)
                return string.Format(lengthOfFile.ToString() + 'B');
            else if (lengthOfFile > 1024 && lengthOfFile <= Math.Pow(1024, 2))
                return string.Format((lengthOfFile / 1024.0).ToString() + "KB");
            else if (lengthOfFile > Math.Pow(1024, 2) && lengthOfFile <= Math.Pow(1024, 3))
                return string.Format((lengthOfFile / 1024.0 / 1024.0).ToString() + "MB");
            else
                return string.Format((lengthOfFile / 1024.0 / 1024.0 / 1024.0).ToString() + "GB");
        }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        /// <param name="filePath">文件的保存路径</param>
        /// <returns></returns>
        public static long GetFileLength(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        /// <summary>
        /// 获取文件的大小的字符串
        /// </summary>
        /// <param name="filePath">文件的保存路径</param>
        /// <returns></returns>
        public static string GetFileLengthString(string filePath)
        {
            return ConvertLength(new FileInfo(filePath).Length);
        }
    }
}
