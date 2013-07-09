using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using Scalar = System.Single;

namespace TangramTheatre.SharedObjects
{
    public enum TangramCategory
    {
        None = -1,
        HorizontalSmallTriangle,
        VerticalSmallTriangle,
        Prallelogram,
        HorizontalBigTriangle,
        MiddleTriangle,
        Square,
        VerticalBigTriangle,
    }

    public static class TangramInformation
    {
        public static readonly int TangramSize = 150;
        public static readonly int[] TangramWidth = new int[7] { 50, 25, 75, 100, 50, 50, 50 };
        public static readonly int[] TangramHeight = new int[7] { 25, 50, 25, 50, 50, 50, 100 };
        public static readonly int TangramMass = 10000;
        public static readonly Scalar Restitution = 0.6f;
        public static readonly Scalar Friction = 0.95f;
        public static readonly Scalar DefaultLinearDamping = 0.5f;
        public static readonly Scalar DefaultAngularDamping = 0.5f;
        public static readonly Scalar Softness = 0;
        public static readonly Scalar BiasFactor = 1f;

        public static readonly Point[] TangramInitializationPosition = new Point[7]
            {
                new Point(87, 175), new Point(163, 100), new Point(50, 213), new Point(50, 100),
                new Point(125, 175), new Point(125, 137), new Point(50, 100)
            };

    }
}
