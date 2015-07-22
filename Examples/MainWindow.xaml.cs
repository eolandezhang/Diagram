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

namespace Examples
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Canvas.MouseMove += Grid_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
            Border.MouseDown += Border_MouseDown;

        }

        void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            position = e.GetPosition(Canvas);
            iPosition = e.GetPosition(Border);
            Canvas.SetLeft(Shadow, position.X - iPosition.X);
            Canvas.SetTop(Shadow, position.Y - iPosition.Y);
            Shadow.Visibility = Visibility.Visible;
        }


        void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Shadow.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(Border, position.X - iPosition.X);
            Canvas.SetTop(Border, position.Y - iPosition.Y);

        }

        private Point position;
        private Point iPosition;
        void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(Canvas);
            Canvas.SetLeft(Shadow, position.X - iPosition.X);
            Canvas.SetTop(Shadow, position.Y - iPosition.Y);
        }
    }
}
