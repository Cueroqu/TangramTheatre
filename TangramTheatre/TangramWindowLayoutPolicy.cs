using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AdvanceMath;
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Solvers;
using TangramTheatre.SharedObjects;
using UTable.Input;
using UTable.Input.MultiTouch;
using UTable.Objects.Controls;
using TangramTheatre;
using UTangramAnimator;
using Scalar = System.Single;

namespace TangramTheatre
{
    #region Inner Classes

    class ScaleState
    {
        public double Scale { get; set; }
        public Point Center { get; set; }

        public ScaleState()
        {
            Scale = 1.0;
        }
    }

    class Contact
    {
        public Point Position { get; set; }
        public Point PreviousPosition { get; set; }
        public int FingerID { get; set; }

        public Contact(int FingerID, Point position)
        {
            this.PreviousPosition = this.Position = position;
            this.FingerID = FingerID;
        }

        public void UpdatePosition(Point position)
        {
            this.PreviousPosition = this.Position;
            this.Position = position;
        }
    }

    #endregion

    public class TangramWindowLayoutPolicy
    {
        #region Private Fields

        private PhysicsEngine _engine;
        private DesignCanvas _canvas;
        private PhysicsTimer _timer;

        private Body[] _bodies;
        private Robot[] _robots;
        private ScaleState[] _scaleses;
        private Dictionary<int, Contact>[] _elementsContacts;
        private Dictionary<int, FixedHingeJoint> _contactJoints;
        private Dictionary<int, Body> _contactBody; 
        private List<Point> _fingers; 

        #endregion

        #region Properties

        public PhysicsEngine Engine
        {
            get { return _engine; }
        }

        #endregion

        #region Constructor

        public TangramWindowLayoutPolicy(DesignCanvas tangramWindow)
        {
            _canvas = tangramWindow;
            AllocateMemory();
            InitializePhysicsEngine();
            CreateBodies();
        }

        #endregion

        #region Private Methods

        private void AllocateMemory()
        {
            _fingers = new List<Point>();
            _contactJoints = new Dictionary<int, FixedHingeJoint>();
            _contactBody = new Dictionary<int, Body>();
        }

        private void InitializePhysicsEngine()
        {
            _engine = new PhysicsEngine();
            _engine.BroadPhase = new SpatialHashDetector();
            _engine.Solver = new SequentialImpulsesSolver();
            _timer = new PhysicsTimer(PhysicsTimerCallback, 0.02f);
        }

        private void CreateBodies()
        {
            _bodies = new Body[_canvas.Pieces.Length];
            _scaleses = new ScaleState[_canvas.Pieces.Length];
            _elementsContacts = new Dictionary<int, Contact>[_canvas.Pieces.Length];
            _robots = new Robot[_canvas.Pieces.Length];
            for (int i = 0; i < _canvas.Pieces.Length; ++i)
                _bodies[i] = CreateBody(_canvas.Pieces[i]);
            _robots[0] = new Robot("192.168.0.180");
            _timer.IsRunning = true;
        }

        private Body CreateBody(ScatterRectangle rectangle)
        {
            float radius = MathHelper.ToRadians((Scalar) rectangle.Orientation);
            PhysicsState state = PhysicsHelper.CreateState(rectangle, radius, (Scalar)rectangle.Width, (Scalar)rectangle.Height);
            IShape shape = PhysicsHelper.CreateShape(rectangle, (Scalar) rectangle.Width, (Scalar) rectangle.Height);
            MassInfo mass = MassInfo.FromPolygon(shape.Vertexes, TangramInformation.TangramMass);
            Body body = new Body(state, shape, mass, new Coefficients(TangramInformation.Restitution, TangramInformation.Restitution), new Lifespan());
            body.LinearDamping = TangramInformation.DefaultLinearDamping;
            body.AngularDamping = TangramInformation.DefaultAngularDamping;
            body.Transformation *=
                Matrix2x3.FromScale(new Vector2D((Scalar) rectangle.ScaleX, (Scalar) rectangle.ScaleY));
            body.IsCollidable = true;
            body.Tag = rectangle;
            _engine.AddBody(body);

            ScaleState scale = new ScaleState();
            scale.Scale = rectangle.ScaleX;
            scale.Center = new Point(rectangle.Width/2, rectangle.Height/2);
            _scaleses[(int)rectangle.Category] = scale;
            _elementsContacts[(int) rectangle.Category] = new Dictionary<int, Contact>();
            rectangle.InputReceived += PieceReceiveInput;
            return body;
        }

        private void PieceReceiveInput(InputEventArgs args)
        {
            if (args is MultiTouchEventArgs && args.Receiver is ScatterControl)
                GetMultiTouchInput(args.Receiver as ScatterRectangle, args as MultiTouchEventArgs);
        }

        private void GetMultiTouchInput(ScatterRectangle rectangle, MultiTouchEventArgs args)
        {
            ScatterRectangle obj;
            foreach (FingerEventArgs e in args.FingerEvents)
            {
                switch (e.EventType)
                {
                    case FingerEventType.FINGER_DOWN_EVENT:
                    case FingerEventType.FINGER_IN_EVENT:
                        obj = HitTest(rectangle, e); // Since each ScatterRectangle is a rectangle but actually some are triangles, we need get the real "rectangle" according to the color of the mouse hit position
                        if (obj == null) return;
                        OnNewContact(obj, e);
                        break;
                    case FingerEventType.FINGER_MOVE_EVENT:
                        OnContactMove(rectangle, e);
                        break;
                    case FingerEventType.FINGER_OUT_EVENT:
                    case FingerEventType.FINGER_UP_EVENT:
                        OnContactRemoved(rectangle, e);
                        break;
                }
            }
            args.Handled = true;
        }

