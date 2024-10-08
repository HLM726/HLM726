using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkOrder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void StartProgress_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("ProgressAnimation");
            storyboard.Begin(this);
        }
        private void MinClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var mes = MessageBox.Show(this, "您确定要退出吗？", "", MessageBoxButton.OKCancel);
            if (mes.Equals(MessageBoxResult.OK))
            {
                this.Close();
                System.Windows.Application current = System.Windows.Application.Current;
                if (current != null) current.Shutdown();
            }
        }
    }
}
