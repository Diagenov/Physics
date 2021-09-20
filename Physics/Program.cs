using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;

namespace Physics
{
    class Program
    {
        const double g = 9.807; // ускорение свободного падения, м/с^2

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Worker();
                Console.ReadKey();
            }
        }

        static void Worker()
        {
            Console.WriteLine("= = = = = Построение траектории полета снаряда с учетом сопротивления воздуха = = = = =\n");

            Console.Write("Укажите начальную скорость (м/с): ");
            var V0 = ReadNum(); // 3000

            Console.Write("Укажите угол к горизонту, под которым снаряд будет вылетать (°): ");
            var angle = ReadNum(); // 30°
            var fi = angle * 0.017; // переводятся в радианы

            Console.Write("Укажите коэффициент сопротивления воздуха (1/с): ");
            var k = ReadNum(); // 0,01

            double X(double t) => V0 * Math.Cos(fi) * (1 - Math.Exp(-k * t)) / k;
            double Y(double t) => ((V0 * Math.Sin(fi) + (g / k)) * (1 - Math.Exp(-k * t)) / k) - (g * t / k);

            int time = 0;
            var points = new List<PointF>();
            for (float y;; time++) // Ищем точки с ходом в одну секунду
            {
                y = (float)Y(time);
                if (y < 0)
                    break;
                points.Add(new PointF((float)X(time), y)); 
            }
            Draw(points.ToArray(), V0, angle, k, time);
            Console.WriteLine("Готово!\n\nНажмите любую клавишу.");
        }

        static void Draw(PointF[] points, double V0, double fi, double k, int t)
        {
            string directory = Path.Combine("Graphics");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string path = Path.Combine("Graphics", $"graphic_{Directory.GetFiles("Graphics").Length}.jpg");

            var image = new Bitmap(1200, 1200); 
            float maxX = points.Max(i => i.X),
                  maxY = points.Max(i => i.Y),
                  size;
            if (maxX > maxY)
                size = maxX / 1200;
            else
                size = maxY / 1000;
            using (var graphic = Graphics.FromImage(image))
            {
                graphic.Clear(Color.White);
                string values = $"Начальная скорость {V0} м/с" + '\n' +
                                $"Угол к горизонту {fi}°" + '\n' +
                                $"Коэффициент сопротивления воздуха {k} 1/с" + '\n' +
                                $"Время полета {t} с" + '\n' +
                                $"Дальность полета {maxX} м" + '\n' + 
                                $"Высота полета {maxY} м";
                graphic.DrawString(values, new Font("Calibri", 15f, FontStyle.Bold | FontStyle.Italic), Brushes.DarkBlue, 700, 10);
                graphic.DrawLine(new Pen(Color.SandyBrown, 5f), 0f, 995f, 1200f, 995f);
                graphic.TranslateTransform(0, 1000);
                graphic.ScaleTransform(1f / size, -1f / size);
                graphic.DrawLines(new Pen(Color.Black, 3 * size), points);
            }
            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
        }

        static double ReadNum()
        {
            double x = 0;
            while (!double.TryParse(Console.ReadLine(), out x) || x <= 0)
            {
                Console.Write("Некорректный ввод. Попробуйте снова: ");
            }
            return x;
        }
    }
}
