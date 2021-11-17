﻿using System;
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

            if (output || editor)
            {
                bottomOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                bottomOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}