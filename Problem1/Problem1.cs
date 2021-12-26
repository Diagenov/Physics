using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Problem1
{
    public partial class Problem1 : Form
    {
        double c => 3 * Math.Pow(10, 8); // скорость света
        double h => 6.6 * Math.Pow(10, -34); // постоянная Планка
        double k => 1.38 * Math.Pow(10, -23); // постоянная Больцмана

        public Problem1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "λ, мкм";
            chart1.ChartAreas[0].AxisY.Title = "r, Дж/м^2 * 10^12";

            double r(double lymbda, double T) => 2 * Math.PI * c * c * h / Math.Pow(lymbda, 5) / Math.Exp(h * c / k / T / lymbda);

            var x = new List<double>();
            var y = new List<double>();
            for (float i = 0; i <= 10; i += 0.1f)
            {
                x.Add(Math.Round(i, 2));
                y.Add(r(i * (float)Math.Pow(10, -6), 1000) * 0.000000000001);
            }
            Draw(x, y, Color.Red, 1000);

            x.Clear();
            y.Clear();
            for (float i = 0; i <= 10; i += 0.1f)
            {
                x.Add(Math.Round(i, 2));
                y.Add(r(i * (float)Math.Pow(10, -6), 2000) * 0.000000000001);
            }
            Draw(x, y, Color.Blue, 2000);
        }

        void Draw(List<double> x, List<double> y, Color color, int T)
        {
            chart1.Series.Add($"T = {T} K");
            chart1.Series.Last().Color = color;
            chart1.Series.Last().ChartType = SeriesChartType.Spline;
            chart1.Series.Last().Points.DataBindXY(x, y);
        }
    }
}
