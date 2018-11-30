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
        private HistogramGrap _histogramGrap;
        private List<Histogram> _histogram;

        private Bitmap _bmpAwal;
        private Bitmap _bmpHpf;
        private Bitmap _bmpLpf;
        private Bitmap _bmpMedian;
        private Bitmap _bmpQuantization;
        private int _bmpW;
        private int _bmpH;
        Stopwatch _sw = new Stopwatch();
        private int _processorCount = Environment.ProcessorCount/4;

        private string _msg;
        public bool changedBytes = true;
        private int bytes;

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

            Console.WriteLine("TEST "+pixels.Count(x=>x==255));

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

        private void Quantization(Bitmap processedBitmap, int bytes)
        {
            
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
                        currentLine[w] = (byte)(a);
                        currentLine[w + 1] = (byte)(a);
                        currentLine[w + 2] = (byte)(a);
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
            grayLevel.SelectedIndex = 2;
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
            ImageBox.Image = e.Result as Bitmap;
            _bmpAwal = e.Result as Bitmap;

            ImageBox.Image = _bmpAwal;
            
            bmpSize.Text = (_bmpAwal.Height * _bmpAwal.Width * Image.GetPixelFormatSize(_bmpAwal.PixelFormat)/8).ToString();
            jpegSize.Text = new FileInfo(openFileDialog1.FileName).Length.ToString();
            progressBar1.Visible = false;
        }

        private void sAVEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName)+".rle";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageToRLE(_bmpAwal,saveFileDialog1.FileName);
                RLESize.Text = new FileInfo(@"rleTemp.rle").Length.ToString();
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
                _bmpQuantization = null;
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
            ImageBox.Image = _bmpHpf;
            progressBar1.Visible = false;
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
               
            ImageBox.Image = _bmpLpf; 
            progressBar1.Visible = false;

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
                ImageBox.Image = e.Result as Bitmap;
                progressBar1.Visible = false;
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

        private void Quantize_Click(object sender, EventArgs e)
        {

            try
            {
                if (_bmpQuantization ==null || changedBytes)
                {

                    _bmpQuantization = new Bitmap(_bmpAwal);
                    ShowProgressBar(_bmpAwal.Height);
                    //Quantization(_bmpQuantization, Int32.Parse(grayLevel.Text));
                    //ImageBox.Image = _bmpQuantization;
                    bytes = int.Parse(grayLevel.Text);
                    QuantizationWorker.RunWorkerAsync(); 
                }
                else
                {
                    ImageBox.Image = _bmpQuantization;
                }
            }
            catch (Exception ex)
            {

            MessageBox.Show(this, "Pilih gambar terlebih dahulu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void QuantizationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BitmapData bitmapData = _bmpQuantization.LockBits(new Rectangle(0, 0, _bmpQuantization.Width, _bmpQuantization.Height), ImageLockMode.ReadWrite, _bmpQuantization.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(_bmpQuantization.PixelFormat) / 8;
            long byteCount = bitmapData.Stride * _bmpQuantization.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            
            StringBuilder sb = new StringBuilder();
            int count = 1;

            _histogram = new List<Histogram>();

            Parallel.For(0,256, i =>
            {
                var bCount = pixels.Count(x => x == (byte)i);
                if ( bCount != 0)
                {
                    _histogram.Add(new Histogram(){Byte = (int)i, Count = bCount/3});
                }
                
            });
            _histogram=_histogram.OrderBy(x => x.Byte).ToList();
            int groupCount = (int)Math.Pow(2, bytes);
            long perGroup = _bmpQuantization.Height * _bmpQuantization.Width / groupCount;

            Console.WriteLine("jumlah pergrup : "+perGroup+" bit:"+groupCount );

            int penambah = 255 / (groupCount-1);
            int g = 0;
            int p = 0;
            var total = 0;
            while (p<_histogram.Count)
            {
                
                total += _histogram[p].Count;
                if (total < perGroup)
                {
                    _histogram[p].Group = g;
                    p++;
                }
                else if (total > perGroup)
                {
                    if (total-perGroup > perGroup-total-_histogram[p-1].Count)
                    {
                        if (g + penambah < (groupCount) * penambah)
                        {
                            g+=penambah;
                        }
                        
                        _histogram[p].Group = g;
                        p++; 
                        total = 0;
                    }
                    else
                    {   
                        _histogram[p].Group = g;
                        p++;
                    } 
                }
            }

            foreach (var i in _histogram)
            {
                Console.WriteLine($"byte:{i.Byte} count:{i.Count} group:{i.Group}");
            }

            Console.WriteLine(_histogram.Sum(x=>x.Count));

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    var newColor = _histogram.First(s => s.Byte == pixels[currentLine + x]);
                    pixels[currentLine + x] = (byte)newColor.Group;
                    pixels[currentLine + x + 1] = (byte)newColor.Group;
                    pixels[currentLine + x + 2] = (byte)newColor.Group;
                }
                QuantizationWorker.ReportProgress(1);
            }
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            _bmpQuantization.UnlockBits(bitmapData);
            e.Result = _bmpQuantization;
        }

        private void QuantizationWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void QuantizationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                ImageBox.Image = e.Result as Bitmap; 
                progressBar1.Visible = false;
                changedBytes = false;
            }
           
        }

        private void grayLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedBytes = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _histogramGrap = new HistogramGrap(_histogram);
            _histogramGrap.ShowDialog();
        }
    }
}
