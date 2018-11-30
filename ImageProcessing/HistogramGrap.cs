using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ImageProcessing
{
    public partial class HistogramGrap : Form
    {
        public List<Histogram> data { get; set; }

        public HistogramGrap()
        {
            InitializeComponent(); 
        }

        public HistogramGrap(List<Histogram> data)
        {
            InitializeComponent();
            this.data = data;
            int sum = data.Sum(x => x.Count);
            var group = data.Select(x => x.Group).Distinct().ToArray();
            Console.WriteLine(String.Join(" ",group));

            int penambah = 255 / (group.Length - 1);
            if (data != null)
            {  
                foreach (var d in data)
                {
                    chart1.Series[0].Points.AddXY(d.Byte,(double) d.Count/sum*100);
                }

                foreach (var g in group)
                {
                    chart1.Series[1].Points.AddXY(g/penambah,(double) data.Where(x=>x.Group==g).Sum(x=>x.Count)/sum*100); 
                }
            }
        }
    }
}
