using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        const int windowWidth = 1920;
        const int windowHeight = 1080;


        /// <summary>
        /// 获得当前屏幕的Bitmap，转换为1920*1080
        /// </summary>
        /// <returns>返回BItMap</returns>
        public Bitmap GetScreenBitmap() 
        {
            Tuple<int, int> size = GetPhysicalScreenSize();

            Rectangle bounds = new Rectangle(0, 0, size.Item1, size.Item2);

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using(Graphics g = Graphics.FromImage(bitmap)) {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
            }

            if(size.Item1!=1920 || size.Item2!=1080)
            {
                bitmap = ResizeBitmap(bitmap, windowWidth, windowHeight);
            }

            // bitmap.Save($".//test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return bitmap;
        }

        /// <summary>
        /// 返回行主序的将转化成了1920*1080的屏幕截图分成16块等大的480*270的位图的列表
        /// </summary>
        /// <returns></returns>
        public List<Bitmap> GetScreenBitmapChunk()
        {
            const int stepX = windowWidth / 4;
            const int stepY = windowHeight / 4;

            List<Bitmap> result = new List<Bitmap>();
            Bitmap source = GetScreenBitmap();

            int startY = 0;
            for (int y = 1; y <= 4; ++y, startY += stepY) 
            {
                int startX = 0;
                for (int x = 1; x <= 4; ++x, startX += stepX) 
                {
                    result.Add(CutBitmap(source, startX, startY, stepX, stepY));
                }
            }
            return result;
        }

        /// <summary>
        /// 缩放位图
        /// </summary>
        /// <param name="bmp">源</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <returns></returns>
        private static Bitmap ResizeBitmap(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 裁剪位图
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="startX">原起始位置X</param>
        /// <param name="startY">原起始位置Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns></returns>
        private static Bitmap CutBitmap(Bitmap source, int startX, int startY, int iWidth, int iHeight)
        {
            if (source == null)
            {
                return null;
            }

            int w = source.Width;
            int h = source.Height;

            if (startX >= w || startY >= h)
            {
                return null;
            }

            if (startX + iWidth > w)
            {
                   iWidth = w - startX;
            }

            if (startY + iHeight > h)
            {
                   iHeight = h - startY;
            }

            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(source, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(startX, startY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();

                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 得到当前屏幕jpeg图像zip压缩的字节流
        /// </summary>
        /// <returns>当前屏幕jpeg图像zip压缩的字节流</returns>
        [Obsolete]
        private byte[] WindowGetter() {
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
        [Obsolete]
        private Bitmap WindowSetter(byte[] stream) {
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