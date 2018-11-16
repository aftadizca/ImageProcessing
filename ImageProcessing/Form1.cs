using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        private string msg; 

        private int[][] HPFSet = new int[][] { new int[] { -1,-1,-1 },
                                               new int[] { -1,8,-1 },
                                               new int[] { -1,-1,-1 }};

        private int[][] LPFSet = new int[][] { new int[] { 1,1,1 },
                                               new int[] { 1,1,1 },
                                               new int[] { 1,1,1 }};

        private int MedianLenght = 3;


        public int LPFGetPixel(int[][] set, Bitmap bmp, int h, int w)
        {
            int total = set.Sum(arr => arr.Sum());
            int n = set.Length/2;
            int pixel = 0;
            int sety = 0;
            for (int y = -n; y<=n; y++)
            {
                int setx = 0;
                for (int x = -n; x <=n; x++)
                {   
                     if( (h+y >= 0 && h + y <= bmp.Height - 1) && (w+x >= 0 && w + x <= bmp.Width - 1))
                    {
                        if (set[sety][setx] != 0)
                        {
                            pixel += bmp.GetPixel(w + x, h + y).R * set[sety][setx];
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
            else if (a>255)
            {
                a = 255;
            }

            return a ;

        }

        public int HPFGetPixel(int[][] set, Bitmap bmp, int h, int w)
        {
            int total = 9;
            int n = set.Length / 2;
            int pixel = 0;
            int sety = 0;
            for (int y = -n; y <= n; y++)
            {
                int setx = 0;
                for (int x = -n; x <= n; x++)
                {
                    if ((h + y >= 0 && h + y <= bmp.Height - 1) && (w + x >= 0 && w + x <= bmp.Width - 1))
                    {
                        if (set[sety][setx] != 0)
                        {
                            pixel += bmp.GetPixel(w + x, h + y).R * set[sety][setx];
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

            return a;

        }

        public int MedianGetPixel(Bitmap bmp, int h, int w, int nMatrix)
        {
            int n = nMatrix / 2;
            List<int> pixel = new List<int>();

            for (int y = -n; y <= n; y++)
            {
                for (int x = -n; x <= n; x++)
                {
                    if ((h + y >= 0 && h + y <= bmp.Height - 1) && (w + x >= 0 && w + x <= bmp.Width - 1))
                    {
                        pixel.Add(bmp.GetPixel(w + x, h + y).R);
                    }
                }
            }
            pixel = pixel.OrderBy(x => x).ToList();
            return pixel[(pixel.Count()) / 2];
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

        public void HPF_Parallel(Bitmap bmp, int[][] set)
        {
            
            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0; 

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
                          var a = pixel / 9;
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
                      HPF.ReportProgress(1);

                  });
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

            //for (int i = 0; i < bmp.Height; i++)
            //{   
            //    var w = 0;
            //    for (int j = 0; j < bmp.Width; j++)
            //    {
            //        w++;
            //        var Average = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;
            //        bmp.SetPixel(j, i, Color.FromArgb(Average, Average, Average));
            //    }
            //    toGrayscaleBW.ReportProgress(i);
            //}

            e.Result = bmp;
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Grayscale processes in {sw.ElapsedMilliseconds} ms");
            Console.ResetColor();
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

        }

        private void button1_Click(object sender, EventArgs e)
        {     
            try{  
                if(bmpHPF == null)
                {
                    ShowProgressBar(bmpAwal.Height);
                    bmpHPF = new Bitmap(bmpAwal);
                    HPF.RunWorkerAsync(argument: bmpHPF);
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
            if (HPF.IsBusy)
            {
                HPF.CancelAsync();
            }
            var bmp = e.Argument as Bitmap;

            HPF_Parallel(bmp, HPFSet);

            #region Alternatives
            //
            //for (int h = 0; h < bmp.Height; h++) { 
            //    for (int w = 0; w < bmp.Width; w++)
            //    {  
            //        int pixel = HPFGetPixel(HPFSet,bmp,h,w);

            //        bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
            //    }

            //    HPF.ReportProgress(h);
            //} 
            #endregion

            e.Result = bmp;
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
            ImageBox.Image = e.Result as Bitmap; 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ImageBox.Image = bmpAwal;
        }

        private void LPF_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (LPF.IsBusy)
            //{
            //    LPF.CancelAsync();
            //}

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var bmp = e.Argument as Bitmap;

            LPF_Parallel(bmp, LPFSet);

            #region Sequential Version
            //for (int h = 0; h < bmp.Height; h++)
            //{
            //    for (int w = 0; w < bmp.Width; w++)
            //    {

            //        int pixel = LPFGetPixel(LPFSet, bmp, h, w);

            //        bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
            //    }

            //    LPF.ReportProgress(h);
            //} 
            #endregion

            e.Result = bmp;
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Low Passing Filter Processed in {sw.ElapsedMilliseconds} ms");
            Console.ResetColor();
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
                   
                    LPF.RunWorkerAsync(argument: bmpLPF);  
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
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
