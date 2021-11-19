using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Interpreter_ATARI_Logo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand Execute = new RoutedCommand();
        public static List<Turtle> turtles = new List<Turtle>();

        double canvasWidth;
        double canvasHeight;

        public MainWindow()
        {
            InitializeComponent();   

            UpdateBottomOverlay();

            ContentRendered += Window_ContentRendered;
            board.SizeChanged += Board_SizeChanged;

            Execute.InputGestures.Add(new KeyGesture(Key.F6));
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double widthDiffrence = (canvasWidth - board.ActualWidth) / 2;
            double heightDiffrence = (canvasHeight - board.ActualHeight) / 2;



            foreach (Turtle turtle in turtles)
            {
                double currentLeft = (double)turtle.GetValue(Canvas.LeftProperty);
                double currentTop = (double)turtle.GetValue(Canvas.TopProperty);

                turtle.SetValue(Canvas.LeftProperty, currentLeft - widthDiffrence);
                turtle.SetValue(Canvas.TopProperty, currentTop - heightDiffrence);
            }

            canvasWidth = board.ActualWidth;
            canvasHeight = board.ActualHeight;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            canvasWidth = board.ActualWidth;
            canvasHeight = board.ActualHeight;

            AddTurtle();
        }

        private void UpdateBottomOverlay(object sender = null, RoutedEventArgs e = null)
        {
            bool output = showOutput.IsChecked;
            bool editor = showEditor.IsChecked;

            bottomOverlay.Visibility = Visibility.Collapsed;
            overlaySplitter.Visibility = Visibility.Collapsed;

            editorCanv.Visibility = Visibility.Collapsed;
            outputCanv.Visibility = Visibility.Collapsed;

            if (editor)
            {
                bottomOverlay.Visibility = Visibility.Visible;
                editorCanv.Visibility = Visibility.Visible;

                if (output)
                {
                    overlaySplitter.Visibility = Visibility.Visible;

                    editorCanv.SetValue(Grid.ColumnSpanProperty, 1);
                    outputCanv.SetValue(Grid.ColumnSpanProperty, 1);
                    outputCanv.SetValue(Grid.ColumnProperty, 2);
                }
                else
                {
                    editorCanv.SetValue(Grid.ColumnSpanProperty, 3);
                    editorCanv.SetValue(Grid.ColumnProperty, 0);
                }
            }

            if (output)
            {
                bottomOverlay.Visibility = Visibility.Visible;
                outputCanv.Visibility = Visibility.Visible;

                if (!editor)
                {
                    outputCanv.SetValue(Grid.ColumnSpanProperty, 3);
                    outputCanv.SetValue(Grid.ColumnProperty, 0);
                }
            }
        }

        public void AddTurtle()
        {
            Turtle turtle = new Turtle();

            double size = turtle.Size;

            turtle.SetValue(Canvas.LeftProperty, canvasWidth / 2 - size / 2);
            turtle.SetValue(Canvas.TopProperty, canvasHeight / 2 - size / 2);

            turtles.Add(turtle);

            board.Children.Add(turtle);
        }

        public void Move(float distance)
        {
            Turtle turtle = turtles[0];
            double angle = Math.PI / 180 * turtle.Angle;

            double currentTop = (double)turtle.GetValue(Canvas.TopProperty);
            double currentLeft = (double)turtle.GetValue(Canvas.LeftProperty);

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            float absDistance = Math.Abs(distance);

            double a = sin * distance;
            double b = cos * distance;

            if (angle == 0 || angle == 360)
            {
                turtle.SetValue(Canvas.TopProperty, currentTop - distance);
            }
            else if (angle == 90)
            {
                turtle.SetValue(Canvas.LeftProperty, currentLeft + distance);
            }
            else if (angle == 180)
            {
                turtle.SetValue(Canvas.TopProperty, currentTop + distance);
            }
            else if (angle == 270)
            {
                turtle.SetValue(Canvas.LeftProperty, currentLeft - distance);
            }
            else
            {
                turtle.SetValue(Canvas.LeftProperty, currentLeft + a);
                turtle.SetValue(Canvas.TopProperty, currentTop - b);
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CloseTab(object sender, RoutedEventArgs e)
        {
            string value = ((Button)sender).Tag.ToString();

            switch (value)
            {
                case "editor":
                    showEditor.IsChecked = false;
                    UpdateBottomOverlay();
                    break;

                case "output":
                    showOutput.IsChecked = false;
                    UpdateBottomOverlay();
                    break;
            }
        }

        private void Command_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Interpreter.ProccessInput();
        }
    }
}
