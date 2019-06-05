using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestGravityField
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double xc;
        double yc;
        double w;
        double h;
        double ykoeff = 1;
        double koeffScale = 1;
        double IncKoeff(double val)
        {
            return val * koeffScale;
        }
        double DecKoeff(double val)
        {
            return val / koeffScale;
        }
        double xToCanv(double x)
        {
            var rr = (IncKoeff(x) + xc);
            return rr;
        }

        double yToCanv(double y)
        {
            var rr = ((yc) - IncKoeff(y) / ykoeff);
            return rr;
        }

        double xFromCanv(double x)
        {
            return DecKoeff(x - xc);
        }

        double yFromCanv(double y)
        {
            return DecKoeff(yc - y) * ykoeff;
        }

        public Ellipse center = new Ellipse();



        public MainWindow()
        {
            InitializeComponent();


        }


        Random rnd = new Random();

        struct Vect
        {
            public double x;
            public double y;

            public Vect Minus()
            {
                Vect vv = this;
                vv.x = -vv.x;
                vv.y = -vv.y;
                return vv;
            }

            public Vect Mult(double v)
            {
                Vect vv = this;
                vv.x = vv.x * v;
                vv.y = vv.y * v;
                return vv;
            }

            public double Len()
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        class Particle
        {
            public Vect v; // текущая скорость
            public Vect r; // текущий радиус вектор

            public Ellipse point = new Ellipse();
        }

        Particle p1 = new Particle();
        Particle p2 = new Particle();



        void Cycle()
        {
            while(!fexit)
            {
                Tick();
                Thread.Sleep(1);
            }
        }

        void ToCanvas(Ellipse center, double x, double y)
        {
            Canvas.SetTop(center, yToCanv(y) - center.Height / 2);
            Canvas.SetLeft(center, xToCanv(x) - center.Width / 2);

            v1.Text = $"v1={p1.v.Len()}";
            v2.Text = $"v2={p2.v.Len()}";
        }

        double R_field = 50;

        void applyF(Particle p)
        {
            double timekoeff = 100;
            var v = p.v;
            var r = p.r;

            double R = r.Len();

            // сила пропорциональна радиусу.
            // движение положительного ядра в электронном облаке 
            var f = r.Minus();

            var actualR = R;
            if(R < R_field)
            {
                actualR = R_field;
            }
            f = f.Mult( 5000000 / (actualR * actualR * actualR));


            var vnew = v;
            vnew.x = v.x + f.x / timekoeff;
            vnew.y = v.y + f.y / timekoeff;

            p.v = vnew;
            p.r.x += vnew.x / timekoeff;
            p.r.y += vnew.y / timekoeff;

            Dispatcher.Invoke(() =>
            {
                PointToCanvas();
            });
        }

        public void Tick()
        {
            applyF(p1);
            applyF(p2);



        }

        void PointToCanvas()
        {
            ToCanvas(p1.point, p1.r.x, p1.r.y);
            ToCanvas(p2.point, p2.r.x, p2.r.y);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            w = canvas.ActualWidth;
            h = canvas.ActualHeight;
            xc = w / 2.0;
            yc = h / 2.0;

            Ellipse pcenter = new Ellipse();
            pcenter.Width = 5;
            pcenter.Height = 5;

            pcenter.Fill = Brushes.Blue;
            pcenter.Stroke = Brushes.Black;

            center.Width = R_field*2;
            center.Height = R_field*2;

            p1.point.Width = 10;
            p1.point.Height = 10;

            p2.point.Width = 10;
            p2.point.Height = 10;

            center.Fill = Brushes.Red;
            center.Stroke = Brushes.Black;

            p1.point.Fill = Brushes.Yellow;
            p1.point.Stroke = Brushes.Black;

            p2.point.Fill = Brushes.Azure;
            p2.point.Stroke = Brushes.Black;

            canvas.Children.Add(center);
            canvas.Children.Add(pcenter);
            Canvas.SetZIndex(center, -5);
            Canvas.SetZIndex(pcenter, -4);

            canvas.Children.Add(p1.point);
            canvas.Children.Add(p2.point);

            p1.v.x = 300;
            p1.v.y = -30;

            p1.r.x = 0;
            p1.r.y = 75;

            p2.v.x = 100;
            p2.v.y = 0;

            p2.r.x = 0;
            p2.r.y = 30;

            ToCanvas(center, 0, 0);
            ToCanvas(pcenter, 0, 0);

            PointToCanvas();


            tsk = Task.Factory.StartNew(Cycle);
        }
        Task tsk;
        bool fexit;

        private  async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            fexit = true;
            await tsk;
        }
    }
}
