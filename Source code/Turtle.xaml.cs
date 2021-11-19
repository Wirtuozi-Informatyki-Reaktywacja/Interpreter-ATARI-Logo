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
    }
}
