using FixMath.NET;
using Microsoft.Xna.Framework;

namespace ConversionHelper
{
    /// <summary>
    /// Helps convert between XNA math types and the BEPUphysics replacement math types.
    /// A version of this converter could be created for other platforms to ease the integration of the engine.
    /// </summary>
    public static class MathConverter
    {
        //Vector2
        public static Vector2 Convert(BEPUutilities.Vector2 bepuVector)
        {
            Vector2 toReturn;
            toReturn.X = (float)bepuVector.X.ToFloat();
            toReturn.Y = (float)bepuVector.Y.ToFloat();
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Vector2 bepuVector, out Vector2 xnaVector)
        {
            xnaVector.X = (float)bepuVector.X.ToFloat();
            xnaVector.Y = (float)bepuVector.Y.ToFloat();
        }

        public static BEPUutilities.Vector2 Convert(Vector2 xnaVector)
        {
            BEPUutilities.Vector2 toReturn;
            toReturn.X = xnaVector.X.ToFix();
            toReturn.Y = xnaVector.Y.ToFix();
            return toReturn;
        }

        public static void Convert(ref Vector2 xnaVector, out BEPUutilities.Vector2 bepuVector)
        {
            bepuVector.X = xnaVector.X.ToFix();
            bepuVector.Y = xnaVector.Y.ToFix();
        }

        //Vector3
        public static Vector3 Convert(BEPUutilities.Vector3 bepuVector)
        {
            Vector3 toReturn;
            toReturn.X = (float)bepuVector.X.ToFloat();
            toReturn.Y = (float)bepuVector.Y.ToFloat();
            toReturn.Z = (float)bepuVector.Z.ToFloat();
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Vector3 bepuVector, out Vector3 xnaVector)
        {
            xnaVector.X = (float)bepuVector.X.ToFloat();
            xnaVector.Y = (float)bepuVector.Y.ToFloat();
            xnaVector.Z = (float)bepuVector.Z.ToFloat();
        }

        public static BEPUutilities.Vector3 Convert(Vector3 xnaVector)
        {
            BEPUutilities.Vector3 toReturn;
            toReturn.X = xnaVector.X.ToFix();
            toReturn.Y = xnaVector.Y.ToFix();
            toReturn.Z = xnaVector.Z.ToFix();
            return toReturn;
        }

        public static void Convert(ref Vector3 xnaVector, out BEPUutilities.Vector3 bepuVector)
        {
            bepuVector.X = xnaVector.X.ToFix();
            bepuVector.Y = xnaVector.Y.ToFix();
            bepuVector.Z = xnaVector.Z.ToFix();
        }

        public static Vector3[] Convert(BEPUutilities.Vector3[] bepuVectors)
        {
            Vector3[] xnaVectors = new Vector3[bepuVectors.Length];
            for (int i = 0; i < bepuVectors.Length; i++)
            {
                Convert(ref bepuVectors[i], out xnaVectors[i]);
            }
            return xnaVectors;

        }

        public static BEPUutilities.Vector3[] Convert(Vector3[] xnaVectors)
        {
            var bepuVectors = new BEPUutilities.Vector3[xnaVectors.Length];
            for (int i = 0; i < xnaVectors.Length; i++)
            {
                Convert(ref xnaVectors[i], out bepuVectors[i]);
            }
            return bepuVectors;

        }

        //Matrix
        public static Matrix Convert(BEPUutilities.Matrix matrix)
        {
            Matrix toReturn;
            Convert(ref matrix, out toReturn);
            return toReturn;
        }

        public static BEPUutilities.Matrix Convert(Matrix matrix)
        {
            BEPUutilities.Matrix toReturn;
            Convert(ref matrix, out toReturn);
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Matrix matrix, out Matrix xnaMatrix)
        {
            xnaMatrix.M11 = (float)matrix.M11.ToFloat();
            xnaMatrix.M12 = (float)matrix.M12.ToFloat();
            xnaMatrix.M13 = (float)matrix.M13.ToFloat();
            xnaMatrix.M14 = (float)matrix.M14.ToFloat();

            xnaMatrix.M21 = (float)matrix.M21.ToFloat();
            xnaMatrix.M22 = (float)matrix.M22.ToFloat();
            xnaMatrix.M23 = (float)matrix.M23.ToFloat();
            xnaMatrix.M24 = (float)matrix.M24.ToFloat();

            xnaMatrix.M31 = (float)matrix.M31.ToFloat();
            xnaMatrix.M32 = (float)matrix.M32.ToFloat();
            xnaMatrix.M33 = (float)matrix.M33.ToFloat();
            xnaMatrix.M34 = (float)matrix.M34.ToFloat();

            xnaMatrix.M41 = (float)matrix.M41.ToFloat();
            xnaMatrix.M42 = (float)matrix.M42.ToFloat();
            xnaMatrix.M43 = (float)matrix.M43.ToFloat();
            xnaMatrix.M44 = (float)matrix.M44.ToFloat();

        }

