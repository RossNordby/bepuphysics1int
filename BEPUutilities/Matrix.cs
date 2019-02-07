
using System;

namespace BEPUutilities
{
    /// <summary>
    /// Provides XNA-like 4x4 matrix math.
    /// </summary>
    public struct Matrix
    {
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public Fix32 M11;

        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public Fix32 M12;

        /// <summary>
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public Fix32 M13;

        /// <summary>
        /// Value at row 1, column 4 of the matrix.
        /// </summary>
        public Fix32 M14;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public Fix32 M21;

        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public Fix32 M22;

        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public Fix32 M23;

        /// <summary>
        /// Value at row 2, column 4 of the matrix.
        /// </summary>
        public Fix32 M24;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public Fix32 M31;

        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public Fix32 M32;

        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public Fix32 M33;

        /// <summary>
        /// Value at row 3, column 4 of the matrix.
        /// </summary>
        public Fix32 M34;

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public Fix32 M41;

        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public Fix32 M42;

        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public Fix32 M43;

        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public Fix32 M44;

        /// <summary>
        /// Constructs a new 4 row, 4 column matrix.
        /// </summary>
        /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
        /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
        /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
        /// <param name="m14">Value at row 1, column 4 of the matrix.</param>
        /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
        /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
        /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
        /// <param name="m24">Value at row 2, column 4 of the matrix.</param>
        /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
        /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
        /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
        /// <param name="m34">Value at row 3, column 4 of the matrix.</param>
        /// <param name="m41">Value at row 4, column 1 of the matrix.</param>
        /// <param name="m42">Value at row 4, column 2 of the matrix.</param>
        /// <param name="m43">Value at row 4, column 3 of the matrix.</param>
        /// <param name="m44">Value at row 4, column 4 of the matrix.</param>
        public Matrix(Fix32 m11, Fix32 m12, Fix32 m13, Fix32 m14,
                      Fix32 m21, Fix32 m22, Fix32 m23, Fix32 m24,
                      Fix32 m31, Fix32 m32, Fix32 m33, Fix32 m34,
                      Fix32 m41, Fix32 m42, Fix32 m43, Fix32 m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        /// <summary>
        /// Gets or sets the translation component of the transform.
        /// </summary>
        public Vector3 Translation
        {
            get
            {
                return new Vector3()
                {
                    X = M41,
                    Y = M42,
                    Z = M43
                };
            }
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
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
        /// Computes the determinant of the matrix.
        /// </summary>
        /// <returns></returns>
        public Fix32 Determinant()
        {
            //Compute the re-used 2x2 determinants.
            Fix32 det1 = (M33.Mul(M44)).Sub(M34.Mul(M43));
            Fix32 det2 = (M32.Mul(M44)).Sub(M34.Mul(M42));
            Fix32 det3 = (M32.Mul(M43)).Sub(M33.Mul(M42));
            Fix32 det4 = (M31.Mul(M44)).Sub(M34.Mul(M41));
            Fix32 det5 = (M31.Mul(M43)).Sub(M33.Mul(M41));
            Fix32 det6 = (M31.Mul(M42)).Sub(M32.Mul(M41));
            return
(((M11.Mul((((M22.Mul(det1)).Sub(M23.Mul(det2))).Add(M24.Mul(det3))))).Sub((M12.Mul((((M21.Mul(det1)).Sub(M23.Mul(det4))).Add(M24.Mul(det5))))))).Add((M13.Mul((((M21.Mul(det2)).Sub(M22.Mul(det4))).Add(M24.Mul(det6))))))).Sub((M14.Mul((((M21.Mul(det3)).Sub(M22.Mul(det5))).Add(M23.Mul(det6))))));
        }

        /// <summary>
        /// Transposes the matrix in-place.
        /// </summary>
        public void Transpose()
        {
            Fix32 intermediate = M12;
            M12 = M21;
            M21 = intermediate;

            intermediate = M13;
            M13 = M31;
            M31 = intermediate;

            intermediate = M14;
            M14 = M41;
            M41 = intermediate;

            intermediate = M23;
            M23 = M32;
            M32 = intermediate;

            intermediate = M24;
            M24 = M42;
            M42 = intermediate;

            intermediate = M34;
            M34 = M43;
            M43 = intermediate;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Matrix created from the axis and angle.</returns>
        public static Matrix CreateFromAxisAngle(Vector3 axis, Fix32 angle)
        {
            Matrix toReturn;
            CreateFromAxisAngle(ref axis, angle, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a matrix representing the given axis and angle rotation.
        /// </summary>
        /// <param name="axis">Axis around which to rotate.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="result">Matrix created from the axis and angle.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, Fix32 angle, out Matrix result)
        {
            Fix32 xx = axis.X.Mul(axis.X);
            Fix32 yy = axis.Y.Mul(axis.Y);
            Fix32 zz = axis.Z.Mul(axis.Z);
            Fix32 xy = axis.X.Mul(axis.Y);
            Fix32 xz = axis.X.Mul(axis.Z);
            Fix32 yz = axis.Y.Mul(axis.Z);

            Fix32 sinAngle = Fix32Ext.Sin(angle);
            Fix32 oneMinusCosAngle = F64.C1.Sub(Fix32Ext.Cos(angle));

            result.M11 = F64.C1.Add(oneMinusCosAngle.Mul((xx.Sub(F64.C1))));
            result.M21 = ((axis.Z.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xy));
            result.M31 = (axis.Y.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xz));
            result.M41 = F64.C0;

            result.M12 = (axis.Z.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xy));
            result.M22 = F64.C1.Add(oneMinusCosAngle.Mul((yy.Sub(F64.C1))));
            result.M32 = ((axis.X.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(yz));
            result.M42 = F64.C0;

            result.M13 = ((axis.Y.Neg()).Mul(sinAngle)).Add(oneMinusCosAngle.Mul(xz));
            result.M23 = (axis.X.Mul(sinAngle)).Add(oneMinusCosAngle.Mul(yz));
            result.M33 = F64.C1.Add(oneMinusCosAngle.Mul((zz.Sub(F64.C1))));
            result.M43 = F64.C0;

            result.M14 = F64.C0;
            result.M24 = F64.C0;
            result.M34 = F64.C0;
            result.M44 = F64.C1;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to convert.</param>
        /// <param name="result">Rotation matrix created from the quaternion.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
        {
            Fix32 qX2 = quaternion.X.Add(quaternion.X);
            Fix32 qY2 = quaternion.Y.Add(quaternion.Y);
            Fix32 qZ2 = quaternion.Z.Add(quaternion.Z);
            Fix32 XX = qX2.Mul(quaternion.X);
            Fix32 YY = qY2.Mul(quaternion.Y);
            Fix32 ZZ = qZ2.Mul(quaternion.Z);
            Fix32 XY = qX2.Mul(quaternion.Y);
            Fix32 XZ = qX2.Mul(quaternion.Z);
            Fix32 XW = qX2.Mul(quaternion.W);
            Fix32 YZ = qY2.Mul(quaternion.Z);
            Fix32 YW = qY2.Mul(quaternion.W);
            Fix32 ZW = qZ2.Mul(quaternion.W);

            result.M11 = (F64.C1.Sub(YY)).Sub(ZZ);
            result.M21 = XY.Sub(ZW);
            result.M31 = XZ.Add(YW);
            result.M41 = F64.C0;

            result.M12 = XY.Add(ZW);
            result.M22 = (F64.C1.Sub(XX)).Sub(ZZ);
            result.M32 = YZ.Sub(XW);
            result.M42 = F64.C0;

            result.M13 = XZ.Sub(YW);
            result.M23 = YZ.Add(XW);
            result.M33 = (F64.C1.Sub(XX)).Sub(YY);
            result.M43 = F64.C0;

            result.M14 = F64.C0;
            result.M24 = F64.C0;
            result.M34 = F64.C0;
            result.M44 = F64.C1;
        }

        /// <summary>
        /// Creates a rotation matrix from a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to convert.</param>
        /// <returns>Rotation matrix created from the quaternion.</returns>
        public static Matrix CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix toReturn;
            CreateFromQuaternion(ref quaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <param name="result">Combined transformation.</param>
        public static void Multiply(ref Matrix a, ref Matrix b, out Matrix result)
        {
            Fix32 resultM11 = (((a.M11.Mul(b.M11)).Add(a.M12.Mul(b.M21))).Add(a.M13.Mul(b.M31))).Add(a.M14.Mul(b.M41));
            Fix32 resultM12 = (((a.M11.Mul(b.M12)).Add(a.M12.Mul(b.M22))).Add(a.M13.Mul(b.M32))).Add(a.M14.Mul(b.M42));
            Fix32 resultM13 = (((a.M11.Mul(b.M13)).Add(a.M12.Mul(b.M23))).Add(a.M13.Mul(b.M33))).Add(a.M14.Mul(b.M43));
            Fix32 resultM14 = (((a.M11.Mul(b.M14)).Add(a.M12.Mul(b.M24))).Add(a.M13.Mul(b.M34))).Add(a.M14.Mul(b.M44));

            Fix32 resultM21 = (((a.M21.Mul(b.M11)).Add(a.M22.Mul(b.M21))).Add(a.M23.Mul(b.M31))).Add(a.M24.Mul(b.M41));
            Fix32 resultM22 = (((a.M21.Mul(b.M12)).Add(a.M22.Mul(b.M22))).Add(a.M23.Mul(b.M32))).Add(a.M24.Mul(b.M42));
            Fix32 resultM23 = (((a.M21.Mul(b.M13)).Add(a.M22.Mul(b.M23))).Add(a.M23.Mul(b.M33))).Add(a.M24.Mul(b.M43));
            Fix32 resultM24 = (((a.M21.Mul(b.M14)).Add(a.M22.Mul(b.M24))).Add(a.M23.Mul(b.M34))).Add(a.M24.Mul(b.M44));

            Fix32 resultM31 = (((a.M31.Mul(b.M11)).Add(a.M32.Mul(b.M21))).Add(a.M33.Mul(b.M31))).Add(a.M34.Mul(b.M41));
            Fix32 resultM32 = (((a.M31.Mul(b.M12)).Add(a.M32.Mul(b.M22))).Add(a.M33.Mul(b.M32))).Add(a.M34.Mul(b.M42));
            Fix32 resultM33 = (((a.M31.Mul(b.M13)).Add(a.M32.Mul(b.M23))).Add(a.M33.Mul(b.M33))).Add(a.M34.Mul(b.M43));
            Fix32 resultM34 = (((a.M31.Mul(b.M14)).Add(a.M32.Mul(b.M24))).Add(a.M33.Mul(b.M34))).Add(a.M34.Mul(b.M44));

            Fix32 resultM41 = (((a.M41.Mul(b.M11)).Add(a.M42.Mul(b.M21))).Add(a.M43.Mul(b.M31))).Add(a.M44.Mul(b.M41));
            Fix32 resultM42 = (((a.M41.Mul(b.M12)).Add(a.M42.Mul(b.M22))).Add(a.M43.Mul(b.M32))).Add(a.M44.Mul(b.M42));
            Fix32 resultM43 = (((a.M41.Mul(b.M13)).Add(a.M42.Mul(b.M23))).Add(a.M43.Mul(b.M33))).Add(a.M44.Mul(b.M43));
            Fix32 resultM44 = (((a.M41.Mul(b.M14)).Add(a.M42.Mul(b.M24))).Add(a.M43.Mul(b.M34))).Add(a.M44.Mul(b.M44));

            result.M11 = resultM11;
            result.M12 = resultM12;
            result.M13 = resultM13;
            result.M14 = resultM14;

            result.M21 = resultM21;
            result.M22 = resultM22;
            result.M23 = resultM23;
            result.M24 = resultM24;

            result.M31 = resultM31;
            result.M32 = resultM32;
            result.M33 = resultM33;
            result.M34 = resultM34;

            result.M41 = resultM41;
            result.M42 = resultM42;
            result.M43 = resultM43;
            result.M44 = resultM44;
        }


        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static Matrix Multiply(Matrix a, Matrix b)
        {
            Matrix result;
            Multiply(ref a, ref b, out result);
            return result;
        }


        /// <summary>
        /// Scales all components of the matrix.
        /// </summary>
        /// <param name="matrix">Matrix to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled matrix.</param>
        public static void Multiply(ref Matrix matrix, Fix32 scale, out Matrix result)
        {
            result.M11 = matrix.M11.Mul(scale);
            result.M12 = matrix.M12.Mul(scale);
            result.M13 = matrix.M13.Mul(scale);
            result.M14 = matrix.M14.Mul(scale);

            result.M21 = matrix.M21.Mul(scale);
            result.M22 = matrix.M22.Mul(scale);
            result.M23 = matrix.M23.Mul(scale);
            result.M24 = matrix.M24.Mul(scale);

            result.M31 = matrix.M31.Mul(scale);
            result.M32 = matrix.M32.Mul(scale);
            result.M33 = matrix.M33.Mul(scale);
            result.M34 = matrix.M34.Mul(scale);

            result.M41 = matrix.M41.Mul(scale);
            result.M42 = matrix.M42.Mul(scale);
            result.M43 = matrix.M43.Mul(scale);
            result.M44 = matrix.M44.Mul(scale);
        }

        /// <summary>
        /// Multiplies two matrices together.
        /// </summary>
        /// <param name="a">First matrix to multiply.</param>
        /// <param name="b">Second matrix to multiply.</param>
        /// <returns>Combined transformation.</returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix toReturn;
            Multiply(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix operator *(Matrix m, Fix32 f)
        {
            Matrix result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Scales all components of the matrix by the given value.
        /// </summary>
        /// <param name="m">First matrix to multiply.</param>
        /// <param name="f">Scaling value to apply to all components of the matrix.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Matrix operator *(Fix32 f, Matrix m)
        {
            Matrix result;
            Multiply(ref m, f, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref Vector4 v, ref Matrix matrix, out Vector4 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            Fix32 vW = v.W;
            result.X = (((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M21))).Add(vZ.Mul(matrix.M31))).Add(vW.Mul(matrix.M41));
            result.Y = (((vX.Mul(matrix.M12)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M32))).Add(vW.Mul(matrix.M42));
            result.Z = (((vX.Mul(matrix.M13)).Add(vY.Mul(matrix.M23))).Add(vZ.Mul(matrix.M33))).Add(vW.Mul(matrix.M43));
            result.W = (((vX.Mul(matrix.M14)).Add(vY.Mul(matrix.M24))).Add(vZ.Mul(matrix.M34))).Add(vW.Mul(matrix.M44));
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector4 Transform(Vector4 v, Matrix matrix)
        {
            Vector4 toReturn;
            Transform(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref Vector4 v, ref Matrix matrix, out Vector4 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            Fix32 vW = v.W;
            result.X = (((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M12))).Add(vZ.Mul(matrix.M13))).Add(vW.Mul(matrix.M14));
            result.Y = (((vX.Mul(matrix.M21)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M23))).Add(vW.Mul(matrix.M24));
            result.Z = (((vX.Mul(matrix.M31)).Add(vY.Mul(matrix.M32))).Add(vZ.Mul(matrix.M33))).Add(vW.Mul(matrix.M34));
            result.W = (((vX.Mul(matrix.M41)).Add(vY.Mul(matrix.M42))).Add(vZ.Mul(matrix.M43))).Add(vW.Mul(matrix.M44));
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector4 TransformTranspose(Vector4 v, Matrix matrix)
        {
            Vector4 toReturn;
            TransformTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref Vector3 v, ref Matrix matrix, out Vector4 result)
        {
            result.X = (((v.X.Mul(matrix.M11)).Add(v.Y.Mul(matrix.M21))).Add(v.Z.Mul(matrix.M31))).Add(matrix.M41);
            result.Y = (((v.X.Mul(matrix.M12)).Add(v.Y.Mul(matrix.M22))).Add(v.Z.Mul(matrix.M32))).Add(matrix.M42);
            result.Z = (((v.X.Mul(matrix.M13)).Add(v.Y.Mul(matrix.M23))).Add(v.Z.Mul(matrix.M33))).Add(matrix.M43);
            result.W = (((v.X.Mul(matrix.M14)).Add(v.Y.Mul(matrix.M24))).Add(v.Z.Mul(matrix.M34))).Add(matrix.M44);
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector4 Transform(Vector3 v, Matrix matrix)
        {
            Vector4 toReturn;
            Transform(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref Vector3 v, ref Matrix matrix, out Vector4 result)
        {
            result.X = (((v.X.Mul(matrix.M11)).Add(v.Y.Mul(matrix.M12))).Add(v.Z.Mul(matrix.M13))).Add(matrix.M14);
            result.Y = (((v.X.Mul(matrix.M21)).Add(v.Y.Mul(matrix.M22))).Add(v.Z.Mul(matrix.M23))).Add(matrix.M24);
            result.Z = (((v.X.Mul(matrix.M31)).Add(v.Y.Mul(matrix.M32))).Add(v.Z.Mul(matrix.M33))).Add(matrix.M34);
            result.W = (((v.X.Mul(matrix.M41)).Add(v.Y.Mul(matrix.M42))).Add(v.Z.Mul(matrix.M43))).Add(matrix.M44);
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector4 TransformTranspose(Vector3 v, Matrix matrix)
        {
            Vector4 toReturn;
            TransformTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref Vector3 v, ref Matrix matrix, out Vector3 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            result.X = (((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M21))).Add(vZ.Mul(matrix.M31))).Add(matrix.M41);
            result.Y = (((vX.Mul(matrix.M12)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M32))).Add(matrix.M42);
            result.Z = (((vX.Mul(matrix.M13)).Add(vY.Mul(matrix.M23))).Add(vZ.Mul(matrix.M33))).Add(matrix.M43);
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformTranspose(ref Vector3 v, ref Matrix matrix, out Vector3 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            result.X = (((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M12))).Add(vZ.Mul(matrix.M13))).Add(matrix.M14);
            result.Y = (((vX.Mul(matrix.M21)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M23))).Add(matrix.M24);
            result.Z = (((vX.Mul(matrix.M31)).Add(vY.Mul(matrix.M32))).Add(vZ.Mul(matrix.M33))).Add(matrix.M34);
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormal(ref Vector3 v, ref Matrix matrix, out Vector3 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M21))).Add(vZ.Mul(matrix.M31));
            result.Y = ((vX.Mul(matrix.M12)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M32));
            result.Z = ((vX.Mul(matrix.M13)).Add(vY.Mul(matrix.M23))).Add(vZ.Mul(matrix.M33));
        }

        /// <summary>
        /// Transforms a vector using a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector3 TransformNormal(Vector3 v, Matrix matrix)
        {
            Vector3 toReturn;
            TransformNormal(ref v, ref matrix, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformNormalTranspose(ref Vector3 v, ref Matrix matrix, out Vector3 result)
        {
            Fix32 vX = v.X;
            Fix32 vY = v.Y;
            Fix32 vZ = v.Z;
            result.X = ((vX.Mul(matrix.M11)).Add(vY.Mul(matrix.M12))).Add(vZ.Mul(matrix.M13));
            result.Y = ((vX.Mul(matrix.M21)).Add(vY.Mul(matrix.M22))).Add(vZ.Mul(matrix.M23));
            result.Z = ((vX.Mul(matrix.M31)).Add(vY.Mul(matrix.M32))).Add(vZ.Mul(matrix.M33));
        }

        /// <summary>
        /// Transforms a vector using the transpose of a matrix.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector3 TransformNormalTranspose(Vector3 v, Matrix matrix)
        {
            Vector3 toReturn;
            TransformNormalTranspose(ref v, ref matrix, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Transposes the matrix.
        /// </summary>
        /// <param name="m">Matrix to transpose.</param>
        /// <param name="transposed">Matrix to transpose.</param>
        public static void Transpose(ref Matrix m, out Matrix transposed)
        {
            Fix32 intermediate = m.M12;
            transposed.M12 = m.M21;
            transposed.M21 = intermediate;

            intermediate = m.M13;
            transposed.M13 = m.M31;
            transposed.M31 = intermediate;

            intermediate = m.M14;
            transposed.M14 = m.M41;
            transposed.M41 = intermediate;

            intermediate = m.M23;
            transposed.M23 = m.M32;
            transposed.M32 = intermediate;

            intermediate = m.M24;
            transposed.M24 = m.M42;
            transposed.M42 = intermediate;

            intermediate = m.M34;
            transposed.M34 = m.M43;
            transposed.M43 = intermediate;

            transposed.M11 = m.M11;
            transposed.M22 = m.M22;
            transposed.M33 = m.M33;
            transposed.M44 = m.M44;
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void Invert(ref Matrix m, out Matrix inverted)
        {
			Matrix4x8.Invert(ref m, out inverted);
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static Matrix Invert(Matrix m)
        {
            Matrix inverted;
            Invert(ref m, out inverted);
            return inverted;
        }
		
        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <param name="inverted">Inverted version of the matrix.</param>
        public static void InvertRigid(ref Matrix m, out Matrix inverted)
        {
            //Invert (transpose) the upper left 3x3 rotation.
            Fix32 intermediate = m.M12;
            inverted.M12 = m.M21;
            inverted.M21 = intermediate;

            intermediate = m.M13;
            inverted.M13 = m.M31;
            inverted.M31 = intermediate;

            intermediate = m.M23;
            inverted.M23 = m.M32;
            inverted.M32 = intermediate;

            inverted.M11 = m.M11;
            inverted.M22 = m.M22;
            inverted.M33 = m.M33;

            //Translation component
            var vX = m.M41;
            var vY = m.M42;
            var vZ = m.M43;
            inverted.M41 = ((vX.Mul(inverted.M11)).Add(vY.Mul(inverted.M21))).Add(vZ.Mul(inverted.M31)).Neg();
            inverted.M42 = ((vX.Mul(inverted.M12)).Add(vY.Mul(inverted.M22))).Add(vZ.Mul(inverted.M32)).Neg();
            inverted.M43 = ((vX.Mul(inverted.M13)).Add(vY.Mul(inverted.M23))).Add(vZ.Mul(inverted.M33)).Neg();

            //Last chunk.
            inverted.M14 = F64.C0;
            inverted.M24 = F64.C0;
            inverted.M34 = F64.C0;
            inverted.M44 = F64.C1;
        }

        /// <summary>
        /// Inverts the matrix using a process that only works for rigid transforms.
        /// </summary>
        /// <param name="m">Matrix to invert.</param>
        /// <returns>Inverted version of the matrix.</returns>
        public static Matrix InvertRigid(Matrix m)
        {
            Matrix inverse;
            InvertRigid(ref m, out inverse);
            return inverse;
        }

        /// <summary>
        /// Gets the 4x4 identity matrix.
        /// </summary>
        public static Matrix Identity
        {
            get
            {
                Matrix toReturn;
                toReturn.M11 = F64.C1;
                toReturn.M12 = F64.C0;
                toReturn.M13 = F64.C0;
                toReturn.M14 = F64.C0;

                toReturn.M21 = F64.C0;
                toReturn.M22 = F64.C1;
                toReturn.M23 = F64.C0;
                toReturn.M24 = F64.C0;

                toReturn.M31 = F64.C0;
                toReturn.M32 = F64.C0;
                toReturn.M33 = F64.C1;
                toReturn.M34 = F64.C0;

                toReturn.M41 = F64.C0;
                toReturn.M42 = F64.C0;
                toReturn.M43 = F64.C0;
                toReturn.M44 = F64.C1;
                return toReturn;
            }
        }

        /// <summary>
        /// Creates a right handed orthographic projection.
        /// </summary>
        /// <param name="left">Leftmost coordinate of the projected area.</param>
        /// <param name="right">Rightmost coordinate of the projected area.</param>
        /// <param name="bottom">Bottom coordinate of the projected area.</param>
        /// <param name="top">Top coordinate of the projected area.</param>
        /// <param name="zNear">Near plane of the projection.</param>
        /// <param name="zFar">Far plane of the projection.</param>
        /// <param name="projection">The resulting orthographic projection matrix.</param>
        public static void CreateOrthographicRH(Fix32 left, Fix32 right, Fix32 bottom, Fix32 top, Fix32 zNear, Fix32 zFar, out Matrix projection)
        {
            Fix32 width = right.Sub(left);
            Fix32 height = top.Sub(bottom);
            Fix32 depth = zFar.Sub(zNear);
            projection.M11 = F64.C2.Div(width);
            projection.M12 = F64.C0;
            projection.M13 = F64.C0;
            projection.M14 = F64.C0;

            projection.M21 = F64.C0;
            projection.M22 = F64.C2.Div(height);
            projection.M23 = F64.C0;
            projection.M24 = F64.C0;

            projection.M31 = F64.C0;
            projection.M32 = F64.C0;
            projection.M33 = (F64.C1.Neg()).Div(depth);
            projection.M34 = F64.C0;

            projection.M41 = (left.Add(right)).Div(width.Neg());
            projection.M42 = (top.Add(bottom)).Div(height.Neg());
            projection.M43 = zNear.Div(depth.Neg());
            projection.M44 = F64.C1;

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <param name="perspective">Resulting perspective matrix.</param>
        public static void CreatePerspectiveFieldOfViewRH(Fix32 fieldOfView, Fix32 aspectRatio, Fix32 nearClip, Fix32 farClip, out Matrix perspective)
        {
            Fix32 h = F64.C1.Div(Fix32Ext.Tan(fieldOfView.Div(F64.C2)));
            Fix32 w = h.Div(aspectRatio);
            perspective.M11 = w;
            perspective.M12 = F64.C0;
            perspective.M13 = F64.C0;
            perspective.M14 = F64.C0;

            perspective.M21 = F64.C0;
            perspective.M22 = h;
            perspective.M23 = F64.C0;
            perspective.M24 = F64.C0;

            perspective.M31 = F64.C0;
            perspective.M32 = F64.C0;
            perspective.M33 = farClip.Div((nearClip.Sub(farClip)));
            perspective.M34 = F64.C1.Neg();

            perspective.M41 = F64.C0;
            perspective.M42 = F64.C0;
            perspective.M44 = F64.C0;
            perspective.M43 = nearClip.Mul(perspective.M33);

        }

        /// <summary>
        /// Creates a right-handed perspective matrix.
        /// </summary>
        /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
        /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
        /// <param name="nearClip">Near clip plane of the perspective.</param>
        /// <param name="farClip">Far clip plane of the perspective.</param>
        /// <returns>Resulting perspective matrix.</returns>
        public static Matrix CreatePerspectiveFieldOfViewRH(Fix32 fieldOfView, Fix32 aspectRatio, Fix32 nearClip, Fix32 farClip)
        {
            Matrix perspective;
            CreatePerspectiveFieldOfViewRH(fieldOfView, aspectRatio, nearClip, farClip, out perspective);
            return perspective;
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewMatrix">Look at matrix.</param>
        public static void CreateLookAtRH(ref Vector3 position, ref Vector3 target, ref Vector3 upVector, out Matrix viewMatrix)
        {
            Vector3 forward;
            Vector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out viewMatrix);
        }

        /// <summary>
        /// Creates a view matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">Target of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static Matrix CreateLookAtRH(Vector3 position, Vector3 target, Vector3 upVector)
        {
            Matrix lookAt;
            Vector3 forward;
            Vector3.Subtract(ref target, ref position, out forward);
            CreateViewRH(ref position, ref forward, ref upVector, out lookAt);
            return lookAt;
        }


        /// <summary>
        /// Creates a view matrix pointing in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <param name="viewMatrix">Look at matrix.</param>
        public static void CreateViewRH(ref Vector3 position, ref Vector3 forward, ref Vector3 upVector, out Matrix viewMatrix)
        {
            Vector3 z;
            Fix32 length = forward.Length();
            Vector3.Divide(ref forward, length.Neg(), out z);
            Vector3 x;
            Vector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            Vector3 y;
            Vector3.Cross(ref z, ref x, out y);

            viewMatrix.M11 = x.X;
            viewMatrix.M12 = y.X;
            viewMatrix.M13 = z.X;
            viewMatrix.M14 = F64.C0;
            viewMatrix.M21 = x.Y;
            viewMatrix.M22 = y.Y;
            viewMatrix.M23 = z.Y;
            viewMatrix.M24 = F64.C0;
            viewMatrix.M31 = x.Z;
            viewMatrix.M32 = y.Z;
            viewMatrix.M33 = z.Z;
            viewMatrix.M34 = F64.C0;
            Vector3.Dot(ref x, ref position, out viewMatrix.M41);
            Vector3.Dot(ref y, ref position, out viewMatrix.M42);
            Vector3.Dot(ref z, ref position, out viewMatrix.M43);
            viewMatrix.M41 = viewMatrix.M41.Neg();
            viewMatrix.M42 = viewMatrix.M42.Neg();
            viewMatrix.M43 = viewMatrix.M43.Neg();
            viewMatrix.M44 = F64.C1;

        }

        /// <summary>
        /// Creates a view matrix pointing looking in a direction with a given up vector.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="forward">Forward direction of the camera.</param>
        /// <param name="upVector">Up vector of the camera.</param>
        /// <returns>Look at matrix.</returns>
        public static Matrix CreateViewRH(Vector3 position, Vector3 forward, Vector3 upVector)
        {
            Matrix lookat;
            CreateViewRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <param name="worldMatrix">World matrix.</param>
        public static void CreateWorldRH(ref Vector3 position, ref Vector3 forward, ref Vector3 upVector, out Matrix worldMatrix)
        {
            Vector3 z;
            Fix32 length = forward.Length();
            Vector3.Divide(ref forward, length.Neg(), out z);
            Vector3 x;
            Vector3.Cross(ref upVector, ref z, out x);
            x.Normalize();
            Vector3 y;
            Vector3.Cross(ref z, ref x, out y);

            worldMatrix.M11 = x.X;
            worldMatrix.M12 = x.Y;
            worldMatrix.M13 = x.Z;
            worldMatrix.M14 = F64.C0;
            worldMatrix.M21 = y.X;
            worldMatrix.M22 = y.Y;
            worldMatrix.M23 = y.Z;
            worldMatrix.M24 = F64.C0;
            worldMatrix.M31 = z.X;
            worldMatrix.M32 = z.Y;
            worldMatrix.M33 = z.Z;
            worldMatrix.M34 = F64.C0;

            worldMatrix.M41 = position.X;
            worldMatrix.M42 = position.Y;
            worldMatrix.M43 = position.Z;
            worldMatrix.M44 = F64.C1;

        }


        /// <summary>
        /// Creates a world matrix pointing from a position to a target with the given up vector.
        /// </summary>
        /// <param name="position">Position of the transform.</param>
        /// <param name="forward">Forward direction of the transformation.</param>
        /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
        /// <returns>World matrix.</returns>
        public static Matrix CreateWorldRH(Vector3 position, Vector3 forward, Vector3 upVector)
        {
            Matrix lookat;
            CreateWorldRH(ref position, ref forward, ref upVector, out lookat);
            return lookat;
        }



        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <param name="translationMatrix">Matrix representing the given translation.</param>
        public static void CreateTranslation(ref Vector3 translation, out Matrix translationMatrix)
        {
            translationMatrix = new Matrix
            {
                M11 = F64.C1,
                M22 = F64.C1,
                M33 = F64.C1,
                M44 = F64.C1,
                M41 = translation.X,
                M42 = translation.Y,
                M43 = translation.Z
            };
        }

        /// <summary>
        /// Creates a matrix representing a translation.
        /// </summary>
        /// <param name="translation">Translation to be represented by the matrix.</param>
        /// <returns>Matrix representing the given translation.</returns>
        public static Matrix CreateTranslation(Vector3 translation)
        {
            Matrix translationMatrix;
            CreateTranslation(ref translation, out translationMatrix);
            return translationMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <param name="scaleMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(ref Vector3 scale, out Matrix scaleMatrix)
        {
            scaleMatrix = new Matrix
                {
                    M11 = scale.X,
                    M22 = scale.Y,
                    M33 = scale.Z,
                    M44 = F64.C1
			};
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="scale">Scale to be represented by the matrix.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static Matrix CreateScale(Vector3 scale)
        {
            Matrix scaleMatrix;
            CreateScale(ref scale, out scaleMatrix);
            return scaleMatrix;
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <param name="scaleMatrix">Matrix representing the given scale.</param>
        public static void CreateScale(Fix32 x, Fix32 y, Fix32 z, out Matrix scaleMatrix)
        {
            scaleMatrix = new Matrix
            {
                M11 = x,
                M22 = y,
                M33 = z,
                M44 = F64.C1
			};
        }

        /// <summary>
        /// Creates a matrix representing the given axis aligned scale.
        /// </summary>
        /// <param name="x">Scale along the x axis.</param>
        /// <param name="y">Scale along the y axis.</param>
        /// <param name="z">Scale along the z axis.</param>
        /// <returns>Matrix representing the given scale.</returns>
        public static Matrix CreateScale(Fix32 x, Fix32 y, Fix32 z)
        {
            Matrix scaleMatrix;
            CreateScale(x, y, z, out scaleMatrix);
            return scaleMatrix;
        }

        /// <summary>
        /// Creates a string representation of the matrix.
        /// </summary>
        /// <returns>A string representation of the matrix.</returns>
        public override string ToString()
        {
            return "{" + M11.ToStringExt() + ", " + M12.ToStringExt() + ", " + M13.ToStringExt() + ", " + M14.ToStringExt() + "} " +
                   "{" + M21.ToStringExt() + ", " + M22.ToStringExt() + ", " + M23.ToStringExt() + ", " + M24.ToStringExt() + "} " +
                   "{" + M31.ToStringExt() + ", " + M32.ToStringExt() + ", " + M33.ToStringExt() + ", " + M34.ToStringExt() + "} " +
                   "{" + M41.ToStringExt() + ", " + M42.ToStringExt() + ", " + M43.ToStringExt() + ", " + M44.ToStringExt() + "}";
        }
    }
}
