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
    public partial class MedianSetting : Form
    {   
        public int MedianLenght { get; set; }

        public MedianSetting()
        {
            InitializeComponent();
        }

        public MedianSetting(int MedianLenght)
        {
            InitializeComponent();

            textBox1.Text = MedianLenght.ToString();
        }

        private void LPFSetting_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {  
            this.MedianLenght = int.Parse(textBox1.Text);
            this.DialogResult = DialogResult.OK;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
