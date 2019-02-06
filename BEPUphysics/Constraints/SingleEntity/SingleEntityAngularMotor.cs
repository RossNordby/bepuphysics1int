using System;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUutilities;


namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Constraint which attempts to restrict the relative angular velocity of two entities to some value.
    /// Can use a target relative orientation to apply additional force.
    /// </summary>
    public class SingleEntityAngularMotor : SingleEntityConstraint, I3DImpulseConstraintWithError
    {
        private readonly JointBasis3D basis = new JointBasis3D();

        private readonly MotorSettingsOrientation settings;
        private Vector3 accumulatedImpulse;


        private Fix32 angle;
        private Vector3 axis;

        private Vector3 biasVelocity;
        private Matrix3x3 effectiveMassMatrix;

        private Fix32 maxForceDt;
        private Fix32 maxForceDtSquared;
        private Fix32 usedSoftness;

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the angular velocity or orientation to a goal.
        /// </summary>
        /// <param name="entity">Affected entity.</param>
        public SingleEntityAngularMotor(Entity entity)
        {
            Entity = entity;

            settings = new MotorSettingsOrientation(this) {servo = {goal = base.entity.orientation}};
            //Since no target relative orientation was specified, just use the current relative orientation.  Prevents any nasty start-of-sim 'snapping.'

            //mySettings.myServo.springSettings.stiffnessConstant *= .5f;
        }

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the angular velocity or orientation to a goal.
        /// This constructor will make the angular motor start with isActive set to false.
        /// </summary>
        public SingleEntityAngularMotor()
        {
            settings = new MotorSettingsOrientation(this);
            IsActive = false;
        }

        /// <summary>
        /// Gets the basis attached to the entity.
        /// The target velocity/orientation of this motor is transformed by the basis.
        /// </summary>
        public JointBasis3D Basis
        {
            get { return basis; }
        }

        /// <summary>
        /// Gets the motor's velocity and servo settings.
        /// </summary>
        public MotorSettingsOrientation Settings
        {
            get { return settings; }
        }

        #region I3DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity with respect to the constraint.
        /// For single entity constraints, this is pretty straightforward.  It is taken directly from the 
        /// entity.
        /// </summary>
        public Vector3 RelativeVelocity
        {
            get { return -Entity.AngularVelocity; }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public Vector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// If the motor is in velocity only mode, error is zero.
        /// </summary>
        public Vector3 Error
        {
            get { return axis * angle; }
        }

        #endregion

        /// <summary>
        /// Applies the corrective impulses required by the constraint.
        /// </summary>
        public override Fix32 SolveIteration()
        {
#if !WINDOWS
            Vector3 lambda = new Vector3();
#else
            Vector3 lambda;
#endif
            Vector3 aVel = entity.angularVelocity;
            lambda.X = ((aVel.X.Neg()).Add(biasVelocity.X)).Sub(usedSoftness.Mul(accumulatedImpulse.X));
            lambda.Y = ((aVel.Y.Neg()).Add(biasVelocity.Y)).Sub(usedSoftness.Mul(accumulatedImpulse.Y));
            lambda.Z = ((aVel.Z.Neg()).Add(biasVelocity.Z)).Sub(usedSoftness.Mul(accumulatedImpulse.Z));

            Matrix3x3.Transform(ref lambda, ref effectiveMassMatrix, out lambda);

            Vector3 previousAccumulatedImpulse = accumulatedImpulse;
			accumulatedImpulse.X = accumulatedImpulse.X.Add(lambda.X);
			accumulatedImpulse.Y = accumulatedImpulse.Y.Add(lambda.Y);
			accumulatedImpulse.Z = accumulatedImpulse.Z.Add(lambda.Z);
            Fix32 sumLengthSquared = accumulatedImpulse.LengthSquared();

            if (sumLengthSquared > maxForceDtSquared)
            {
                //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                Fix32 multiplier = maxForceDt.Div(Fix32Ext.Sqrt(sumLengthSquared));
				accumulatedImpulse.X = accumulatedImpulse.X.Mul(multiplier);
				accumulatedImpulse.Y = accumulatedImpulse.Y.Mul(multiplier);
				accumulatedImpulse.Z = accumulatedImpulse.Z.Mul(multiplier);

                //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                lambda.X = accumulatedImpulse.X.Sub(previousAccumulatedImpulse.X);
                lambda.Y = accumulatedImpulse.Y.Sub(previousAccumulatedImpulse.Y);
                lambda.Z = accumulatedImpulse.Z.Sub(previousAccumulatedImpulse.Z);
            }


            entity.ApplyAngularImpulse(ref lambda);


            return (Fix32Ext.Abs(lambda.X).Add(Fix32Ext.Abs(lambda.Y))).Add(Fix32Ext.Abs(lambda.Z));
        }

        /// <summary>
        /// Initializes the constraint for the current frame.
        /// </summary>
        /// <param name="dt">Time between frames.</param>
        public override void Update(Fix32 dt)
        {
            basis.rotationMatrix = entity.orientationMatrix;
            basis.ComputeWorldSpaceAxes();

            Fix32 updateRate = F64.C1.Div(dt);
            if (settings.mode == MotorMode.Servomechanism) //Only need to do the bulk of this work if it's a servo.
            {
                Quaternion currentRelativeOrientation;
                var worldTransform = basis.WorldTransform;
                Quaternion.CreateFromRotationMatrix(ref worldTransform, out currentRelativeOrientation);


                //Compute the relative orientation R' between R and the target relative orientation.
                Quaternion errorOrientation;
                Quaternion.Conjugate(ref currentRelativeOrientation, out errorOrientation);
                Quaternion.Multiply(ref settings.servo.goal, ref errorOrientation, out errorOrientation);


                Fix32 errorReduction;
                settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, updateRate, out errorReduction, out usedSoftness);

                //Turn this into an axis-angle representation.
                Quaternion.GetAxisAngleFromQuaternion(ref errorOrientation, out axis, out angle);

                //Scale the axis by the desired velocity if the angle is sufficiently large (epsilon).
                if (angle > Toolbox.BigEpsilon)
                {
                    Fix32 velocity = MathHelper.Min(settings.servo.baseCorrectiveSpeed, angle.Mul(updateRate)).Add(angle.Mul(errorReduction));

                    biasVelocity.X = axis.X.Mul(velocity);
                    biasVelocity.Y = axis.Y.Mul(velocity);
                    biasVelocity.Z = axis.Z.Mul(velocity);


                    //Ensure that the corrective velocity doesn't exceed the max.
                    Fix32 length = biasVelocity.LengthSquared();
                    if (length > settings.servo.maxCorrectiveVelocitySquared)
                    {
                        Fix32 multiplier = settings.servo.maxCorrectiveVelocity.Div(Fix32Ext.Sqrt(length));
						biasVelocity.X = biasVelocity.X.Mul(multiplier);
						biasVelocity.Y = biasVelocity.Y.Mul(multiplier);
						biasVelocity.Z = biasVelocity.Z.Mul(multiplier);
                    }
                }
                else
                {
                    //Wouldn't want an old frame's bias velocity to sneak in.
                    biasVelocity = new Vector3();
                }
            }
            else
            {
                usedSoftness = settings.velocityMotor.softness.Mul(updateRate);
                angle = F64.C0; //Zero out the error;
                Matrix3x3 transform = basis.WorldTransform;
                Matrix3x3.Transform(ref settings.velocityMotor.goalVelocity, ref transform, out biasVelocity);
            }

            //Compute effective mass
            effectiveMassMatrix = entity.inertiaTensorInverse;
			effectiveMassMatrix.M11 = effectiveMassMatrix.M11.Add(usedSoftness);
			effectiveMassMatrix.M22 = effectiveMassMatrix.M22.Add(usedSoftness);
			effectiveMassMatrix.M33 = effectiveMassMatrix.M33.Add(usedSoftness);
            Matrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);

            //Update the maximum force
            ComputeMaxForces(settings.maximumForce, dt);


            
        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Apply accumulated impulse
            entity.ApplyAngularImpulse(ref accumulatedImpulse);
        }

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        private void ComputeMaxForces(Fix32 maxForce, Fix32 dt)
        {
            //Determine maximum force
            if (maxForce < Fix32.MaxValue)
            {
                maxForceDt = maxForce.Mul(dt);
                maxForceDtSquared = maxForceDt.Mul(maxForceDt);
            }
            else
            {
                maxForceDt = Fix32.MaxValue;
                maxForceDtSquared = Fix32.MaxValue;
            }
        }
    }
}