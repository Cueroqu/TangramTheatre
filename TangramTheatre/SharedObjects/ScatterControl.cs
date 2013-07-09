using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UTable.Objects.Controls;

namespace UTangramAnimator
{
    public class ScatterControl : UUserControl
    {
        #region Private Fields

        private RotateTransform _rotateTransform;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        #endregion

        #region Constructor

        public ScatterControl()
        {
            // set the transform 
            TransformGroup _transformGroup = new TransformGroup();
            this._rotateTransform = new RotateTransform();
            this._scaleTransform = new ScaleTransform();
            this._translateTransform = new TranslateTransform();
            this._translateTransform.X = this._translateTransform.Y = 0;
            _transformGroup.Children.Add(_rotateTransform);
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);
            this.RenderTransform = _transformGroup;
        }

        #endregion

        #region Properties

        public Point Position
        {
            get
            {
                return new Point(_translateTransform.X, _translateTransform.Y);
            }
            set
            {
                _translateTransform.X = value.X;
                _translateTransform.Y = value.Y;
            }
        }

        public double Orientation  //degree not raidus
        {
            get
            {
                return _rotateTransform.Angle;
            }
            set
            {
                _rotateTransform.Angle = value;
            }
        }

        public Point RotateCenter
        {
            get
            {
                return new Point(_rotateTransform.CenterX, _rotateTransform.CenterY);
            }
            set
            {
                this._rotateTransform.CenterX = value.X;
                this._rotateTransform.CenterY = value.Y;
            }
        }

        public double ScaleX
        {
            get
            {
                return _scaleTransform.ScaleX;
            }
            set
            {
                _scaleTransform.ScaleX = value;
            }
        }

        public double ScaleY
        {
            get
            {
                return _scaleTransform.ScaleY;
            }
            set
            {
                _scaleTransform.ScaleY = value;
            }
        }

        public Point ScaleCenter
        {
            get
            {
                return new Point(_scaleTransform.CenterX, _scaleTransform.CenterY);
            }
            set
            {
                _scaleTransform.CenterX = value.X;
                _scaleTransform.CenterY = value.Y;
            }
        }

        public int ZIndex
        {
            get
            {
                return Canvas.GetZIndex(this);
            }
            set
            {
                Canvas.SetZIndex(this, value);
            }
        }

        #endregion
    }
}
