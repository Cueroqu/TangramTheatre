using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Mathematics;
using System.Windows.Media;
using TangramTheatre.VisualUpdaters;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects.Controls;

namespace TangramTheatre.SharedObjects
{
    public abstract class TangramWindow : UCanvas
    {
        protected abstract void Initiallize();

        public float ScreenWidth
        {
            get { return (float) this.ActualWidth; }
        }

        public float ScreenHeight
        {
            get { return (float) this.ActualHeight; }
        }

        public Vector2 ScreenCenter
        {
            get { return new Vector2(ScreenWidth / 2, ScreenHeight / 2); }
        }

        protected TangramWindow()
        {
            Loaded += OnLoad;
            IsHitTestVisible = true;
        }

        //? I don't know it should be windows.media.hittestresult of utable.objects.hittestresut
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.Initiallize();
        }

        #region add visual to canvs

        private void RemoveVisual(FrameworkElement visual)
        {
            Children.Remove(visual);
        }

        public Rectangle AddRectangleToCanvas(Color color, Vector2 size)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.StrokeThickness = 0;
            rectangle.Fill = new SolidColorBrush(color);
            rectangle.Width = size.X;
            rectangle.Height = size.Y;
            //AddVisualToCanvas(rectangle, body);
            return rectangle;
        }

        public Rectangle AddRectangleToCanvas(Vector2 size)
        {
            return AddRectangleToCanvas(Colors.Yellow, size);
        }

        public void AddVisualToCanvas(FrameworkElement visual, Body body)
        {
            if (body != null)
                new BodyVisualHelper(visual, body);

            //hasn't been rendered yet, so needs to work out it's size
            if (visual.ActualWidth.Equals(0) && visual.ActualHeight.Equals(0))
            {
                visual.Arrange(new Rect(0, 0, ActualWidth, ActualHeight));
            }

            Children.Add(visual);
            visual.IsHitTestVisible = true;
        }

        #endregion

        public static void PositionTopLeft(FrameworkElement visual, Vector2 position)
        {
            SetLeft(visual, position.X);
            SetTop(visual, position.Y);
        }

        public static void CenterAround(FrameworkElement visual, Vector2 position)
        {
            SetLeft(visual, position.X - (visual.ActualWidth / 2));
            SetTop(visual, position.Y - (visual.ActualHeight / 2));
        }

        public static void SetRotation(FrameworkElement visual, float radians)
        {
            visual.RenderTransform = new RotateTransform((radians * 360) / (2 * Math.PI), visual.ActualWidth / 2, visual.ActualHeight / 2);
        }
    }
}
