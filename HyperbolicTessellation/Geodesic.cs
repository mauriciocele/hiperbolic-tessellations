using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HyperbolicTessellation
{
    public class Geodesic
    {
        //Puntos por los que pasa la linea
        public Point A { get; set; }
        public Point B { get; set; }
        //Línea hiperbólica cuando NO pasa por el origen
        public Circle C { get; set; }
        //Línea hiperbólica cuando pasa por el origen
        public Line L { get; set; }

        public bool IsLine { get { return C.HasInfiniteRadius; } }

        public Geodesic(Point A, Point B, Circle C)
        {
            this.A = A;
            this.B = B;
            this.C = C;

            if (C.HasInfiniteRadius)
            {
                L = new Line(o: A, dir: Vector.Divide(B - A, (B - A).Length));
            }
        }
    }
}
