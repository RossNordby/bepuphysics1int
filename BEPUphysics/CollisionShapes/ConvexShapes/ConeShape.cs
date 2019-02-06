using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

using BEPUutilities;


namespace BEPUphysics.CollisionShapes.ConvexShapes
{
    ///<summary>
    /// Symmetrical shape with a circular base and a point at the top.
    ///</summary>
    public class ConeShape : ConvexShape
    {

        Fix32 height;
        ///<summary>
        /// Gets or sets the height of the cone.
        ///</summary>
        public Fix32 Height
        {
            get { return height; }
            set
            {
                height = value;
                OnShapeChanged();
            }
        }

        Fix32 radius;
        ///<summary>
        /// Gets or sets the radius of the cone base.
        ///</summary>
        public Fix32 Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnShapeChanged();
            }
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        public ConeShape(Fix32 height, Fix32 radius)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
        }

        ///<summary>
        /// Constructs a new cone shape.
        ///</summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        /// <param name="description">Cached information about the shape. Assumed to be correct; no extra processing or validation is performed.</param>
        public ConeShape(Fix32 height, Fix32 radius, ConvexShapeDescription description)
        {
            this.height = height;
            this.radius = radius;

            UpdateConvexShapeInfo(description);
        }


        protected override void OnShapeChanged()
        {
            UpdateConvexShapeInfo(ComputeDescription(height, radius, collisionMargin));
            base.OnShapeChanged();
        }


        /// <summary>
        /// Computes a convex shape description for a ConeShape.
        /// </summary>
        ///<param name="height">Height of the cone.</param>
        ///<param name="radius">Radius of the cone base.</param>
        ///<param name="collisionMargin">Collision margin of the shape.</param>
        /// <returns>Description required to define a convex shape.</returns>
        public static ConvexShapeDescription ComputeDescription(Fix32 height, Fix32 radius, Fix32 collisionMargin)
        {
            ConvexShapeDescription description;
            description.EntityShapeVolume.Volume = (((F64.OneThird.Mul(MathHelper.Pi)).Mul(radius)).Mul(radius)).Mul(height);

            description.EntityShapeVolume.VolumeDistribution = new Matrix3x3();
            Fix32 diagValue = (((F64.C0p1.Mul(height)).Mul(height)).Add((F64.C0p15.Mul(radius)).Mul(radius)));
            description.EntityShapeVolume.VolumeDistribution.M11 = diagValue;
            description.EntityShapeVolume.VolumeDistribution.M22 = (F64.C0p3.Mul(radius)).Mul(radius);
            description.EntityShapeVolume.VolumeDistribution.M33 = diagValue;

            description.MaximumRadius = collisionMargin.Add(MathHelper.Max(F64.C0p75.Mul(height), Fix32Ext.Sqrt(((F64.C0p0625.Mul(height)).Mul(height)).Add(radius.Mul(radius)))));

            Fix32 denominator = radius.Div(height);
            denominator = denominator.Div(Fix32Ext.Sqrt((denominator.Mul(denominator)).Add(F64.C1)));
            description.MinimumRadius = collisionMargin.Add(MathHelper.Min(F64.C0p25.Mul(height), (denominator.Mul(F64.C0p75)).Mul(height)));

            description.CollisionMargin = collisionMargin;
            return description;
        }


        ///<summary>
        /// Gets the extreme point of the shape in local space in a given direction.
        ///</summary>
        ///<param name="direction">Direction to find the extreme point in.</param>
        ///<param name="extremePoint">Extreme point on the shape.</param>
        public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
        {
            //Is it the tip of the cone?
            Fix32 sinThetaSquared = (radius.Mul(radius)).Div(((radius.Mul(radius)).Add(height.Mul(height))));
            //If d.Y * d.Y / d.LengthSquared >= sinthetaSquared
            if (direction.Y > F64.C0 && direction.Y.Mul(direction.Y) >= direction.LengthSquared().Mul(sinThetaSquared))
            {
                extremePoint = new Vector3(F64.C0, F64.C0p75.Mul(height), F64.C0);
                return;
            }
            //Is it a bottom edge of the cone?
            Fix32 horizontalLengthSquared = (direction.X.Mul(direction.X)).Add(direction.Z.Mul(direction.Z));
            if (horizontalLengthSquared > Toolbox.Epsilon)
            {
                var radOverSigma = radius.Div(Fix32Ext.Sqrt(horizontalLengthSquared));
                extremePoint = new Vector3((radOverSigma.Mul(direction.X)), F64.Cm0p25.Mul(height), (radOverSigma.Mul(direction.Z)));
            }
            else // It's pointing almost straight down...
                extremePoint = new Vector3(F64.C0, F64.Cm0p25.Mul(height), F64.C0);


        }


        /// <summary>
        /// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
        /// </summary>
        /// <returns>EntityCollidable that uses this shape.</returns>
        public override EntityCollidable GetCollidableInstance()
        {
            return new ConvexCollidable<ConeShape>(this);
        }

    }
}
