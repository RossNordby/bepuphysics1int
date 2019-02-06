using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Convex shape with width, length, and height.
    ///</summary>
    public class BoxShape : ConvexShape
    {
        internal Fix64 halfWidth;
        internal Fix64 halfHeight;
        internal Fix64 halfLength;


        /// <summary>
        /// Width of the box divided by two.
        /// </summary>
        public Fix64 HalfWidth
        {
            get { return halfWidth; }
            set { halfWidth = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Height of the box divided by two.
        /// </summary>
        public Fix64 HalfHeight
        {
            get { return halfHeight; }
            set { halfHeight = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Length of the box divided by two.
        /// </summary>
        public Fix64 HalfLength
        {
            get { return halfLength; }
            set { halfLength = value; OnShapeChanged(); }
        }

        /// <summary>
        /// Width of the box.
        /// </summary>
        public Fix64 Width
        {
            get { return halfWidth.Mul(F64.C2); }
            set { halfWidth = value.Mul(F64.C0p5); OnShapeChanged(); }
        }

        /// <summary>
        /// Height of the box.
        /// </summary>
        public Fix64 Height
        {
            get { return halfHeight.Mul(F64.C2); }
            set { halfHeight = value.Mul(F64.C0p5); OnShapeChanged(); }
        }

        /// <summary>
        /// Length of the box.
        /// </summary>
        public Fix64 Length
        {
            get { return halfLength.Mul(F64.C2); }
            set { halfLength = value.Mul(F64.C0p5); OnShapeChanged(); }
        }


        ///<summary>
        /// Constructs a new box shape.
        ///</summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        public BoxShape(Fix64 width, Fix64 height, Fix64 length)
        {
            halfWidth = width.Mul(F64.C0p5);
            halfHeight = height.Mul(F64.C0p5);
            halfLength = length.Mul(F64.C0p5);

            UpdateConvexShapeInfo(ComputeDescription(width, height, length, collisionMargin));
        }

        ///<summary>
        /// Constructs a new box shape from cached information.
        ///</summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public BoxShape(Fix64 width, Fix64 height, Fix64 length, ConvexShapeDescription description)
        {
            halfWidth = width.Mul(F64.C0p5);
            halfHeight = height.Mul(F64.C0p5);
            halfLength = length.Mul(F64.C0p5);

            UpdateConvexShapeInfo(description);
        }

        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(halfWidth, halfHeight, halfLength, collisionMargin));
            base.OnShapeChanged();
        }

        /// <summary>
        /// Computes a convex shape description for a BoxShape.
        /// </summary>
        ///<param name="width">Width of the box.</param>
        ///<param name="height">Height of the box.</param>
        ///<param name="length">Length of the box.</param>
        /// <param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(Fix64 width, Fix64 height, Fix64 length, Fix64 collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = (width.Mul(height)).Mul(length);

            Fix64 widthSquared = width.Mul(width);
            Fix64 heightSquared = height.Mul(height);
            Fix64 lengthSquared = length.Mul(length);
			Fix64 inv12 = F64.OneTwelfth;

            description.EntityShapeVolume.VolumeDistribution = new Matrix3x3();
            description.EntityShapeVolume.VolumeDistribution.M11 = (heightSquared.Add(lengthSquared)).Mul(inv12);
            description.EntityShapeVolume.VolumeDistribution.M22 = (widthSquared.Add(lengthSquared)).Mul(inv12);
            description.EntityShapeVolume.VolumeDistribution.M33 = (widthSquared.Add(heightSquared)).Mul(inv12);

            description.MaximumRadius = F64.C0p5.Mul(Fix64Ext.Sqrt(((width.Mul(width)).Add(height.Mul(height))).Add(length.Mul(length))));
            description.MinimumRadius = F64.C0p5.Mul(MathHelper.Min(width, MathHelper.Min(height, length)));

            description.CollisionMargin = collisionMargin;
            return description;
        }





        /// <summary>
        /// Gets the bounding box of the shape given a transform.
        /// </summary>
        /// <param name="shapeTransform">Transform to use.</param>
        /// <param name="boundingBox">Bounding box of the transformed shape.</param>
        public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
        {
#if !WINDOWS
            boundingBox = new BoundingBox();
#endif

            Matrix3x3 o;
            Matrix3x3.CreateFromQuaternion(ref shapeTransform.Orientation, out o);
            //Sample the local directions from the orientation matrix, implicitly transposed.
            //Notice only three directions are used.  Due to box symmetry, 'left' is just -right.
            var right = new Vector3(Fix64Ext.Sign(o.M11).Mul(halfWidth), Fix64Ext.Sign(o.M21).Mul(halfHeight), Fix64Ext.Sign(o.M31).Mul(halfLength));

            var up = new Vector3(Fix64Ext.Sign(o.M12).Mul(halfWidth), Fix64Ext.Sign(o.M22).Mul(halfHeight), Fix64Ext.Sign(o.M32).Mul(halfLength));

            var backward = new Vector3(Fix64Ext.Sign(o.M13).Mul(halfWidth), Fix64Ext.Sign(o.M23).Mul(halfHeight), Fix64Ext.Sign(o.M33).Mul(halfLength));


            //Rather than transforming each axis independently (and doing three times as many operations as required), just get the 3 required values directly.
            Vector3 offset;
            TransformLocalExtremePoints(ref right, ref up, ref backward, ref o, out offset);

            //The positive and negative vectors represent the X, Y and Z coordinates of the extreme points in world space along the world space axes.
            Vector3.Add(ref shapeTransform.Position, ref offset, out boundingBox.Max);
            Vector3.Subtract(ref shapeTransform.Position, ref offset, out boundingBox.Min);

        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
        {
            extremePoint = new Vector3(Fix64Ext.Sign(direction.X).Mul((halfWidth.Sub(collisionMargin))), Fix64Ext.Sign(direction.Y).Mul((halfHeight.Sub(collisionMargin))), Fix64Ext.Sign(direction.Z).Mul((halfLength.Sub(collisionMargin))));
        }




        /// <summary>
        /// Gets the intersection between the box and the ray.
        /// </summary>
        /// <param name="ray">Ray to test against the box.</param>
        /// <param name="transform">Transform of the shape.</param>
        /// <param name="maximumLength">Maximum distance to travel in units of the direction vector's length.</param>
        /// <param name="hit">Hit data for the raycast, if any.</param>
        /// <returns>Whether or not the ray hit the target.</returns>
        public override bool RayTest(ref Ray ray, ref RigidTransform transform, Fix64 maximumLength, out RayHit hit)
        {
            hit = new RayHit();

            Quaternion conjugate;
            Quaternion.Conjugate(ref transform.Orientation, out conjugate);
            Vector3 localOrigin;
            Vector3.Subtract(ref ray.Position, ref transform.Position, out localOrigin);
            Quaternion.Transform(ref localOrigin, ref conjugate, out localOrigin);
            Vector3 localDirection;
            Quaternion.Transform(ref ray.Direction, ref conjugate, out localDirection);
            Vector3 normal = Toolbox.ZeroVector;
            Fix64 temp, tmin = F64.C0, tmax = maximumLength;

            if (Fix64Ext.Abs(localDirection.X) < Toolbox.Epsilon && (localOrigin.X < halfWidth.Neg() || localOrigin.X > halfWidth))
                return false;
            Fix64 inverseDirection = F64.C1.Div(localDirection.X);
			// inverseDirection might be Infinity (Fix64.MaxValue), so use SafeMul here to handle overflow
            Fix64 t1 = Fix64Ext.SafeMul(((halfWidth.Neg()).Sub(localOrigin.X)), inverseDirection);
            Fix64 t2 = Fix64Ext.SafeMul((halfWidth.Sub(localOrigin.X)), inverseDirection);
            var tempNormal = new Vector3(F64.C1.Neg(), F64.C0, F64.C0);
            if (t1 > t2)
            {
                temp = t1;
                t1 = t2;
                t2 = temp;
                tempNormal *= F64.C1.Neg();
            }
            temp = tmin;
            tmin = MathHelper.Max(tmin, t1);
            if (temp != tmin)
                normal = tempNormal;
            tmax = MathHelper.Min(tmax, t2);
            if (tmin > tmax)
                return false;
            if (Fix64Ext.Abs(localDirection.Y) < Toolbox.Epsilon && (localOrigin.Y < halfHeight.Neg() || localOrigin.Y > halfHeight))
                return false;
            inverseDirection = F64.C1.Div(localDirection.Y);
            t1 = Fix64Ext.SafeMul(((halfHeight.Neg()).Sub(localOrigin.Y)), inverseDirection);
            t2 = Fix64Ext.SafeMul((halfHeight.Sub(localOrigin.Y)), inverseDirection);
            tempNormal = new Vector3(F64.C0, F64.C1.Neg(), F64.C0);
            if (t1 > t2)
            {
                temp = t1;
                t1 = t2;
                t2 = temp;
                tempNormal *= F64.C1.Neg();
            }
            temp = tmin;
            tmin = MathHelper.Max(tmin, t1);
            if (temp != tmin)
                normal = tempNormal;
            tmax = MathHelper.Min(tmax, t2);
            if (tmin > tmax)
                return false;
            if (Fix64Ext.Abs(localDirection.Z) < Toolbox.Epsilon && (localOrigin.Z < halfLength.Neg() || localOrigin.Z > halfLength))
                return false;
            inverseDirection = F64.C1.Div(localDirection.Z);
            t1 = Fix64Ext.SafeMul(((halfLength.Neg()).Sub(localOrigin.Z)), inverseDirection);
            t2 = Fix64Ext.SafeMul((halfLength.Sub(localOrigin.Z)), inverseDirection);
            tempNormal = new Vector3(F64.C0, F64.C0, F64.C1.Neg());
            if (t1 > t2)
            {
                temp = t1;
                t1 = t2;
                t2 = temp;
                tempNormal *= F64.C1.Neg();
            }
            temp = tmin;
            tmin = MathHelper.Max(tmin, t1);
            if (temp != tmin)
                normal = tempNormal;
            tmax = MathHelper.Min(tmax, t2);
            if (tmin > tmax)
                return false;
            hit.T = tmin;
            Vector3.Multiply(ref ray.Direction, tmin, out hit.Location);
            Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
            Quaternion.Transform(ref normal, ref transform.Orientation, out normal);
            hit.Normal = normal;
            return true;
        }

        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<BoxShape>(this);
        }

    }
}
