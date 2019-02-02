using System;
using BEPUphysics.Entities;
using BEPUutilities;
 
using BEPUphysics.Settings;
using BEPUutilities.DataStructures;


namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces to slow down and stop sliding motion between two entities when centralized friction is active.
    /// </summary>
    public class SlidingFrictionTwoAxis : SolverUpdateable
    {
        private ConvexContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the contact manifold constraint that owns this constraint.
        ///</summary>
        public ConvexContactManifoldConstraint ContactManifoldConstraint
        {
            get
            {
                return contactManifoldConstraint;
            }
        }
        internal Vector2 accumulatedImpulse;
        internal Matrix2x3 angularA, angularB;
        private int contactCount;
        private Fix32 friction;
        internal Matrix2x3 linearA;
        private Entity entityA, entityB;
        private bool entityADynamic, entityBDynamic;
        private Vector3 ra, rb;
        private Matrix2x2 velocityToImpulse;


        /// <summary>
        /// Gets the first direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public Vector3 FrictionDirectionX
        {
            get { return new Vector3(linearA.M11, linearA.M12, linearA.M13); }
        }

        /// <summary>
        /// Gets the second direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public Vector3 FrictionDirectionY
        {
            get { return new Vector3(linearA.M21, linearA.M22, linearA.M23); }
        }

        /// <summary>
        /// Gets the total impulse applied by sliding friction in the last time step.
        /// The X component of this vector is the force applied along the frictionDirectionX,
        /// while the Y component is the force applied along the frictionDirectionY.
        /// </summary>
        public Vector2 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the tangential relative velocity between the associated entities at the contact point.
        ///</summary>
        public Vector2 RelativeVelocity
        {
            get
            {
                //Compute relative velocity
                //Explicit version:
                //Vector2 dot;
                //Matrix2x3.Transform(ref parentA.myInternalLinearVelocity, ref linearA, out lambda);
                //Matrix2x3.Transform(ref parentB.myInternalLinearVelocity, ref linearA, out dot);
                //lambda.X -= dot.X; lambda.Y = lambda.Y - (dot.Y);
                //Matrix2x3.Transform(ref parentA.myInternalAngularVelocity, ref angularA, out dot);
                //lambda.X += dot.X; lambda.Y = lambda.Y + (dot.Y);
                //Matrix2x3.Transform(ref parentB.myInternalAngularVelocity, ref angularB, out dot);
                //lambda.X += dot.X; lambda.Y = lambda.Y + (dot.Y);

                //Inline version:
                //lambda.X = linearA.M11 * parentA.myInternalLinearVelocity.X + linearA.M12 * parentA.myInternalLinearVelocity.Y + linearA.M13 * parentA.myInternalLinearVelocity.Z -
                //           linearA.M11 * parentB.myInternalLinearVelocity.X - linearA.M12 * parentB.myInternalLinearVelocity.Y - linearA.M13 * parentB.myInternalLinearVelocity.Z +
                //           angularA.M11 * parentA.myInternalAngularVelocity.X + angularA.M12 * parentA.myInternalAngularVelocity.Y + angularA.M13 * parentA.myInternalAngularVelocity.Z +
                //           angularB.M11 * parentB.myInternalAngularVelocity.X + angularB.M12 * parentB.myInternalAngularVelocity.Y + angularB.M13 * parentB.myInternalAngularVelocity.Z;
                //lambda.Y = linearA.M21 * parentA.myInternalLinearVelocity.X + linearA.M22 * parentA.myInternalLinearVelocity.Y + linearA.M23 * parentA.myInternalLinearVelocity.Z -
                //           linearA.M21 * parentB.myInternalLinearVelocity.X - linearA.M22 * parentB.myInternalLinearVelocity.Y - linearA.M23 * parentB.myInternalLinearVelocity.Z +
                //           angularA.M21 * parentA.myInternalAngularVelocity.X + angularA.M22 * parentA.myInternalAngularVelocity.Y + angularA.M23 * parentA.myInternalAngularVelocity.Z +
                //           angularB.M21 * parentB.myInternalAngularVelocity.X + angularB.M22 * parentB.myInternalAngularVelocity.Y + angularB.M23 * parentB.myInternalAngularVelocity.Z;

                //Re-using information version:
                //TODO: va + wa x ra - vb - wb x rb, dotted against each axis, is it faster?
                Fix32 dvx = Fix32.Zero, dvy = Fix32.Zero, dvz = Fix32.Zero;
                if (entityA != null)
                {
                    dvx = entityA.linearVelocity.X .Add (entityA.angularVelocity.Y .Mul (ra.Z)) .Sub (entityA.angularVelocity.Z .Mul (ra.Y));
                    dvy = entityA.linearVelocity.Y .Add (entityA.angularVelocity.Z .Mul (ra.X)) .Sub (entityA.angularVelocity.X .Mul (ra.Z));
                    dvz = entityA.linearVelocity.Z .Add (entityA.angularVelocity.X .Mul (ra.Y)) .Sub (entityA.angularVelocity.Y .Mul (ra.X));
                }
                if (entityB != null)
                {
                    dvx = dvx .Add (entityB.linearVelocity.X.Neg() .Sub (entityB.angularVelocity.Y .Mul (rb.Z)) .Add (entityB.angularVelocity.Z .Mul (rb.Y)));
                    dvy = dvy .Add (entityB.linearVelocity.Y.Neg() .Sub (entityB.angularVelocity.Z .Mul (rb.X)) .Add (entityB.angularVelocity.X .Mul (rb.Z)));
                    dvz = dvz .Add (entityB.linearVelocity.Z.Neg() .Sub (entityB.angularVelocity.X .Mul (rb.Y)) .Add (entityB.angularVelocity.Y .Mul (rb.X)));
                }

                //Fix32 dvx = entityA.linearVelocity.X + (entityA.angularVelocity.Y * ra.Z) - (entityA.angularVelocity.Z * ra.Y)
                //            - entityB.linearVelocity.X - (entityB.angularVelocity.Y * rb.Z) + (entityB.angularVelocity.Z * rb.Y);

                //Fix32 dvy = entityA.linearVelocity.Y + (entityA.angularVelocity.Z * ra.X) - (entityA.angularVelocity.X * ra.Z)
                //            - entityB.linearVelocity.Y - (entityB.angularVelocity.Z * rb.X) + (entityB.angularVelocity.X * rb.Z);

                //Fix32 dvz = entityA.linearVelocity.Z + (entityA.angularVelocity.X * ra.Y) - (entityA.angularVelocity.Y * ra.X)
                //            - entityB.linearVelocity.Z - (entityB.angularVelocity.X * rb.Y) + (entityB.angularVelocity.Y * rb.X);

#if !WINDOWS
                Vector2 lambda = new Vector2();
#else
                Vector2 lambda;
#endif
                lambda.X = dvx .Mul (linearA.M11) .Add (dvy .Mul (linearA.M12)) .Add (dvz .Mul (linearA.M13));
                lambda.Y = dvx .Mul (linearA.M21) .Add (dvy .Mul (linearA.M22)) .Add (dvz .Mul (linearA.M23));
                return lambda;

                //Using XNA Cross product instead of inline
                //Vector3 wara, wbrb;
                //Vector3.Cross(ref parentA.myInternalAngularVelocity, ref Ra, out wara);
                //Vector3.Cross(ref parentB.myInternalAngularVelocity, ref Rb, out wbrb);

                //Fix32 dvx, dvy, dvz;
                //dvx = wara.X + parentA.myInternalLinearVelocity.X - wbrb.X - parentB.myInternalLinearVelocity.X;
                //dvy = wara.Y + parentA.myInternalLinearVelocity.Y - wbrb.Y - parentB.myInternalLinearVelocity.Y;
                //dvz = wara.Z + parentA.myInternalLinearVelocity.Z - wbrb.Z - parentB.myInternalLinearVelocity.Z;

                //lambda.X = dvx * linearA.M11 + dvy * linearA.M12 + dvz * linearA.M13;
                //lambda.Y = dvx * linearA.M21 + dvy * linearA.M22 + dvz * linearA.M23;
            }
        }


        ///<summary>
        /// Constructs a new sliding friction constraint.
        ///</summary>
        public SlidingFrictionTwoAxis()
        {
            isActive = false;
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override Fix32 SolveIteration()
        {

            Vector2 lambda = RelativeVelocity;

            //Convert to impulse
            //Matrix2x2.Transform(ref lambda, ref velocityToImpulse, out lambda);
            Fix32 x = lambda.X;
            lambda.X = x .Mul (velocityToImpulse.M11) .Add (lambda.Y .Mul (velocityToImpulse.M21));
            lambda.Y = x .Mul (velocityToImpulse.M12) .Add (lambda.Y .Mul (velocityToImpulse.M22));

            //Accumulate and clamp
            Vector2 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse.X = accumulatedImpulse.X .Add (lambda.X);
            accumulatedImpulse.Y = accumulatedImpulse.Y .Add (lambda.Y);
            Fix32 length = accumulatedImpulse.LengthSquared();
            Fix32 maximumFrictionForce = Fix32.Zero;
            for (int i = 0; i < contactCount; i++)
            {
                maximumFrictionForce = maximumFrictionForce .Add (contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse);
            }
            maximumFrictionForce = maximumFrictionForce .Mul (friction);
            if (length > maximumFrictionForce .Mul (maximumFrictionForce))
            {
                length = maximumFrictionForce .Div (length.Sqrt());
                accumulatedImpulse.X = accumulatedImpulse.X .Mul (length);
                accumulatedImpulse.Y = accumulatedImpulse.Y .Mul (length);
            }
            lambda.X = accumulatedImpulse.X .Sub (previousAccumulatedImpulse.X);
            lambda.Y = accumulatedImpulse.Y .Sub (previousAccumulatedImpulse.Y);
            //Single Axis clamp
            //Fix32 maximumFrictionForce = 0;
            //for (int i = 0; i < contactCount; i++)
            //{
            //    maximumFrictionForce = maximumFrictionForce + (pair.contacts[i].penetrationConstraint.accumulatedImpulse);
            //}
            //maximumFrictionForce *= friction;
            //Fix32 previousAccumulatedImpulse = accumulatedImpulse.X;
            //accumulatedImpulse.X = MathHelper.Clamp(accumulatedImpulse.X + lambda.X, -maximumFrictionForce, maximumFrictionForce);
            //lambda.X = accumulatedImpulse.X - previousAccumulatedImpulse;
            //previousAccumulatedImpulse = accumulatedImpulse.Y;
            //accumulatedImpulse.Y = MathHelper.Clamp(accumulatedImpulse.Y + lambda.Y, -maximumFrictionForce, maximumFrictionForce);
            //lambda.Y = accumulatedImpulse.Y - previousAccumulatedImpulse;

            //Apply impulse
#if !WINDOWS
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();
#else
            Vector3 linear, angular;
#endif
            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.X = lambda.X .Mul (linearA.M11) .Add (lambda.Y .Mul (linearA.M21));
            linear.Y = lambda.X .Mul (linearA.M12) .Add (lambda.Y .Mul (linearA.M22));
            linear.Z = lambda.X .Mul (linearA.M13) .Add (lambda.Y .Mul (linearA.M23));
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.X = lambda.X .Mul (angularA.M11) .Add (lambda.Y .Mul (angularA.M21));
                angular.Y = lambda.X .Mul (angularA.M12) .Add (lambda.Y .Mul (angularA.M22));
                angular.Z = lambda.X .Mul (angularA.M13) .Add (lambda.Y .Mul (angularA.M23));
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.X = lambda.X .Mul (angularB.M11) .Add (lambda.Y .Mul (angularB.M21));
                angular.Y = lambda.X .Mul (angularB.M12) .Add (lambda.Y .Mul (angularB.M22));
                angular.Z = lambda.X .Mul (angularB.M13) .Add (lambda.Y .Mul (angularB.M23));
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }


            return (lambda.X).Abs() .Add ((lambda.Y).Abs());
        }

        internal Vector3 manifoldCenter, relativeVelocity;

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(Fix32 dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            contactCount = contactManifoldConstraint.penetrationConstraints.Count;
            switch (contactCount)
            {
                case 1:
                    manifoldCenter = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position;
                    break;
                case 2:
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    manifoldCenter.X = manifoldCenter.X .Mul ((F64.C0p5));
                    manifoldCenter.Y = manifoldCenter.Y .Mul ((F64.C0p5));
                    manifoldCenter.Z = manifoldCenter.Z .Mul ((F64.C0p5));
                    break;
                case 3:
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.X = manifoldCenter.X .Mul ((F64.OneThird));
                    manifoldCenter.Y = manifoldCenter.Y .Mul ((F64.OneThird));
                    manifoldCenter.Z = manifoldCenter.Z .Mul ((F64.OneThird));
                    break;
                case 4:
                    //This isn't actually the center of the manifold.  Is it good enough?  Sure seems like it.
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[3].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.X = manifoldCenter.X .Mul ((F64.C0p25));
                    manifoldCenter.Y = manifoldCenter.Y .Mul ((F64.C0p25));
                    manifoldCenter.Z = manifoldCenter.Z .Mul ((F64.C0p25));
                    break;
                default:
                    manifoldCenter = Toolbox.NoVector;
                    break;
            }

            //Compute the three dimensional relative velocity at the point.


            Vector3 velocityA, velocityB;
            if (entityA != null)
            {
                Vector3.Subtract(ref manifoldCenter, ref entityA.position, out ra);
                Vector3.Cross(ref entityA.angularVelocity, ref ra, out velocityA);
                Vector3.Add(ref velocityA, ref entityA.linearVelocity, out velocityA);
            }
            else
                velocityA = new Vector3();
            if (entityB != null)
            {
                Vector3.Subtract(ref manifoldCenter, ref entityB.position, out rb);
                Vector3.Cross(ref entityB.angularVelocity, ref rb, out velocityB);
                Vector3.Add(ref velocityB, ref entityB.linearVelocity, out velocityB);
            }
            else
                velocityB = new Vector3();
            Vector3.Subtract(ref velocityA, ref velocityB, out relativeVelocity);

            //Get rid of the normal velocity.
            Vector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
            Fix32 normalVelocityScalar = normal.X .Mul (relativeVelocity.X) .Add (normal.Y .Mul (relativeVelocity.Y)) .Add (normal.Z .Mul (relativeVelocity.Z));
            relativeVelocity.X = relativeVelocity.X .Sub (normalVelocityScalar .Mul (normal.X));
            relativeVelocity.Y = relativeVelocity.Y .Sub (normalVelocityScalar .Mul (normal.Y));
            relativeVelocity.Z = relativeVelocity.Z .Sub (normalVelocityScalar .Mul (normal.Z));

            //Create the jacobian entry and decide the friction coefficient.
            Fix32 length = relativeVelocity.LengthSquared();
            if (length > Toolbox.Epsilon)
            {
                length = length.Sqrt();
                Fix32 inverseLength = F64.C1 .Div (length);
                linearA.M11 = relativeVelocity.X .Mul (inverseLength);
                linearA.M12 = relativeVelocity.Y .Mul (inverseLength);
                linearA.M13 = relativeVelocity.Z .Mul (inverseLength);


                friction = length > CollisionResponseSettings.StaticFrictionVelocityThreshold ?
                           contactManifoldConstraint.materialInteraction.KineticFriction :
                           contactManifoldConstraint.materialInteraction.StaticFriction;
            }
            else
            {
                friction = contactManifoldConstraint.materialInteraction.StaticFriction;

                //If there was no velocity, try using the previous frame's jacobian... if it exists.
                //Reusing an old one is okay since jacobians are cleared when a contact is initialized.
                if (!(linearA.M11 != Fix32.Zero || linearA.M12 != Fix32.Zero || linearA.M13 != Fix32.Zero))
                {
                    //Otherwise, just redo it all.
                    //Create arbitrary axes.
                    Vector3 axis1;
                    Vector3.Cross(ref normal, ref Toolbox.RightVector, out axis1);
                    length = axis1.LengthSquared();
                    if (length > Toolbox.Epsilon)
                    {
                        length = length.Sqrt();
                        Fix32 inverseLength = F64.C1 .Div (length);
                        linearA.M11 = axis1.X .Mul (inverseLength);
                        linearA.M12 = axis1.Y .Mul (inverseLength);
                        linearA.M13 = axis1.Z .Mul (inverseLength);
                    }
                    else
                    {
                        Vector3.Cross(ref normal, ref Toolbox.UpVector, out axis1);
                        axis1.Normalize();
                        linearA.M11 = axis1.X;
                        linearA.M12 = axis1.Y;
                        linearA.M13 = axis1.Z;
                    }
                }
            }

            //Second axis is first axis x normal
            linearA.M21 = (linearA.M12 .Mul (normal.Z)) .Sub (linearA.M13 .Mul (normal.Y));
            linearA.M22 = (linearA.M13 .Mul (normal.X)) .Sub (linearA.M11 .Mul (normal.Z));
            linearA.M23 = (linearA.M11 .Mul (normal.Y)) .Sub (linearA.M12 .Mul (normal.X));


            //Compute angular jacobians
            if (entityA != null)
            {
                //angularA 1 =  ra x linear axis 1
                angularA.M11 = (ra.Y .Mul (linearA.M13)) .Sub (ra.Z .Mul (linearA.M12));
                angularA.M12 = (ra.Z .Mul (linearA.M11)) .Sub (ra.X .Mul (linearA.M13));
                angularA.M13 = (ra.X .Mul (linearA.M12)) .Sub (ra.Y .Mul (linearA.M11));

                //angularA 2 =  ra x linear axis 2
                angularA.M21 = (ra.Y .Mul (linearA.M23)) .Sub (ra.Z .Mul (linearA.M22));
                angularA.M22 = (ra.Z .Mul (linearA.M21)) .Sub (ra.X .Mul (linearA.M23));
                angularA.M23 = (ra.X .Mul (linearA.M22)) .Sub (ra.Y .Mul (linearA.M21));
            }

            //angularB 1 =  linear axis 1 x rb
            if (entityB != null)
            {
                angularB.M11 = (linearA.M12 .Mul (rb.Z)) .Sub (linearA.M13 .Mul (rb.Y));
                angularB.M12 = (linearA.M13 .Mul (rb.X)) .Sub (linearA.M11 .Mul (rb.Z));
                angularB.M13 = (linearA.M11 .Mul (rb.Y)) .Sub (linearA.M12 .Mul (rb.X));

                //angularB 2 =  linear axis 2 x rb
                angularB.M21 = (linearA.M22 .Mul (rb.Z)) .Sub (linearA.M23 .Mul (rb.Y));
                angularB.M22 = (linearA.M23 .Mul (rb.X)) .Sub (linearA.M21 .Mul (rb.Z));
                angularB.M23 = (linearA.M21 .Mul (rb.Y)) .Sub (linearA.M22 .Mul (rb.X));
            }
            //Compute inverse effective mass matrix
            Matrix2x2 entryA, entryB;

            //these are the transformed coordinates
            Matrix2x3 transform;
            Matrix3x2 transpose;
            if (entityADynamic)
            {
                Matrix2x3.Multiply(ref angularA, ref entityA.inertiaTensorInverse, out transform);
                Matrix2x3.Transpose(ref angularA, out transpose);
                Matrix2x2.Multiply(ref transform, ref transpose, out entryA);
                entryA.M11 = entryA.M11 .Add (entityA.inverseMass);
                entryA.M22 = entryA.M22 .Add (entityA.inverseMass);
            }
            else
            {
                entryA = new Matrix2x2();
            }

            if (entityBDynamic)
            {
                Matrix2x3.Multiply(ref angularB, ref entityB.inertiaTensorInverse, out transform);
                Matrix2x3.Transpose(ref angularB, out transpose);
                Matrix2x2.Multiply(ref transform, ref transpose, out entryB);
                entryB.M11 = entryB.M11 .Add (entityB.inverseMass);
                entryB.M22 = entryB.M22 .Add (entityB.inverseMass);
            }
            else
            {
                entryB = new Matrix2x2();
            }

            velocityToImpulse.M11 = entryA.M11.Neg() .Sub (entryB.M11);
            velocityToImpulse.M12 = entryA.M12.Neg() .Sub (entryB.M12);
            velocityToImpulse.M21 = entryA.M21.Neg() .Sub (entryB.M21);
            velocityToImpulse.M22 = entryA.M22.Neg() .Sub (entryB.M22);
            Matrix2x2.Invert(ref velocityToImpulse, out velocityToImpulse);


        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {

            //Warm starting
#if !WINDOWS
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();
#else
            Vector3 linear, angular;
#endif
            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.X = accumulatedImpulse.X .Mul (linearA.M11) .Add (accumulatedImpulse.Y .Mul (linearA.M21));
            linear.Y = accumulatedImpulse.X .Mul (linearA.M12) .Add (accumulatedImpulse.Y .Mul (linearA.M22));
            linear.Z = accumulatedImpulse.X .Mul (linearA.M13) .Add (accumulatedImpulse.Y .Mul (linearA.M23));
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.X = accumulatedImpulse.X .Mul (angularA.M11) .Add (accumulatedImpulse.Y .Mul (angularA.M21));
                angular.Y = accumulatedImpulse.X .Mul (angularA.M12) .Add (accumulatedImpulse.Y .Mul (angularA.M22));
                angular.Z = accumulatedImpulse.X .Mul (angularA.M13) .Add (accumulatedImpulse.Y .Mul (angularA.M23));
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.X = accumulatedImpulse.X .Mul (angularB.M11) .Add (accumulatedImpulse.Y .Mul (angularB.M21));
                angular.Y = accumulatedImpulse.X .Mul (angularB.M12) .Add (accumulatedImpulse.Y .Mul (angularB.M22));
                angular.Z = accumulatedImpulse.X .Mul (angularB.M13) .Add (accumulatedImpulse.Y .Mul (angularB.M23));
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }
        }

        internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            isActive = true;

            linearA = new Matrix2x3();

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        internal void CleanUp()
        {
            accumulatedImpulse = new Vector2();
            contactManifoldConstraint = null;
            entityA = null;
            entityB = null;
            isActive = false;
        }

        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            //This should never really have to be called.
            if (entityA != null)
                outputInvolvedEntities.Add(entityA);
            if (entityB != null)
                outputInvolvedEntities.Add(entityB);
        }
    }
}