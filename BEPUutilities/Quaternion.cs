
using System;

namespace BEPUutilities
{
    /// <summary>
    /// Provides XNA-like quaternion support.
    /// </summary>
    [Serializable]
    public struct Quaternion : IEquatable<Quaternion>
    {
        /// <summary>
        /// X component of the quaternion.
        /// </summary>
        public Fix32 X;

        /// <summary>
        /// Y component of the quaternion.
        /// </summary>
        public Fix32 Y;

        /// <summary>
        /// Z component of the quaternion.
        /// </summary>
        public Fix32 Z;

        /// <summary>
        /// W component of the quaternion.
        /// </summary>
        public Fix32 W;

        /// <summary>
        /// Constructs a new Quaternion.
        /// </summary>
        /// <param name="x">X component of the quaternion.</param>
        /// <param name="y">Y component of the quaternion.</param>
        /// <param name="z">Z component of the quaternion.</param>
        /// <param name="w">W component of the quaternion.</param>
        public Quaternion(Fix32 x, Fix32 y, Fix32 z, Fix32 w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Adds two quaternions together.
        /// </summary>
        /// <param name="a">First quaternion to add.</param>
        /// <param name="b">Second quaternion to add.</param>
        /// <param name="result">Sum of the addition.</param>
        public static void Add(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            result.X = a.X.Add(b.X);
            result.Y = a.Y.Add(b.Y);
            result.Z = a.Z.Add(b.Z);
            result.W = a.W.Add(b.W);
        }

        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Multiply(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            Fix32 x = a.X;
            Fix32 y = a.Y;
            Fix32 z = a.Z;
            Fix32 w = a.W;
            Fix32 bX = b.X;
            Fix32 bY = b.Y;
            Fix32 bZ = b.Z;
            Fix32 bW = b.W;
            result.X = (((x.Mul(bW)).Add(bX.Mul(w))).Add(y.Mul(bZ))).Sub(z.Mul(bY));
            result.Y = (((y.Mul(bW)).Add(bY.Mul(w))).Add(z.Mul(bX))).Sub(x.Mul(bZ));
            result.Z = (((z.Mul(bW)).Add(bZ.Mul(w))).Add(x.Mul(bY))).Sub(y.Mul(bX));
            result.W = (((w.Mul(bW)).Sub(x.Mul(bX))).Sub(y.Mul(bY))).Sub(z.Mul(bZ));
        }

        /// <summary>
        /// Scales a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to multiply.</param>
        /// <param name="scale">Amount to multiply each component of the quaternion by.</param>
        /// <param name="result">Scaled quaternion.</param>
        public static void Multiply(ref Quaternion q, Fix32 scale, out Quaternion result)
        {
            result.X = q.X.Mul(scale);
            result.Y = q.Y.Mul(scale);
            result.Z = q.Z.Mul(scale);
            result.W = q.W.Mul(scale);
        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <param name="result">Product of the multiplication.</param>
        public static void Concatenate(ref Quaternion a, ref Quaternion b, out Quaternion result)
        {
            Fix32 aX = a.X;
            Fix32 aY = a.Y;
            Fix32 aZ = a.Z;
            Fix32 aW = a.W;
            Fix32 bX = b.X;
            Fix32 bY = b.Y;
            Fix32 bZ = b.Z;
            Fix32 bW = b.W;

            result.X = (((aW.Mul(bX)).Add(aX.Mul(bW))).Add(aZ.Mul(bY))).Sub(aY.Mul(bZ));
            result.Y = (((aW.Mul(bY)).Add(aY.Mul(bW))).Add(aX.Mul(bZ))).Sub(aZ.Mul(bX));
            result.Z = (((aW.Mul(bZ)).Add(aZ.Mul(bW))).Add(aY.Mul(bX))).Sub(aX.Mul(bY));
            result.W = (((aW.Mul(bW)).Sub(aX.Mul(bX))).Sub(aY.Mul(bY))).Sub(aZ.Mul(bZ));


        }

        /// <summary>
        /// Multiplies two quaternions together in opposite order.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Quaternion Concatenate(Quaternion a, Quaternion b)
        {
            Quaternion result;
            Concatenate(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Quaternion representing the identity transform.
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return new Quaternion(F64.C0, F64.C0, F64.C0, F64.C1);
            }
        }




        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref Matrix3x3 r, out Quaternion q)
        {
            Fix32 trace = (r.M11.Add(r.M22)).Add(r.M33);
#if !WINDOWS
            q = new Quaternion();
#endif
            if (trace >= F64.C0)
            {
                var S = Fix32Ext.Sqrt(trace.Add(F64.C1)).Mul(F64.C2); // S=4*qw 
                q.W = F64.C0p25.Mul(S);
                q.X = (r.M23.Sub(r.M32)).Div(S);
                q.Y = (r.M31.Sub(r.M13)).Div(S);
                q.Z = (r.M12.Sub(r.M21)).Div(S);
            }
            else if ((r.M11 > r.M22) & (r.M11 > r.M33))
            {
                var S = Fix32Ext.Sqrt(((F64.C1.Add(r.M11)).Sub(r.M22)).Sub(r.M33)).Mul(F64.C2); // S=4*qx 
                q.W = (r.M23.Sub(r.M32)).Div(S);
                q.X = F64.C0p25.Mul(S);
                q.Y = (r.M21.Add(r.M12)).Div(S);
                q.Z = (r.M31.Add(r.M13)).Div(S);
            }
            else if (r.M22 > r.M33)
            {
                var S = Fix32Ext.Sqrt(((F64.C1.Add(r.M22)).Sub(r.M11)).Sub(r.M33)).Mul(F64.C2); // S=4*qy
                q.W = (r.M31.Sub(r.M13)).Div(S);
                q.X = (r.M21.Add(r.M12)).Div(S);
                q.Y = F64.C0p25.Mul(S);
                q.Z = (r.M32.Add(r.M23)).Div(S);
            }
            else
            {
                var S = Fix32Ext.Sqrt(((F64.C1.Add(r.M33)).Sub(r.M11)).Sub(r.M22)).Mul(F64.C2); // S=4*qz
                q.W = (r.M12.Sub(r.M21)).Div(S);
                q.X = (r.M31.Add(r.M13)).Div(S);
                q.Y = (r.M32.Add(r.M23)).Div(S);
                q.Z = F64.C0p25.Mul(S);
            }
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix3x3 r)
        {
            Quaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix to create the quaternion from.</param>
        /// <param name="q">Quaternion based on the rotation matrix.</param>
        public static void CreateFromRotationMatrix(ref Matrix r, out Quaternion q)
        {
            Matrix3x3 downsizedMatrix;
            Matrix3x3.CreateFromMatrix(ref r, out downsizedMatrix);
            CreateFromRotationMatrix(ref downsizedMatrix, out q);
        }

        /// <summary>
        /// Creates a quaternion from a rotation matrix.
        /// </summary>
        /// <param name="r">Rotation matrix used to create a new quaternion.</param>
        /// <returns>Quaternion representing the same rotation as the matrix.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix r)
        {
            Quaternion toReturn;
            CreateFromRotationMatrix(ref r, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <returns>Normalized quaternion.</returns>
        public static Quaternion Normalize(Quaternion quaternion)
        {
            Quaternion toReturn;
            Normalize(ref quaternion, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Ensures the quaternion has unit length.
        /// </summary>
        /// <param name="quaternion">Quaternion to normalize.</param>
        /// <param name="toReturn">Normalized quaternion.</param>
        public static void Normalize(ref Quaternion quaternion, out Quaternion toReturn)
        {
            Fix32 l = Fix32Ext.Sqrt((((quaternion.X.Mul(quaternion.X)).Add(quaternion.Y.Mul(quaternion.Y))).Add(quaternion.Z.Mul(quaternion.Z))).Add(quaternion.W.Mul(quaternion.W)));
            toReturn.X = quaternion.X.Div(l);
            toReturn.Y = quaternion.Y.Div(l);
            toReturn.Z = quaternion.Z.Div(l);
            toReturn.W = quaternion.W.Div(l);
        }

        /// <summary>
        /// Scales the quaternion such that it has unit length.
        /// </summary>
        public void Normalize()
        {
            Fix32 l = Fix32Ext.Sqrt((((X.Mul(X)).Add(Y.Mul(Y))).Add(Z.Mul(Z))).Add(W.Mul(W)));
			X = X.Div(l);
			Y = Y.Div(l);
			Z = Z.Div(l);
			W = W.Div(l);
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        /// <returns>Squared length of the quaternion.</returns>
        public Fix32 LengthSquared()
        {
            return (((X.Mul(X)).Add(Y.Mul(Y))).Add(Z.Mul(Z))).Add(W.Mul(W));
        }

        /// <summary>
        /// Computes the length of the quaternion.
        /// </summary>
        /// <returns>Length of the quaternion.</returns>
        public Fix32 Length()
        {
            return Fix32Ext.Sqrt((((X.Mul(X)).Add(Y.Mul(Y))).Add(Z.Mul(Z))).Add(W.Mul(W)));
        }


        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <param name="result">Interpolated intermediate quaternion.</param>
        public static void Slerp(ref Quaternion start, ref Quaternion end, Fix32 interpolationAmount, out Quaternion result)
        {
			Fix32 cosHalfTheta = (((start.W.Mul(end.W)).Add(start.X.Mul(end.X))).Add(start.Y.Mul(end.Y))).Add(start.Z.Mul(end.Z));
            if (cosHalfTheta < F64.C0)
            {
                //Negating a quaternion results in the same orientation, 
                //but we need cosHalfTheta to be positive to get the shortest path.
                end.X = end.X.Neg();
                end.Y = end.Y.Neg();
                end.Z = end.Z.Neg();
                end.W = end.W.Neg();
                cosHalfTheta = cosHalfTheta.Neg();
            }
            // If the orientations are similar enough, then just pick one of the inputs.
            if (cosHalfTheta > F64.C1m1em12)
            {
                result.W = start.W;
                result.X = start.X;
                result.Y = start.Y;
                result.Z = start.Z;
                return;
            }
            // Calculate temporary values.
            Fix32 halfTheta = Fix32Ext.Acos(cosHalfTheta);
			Fix32 sinHalfTheta = Fix32Ext.Sqrt(F64.C1.Sub(cosHalfTheta.Mul(cosHalfTheta)));

			Fix32 aFraction = Fix32Ext.Sin((F64.C1.Sub(interpolationAmount)).Mul(halfTheta)).Div(sinHalfTheta);
			Fix32 bFraction = Fix32Ext.Sin(interpolationAmount.Mul(halfTheta)).Div(sinHalfTheta);

            //Blend the two quaternions to get the result!
            result.X = ((start.X.Mul(aFraction)).Add(end.X.Mul(bFraction)));
            result.Y = ((start.Y.Mul(aFraction)).Add(end.Y.Mul(bFraction)));
            result.Z = ((start.Z.Mul(aFraction)).Add(end.Z.Mul(bFraction)));
            result.W = ((start.W.Mul(aFraction)).Add(end.W.Mul(bFraction)));




        }

        /// <summary>
        /// Blends two quaternions together to get an intermediate state.
        /// </summary>
        /// <param name="start">Starting point of the interpolation.</param>
        /// <param name="end">Ending point of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end point to use.</param>
        /// <returns>Interpolated intermediate quaternion.</returns>
        public static Quaternion Slerp(Quaternion start, Quaternion end, Fix32 interpolationAmount)
        {
            Quaternion toReturn;
            Slerp(ref start, ref end, interpolationAmount, out toReturn);
            return toReturn;
        }


        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <param name="result">Conjugated quaternion.</param>
        public static void Conjugate(ref Quaternion quaternion, out Quaternion result)
        {
            result.X = quaternion.X.Neg();
            result.Y = quaternion.Y.Neg();
            result.Z = quaternion.Z.Neg();
            result.W = quaternion.W;
        }

        /// <summary>
        /// Computes the conjugate of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to conjugate.</param>
        /// <returns>Conjugated quaternion.</returns>
        public static Quaternion Conjugate(Quaternion quaternion)
        {
            Quaternion toReturn;
            Conjugate(ref quaternion, out toReturn);
            return toReturn;
        }



        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <param name="result">Result of the inversion.</param>
        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            Fix32 inverseSquaredNorm = (((quaternion.X.Mul(quaternion.X)).Add(quaternion.Y.Mul(quaternion.Y))).Add(quaternion.Z.Mul(quaternion.Z))).Add(quaternion.W.Mul(quaternion.W));
            result.X = (quaternion.X.Neg()).Mul(inverseSquaredNorm);
            result.Y = (quaternion.Y.Neg()).Mul(inverseSquaredNorm);
            result.Z = (quaternion.Z.Neg()).Mul(inverseSquaredNorm);
            result.W = quaternion.W.Mul(inverseSquaredNorm);
        }

        /// <summary>
        /// Computes the inverse of the quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion to invert.</param>
        /// <returns>Result of the inversion.</returns>
        public static Quaternion Inverse(Quaternion quaternion)
        {
            Quaternion result;
            Inverse(ref quaternion, out result);
            return result;

        }

        /// <summary>
        /// Tests components for equality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether or not the quaternions' components were equal.</returns>
        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        /// <summary>
        /// Tests components for inequality.
        /// </summary>
        /// <param name="a">First quaternion to test for equivalence.</param>
        /// <param name="b">Second quaternion to test for equivalence.</param>
        /// <returns>Whether the quaternions' components were not equal.</returns>
        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="a">Quaternion to negate.</param>
        /// <param name="b">Negated result.</param>
        public static void Negate(ref Quaternion a, out Quaternion b)
        {
            b.X = a.X.Neg();
            b.Y = a.Y.Neg();
            b.Z = a.Z.Neg();
            b.W = a.W.Neg();
        }      
        
        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static Quaternion Negate(Quaternion q)
        {
            Negate(ref q, out var result);
            return result;
        }

        /// <summary>
        /// Negates the components of a quaternion.
        /// </summary>
        /// <param name="q">Quaternion to negate.</param>
        /// <returns>Negated result.</returns>
        public static Quaternion operator -(Quaternion q)
        {
            Negate(ref q, out var result);
            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Quaternion other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (int) X ^ (int) Y ^ (int) Z ^ (int) W;
        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void Transform(ref Vector3 v, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            Fix32 x2 = rotation.X.Add(rotation.X);
            Fix32 y2 = rotation.Y.Add(rotation.Y);
            Fix32 z2 = rotation.Z.Add(rotation.Z);
            Fix32 xx2 = rotation.X.Mul(x2);
            Fix32 xy2 = rotation.X.Mul(y2);
            Fix32 xz2 = rotation.X.Mul(z2);
            Fix32 yy2 = rotation.Y.Mul(y2);
            Fix32 yz2 = rotation.Y.Mul(z2);
            Fix32 zz2 = rotation.Z.Mul(z2);
            Fix32 wx2 = rotation.W.Mul(x2);
            Fix32 wy2 = rotation.W.Mul(y2);
            Fix32 wz2 = rotation.W.Mul(z2);
            //Defer the component setting since they're used in computation.
            Fix32 transformedX = ((v.X.Mul(((F64.C1.Sub(yy2)).Sub(zz2)))).Add(v.Y.Mul((xy2.Sub(wz2))))).Add(v.Z.Mul((xz2.Add(wy2))));
            Fix32 transformedY = ((v.X.Mul((xy2.Add(wz2)))).Add(v.Y.Mul(((F64.C1.Sub(xx2)).Sub(zz2))))).Add(v.Z.Mul((yz2.Sub(wx2))));
            Fix32 transformedZ = ((v.X.Mul((xz2.Sub(wy2)))).Add(v.Y.Mul((yz2.Add(wx2))))).Add(v.Z.Mul(((F64.C1.Sub(xx2)).Sub(yy2))));
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms the vector using a quaternion.
        /// </summary>
        /// <param name="v">Vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <returns>Transformed vector.</returns>
        public static Vector3 Transform(Vector3 v, Quaternion rotation)
        {
            Vector3 toReturn;
            Transform(ref v, ref rotation, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for x,0,0 vectors.
        /// </summary>
        /// <param name="x">X component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformX(Fix32 x, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            Fix32 y2 = rotation.Y.Add(rotation.Y);
            Fix32 z2 = rotation.Z.Add(rotation.Z);
            Fix32 xy2 = rotation.X.Mul(y2);
            Fix32 xz2 = rotation.X.Mul(z2);
            Fix32 yy2 = rotation.Y.Mul(y2);
            Fix32 zz2 = rotation.Z.Mul(z2);
            Fix32 wy2 = rotation.W.Mul(y2);
            Fix32 wz2 = rotation.W.Mul(z2);
            //Defer the component setting since they're used in computation.
            Fix32 transformedX = x.Mul(((F64.C1.Sub(yy2)).Sub(zz2)));
            Fix32 transformedY = x.Mul((xy2.Add(wz2)));
            Fix32 transformedZ = x.Mul((xz2.Sub(wy2)));
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,y,0 vectors.
        /// </summary>
        /// <param name="y">Y component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformY(Fix32 y, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            Fix32 x2 = rotation.X.Add(rotation.X);
            Fix32 y2 = rotation.Y.Add(rotation.Y);
            Fix32 z2 = rotation.Z.Add(rotation.Z);
            Fix32 xx2 = rotation.X.Mul(x2);
            Fix32 xy2 = rotation.X.Mul(y2);
            Fix32 yz2 = rotation.Y.Mul(z2);
            Fix32 zz2 = rotation.Z.Mul(z2);
            Fix32 wx2 = rotation.W.Mul(x2);
            Fix32 wz2 = rotation.W.Mul(z2);
            //Defer the component setting since they're used in computation.
            Fix32 transformedX = y.Mul((xy2.Sub(wz2)));
            Fix32 transformedY = y.Mul(((F64.C1.Sub(xx2)).Sub(zz2)));
            Fix32 transformedZ = y.Mul((yz2.Add(wx2)));
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }

        /// <summary>
        /// Transforms a vector using a quaternion. Specialized for 0,0,z vectors.
        /// </summary>
        /// <param name="z">Z component of the vector to transform.</param>
        /// <param name="rotation">Rotation to apply to the vector.</param>
        /// <param name="result">Transformed vector.</param>
        public static void TransformZ(Fix32 z, ref Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            Fix32 x2 = rotation.X.Add(rotation.X);
            Fix32 y2 = rotation.Y.Add(rotation.Y);
            Fix32 z2 = rotation.Z.Add(rotation.Z);
            Fix32 xx2 = rotation.X.Mul(x2);
            Fix32 xz2 = rotation.X.Mul(z2);
            Fix32 yy2 = rotation.Y.Mul(y2);
            Fix32 yz2 = rotation.Y.Mul(z2);
            Fix32 wx2 = rotation.W.Mul(x2);
            Fix32 wy2 = rotation.W.Mul(y2);
            //Defer the component setting since they're used in computation.
            Fix32 transformedX = z.Mul((xz2.Add(wy2)));
            Fix32 transformedY = z.Mul((yz2.Sub(wx2)));
            Fix32 transformedZ = z.Mul(((F64.C1.Sub(xx2)).Sub(yy2)));
            result.X = transformedX;
            result.Y = transformedY;
            result.Z = transformedZ;

        }


        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion to multiply.</param>
        /// <param name="b">Second quaternion to multiply.</param>
        /// <returns>Product of the multiplication.</returns>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Quaternion toReturn;
            Multiply(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <returns>Quaternion representing the axis and angle rotation.</returns>
        public static Quaternion CreateFromAxisAngle(Vector3 axis, Fix32 angle)
        {
			Fix32 halfAngle = angle.Mul(F64.C0p5);
			Fix32 s = Fix32Ext.Sin(halfAngle);
            Quaternion q;
            q.X = axis.X.Mul(s);
            q.Y = axis.Y.Mul(s);
            q.Z = axis.Z.Mul(s);
            q.W = Fix32Ext.Cos(halfAngle);
            return q;
        }

        /// <summary>
        /// Creates a quaternion from an axis and angle.
        /// </summary>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="angle">Angle to rotate around the axis.</param>
        /// <param name="q">Quaternion representing the axis and angle rotation.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, Fix32 angle, out Quaternion q)
        {
			Fix32 halfAngle = angle.Mul(F64.C0p5);
			Fix32 s = Fix32Ext.Sin(halfAngle);
            q.X = axis.X.Mul(s);
            q.Y = axis.Y.Mul(s);
            q.Z = axis.Z.Mul(s);
            q.W = Fix32Ext.Cos(halfAngle);
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <returns>Quaternion representing the yaw, pitch, and roll.</returns>
        public static Quaternion CreateFromYawPitchRoll(Fix32 yaw, Fix32 pitch, Fix32 roll)
        {
            Quaternion toReturn;
            CreateFromYawPitchRoll(yaw, pitch, roll, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Constructs a quaternion from yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw of the rotation.</param>
        /// <param name="pitch">Pitch of the rotation.</param>
        /// <param name="roll">Roll of the rotation.</param>
        /// <param name="q">Quaternion representing the yaw, pitch, and roll.</param>
        public static void CreateFromYawPitchRoll(Fix32 yaw, Fix32 pitch, Fix32 roll, out Quaternion q)
        {
			Fix32 halfRoll = roll.Mul(F64.C0p5);
			Fix32 halfPitch = pitch.Mul(F64.C0p5);
			Fix32 halfYaw = yaw.Mul(F64.C0p5);

			Fix32 sinRoll = Fix32Ext.Sin(halfRoll);
			Fix32 sinPitch = Fix32Ext.Sin(halfPitch);
			Fix32 sinYaw = Fix32Ext.Sin(halfYaw);

			Fix32 cosRoll = Fix32Ext.Cos(halfRoll);
			Fix32 cosPitch = Fix32Ext.Cos(halfPitch);
			Fix32 cosYaw = Fix32Ext.Cos(halfYaw);

			Fix32 cosYawCosPitch = cosYaw.Mul(cosPitch);
			Fix32 cosYawSinPitch = cosYaw.Mul(sinPitch);
			Fix32 sinYawCosPitch = sinYaw.Mul(cosPitch);
			Fix32 sinYawSinPitch = sinYaw.Mul(sinPitch);

            q.X = (cosYawSinPitch.Mul(cosRoll)).Add(sinYawCosPitch.Mul(sinRoll));
            q.Y = (sinYawCosPitch.Mul(cosRoll)).Sub(cosYawSinPitch.Mul(sinRoll));
            q.Z = (cosYawCosPitch.Mul(sinRoll)).Sub(sinYawSinPitch.Mul(cosRoll));
            q.W = (cosYawCosPitch.Mul(cosRoll)).Add(sinYawSinPitch.Mul(sinRoll));

        }

        /// <summary>
        /// Computes the angle change represented by a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <returns>Angle around the axis represented by the quaternion.</returns>
        public static Fix32 GetAngleFromQuaternion(ref Quaternion q)
        {
            Fix32 qw = Fix32Ext.Abs(q.W);
            if (qw > F64.C1)
                return F64.C0;
            return F64.C2.Mul(Fix32Ext.Acos(qw));
        }

        /// <summary>
        /// Computes the axis angle representation of a normalized quaternion.
        /// </summary>
        /// <param name="q">Quaternion to be converted.</param>
        /// <param name="axis">Axis represented by the quaternion.</param>
        /// <param name="angle">Angle around the axis represented by the quaternion.</param>
        public static void GetAxisAngleFromQuaternion(ref Quaternion q, out Vector3 axis, out Fix32 angle)
        {
#if !WINDOWS
            axis = new Vector3();
#endif
            Fix32 qw = q.W;
            if (qw > F64.C0)
            {
                axis.X = q.X;
                axis.Y = q.Y;
                axis.Z = q.Z;
            }
            else
            {
                axis.X = q.X.Neg();
                axis.Y = q.Y.Neg();
                axis.Z = q.Z.Neg();
                qw = qw.Neg();
            }

            Fix32 lengthSquared = axis.LengthSquared();
            if (lengthSquared > F64.C1em14)
            {
                Vector3.Divide(ref axis, Fix32Ext.Sqrt(lengthSquared), out axis);
                angle = F64.C2.Mul(Fix32Ext.Acos(MathHelper.Clamp(qw, F64.C1.Neg(), F64.C1)));
            }
            else
            {
                axis = Toolbox.UpVector;
                angle = F64.C0;
            }
        }

        /// <summary>
        /// Computes the quaternion rotation between two normalized vectors.
        /// </summary>
        /// <param name="v1">First unit-length vector.</param>
        /// <param name="v2">Second unit-length vector.</param>
        /// <param name="q">Quaternion representing the rotation from v1 to v2.</param>
        public static void GetQuaternionBetweenNormalizedVectors(ref Vector3 v1, ref Vector3 v2, out Quaternion q)
        {
            Fix32 dot;
            Vector3.Dot(ref v1, ref v2, out dot);
            //For non-normal vectors, the multiplying the axes length squared would be necessary:
            //Fix32 w = dot + (Fix32)Math.Sqrt(v1.LengthSquared() * v2.LengthSquared());
            if (dot < F64.Cm0p9999) //parallel, opposing direction
            {
                //If this occurs, the rotation required is ~180 degrees.
                //The problem is that we could choose any perpendicular axis for the rotation. It's not uniquely defined.
                //The solution is to pick an arbitrary perpendicular axis.
                //Project onto the plane which has the lowest component magnitude.
                //On that 2d plane, perform a 90 degree rotation.
                Fix32 absX = Fix32Ext.Abs(v1.X);
                Fix32 absY = Fix32Ext.Abs(v1.Y);
                Fix32 absZ = Fix32Ext.Abs(v1.Z);
                if (absX < absY && absX < absZ)
                    q = new Quaternion(F64.C0, v1.Z.Neg(), v1.Y, F64.C0);
                else if (absY < absZ)
                    q = new Quaternion(v1.Z.Neg(), F64.C0, v1.X, F64.C0);
                else
                    q = new Quaternion(v1.Y.Neg(), v1.X, F64.C0, F64.C0);
            }
            else
            {
                Vector3 axis;
                Vector3.Cross(ref v1, ref v2, out axis);
                q = new Quaternion(axis.X, axis.Y, axis.Z, dot.Add(F64.C1));
            }
            q.Normalize();
        }

        //The following two functions are highly similar, but it's a bit of a brain teaser to phrase one in terms of the other.
        //Providing both simplifies things.

        /// <summary>
        /// Computes the rotation from the start orientation to the end orientation such that end = Quaternion.Concatenate(start, relative).
        /// </summary>
        /// <param name="start">Starting orientation.</param>
        /// <param name="end">Ending orientation.</param>
        /// <param name="relative">Relative rotation from the start to the end orientation.</param>
        public static void GetRelativeRotation(ref Quaternion start, ref Quaternion end, out Quaternion relative)
        {
            Quaternion startInverse;
            Conjugate(ref start, out startInverse);
            Concatenate(ref startInverse, ref end, out relative);
        }

        
        /// <summary>
        /// Transforms the rotation into the local space of the target basis such that rotation = Quaternion.Concatenate(localRotation, targetBasis)
        /// </summary>
        /// <param name="rotation">Rotation in the original frame of reference.</param>
        /// <param name="targetBasis">Basis in the original frame of reference to transform the rotation into.</param>
        /// <param name="localRotation">Rotation in the local space of the target basis.</param>
        public static void GetLocalRotation(ref Quaternion rotation, ref Quaternion targetBasis, out Quaternion localRotation)
        {
            Quaternion basisInverse;
            Conjugate(ref targetBasis, out basisInverse);
            Concatenate(ref rotation, ref basisInverse, out localRotation);
        }

        /// <summary>
        /// Gets a string representation of the quaternion.
        /// </summary>
        /// <returns>String representing the quaternion.</returns>
        public override string ToString()
        {
            return "{ X: " + X.ToStringExt() + ", Y: " + Y.ToStringExt() + ", Z: " + Z.ToStringExt() + ", W: " + W.ToStringExt() + "}";
        }
    }
}
