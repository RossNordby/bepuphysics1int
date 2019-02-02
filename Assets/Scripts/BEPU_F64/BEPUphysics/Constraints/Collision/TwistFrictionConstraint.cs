using System;
using BEPUphysics.Entities;
 
using BEPUphysics.Settings;
using BEPUutilities;
using BEPUutilities.DataStructures;


namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces necessary to slow down and stop twisting motion in a collision between two entities.
    /// </summary>
    public class TwistFrictionConstraint : SolverUpdateable
    {
        private readonly Fix32[] leverArms = new Fix32[4];
        private ConvexContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the contact manifold constraint that owns this constraint.
        ///</summary>
        public ConvexContactManifoldConstraint ContactManifoldConstraint { get { return contactManifoldConstraint; } }
        internal Fix32 accumulatedImpulse;
        private Fix32 angularX, angularY, angularZ;
        private int contactCount;
        private Fix32 friction;
        Entity entityA, entityB;
        bool entityADynamic, entityBDynamic;
        private Fix32 velocityToImpulse;

        ///<summary>
        /// Constructs a new twist friction constraint.
        ///</summary>
        public TwistFrictionConstraint()
        {
            isActive = false;
        }

        /// <summary>
        /// Gets the torque applied by twist friction.
        /// </summary>
        public Fix32 TotalTorque
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the angular velocity between the associated entities.
        ///</summary>
        public Fix32 RelativeVelocity
        {
            get
            {
                Fix32 lambda = Fix32.Zero;
                if (entityA != null)
                    lambda = entityA.angularVelocity.X .Mul (angularX) .Add (entityA.angularVelocity.Y .Mul (angularY)) .Add (entityA.angularVelocity.Z .Mul (angularZ));
                if (entityB != null)
                    lambda = lambda .Sub (entityB.angularVelocity.X .Mul (angularX) .Add (entityB.angularVelocity.Y .Mul (angularY)) .Add (entityB.angularVelocity.Z .Mul (angularZ)));
                return lambda;
            }
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override Fix32 SolveIteration()
        {
            //Compute relative velocity.  Collisions can occur between an entity and a non-entity.  If it's not an entity, assume it's not moving.
            Fix32 lambda = RelativeVelocity;
            
            lambda = lambda .Mul (velocityToImpulse); //convert to impulse

            //Clamp accumulated impulse
            Fix32 previousAccumulatedImpulse = accumulatedImpulse;
            Fix32 maximumFrictionForce = Fix32.Zero;
            for (int i = 0; i < contactCount; i++)
            {
                maximumFrictionForce = maximumFrictionForce .Add (leverArms[i] .Mul (contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse));
            }
            maximumFrictionForce = maximumFrictionForce .Mul (friction);
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse .Add (lambda), maximumFrictionForce.Neg(), maximumFrictionForce); //instead of maximumFrictionForce, could recompute each iteration...
            lambda = accumulatedImpulse .Sub (previousAccumulatedImpulse);


            //Apply the impulse
#if !WINDOWS
            Vector3 angular = new Vector3();
#else
            Vector3 angular;
#endif
            angular.X = lambda .Mul (angularX);
            angular.Y = lambda .Mul (angularY);
            angular.Z = lambda .Mul (angularZ);
            if (entityADynamic)
            {
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                angular.X = angular.X.Neg();
                angular.Y = angular.Y.Neg();
                angular.Z = angular.Z.Neg();
                entityB.ApplyAngularImpulse(ref angular);
            }


            return lambda.Abs();
        }


        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(Fix32 dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            //Compute the jacobian......  Real hard!
            Vector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
            angularX = normal.X;
            angularY = normal.Y;
            angularZ = normal.Z;

            //Compute inverse effective mass matrix
            Fix32 entryA, entryB;

            //these are the transformed coordinates
            Fix32 tX, tY, tZ;
            if (entityADynamic)
            {
                tX = angularX .Mul (entityA.inertiaTensorInverse.M11) .Add (angularY .Mul (entityA.inertiaTensorInverse.M21)) .Add (angularZ .Mul (entityA.inertiaTensorInverse.M31));
                tY = angularX .Mul (entityA.inertiaTensorInverse.M12) .Add (angularY .Mul (entityA.inertiaTensorInverse.M22)) .Add (angularZ .Mul (entityA.inertiaTensorInverse.M32));
                tZ = angularX .Mul (entityA.inertiaTensorInverse.M13) .Add (angularY .Mul (entityA.inertiaTensorInverse.M23)) .Add (angularZ .Mul (entityA.inertiaTensorInverse.M33));
                entryA = tX .Mul (angularX) .Add (tY .Mul (angularY)) .Add (tZ .Mul (angularZ)) .Add (entityA.inverseMass);
            }
            else
                entryA = Fix32.Zero;

            if (entityBDynamic)
            {
                tX = angularX .Mul (entityB.inertiaTensorInverse.M11) .Add (angularY .Mul (entityB.inertiaTensorInverse.M21)) .Add (angularZ .Mul (entityB.inertiaTensorInverse.M31));
                tY = angularX .Mul (entityB.inertiaTensorInverse.M12) .Add (angularY .Mul (entityB.inertiaTensorInverse.M22)) .Add (angularZ .Mul (entityB.inertiaTensorInverse.M32));
                tZ = angularX .Mul (entityB.inertiaTensorInverse.M13) .Add (angularY .Mul (entityB.inertiaTensorInverse.M23)) .Add (angularZ .Mul (entityB.inertiaTensorInverse.M33));
                entryB = tX .Mul (angularX) .Add (tY .Mul (angularY)) .Add (tZ .Mul (angularZ)) .Add (entityB.inverseMass);
            }
            else
                entryB = Fix32.Zero;

            velocityToImpulse = Fix32.MinusOne .Div (entryA .Add (entryB));


            //Compute the relative velocity to determine what kind of friction to use
            Fix32 relativeAngularVelocity = RelativeVelocity;
            //Set up friction and find maximum friction force
            Vector3 relativeSlidingVelocity = contactManifoldConstraint.SlidingFriction.relativeVelocity;
            friction = relativeAngularVelocity.Abs() > CollisionResponseSettings.StaticFrictionVelocityThreshold ||
					   (relativeSlidingVelocity.X).Abs() .Add ((relativeSlidingVelocity.Y).Abs()) .Add ((relativeSlidingVelocity.Z).Abs()) > CollisionResponseSettings.StaticFrictionVelocityThreshold
                           ? contactManifoldConstraint.materialInteraction.KineticFriction
                           : contactManifoldConstraint.materialInteraction.StaticFriction;
            friction = friction .Mul (CollisionResponseSettings.TwistFrictionFactor);

            contactCount = contactManifoldConstraint.penetrationConstraints.Count;

            Vector3 contactOffset;
            for (int i = 0; i < contactCount; i++)
            {
                Vector3.Subtract(ref contactManifoldConstraint.penetrationConstraints.Elements[i].contact.Position, ref contactManifoldConstraint.SlidingFriction.manifoldCenter, out contactOffset);
                leverArms[i] = contactOffset.Length();
            }



        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Apply the warmstarting impulse.
#if !WINDOWS
            Vector3 angular = new Vector3();
#else
            Vector3 angular;
#endif
            angular.X = accumulatedImpulse .Mul (angularX);
            angular.Y = accumulatedImpulse .Mul (angularY);
            angular.Z = accumulatedImpulse .Mul (angularZ);
            if (entityADynamic)
            {
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                angular.X = angular.X.Neg();
                angular.Y = angular.Y.Neg();
                angular.Z = angular.Z.Neg();
                entityB.ApplyAngularImpulse(ref angular);
            }
        }

        internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            isActive = true;

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        internal void CleanUp()
        {
            accumulatedImpulse = Fix32.Zero;
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