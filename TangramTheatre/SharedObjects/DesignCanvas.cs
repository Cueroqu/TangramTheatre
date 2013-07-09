using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTangramAnimator;

namespace TangramTheatre.SharedObjects
{
    public class DesignCanvas : TangramWindow
    {
        private TangramWindowLayoutPolicy _policy;
        private ScatterRectangle[] _pieces;

        protected override void Initiallize()
        {
            this.Children.Clear();
            AddScatterRectangles();
            _policy = new TangramWindowLayoutPolicy(this);
        }

        private void AddScatterRectangles()
        {
            _pieces = new ScatterRectangle[7];
            for (int i = 0; i < 7; ++i)
            {
                ScatterRectangle rectangle = new ScatterRectangle();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource =
                    new Uri(String.Format("{0}{1}{2}{3}{4}", Environment.CurrentDirectory, Define.ResoucePath,
                                          @"\widgets\tangram", i + 1, ".png"));
                bi.EndInit();
                rectangle.Background = new ImageBrush(bi);
                rectangle.Width = (int)(TangramInformation.TangramWidth[i] * TangramInformation.TangramSize / 100);
                rectangle.Height = (int)(TangramInformation.TangramHeight[i] * TangramInformation.TangramSize / 100);
                rectangle.Category = (TangramCategory)i;
                rectangle.Position = new Point(TangramInformation.TangramInitializationPosition[i].X, TangramInformation.TangramInitializationPosition[i].Y);
                this.Children.Add(rectangle);
                _pieces[i] = rectangle;
            }
        }

        #region Properties

        public ScatterRectangle[] Pieces
        {
            get { return _pieces; }
        }

        public TangramWindowLayoutPolicy Policy
        {
            get { return _policy; }
        }

        #endregion
    }
}