        public static void Convert(ref Matrix matrix, out BEPUutilities.Matrix bepuMatrix)
        {
            bepuMatrix.M11 = matrix.M11.ToFix();
            bepuMatrix.M12 = matrix.M12.ToFix();
            bepuMatrix.M13 = matrix.M13.ToFix();
            bepuMatrix.M14 = matrix.M14.ToFix();

            bepuMatrix.M21 = matrix.M21.ToFix();
            bepuMatrix.M22 = matrix.M22.ToFix();
            bepuMatrix.M23 = matrix.M23.ToFix();
            bepuMatrix.M24 = matrix.M24.ToFix();

            bepuMatrix.M31 = matrix.M31.ToFix();
            bepuMatrix.M32 = matrix.M32.ToFix();
            bepuMatrix.M33 = matrix.M33.ToFix();
            bepuMatrix.M34 = matrix.M34.ToFix();

            bepuMatrix.M41 = matrix.M41.ToFix();
            bepuMatrix.M42 = matrix.M42.ToFix();
            bepuMatrix.M43 = matrix.M43.ToFix();
            bepuMatrix.M44 = matrix.M44.ToFix();

        }

        public static Matrix Convert(BEPUutilities.Matrix3x3 matrix)
        {
            Matrix toReturn;
            Convert(ref matrix, out toReturn);
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Matrix3x3 matrix, out Matrix xnaMatrix)
        {
            xnaMatrix.M11 = (float)matrix.M11.ToFloat();
            xnaMatrix.M12 = (float)matrix.M12.ToFloat();
            xnaMatrix.M13 = (float)matrix.M13.ToFloat();
            xnaMatrix.M14 = 0;

            xnaMatrix.M21 = (float)matrix.M21.ToFloat();
            xnaMatrix.M22 = (float)matrix.M22.ToFloat();
            xnaMatrix.M23 = (float)matrix.M23.ToFloat();
            xnaMatrix.M24 = 0;

            xnaMatrix.M31 = (float)matrix.M31.ToFloat();
            xnaMatrix.M32 = (float)matrix.M32.ToFloat();
            xnaMatrix.M33 = (float)matrix.M33.ToFloat();
            xnaMatrix.M34 = 0;

            xnaMatrix.M41 = 0;
            xnaMatrix.M42 = 0;
            xnaMatrix.M43 = 0;
            xnaMatrix.M44 = 1;
        }

        public static void Convert(ref Matrix matrix, out BEPUutilities.Matrix3x3 bepuMatrix)
        {
            bepuMatrix.M11 = matrix.M11.ToFix();
            bepuMatrix.M12 = matrix.M12.ToFix();
            bepuMatrix.M13 = matrix.M13.ToFix();

            bepuMatrix.M21 = matrix.M21.ToFix();
            bepuMatrix.M22 = matrix.M22.ToFix();
            bepuMatrix.M23 = matrix.M23.ToFix();

            bepuMatrix.M31 = matrix.M31.ToFix();
            bepuMatrix.M32 = matrix.M32.ToFix();
            bepuMatrix.M33 = matrix.M33.ToFix();

        }

        //Quaternion
        public static Quaternion Convert(BEPUutilities.Quaternion quaternion)
        {
            Quaternion toReturn;
            toReturn.X = (float)quaternion.X.ToFloat();
            toReturn.Y = (float)quaternion.Y.ToFloat();
            toReturn.Z = (float)quaternion.Z.ToFloat();
            toReturn.W = (float)quaternion.W.ToFloat();
            return toReturn;
        }

        public static BEPUutilities.Quaternion Convert(Quaternion quaternion)
        {
            BEPUutilities.Quaternion toReturn;
            toReturn.X = quaternion.X.ToFix();
            toReturn.Y = quaternion.Y.ToFix();
            toReturn.Z = quaternion.Z.ToFix();
            toReturn.W = quaternion.W.ToFix();
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Quaternion bepuQuaternion, out Quaternion quaternion)
        {
            quaternion.X = (float)bepuQuaternion.X.ToFloat();
            quaternion.Y = (float)bepuQuaternion.Y.ToFloat();
            quaternion.Z = (float)bepuQuaternion.Z.ToFloat();
            quaternion.W = (float)bepuQuaternion.W.ToFloat();
        }

