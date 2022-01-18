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
        const double Rz = 6371000; // радиус Земли, м

        public Problem5()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            Trajectory1(3000, 30, 0.01);
            Earth();
        }

        void Trajectory1(double V0, double fi, double k)
        {
            fi = fi * Math.PI / 180;

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
            Draw1(x, y);
        }

        void Trajectory2(double V, double H, double alpha)
        {
            double Mod(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);
            double A(double x, double l) => -4 * Math.Pow(10, 14) * x / Math.Pow(Mod(x, l), 3);

            double r = 0;
            double dt = Rz / V / 20;
            Vector V1 = new Vector(), V2 = new Vector();
            Vector A1 = new Vector(), A2 = new Vector();

            var X = new List<double>();
            var Y = new List<double>();

            X.Add(0);
            Y.Add(H + Rz);
            alpha = alpha * Math.PI / 180;
            V1.X = V * Math.Cos(alpha);
            V1.Y = V * Math.Sin(alpha);
            A1.X = A(X[0], Y[0]);
            A1.Y = A(Y[0], X[0]);

            do
            {
                V2.X = V1.X + A1.X * dt;
                V2.Y = V1.Y + A1.Y * dt;

                double x = X.Last() + (V1.X + V2.X) * dt / 2;
                double y = Y.Last() + (V1.Y + V2.Y) * dt / 2;

                A2.X = A(x, y);
                A2.Y = A(y, x);

                V2.X = V1.X + (A1.X + A2.X) * dt / 2;
                V2.Y = V1.Y + (A1.Y + A2.Y) * dt / 2;

                x = X.Last() + (V1.X + V2.X) * dt / 2;
                y = Y.Last() + (V1.Y + V2.Y) * dt / 2;

                V1 = V2;
                A1.X = A(x, y);
                A1.Y = A(y, x);

                r += Mod(x - X.Last(), y - Y.Last());
                X.Add(x);
                Y.Add(y);
            }
            while (r < 100 * Rz && Mod(X.Last(), Y.Last()) > Rz);
            Draw2(X, Y, V);
        }

        void Draw1(List<double> x, List<double> y)
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

        void Draw2(List<double> x, List<double> y, double V0)
        {
            var X = new List<double>();
            var Y = new List<double>();
            for (int i = 0; i < x.Count; i++)
            {
                X.Add(x[i] / 1000);
                Y.Add(y[i] / 1000);
            }
            var max = Math.Max(X.Max(), Y.Max());
            var min = Math.Min(X.Min(), Y.Min());
            var size = (int)Math.Max(max, Math.Abs(min)) + 1000;
            
            if (chart2.ChartAreas[0].AxisX.Maximum < size)
            {
                chart2.ChartAreas[0].AxisX.Maximum = size;
                chart2.ChartAreas[0].AxisX.Minimum = -size;
                chart2.ChartAreas[0].AxisY.Maximum = size;
                chart2.ChartAreas[0].AxisY.Minimum = -size;
            }

            chart2.ChartAreas[0].AxisX.Title = "x, км";
            chart2.ChartAreas[0].AxisY.Title = "y, км";
            chart2.Series.Add($"V0 = {V0 / 1000} км/с" + '\n' +
                              "H = 2 км" + '\n' +
                              "alpha = 0°" + '\n');
            chart2.Series.Last().ChartType = SeriesChartType.Spline;
            chart2.Series.Last().Points.DataBindXY(X, Y);
            
        }

        void Earth()
        {
            chart2.Series.Clear();
            var x = new List<double>();
            var y = new List<double>();
            for (int fi = 0; fi <= 360; fi += 5)
            {
                double alpha = fi * Math.PI / 180;
                x.Add(Rz * Math.Cos(alpha) / 1000);
                y.Add(Rz * Math.Sin(alpha) / 1000);
            }
            chart2.Series.Add("Земля");
            chart2.Series.Last().ChartType = SeriesChartType.Spline;
            chart2.Series.Last().Points.DataBindXY(x, y);
        }

        public struct Vector
        {
            public double X;
            public double Y;
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out double V0))
                return;

            Trajectory2(V0 * 1000, 2000, 0);
        }
    }
}
