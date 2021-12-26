using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Problem3
{
    public partial class Problem3 : Form
    {
        public Problem3()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs _e)
        {
            Worker(true, chart1);
            Worker(false, chart2);
        }

        void Worker(bool rect, Chart chart)
        {
            chart.Series.Clear();

            double a = 6; // ширина ямы, A
            double dx = 0.01; // приращение x
            double dE = 0.05; // приращение E

            double u(double x) // задает вид ямы
            {
                if (x > 0 && x < a)
                    return rect ? -20 : -2 - x * 3;
                return 0;
            }
            double alpha(double x, double e) => Math.Sqrt(0.26 * (u(x) - e)); // коэффициент из уравнения Шредингера
            double sqr_alpha(double x, double e) => 0.26 * (u(x) - e);

            double w; // w = dy/dx
            var yI = new List<Vector>(); // область I
            var yII = new List<Vector>(); // область II
            var yIII = new List<Vector>(); // область III
            for (double E = -20, _E = -30; E < 0; E += dE) // E = -20 (начальное значение энергии)
            {
                for (double x = -4; x < 0; x += dx)
                    yI.Add(new Vector(x, Math.Exp(alpha(x, E) * x))); // в области I имеем точное решение

                for (double x = a; x < 10; x += dx)
                    yIII.Add(new Vector(x, Math.Exp(-alpha(x, E) * (x - a)))); // в области III тоже

                yII.Add(new Vector(0, 1)); // начальные значения и условия сшивания I и II
                w = alpha(0, E);
                for (double x = dx; x < a; x += dx) // в области II получаем численное решение
                {
                    w = w + sqr_alpha(x, E) * yII.Last().Y * dx;
                    yII.Add(new Vector(x, yII.Last().Y + w * dx));
                }

                if (Math.Abs(w / yII.Last().Y + alpha(a, E)) < alpha(a, E) * 0.1 && E - _E > dE * 10) // условия сшивания II и III (равенство производных w = dy/dx)
                {
                    var x = new List<double>();
                    var y = new List<double>();
                    foreach (var i in yI.Concat(yII).Concat(yIII))
                    {
                        x.Add(Math.Round(i.X, 2));
                        y.Add(Math.Round(i.Y, 2));
                    }
                    Draw(x, y, E, chart);
                    _E = E;
                }

                yI.Clear();
                yII.Clear();
                yIII.Clear();
            }
        }

        void Draw(List<double> x, List<double> y, double E, Chart chart)
        {
            chart.ChartAreas[0].AxisX.Title = "x, Å";
            chart.ChartAreas[0].AxisY.Title = "Ѱ, эВ";
            chart.Series.Add($"E = {Math.Round(E, 2)} эВ");
            chart.Series.Last().ChartType = SeriesChartType.Spline;
            chart.Series.Last().Points.DataBindXY(x, y);
        }

        struct Vector
        {
            public double X;
            public double Y;

            public Vector(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
