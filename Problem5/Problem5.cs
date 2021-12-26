using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Problem5
{
    public partial class Problem5 : Form
    {
        const double g = 9.807; // ускорение свободного падения, м/с^2

        public Problem5()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            double V0 = 3000;
            double fi = 30 * 0.017;
            double k = 0.01;

            double X(double t) => V0 * Math.Cos(fi) * (1 - Math.Exp(-k * t)) / k;
            double Y(double t) => ((V0 * Math.Sin(fi) + (g / k)) * (1 - Math.Exp(-k * t)) / k) - (g * t / k);

            var x = new List<double>();
            var y = new List<double>();
            for (double t = 0, j = 0; j >= 0; t++) 
            {
                j = Y(t);
                y.Add(j / 1000);
                x.Add(Math.Round(X(t) / 1000, 1));
            }
            Draw(x, y);
        }

        void Draw(List<double> x, List<double> y)
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "x, км";
            chart1.ChartAreas[0].AxisY.Title = "y, км";
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.Series.Add("V0 = 3 км/с" + '\n' +
                              "fi = 30°" + '\n' +
                              "k = 0.01 1/c" + '\n');
            chart1.Series.Last().ChartType = SeriesChartType.Spline;
            chart1.Series.Last().Points.DataBindXY(x, y);
        }
    }
}
