using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class HpfSetting : Form
    {   
        public int[][] HpfSet { get; set; }

        public HpfSetting()
        {
            InitializeComponent();
        }

        public HpfSetting(int[][] hpf)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in hpf)
            {
                if(hpf.Last() == item)
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

                    HpfSet = temp.Select(p => p.ToArray()).ToArray();
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
