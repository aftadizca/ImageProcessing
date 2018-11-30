using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class HistogramGrap : Form
    {
        public List<Histogram> Data { get; set; }

        public HistogramGrap()
        {
            InitializeComponent(); 
        }

        public HistogramGrap(List<Histogram> data)
        {
            InitializeComponent();
            Data = data;
            int sum = data.Sum(x => x.Count);
            var group = data.Select(x => x.Group).Distinct().ToArray();
            Console.WriteLine(String.Join(" ",group));

            int penambah = 255 / (group.Length - 1);
            foreach (var d in data)
            {
                chart1.Series[0].Points.AddXY(d.Byte,(double) d.Count/sum*100);
            }

            foreach (var g in @group)
            {
                chart1.Series[1].Points.AddXY(
                    g / penambah, 
                    (double) data.Where(x=>x.Group==g).Sum(x=>x.Count)/sum*100); 
            }
        }
    }
}
