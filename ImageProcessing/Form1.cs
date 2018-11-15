using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private BackgroundWorker[] LPFthread = new BackgroundWorker[10];
        private int BGCount = 0;


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
            return pixel[(pixel.Count())/2];
        }

        public void ShowProgressBar(int max)
        {
            progressBar1.Visible = true;
            progressBar1.Maximum = max;
            progressBar1.Value = 0;
        }

        public void LPF_ThreadStart()
        {
            var sisa = bmpAwal.Height % 10;
            var perThread = bmpAwal.Height / 10;

            List<object> arg = new List<object>();
            arg.Add(0);
            arg.Add(0);
            for (int i = 0; i < 10; i++)
            {
                
                arg[1] = (int)arg[1] + perThread;
                if(sisa != 0) { arg[1] = (int)arg[1] + sisa; }
                LPFthread[i] = new BackgroundWorker();
                LPFthread[i].DoWork += new DoWorkEventHandler(LPF_DoWork);
                LPFthread[i].RunWorkerCompleted += new RunWorkerCompletedEventHandler(LPF_RunWorkerCompleted);
                LPFthread[i].RunWorkerAsync(arg);
                arg[0] =  (int)arg[1] + 1;
                Console.WriteLine("Start{0}",i);
            }

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
            var bmp = e.Argument as Bitmap; 
            for (int i = 0; i < bmp.Height; i++)
            {   
                var w = 0;
                for (int j = 0; j < bmp.Width; j++)
                {
                    w++;
                    var Average = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;
                    bmp.SetPixel(j, i, Color.FromArgb(Average, Average, Average));
                }
                toGrayscaleBW.ReportProgress(i);
            }
            e.Result = bmp;
        }

        private void toGrayscaleBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {  
           progressBar1.Value = e.ProgressPercentage;
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
                ShowProgressBar(bmpH-1);
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
                    ShowProgressBar(bmpAwal.Height - 1);
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
            for (int h = 0; h < bmp.Height; h++) { 
                for (int w = 0; w < bmp.Width; w++)
                {  
                    int pixel = HPFGetPixel(LPFSet,bmp,h,w);

                    bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
                }

                HPF.ReportProgress(h);
            }
            e.Result = bmp;
        }

        private void HPF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
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

            //var bmp = e.Argument as Bitmap;
            var arg = e.Argument as List<object>;
            

            var bmp = this.bmpLPF;

            for (int h = (int)arg[0]; h < (int)arg[1]; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {

                    int pixel = LPFGetPixel(LPFSet, bmp, h, w);

                    bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
                }

                //LPF.ReportProgress(h);
            }

            //e.Result = bmp;
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
            if(BGCount == 10)
            {
                progressBar1.Visible = false;
                BGCount = 0;
                ImageBox.Image = bmpLPF;
                progressBar1.Value += 10;
            }
            else
            {
                BGCount++;
            }
            Console.WriteLine("Bgcount: {0}", BGCount);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (bmpLPF == null)
                {  
                    ShowProgressBar(100);
                    bmpLPF = new Bitmap(bmpAwal);

                    LPF_ThreadStart();
                   
                    //LPF.RunWorkerAsync(argument: bmpLPF);  
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
            if (Median.IsBusy)
            {
                Median.CancelAsync();
            }

            var bmp = e.Argument as Bitmap;
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {    
                    int pixel = MedianGetPixel(bmp, h, w, MedianLenght); 

                    bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
                }

                Median.ReportProgress(h);
            }
            e.Result = bmp;
        }

        private void Median_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
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
                    ShowProgressBar(bmpAwal.Height - 1);
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
                    bmpLPF = null;
                    button3.PerformClick();
                }
            }
           
        }
    }
}
