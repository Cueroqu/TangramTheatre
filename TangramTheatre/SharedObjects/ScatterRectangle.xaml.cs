using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Xml;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects;
using UTable.Objects.Controls;
using TangramTheatre.SharedObjects;

namespace UTangramAnimator
{
    /// <summary>
    /// Interaction logic for ScatterRectangle.xaml
    /// </summary>
    public partial class ScatterRectangle : ScatterControl
    {
        public ScatterRectangle()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(ScatterRectangle), new UIPropertyMetadata(0.0));

        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(ScatterRectangle), new UIPropertyMetadata(0.0));

        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

//         public void Serialize(XmlWriter writer)
//         {
//             writer.WriteStartElement("Rectangle");
//             writer.WriteAttributeString("height", this.Height.ToString());
//             writer.WriteAttributeString("width", this.Width.ToString());
//             writer.WriteAttributeString("position", String.Format("{0} {1}", this.Position.X, this.Position.Y));
//             writer.WriteAttributeString("orientation", this.Orientation.ToString());
//             writer.WriteAttributeString("tanclass", ((int)this.tanClass).ToString());
//             writer.WriteEndElement();
//         }
// 
//         public static ScatterRectangle Deserialize(XmlElement ele)
//         {
//             ScatterRectangle rectangle = new ScatterRectangle();
//             rectangle.Height = Double.Parse(ele.GetAttribute("height"));
//             rectangle.Width = Double.Parse(ele.GetAttribute("width"));
//             string[] pos = ele.GetAttribute("position").Split(' ');
//             rectangle.Position = new Point(Double.Parse(pos[0]), Double.Parse(pos[1]));
//             rectangle.Orientation = Double.Parse(ele.GetAttribute("orientation"));
//             rectangle.tanClass = (TanClass)(Int32.Parse(ele.GetAttribute("tanclass")));
//             return rectangle;
//         }

        public TangramCategory Category { get; set; }

        #endregion
    }

    public class RectValueConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Rect(0, 0, System.Convert.ToDouble(values[0]), System.Convert.ToDouble(values[1]));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
