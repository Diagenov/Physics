using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Problem6
{
    public partial class Problem6 : Form
    {
        public Problem6()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out double A) || !double.TryParse(textBox2.Text, out double T))
                return;

            int N = 1000;
            double dt = T / N;

            double f(int i)
            {
                double p = N * dt / 4;
                if (i <= N / 2)
                    return A * (p - Math.Abs(i * dt - p)); 
                return (Math.Abs(i * dt - (3 * p)) - p) * A; 
            }

            var arr = new List<Harmonic>();
            for (int n = 0; n < 100; n++)
            {
                double a = 0, b = 0;
                for (int i = 1; i <= N; i++)
                {
                    a += f(i) * Math.Cos(2 * Math.PI * i * n / N); 
                    b += f(i) * Math.Sin(2 * Math.PI * i * n / N);
                }
                arr.Add(new Harmonic(2 * a / N, 2 * b / N, n));
            }

            textBox3.Text =
                $"Амплитуда A = {A}" + "\r\n" +
                $"Период T = {T}" + "\r\n\r\n" +
                $"ГАРМОНИКИ: {string.Join("", arr.Select(i => $"\r\n{i.i}) a = {i.a}\r\n     b = {i.b}"))}" + "\r\n\r\n" +
                $"АМЛИТУДЫ: {string.Join("", arr.Select(i => $"\r\n{i.i}) A = {i.A}"))}" + "\r\n\r\n" + 
                $"Основной тон max(A) = {arr.Max(i => i.A)}";

            double w = 2 * Math.PI / T;
            var x = new List<double>();
            var y = new List<double>();
            for (double t = 0; t < T; t += dt)
            {
                double _y = arr[0].A / 2;
                for (int n = 1; n < arr.Count; n++)
                    _y += arr[n].a * Math.Cos(n * w * t) + arr[n].b * Math.Sin(n * w * t); 
                x.Add(t);
                y.Add(_y);
            }
            Draw(x, y, A, T);
        }

        void Draw(List<double> x, List<double> y, double A, double T)
        {
            chart1.ChartAreas[0].AxisX.Title = "t";
            chart1.ChartAreas[0].AxisY.Title = "y";
            chart1.Series.Add($"A = {A}" + '\n' +
                              $"T = {T}" + '\n');
            chart1.Series.Last().ChartType = SeriesChartType.Line;
            chart1.Series.Last().Points.DataBindXY(x, y);
        }

        struct Harmonic
        {
            public double a;
            public double b;
            public int i;

            public double A => Math.Sqrt(a * a + b * b);

            public Harmonic(double a, double b, int i)
            {
                this.a = a;
                this.b = b;
                this.i = i;
            }
        }
    }
}
