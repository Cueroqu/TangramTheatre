using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TangramTheatre.SharedObjects;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects;
using UTable.Objects.Controls;
using UTable.Objects.Policies;
using UTable.Objects.Shapes;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows;

namespace TangramTheatre
{
    public class TangramTable : UObject
    {
        #region Private Members

        private Dictionary<int, DispatcherTimer> _holdFingerList = new Dictionary<int, DispatcherTimer>();
        private Dictionary<int, Point> _holdFingerPositionList = new Dictionary<int, Point>();
        private Dictionary<UElementMenu, int> _menuList = new Dictionary<UElementMenu, int>();
        private Dictionary<UElementMenu, Point> _menuPositionList = new Dictionary<UElementMenu, Point>();
        private readonly int _holdRaidus = 10;
        private readonly int _holdTime = 1000;

        #endregion

        #region Constructor

        public TangramTable()
        {
            this.LayoutState = ObjectLayoutState.Maximized;
            PhysicalLayoutPolicyParameter parameter = new PhysicalLayoutPolicyParameter(true, 0.6d, 0.45d);
            parameter.CollisionPolicy = new TabletopCollisionPolicy();
            this.LayoutPolicyParameter = parameter;
            this.LayoutPolicyType = typeof (PhysicalLayoutPolicy);
            this.InputReceived += new InputEventHandler(TangramTable_OnInputReceived);

            this.MenuDisplay = UObjectMenuDisplayType.Hide;
            this.ShowsActivationEffects = false;
        }

        #endregion

        #region Private Methods

        #region Input Handled

        private void TangramTable_OnInputReceived(InputEventArgs args)
        {
            if (args is MultiTouchEventArgs && !args.Handled)
            {
                TangramTable_OnMultiTouchReceived(args as MultiTouchEventArgs);
            }
        }

        private void TangramTable_OnMultiTouchReceived(MultiTouchEventArgs args)
        {
            foreach (FingerEventArgs finger in args.FingerEvents)
            {
                switch (finger.EventType)
                {
                    case FingerEventType.FINGER_DOWN_EVENT:
                        AddNewPiePiece(finger);
                        break;
                    case FingerEventType.FINGER_OUT_EVENT:
                    case FingerEventType.FINGER_UP_EVENT:
                        if (_holdFingerList.ContainsKey(finger.FingerID))
                            RemovePiePiece(finger);
                        break;
                    case FingerEventType.FINGER_MOVE_EVENT:
                        if (_holdFingerList.ContainsKey(finger.FingerID))
                        {
                            double distance = (finger.Position - _holdFingerPositionList[finger.FingerID]).Length;
                            if (distance > this._holdRaidus)
                                RemovePiePiece(finger);
                        }
                        break;
                }
            }
        }

        #endregion

        private void AddNewPiePiece(FingerEventArgs finger)
        {
            DispatcherTimer startTimer = new DispatcherTimer();
            startTimer.Tick += new EventHandler(startTimer_Elapsed);
            startTimer.Interval = TimeSpan.FromMilliseconds(this._holdTime);
            _holdFingerList.Add(finger.FingerID, startTimer);
            _holdFingerPositionList.Add(finger.FingerID, finger.Position);

            PiePiece pie = new PiePiece();
            pie.Radius = 24;
            pie.InnerRadius = 12;
            pie.Fill = Brushes.Beige;
            pie.Opacity = .8d;
            Canvas.SetLeft(pie, finger.Position.X);
            Canvas.SetTop(pie, finger.Position.Y);
            PopupStage.Children.Add(pie);
            pie.BeginAnimation(PiePiece.WedgeAngleProperty, new DoubleAnimation(360, new Duration(TimeSpan.FromMilliseconds(this._holdTime))));
            startTimer.Tag = pie;
            startTimer.Start();
        }

        private void RemovePiePiece(FingerEventArgs finger)
        {
            DispatcherTimer timer = _holdFingerList[finger.FingerID];
            timer.Stop();
            PiePiece pie = timer.Tag as PiePiece;
            pie.BeginAnimation(PiePiece.WedgeAngleProperty, null);
            PopupStage.Children.Remove(pie);

            _holdFingerList.Remove(finger.FingerID);
            _holdFingerPositionList.Remove(finger.FingerID);
        }

