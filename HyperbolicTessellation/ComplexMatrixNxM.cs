using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace HyperbolicTessellation
{
    public class ComplexMatrixNxM
    {
        Complex[,] data;
        public int rows;
        public int cols;

        public ComplexMatrixNxM(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.data = new Complex[rows, cols];
        }

        public ComplexMatrixNxM(Complex[,] data)
        {
            this.rows = data.GetLength(0);
            this.cols = data.GetLength(1);
            this.data = new Complex[rows, cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    this.data[i, j] = data[i, j];
        }

        public ComplexMatrixNxM ZeroMatrix()
        {
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    data[i, j] = Complex.Zero;
            return this;
        }

        public ComplexMatrixNxM Identity()
        {
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                {
                    if (i == j)
                        data[i, j] = Complex.One;
                    else
                        data[i, j] = Complex.Zero;
                }
            return this;
        }

        public ComplexMatrixNxM CopyMatrix()
        {
            return ComplexMatrixNxM.CopyMatrix(this);
        }

        public ComplexMatrixNxM Transpose()
        {
            return ComplexMatrixNxM.Transpose(this);
        }

        public Complex this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        public static ComplexMatrixNxM operator *(ComplexMatrixNxM a, ComplexMatrixNxM b)
        {
            Debug.Assert(a.cols == b.rows);

            ComplexMatrixNxM c = new ComplexMatrixNxM(a.rows, b.cols);

            c.ZeroMatrix();

            for (int i = 0; i < a.rows; ++i)
                for (int j = 0; j < b.cols; ++j)
                    for (int k = 0; k < a.cols; ++k)
                        c[i, j] += a[i, k] * b[k, j];

            return c;
        }

        public static ComplexMatrixNxM operator *(ComplexMatrixNxM a, Complex b)
        {
            ComplexMatrixNxM c = new ComplexMatrixNxM(a.rows, a.cols);

            for (int i = 0; i < a.rows; ++i)
                for (int j = 0; j < a.cols; ++j)
                    c[i, j] = a[i, j] * b;

            return c;
        }

        public static ComplexMatrixNxM operator *(Complex b, ComplexMatrixNxM a)
        {
            return a * b;
        }

        public static ComplexMatrixNxM operator +(ComplexMatrixNxM a, ComplexMatrixNxM b)
        {
            Debug.Assert(a.rows == b.rows && a.cols == b.cols);

            ComplexMatrixNxM c = new ComplexMatrixNxM(a.rows, a.cols);

            for (int i = 0; i < a.rows; ++i)
                for (int j = 0; j < a.cols; ++j)
                    c[i, j] = a[i, j] + b[i, j];

            return c;
        }

        public static ComplexMatrixNxM Transpose(ComplexMatrixNxM a)
        {
            ComplexMatrixNxM c = new ComplexMatrixNxM(a.cols, a.rows);

            for (int i = 0; i < a.rows; ++i)
                for (int j = 0; j < a.cols; ++j)
                    c[j, i] = a[i, j];
            return c;
        }

        public static ComplexMatrixNxM CopyMatrix(ComplexMatrixNxM a)
        {
            ComplexMatrixNxM c = new ComplexMatrixNxM(a.rows, a.cols);

            for (int i = 0; i < a.rows; ++i)
                for (int j = 0; j < a.cols; ++j)
                    c[i, j] = a[i, j];

            return c;
        }
    }
}
