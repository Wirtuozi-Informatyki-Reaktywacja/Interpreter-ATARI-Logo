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
        public MainWindow()
        {
            InitializeComponent();

            UpdateBottomOverlay();
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
    }
}
