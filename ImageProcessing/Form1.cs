using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    
    public partial class Form1 : Form
    {

        //LPF pakai 1/8
        //HPF pakai 1/9

        private Bitmap bmpAwal;
        private Bitmap bmpOutput;
        private int bmpW;
        private int bmpH;

        public void ShowProgressBar(int max)
        {
            progressBar1.Visible = true;
            progressBar1.Maximum = max;
        }

        public Form1()
        {
            InitializeComponent();
            progressBar1.Visible = false;
            button4.Focus();
        } 

        private void toGrayscaleBW_DoWork(object sender, DoWorkEventArgs e)
        {
            if (toGrayscaleBW.IsBusy)
            {
                toGrayscaleBW.CancelAsync();
            }
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
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(bmpAwal.Height);
            ShowProgressBar(bmpH - 1);
            var bmp = new Bitmap(bmpAwal);
            HPF.RunWorkerAsync(argument: bmp);
        }

        private void HPF_DoWork(object sender, DoWorkEventArgs e)
        {
            if (HPF.IsBusy)
            {
                HPF.CancelAsync();
            }

            var bmp = e.Argument as Bitmap;
            for (int h = 0; h < bmp.Height; h++)
            {
                var wX = 0;
                for (int w = 0; w < bmp.Width; w++)
                {
                    wX++;
                    int pixel = 0;

                    if ( h == 0 && w ==0 )  //kiri atas
                    {
                       // Console.WriteLine("kiriatas");
                        pixel = (bmp.GetPixel(w, h).R*4 - bmp.GetPixel(w + 1, h + 1).R)/9;
                    } 
                    else if (h == 0 && w == bmp.Width - 1)  // kanan atas
                    {   
                        //Console.WriteLine("kananatas");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w - 1, h + 1).R) / 9;
                    }
                    else if(h==0 && w>0) //tengah atas
                    {
                       // Console.WriteLine("tengahatas");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w + 1, h + 1).R - bmp.GetPixel(w - 1, h + 1).R) / 9;
                    }
                    else if (h == bmp.Height - 1 && w == 0)  // kiri bawah
                    {
                       // Console.WriteLine("kiribawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w + 1, h - 1).R) / 9;
                    } 
                    else if (h == bmp.Height - 1 && w == bmp.Width - 1) // kanan bawah
                    {
                       // Console.WriteLine("kiribawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w - 1, h - 1).R) / 9;
                    }
                    else if (h == bmp.Height - 1 && w > 0) //tengah bawah
                    {
                        //Console.WriteLine("tengahbawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w - 1, h - 1).R - bmp.GetPixel(w + 1, h - 1).R) / 9;
                    }
                    else if (h > 0 && w == 0)  // kiri tengah
                    {
                        //Console.WriteLine("kiritengah");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w + 1, h + 1).R - bmp.GetPixel(w + 1, h - 1).R) / 9;
                    }
                    else if (h > 0 && w == bmp.Width - 1)  // kanan tengah
                    {
                        //Console.WriteLine("kanantengah");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w - 1, h + 1).R - bmp.GetPixel(w - 1, h - 1).R) / 9;
                    }
                    else
                    {
                        //Console.WriteLine("biasa");
                        pixel = (bmp.GetPixel(w, h).R * 4 - bmp.GetPixel(w - 1, h + 1).R - bmp.GetPixel(w - 1, h - 1).R - bmp.GetPixel(w + 1, h - 1).R - bmp.GetPixel(w + 1, h + 1).R) / 9;
                    }

                    
                    if(pixel < 0)
                    {
                        pixel = 0;
                    }
                   // Console.WriteLine(pixel);

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
            if (LPF.IsBusy)
            {
                LPF.CancelAsync();
            }

            var bmp = e.Argument as Bitmap;
            for (int h = 0; h < bmp.Height; h++)
            {
                var wX = 0;
                for (int w = 0; w < bmp.Width; w++)
                {
                    wX++;
                    int pixel = 0;

                    if (h == 0 && w == 0)  //kiri atas
                    {
                        // Console.WriteLine("kiriatas");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h + 1).R + bmp.GetPixel(w + 1, h).R ) / 8;
                    }
                    else if (h == 0 && w == bmp.Width - 1)  // kanan atas
                    {
                        //Console.WriteLine("kananatas");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h + 1).R + bmp.GetPixel(w - 1, h).R ) / 8;
                    }
                    else if (h == 0 && w > 0) //tengah atas
                    {
                        // Console.WriteLine("tengahatas");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w + 1, h).R + bmp.GetPixel(w - 1, h).R + bmp.GetPixel(w, h+1).R) / 8;
                    }
                    else if (h == bmp.Height - 1 && w == 0)  // kiri bawah
                    {
                        // Console.WriteLine("kiribawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h - 1).R + bmp.GetPixel(w + 1, h).R ) / 8;
                    }
                    else if (h == bmp.Height - 1 && w == bmp.Width - 1) // kanan bawah
                    {
                        // Console.WriteLine("kananbawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h - 1).R + bmp.GetPixel(w - 1, h).R) / 8;
                    }
                    else if (h == bmp.Height - 1 && w > 0) //tengah bawah
                    {
                        //Console.WriteLine("tengahbawah");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w + 1, h).R + bmp.GetPixel(w - 1, h).R + bmp.GetPixel(w, h - 1).R) / 8;
                    }
                    else if (h > 0 && w == 0)  // kiri tengah
                    {
                        //Console.WriteLine("kiritengah");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w , h + 1).R + bmp.GetPixel(w, h - 1).R + bmp.GetPixel(w + 1,h).R) / 8;
                    }
                    else if (h > 0 && w == bmp.Width - 1)  // kanan tengah
                    {
                        //Console.WriteLine("kanantengah");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h + 1).R + bmp.GetPixel(w, h - 1).R + bmp.GetPixel(w - 1, h).R) / 8;
                    }
                    else
                    {
                        //Console.WriteLine("biasa");
                        pixel = (bmp.GetPixel(w, h).R * 4 + bmp.GetPixel(w, h + 1).R + bmp.GetPixel(w, h - 1).R + bmp.GetPixel(w - 1 , h).R + bmp.GetPixel(w + 1, h).R) / 8;
                    }

                    // Console.WriteLine(pixel);

                    bmp.SetPixel(w, h, Color.FromArgb(pixel, pixel, pixel));
                }

                LPF.ReportProgress(h);
            }
            e.Result = bmp;
        }

        private void LPF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void LPF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else
            progressBar1.Visible = false;
            ImageBox.Image = e.Result as Bitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowProgressBar(bmpH - 1);
            var bmp = new Bitmap(bmpAwal);
            LPF.RunWorkerAsync(argument: bmp);
        }
    }
}
