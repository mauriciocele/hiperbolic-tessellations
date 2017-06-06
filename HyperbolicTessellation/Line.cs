using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HyperbolicTessellation
{
    public class Line
    {
        public Point O { get; set; }
        public Vector Dir { get; set; }

        public Line(Point o, Vector dir)
        {
            this.O = o;
            this.Dir = dir;
        }

        public Point Reflect(Point P)
        {
            double d = (P - O) * Dir; //dot product
            Point a = O + (Dir * d);
            Vector v = a - P;
            return P + 2 * v;
        }
    }
}
