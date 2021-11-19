using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interpreter_ATARI_Logo
{
    /// <summary>
    /// Logika interakcji dla klasy Turtle.xaml
    /// </summary>
    public partial class Turtle : UserControl
    {
        public Turtle()
        {
            InitializeComponent();
            DataContext = this;
        }
        public Brush Color { get; set; } = new SolidColorBrush(Colors.Black);
        public double Size { get; set; } = 70;
        public double Angle { get; set; } = 0;

        public void Rotate(double angle)
        {
            Angle += angle;
            if (Angle > 360)
            {
                Angle -= 360;
            }
            else if (Angle < 0)
            {
                Angle += 360;
            }

            RotateTransform rotation = new RotateTransform(Angle, svg.ActualWidth / 2, svg.ActualHeight / 2);
            svg.RenderTransform = rotation;
        }
    }
}
