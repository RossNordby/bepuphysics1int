using System;
using BEPUphysics.Entities;
 
using BEPUutilities.DataStructures;
using BEPUphysics.Settings;
using BEPUutilities;


namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the friction force for a contact when central friction cannot be used.
    /// </summary>
    public class ContactFrictionConstraint : SolverUpdateable
    {
        private ContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the manifold constraint associated with this friction constraint.
        ///</summary>
        public ContactManifoldConstraint ContactManifoldConstraint
        {
            get
            {
                return contactManifoldConstraint;
            }
        }
        private ContactPenetrationConstraint penetrationConstraint;
        ///<summary>
        /// Gets the penetration constraint associated with this friction constraint.
        ///</summary>
        public ContactPenetrationConstraint PenetrationConstraint
        {
            get
            {
                return penetrationConstraint;
            }
        }

        ///<summary>
        /// Constructs a new friction constraint.
        ///</summary>
        public ContactFrictionConstraint()
        {
            isActive = false;
        }

        internal Fix accumulatedImpulse;
        //Fix32 linearBX, linearBY, linearBZ;
        private Fix angularAX, angularAY, angularAZ;
        private Fix angularBX, angularBY, angularBZ;

        //Inverse effective mass matrix


        private Fix friction;
        internal Fix linearAX, linearAY, linearAZ;
        private Entity entityA, entityB;
        private bool entityAIsDynamic, entityBIsDynamic;
        private Fix velocityToImpulse;


        ///<summary>
        /// Configures the friction constraint for a new contact.
        ///</summary>
        ///<param name="contactManifoldConstraint">Manifold to which the constraint belongs.</param>
        ///<param name="penetrationConstraint">Penetration constraint associated with this friction constraint.</param>
        public void Setup(ContactManifoldConstraint contactManifoldConstraint, ContactPenetrationConstraint penetrationConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            this.penetrationConstraint = penetrationConstraint;
            IsActive = true;
            linearAX = F64.C0;
            linearAY = F64.C0;
            linearAZ = F64.C0;

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        ///<summary>
        /// Cleans upt he friction constraint.
        ///</summary>
        public void CleanUp()
        {
            accumulatedImpulse = F64.C0;
            contactManifoldConstraint = null;
            penetrationConstraint = null;
            entityA = null;
            entityB = null;
            IsActive = false;
        }

        /// <summary>
        /// Gets the direction in which the friction force acts.
        /// </summary>
        public Vector3 FrictionDirection
        {
            get { return new Vector3(linearAX, linearAY, linearAZ); }
        }

        /// <summary>
        /// Gets the total impulse applied by this friction constraint in the last time step.
        /// </summary>
        public Fix TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the relative velocity of the constraint.  This is the velocity along the tangent movement direction.
        ///</summary>
        public Fix RelativeVelocity
        {
            get
            {
                Fix velocity = F64.C0;
                if (entityA != null)
					velocity = velocity.Add((((((entityA.linearVelocity.X.Mul(linearAX)).Add(entityA.linearVelocity.Y.Mul(linearAY))).Add(entityA.linearVelocity.Z.Mul(linearAZ))).Add(entityA.angularVelocity.X.Mul(angularAX))).Add(entityA.angularVelocity.Y.Mul(angularAY))).Add(entityA.angularVelocity.Z.Mul(angularAZ)));
                if (entityB != null)
					velocity = velocity.Add((((((entityB.linearVelocity.X.Neg().Mul(linearAX)).Sub(entityB.linearVelocity.Y.Mul(linearAY))).Sub(entityB.linearVelocity.Z.Mul(linearAZ))).Add(entityB.angularVelocity.X.Mul(angularBX))).Add(entityB.angularVelocity.Y.Mul(angularBY))).Add(entityB.angularVelocity.Z.Mul(angularBZ)));
                return velocity;
            }
        }


        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override Fix SolveIteration()
        {
            //Compute relative velocity and convert to impulse
            Fix lambda = RelativeVelocity.Mul(velocityToImpulse);


            //Clamp accumulated impulse
            Fix previousAccumulatedImpulse = accumulatedImpulse;
            Fix maxForce = friction.Mul(penetrationConstraint.accumulatedImpulse);
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse.Add(lambda), maxForce.Neg(), maxForce);
            lambda = accumulatedImpulse.Sub(previousAccumulatedImpulse);

            //Apply the impulse
#if !WINDOWS
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();
#else
            Vector3 linear, angular;
#endif
            linear.X = lambda.Mul(linearAX);
            linear.Y = lambda.Mul(linearAY);
            linear.Z = lambda.Mul(linearAZ);
            if (entityAIsDynamic)
            {
                angular.X = lambda.Mul(angularAX);
                angular.Y = lambda.Mul(angularAY);
                angular.Z = lambda.Mul(angularAZ);
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBIsDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                angular.X = lambda.Mul(angularBX);
                angular.Y = lambda.Mul(angularBY);
                angular.Z = lambda.Mul(angularBZ);
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }

            return Fix32Ext.Abs(lambda);
        }

        /// <summary>
        /// Initializes the constraint for this frame.
        /// </summary>
        /// <param name="dt">Time since the last frame.</param>
        public override void Update(Fix dt)
        {


            entityAIsDynamic = entityA != null && entityA.isDynamic;
            entityBIsDynamic = entityB != null && entityB.isDynamic;

            //Compute the three dimensional relative velocity at the point.

            Vector3 velocityA = new Vector3(), velocityB = new Vector3();
            Vector3 ra = penetrationConstraint.ra, rb = penetrationConstraint.rb;
            if (entityA != null)
            {
                Vector3.Cross(ref entityA.angularVelocity, ref ra, out velocityA);
                Vector3.Add(ref velocityA, ref entityA.linearVelocity, out velocityA);
            }
            if (entityB != null)
            {
                Vector3.Cross(ref entityB.angularVelocity, ref rb, out velocityB);
                Vector3.Add(ref velocityB, ref entityB.linearVelocity, out velocityB);
            }
            Vector3 relativeVelocity;
            Vector3.Subtract(ref velocityA, ref velocityB, out relativeVelocity);

            //Get rid of the normal velocity.
            Vector3 normal = penetrationConstraint.contact.Normal;
            Fix normalVelocityScalar = ((normal.X.Mul(relativeVelocity.X)).Add(normal.Y.Mul(relativeVelocity.Y))).Add(normal.Z.Mul(relativeVelocity.Z));
			relativeVelocity.X = relativeVelocity.X.Sub(normalVelocityScalar.Mul(normal.X));
			relativeVelocity.Y = relativeVelocity.Y.Sub(normalVelocityScalar.Mul(normal.Y));
			relativeVelocity.Z = relativeVelocity.Z.Sub(normalVelocityScalar.Mul(normal.Z));

            //Create the jacobian entry and decide the friction coefficient.
            Fix length = relativeVelocity.LengthSquared();
            if (length > Toolbox.Epsilon)
            {
                length = Fix32Ext.Sqrt(length);
                linearAX = relativeVelocity.X.Div(length);
                linearAY = relativeVelocity.Y.Div(length);
                linearAZ = relativeVelocity.Z.Div(length);

                friction = length > CollisionResponseSettings.StaticFrictionVelocityThreshold
                               ? contactManifoldConstraint.materialInteraction.KineticFriction
                               : contactManifoldConstraint.materialInteraction.StaticFriction;
            }
            else
            {
                //If there's no velocity, there's no jacobian.  Give up.
                //This is 'fast' in that it will early out on essentially resting objects,
                //but it may introduce instability.
                //If it doesn't look good, try the next approach.
                //isActive = false;
                //return;

                //if the above doesn't work well, try using the previous frame's jacobian.
                if (linearAX != F64.C0 || linearAY != F64.C0 || linearAZ != F64.C0)
                {
                    friction = contactManifoldConstraint.materialInteraction.StaticFriction;
                }
                else
                {
                    //Can't really do anything here, give up.
                    isActiveInSolver = false;
                    return;
                    //Could also cross the up with normal to get a random direction.  Questionable value.
                }
            }


            //angular A = Ra x N
            angularAX = (ra.Y.Mul(linearAZ)).Sub((ra.Z.Mul(linearAY)));
            angularAY = (ra.Z.Mul(linearAX)).Sub((ra.X.Mul(linearAZ)));
            angularAZ = (ra.X.Mul(linearAY)).Sub((ra.Y.Mul(linearAX)));

            //Angular B = N x Rb
            angularBX = (linearAY.Mul(rb.Z)).Sub((linearAZ.Mul(rb.Y)));
            angularBY = (linearAZ.Mul(rb.X)).Sub((linearAX.Mul(rb.Z)));
            angularBZ = (linearAX.Mul(rb.Y)).Sub((linearAY.Mul(rb.X)));

            //Compute inverse effective mass matrix
            Fix entryA, entryB;

            //these are the transformed coordinates
            Fix tX, tY, tZ;
            if (entityAIsDynamic)
            {
                tX = ((angularAX.Mul(entityA.inertiaTensorInverse.M11)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M21))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M31));
                tY = ((angularAX.Mul(entityA.inertiaTensorInverse.M12)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M22))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M32));
                tZ = ((angularAX.Mul(entityA.inertiaTensorInverse.M13)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M23))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M33));
                entryA = (((tX.Mul(angularAX)).Add(tY.Mul(angularAY))).Add(tZ.Mul(angularAZ))).Add(entityA.inverseMass);
            }
            else
                entryA = F64.C0;

            if (entityBIsDynamic)
            {
                tX = ((angularBX.Mul(entityB.inertiaTensorInverse.M11)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M21))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M31));
                tY = ((angularBX.Mul(entityB.inertiaTensorInverse.M12)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M22))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M32));
                tZ = ((angularBX.Mul(entityB.inertiaTensorInverse.M13)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M23))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M33));
                entryB = (((tX.Mul(angularBX)).Add(tY.Mul(angularBY))).Add(tZ.Mul(angularBZ))).Add(entityB.inverseMass);
            }
            else
                entryB = F64.C0;

            velocityToImpulse = (F64.C1.Neg()).Div((entryA.Add(entryB))); //Softness?



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
            linear.X = accumulatedImpulse.Mul(linearAX);
            linear.Y = accumulatedImpulse.Mul(linearAY);
            linear.Z = accumulatedImpulse.Mul(linearAZ);
            if (entityAIsDynamic)
            {
                angular.X = accumulatedImpulse.Mul(angularAX);
                angular.Y = accumulatedImpulse.Mul(angularAY);
                angular.Z = accumulatedImpulse.Mul(angularAZ);
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBIsDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                angular.X = accumulatedImpulse.Mul(angularBX);
                angular.Y = accumulatedImpulse.Mul(angularBY);
                angular.Z = accumulatedImpulse.Mul(angularBZ);
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }
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