using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvanceMath;
using Physics2DDotNet;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using TangramTheatre.SharedObjects;
using UTangramAnimator;
using Scalar = System.Single;

namespace TangramTheatre
{
    public class PhysicsHelper
    {
        public static PhysicsState CreateState(ScatterRectangle target, Scalar radius, Scalar width, Scalar height)
        {
            ALVector2D answer;
            answer = new ALVector2D(radius, (float)(target.Position.X + width * .5f), (float)(target.Position.Y + height * .5f));
            return new PhysicsState(answer);
        }

        public static IShape CreateShape(ScatterRectangle target, Scalar width, Scalar height)
        {
            if (width <= 0) { throw new ArgumentOutOfRangeException("width", "must be greater then 0"); }
            if (height <= 0) { throw new ArgumentOutOfRangeException("height", "must be greater then 0"); }
            Vector2D[] vertexs = null;
            Scalar temp, temp2;

            switch (target.Category)
            {
                case TangramCategory.HorizontalSmallTriangle:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[3]
                    {
                        new Vector2D(temp, temp2),
                        new Vector2D(-temp, temp2),
                        new Vector2D(0, -temp2)
                    };
                    break;
                case TangramCategory.VerticalSmallTriangle:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[3]
                    {
                        new Vector2D(temp, -temp2),
                        new Vector2D(temp, temp2),
                        new Vector2D(-temp, 0)
                    };
                    break;
                case TangramCategory.Prallelogram:
                    temp = height * .5f;
                    temp2 = height + temp;
                    vertexs = new Vector2D[4]
                    {
                        new Vector2D(-temp2, temp),
                        new Vector2D(-temp, -temp),
                        new Vector2D(temp2, -temp),
                        new Vector2D(temp, temp)
                    };
                    break;
                case TangramCategory.HorizontalBigTriangle:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[3]
                    {
                        new Vector2D(-temp, -temp2),
                        new Vector2D(temp, -temp2),
                        new Vector2D(0, temp2)
                    };
                    break;
                case TangramCategory.MiddleTriangle:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[3]
                    {
                        new Vector2D(-temp, temp2),
                        new Vector2D(temp, -temp2),
                        new Vector2D(temp, temp2)
                    };
                    break;
                case TangramCategory.Square:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[4]
                    {
                        new Vector2D(-temp, 0),
                        new Vector2D(0, -temp2),
                        new Vector2D(temp, 0),
                        new Vector2D(0, temp2)
                    };
                    break;
                case TangramCategory.VerticalBigTriangle:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[3]
                    {
                        new Vector2D(-temp, -temp2),
                        new Vector2D(temp, 0),
                        new Vector2D(-temp, temp2)
                    };
                    break;
                default:
                    temp = width * .5f;
                    temp2 = height * .5f;
                    vertexs = new Vector2D[4]
                    {
                        new Vector2D(temp, 0),
                        new Vector2D(0, temp2),
                        new Vector2D(-temp, 0),
                        new Vector2D(0, -temp2)
                    };
                    break;
            }
            return new PolygonShape(vertexs, 2f);
        }

        public static FixedHingeJoint FixedHingeJointFactory(Body body, Vector2D anchor, Lifespan lifetime)
        {
            FixedHingeJoint joint = new FixedHingeJoint(body, anchor, lifetime);
            joint.Softness = TangramInformation.Softness;
            joint.BiasFactor = TangramInformation.BiasFactor - 0.1f;
            return joint;
        }
    }
}
