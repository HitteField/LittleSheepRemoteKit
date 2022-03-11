using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LittleSheep
{
    class WindowManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly WindowManager instance = new WindowManager();
        }
        private WindowManager() { }
        public static WindowManager Instance { get { return Nested.instance; } }
        #endregion

        /// <summary>
        /// 获得当前屏幕的Bitmap
        /// </summary>
        /// <returns>返回BItMap</returns>
        private Bitmap GetScreenBitmap() {
            Tuple<int, int> size = GetPhysicalScreenSize();

            Rectangle bounds = new Rectangle(0, 0, size.Item1, size.Item2);

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using(Graphics g = Graphics.FromImage(bitmap)) {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
            }

            // bitmap.Save($".//test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return bitmap;
        }

        /// <summary>
        /// 得到当前屏幕jpeg图像zip压缩的字节流
        /// </summary>
        /// <returns>当前屏幕jpeg图像zip压缩的字节流</returns>
        public byte[] WindowGetter() {
            Bitmap cur = GetScreenBitmap();
            using(MemoryStream stream = new MemoryStream()) {
                cur.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return CompressKit.CompressBytes(stream.ToArray());
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceCaps", SetLastError = true)]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        /// <summary>
        /// 获得物理显示器的分辨率
        /// </summary>
        /// <returns>返回(水平分辨率,垂直分辨率)Tuple</returns>
        private Tuple<int, int> GetPhysicalScreenSize() {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            var physicalScreenHeight = GetDeviceCaps(desktop, 117); // 117为垂直分辨率
            int physicalScreenWidth = GetDeviceCaps(desktop, 118); // 118为水平分辨率
            return new Tuple<int, int>(physicalScreenWidth, physicalScreenHeight);
        }

        /// <summary>
        /// 将压缩的字节流解压并转换成bitmap
        /// </summary>
        /// <param name="stream">压缩的字节流</param>
        /// <returns>解压字节流后并解码的Bitmap</returns>
        public Bitmap WindowSetter(byte[] stream) {
            byte[] decodebytes = CompressKit.DecompressBytes(stream);
            using(MemoryStream m_stream = new MemoryStream()) {
                m_stream.Read(decodebytes,0,decodebytes.Length);
                return new Bitmap(m_stream);
            }
        }

        //-------------------------传输--------------------------------------
        /// <summary>
        /// 获取oldone和newone进行异或得到的位图所对应的字节流
        /// </summary>
        /// <param name="oldone">前一张位图</param>
        /// <param name="newone">后一张位图</param>
        /// <returns></returns>
        public byte[] GetBitmapXOR(Bitmap oldone,Bitmap newone)
        {
            var size = WindowManager.Instance.GetPhysicalScreenSize();
            int width = size.Item1, height = size.Item2;

            BitmapData olddata = oldone.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData newdata = newone.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            width *= 3;

            int stride = olddata.Stride;
            int offset = stride - width;
            IntPtr iptr1 = olddata.Scan0;
            IntPtr iptr2 = newdata.Scan0;
            int scanBytes = stride * height;
            int posScan = 0, posReal = 0;

            byte[] xorResult = new byte[scanBytes];

            byte[] oldbytes = new byte[scanBytes];
            byte[] newbytes = new byte[scanBytes];

            Marshal.Copy(iptr1, oldbytes, 0, scanBytes);
            Marshal.Copy(iptr2, newbytes, 0, scanBytes);

            for (int x = 0; x < height; ++x)
            {
                for (int y = 0; y < width; ++y)
                {
                    xorResult[posScan] = (byte)(oldbytes[posReal] ^ newbytes[posReal]);
                    posScan++;
                    posReal++;
                }
                posScan += offset;
            }

            return xorResult;
        }

        /// <summary>
        /// 获取oldone和newone进行异或得到的位图所对应的被压缩过的字节流
        /// </summary>
        /// <param name="oldone">前一张位图</param>
        /// <param name="newone">后一张位图</param>
        /// <returns></returns>
        public byte[] GetCompressedBitmapXOR(Bitmap oldone,Bitmap newone)
        {
            return CompressKit.GZipSevenZipCompress(GetBitmapXOR(oldone, newone));
        }

        /// <summary>
        /// 获取oldone和newone进行异或得到的位图所对应的被压缩过的字节流的解压
        /// </summary>
        /// <param name="source">被压缩的字节流</param>
        /// <returns></returns>
        public byte[] GetDecompressedBitmapXOR(byte[] source)
        {
            return CompressKit.GZipSevenZipDecompress(source);
        }
    }
}