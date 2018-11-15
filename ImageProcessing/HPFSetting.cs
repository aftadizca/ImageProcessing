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
    public partial class HPFSetting : Form
    {   
        public int[][] HPFSet { get; set; }

        public HPFSetting()
        {
            InitializeComponent();
        }

        public HPFSetting(int[][] HPF)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in HPF)
            {
                if(HPF.Last() == item)
                {
                    sb.Append(string.Join(",", item));
                }
                else
                {
                    sb.Append(string.Join(",", item));
                    sb.Append(Environment.NewLine);
                }
                
            }
            
            InitializeComponent();

            textBox1.Text = sb.ToString();
        }

        private void LPFSetting_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<List<int>> temp = new List<List<int>>();

            var msg = textBox1.Text.Split('\n');

            for(int i=0; i<msg.Length; i++)
            {
                var item = msg[i].Split(',');
                if (item.Length > 1)
                {
                    Console.WriteLine(item.Length);
                    var intArr = Array.ConvertAll(item, x => int.Parse(x)).ToList();
                    temp.Add(intArr);

                    this.HPFSet = temp.Select(p => p.ToArray()).ToArray();
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