        public static void Convert(ref Quaternion quaternion, out  BEPUutilities.Quaternion bepuQuaternion)
        {
            bepuQuaternion.X = quaternion.X.ToFix();
            bepuQuaternion.Y = quaternion.Y.ToFix();
            bepuQuaternion.Z = quaternion.Z.ToFix();
            bepuQuaternion.W = quaternion.W.ToFix();
        }

        //Ray
        public static BEPUutilities.Ray Convert(Ray ray)
        {
            BEPUutilities.Ray toReturn;
            Convert(ref ray.Position, out toReturn.Position);
            Convert(ref ray.Direction, out toReturn.Direction);
            return toReturn;
        }

        public static void Convert(ref Ray ray, out BEPUutilities.Ray bepuRay)
        {
            Convert(ref ray.Position, out bepuRay.Position);
            Convert(ref ray.Direction, out bepuRay.Direction);
        }

        public static Ray Convert(BEPUutilities.Ray ray)
        {
            Ray toReturn;
            Convert(ref ray.Position, out toReturn.Position);
            Convert(ref ray.Direction, out toReturn.Direction);
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Ray ray, out Ray xnaRay)
        {
            Convert(ref ray.Position, out xnaRay.Position);
            Convert(ref ray.Direction, out xnaRay.Direction);
        }

        //BoundingBox
        public static BoundingBox Convert(BEPUutilities.BoundingBox boundingBox)
        {
            BoundingBox toReturn;
            Convert(ref boundingBox.Min, out toReturn.Min);
            Convert(ref boundingBox.Max, out toReturn.Max);
            return toReturn;
        }

        public static BEPUutilities.BoundingBox Convert(BoundingBox boundingBox)
        {
            BEPUutilities.BoundingBox toReturn;
            Convert(ref boundingBox.Min, out toReturn.Min);
            Convert(ref boundingBox.Max, out toReturn.Max);
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.BoundingBox boundingBox, out BoundingBox xnaBoundingBox)
        {
            Convert(ref boundingBox.Min, out xnaBoundingBox.Min);
            Convert(ref boundingBox.Max, out xnaBoundingBox.Max);
        }

        public static void Convert(ref BoundingBox boundingBox, out BEPUutilities.BoundingBox bepuBoundingBox)
        {
            Convert(ref boundingBox.Min, out bepuBoundingBox.Min);
            Convert(ref boundingBox.Max, out bepuBoundingBox.Max);
        }

        //BoundingSphere
        public static BoundingSphere Convert(BEPUutilities.BoundingSphere boundingSphere)
        {
            BoundingSphere toReturn;
            Convert(ref boundingSphere.Center, out toReturn.Center);
            toReturn.Radius = (float)boundingSphere.Radius.ToFloat();
            return toReturn;
        }

        public static BEPUutilities.BoundingSphere Convert(BoundingSphere boundingSphere)
        {
            BEPUutilities.BoundingSphere toReturn;
            Convert(ref boundingSphere.Center, out toReturn.Center);
            toReturn.Radius = boundingSphere.Radius.ToFix();
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.BoundingSphere boundingSphere, out BoundingSphere xnaBoundingSphere)
        {
            Convert(ref boundingSphere.Center, out xnaBoundingSphere.Center);
            xnaBoundingSphere.Radius = (float)boundingSphere.Radius.ToFloat();
        }

        public static void Convert(ref BoundingSphere boundingSphere, out BEPUutilities.BoundingSphere bepuBoundingSphere)
        {
            Convert(ref boundingSphere.Center, out bepuBoundingSphere.Center);
            bepuBoundingSphere.Radius = boundingSphere.Radius.ToFix();
        }

        //Plane
        public static Plane Convert(BEPUutilities.Plane plane)
        {
            Plane toReturn;
            Convert(ref plane.Normal, out toReturn.Normal);
            toReturn.D = (float)plane.D.ToFloat();
            return toReturn;
        }

        public static BEPUutilities.Plane Convert(Plane plane)
        {
            BEPUutilities.Plane toReturn;
            Convert(ref plane.Normal, out toReturn.Normal);
            toReturn.D = plane.D.ToFix();
            return toReturn;
        }

        public static void Convert(ref BEPUutilities.Plane plane, out Plane xnaPlane)
        {
            Convert(ref plane.Normal, out xnaPlane.Normal);
            xnaPlane.D = (float)plane.D.ToFloat();
        }

        public static void Convert(ref Plane plane, out BEPUutilities.Plane bepuPlane)
        {
            Convert(ref plane.Normal, out bepuPlane.Normal);
            bepuPlane.D = plane.D.ToFix();
        }
    }
}