        private void startTimer_Elapsed(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            timer.Stop();
            if (_holdFingerList.ContainsValue(timer))
            {
                int finger = GetFingerIDByTimer(timer);

                UElementMenu menu = new UElementMenu();
                UElementMenuItem shutItem = new UElementMenuItem();
                shutItem.Header = "Shut down";
                shutItem.Command = "ShutDown";
                UElementMenuItem resetItem = new UElementMenuItem();
                resetItem.Header = "Reset tabletop";
                resetItem.Command = "ResetTabletop";
                UElementMenuItem loadItem = new UElementMenuItem();
                loadItem.Header = "Design Window";
                loadItem.Command = "DesignWindow";
                UElementMenuItem procudeItem = new UElementMenuItem();
                procudeItem.Header = "Direct Window";
                procudeItem.Command = "DirectWindow";
                UElementMenuItem nothingItem = new UElementMenuItem();
                nothingItem.Header = "Close Menu";
                nothingItem.Command = "CloseMenu";

                menu.Items.Add(shutItem);
                menu.Items.Add(loadItem);
                menu.Items.Add(resetItem);
                menu.Items.Add(procudeItem);
                menu.Items.Add(nothingItem);
                menu.MaxAngle = 320;
                Random r = new Random();
                menu.Orientation = r.NextDouble() * 360;

                Canvas.SetLeft(menu, _holdFingerPositionList[finger].X - 30);
                Canvas.SetTop(menu, _holdFingerPositionList[finger].Y - 30);
                menu.Opacity = 1;
                menu.Width = 60;
                menu.Height = 60;
                _menuPositionList.Add(menu, _holdFingerPositionList[finger]);
                _menuList.Add(menu, finger);
                _holdFingerList.Remove(finger);
                _holdFingerPositionList.Remove(finger);
                PopupStage.Children.Add(menu);

                PiePiece pie = timer.Tag as PiePiece;
                pie.BeginAnimation(PiePiece.WedgeAngleProperty, null);
                PopupStage.Children.Remove(pie);

                menu.Loaded += new RoutedEventHandler(menu_Loaded);
                menu.SubmenuClosed += new EventHandler(menu_SubmenuClosed);
                menu.CommandReceived += new ElementMenuCommandReceivedEventHandler(OnCommandReceived);
            }
        }

        private int GetFingerIDByTimer(DispatcherTimer timer)
        {
            foreach (int fingerID in _holdFingerList.Keys)
            {
                if (_holdFingerList[fingerID] == timer) return fingerID;
            }
            return -1;
        }

        void menu_Loaded(object sender, RoutedEventArgs e)
        {
            UElementMenu menu = sender as UElementMenu;
            if (!_menuList.ContainsKey(menu))
            {
                // remove the menu
                PopupStage.Children.Remove(menu);
                return;
            }

            int finger = _menuList[menu];
            // raise the finger down event on that menu so that it can be open when it is shown
            FingerEventArgs args = new FingerEventArgs(FingerEventType.FINGER_DOWN_EVENT, finger, new Point(20, 20));
            Collection<FingerEventArgs> collection = new Collection<FingerEventArgs>();
            collection.Add(args);
            menu.RaiseEvent(new MultiTouchEventArgs(collection, 0));
        }

        void menu_SubmenuClosed(object sender, EventArgs e)
        {
            UElementMenu menu = sender as UElementMenu;

            if (!_menuList.ContainsKey(menu))
            {
                //remove the menu
                PopupStage.Children.Remove(menu);
                return;
            }
            
            PopupStage.Children.Remove(menu);
            _menuList.Remove(menu);
            _menuPositionList.Remove(menu);

        }

        protected virtual void OnCommandReceived(UElementMenu sender, ElementMenuCommandReceivedEventArgs args)
        {
            switch (args.Command)
            {
                case "ShutDown":
                    UTableHelper.Shutdown(0);
                    break;
                case "DesignWindow":
                    {                        
                         ObjectCreateParameter loadAppParam = new ObjectCreateParameter(typeof(DesignWindow));
                         IObject appLoader = UTableHelper.CreateObject(loadAppParam, this);
                         PlaceAppLoader(appLoader, this._menuPositionList[sender]);
                    }
                    break;
                case "DirectWindow":
                    //TODO
                    break;
                case "ResetTabletop":
                    //TODO
                    break;
                case "CloseMenu":
                    break;
            }
        }

        protected virtual void PlaceAppLoader(IObject appLoader, Point holdPosition)
        {
            Point expectedPosition = new Point(holdPosition.X - appLoader.Width / 2 + 50, holdPosition.Y - appLoader.Height / 2 + 50);

            if (expectedPosition.X < 0)
                expectedPosition.X = 0;
            else if (expectedPosition.X + appLoader.Width > this.Width)
                expectedPosition.X = this.Width - appLoader.Width;

            if (expectedPosition.Y < 0)
                expectedPosition.Y = 0;
            else if (expectedPosition.Y + appLoader.Height > this.Height)
                expectedPosition.Y = this.Height - appLoader.Height;

            appLoader.Position = expectedPosition;
        }

        #endregion
    }
}
