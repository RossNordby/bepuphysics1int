using BEPUphysics.Entities;
using BEPUphysics.CollisionTests;
using BEPUphysics.Settings;
using BEPUutilities;
using System;
using BEPUutilities.DataStructures;


namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces necessary to keep two entities from going through each other at a contact point.
    /// </summary>
    public class ContactPenetrationConstraint : SolverUpdateable
    {
        internal Contact contact;

        ///<summary>
        /// Gets the contact associated with this penetration constraint.
        ///</summary>
        public Contact Contact { get { return contact; } }
        internal Fix32 accumulatedImpulse;
        //Fix32 linearBX, linearBY, linearBZ;
        internal Fix32 angularAX, angularAY, angularAZ;
        internal Fix32 angularBX, angularBY, angularBZ;

        private Fix32 softness;
        private Fix32 bias;
        private Fix32 linearAX, linearAY, linearAZ;
        private Entity entityA, entityB;
        private bool entityADynamic, entityBDynamic;
        //Inverse effective mass matrix
        internal Fix32 velocityToImpulse;
        private ContactManifoldConstraint contactManifoldConstraint;

        internal Vector3 ra, rb;

        ///<summary>
        /// Constructs a new penetration constraint.
        ///</summary>
        public ContactPenetrationConstraint()
        {
            isActive = false;
        }


        ///<summary>
        /// Configures the penetration constraint.
        ///</summary>
        ///<param name="contactManifoldConstraint">Owning manifold constraint.</param>
        ///<param name="contact">Contact associated with the penetration constraint.</param>
        public void Setup(ContactManifoldConstraint contactManifoldConstraint, Contact contact)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            this.contact = contact;
            isActive = true;

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;

        }

        ///<summary>
        /// Cleans up the constraint.
        ///</summary>
        public void CleanUp()
        {
            accumulatedImpulse = F64.C0;
            contactManifoldConstraint = null;
            contact = null;
            entityA = null;
            entityB = null;
            isActive = false;


        }

        /// <summary>
        /// Gets the total normal impulse applied by this penetration constraint to maintain the separation of the involved entities.
        /// </summary>
        public Fix32 NormalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the relative velocity between the associated entities at the contact point along the contact normal.
        ///</summary>
        public Fix32 RelativeVelocity
        {
            get
            {
                Fix32 lambda = F64.C0;
                if (entityA != null)
                {
                    lambda = (((((entityA.linearVelocity.X.Mul(linearAX)).Add(entityA.linearVelocity.Y.Mul(linearAY))).Add(entityA.linearVelocity.Z.Mul(linearAZ))).Add(entityA.angularVelocity.X.Mul(angularAX))).Add(entityA.angularVelocity.Y.Mul(angularAY))).Add(entityA.angularVelocity.Z.Mul(angularAZ));
                }
                if (entityB != null)
                {
					lambda = lambda.Add((((((entityB.linearVelocity.X.Neg().Mul(linearAX)).Sub(entityB.linearVelocity.Y.Mul(linearAY))).Sub(entityB.linearVelocity.Z.Mul(linearAZ))).Add(entityB.angularVelocity.X.Mul(angularBX))).Add(entityB.angularVelocity.Y.Mul(angularBY))).Add(entityB.angularVelocity.Z.Mul(angularBZ)));
                }
                return lambda;
            }
        }




        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(Fix32 dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            //Set up the jacobians.
            linearAX = contact.Normal.X.Neg();
            linearAY = contact.Normal.Y.Neg();
            linearAZ = contact.Normal.Z.Neg();
            //linearBX = -linearAX;
            //linearBY = -linearAY;
            //linearBZ = -linearAZ;



            //angular A = Ra x N
            if (entityA != null)
            {
                Vector3.Subtract(ref contact.Position, ref entityA.position, out ra);
                angularAX = (ra.Y.Mul(linearAZ)).Sub((ra.Z.Mul(linearAY)));
                angularAY = (ra.Z.Mul(linearAX)).Sub((ra.X.Mul(linearAZ)));
                angularAZ = (ra.X.Mul(linearAY)).Sub((ra.Y.Mul(linearAX)));
            }


            //Angular B = N x Rb
            if (entityB != null)
            {
                Vector3.Subtract(ref contact.Position, ref entityB.position, out rb);
                angularBX = (linearAY.Mul(rb.Z)).Sub((linearAZ.Mul(rb.Y)));
                angularBY = (linearAZ.Mul(rb.X)).Sub((linearAX.Mul(rb.Z)));
                angularBZ = (linearAX.Mul(rb.Y)).Sub((linearAY.Mul(rb.X)));
            }


            //Compute inverse effective mass matrix
            Fix32 entryA, entryB;

            //these are the transformed coordinates
            Fix32 tX, tY, tZ;
            if (entityADynamic)
            {
                tX = ((angularAX.Mul(entityA.inertiaTensorInverse.M11)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M21))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M31));
                tY = ((angularAX.Mul(entityA.inertiaTensorInverse.M12)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M22))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M32));
                tZ = ((angularAX.Mul(entityA.inertiaTensorInverse.M13)).Add(angularAY.Mul(entityA.inertiaTensorInverse.M23))).Add(angularAZ.Mul(entityA.inertiaTensorInverse.M33));
                entryA = (((tX.Mul(angularAX)).Add(tY.Mul(angularAY))).Add(tZ.Mul(angularAZ))).Add(entityA.inverseMass);
            }
            else
                entryA = F64.C0;

            if (entityBDynamic)
            {
                tX = ((angularBX.Mul(entityB.inertiaTensorInverse.M11)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M21))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M31));
                tY = ((angularBX.Mul(entityB.inertiaTensorInverse.M12)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M22))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M32));
                tZ = ((angularBX.Mul(entityB.inertiaTensorInverse.M13)).Add(angularBY.Mul(entityB.inertiaTensorInverse.M23))).Add(angularBZ.Mul(entityB.inertiaTensorInverse.M33));
                entryB = (((tX.Mul(angularBX)).Add(tY.Mul(angularBY))).Add(tZ.Mul(angularBZ))).Add(entityB.inverseMass);
            }
            else
                entryB = F64.C0;

            //If we used a single fixed softness value, then heavier objects will tend to 'squish' more than light objects.
            //In the extreme case, very heavy objects could simply fall through the ground by force of gravity.
            //To see why this is the case, consider that a given dt, softness, and bias factor correspond to an equivalent spring's damping and stiffness coefficients.
            //Imagine trying to hang objects of different masses on the fixed-strength spring: obviously, heavier ones will pull it further down.

            //To counteract this, scale the softness value based on the effective mass felt by the constraint.
            //Larger effective masses should correspond to smaller softnesses so that the spring has the same positional behavior.
            //Fortunately, we're already computing the necessary values: the raw, unsoftened effective mass inverse shall be used to compute the softness.

            Fix32 effectiveMassInverse = entryA.Add(entryB);
            softness = (CollisionResponseSettings.Softness.Mul(effectiveMassInverse)).Div(dt);
            velocityToImpulse = (F64.C1.Neg()).Div((softness.Add(effectiveMassInverse)));


            //Bounciness and bias (penetration correction)
            if (contact.PenetrationDepth >= F64.C0)
            {
                bias = MathHelper.Min(
(MathHelper.Max(F64.C0, contact.PenetrationDepth.Sub(CollisionDetectionSettings.AllowedPenetration)).Mul(CollisionResponseSettings.PenetrationRecoveryStiffness)).Div(dt),
                    CollisionResponseSettings.MaximumPenetrationRecoverySpeed);

                if (contactManifoldConstraint.materialInteraction.Bounciness > F64.C0)
                {
                    //Target a velocity which includes a portion of the incident velocity.
                    Fix32 bounceVelocity = RelativeVelocity.Neg();
                    if (bounceVelocity > F64.C0)
                    {
                        var lowThreshold = CollisionResponseSettings.BouncinessVelocityThreshold.Mul(F64.C0p3);
                        var velocityFraction = MathHelper.Clamp((bounceVelocity.Sub(lowThreshold)).Div(((CollisionResponseSettings.BouncinessVelocityThreshold.Sub(lowThreshold)).Add(Toolbox.Epsilon))), F64.C0, F64.C1);
                        var bouncinessVelocity = (velocityFraction.Mul(bounceVelocity)).Mul(contactManifoldConstraint.materialInteraction.Bounciness);
                        bias = MathHelper.Max(bouncinessVelocity, bias);
                    }
                }
            }
            else
            {
                //The contact is actually separated right now.  Allow the solver to target a position that is just barely in collision.
                //If the solver finds that an accumulated negative impulse is required to hit this target, then no work will be done.
                bias = contact.PenetrationDepth.Div(dt);

                //This implementation is going to ignore bounciness for now.
                //Since it's not being used for CCD, these negative-depth contacts
                //only really occur in situations where no bounce should occur.

                //if (contactManifoldConstraint.materialInteraction.Bounciness > 0)
                //{
                //    //Target a velocity which includes a portion of the incident velocity.
                //    //The contact isn't colliding currently, but go ahead and target the post-bounce velocity.
                //    //The bias is added to the bounce velocity to simulate the object continuing to the surface and then bouncing off.
                //    Fix32 relativeVelocity = -RelativeVelocity;
                //    if (relativeVelocity > CollisionResponseSettings.BouncinessVelocityThreshold)
                //        bias = relativeVelocity * contactManifoldConstraint.materialInteraction.Bounciness + bias;
                //}
            }


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
            if (entityADynamic)
            {
                angular.X = accumulatedImpulse.Mul(angularAX);
                angular.Y = accumulatedImpulse.Mul(angularAY);
                angular.Z = accumulatedImpulse.Mul(angularAZ);
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
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


        /// <summary>
        /// Computes and applies an impulse to keep the colliders from penetrating.
        /// </summary>
        /// <returns>Impulse applied.</returns>
        public override Fix32 SolveIteration()
        {

            //Compute relative velocity
            Fix32 lambda = ((RelativeVelocity.Sub(bias)).Add(softness.Mul(accumulatedImpulse))).Mul(velocityToImpulse);

            //Clamp accumulated impulse
            Fix32 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Max(F64.C0, accumulatedImpulse.Add(lambda));
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
            if (entityADynamic)
            {
                angular.X = lambda.Mul(angularAX);
                angular.Y = lambda.Mul(angularAY);
                angular.Z = lambda.Mul(angularAZ);
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
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