using System;
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

        public MedianSetting(int medianLenght)
        {
            InitializeComponent();

            textBox1.Text = medianLenght.ToString();
        }

        private void LPFSetting_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {  
            MedianLenght = int.Parse(textBox1.Text);
            DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
