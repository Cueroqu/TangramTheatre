using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Physics2DDotNet.Shapes;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects;
using UTable.Objects.Controls;
using UTable.Objects.Shapes;
using TangramTheatre.SharedObjects;
using Physics2DDotNet;
using AdvanceMath;
using UTangramAnimator;
using System.Threading;

namespace TangramTheatre
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : UObject
    {
        private TangramWindowLayoutPolicy _policy;

        public DesignWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            window.Visibility = Visibility.Visible;
            SetBackground();
            _policy = window.Policy;
        }

        private void SetBackground()
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(Environment.CurrentDirectory + Define.ResoucePath + @"\windowback.png");
            b.EndInit();
            window.Background = new ImageBrush(b);
        }

        private void RunRobotClick(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RunRobot));
            thread.Start();
        }

        private void RunRobot()
        {
            window.Policy.RunRobot();
        }
    }    
}
