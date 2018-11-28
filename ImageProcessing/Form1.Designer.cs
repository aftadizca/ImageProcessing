namespace ImageProcessing
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toGrayscaleBW = new System.ComponentModel.BackgroundWorker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.jpegSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RLESize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bmpSize = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.buttonLPF = new System.Windows.Forms.Button();
            this.buttonMedian = new System.Windows.Forms.Button();
            this.buttonHPF = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fILEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oPENToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sAVEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sETTINGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hPFSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lPFSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medianSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HPF = new System.ComponentModel.BackgroundWorker();
            this.LPF = new System.ComponentModel.BackgroundWorker();
            this.Median = new System.ComponentModel.BackgroundWorker();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Quantize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.BackColor = System.Drawing.Color.Black;
            this.ImageBox.Location = new System.Drawing.Point(12, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(680, 389);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.ImageBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(704, 417);
            this.panel1.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Red;
            this.progressBar1.Location = new System.Drawing.Point(12, 401);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(680, 2);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Value = 50;
            this.progressBar1.VisibleChanged += new System.EventHandler(this.progressBar1_VisibleChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toGrayscaleBW
            // 
            this.toGrayscaleBW.WorkerReportsProgress = true;
            this.toGrayscaleBW.WorkerSupportsCancellation = true;
            this.toGrayscaleBW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.toGrayscaleBW_DoWork);
            this.toGrayscaleBW.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.toGrayscaleBW_ProgressChanged);
            this.toGrayscaleBW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.toGrayscaleBW_RunWorkerCompleted);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Quantize);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.jpegSize);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.RLESize);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.bmpSize);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.buttonLPF);
            this.panel2.Controls.Add(this.buttonMedian);
            this.panel2.Controls.Add(this.buttonHPF);
            this.panel2.Location = new System.Drawing.Point(12, 447);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(680, 130);
            this.panel2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(468, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Original";
            // 
            // jpegSize
            // 
            this.jpegSize.Location = new System.Drawing.Point(516, 55);
            this.jpegSize.Name = "jpegSize";
            this.jpegSize.Size = new System.Drawing.Size(161, 20);
            this.jpegSize.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(468, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "RLE";
            // 
            // RLESize
            // 
            this.RLESize.Location = new System.Drawing.Point(516, 29);
            this.RLESize.Name = "RLESize";
            this.RLESize.Size = new System.Drawing.Size(161, 20);
            this.RLESize.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(468, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "BMP";
            // 
            // bmpSize
            // 
            this.bmpSize.Location = new System.Drawing.Point(516, 3);
            this.bmpSize.Name = "bmpSize";
            this.bmpSize.Size = new System.Drawing.Size(161, 20);
            this.bmpSize.TabIndex = 24;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(3, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(161, 37);
            this.button4.TabIndex = 20;
            this.button4.Text = "DEFAULT";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            // 
            // buttonLPF
            // 
            this.buttonLPF.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonLPF.FlatAppearance.BorderSize = 0;
            this.buttonLPF.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.buttonLPF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLPF.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLPF.Location = new System.Drawing.Point(170, 3);
            this.buttonLPF.Name = "buttonLPF";
            this.buttonLPF.Size = new System.Drawing.Size(161, 37);
            this.buttonLPF.TabIndex = 23;
            this.buttonLPF.Text = "LPF";
            this.buttonLPF.UseVisualStyleBackColor = false;
            this.buttonLPF.Click += new System.EventHandler(this.button3_Click);
            this.buttonLPF.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            // 
            // buttonMedian
            // 
            this.buttonMedian.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonMedian.FlatAppearance.BorderSize = 0;
            this.buttonMedian.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.buttonMedian.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMedian.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMedian.Location = new System.Drawing.Point(170, 46);
            this.buttonMedian.Name = "buttonMedian";
            this.buttonMedian.Size = new System.Drawing.Size(161, 37);
            this.buttonMedian.TabIndex = 22;
            this.buttonMedian.Text = "MEDIAN";
            this.buttonMedian.UseVisualStyleBackColor = false;
            this.buttonMedian.Click += new System.EventHandler(this.button2_Click);
            this.buttonMedian.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            // 
            // buttonHPF
            // 
            this.buttonHPF.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonHPF.FlatAppearance.BorderSize = 0;
            this.buttonHPF.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.buttonHPF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHPF.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHPF.Location = new System.Drawing.Point(3, 46);
            this.buttonHPF.Name = "buttonHPF";
            this.buttonHPF.Size = new System.Drawing.Size(161, 37);
            this.buttonHPF.TabIndex = 21;
            this.buttonHPF.Text = "HPF";
            this.buttonHPF.UseVisualStyleBackColor = false;
            this.buttonHPF.Click += new System.EventHandler(this.button1_Click);
            this.buttonHPF.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Gray;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fILEToolStripMenuItem,
            this.sETTINGToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(704, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fILEToolStripMenuItem
            // 
            this.fILEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oPENToolStripMenuItem,
            this.sAVEToolStripMenuItem});
            this.fILEToolStripMenuItem.Name = "fILEToolStripMenuItem";
            this.fILEToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fILEToolStripMenuItem.Text = "FILE";
            // 
            // oPENToolStripMenuItem
            // 
            this.oPENToolStripMenuItem.Name = "oPENToolStripMenuItem";
            this.oPENToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.oPENToolStripMenuItem.Text = "OPEN";
            this.oPENToolStripMenuItem.Click += new System.EventHandler(this.oPENToolStripMenuItem_Click);
            // 
            // sAVEToolStripMenuItem
            // 
            this.sAVEToolStripMenuItem.Name = "sAVEToolStripMenuItem";
            this.sAVEToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sAVEToolStripMenuItem.Text = "SAVE AS RLE";
            this.sAVEToolStripMenuItem.Click += new System.EventHandler(this.sAVEToolStripMenuItem_Click);
            // 
            // sETTINGToolStripMenuItem
            // 
            this.sETTINGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hPFSettingToolStripMenuItem,
            this.lPFSettingToolStripMenuItem,
            this.medianSettingToolStripMenuItem});
            this.sETTINGToolStripMenuItem.Name = "sETTINGToolStripMenuItem";
            this.sETTINGToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.sETTINGToolStripMenuItem.Text = "SETTING";
            this.sETTINGToolStripMenuItem.Click += new System.EventHandler(this.sETTINGToolStripMenuItem_Click);
            // 
            // hPFSettingToolStripMenuItem
            // 
            this.hPFSettingToolStripMenuItem.Name = "hPFSettingToolStripMenuItem";
            this.hPFSettingToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.hPFSettingToolStripMenuItem.Text = "HPF Setting";
            this.hPFSettingToolStripMenuItem.Click += new System.EventHandler(this.hPFSettingToolStripMenuItem_Click);
            // 
            // lPFSettingToolStripMenuItem
            // 
            this.lPFSettingToolStripMenuItem.Name = "lPFSettingToolStripMenuItem";
            this.lPFSettingToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.lPFSettingToolStripMenuItem.Text = "LPF Setting";
            this.lPFSettingToolStripMenuItem.Click += new System.EventHandler(this.lPFSettingToolStripMenuItem_Click);
            // 
            // medianSettingToolStripMenuItem
            // 
            this.medianSettingToolStripMenuItem.Name = "medianSettingToolStripMenuItem";
            this.medianSettingToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.medianSettingToolStripMenuItem.Text = "Median Setting";
            this.medianSettingToolStripMenuItem.Click += new System.EventHandler(this.medianSettingToolStripMenuItem_Click);
            // 
            // HPF
            // 
            this.HPF.WorkerReportsProgress = true;
            this.HPF.WorkerSupportsCancellation = true;
            this.HPF.DoWork += new System.ComponentModel.DoWorkEventHandler(this.HPF_DoWork);
            this.HPF.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.HPF_ProgressChanged);
            this.HPF.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.HPF_RunWorkerCompleted);
            // 
            // LPF
            // 
            this.LPF.WorkerReportsProgress = true;
            this.LPF.WorkerSupportsCancellation = true;
            this.LPF.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LPF_DoWork);
            this.LPF.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.LPF_ProgressChanged);
            this.LPF.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.LPF_RunWorkerCompleted);
            // 
            // Median
            // 
            this.Median.WorkerReportsProgress = true;
            this.Median.WorkerSupportsCancellation = true;
            this.Median.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Median_DoWork);
            this.Median.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Median_ProgressChanged);
            this.Median.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Median_RunWorkerCompleted);
            // 
            // Quantize
            // 
            this.Quantize.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Quantize.FlatAppearance.BorderSize = 0;
            this.Quantize.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
            this.Quantize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Quantize.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quantize.Location = new System.Drawing.Point(170, 87);
            this.Quantize.Name = "Quantize";
            this.Quantize.Size = new System.Drawing.Size(161, 37);
            this.Quantize.TabIndex = 30;
            this.Quantize.Text = "Quantization";
            this.Quantize.UseVisualStyleBackColor = false;
            this.Quantize.Click += new System.EventHandler(this.Quantize_Click);
            this.Quantize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 583);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker toGrayscaleBW;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonHPF;
        private System.Windows.Forms.Button buttonLPF;
        private System.Windows.Forms.Button buttonMedian;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fILEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oPENToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sAVEToolStripMenuItem;
        private System.Windows.Forms.Button button4;
        private System.ComponentModel.BackgroundWorker HPF;
        private System.ComponentModel.BackgroundWorker LPF;
        private System.Windows.Forms.ToolStripMenuItem sETTINGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lPFSettingToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker Median;
        private System.Windows.Forms.ToolStripMenuItem medianSettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hPFSettingToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RLESize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox bmpSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox jpegSize;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button Quantize;
    }
}