        private ScatterRectangle HitTest(ScatterRectangle rectangle, FingerEventArgs args)
        {
            Point global = rectangle.TranslatePoint(args.Position, _canvas), temp;
            ScatterRectangle curr;
            BitmapImage bi;
            byte[] pixel = new byte[4];
            for (int i = 0; i < _canvas.Pieces.Length; ++i)
            {
                curr = _canvas.Pieces[i];
                temp = _canvas.TranslatePoint(global, curr);
                if (temp.X > curr.Width || temp.X < 0 || temp.Y > curr.Height || temp.Y < 0) continue;
                bi = (curr.Background as ImageBrush).ImageSource as BitmapImage;
                bi.CopyPixels(new System.Windows.Int32Rect((int) (temp.X / curr.Width * bi.PixelWidth),
                                                           (int) (temp.Y / curr.Height * bi.PixelHeight), 1, 1), pixel,
                              bi.PixelWidth * 4, 0);
                if (pixel[3] != 0)
                    return curr;
            }
            return null;
        }

        private void OnNewContact(ScatterRectangle rectangle, FingerEventArgs e)
        {
            int index = (int)rectangle.Category;
            //not a new contact
            if (_elementsContacts[index].ContainsKey(e.FingerID))
                return;

            //e.Position stands the whole table, get the relative position of Target, i.e. Design Window here
            Point position = rectangle.TranslatePoint(e.Position, _canvas);
            Vector2D contactPoint = new Vector2D((Scalar)position.X, (Scalar)position.Y);
            _fingers.Add(position);

            Body body = _bodies[index];
            if (body.Shape.CanGetIntersection)
            {
                FixedHingeJoint joint = PhysicsHelper.FixedHingeJointFactory(body, contactPoint, new Lifespan());
                _engine.AddJoint(joint);
                _contactJoints[e.FingerID] = joint;
                _contactBody[e.FingerID] = body;
                _elementsContacts[index].Add(e.FingerID, new Contact(e.FingerID, position));
            }
            rectangle.CaptureFinger(e.FingerID);
        }

        private void OnContactMove(ScatterRectangle rectangle, FingerEventArgs e)
        {
            int index = (int)rectangle.Category;
            if (!_elementsContacts[index].ContainsKey(e.FingerID)) return;
            Point position = rectangle.TranslatePoint(e.Position, _canvas);
            _elementsContacts[index][e.FingerID].UpdatePosition(position);

            FixedHingeJoint joint;
            if (_contactJoints.TryGetValue(e.FingerID, out joint))
            {
                // move
                joint.Anchor = new Vector2D((Scalar)position.X, (Scalar)position.Y);
            }
        }

        private void OnContactRemoved(ScatterRectangle rectangle, FingerEventArgs e)
        {
            int index = (int) rectangle.Category;
            if (!_elementsContacts[index].ContainsKey(e.FingerID))
                return;

            FixedHingeJoint removedJoint;
            if (_contactJoints.TryGetValue(e.FingerID, out removedJoint))
            {
                removedJoint.Lifetime.IsExpired = true;
                _contactBody.Remove(e.FingerID);
                _contactJoints.Remove(e.FingerID);

            }
            _elementsContacts[index].Remove(e.FingerID);
            _fingers.Remove(e.Position);
        }

        private void PhysicsTimerCallback(float dt, float trueDt)
        {
            this._engine.Update(dt, trueDt);
            if (_canvas.Dispatcher != null)
                _canvas.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) UpdateChildren);
        }

        private void UpdateChildren()
        {
            ScatterRectangle rectangle;
            for (int i = 0; i < _bodies.Length; ++i)
            {
                rectangle = _bodies[i].Tag as ScatterRectangle;
                Vector2D linearPosition = _bodies[i].State.Position.Linear;
                rectangle.Position = new Point(linearPosition.X - rectangle.Width/2, linearPosition.Y - rectangle.Height/2);
                rectangle.Orientation = MathHelper.ToDegrees(_bodies[i].State.Position.Angular);
                rectangle.RotateCenter = new Point(rectangle.Width / 2, rectangle.Height / 2);
                rectangle.ScaleX = rectangle.ScaleY = _scaleses[i].Scale;
                rectangle.ScaleCenter = _scaleses[i].Center;
            }
        }

        #endregion

        #region Public Methods

        private Timer timer;

        public void RunRobot()
        {
            /*
             * while not in right position
                 * Get body current position
                 * Get robot current position
                 * Compute direction and distance and to above two positions
                 * Compute the moving time according to distance and velocity
                 * using DispatcherTimer to stop the car
                 * if moving time is less than threshold, break
             */
            Robot.Move(_robots[0], 45, 100);
            timer = new Timer(1000);
            
            timer.Elapsed += Test;
            //timer.Tag = _robots[0];
            Console.WriteLine("before start");
            timer.Enabled = true;
        }

        private void Test(object sender, ElapsedEventArgs e)
        {
            Robot.Stop(_robots[0]);
            (sender as Timer).Stop();
        }

        #endregion
    }
}
