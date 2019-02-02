using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
 
using BEPUphysics.Settings;


namespace BEPUphysics.CollisionTests.CollisionAlgorithms
{
    ///<summary>
    /// Static class with methods to help with testing box shapes against sphere shapes.
    ///</summary>
    public static class BoxSphereTester
    {
        ///<summary>
        /// Tests if a box and sphere are colliding.
        ///</summary>
        ///<param name="box">Box to test.</param>
        ///<param name="sphere">Sphere to test.</param>
        ///<param name="boxTransform">Transform to apply to the box.</param>
        ///<param name="spherePosition">Transform to apply to the sphere.</param>
        ///<param name="contact">Contact point between the shapes, if any.</param>
        ///<returns>Whether or not the shapes were colliding.</returns>
        public static bool AreShapesColliding(BoxShape box, SphereShape sphere, ref RigidTransform boxTransform, ref Vector3 spherePosition, out ContactData contact)
        {
            contact = new ContactData();

            Vector3 localPosition;
            RigidTransform.TransformByInverse(ref spherePosition, ref boxTransform, out localPosition);
#if !WINDOWS
            Vector3 localClosestPoint = new Vector3();
#else
            Vector3 localClosestPoint;
#endif
            localClosestPoint.X = MathHelper.Clamp(localPosition.X, box.halfWidth .Neg(), box.halfWidth);
            localClosestPoint.Y = MathHelper.Clamp(localPosition.Y, box.halfHeight.Neg(), box.halfHeight);
            localClosestPoint.Z = MathHelper.Clamp(localPosition.Z, box.halfLength.Neg(), box.halfLength);

            RigidTransform.Transform(ref localClosestPoint, ref boxTransform, out contact.Position);

            Vector3 offset;
            Vector3.Subtract(ref spherePosition, ref contact.Position, out offset);
            Fix32 offsetLength = offset.LengthSquared();

            if (offsetLength > (sphere.collisionMargin .Add (CollisionDetectionSettings.maximumContactDistance)) .Mul (sphere.collisionMargin .Add (CollisionDetectionSettings.maximumContactDistance)))
            {
                return false;
            }

            //Colliding.
            if (offsetLength > Toolbox.Epsilon)
            {
                offsetLength = offsetLength.Sqrt();
                //Outside of the box.
                Vector3.Divide(ref offset, offsetLength, out contact.Normal);
                contact.PenetrationDepth = sphere.collisionMargin .Sub (offsetLength);
            }
            else
            {
                //Inside of the box.
                Vector3 penetrationDepths;
                penetrationDepths.X = localClosestPoint.X < Fix32.Zero ? localClosestPoint.X .Add (box.halfWidth) : box.halfWidth .Sub (localClosestPoint.X);
                penetrationDepths.Y = localClosestPoint.Y < Fix32.Zero ? localClosestPoint.Y .Add (box.halfHeight) : box.halfHeight .Sub (localClosestPoint.Y);
                penetrationDepths.Z = localClosestPoint.Z < Fix32.Zero ? localClosestPoint.Z .Add (box.halfLength) : box.halfLength .Sub (localClosestPoint.Z);
                if (penetrationDepths.X < penetrationDepths.Y && penetrationDepths.X < penetrationDepths.Z)
                {
                    contact.Normal = localClosestPoint.X > Fix32.Zero ? Toolbox.RightVector : Toolbox.LeftVector; 
                    contact.PenetrationDepth = penetrationDepths.X;
                }
                else if (penetrationDepths.Y < penetrationDepths.Z)
                {
                    contact.Normal = localClosestPoint.Y > Fix32.Zero ? Toolbox.UpVector : Toolbox.DownVector; 
                    contact.PenetrationDepth = penetrationDepths.Y;
                }
                else
                {
                    contact.Normal = localClosestPoint.Z > Fix32.Zero ? Toolbox.BackVector : Toolbox.ForwardVector; 
                    contact.PenetrationDepth = penetrationDepths.Z;
                }
                contact.PenetrationDepth = contact.PenetrationDepth .Add (sphere.collisionMargin);
                Quaternion.Transform(ref contact.Normal, ref boxTransform.Orientation, out contact.Normal);
            }


            return true;
        }
    }
}
