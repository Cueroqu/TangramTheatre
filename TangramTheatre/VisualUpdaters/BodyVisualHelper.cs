using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FarseerGames.FarseerPhysics.Dynamics;
using TangramTheatre.SharedObjects;

namespace TangramTheatre.VisualUpdaters
{
    public class BodyVisualHelper
    {
        private FrameworkElement visual;
        public FrameworkElement Visual
        {
            get { return visual; }
        }

        private Body body;
        public Body Body
        {
            get { return body; }
        }

        public BodyVisualHelper(FrameworkElement visual, Body body)
        {
            this.visual = visual;
            this.body = body;

            //update when moves/rotates or resized
            body.Updated += delegate { Update(); };
            visual.SizeChanged += delegate { Update(); };
        }

        public void Update()
        {
            TangramWindow.CenterAround(visual, body.Position);
            TangramWindow.SetRotation(visual, body.Rotation);
        }
    }
}
