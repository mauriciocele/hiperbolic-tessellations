using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace HyperbolicTessellation
{
    public class Mobius
    {
        ComplexMatrixNxM M;
        int R; //reflexion 1 or -1

        public Complex A { get { return M[0, 0]; } set { M[0, 0] = value; } }
        public Complex B { get { return M[0, 1]; } set { M[0, 1] = value; } }
        public Complex C { get { return M[1, 0]; } set { M[1, 0] = value; } }
        public Complex D { get { return M[1, 1]; } set { M[1, 1] = value; } }

        public Mobius(Complex a, Complex b, Complex c, Complex d, int R = 1)
        {
            M = new ComplexMatrixNxM(new Complex[,] { { a, b }, { c, d } });
            this.R = R;
        }

        public Mobius(ComplexMatrixNxM M, int R = 1)
        {
            Debug.Assert(M != null);
            Debug.Assert(M.cols == 2 && M.rows == 2);
            this.M = M;
            this.R = R;
        }

        public Complex Apply(Complex z)
        {
            if (R < 0)
                z = Complex.Conjugate(z);
            return (A * z + B) / (C * z + D);
        }

        //Rotacion en el origen
        public static Mobius Rotation(double angle)
        {
            return new Mobius(a: Exp(angle), b: Complex.Zero, c: Complex.Zero, d: Complex.One);
        }

        //Traslacion en el origen
        public static Mobius Translation(double x1, double y1)
        {
            return new Mobius(a: Complex.One, b: new Complex(x1, y1), c: Complex.Conjugate(new Complex(x1, y1)), d: Complex.One);
        }

        public static Mobius ReflectionAcrossLine(Complex l0, Complex l1)
        {
            var takel0ToOrigin = Translation(-l0.Real, -l0.Imaginary);
            var takeOriginTol0 = Translation(l0.Real, l0.Imaginary);

            Complex l1_ = takel0ToOrigin.Apply(l1);
            double angle = Math.Atan2(l1_.Imaginary, l1_.Real);
            Mobius rotatel1_ToXAxis = Rotation(-angle);
            Mobius rotateXAxisTol1_ = Rotation(angle);
            Mobius reflectAboutXAxis = new Mobius(a: Complex.One, b: Complex.Zero, c: Complex.Zero, d: Complex.One, R: -1);

            Mobius result = takeOriginTol0 * rotateXAxisTol1_ * reflectAboutXAxis * rotatel1_ToXAxis * takel0ToOrigin;
            return result;
        }

        // Composicion de transformaciones: (F1*F0)(z) = F1(F0(z))
        public static Mobius operator *(Mobius F1, Mobius F0)
        {
            if (F1.R < 0)
            {
                F0.A = Complex.Conjugate(F0.A);
                F0.B = Complex.Conjugate(F0.B);
                F0.C = Complex.Conjugate(F0.C);
                F0.D = Complex.Conjugate(F0.D);
            }
            return new Mobius(M: F1.M * F0.M, R: F1.R * F0.R);
        }

        public static Complex Exp(double x)
        {
            return new Complex(Math.Cos(x), Math.Sin(x));
        }
    }
}
