


using FixMath.NET;

namespace BEPUutilities
{
    /// <summary>
    /// 3 row, 2 column matrix.
    /// </summary>
    public struct Matrix3x2
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public Fix64 M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public Fix64 M12;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public Fix64 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public Fix64 M22;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public Fix64 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public Fix64 M32;


        /// <summary>
        /// Constructs a new 3 row, 2 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m31">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 2, column 2 of the matrix.</param>
        public Matrix3x2(Fix64 m11, Fix64 m12, Fix64 m21, Fix64 m22, Fix64 m31, Fix64 m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
        }


        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix3x2 a, ref Matrix3x2 b, out Matrix3x2 result)
        {
            Fix64 m11 = a.M11.Add(b.M11);
            Fix64 m12 = a.M12.Add(b.M12);

            Fix64 m21 = a.M21.Add(b.M21);
            Fix64 m22 = a.M22.Add(b.M22);

            Fix64 m31 = a.M31.Add(b.M31);
            Fix64 m32 = a.M32.Add(b.M32);

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix3x3 a, ref Matrix3x2 b, out Matrix3x2 result)
        {
            Fix64 resultM11 = ((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31));
            Fix64 resultM12 = ((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32));

            Fix64 resultM21 = ((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31));
            Fix64 resultM22 = ((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32));

            Fix64 resultM31 = ((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31));
            Fix64 resultM32 = ((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32));

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;

            result.M31 = resultM31;
            result.M32 = resultM32;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix a, ref Matrix3x2 b, out Matrix3x2 result)
        {
            Fix64 resultM11 = ((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31));
            Fix64 resultM12 = ((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32));

            Fix64 resultM21 = ((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31));
            Fix64 resultM22 = ((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32));

            Fix64 resultM31 = ((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31));
            Fix64 resultM32 = ((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32));

            result.M11 = resultM11;
            result.M12 = resultM12;

            result.M21 = resultM21;
            result.M22 = resultM22;

            result.M31 = resultM31;
            result.M32 = resultM32;
        }

        /// <summary>
        /// Negates every element in the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to negate.</param>
        /// <param name="result">Negated matrix.</param>
        public static void Negate(ref Matrix3x2 matrix, out Matrix3x2 result)
        {
            Fix64 m11 = matrix.M11.Neg();
            Fix64 m12 = matrix.M12.Neg();

            Fix64 m21 = matrix.M21.Neg();
            Fix64 m22 = matrix.M22.Neg();

            Fix64 m31 = matrix.M31.Neg();
            Fix64 m32 = matrix.M32.Neg();

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Subtracts the two matrices from each other on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to subtract.</param>
        /// <param name="b">Second matrix to subtract.</param>
        /// <param name="result">Difference of the two matrices.</param>
        public static void Subtract(ref Matrix3x2 a, ref Matrix3x2 b, out Matrix3x2 result)
        {
            Fix64 m11 = a.M11.Sub(b.M11);
            Fix64 m12 = a.M12.Sub(b.M12);

            Fix64 m21 = a.M21.Sub(b.M21);
            Fix64 m22 = a.M22.Sub(b.M22);

            Fix64 m31 = a.M31.Sub(b.M31);
            Fix64 m32 = a.M32.Sub(b.M32);

            result.M11 = m11;
            result.M12 = m12;

            result.M21 = m21;
            result.M22 = m22;

            result.M31 = m31;
            result.M32 = m32;
        }

        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector2 to transform.  Considered to be a column vector for purposes of multiplication.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Column vector product of the transformation.</param>
        public static void Transform(ref Vector2 v, ref Matrix3x2 matrix, out Vector3 result)
        {
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = (matrix.M11.Mul(v.X)).Add(matrix.M12.Mul(v.Y));
            result.Y = (matrix.M21.Mul(v.X)).Add(matrix.M22.Mul(v.Y));
            result.Z = (matrix.M31.Mul(v.X)).Add(matrix.M32.Mul(v.Y));
        }

        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector2 to transform.  Considered to be a row vector for purposes of multiplication.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Row vector product of the transformation.</param>
        public static void Transform(ref Vector3 v, ref Matrix3x2 matrix, out Vector2 result)
        {
#if !WINDOWS
            result = new Vector2();
#endif
            result.X = ((v.X.Mul(matrix.M11)).Add(v.Y.Mul(matrix.M21))).Add(v.Z.Mul(matrix.M31));
            result.Y = ((v.X.Mul(matrix.M12)).Add(v.Y.Mul(matrix.M22))).Add(v.Z.Mul(matrix.M32));
        }


        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix3x2 matrix, out Matrix2x3 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;

            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
        }


        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + "} " +
                   "{" + M21 + ", " + M22 + "} " +
                   "{" + M31 + ", " + M32 + "}";
        }
    }
}