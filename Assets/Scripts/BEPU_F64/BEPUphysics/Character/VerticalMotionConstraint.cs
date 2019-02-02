using System;
using System.Diagnostics;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Settings;


namespace BEPUphysics.Character
{
    /// <summary>
    /// Keeps a character glued to the ground, if possible.
    /// </summary>
    public class VerticalMotionConstraint : SolverUpdateable
    {
        Entity characterBody;
        private SupportFinder supportFinder;

        SupportData supportData;

        private Fix32 maximumGlueForce;
        /// <summary>
        /// Gets or sets the maximum force that the constraint will apply in attempting to keep the character stuck to the ground.
        /// </summary>
        public Fix32 MaximumGlueForce
        {
            get
            {
                return maximumGlueForce;
            }
            set
            {
                if (maximumGlueForce < Fix32.Zero)
                    throw new ArgumentException("Value must be nonnegative.");
                maximumGlueForce = value;
            }
        }
        Fix32 maximumForce;

        Fix32 supportForceFactor = F64.C1;
        /// <summary>
        /// Gets or sets the scaling factor of forces applied to the supporting object if it is a dynamic entity.
        /// Low values (below 1) reduce the amount of motion imparted to the support object; it acts 'heavier' as far as vertical motion is concerned.
        /// High values (above 1) increase the force applied to support objects, making them appear lighter.
        /// </summary>
        public Fix32 SupportForceFactor
        {
            get
            {
                return supportForceFactor;
            }
            set
            {
                if (value < Fix32.Zero)
                    throw new ArgumentException("Value must be nonnegative.");
                supportForceFactor = value;
            }
        }


        /// <summary>
        /// Gets the effective mass felt by the constraint.
        /// </summary>
        public Fix32 EffectiveMass
        {
            get
            {
                return effectiveMass;
            }
        }
        Fix32 effectiveMass;
        Entity supportEntity;
        Vector3 linearJacobianA;
        Vector3 linearJacobianB;
        Vector3 angularJacobianB;

   
        Fix32 accumulatedImpulse;
        Fix32 permittedVelocity;

		/// <summary>
		/// Constructs a new vertical motion constraint.
		/// </summary>
		/// <param name="characterBody">Character body governed by the constraint.</param>
		/// <param name="supportFinder">Support finder used by the character.</param>
		/// <param name="maximumGlueForce">Maximum force the vertical motion constraint is allowed to apply in an attempt to keep the character on the ground.</param>
		public VerticalMotionConstraint(Entity characterBody, SupportFinder supportFinder)
		{
			this.characterBody = characterBody;
			this.supportFinder = supportFinder;
			MaximumGlueForce = 5000.ToFix32();
		}

		/// <summary>
		/// Constructs a new vertical motion constraint.
		/// </summary>
		/// <param name="characterBody">Character body governed by the constraint.</param>
		/// <param name="supportFinder">Support finder used by the character.</param>
		/// <param name="maximumGlueForce">Maximum force the vertical motion constraint is allowed to apply in an attempt to keep the character on the ground.</param>
		public VerticalMotionConstraint(Entity characterBody, SupportFinder supportFinder, Fix32 maximumGlueForce)
        {
            this.characterBody = characterBody;
            this.supportFinder = supportFinder;
            MaximumGlueForce = maximumGlueForce;
        }

        /// <summary>
        /// Updates the movement basis of the horizontal motion constraint and updates the horizontal motion constraint's support data.
        /// Should be updated automatically by the character on each time step; other code should not need to call this.
        /// </summary>
        public void UpdateSupportData()
        {
            //Check if the support has changed, and perform the necessary bookkeeping to keep the connections up to date.
            var oldSupport = supportData.SupportObject;
            supportData = supportFinder.VerticalSupportData;
            if (oldSupport != supportData.SupportObject)
            {
                OnInvolvedEntitiesChanged();
            }

        }

        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            var entityCollidable = supportData.SupportObject as EntityCollidable;
            if (entityCollidable != null)
                outputInvolvedEntities.Add(entityCollidable.Entity);
            outputInvolvedEntities.Add(characterBody);

        }


        /// <summary>
        /// Updates the activity state of the constraint.
        /// Called automatically by the solver.
        /// </summary>
        public override void UpdateSolverActivity()
        {
            if (supportFinder.HasTraction)
                base.UpdateSolverActivity();
            else
                isActiveInSolver = false;
        }

