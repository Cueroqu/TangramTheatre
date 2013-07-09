using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects;
using UTable.Objects.Controls;

namespace TangramTheatre
{
    /// <summary>
    /// Interaction logic for UObject1.xaml
    /// </summary>
    public partial class UObject1 : TangramTable
    {
        public UObject1()
        {
            InitializeComponent();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(Environment.CurrentDirectory + Define.ResoucePath + @"\table.jpg");
            bi.EndInit();
            Table.Background = new ImageBrush(bi);
        }
    }
}
