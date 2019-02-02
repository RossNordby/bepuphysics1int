using System;
using BEPUphysics.Entities;
using BEPUutilities;


namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Prevents the target entity from moving faster than the specified speeds.
    /// </summary>
    public class MaximumAngularSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
    {
        private Matrix3x3 effectiveMassMatrix;
        private Fix32 maxForceDt = Fix32.MaxValue;
        private Fix32 maxForceDtSquared = Fix32.MaxValue;
        private Vector3 accumulatedImpulse;
        private Fix32 maximumForce = Fix32.MaxValue;
        private Fix32 maximumSpeed;
        private Fix32 maximumSpeedSquared;

        private Fix32 softness = .00001m.ToFix32();
        private Fix32 usedSoftness;

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// Set its Entity and MaximumSpeed to complete the configuration.
        /// IsActive also starts as false with this constructor.
        /// </summary>
        public MaximumAngularSpeedConstraint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// </summary>
        /// <param name="e">Affected entity.</param>
        /// <param name="maxSpeed">Maximum angular speed allowed.</param>
        public MaximumAngularSpeedConstraint(Entity e, Fix32 maxSpeed)
        {
            Entity = e;
            MaximumSpeed = maxSpeed;
        }

        /// <summary>
        /// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
        /// This field can be used to simulate friction in a constraint.
        /// </summary>
        public Fix32 MaximumForce
        {
            get
            {
                if (maximumForce > Fix32.Zero)
                {
                    return maximumForce;
                }
                return Fix32.Zero;
            }
            set { maximumForce = value >= Fix32.Zero ? value : Fix32.Zero; }
        }

        /// <summary>
        /// Gets or sets the maximum angular speed that this constraint allows.
        /// </summary>
        public Fix32 MaximumSpeed
        {
            get { return maximumSpeed; }
            set
            {
                maximumSpeed = MathHelper.Max(Fix32.Zero, value);
                maximumSpeedSquared = maximumSpeed .Mul (maximumSpeed);
            }
        }


        /// <summary>
        /// Gets and sets the softness of this constraint.
        /// Higher values of softness allow the constraint to be violated more.
        /// Must be greater than zero.
        /// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
        /// For motors, softness can be used to implement damping.  For a damping constant k, the appropriate softness is 1/k.
        /// </summary>
        public Fix32 Softness
        {
            get { return softness; }
            set { softness = MathHelper.Max(Fix32.Zero, value); }
        }

        #region I3DImpulseConstraint Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        Vector3 I3DImpulseConstraint.RelativeVelocity
        {
            get { return entity.angularVelocity; }
        }

        /// <summary>
        /// Gets the total impulse applied by the constraint.
        /// </summary>
        public Vector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        #endregion

        /// <summary>
        /// Calculates and applies corrective impulses.
        /// Called automatically by space.
        /// </summary>
        public override Fix32 SolveIteration()
        {
            Fix32 angularSpeed = entity.angularVelocity.LengthSquared();
            if (angularSpeed > maximumSpeedSquared)
            {
                angularSpeed = angularSpeed.Sqrt();
                Vector3 impulse;
                //divide by angularSpeed to normalize the velocity.
                //Multiply by angularSpeed - maximumSpeed to get the 'velocity change vector.'
                Vector3.Multiply(ref entity.angularVelocity, (maximumSpeed .Sub (angularSpeed)) .Div (angularSpeed), out impulse);

                //incorporate softness
                Vector3 softnessImpulse;
                Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out softnessImpulse);
                Vector3.Subtract(ref impulse, ref softnessImpulse, out impulse);

                //Transform into impulse
                Matrix3x3.Transform(ref impulse, ref effectiveMassMatrix, out impulse);


                //Accumulate
                Vector3 previousAccumulatedImpulse = accumulatedImpulse;
                Vector3.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
                Fix32 forceMagnitude = accumulatedImpulse.LengthSquared();
                if (forceMagnitude > maxForceDtSquared)
                {
                    //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                    Fix32 multiplier = maxForceDt .Div (forceMagnitude.Sqrt());
                    accumulatedImpulse.X = accumulatedImpulse.X .Mul (multiplier);
                    accumulatedImpulse.Y = accumulatedImpulse.Y .Mul (multiplier);
					accumulatedImpulse.Z = accumulatedImpulse.Z .Mul (multiplier);

                    //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                    impulse.X = accumulatedImpulse.X .Sub (previousAccumulatedImpulse.X);
                    impulse.Y = accumulatedImpulse.Y .Sub (previousAccumulatedImpulse.Y);
                    impulse.Z = accumulatedImpulse.Z .Sub (previousAccumulatedImpulse.Z);
                }

                entity.ApplyAngularImpulse(ref impulse);


                return ((impulse.X).Abs() .Add (impulse.Y).Abs().Add (impulse.Z).Abs());
            }

            return Fix32.Zero;
        }

        /// <summary>
        /// Calculates necessary information for velocity solving.
        /// Called automatically by space.
        /// </summary>
        /// <param name="dt">Time in seconds since the last update.</param>
        public override void Update(Fix32 dt)
        {
            usedSoftness = softness .Div (dt);

            effectiveMassMatrix = entity.inertiaTensorInverse;

            effectiveMassMatrix.M11 = effectiveMassMatrix.M11 .Add (usedSoftness);
            effectiveMassMatrix.M22 = effectiveMassMatrix.M22 .Add (usedSoftness);
            effectiveMassMatrix.M33 = effectiveMassMatrix.M33 .Add (usedSoftness);

            Matrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);

            //Determine maximum force
            if (maximumForce < Fix32.MaxValue)
            {
                maxForceDt = maximumForce .Mul (dt);
                maxForceDtSquared = maxForceDt .Mul (maxForceDt);
            }
            else
            {
                maxForceDt = Fix32.MaxValue;
                maxForceDtSquared = Fix32.MaxValue;
            }

        }


        public override void ExclusiveUpdate()
        {

            //Can't do warmstarting due to the strangeness of this constraint (not based on a position error, nor is it really a motor).
            accumulatedImpulse = Toolbox.ZeroVector;
        }
    }
}