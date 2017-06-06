using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HyperbolicTessellation
{
    public class Geometry
    {
        static bool r8vec_eq ( int n, double[] a1, double[] a2 )
        //****************************************************************************80
        //
        //  Purpose:
        //
        //    R8VEC_EQ is true two R8VEC's are equal.
        //
        //  Licensing:
        //
        //    This code is distributed under the GNU LGPL license.
        //
        //  Modified:
        //
        //    28 August 2003
        //
        //  Author:
        //
        //    John Burkardt
        //
        //  Parameters:
        //
        //    Input, int N, the number of entries in the vectors.
        //
        //    Input, double A1[N], A2[N], two vectors to compare.
        //
        //    Output, bool R8VEC_EQ.
        //    R8VEC_EQ is TRUE if every pair of elements A1(I) and A2(I) are equal,
        //    and FALSE otherwise.
        //
        {
          int i;

          for ( i = 0; i < n; i++ )
          {
            if ( Math.Abs(a1[i] - a2[i]) > 1e-7 )
            {
              return false;
            }
          }
          return true;
        }

        static void r8vec_copy(int n, double[] a1, double[] a2)
        //****************************************************************************80
        //
        //  Purpose:
        //
        //    R8VEC_COPY copies an R8VEC.
        //
        //  Licensing:
        //
        //    This code is distributed under the GNU LGPL license.
        //
        //  Modified:
        //
        //    03 July 2005
        //
        //  Author:
        //
        //    John Burkardt
        //
        //  Parameters:
        //
        //    Input, int N, the number of entries in the vectors.
        //
        //    Input, double A1[N], the vector to be copied.
        //
        //    Input, double A2[N], the copy of A1.
        //
        {
          int i;

          for ( i = 0; i < n; i++ )
          {
            a2[i] = a1[i];
          }
          return;
        }

        public static void circle_exp2imp_2d(double[] p1, double[] p2, double[] p3, out double r, double[] pc)
        //****************************************************************************80
        //
        //  Purpose:
        //
        //    CIRCLE_EXP2IMP_2D converts a circle from explicit to implicit form in 2D.
        //
        //  Discussion:
        //
        //    The explicit form of a circle in 2D is:
        //
        //      The circle passing through points P1, P2 and P3.
        //
        //    Points P on an implicit circle in 2D satisfy the equation:
        //
        //      pow ( P[0] - PC[0], 2 ) + pow ( P[1] - PC[1], 2 ) = pow ( R, 2 )
        //
        //    Any three points define a circle, as long as they don't lie on a straight
        //    line.  (If the points do lie on a straight line, we could stretch the
        //    definition of a circle to allow an infinite radius and a center at
        //    some infinite point.)
        //
        //    Instead of the formulas used here, you can use the linear system
        //    approach in the routine TRIANGLE_OUTCIRCLE_2D.
        //
        //    The diameter of the circle can be found by solving a 2 by 2 linear system.
        //    This is because the vectors P2 - P1 and P3 - P1 are secants of the circle,
        //    and each forms a right triangle with the diameter.  Hence, the dot product
        //    of P2 - P1 with the diameter is equal to the square of the length
        //    of P2 - P1, and similarly for P3 - P1.  These two equations determine the
        //    diameter vector originating at P1.
        //
        //    If all three points are equal, return a circle of radius 0 and
        //    the obvious center.
        //
        //    If two points are equal, return a circle of radius half the distance
        //    between the two distinct points, and center their average.
        //
        //  Licensing:
        //
        //    This code is distributed under the GNU LGPL license.
        //
        //  Modified:
        //
        //    10 March 2006
        //
        //  Author:
        //
        //    John Burkardt
        //
        //  Reference:
        //
        //    Joseph ORourke,
        //    Computational Geometry,
        //    Second Edition,
        //    Cambridge, 1998,
        //    ISBN: 0521649765,
        //    LC: QA448.D38.
        //
        //  Parameters:
        //
        //    Input, double P1[2], P2[2], P3[2], are the coordinates
        //    of three points that lie on the circle.  These points should be
        //    distinct, and not collinear.
        //
        //    Output, double *R, the radius of the circle.  Normally, R will be positive.
        //    R will be (meaningfully) zero if all three points are
        //    equal.  If two points are equal, R is returned as the distance between
        //    two nonequal points.  R is returned as -1 in the unlikely event that
        //    the points are numerically collinear; philosophically speaking, R
        //    should actually be "infinity" in this case.
        //
        //    Output, double PC[2], the center of the circle.
        //
        {
          const int DIM_NUM = 2;

          double a;
          double b;
          double c;
          double d;
          double e;
          double f;
          double g;
        //
        //  If all three points are equal, then the
        //  circle of radius 0 and center P1 passes through the points.
        //
          if ( r8vec_eq ( DIM_NUM, p1, p2 ) && r8vec_eq ( DIM_NUM, p1, p3 ) )
          {
            r = 0.0;
            r8vec_copy ( DIM_NUM, p1, pc );
            return;
          }
        //
        //  If exactly two points are equal, then the circle is defined as
        //  having the obvious radius and center.
        //
          if ( r8vec_eq ( DIM_NUM, p1, p2 ) )
          {
              r = 0.5 * Math.Sqrt((p1[0] - p3[0]) * (p1[0] - p3[0])
                            + ( p1[1] - p3[1] ) * ( p1[1] - p3[1] ) );
            pc[0] = 0.5 * ( p1[0] + p3[0] );
            pc[1] = 0.5 * ( p1[1] + p3[1] );
            return;
          }
          else if ( r8vec_eq ( DIM_NUM, p1, p3 ) )
          {
              r = 0.5 * Math.Sqrt((p1[0] - p2[0]) * (p1[0] - p2[0])
                            + ( p1[1] - p2[1] ) * ( p1[1] - p2[1] ) );
            pc[0] = 0.5 * ( p1[0] + p2[0] );
            pc[1] = 0.5 * ( p1[1] + p2[1] );
            return;
          }
          else if ( r8vec_eq ( DIM_NUM, p2, p3 ) )
          {
              r = 0.5 * Math.Sqrt((p1[0] - p2[0]) * (p1[0] - p2[0])
                            + ( p1[1] - p2[1] ) * ( p1[1] - p2[1] ) );
            pc[0] = 0.5 * ( p1[0] + p2[0] );
            pc[1] = 0.5 * ( p1[1] + p2[1] );
            return;
          }

          a = p2[0] - p1[0];
          b = p2[1] - p1[1];
          c = p3[0] - p1[0];
          d = p3[1] - p1[1];

          e = a * ( p1[0] + p2[0] ) + b * ( p1[1] + p2[1] );
          f = c * ( p1[0] + p3[0] ) + d * ( p1[1] + p3[1] );
        //
        //  Our formula is:
        //
        //    G = a * ( d - b ) - b * ( c - a )
        //
        //  but we get slightly better results using the original data.
        //
          g = a * ( p3[1] - p2[1] ) - b * ( p3[0] - p2[0] );
        //
        //  We check for collinearity.  A more useful check would compare the
        //  absolute value of G to a small quantity.
        //
          if ( Math.Abs(g) <= 1e-7 )
          {
            pc[0] = 0.0;
            pc[1] = 0.0;
            r = -1.0;
            return;
          }
        //
        //  The center is halfway along the diameter vector from P1.
        //
          pc[0] = 0.5 * ( d * e - b * f ) / g;
          pc[1] = 0.5 * ( a * f - c * e ) / g;
        //
        //  Knowing the center, the radius is now easy to compute.
        //
          r = Math.Sqrt((p1[0] - pc[0]) * (p1[0] - pc[0])
                    + ( p1[1] - pc[1] ) * ( p1[1] - pc[1] ) );

          return;
        }

        public static void circle_imp_line_par_int_2d ( double r, double[] pc, double x0, double y0, double f, double g, out int int_num, double[] p )
        //****************************************************************************80
        //
        //  Purpose:
        //
        //    CIRCLE_IMP_LINE_PAR_INT_2D: ( implicit circle, parametric line ) intersection in 2D.
        //
        //  Discussion:
        //
        //    An implicit circle in 2D satisfies the equation:
        //
        //      pow ( P[0] - PC[0], 2 ) + pow ( P[1] - PC[1], 2 ) = pow ( R, 2 )
        //
        //    The parametric form of a line in 2D is:
        //
        //      X = X0 + F * T
        //      Y = Y0 + G * T
        //
        //    For normalization, we choose F*F+G*G = 1 and 0 <= F.
        //
        //  Licensing:
        //
        //    This code is distributed under the GNU LGPL license.
        //
        //  Modified:
        //
        //    29 June 2005
        //
        //  Author:
        //
        //    John Burkardt
        //
        //  Parameters:
        //
        //    Input, double R, the radius of the circle.
        //
        //    Input, double PC[2], the coordinates of the center of the circle.
        //
        //    Input, double F, G, X0, Y0, the parametric parameters of the line.
        //
        //    Output, int *INT_NUM, the number of intersecting points found.
        //    INT_NUM will be 0, 1 or 2.
        //
        //    Output, double P[2*2], contains the X and Y coordinates of
        //    the intersecting points.
        //
        {
          double root;
          double t;

          root = r * r * ( f * f + g * g )
            - ( f * ( pc[1] - y0 ) - g * ( pc[0] - x0 ) )
            * ( f * ( pc[1] - y0 ) - g * ( pc[0] - x0 ) );

          if ( root < 0.0 )
          {
            int_num = 0;
          }
          else if ( root == 0.0 )
          {

            int_num = 1;

            t = ( f * ( pc[0] - x0 ) + g * ( pc[1] - y0 ) ) / ( f * f + g * g );
            p[0+0*2] = x0 + f * t;
            p[1+0*2] = y0 + g * t;

          }
          else //if ( root > 0.0 )
          {

            int_num = 2;

            t = ((f * (pc[0] - x0) + g * (pc[1] - y0)) - Math.Sqrt(root))
              / (f * f + g * g);

            p[0+0*2] = x0 + f * t;
            p[1+0*2] = y0 + g * t;

            t = ((f * (pc[0] - x0) + g * (pc[1] - y0)) + Math.Sqrt(root))
              / ( f * f + g * g );

            p[0+1*2] = x0 + f * t;
            p[1+1*2] = y0 + g * t;
          }

          return;
        }

        public static Point LineCircleIntersection(Line L, Circle C)
        {
		    int num_int;
            double[] O = { C.Center.X, C.Center.Y };
            double[] B = new double[2 * 2];

            circle_imp_line_par_int_2d(C.Radius, O, L.O.X, L.O.Y, L.Dir.X, L.Dir.Y, out num_int, B);
            if (num_int == 0)
                throw new Exception();
            else if (num_int == 1)
            {
                return new Point(B[0], B[1]);
            }
            else //if (num_int == 2)
            {
                var p0 = new Point(B[0], B[1]);
                var p1 = new Point(B[2], B[3]);
                if ((p0 - L.O).Length < (p1 - L.O).Length)
                    return p0;
                else
                    return p1;
            }
        }
    }
}
