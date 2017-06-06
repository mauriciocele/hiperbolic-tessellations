using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HyperbolicTessellation
{
    public class Circle
    {
        public double Radius { get; set; }
        public Point Center { get; set; }
        public bool HasInfiniteRadius { get { return Radius == -1 || Radius >= 10000; } }
        public bool IsDegenerate { get { return Radius == 0; } }

        public Circle()
        {
        }

        public Circle(Point p1, Point p2, Point p3)
        {
            double[] ap1 = { p1.X, p1.Y };
            double[] ap2 = { p2.X, p2.Y };
            double[] ap3 = { p3.X, p3.Y };
            double[] pc = { 0, 0 };
            double r;
            Geometry.circle_exp2imp_2d(ap1, ap2, ap3, out r, pc);
            Radius = r;
            Center = new Point(pc[0], pc[1]);
        }

        public Circle(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
