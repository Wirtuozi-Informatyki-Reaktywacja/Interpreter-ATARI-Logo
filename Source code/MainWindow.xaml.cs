using System;
using System.Collections.Generic;
using System.Numerics;
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
        public static RoutedCommand Help = new RoutedCommand();
        public static List<Turtle> turtles = new List<Turtle>();
        private List<Line> lines = new List<Line>();

        public bool PenDown { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();   

            UpdateBottomOverlay();

            ContentRendered += Window_ContentRendered;
            board.SizeChanged += Board_SizeChanged;

            Execute.InputGestures.Add(new KeyGesture(Key.F6));
            Help.InputGestures.Add(new KeyGesture(Key.F5));
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double widthDiffrence = (e.PreviousSize.Width - e.NewSize.Width) / 2;
            double heightDiffrence = (e.PreviousSize.Height - e.NewSize.Height) / 2;



            foreach (Turtle turtle in turtles)
            {
                double currentLeft = (double)turtle.GetValue(Canvas.LeftProperty);
                double currentTop = (double)turtle.GetValue(Canvas.TopProperty);

                turtle.SetValue(Canvas.LeftProperty, currentLeft - widthDiffrence);
                turtle.SetValue(Canvas.TopProperty, currentTop - heightDiffrence);
            }

            foreach (Line line in lines)
            {
                double x1 = line.X1;
                double x2 = line.X2;
                double y1 = line.Y1;
                double y2 = line.Y2;

                line.X1 = x1 - widthDiffrence;
                line.X2 = x2 - widthDiffrence;
                line.Y1 = y1 - heightDiffrence;
                line.Y2 = y2 - heightDiffrence;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
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

        public void ClearScreen()
        {
            board.Children.Clear();
            lines.Clear();

            foreach (Turtle turtle in turtles)
            {
                board.Children.Add(turtle);
            }
        }

        public void AddTurtle()
        {
            Turtle turtle = new Turtle();

            float size = turtle.Size;

            turtle.SetValue(Canvas.LeftProperty, board.ActualWidth / 2 - size / 2);
            turtle.SetValue(Canvas.TopProperty, board.ActualHeight / 2 - size / 2);
            turtle.SetValue(Canvas.ZIndexProperty, 1);

            turtles.Add(turtle);

            board.Children.Add(turtle);
        }

        public void Move(float distance)
        {
            Turtle turtle = turtles[0];
            float angle = MathF.PI / 180 * turtle.Angle;
            float size = turtle.Size;

            Vector2 direction = new Vector2(0, 1);

            double currentTop = (double)turtle.GetValue(Canvas.TopProperty);
            double currentLeft = (double)turtle.GetValue(Canvas.LeftProperty);

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            float tx = direction.X;
            float ty = direction.Y;

            direction.X = (cos * tx) - (sin * ty);
            direction.Y = (sin * tx) + (cos * ty);

            turtle.SetValue(Canvas.LeftProperty, currentLeft - direction.X * distance);
            turtle.SetValue(Canvas.TopProperty, currentTop - direction.Y * distance);

            if (PenDown)
            {
                Vector2 begin = new Vector2((float)currentLeft + size / 2, (float)currentTop + size / 2);
                Vector2 end = begin - direction * distance;

                Draw(begin, end);
            }
        }

        private void Draw (Vector2 begin, Vector2 end)
        {
            Line line = new Line()
            {
                Stroke = Brushes.Black,
                X1 = begin.X,
                X2 = end.X,
                Y1 = begin.Y,
                Y2 = end.Y,
                StrokeThickness = 2,
            };

            lines.Add(line);

            board.Children.Add(line);
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

        private void Command_Execute(object sender, RoutedEventArgs e)
        {
            Interpreter.Execute();
        }

        private void Command_Help(object sender, RoutedEventArgs e)
        {
            if (showOutput.IsChecked == false)
            {
                showOutput.IsChecked = true;
                UpdateBottomOverlay();
            }

            List<CommandBase> commandList = Interpreter.commandList;

            foreach(CommandBase command in commandList)
            {
                PrintLine(command.commandId + " - " + command.commandDescription + " - " + command.commandFormat);
            }
        }

        public void PrintLine(string text)
        {
            if (output.Text == "")
            {
                output.Text += text;
            }
            else
            {
                output.Text += Environment.NewLine + text;
            }

            outputScroll.ScrollToBottom();
        }
    }
}