        /// <summary>
        /// Performs any per-frame computation needed by the constraint.
        /// </summary>
        /// <param name="dt">Time step duration.</param>
        public override void Update(Fix32 dt)
        {
            //Collect references, pick the mode, and configure the coefficients to be used by the solver.
            if (supportData.SupportObject != null)
            {
                //Get an easy reference to the support.
                var support = supportData.SupportObject as EntityCollidable;
                if (support != null)
                {
                    supportEntity = support.Entity;

                }
                else
                {
                    supportEntity = null;

                }

            }
            else
            {
                supportEntity = null;
            }

            maximumForce = maximumGlueForce .Mul (dt);

            //If we don't allow the character to get out of the ground, it could apply some significant forces to a dynamic support object.
            //Let the character escape penetration in a controlled manner. This mirrors the regular penetration recovery speed.
            //Since the vertical motion constraint works in the opposite direction of the contact penetration constraint,
            //this actually eliminates the 'bounce' that can occur with non-character objects in deep penetration.
            permittedVelocity = MathHelper.Min(MathHelper.Max(supportData.Depth .Mul (CollisionResponseSettings.PenetrationRecoveryStiffness) .Div (dt), Fix32.Zero), CollisionResponseSettings.MaximumPenetrationRecoverySpeed);

            //Compute the jacobians and effective mass matrix.  This constraint works along a single degree of freedom, so the mass matrix boils down to a scalar.

            linearJacobianA = supportData.Normal;
            Vector3.Negate(ref linearJacobianA, out linearJacobianB);
            Fix32 inverseEffectiveMass = characterBody.InverseMass;
            if (supportEntity != null)
            {
                Vector3 offsetB = supportData.Position - supportEntity.Position;
                Vector3.Cross(ref offsetB, ref linearJacobianB, out angularJacobianB);
                if (supportEntity.IsDynamic)
                {
                    //Only dynamic entities can actually contribute anything to the effective mass.
                    //Kinematic entities have infinite mass and inertia, so this would all zero out.
                    Matrix3x3 inertiaInverse = supportEntity.InertiaTensorInverse;
                    Vector3 angularComponentB;
                    Matrix3x3.Transform(ref angularJacobianB, ref inertiaInverse, out angularComponentB);
                    Fix32 effectiveMassContribution;
                    Vector3.Dot(ref angularComponentB, ref angularJacobianB, out effectiveMassContribution);

                    inverseEffectiveMass = inverseEffectiveMass .Add (supportForceFactor .Mul (effectiveMassContribution .Add (supportEntity.InverseMass)));
                }
            }
            effectiveMass = F64.C1 .Div (inverseEffectiveMass);
            //So much nicer and shorter than the horizontal constraint!

        }

        /// <summary>
        /// Performs any per-frame computations needed by the constraint that require exclusive access to the involved entities.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Warm start the constraint using the previous impulses and the new jacobians!
#if !WINDOWS
            Vector3 impulse = new Vector3();
            Vector3 torque= new Vector3();
#else
            Vector3 impulse;
            Vector3 torque;
#endif
            Vector3.Multiply(ref linearJacobianA, accumulatedImpulse, out impulse);

            characterBody.ApplyLinearImpulse(ref impulse);

            if (supportEntity != null && supportEntity.IsDynamic)
            {
                Vector3.Multiply(ref impulse, supportForceFactor.Neg(), out impulse);
                Vector3.Multiply(ref angularJacobianB, accumulatedImpulse .Mul (supportForceFactor), out torque);

                supportEntity.ApplyLinearImpulse(ref impulse);
                supportEntity.ApplyAngularImpulse(ref torque);
            }
        }

        /// <summary>
        /// Computes a solution to the constraint.
        /// </summary>
        /// <returns>Magnitude of the applied impulse.</returns>
        public override Fix32 SolveIteration()
        {
            //The relative velocity's x component is in the movement direction.
            //y is the perpendicular direction.

            //Note that positive velocity is penetrating velocity.
            Fix32 relativeVelocity = RelativeVelocity .Add (permittedVelocity);


            //Create the full velocity change, and convert it to an impulse in constraint space.
            Fix32 lambda = relativeVelocity.Neg() .Mul (effectiveMass);

            //Add and clamp the impulse.
            Fix32 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse .Add (lambda), Fix32.Zero, maximumForce);
            lambda = accumulatedImpulse .Sub (previousAccumulatedImpulse);
            //Use the jacobians to put the impulse into world space.

#if !WINDOWS
            Vector3 impulse = new Vector3();
            Vector3 torque= new Vector3();
#else
            Vector3 impulse;
            Vector3 torque;
#endif
            Vector3.Multiply(ref linearJacobianA, lambda, out impulse);

            characterBody.ApplyLinearImpulse(ref impulse);

            if (supportEntity != null && supportEntity.IsDynamic)
            {
                Vector3.Multiply(ref impulse, supportForceFactor.Neg(), out impulse);

                Vector3.Multiply(ref angularJacobianB, lambda .Mul (supportForceFactor), out torque);

                supportEntity.ApplyLinearImpulse(ref impulse);
                supportEntity.ApplyAngularImpulse(ref torque);
            }
            return lambda.Abs();


        }

        /// <summary>
        /// Gets the relative velocity between the character and its support along the support normal.
        /// </summary>
        public Fix32 RelativeVelocity
        {
            get
            {
                Fix32 relativeVelocity;

                Vector3.Dot(ref linearJacobianA, ref characterBody.linearVelocity, out relativeVelocity);

                if (supportEntity != null)
                {
                    Fix32 supportVelocity;
                    Vector3.Dot(ref linearJacobianB, ref supportEntity.linearVelocity, out supportVelocity);
                    relativeVelocity = relativeVelocity .Add (supportVelocity);
                    Vector3.Dot(ref angularJacobianB, ref supportEntity.angularVelocity, out supportVelocity);
                    relativeVelocity = relativeVelocity .Add (supportVelocity);

                }
                return relativeVelocity;
            }
        }


    }
}
