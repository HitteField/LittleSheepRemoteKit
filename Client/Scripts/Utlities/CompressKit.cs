using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SevenZip;

namespace LittleSheep
{
    class CompressKit
    {
        private const int BufferSize = 65536;
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="sourceBytes">源字节数组</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="password">密码</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressBytes(byte[] sourceBytes, string password = null, int compressionLevel = 6)
        {
            byte[] result = new byte[] { };

            if (sourceBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        using (MemoryStream readStream = new MemoryStream(sourceBytes))
                        {
                            using (ZipOutputStream zipStream = new ZipOutputStream(tempStream))
                            {
                                zipStream.Password = password;//设置密码
                                zipStream.SetLevel(compressionLevel);//设置压缩等级

                                ZipEntry zipEntry = new ZipEntry("ZipBytes");
                                zipEntry.DateTime = DateTime.Now;
                                zipEntry.Size = sourceBytes.Length;
                                zipStream.PutNextEntry(zipEntry);
                                int readLength = 0;
                                byte[] buffer = new byte[BufferSize];

                                do
                                {
                                    readLength = readStream.Read(buffer, 0, BufferSize);
                                    zipStream.Write(buffer, 0, readLength);
                                } while (readLength == BufferSize);

                                readStream.Close();
                                zipStream.Flush();
                                zipStream.Finish();
                                result = tempStream.ToArray();
                                zipStream.Close();
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new Exception("压缩字节数组发生错误", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 解压字节数组
        /// </summary>
        /// <param name="sourceBytes">源字节数组</param>
        /// <param name="password">密码</param>
        /// <returns>解压后的字节数组</returns>
        public static byte[] DecompressBytes(byte[] sourceBytes, string password = null)
        {
            byte[] result = new byte[] { };

            if (sourceBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream tempStream = new MemoryStream(sourceBytes))
                    {
                        using (MemoryStream writeStream = new MemoryStream())
                        {
                            using (ZipInputStream zipStream = new ZipInputStream(tempStream))
                            {
                                zipStream.Password = password;
                                ZipEntry zipEntry = zipStream.GetNextEntry();

                                if (zipEntry != null)
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    int readLength = 0;

                                    do
                                    {
                                        readLength = zipStream.Read(buffer, 0, BufferSize);
                                        writeStream.Write(buffer, 0, readLength);
                                    } while (readLength == BufferSize);

                                    writeStream.Flush();
                                    result = writeStream.ToArray();
                                    writeStream.Close();
                                }
                                zipStream.Close();
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new Exception("解压字节数组发生错误", ex);
                }
            }
            return result;
        }

        static bool hasInit = false;
        public static void InitSevenZipCompress()
        {
            if (hasInit) return;
            // 指定Dll路径
            SevenZipBase.SetLibraryPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Environment.Is64BitProcess ? "7z64.dll" : "7z.dll"));
            hasInit = true;
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>压缩后字节数组</returns>
        public static byte[] SevenZipCompress(byte[] input)
        {
            InitSevenZipCompress();
            byte[] compressed = null;
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionMethod = SevenZip.CompressionMethod.Ppmd;
            compressor.CompressionLevel = CompressionLevel.High;
            using (MemoryStream msin = new MemoryStream(input))
            {
                using (MemoryStream msout = new MemoryStream())
                {
                    compressor.CompressStream(msin, msout);

                    msout.Position = 0;
                    compressed = new byte[msout.Length];
                    msout.Read(compressed, 0, compressed.Length);
                }
            }
            return compressed;
        }

        /// <summary>
        /// 解压字节数组
        /// </summary>
        /// <param name="input">源字节数组</param>
        /// <returns>解压后字符串</returns>
        public static byte[] SevenZipDecompress(byte[] input)
        {
            InitSevenZipCompress();
            byte[] uncompressedbuffer = null;
            using (MemoryStream msin = new MemoryStream())
            {
                msin.Write(input, 0, input.Length);
                uncompressedbuffer = new byte[input.Length];
                msin.Position = 0;
                using (SevenZipExtractor extractor = new SevenZipExtractor(msin))
                {
                    using (MemoryStream msout = new MemoryStream())
                    {
                        extractor.ExtractFile(0, msout);
                        msout.Position = 0;
                        uncompressedbuffer = new byte[msout.Length];
                        msout.Read(uncompressedbuffer, 0, uncompressedbuffer.Length);
                    }
                }
            }
            return uncompressedbuffer;
        }

        //压缩字节
        //1.创建压缩的数据流 
        //2.设定compressStream为存放被压缩的文件流,并设定为压缩模式
        //3.将需要压缩的字节写到被压缩的文件流
        public static byte[] GZipCompress(byte[] bytes)
        {
            using (MemoryStream compressStream = new MemoryStream())
            {
                using (var zipStream = new System.IO.Compression.GZipStream(compressStream, System.IO.Compression.CompressionMode.Compress))
                    zipStream.Write(bytes, 0, bytes.Length);
                return compressStream.ToArray();
            }
        }
        //解压缩字节
        //1.创建被压缩的数据流
        //2.创建zipStream对象，并传入解压的文件流
        //3.创建目标流
        //4.zipStream拷贝到目标流
        //5.返回目标流输出字节
        public static byte[] GZipDecompress(byte[] bytes)
        {
            using (var compressStream = new MemoryStream(bytes))
            {
                using (var zipStream = new System.IO.Compression.GZipStream(compressStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// GZip + 7Zip 压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GZipSevenZipCompress(byte[] bytes)
        {
            return SevenZipCompress(GZipCompress(bytes));
        }

        /// <summary>
        /// GZip + 7Zip 解压
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GZipSevenZipDecompress(byte[] bytes)
        {
            return SevenZipDecompress(GZipDecompress(bytes));
        }
        
    }
}
