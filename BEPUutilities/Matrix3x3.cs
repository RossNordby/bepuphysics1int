using FixMath.NET;
using System;
 


namespace BEPUutilities
{
    /// <summary>
    /// 3 row, 3 column matrix.
    /// </summary>
    public struct Matrix3x3
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
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public Fix64 M13;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public Fix64 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public Fix64 M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public Fix64 M23;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public Fix64 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public Fix64 M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public Fix64 M33;

        /// <summary>
        /// Constructs a new 3 row, 3 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
        /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
        /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
        public Matrix3x3(Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m31, Fix64 m32, Fix64 m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        /// Gets the 3x3 identity matrix.
        /// </summary>
        public static Matrix3x3 Identity
        {
            get { return new Matrix3x3(F64.C1, F64.C0, F64.C0, F64.C0, F64.C1, F64.C0, F64.C0, F64.C0, F64.C1); }
        }


        /// <summary>
        /// Gets or sets the backward vector of the matrix.
        /// </summary>
        public Vector3 Backward
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M31;
                vector.Y = M32;
                vector.Z = M33;
                return vector;
            }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix.
        /// </summary>
        public Vector3 Down
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M21.Neg();
                vector.Y = M22.Neg();
                vector.Z = M23.Neg();
                return vector;
            }
            set
            {
                M21 = value.X.Neg();
                M22 = value.Y.Neg();
                M23 = value.Z.Neg();
            }
        }

        /// <summary>
        /// Gets or sets the forward vector of the matrix.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M31.Neg();
                vector.Y = M32.Neg();
                vector.Z = M33.Neg();
                return vector;
            }
            set
            {
                M31 = value.X.Neg();
                M32 = value.Y.Neg();
                M33 = value.Z.Neg();
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix.
        /// </summary>
        public Vector3 Left
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M11.Neg();
                vector.Y = M12.Neg();
                vector.Z = M13.Neg();
                return vector;
            }
            set
            {
                M11 = value.X.Neg();
                M12 = value.Y.Neg();
                M13 = value.Z.Neg();
            }
        }

        /// <summary>
        /// Gets or sets the right vector of the matrix.
        /// </summary>
        public Vector3 Right
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M11;
                vector.Y = M12;
                vector.Z = M13;
                return vector;
            }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the matrix.
        /// </summary>
        public Vector3 Up
        {
            get
            {
#if !WINDOWS
                Vector3 vector = new Vector3();
#else
                Vector3 vector;
#endif
                vector.X = M21;
                vector.Y = M22;
                vector.Z = M23;
                return vector;
            }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            Fix64 m11 = a.M11.Add(b.M11);
            Fix64 m12 = a.M12.Add(b.M12);
            Fix64 m13 = a.M13.Add(b.M13);

            Fix64 m21 = a.M21.Add(b.M21);
            Fix64 m22 = a.M22.Add(b.M22);
            Fix64 m23 = a.M23.Add(b.M23);

            Fix64 m31 = a.M31.Add(b.M31);
            Fix64 m32 = a.M32.Add(b.M32);
            Fix64 m33 = a.M33.Add(b.M33);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            Fix64 m11 = a.M11.Add(b.M11);
            Fix64 m12 = a.M12.Add(b.M12);
            Fix64 m13 = a.M13.Add(b.M13);

            Fix64 m21 = a.M21.Add(b.M21);
            Fix64 m22 = a.M22.Add(b.M22);
            Fix64 m23 = a.M23.Add(b.M23);

            Fix64 m31 = a.M31.Add(b.M31);
            Fix64 m32 = a.M32.Add(b.M32);
            Fix64 m33 = a.M33.Add(b.M33);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix3x3 a, ref Matrix b, out Matrix3x3 result)
        {
            Fix64 m11 = a.M11.Add(b.M11);
            Fix64 m12 = a.M12.Add(b.M12);
            Fix64 m13 = a.M13.Add(b.M13);

            Fix64 m21 = a.M21.Add(b.M21);
            Fix64 m22 = a.M22.Add(b.M22);
            Fix64 m23 = a.M23.Add(b.M23);

            Fix64 m31 = a.M31.Add(b.M31);
            Fix64 m32 = a.M32.Add(b.M32);
            Fix64 m33 = a.M33.Add(b.M33);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Adds the two matrices together on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to add.</param>
        /// <param name="b">Second matrix to add.</param>
        /// <param name="result">Sum of the two matrices.</param>
        public static void Add(ref Matrix a, ref Matrix b, out Matrix3x3 result)
        {
            Fix64 m11 = a.M11.Add(b.M11);
            Fix64 m12 = a.M12.Add(b.M12);
            Fix64 m13 = a.M13.Add(b.M13);

            Fix64 m21 = a.M21.Add(b.M21);
            Fix64 m22 = a.M22.Add(b.M22);
            Fix64 m23 = a.M23.Add(b.M23);

            Fix64 m31 = a.M31.Add(b.M31);
            Fix64 m32 = a.M32.Add(b.M32);
            Fix64 m33 = a.M33.Add(b.M33);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Creates a skew symmetric matrix M from vector A such that M * B for some other vector B is equivalent to the cross product of A and B.
        /// </summary>
        /// <param name="v">Vector to base the matrix on.</param>
        /// <param name="result">Skew-symmetric matrix result.</param>
        public static void CreateCrossProduct(ref Vector3 v, out Matrix3x3 result)
        {
            result.M11 = F64.C0;
            result.M12 = v.Z.Neg();
            result.M13 = v.Y;
            result.M21 = v.Z;
            result.M22 = F64.C0;
            result.M23 = v.X.Neg();
            result.M31 = v.Y.Neg();
            result.M32 = v.X;
            result.M33 = F64.C0;
        }

        /// <summary>
        /// Creates a 3x3 matrix from an XNA 4x4 matrix.
        /// </summary>
        /// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
        /// <param name="matrix3X3">Upper 3x3 matrix extracted from the XNA matrix.</param>
        public static void CreateFromMatrix(ref Matrix matrix4X4, out Matrix3x3 matrix3X3)
        {
            matrix3X3.M11 = matrix4X4.M11;
            matrix3X3.M12 = matrix4X4.M12;
            matrix3X3.M13 = matrix4X4.M13;

            matrix3X3.M21 = matrix4X4.M21;
            matrix3X3.M22 = matrix4X4.M22;
            matrix3X3.M23 = matrix4X4.M23;

            matrix3X3.M31 = matrix4X4.M31;
            matrix3X3.M32 = matrix4X4.M32;
            matrix3X3.M33 = matrix4X4.M33;
        }
        /// <summary>
        /// Creates a 3x3 matrix from an XNA 4x4 matrix.
        /// </summary>
        /// <param name="matrix4X4">Matrix to extract a 3x3 matrix from.</param>
        /// <returns>Upper 3x3 matrix extracted from the XNA matrix.</returns>
        public static Matrix3x3 CreateFromMatrix(Matrix matrix4X4)
        {
            Matrix3x3 matrix3X3;
            matrix3X3.M11 = matrix4X4.M11;
            matrix3X3.M12 = matrix4X4.M12;
            matrix3X3.M13 = matrix4X4.M13;

            matrix3X3.M21 = matrix4X4.M21;
            matrix3X3.M22 = matrix4X4.M22;
            matrix3X3.M23 = matrix4X4.M23;

            matrix3X3.M31 = matrix4X4.M31;
            matrix3X3.M32 = matrix4X4.M32;
            matrix3X3.M33 = matrix4X4.M33;
            return matrix3X3;
        }

        /// <summary>
        /// Constructs a uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Value to use in the diagonal.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(Fix64 scale, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = scale, M22 = scale, M33 = scale};
        }

        /// <summary>
        /// Constructs a uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Value to use in the diagonal.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(Fix64 scale)
        {
            var matrix = new Matrix3x3 {M11 = scale, M22 = scale, M33 = scale};
            return matrix;
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Values defining the axis scales.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(ref Vector3 scale, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = scale.X, M22 = scale.Y, M33 = scale.Z};
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="scale">Values defining the axis scales.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(ref Vector3 scale)
        {
            var matrix = new Matrix3x3 {M11 = scale.X, M22 = scale.Y, M33 = scale.Z};
            return matrix;
        }


        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="x">Scaling along the x axis.</param>
        /// <param name="y">Scaling along the y axis.</param>
        /// <param name="z">Scaling along the z axis.</param>
        /// <param name="matrix">Scaling matrix.</param>
        public static void CreateScale(Fix64 x, Fix64 y, Fix64 z, out Matrix3x3 matrix)
        {
            matrix = new Matrix3x3 {M11 = x, M22 = y, M33 = z};
        }

        /// <summary>
        /// Constructs a non-uniform scaling matrix.
        /// </summary>
        /// <param name="x">Scaling along the x axis.</param>
        /// <param name="y">Scaling along the y axis.</param>
        /// <param name="z">Scaling along the z axis.</param>
        /// <returns>Scaling matrix.</returns>
        public static Matrix3x3 CreateScale(Fix64 x, Fix64 y, Fix64 z)
        {
            var matrix = new Matrix3x3 {M11 = x, M22 = y, M33 = z};
            return matrix;
        }

		/// <summary>
		/// Inverts the given matix.
		/// </summary>
		/// <param name="matrix">Matrix to be inverted.</param>
		/// <param name="result">Inverted matrix.</param>
		/// <returns>false if matrix is singular, true otherwise</returns>
		public static bool Invert(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
			return Matrix3x6.Invert(ref matrix, out result);
        }

        /// <summary>
        /// Inverts the given matix.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <returns>Inverted matrix.</returns>
        public static Matrix3x3 Invert(Matrix3x3 matrix)
        {
            Matrix3x3 toReturn;
            Invert(ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Inverts the largest nonsingular submatrix in the matrix, excluding 2x2's that involve M13 or M31, and excluding 1x1's that include nondiagonal elements.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <param name="result">Inverted matrix.</param>
        public static void AdaptiveInvert(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
			// Perform full Gauss-invert and return if successful
			if (Invert(ref matrix, out result))
				return;

			int submatrix;
            Fix64 determinantInverse = F64.C1.Div(matrix.AdaptiveDeterminant(out submatrix));
            Fix64 m11, m12, m13, m21, m22, m23, m31, m32, m33;
            switch (submatrix)
            {
                case 1: //Upper left matrix, m11, m12, m21, m22.
                    m11 = matrix.M22.Mul(determinantInverse);
                    m12 = (matrix.M12.Neg()).Mul(determinantInverse);
                    m13 = F64.C0;

                    m21 = (matrix.M21.Neg()).Mul(determinantInverse);
                    m22 = matrix.M11.Mul(determinantInverse);
                    m23 = F64.C0;

                    m31 = F64.C0;
                    m32 = F64.C0;
                    m33 = F64.C0;
                    break;
                case 2: //Lower right matrix, m22, m23, m32, m33.
                    m11 = F64.C0;
                    m12 = F64.C0;
                    m13 = F64.C0;

                    m21 = F64.C0;
                    m22 = matrix.M33.Mul(determinantInverse);
                    m23 = (matrix.M23.Neg()).Mul(determinantInverse);

                    m31 = F64.C0;
                    m32 = (matrix.M32.Neg()).Mul(determinantInverse);
                    m33 = matrix.M22.Mul(determinantInverse);
                    break;
                case 3: //Corners, m11, m31, m13, m33.
                    m11 = matrix.M33.Mul(determinantInverse);
                    m12 = F64.C0;
                    m13 = (matrix.M13.Neg()).Mul(determinantInverse);

                    m21 = F64.C0;
                    m22 = F64.C0;
                    m23 = F64.C0;

                    m31 = (matrix.M31.Neg()).Mul(determinantInverse);
                    m32 = F64.C0;
                    m33 = matrix.M11.Mul(determinantInverse);
                    break;
                case 4: //M11
                    m11 = F64.C1.Div(matrix.M11);
                    m12 = F64.C0;
                    m13 = F64.C0;

                    m21 = F64.C0;
                    m22 = F64.C0;
                    m23 = F64.C0;

                    m31 = F64.C0;
                    m32 = F64.C0;
                    m33 = F64.C0;
                    break;
                case 5: //M22
                    m11 = F64.C0;
                    m12 = F64.C0;
                    m13 = F64.C0;

                    m21 = F64.C0;
                    m22 = F64.C1.Div(matrix.M22);
                    m23 = F64.C0;

                    m31 = F64.C0;
                    m32 = F64.C0;
                    m33 = F64.C0;
                    break;
                case 6: //M33
                    m11 = F64.C0;
                    m12 = F64.C0;
                    m13 = F64.C0;

                    m21 = F64.C0;
                    m22 = F64.C0;
                    m23 = F64.C0;

                    m31 = F64.C0;
                    m32 = F64.C0;
                    m33 = F64.C1.Div(matrix.M33);
                    break;
                default: //Completely singular.
                    m11 = F64.C0; m12 = F64.C0; m13 = F64.C0; m21 = F64.C0; m22 = F64.C0; m23 = F64.C0; m31 = F64.C0; m32 = F64.C0; m33 = F64.C0;
                    break;
            }

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// <para>Computes the adjugate transpose of a matrix.</para>
        /// <para>The adjugate transpose A of matrix M is: det(M) * transpose(invert(M))</para>
        /// <para>This is necessary when transforming normals (bivectors) with general linear transformations.</para>
        /// </summary>
        /// <param name="matrix">Matrix to compute the adjugate transpose of.</param>
        /// <param name="result">Adjugate transpose of the input matrix.</param>
        public static void AdjugateTranspose(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            //Despite the relative obscurity of the operation, this is a fairly straightforward operation which is actually faster than a true invert (by virtue of cancellation).
            //Conceptually, this is implemented as transpose(det(M) * invert(M)), but that's perfectly acceptable:
            //1) transpose(invert(M)) == invert(transpose(M)), and
            //2) det(M) == det(transpose(M))
            //This organization makes it clearer that the invert's usual division by determinant drops out.

            Fix64 m11 = ((matrix.M22.Mul(matrix.M33)).Sub(matrix.M23.Mul(matrix.M32)));
            Fix64 m12 = ((matrix.M13.Mul(matrix.M32)).Sub(matrix.M33.Mul(matrix.M12)));
            Fix64 m13 = ((matrix.M12.Mul(matrix.M23)).Sub(matrix.M22.Mul(matrix.M13)));

            Fix64 m21 = ((matrix.M23.Mul(matrix.M31)).Sub(matrix.M21.Mul(matrix.M33)));
            Fix64 m22 = ((matrix.M11.Mul(matrix.M33)).Sub(matrix.M13.Mul(matrix.M31)));
            Fix64 m23 = ((matrix.M13.Mul(matrix.M21)).Sub(matrix.M11.Mul(matrix.M23)));

            Fix64 m31 = ((matrix.M21.Mul(matrix.M32)).Sub(matrix.M22.Mul(matrix.M31)));
            Fix64 m32 = ((matrix.M12.Mul(matrix.M31)).Sub(matrix.M11.Mul(matrix.M32)));
            Fix64 m33 = ((matrix.M11.Mul(matrix.M22)).Sub(matrix.M12.Mul(matrix.M21)));

            //Note transposition.
            result.M11 = m11;
            result.M12 = m21;
            result.M13 = m31;

            result.M21 = m12;
            result.M22 = m22;
            result.M23 = m32;

            result.M31 = m13;
            result.M32 = m23;
            result.M33 = m33;
        }

        /// <summary>
        /// <para>Computes the adjugate transpose of a matrix.</para>
        /// <para>The adjugate transpose A of matrix M is: det(M) * transpose(invert(M))</para>
        /// <para>This is necessary when transforming normals (bivectors) with general linear transformations.</para>
        /// </summary>
        /// <param name="matrix">Matrix to compute the adjugate transpose of.</param>
        /// <returns>Adjugate transpose of the input matrix.</returns>
        public static Matrix3x3 AdjugateTranspose(Matrix3x3 matrix)
        {
            Matrix3x3 toReturn;
            AdjugateTranspose(ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            Matrix3x3 result;
            Matrix3x3.Multiply(ref a, ref b, out result);
            return result;
        }        

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(Matrix3x3 m, Fix64 f)
        {
            Matrix3x3 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix3x3 operator *(Fix64 f, Matrix3x3 m)
        {
            Matrix3x3 result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            Fix64 resultM11 = ((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31));
            Fix64 resultM12 = ((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32));
            Fix64 resultM13 = ((a.M11.Mul(b.M13)).Add(a.M12.Mul(b.M23))).Add(a.M13.Mul(b.M33));

            Fix64 resultM21 = ((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31));
            Fix64 resultM22 = ((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32));
            Fix64 resultM23 = ((a.M21.Mul(b.M13)).Add(a.M22.Mul(b.M23))).Add(a.M23.Mul(b.M33));

            Fix64 resultM31 = ((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31));
            Fix64 resultM32 = ((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32));
            Fix64 resultM33 = ((a.M31.Mul(b.M13)).Add(a.M32.Mul(b.M23))).Add(a.M33.Mul(b.M33));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix3x3 a, ref Matrix b, out Matrix3x3 result)
        {
            Fix64 resultM11 = ((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31));
            Fix64 resultM12 = ((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32));
            Fix64 resultM13 = ((a.M11.Mul(b.M13)).Add(a.M12.Mul(b.M23))).Add(a.M13.Mul(b.M33));

            Fix64 resultM21 = ((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31));
            Fix64 resultM22 = ((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32));
            Fix64 resultM23 = ((a.M21.Mul(b.M13)).Add(a.M22.Mul(b.M23))).Add(a.M23.Mul(b.M33));

            Fix64 resultM31 = ((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31));
            Fix64 resultM32 = ((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32));
            Fix64 resultM33 = ((a.M31.Mul(b.M13)).Add(a.M32.Mul(b.M23))).Add(a.M33.Mul(b.M33));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies the two matrices.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Matrix a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            Fix64 resultM11 = ((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31));
            Fix64 resultM12 = ((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32));
            Fix64 resultM13 = ((a.M11.Mul(b.M13)).Add(a.M12.Mul(b.M23))).Add(a.M13.Mul(b.M33));

            Fix64 resultM21 = ((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31));
            Fix64 resultM22 = ((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32));
            Fix64 resultM23 = ((a.M21.Mul(b.M13)).Add(a.M22.Mul(b.M23))).Add(a.M23.Mul(b.M33));

            Fix64 resultM31 = ((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31));
            Fix64 resultM32 = ((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32));
            Fix64 resultM33 = ((a.M31.Mul(b.M13)).Add(a.M32.Mul(b.M23))).Add(a.M33.Mul(b.M33));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }


        /// <summary>
        /// Multiplies a transposed matrix with another matrix.
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        /// <param name="transpose">Matrix to be transposed and multiplied.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void MultiplyTransposed(ref Matrix3x3 transpose, ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            Fix64 resultM11 = ((transpose.M11.Mul(matrix.M11)).Add(transpose.M21.Mul(matrix.M21))).Add(transpose.M31.Mul(matrix.M31));
            Fix64 resultM12 = ((transpose.M11.Mul(matrix.M12)).Add(transpose.M21.Mul(matrix.M22))).Add(transpose.M31.Mul(matrix.M32));
            Fix64 resultM13 = ((transpose.M11.Mul(matrix.M13)).Add(transpose.M21.Mul(matrix.M23))).Add(transpose.M31.Mul(matrix.M33));

            Fix64 resultM21 = ((transpose.M12.Mul(matrix.M11)).Add(transpose.M22.Mul(matrix.M21))).Add(transpose.M32.Mul(matrix.M31));
            Fix64 resultM22 = ((transpose.M12.Mul(matrix.M12)).Add(transpose.M22.Mul(matrix.M22))).Add(transpose.M32.Mul(matrix.M32));
            Fix64 resultM23 = ((transpose.M12.Mul(matrix.M13)).Add(transpose.M22.Mul(matrix.M23))).Add(transpose.M32.Mul(matrix.M33));

            Fix64 resultM31 = ((transpose.M13.Mul(matrix.M11)).Add(transpose.M23.Mul(matrix.M21))).Add(transpose.M33.Mul(matrix.M31));
            Fix64 resultM32 = ((transpose.M13.Mul(matrix.M12)).Add(transpose.M23.Mul(matrix.M22))).Add(transpose.M33.Mul(matrix.M32));
            Fix64 resultM33 = ((transpose.M13.Mul(matrix.M13)).Add(transpose.M23.Mul(matrix.M23))).Add(transpose.M33.Mul(matrix.M33));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Multiplies a matrix with a transposed matrix.
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        /// <param name="transpose">Matrix to be transposed and multiplied.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void MultiplyByTransposed(ref Matrix3x3 matrix, ref Matrix3x3 transpose, out Matrix3x3 result)
        {
            Fix64 resultM11 = ((matrix.M11.Mul(transpose.M11)).Add(matrix.M12.Mul(transpose.M12))).Add(matrix.M13.Mul(transpose.M13));
            Fix64 resultM12 = ((matrix.M11.Mul(transpose.M21)).Add(matrix.M12.Mul(transpose.M22))).Add(matrix.M13.Mul(transpose.M23));
            Fix64 resultM13 = ((matrix.M11.Mul(transpose.M31)).Add(matrix.M12.Mul(transpose.M32))).Add(matrix.M13.Mul(transpose.M33));

            Fix64 resultM21 = ((matrix.M21.Mul(transpose.M11)).Add(matrix.M22.Mul(transpose.M12))).Add(matrix.M23.Mul(transpose.M13));
            Fix64 resultM22 = ((matrix.M21.Mul(transpose.M21)).Add(matrix.M22.Mul(transpose.M22))).Add(matrix.M23.Mul(transpose.M23));
            Fix64 resultM23 = ((matrix.M21.Mul(transpose.M31)).Add(matrix.M22.Mul(transpose.M32))).Add(matrix.M23.Mul(transpose.M33));

            Fix64 resultM31 = ((matrix.M31.Mul(transpose.M11)).Add(matrix.M32.Mul(transpose.M12))).Add(matrix.M33.Mul(transpose.M13));
            Fix64 resultM32 = ((matrix.M31.Mul(transpose.M21)).Add(matrix.M32.Mul(transpose.M22))).Add(matrix.M33.Mul(transpose.M23));
            Fix64 resultM33 = ((matrix.M31.Mul(transpose.M31)).Add(matrix.M32.Mul(transpose.M32))).Add(matrix.M33.Mul(transpose.M33));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
        }

        /// <summary>
        /// Scales all components of the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled matrix.</param>
        public static void Multiply(ref Matrix3x3 matrix, Fix64 scale, out Matrix3x3 result)
        {
            result.M11 = matrix.M11.Mul(scale);
            result.M12 = matrix.M12.Mul(scale);
            result.M13 = matrix.M13.Mul(scale);

            result.M21 = matrix.M21.Mul(scale);
            result.M22 = matrix.M22.Mul(scale);
            result.M23 = matrix.M23.Mul(scale);

            result.M31 = matrix.M31.Mul(scale);
            result.M32 = matrix.M32.Mul(scale);
            result.M33 = matrix.M33.Mul(scale);
        }

        /// <summary>
        /// Negates every element in the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to negate.</param>
        /// <param name="result">Negated matrix.</param>
        public static void Negate(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            result.M11 = matrix.M11.Neg();
            result.M12 = matrix.M12.Neg();
            result.M13 = matrix.M13.Neg();

            result.M21 = matrix.M21.Neg();
            result.M22 = matrix.M22.Neg();
            result.M23 = matrix.M23.Neg();

            result.M31 = matrix.M31.Neg();
            result.M32 = matrix.M32.Neg();
            result.M33 = matrix.M33.Neg();
        }

        /// <summary>
        /// Subtracts the two matrices from each other on a per-element basis.
        /// </summary>
        /// <param name="a">First matrix to subtract.</param>
        /// <param name="b">Second matrix to subtract.</param>
        /// <param name="result">Difference of the two matrices.</param>
        public static void Subtract(ref Matrix3x3 a, ref Matrix3x3 b, out Matrix3x3 result)
        {
            Fix64 m11 = a.M11.Sub(b.M11);
            Fix64 m12 = a.M12.Sub(b.M12);
            Fix64 m13 = a.M13.Sub(b.M13);

            Fix64 m21 = a.M21.Sub(b.M21);
            Fix64 m22 = a.M22.Sub(b.M22);
            Fix64 m23 = a.M23.Sub(b.M23);

            Fix64 m31 = a.M31.Sub(b.M31);
            Fix64 m32 = a.M32.Sub(b.M32);
            Fix64 m33 = a.M33.Sub(b.M33);

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Creates a 4x4 matrix from a 3x3 matrix.
        /// </summary>
        /// <param name="a">3x3 matrix.</param>
        /// <param name="b">Created 4x4 matrix.</param>
        public static void ToMatrix4X4(ref Matrix3x3 a, out Matrix b)
        {
#if !WINDOWS
            b = new Matrix();
#endif
            b.M11 = a.M11;
            b.M12 = a.M12;
            b.M13 = a.M13;

            b.M21 = a.M21;
            b.M22 = a.M22;
            b.M23 = a.M23;

            b.M31 = a.M31;
            b.M32 = a.M32;
            b.M33 = a.M33;

            b.M44 = F64.C1;
            b.M14 = F64.C0;
            b.M24 = F64.C0;
            b.M34 = F64.C0;
            b.M41 = F64.C0;
            b.M42 = F64.C0;
            b.M43 = F64.C0;
        }

        /// <summary>
        /// Creates a 4x4 matrix from a 3x3 matrix.
        /// </summary>
        /// <param name="a">3x3 matrix.</param>
        /// <returns>Created 4x4 matrix.</returns>
        public static Matrix ToMatrix4X4(Matrix3x3 a)
        {
#if !WINDOWS
            Matrix b = new Matrix();
#else
            Matrix b;
#endif
            b.M11 = a.M11;
            b.M12 = a.M12;
            b.M13 = a.M13;

            b.M21 = a.M21;
            b.M22 = a.M22;
            b.M23 = a.M23;

            b.M31 = a.M31;
            b.M32 = a.M32;
            b.M33 = a.M33;

            b.M44 = F64.C1;
            b.M14 = F64.C0;
            b.M24 = F64.C0;
            b.M34 = F64.C0;
            b.M41 = F64.C0;
            b.M42 = F64.C0;
            b.M43 = F64.C0;
            return b;
        }
        
        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <param name="result">Product of the transformation.</param>
        public static void Transform(ref Vector3 v, ref Matrix3x3 matrix, out Vector3 result)
        {
            Fix64 vX = v.X;
            Fix64 vY = v.Y;
            Fix64 vZ = v.Z;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M21))).Add(vZ.Mul(matrix.M31));
            result.Y = ((vX.Mul(matrix.M12)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M32));
            result.Z = ((vX.Mul(matrix.M13)).Add(vY.Mul(matrix.M23))).Add(vZ.Mul(matrix.M33));
        }


        /// <summary>
        /// Transforms the vector by the matrix.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation.</param>
        /// <returns>Product of the transformation.</returns>
        public static Vector3 Transform(Vector3 v, Matrix3x3 matrix)
        {
            Vector3 result;
#if !WINDOWS
            result = new Vector3();
#endif
            Fix64 vX = v.X;
            Fix64 vY = v.Y;
            Fix64 vZ = v.Z;

            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M21))).Add(vZ.Mul(matrix.M31));
            result.Y = ((vX.Mul(matrix.M12)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M32));
            result.Z = ((vX.Mul(matrix.M13)).Add(vY.Mul(matrix.M23))).Add(vZ.Mul(matrix.M33));
            return result;
        }

        /// <summary>
        /// Transforms the vector by the matrix's transpose.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation transpose.</param>
        /// <param name="result">Product of the transformation.</param>
        public static void TransformTranspose(ref Vector3 v, ref Matrix3x3 matrix, out Vector3 result)
        {
            Fix64 vX = v.X;
            Fix64 vY = v.Y;
            Fix64 vZ = v.Z;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M12))).Add(vZ.Mul(matrix.M13));
            result.Y = ((vX.Mul(matrix.M21)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M23));
            result.Z = ((vX.Mul(matrix.M31)).Add(vY.Mul(matrix.M32))).Add(vZ.Mul(matrix.M33));
        }

        /// <summary>
        /// Transforms the vector by the matrix's transpose.
        /// </summary>
        /// <param name="v">Vector3 to transform.</param>
        /// <param name="matrix">Matrix to use as the transformation transpose.</param>
        /// <returns>Product of the transformation.</returns>
        public static Vector3 TransformTranspose(Vector3 v, Matrix3x3 matrix)
        {
            Fix64 vX = v.X;
            Fix64 vY = v.Y;
            Fix64 vZ = v.Z;
            Vector3 result;
#if !WINDOWS
            result = new Vector3();
#endif
            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M12))).Add(vZ.Mul(matrix.M13));
            result.Y = ((vX.Mul(matrix.M21)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M23));
            result.Z = ((vX.Mul(matrix.M31)).Add(vY.Mul(matrix.M32))).Add(vZ.Mul(matrix.M33));
            return result;
        }

        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            Fix64 m21 = matrix.M12;
            Fix64 m31 = matrix.M13;
            Fix64 m12 = matrix.M21;
            Fix64 m32 = matrix.M23;
            Fix64 m13 = matrix.M31;
            Fix64 m23 = matrix.M32;

            result.M11 = matrix.M11;
            result.M12 = m12;
            result.M13 = m13;
            result.M21 = m21;
            result.M22 = matrix.M22;
            result.M23 = m23;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = matrix.M33;
        }

        /// <summary>
        /// Computes the transposed matrix of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <param name="result">Transposed matrix.</param>
        public static void Transpose(ref Matrix matrix, out Matrix3x3 result)
        {
            Fix64 m21 = matrix.M12;
            Fix64 m31 = matrix.M13;
            Fix64 m12 = matrix.M21;
            Fix64 m32 = matrix.M23;
            Fix64 m13 = matrix.M31;
            Fix64 m23 = matrix.M32;

            result.M11 = matrix.M11;
            result.M12 = m12;
            result.M13 = m13;
            result.M21 = m21;
            result.M22 = matrix.M22;
            result.M23 = m23;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = matrix.M33;
        }
       
        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose()
        {
            Fix64 intermediate = M12;
            M12 = M21;
            M21 = intermediate;

            intermediate = M13;
            M13 = M31;
            M31 = intermediate;

            intermediate = M23;
            M23 = M32;
            M32 = intermediate;
        }


        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11 + ", " + M12 + ", " + M13 + "} " +
                   "{" + M21 + ", " + M22 + ", " + M23 + "} " +
                   "{" + M31 + ", " + M32 + ", " + M33 + "}";
        }		

        /// <summary>
        /// Calculates the determinant of largest nonsingular submatrix, excluding 2x2's that involve M13 or M31, and excluding all 1x1's that involve nondiagonal elements.
        /// </summary>
        /// <param name="subMatrixCode">Represents the submatrix that was used to compute the determinant.
        /// 0 is the full 3x3.  1 is the upper left 2x2.  2 is the lower right 2x2.  3 is the four corners.
        /// 4 is M11.  5 is M22.  6 is M33.</param>
        /// <returns>The matrix's determinant.</returns>
        internal Fix64 AdaptiveDeterminant(out int subMatrixCode)
        {
            // We do not try the full matrix. This is handled by the AdaptiveInverse.

			// We'll play it fast and loose here and assume the following won't overflow
            //Try m11, m12, m21, m22.
            Fix64 determinant = (M11.Mul(M22)).Sub(M12.Mul(M21));
            if (determinant != F64.C0)
            {
                subMatrixCode = 1;
                return determinant;
            }
            //Try m22, m23, m32, m33.
            determinant = (M22.Mul(M33)).Sub(M23.Mul(M32));
            if (determinant != F64.C0)
            {
                subMatrixCode = 2;
                return determinant;
            }
            //Try m11, m13, m31, m33.
            determinant = (M11.Mul(M33)).Sub(M13.Mul(M12));
            if (determinant != F64.C0)
            {
                subMatrixCode = 3;
                return determinant;
            }
            //Try m11.
            if (M11 != F64.C0)
            {
                subMatrixCode = 4;
                return M11;
            }
            //Try m22.
            if (M22 != F64.C0)
            {
                subMatrixCode = 5;
                return M22;
            }
            //Try m33.
            if (M33 != F64.C0)
            {
                subMatrixCode = 6;
                return M33;
            }
            //It's completely singular!
            subMatrixCode = -1;
            return F64.C0;
        }
        
        /// <summary>
        /// Creates a 3x3 matrix representing the orientation stored in the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to use to create a matrix.</param>
        /// <param name="result">Matrix representing the quaternion's orientation.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix3x3 result)
        {
            Fix64 qX2 = quaternion.X.Add(quaternion.X);
            Fix64 qY2 = quaternion.Y.Add(quaternion.Y);
            Fix64 qZ2 = quaternion.Z.Add(quaternion.Z);
            Fix64 XX = qX2.Mul(quaternion.X);
            Fix64 YY = qY2.Mul(quaternion.Y);
            Fix64 ZZ = qZ2.Mul(quaternion.Z);
            Fix64 XY = qX2.Mul(quaternion.Y);
            Fix64 XZ = qX2.Mul(quaternion.Z);
            Fix64 XW = qX2.Mul(quaternion.W);
            Fix64 YZ = qY2.Mul(quaternion.Z);
            Fix64 YW = qY2.Mul(quaternion.W);
            Fix64 ZW = qZ2.Mul(quaternion.W);

            result.M11 = (F64.C1.Sub(YY)).Sub(ZZ);
            result.M21 = XY.Sub(ZW);
            result.M31 = XZ.Add(YW);

            result.M12 = XY.Add(ZW);
            result.M22 = (F64.C1.Sub(XX)).Sub(ZZ);
            result.M32 = YZ.Sub(XW);

            result.M13 = XZ.Sub(YW);
            result.M23 = YZ.Add(XW);
            result.M33 = (F64.C1.Sub(XX)).Sub(YY);
        }

        /// <summary>
        /// Creates a 3x3 matrix representing the orientation stored in the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to use to create a matrix.</param>
        /// <returns>Matrix representing the quaternion's orientation.</returns>
        public static Matrix3x3 CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix3x3 result;
            CreateFromQuaternion(ref quaternion, out result);
            return result;
        }

        /// <summary>
        /// Computes the outer product of the given vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="result">Outer product result.</param>
        public static void CreateOuterProduct(ref Vector3 a, ref Vector3 b, out Matrix3x3 result)
        {
            result.M11 = a.X.Mul(b.X);
            result.M12 = a.X.Mul(b.Y);
            result.M13 = a.X.Mul(b.Z);

            result.M21 = a.Y.Mul(b.X);
            result.M22 = a.Y.Mul(b.Y);
            result.M23 = a.Y.Mul(b.Z);

            result.M31 = a.Z.Mul(b.X);
            result.M32 = a.Z.Mul(b.Y);
            result.M33 = a.Z.Mul(b.Z);
        }

        /// <summary>
        /// Creates a matrix representing a rotation of a given angle around a given axis.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <returns>Matrix representing the rotation.</returns>
        public static Matrix3x3 CreateFromAxisAngle(Vector3 axis, Fix64 angle)
        {
            Matrix3x3 toReturn;
            CreateFromAxisAngle(ref axis, angle, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a matrix representing a rotation of a given angle around a given axis.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Amount to rotate.</param>
        /// <param name="result">Matrix representing the rotation.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, Fix64 angle, out Matrix3x3 result)
        {
            Fix64 xx = axis.X.Mul(axis.X);
            Fix64 yy = axis.Y.Mul(axis.Y);
            Fix64 zz = axis.Z.Mul(axis.Z);
            Fix64 xy = axis.X.Mul(axis.Y);
            Fix64 xz = axis.X.Mul(axis.Z);
            Fix64 yz = axis.Y.Mul(axis.Z);

            Fix64 sinAngle = Fix64Ext.Sin(angle);
            Fix64 oneMinusCosAngle = F64.C1.Sub(Fix64Ext.Cos(angle));

            result.M11 = F64.C1.Add(oneMinusCosAngle.Mul((xx.Sub(F64.C1))));
            result.M21 = ((axis.Z.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xy));
            result.M31 = (axis.Y.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xz));

            result.M12 = (axis.Z.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xy));
            result.M22 = F64.C1.Add(oneMinusCosAngle.Mul((yy.Sub(F64.C1))));
            result.M32 = ((axis.X.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(yz));

            result.M13 = ((axis.Y.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xz));
            result.M23 = (axis.X.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(yz));
            result.M33 = F64.C1.Add(oneMinusCosAngle.Mul((zz.Sub(F64.C1))));
        }






    }
}