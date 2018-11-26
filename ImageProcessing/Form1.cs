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

namespace ImageProcessing
{

    public partial class Form1 : Form
    {

        //LPF pakai 1/8
        //HPF pakai 1/9

        private LPFSetting LPFSetting;
        private HPFSetting HPFSetting;
        private MedianSetting MedianSetting;

        private Bitmap bmpAwal;
        private Bitmap bmpHPF;
        private Bitmap bmpLPF;
        private Bitmap bmpMedian;
        private int bmpW;
        private int bmpH;
        Stopwatch sw = new Stopwatch();
        private int _processorCount = Environment.ProcessorCount/4;

        private string msg;

        private int[][] HPFSet = new int[][] { new int[] { 0,-1,0 },
                                               new int[] { -1,4,-1 },
                                               new int[] { 0,-1,0 }};

        private int[][] LPFSet = new int[][] { new int[] { 1,0,1 },
                                               new int[] { 0,1,0 },
                                               new int[] { 1,0,1 }};

        private int MedianLenght = 3;

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
        static int SRCCOPY = 0x00CC0020;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct BITMAPINFO
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
        static uint MAKERGB(int r,int g,int b)
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
          BITMAPINFO bmi = new BITMAPINFO();
          bmi.biSize=40;  // the size of the BITMAPHEADERINFO struct
          bmi.biWidth=w;
          bmi.biHeight=h;
          bmi.biPlanes=1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
          bmi.biBitCount=(short)bpp; // ie. 1bpp or 8bpp
          bmi.biCompression=BI_RGB; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
          bmi.biSizeImage = (uint)(((w+7)&0xFFFFFFF8)*h/8);
          bmi.biXPelsPerMeter=1000000; // not really important
          bmi.biYPelsPerMeter=1000000; // not really important
          // Now for the colour table.
          uint ncols = (uint)1<<bpp; // 2 colours for 1bpp; 256 colours for 8bpp
          bmi.biClrUsed=ncols;
          bmi.biClrImportant=ncols;
          bmi.cols=new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
          if (bpp==1) {bmi.cols[0]=MAKERGB(0,0,0); bmi.cols[1]=MAKERGB(255,255,255);}
          else {for (int i=0; i<ncols; i++) bmi.cols[i]=MAKERGB(i,i,i);}
          // For 8bpp we've created an palette with just greyscale colours.
          // You can set up any palette you want here. Here are some possibilities:
          // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
          // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
          //          for (int i=0; i<216; i++) bmi.cols[i]=MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
          // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
          // 
          // Now create the indexed bitmap "hbm0"
          IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
          IntPtr hbm0 = CreateDIBSection(IntPtr.Zero,ref bmi,DIB_RGB_COLORS,out bits0,IntPtr.Zero,0);
          //
          // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
          // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
          IntPtr sdc = GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
           // Next, create a DC for the original hbitmap
          IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc,hbm); 
          // and create a DC for the monochrome hbitmap
          IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0,hbm0);
          // Now we can do the BitBlt:
          BitBlt(hdc0,0,0,w,h,hdc,0,0,SRCCOPY);
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

        private unsafe Bitmap BitmapFromArray(Int32[,] pixels, int width, int height)
        {
            
            Bitmap bitmap = new Bitmap(width/3, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            for (int y = 0; y < height; y++)
            {
                byte* row = (byte*)bitmapData.Scan0 + bitmapData.Stride * y;
                for (int x = 0; x < width; x++)
                {  
                    byte grayShade8bit = (byte)(pixels[y, x]);
                    row[x] = grayShade8bit;
                }
            }
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private int[][] GetArrayImage(Bitmap processedBitmap)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            long byteCount = bitmapData.Stride * processedBitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            Console.WriteLine($"width : {processedBitmap.Width} widinByte : {widthInBytes} bitmapDataWidth: {bitmapData.Width}");

            int[][] pix = new int[heightInPixels][];

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                int[] linearray = new int[widthInBytes];
                for (int x = 0; x < widthInBytes; x++)
                {
                    linearray[x] = (int)pixels[currentLine + x];
                }
                pix[y] = linearray;
            }
            processedBitmap.UnlockBits(bitmapData);
            return pix;
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
                        int B = currentLine[x];
                        int G = currentLine[x + 1];
                        int R = currentLine[x + 2];

                        int avg = (R + B + G) / 3;

                        currentLine[x] = (byte)avg;
                        currentLine[x + 1] = (byte)avg;
                        currentLine[x + 2] = (byte)avg;
                    }
                    toGrayscaleBW.ReportProgress(1);
                });
                bmp.UnlockBits(bitmapData);
            }
        }

        public void LPF_Parallel(Bitmap bmp, int[][] setLPF)
        {
            unsafe
            {   
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                var set = setLPF;

                int total = set.Sum(arr => arr.Sum());
                int n = set.Length / 2;

                Parallel.For(0, heightInPixels - 1, h =>
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
                                        //Console.WriteLine($"height:{h + y} width{w + x}");
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
                        //Console.WriteLine($"hasil : {a}");
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    LPF.ReportProgress(1);
                });
                bmp.UnlockBits(bitmapData);
            }
        }

        public void HPF_Parallel(Bitmap bmp, int[][] setHPF)
        {    
            unsafe
            {    
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                var set = setHPF; 
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
                                        byte* getLine = ptrFirstPixel + ((h + y) * bitmapData.Stride);
                                        pixel += getLine[w + x] * set[sety][setx];
                                    }
                                }
                                setx++;
                            }
                            sety++;
                        }
                        var a = pixel/6;
                        if (a < 0)
                        {
                            a = 0;
                        }
                        else if (a > 255)
                        {
                            a = 255;
                        } 
                        Console.Write(a+" ");
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    HPF.ReportProgress(1);
                }
                bmp.UnlockBits(bitmapData);
            }
        }

        public void HPF_Parallel2(Bitmap bmp, int[][] setHPF)
        {

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                var set = setHPF; 
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
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                int n = nMatrix / 2;


                Parallel.For(0, heightInPixels, h =>
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

                                    byte* getLine = ptrFirstPixel + ((h + y) * bitmapData.Stride);
                                    pixel.Add(getLine[w + x]);
                                }
                                setx++;
                            }
                            sety++;
                        }
                        pixel = pixel.OrderBy(x => x).ToList();
                        int a = pixel[pixel.Count() / 2];
                        currentLine[w] = (byte)a;
                        currentLine[w + 1] = (byte)a;
                        currentLine[w + 2] = (byte)a;
                    }
                    Median.ReportProgress(1);

                });
                bmp.UnlockBits(bitmapData);
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
            this.msg = message;
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
            bmpAwal = e.Result as Bitmap;

            Bitmap test = CopyToBpp(bmpAwal, 8);

            var array = GetArrayImage(bmpAwal);

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i<array.Length; i++)
            {
                sb.Append(string.Join("'",array[i]));
                sb.Append(Environment.NewLine);
            }
            File.WriteAllText(@"D:\aaaaa.txt",sb.ToString());

            ImageBox.Image = test;

            Console.WriteLine(test.PixelFormat);

            using (MemoryStream ms = new MemoryStream())
            {
                bmpAwal.Save(ms,ImageFormat.Bmp);
                bmpSize.Text = ms.Length.ToString();
            }

            using (MemoryStream ms = new MemoryStream())
            {  
                bmpAwal.Save(ms,ImageFormat.Jpeg);
                jpegSize.Text = ms.Length.ToString();
            } 
        }

        private void sAVEToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {   
                bmpAwal = new Bitmap(openFileDialog1.FileName) as Bitmap;
                bmpH = bmpAwal.Height;
                bmpW = bmpAwal.Width;
                Console.WriteLine($"Image Resolution {bmpAwal.Width} x {bmpAwal.Height} ");
                ShowProgressBar(bmpAwal.Height);
                var bmp = bmpAwal;
                toGrayscaleBW.RunWorkerAsync(argument: bmp);
                bmpHPF = null;
                bmpLPF = null;
                bmpMedian = null; 
            }
            button4_MouseDown(button4, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            button4.PerformClick();

        }

        private void button1_Click(object sender, EventArgs e)
        {     
            try{  
                if(bmpHPF == null)
                {
                    ShowProgressBar(bmpAwal.Height);
                    bmpHPF = new Bitmap(bmpAwal);
                    HPF.RunWorkerAsync();
                }
                else
                {   
                   ImageBox.Image = bmpHPF;
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(this, "Pilih gambar terlebih dahulu","ERROR",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);

            }
        }

        private void HPF_DoWork(object sender, DoWorkEventArgs e)
        {
            
            sw.Restart();
            HPF_Parallel(bmpHPF, HPFSet);
            sw.Stop();
            Console.WriteLine($"HPF : {sw.ElapsedMilliseconds} ms");
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
            ImageBox.Image = bmpHPF;
            ImageBox.Invalidate();
            ImageBox.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ImageBox.Image = bmpAwal;
        }

        private void LPF_DoWork(object sender, DoWorkEventArgs e)
        {   
            sw.Restart(); 
            LPF_Parallel(bmpLPF, LPFSet);
            sw.Stop();
            Console.WriteLine($"Low Passing Filter Processed in {sw.ElapsedMilliseconds} ms"); 
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
                ImageBox.Image = bmpLPF; 

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (bmpLPF == null)
                {  
                    ShowProgressBar(bmpAwal.Height);
                    bmpLPF = new Bitmap(bmpAwal);
                   
                    LPF.RunWorkerAsync();  
                }
                else
                {
                    ImageBox.Image = bmpLPF;
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
            if(bmpAwal != null)
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
            sw.Restart();
            if (Median.IsBusy)
            {
                Median.CancelAsync();
            }

            var bmp = e.Argument as Bitmap;
            Median_Parallel(bmp, MedianLenght);

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

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Median Filter processed in {sw.ElapsedMilliseconds} ms");
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
                if (bmpMedian == null)
                {
                    ShowProgressBar(bmpAwal.Height);
                    bmpMedian = new Bitmap(bmpAwal);
                    Median.RunWorkerAsync(argument: bmpMedian);
                }
                else
                {
                    ImageBox.Image = bmpMedian;
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(this, "Pilih gambar terlebih dahulu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(msg);
        }

        private void lPFSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (LPFSetting = new LPFSetting(LPFSet)) {
                if(LPFSetting.ShowDialog() == DialogResult.OK)
                {
                    this.LPFSet = LPFSetting.LPFSet;
                    if (bmpLPF!=null)
                    {
                        bmpLPF = null;
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
            using (HPFSetting = new HPFSetting(HPFSet))
            {
                if (HPFSetting.ShowDialog() == DialogResult.OK)
                {
                    this.HPFSet = HPFSetting.HPFSet;
                    if (bmpHPF != null)
                    {
                        bmpHPF = null;
                        button4_MouseDown(buttonHPF, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        buttonHPF.PerformClick();
                    }
                }
                    
            }
        }

        private void medianSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (MedianSetting = new MedianSetting(MedianLenght))
            {
                if (MedianSetting.ShowDialog() == DialogResult.OK)
                {
                    this.MedianLenght = MedianSetting.MedianLenght;
                    if (bmpMedian != null)
                    {
                        bmpMedian = null;
                        button4_MouseDown(buttonMedian, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        buttonMedian.PerformClick();
                    }
                }

            }
        }
    }
}
