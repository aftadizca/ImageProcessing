using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace ImageProcessing
{

    public partial class Form1 : Form
    {

        //LPF pakai 1/8
        //HPF pakai 1/9

        private LpfSetting _lpfSetting;
        private HpfSetting _hpfSetting;
        private MedianSetting _medianSetting;

        private Bitmap _bmpAwal;
        private Bitmap _bmpHpf;
        private Bitmap _bmpLpf;
        private Bitmap _bmpMedian;
        private int _bmpW;
        private int _bmpH;
        Stopwatch _sw = new Stopwatch();
        private int _processorCount = Environment.ProcessorCount/4;

        private string _msg;

        private int[][] _hpfSet = new int[][] { new int[] { 0,-1,0 },
                                               new int[] { -1,4,-1 },
                                               new int[] { 0,-1,0 }};

        private int[][] _lpfSet = new int[][] { new int[] { 1,0,1 },
                                               new int[] { 0,1,0 },
                                               new int[] { 1,0,1 }};

        private int _medianLength = 3;

        #region Alternatives Method

        //public int LPFGetPixel(int[][] set, Bitmap bmp, int h, int w)
        //{
        //    int total = set.Sum(arr => arr.Sum());
        //    int n = set.Length / 2;
        //    int pixel = 0;
        //    int sety = 0;
        //    for (int y = -n; y <= n; y++)
        //    {
        //        int setx = 0;
        //        for (int x = -n; x <= n; x++)
        //        {
        //            if ((h + y >= 0 && h + y <= bmp.Height - 1) && (w + x >= 0 && w + x <= bmp.Width - 1))
        //            {
        //                if (set[sety][setx] != 0)
        //                {
        //                    pixel += bmp.GetPixel(w + x, h + y).R * set[sety][setx];
        //                }
        //            }
        //            setx++;
        //        }
        //        sety++;
        //    }
        //    var a = pixel / total;
        //    if (a < 0)
        //    {
        //        a = 0;
        //    }
        //    else if (a > 255)
        //    {
        //        a = 255;
        //    }

        //    return a;

        //}

        //public int HPFGetPixel(int[][] set, Bitmap bmp, int h, int w)
        //{
        //    int total = 9;
        //    int n = set.Length / 2;
        //    int pixel = 0;
        //    int sety = 0;
        //    for (int y = -n; y <= n; y++)
        //    {
        //        int setx = 0;
        //        for (int x = -n; x <= n; x++)
        //        {
        //            if ((h + y >= 0 && h + y <= bmp.Height - 1) && (w + x >= 0 && w + x <= bmp.Width - 1))
        //            {
        //                if (set[sety][setx] != 0)
        //                {
        //                    pixel += bmp.GetPixel(w + x, h + y).R * set[sety][setx];
        //                }
        //            }
        //            setx++;
        //        }
        //        sety++;
        //    }
        //    var a = pixel / total;
        //    if (a < 0)
        //    {
        //        a = 0;
        //    }
        //    else if (a > 255)
        //    {
        //        a = 255;
        //    }

        //    return a;

        //}

        //public int MedianGetPixel(Bitmap bmp, int h, int w, int nMatrix)
        //{
        //    int n = nMatrix / 2;
        //    List<int> pixel = new List<int>();

        //    for (int y = -n; y <= n; y++)
        //    {
        //        for (int x = -n; x <= n; x++)
        //        {
        //            if ((h + y >= 0 && h + y <= bmp.Height - 1) && (w + x >= 0 && w + x <= bmp.Width - 1))
        //            {
        //                pixel.Add(bmp.GetPixel(w + x, h + y).R);
        //            }
        //        }
        //    }
        //    pixel = pixel.OrderBy(x => x).ToList();
        //    return pixel[(pixel.Count()) / 2];
        //}

        #endregion

        #region API
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        static int _srccopy = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref Bitmapinfo bmi, uint usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint _biRgb = 0;
        static uint _dibRgbColors = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Bitmapinfo
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }
        static uint Makergb(int r,int g,int b)
        { 
            return ((uint)(b&255)) | ((uint)((r&255)<<8)) | ((uint)((g&255)<<16));
        }
        #endregion 

        static Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        { 
         if (bpp!=1 && bpp!=8) throw new System.ArgumentException("1 or 8","bpp");

          // Plan: built into Windows GDI is the ability to convert
          // bitmaps from one format to another. Most of the time, this
          // job is actually done by the graphics hardware accelerator card
          // and so is extremely fast. The rest of the time, the job is done by
          // very fast native code.
          // We will call into this GDI functionality from C#. Our plan:
          // (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
          // (2) Create a GDI monochrome hbitmap
          // (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
          // (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

          int w=b.Width, h=b.Height;
          IntPtr hbm = b.GetHbitmap(); // this is step (1)
          //
          // Step (2): create the monochrome bitmap.
          // "BITMAPINFO" is an interop-struct which we define below.
          // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
          Bitmapinfo bmi = new Bitmapinfo();
          bmi.biSize=40;  // the size of the BITMAPHEADERINFO struct
          bmi.biWidth=w;
          bmi.biHeight=h;
          bmi.biPlanes=1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
          bmi.biBitCount=(short)bpp; // ie. 1bpp or 8bpp
          bmi.biCompression=_biRgb; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
          bmi.biSizeImage = (uint)(((w+7)&0xFFFFFFF8)*h/8);
          bmi.biXPelsPerMeter=1000000; // not really important
          bmi.biYPelsPerMeter=1000000; // not really important
          // Now for the colour table.
          uint ncols = (uint)1<<bpp; // 2 colours for 1bpp; 256 colours for 8bpp
          bmi.biClrUsed=ncols;
          bmi.biClrImportant=ncols;
          bmi.cols=new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
          if (bpp==1) {bmi.cols[0]=Makergb(0,0,0); bmi.cols[1]=Makergb(255,255,255);}
          else {for (int i=0; i<ncols; i++) bmi.cols[i]=Makergb(i,i,i);}
          // For 8bpp we've created an palette with just greyscale colours.
          // You can set up any palette you want here. Here are some possibilities:
          // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
          // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
          //          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
          // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
          // 
          // Now create the indexed bitmap "hbm0"
          IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
          IntPtr hbm0 = CreateDIBSection(IntPtr.Zero,ref bmi,_dibRgbColors,out bits0,IntPtr.Zero,0);
          //
          // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
          // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
          IntPtr sdc = GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
           // Next, create a DC for the original hbitmap
          IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc,hbm); 
          // and create a DC for the monochrome hbitmap
          IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0,hbm0);
          // Now we can do the BitBlt:
          BitBlt(hdc0,0,0,w,h,hdc,0,0,_srccopy);
          // Step (4): convert this monochrome hbitmap back into a Bitmap:
          System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);
          //
          // Finally some cleanup.
          DeleteDC(hdc);
          DeleteDC(hdc0);
          ReleaseDC(IntPtr.Zero,sdc);
          DeleteObject(hbm);
          DeleteObject(hbm0);
          //
          return b0;
        }

        private unsafe Bitmap RLEToImage(string path)
        {

            var data = File.ReadAllText(path);
            var dataLine = data.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            List<List<int>> arrayImage = new List<List<int>>();
            Console.WriteLine(dataLine.Length);
            foreach (var height in dataLine)
            {
                var y = height.Split(' ');
                List<int> temp = new List<int>();
                foreach (var x in y)
                {
                    if (x.Contains("m"))
                    {
                        var xx = x.Split('m');
                        for (int i = 0; i < int.Parse(xx[0]); i++)
                        {
                            temp.Add(int.Parse(xx[1]));
                        }

                    }
                    else if(x != "")
                    {
                        temp.Add(int.Parse(x));
                    }  
                                
                }
                arrayImage.Add(temp);

            }

            Bitmap bitmap = new Bitmap(arrayImage[0].Count, arrayImage.Count, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            for (int y = 0; y < bitmap.Height; y++)
            {
                byte* row = (byte*)bitmapData.Scan0 + bitmapData.Stride * y;
                int xx = 0;
                for (int x = 0; x < widthInBytes; x+=bytesPerPixel)
                {
                    
                    row[x] = (byte)arrayImage[y][xx];
                    row[x+1] = (byte)arrayImage[y][xx];
                    row[x+2] = (byte)arrayImage[y][xx];
                    xx++; 
                }
            }
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private void ImageToRLE(Bitmap processedBitmap, string path)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            long byteCount = bitmapData.Stride * processedBitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            
            StringBuilder sb = new StringBuilder();
            int count = 1;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                int[] linearray = new int[widthInBytes];
                for (int x = 0; x < widthInBytes; x+=bytesPerPixel)
                {
                    if (x == widthInBytes-bytesPerPixel)
                    {

                    }
                    else if(pixels[currentLine + x] == pixels[currentLine + x+bytesPerPixel] )
                    {
                        count++;
                        continue;
                    }  
                    var cstr = count > 1 ? count + "m" : "";
                    sb.Append($"{cstr}{(int) pixels[currentLine + x]} ");
                    count = 1;
                }

                sb.Append(Environment.NewLine);
            }
            processedBitmap.UnlockBits(bitmapData);
            File.WriteAllText(path,sb.ToString());
        } 

        public void GrayScale_Parallel(Bitmap bmp)
        {
            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int b = currentLine[x];
                        int g = currentLine[x + 1];
                        int r = currentLine[x + 2];

                        int avg = (r + b + g) / 3;

                        currentLine[x] = (byte)avg;
                        currentLine[x + 1] = (byte)avg;
                        currentLine[x + 2] = (byte)avg;
                    }
                    toGrayscaleBW.ReportProgress(1);
                });
                bmp.UnlockBits(bitmapData);
            }
        }

        public void LPF_Parallel(Bitmap bmp, int[][] setLpf)
        {
            unsafe
            {   
                BitmapData ori = _bmpAwal.LockBits(new Rectangle(0, 0, _bmpAwal.Width, _bmpAwal.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                byte* ptrDataScan0 = (byte*)ori.Scan0;

                var set = setLpf;

                int total = set.Sum(arr => arr.Sum());
                int n = set.Length / 2;

                for(int h=0; h<heightInPixels; h++)
                {
                    byte* currentLine = ptrFirstPixel + (h * bitmapData.Stride);

                    for (int w = 0; w < widthInBytes; w = w + bytesPerPixel)
                    {
                        int pixel = 0;
                        int sety = 0;

                        for (int y = -n; y <= n; y++)
                        {
                            int setx = 0;
                            for (int x = -n * bytesPerPixel; x <= n * bytesPerPixel; x = x + bytesPerPixel)
                            {
                                if ((h + y >= 0 && h + y < heightInPixels) && (w + x >= 0 && w + x < widthInBytes))
                                {
                                    if (set[sety][setx] != 0)
                                    {
                                        byte* getLine = ptrDataScan0 + ((h + y) * ori.Stride);
                                        pixel += getLine[w + x] * set[sety][setx];
                                    }
                                }
                                setx++;
                            }
                            sety++;
                        }
                        var a = pixel / total;
                        if (a < 0)
                        {
                            a = 0;
                        }
                        else if (a > 255)
                        {
                            a = 255;
                        } 
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    LPF.ReportProgress(1);
                }
                bmp.UnlockBits(bitmapData);
                _bmpAwal.UnlockBits(ori);
            }
        }

        public void HPF_Parallel(Bitmap bmp, int[][] setHpf)
        {    
            unsafe
            {   
                BitmapData ori = _bmpAwal.LockBits(new Rectangle(0, 0, _bmpAwal.Width, _bmpAwal.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                byte* ptrFirstoriScan0 = (byte*)ori.Scan0;

                var set = setHpf; 
                int n = set.Length / 2;

                for(int h = 0; h < heightInPixels; h++) 
                {
                    byte* currentLine = ptrFirstPixel + (h * bitmapData.Stride);

                    for (int w = 0; w < widthInBytes; w = w + bytesPerPixel)
                    {
                        int pixel = 0;
                        int sety = 0;

                        for (int y = -n; y <= n; y++)
                        {
                            int setx = 0;
                            for (int x = -n * bytesPerPixel; x <= n * bytesPerPixel; x = x + bytesPerPixel)
                            {
                                if ((h + y >= 0 && h + y < heightInPixels) && (w + x >= 0 && w + x < widthInBytes))
                                {
                                    if (set[sety][setx] != 0)
                                    {
                                        byte* getLine = ptrFirstoriScan0 + ((h + y) * ori.Stride);
                                        pixel += getLine[w + x] * set[sety][setx];
                                    }
                                }
                                setx++;
                            }
                            sety++;
                        }
                        var a = pixel;
                        if (a < 0)
                        {
                            a = 0;
                        }
                        else if (a > 255)
                        {
                            a = 255;
                        }
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    HPF.ReportProgress(1);
                }
                bmp.UnlockBits(bitmapData);
                _bmpAwal.UnlockBits(ori);
            }
        }

        public void HPF_Parallel2(Bitmap bmp, int[][] setHpf)
        {

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                var set = setHpf; 
                int n = set.Length / 2;


                var tasks = new Task[_processorCount];

                for (int taskNum = 0; taskNum < _processorCount; taskNum++)
                {
                    int taskNumCopy = taskNum;

                    tasks[taskNum] = Task.Factory.StartNew(
                        () =>
                        {
                            var max = heightInPixels / _processorCount * (taskNumCopy+1);
                            var sisa = heightInPixels % _processorCount;
                            if (taskNumCopy == _processorCount - 1)
                            {
                               max += sisa;
                            }
                            
                            Console.WriteLine($"awal:{heightInPixels*taskNumCopy/_processorCount} max:{max-1}");

                            for(int h = heightInPixels*taskNumCopy/_processorCount; h < max; h++) 
                            {
                                byte* currentLine = ptrFirstPixel + (h * bitmapData.Stride);

                                for (int w = 0; w < widthInBytes; w = w + bytesPerPixel)
                                {
                                    int pixel = 0;
                                    int sety = 0;

                                    for (int y = -n; y <= n; y++)
                                    {
                                        int setx = 0;
                                        for (int x = -n * bytesPerPixel; x <= n * bytesPerPixel; x = x + bytesPerPixel)
                                        {
                                            if ((h + y >= 0 && h + y < heightInPixels) && (w + x >= 0 && w + x < widthInBytes))
                                            {
                                                if (set[sety][setx] != 0)
                                                {
                                                    byte* getLine = ptrFirstPixel + ((h + y) * bitmapData.Stride);
                                                    pixel += getLine[w + x] * set[sety][setx];
                                                }
                                            }
                                            setx++;
                                        }
                                        sety++;
                                    }
                                    var a = pixel / 9;
                                    if (a < 0)
                                    {
                                        a = 0;
                                    }
                                    else if (a > 255)
                                    {
                                        a = 255;
                                    }
                                    currentLine[w] = (byte)a;
                                    currentLine[w + 1] = (byte)a;
                                    currentLine[w + 2] = (byte)a;
                                }
                                HPF.ReportProgress(1);
                            } 
                        });
                }

                Task.WaitAll(tasks); 
                bmp.UnlockBits(bitmapData);
            }
        }

        public void Median_Parallel(Bitmap bmp, int nMatrix)
        {

            unsafe
            {
                BitmapData ori = _bmpAwal.LockBits(new Rectangle(0, 0, _bmpAwal.Width, _bmpAwal.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                byte* ptrDataScan0 = (byte*)ori.Scan0;

                int n = nMatrix / 2;


                for(int h=0;h<heightInPixels; h++)
                {
                    byte* currentLine = ptrFirstPixel + (h * bitmapData.Stride);

                    for (int w = 0; w < widthInBytes; w = w + bytesPerPixel)
                    {
                        List<int> pixel = new List<int>();
                        int sety = 0;

                        for (int y = -n; y <= n; y++)
                        {
                            int setx = 0;
                            for (int x = -n * bytesPerPixel; x <= n * bytesPerPixel; x = x + bytesPerPixel)
                            {
                                if ((h + y >= 0 && h + y < heightInPixels) && (w + x >= 0 && w + x < widthInBytes))
                                {

                                    byte* getLine = ptrDataScan0 + ((h + y) * ori.Stride);
                                    pixel.Add(getLine[w + x]);
                                }
                                setx++;
                            }
                            sety++;
                        }
                        pixel = pixel.OrderBy(x => x).AsParallel().ToList();
                        int a = pixel[pixel.Count() / 2];
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    Median.ReportProgress(1);

                }
                bmp.UnlockBits(bitmapData);
                _bmpAwal.UnlockBits(ori);
            }
        }


        public void ShowProgressBar(int max)
        {
            progressBar1.Visible = true;
            progressBar1.Maximum = max;
            progressBar1.Value = 0;
        }  


        public Form1()
        {
            InitializeComponent(); 
        }

        public Form1(string message)
        {
            this._msg = message;
            InitializeComponent();
        }

        private void toGrayscaleBW_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var bmp = e.Argument as Bitmap;

            GrayScale_Parallel(bmp); 

            e.Result = bmp;
            sw.Stop();
            Console.WriteLine($"Grayscale processes in {sw.ElapsedMilliseconds} ms");
        }

        private void toGrayscaleBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {  
           progressBar1.Value += e.ProgressPercentage;
        }

        private void toGrayscaleBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            ImageBox.Image = e.Result as Bitmap;
            _bmpAwal = e.Result as Bitmap;

            ImageBox.Image = _bmpAwal;

            //ImageToRLE(_bmpAwal);
            RLESize.Text = new FileInfo(@"rleTemp.rle").Length.ToString();
            bmpSize.Text = (_bmpAwal.Height * _bmpAwal.Width * Image.GetPixelFormatSize(_bmpAwal.PixelFormat)/8).ToString();
            jpegSize.Text = new FileInfo(openFileDialog1.FileName).Length.ToString();
        }

        private void sAVEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName)+".rle";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageToRLE(_bmpAwal,saveFileDialog1.FileName);
            }
        }

        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog1.FileName) == ".rle")
                {
                    _bmpAwal = RLEToImage(openFileDialog1.FileName);
                }
                else
                {
                    _bmpAwal = new Bitmap(openFileDialog1.FileName) as Bitmap;
                    ShowProgressBar(_bmpAwal.Height);
                    var bmp = _bmpAwal;
                    toGrayscaleBW.RunWorkerAsync(argument: bmp);
                } 
                Console.WriteLine($"Image Resolution {_bmpAwal.Width} x {_bmpAwal.Height} ");
                
                _bmpHpf = null;
                _bmpLpf = null;
                _bmpMedian = null; 
            }
            button4_MouseDown(button4, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            button4.PerformClick();

        }

        private void button1_Click(object sender, EventArgs e)
        {     
            try{  
                if(_bmpHpf == null)
                {
                    ShowProgressBar(_bmpAwal.Height);
                    _bmpHpf = new Bitmap(_bmpAwal);
                    HPF.RunWorkerAsync();
                }
                else
                {   
                   ImageBox.Image = _bmpHpf;
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(this, "Pilih gambar terlebih dahulu","ERROR",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);

            }
        }

        private void HPF_DoWork(object sender, DoWorkEventArgs e)
        {
            
            _sw.Restart();
            HPF_Parallel(_bmpHpf, _hpfSet);
            _sw.Stop();
            Console.WriteLine($"HPF : {_sw.ElapsedMilliseconds} ms");
        }

        private void HPF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void HPF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else 
            progressBar1.Visible = false;
            ImageBox.Image = _bmpHpf;
            ImageBox.Invalidate();
            ImageBox.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ImageBox.Image = _bmpAwal;
        }

        private void LPF_DoWork(object sender, DoWorkEventArgs e)
        {   
            _sw.Restart(); 
            LPF_Parallel(_bmpLpf, _lpfSet);
            _sw.Stop();
            Console.WriteLine($"Low Passing Filter Processed in {_sw.ElapsedMilliseconds} ms"); 
        }

        private void LPF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void LPF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else
                progressBar1.Visible = false;
                ImageBox.Image = _bmpLpf; 

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bmpLpf == null)
                {  
                    ShowProgressBar(_bmpAwal.Height);
                    _bmpLpf = new Bitmap(_bmpAwal);
                   
                    LPF.RunWorkerAsync();  
                }
                else
                {
                    ImageBox.Image = _bmpLpf;
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(this, "Pilih gambar terlebih dahulu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            button4.Focus();
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if(_bmpAwal != null)
            {
                foreach (Control c in panel2.Controls)
                {
                    if (c is Button)
                    {
                        c.BackColor = SystemColors.ControlDark;
                    }
                }
                btn.BackColor = SystemColors.ActiveCaption;
            }
        }

        private void progressBar1_VisibleChanged(object sender, EventArgs e)
        {
            if (progressBar1.Visible)
            {
                foreach (Control c in panel2.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = false;
                    }
                }
            }
            else
            {
                foreach (Control c in panel2.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = true;
                    }
                }
            }
        }

        private void Median_DoWork(object sender, DoWorkEventArgs e)
        {    
            _sw.Restart();
            if (Median.IsBusy)
            {
                Median.CancelAsync();
            }

            var bmp = e.Argument as Bitmap;
            Median_Parallel(bmp, _medianLength);

            #region Alternatives
            //for (int h = 0; h < bmp.Height; h++)
            //{
            //    for (int w = 0; w < bmp.Width; w++)
            //    {    
            //        int pixel = MedianGetPixel(bmp, h, w, MedianLenght); 

            //        bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
            //    }

            //    Median.ReportProgress(h);
            //} 
            #endregion
            e.Result = bmp;

            _sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Median Filter processed in {_sw.ElapsedMilliseconds} ms");
            Console.ResetColor();
        }

        private void Median_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void Median_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else
                progressBar1.Visible = false;
            ImageBox.Image = e.Result as Bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bmpMedian == null)
                {
                    ShowProgressBar(_bmpAwal.Height);
                    _bmpMedian = new Bitmap(_bmpAwal);
                    Median.RunWorkerAsync(argument: _bmpMedian);
                }
                else
                {
                    ImageBox.Image = _bmpMedian;
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(this, "Pilih gambar terlebih dahulu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_msg);
        }

        private void lPFSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (_lpfSetting = new LpfSetting(_lpfSet)) {
                if(_lpfSetting.ShowDialog() == DialogResult.OK)
                {
                    this._lpfSet = _lpfSetting.LpfSet;
                    if (_bmpLpf!=null)
                    {
                        _bmpLpf = null;
                        button4_MouseDown(buttonLPF, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        buttonLPF.PerformClick(); 
                    }
                }
            }
           
        }

        private void sETTINGToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hPFSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (_hpfSetting = new HpfSetting(_hpfSet))
            {
                if (_hpfSetting.ShowDialog() == DialogResult.OK)
                {
                    this._hpfSet = _hpfSetting.HpfSet;
                    if (_bmpHpf != null)
                    {
                        _bmpHpf = null;
                        button4_MouseDown(buttonHPF, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        buttonHPF.PerformClick();
                    }
                }
                    
            }
        }

        private void medianSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (_medianSetting = new MedianSetting(_medianLength))
            {
                if (_medianSetting.ShowDialog() == DialogResult.OK)
                {
                    this._medianLength = _medianSetting.MedianLenght;
                    if (_bmpMedian != null)
                    {
                        _bmpMedian = null;
                        button4_MouseDown(buttonMedian, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        buttonMedian.PerformClick();
                    }
                }

            }
        }
    }
}
