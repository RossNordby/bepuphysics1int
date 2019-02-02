using System;
using BEPUphysics.Entities;

using BEPUutilities;


namespace BEPUphysics.Constraints.TwoEntity.JointLimits
{
    /// <summary>
    /// Keeps the angle between the axes attached to two entities below some maximum value.
    /// </summary>
    public class SwingLimit : JointLimit, I1DImpulseConstraintWithError, I1DJacobianConstraint
    {
        private Fix32 accumulatedImpulse;
        private Fix32 biasVelocity;
        private Vector3 hingeAxis;
        private Fix32 minimumCosine = F64.C1;
        private Fix32 error;

        private Vector3 localAxisA;

        private Vector3 localAxisB;
        private Vector3 worldAxisA;

        private Vector3 worldAxisB;
        private Fix32 velocityToImpulse;

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
        /// as well as the WorldAxisA, WorldAxisB (or their entity-local versions) and the MaximumAngle.
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public SwingLimit()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        /// <param name="axisA">Axis attached to the first connected entity.</param>
        /// <param name="axisB">Axis attached to the second connected entity.</param>
        /// <param name="maximumAngle">Maximum angle between the axes allowed.</param>
        public SwingLimit(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB, Fix32 maximumAngle)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;
            WorldAxisA = axisA;
            WorldAxisB = axisB;
            MaximumAngle = maximumAngle;
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in its local space.
        /// </summary>
        public Vector3 LocalAxisA
        {
            get { return localAxisA; }
            set
            {
                localAxisA = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
            }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in its local space.
        /// </summary>
        public Vector3 LocalAxisB
        {
            get { return localAxisB; }
            set
            {
                localAxisB = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localAxisB, ref connectionA.orientationMatrix, out worldAxisB);
            }
        }

        /// <summary>
        /// Maximum angle allowed between the two axes, from 0 to pi.
        /// </summary>
        public Fix32 MaximumAngle
        {
            get { return minimumCosine.Acos(); }
            set { minimumCosine = MathHelper.Clamp(value, Fix32.Zero, MathHelper.Pi).Cos(); }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in world space.
        /// </summary>
        public Vector3 WorldAxisA
        {
            get { return worldAxisA; }
            set
            {
                worldAxisA = Vector3.Normalize(value);
                Quaternion conjugate;
                Quaternion.Conjugate(ref connectionA.orientation, out conjugate);
                Quaternion.Transform(ref worldAxisA, ref conjugate, out localAxisA);
            }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in world space.
        /// </summary>
        public Vector3 WorldAxisB
        {
            get { return worldAxisB; }
            set
            {
                worldAxisB = Vector3.Normalize(value);
                Quaternion conjugate;
                Quaternion.Conjugate(ref connectionB.orientation, out conjugate);
                Quaternion.Transform(ref worldAxisB, ref conjugate, out localAxisB);
            }
        }

        #region I1DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        public Fix32 RelativeVelocity
        {
            get
            {
                if (isLimitActive)
                {
                    Vector3 relativeVelocity;
                    Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
                    Fix32 lambda;
                    Vector3.Dot(ref relativeVelocity, ref hingeAxis, out lambda);
                    return lambda;
                }
                return Fix32.Zero;
            }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public Fix32 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// </summary>
        public Fix32 Error
        {
            get { return error; }
        }

        #endregion

        #region I1DJacobianConstraint Members

        /// <summary>
        /// Gets the linear jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
        public void GetLinearJacobianA(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the linear jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
        public void GetLinearJacobianB(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
        public void GetAngularJacobianA(out Vector3 jacobian)
        {
            jacobian = hingeAxis;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out Vector3 jacobian)
        {
            jacobian = -hingeAxis;
        }

        /// <summary>
        /// Gets the mass matrix of the constraint.
        /// </summary>
        /// <param name="outputMassMatrix">Constraint's mass matrix.</param>
        public void GetMassMatrix(out Fix32 outputMassMatrix)
        {
            outputMassMatrix = velocityToImpulse;
        }

        #endregion

        /// <summary>
        /// Applies the sequential impulse.
        /// </summary>
        public override Fix32 SolveIteration()
        {
            Fix32 lambda;
            Vector3 relativeVelocity;
            Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
            //Transform the velocity to with the jacobian
            Vector3.Dot(ref relativeVelocity, ref hingeAxis, out lambda);
            //Add in the constraint space bias velocity
            lambda = biasVelocity .Sub (lambda) .Sub (softness.Mul(accumulatedImpulse));

            //Transform to an impulse
            lambda = lambda .Mul (velocityToImpulse);

            //Clamp accumulated impulse (can't go negative)
            Fix32 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Max(accumulatedImpulse .Add (lambda), Fix32.Zero);
            lambda = accumulatedImpulse .Sub (previousAccumulatedImpulse);

            //Apply the impulse
            Vector3 impulse;
            Vector3.Multiply(ref hingeAxis, lambda, out impulse);
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Negate(ref impulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }

            return (lambda.Abs());
        }

        /// <summary>
        /// Initializes the constraint for this frame.
        /// </summary>
        /// <param name="dt">Time since the last frame.</param>
        public override void Update(Fix32 dt)
        {
            Matrix3x3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
            Matrix3x3.Transform(ref localAxisB, ref connectionB.orientationMatrix, out worldAxisB);

            Fix32 dot;
            Vector3.Dot(ref worldAxisA, ref worldAxisB, out dot);

            //Keep in mind, the dot is the cosine of the angle.
            //1: 0 radians
            //0: pi/2 radians
            //-1: pi radians
            if (dot > minimumCosine)
            {
                isActiveInSolver = false;
                error = Fix32.Zero;
                accumulatedImpulse = Fix32.Zero;
                isLimitActive = false;
                return;
            }
            isLimitActive = true;

            //Hinge axis is actually the jacobian entry for angular A (negative angular B).
            Vector3.Cross(ref worldAxisA, ref worldAxisB, out hingeAxis);
            Fix32 lengthSquared = hingeAxis.LengthSquared();
            if (lengthSquared < Toolbox.Epsilon)
            {
                //They're parallel; for the sake of continuity, pick some axis which is perpendicular to both that ISN'T the zero vector.
                Vector3.Cross(ref worldAxisA, ref Toolbox.UpVector, out hingeAxis);
                lengthSquared = hingeAxis.LengthSquared();
                if (lengthSquared < Toolbox.Epsilon)
                {
                    //That's improbable; b's world axis was apparently parallel with the up vector!
                    //So just use the right vector (it can't be parallel with both the up and right vectors).
                    Vector3.Cross(ref worldAxisA, ref Toolbox.RightVector, out hingeAxis);
                }
            }


            Fix32 errorReduction;
            springSettings.ComputeErrorReductionAndSoftness(dt, F64.C1 .Div (dt), out errorReduction, out softness);

            //Further away from 0 degrees is further negative; if the dot is below the minimum cosine, it means the angle is above the maximum angle.
            error = MathHelper.Max(Fix32.Zero, minimumCosine .Sub (dot) .Sub (margin));
            biasVelocity = MathHelper.Clamp(errorReduction .Mul (error), maxCorrectiveVelocity.Neg(), maxCorrectiveVelocity);

            if (bounciness > Fix32.Zero)
            {
                //Compute the speed around the axis.
                Fix32 relativeSpeed;
                Vector3 relativeVelocity;
                Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
                Vector3.Dot(ref relativeVelocity, ref hingeAxis, out relativeSpeed);

                biasVelocity = MathHelper.Max(biasVelocity, ComputeBounceVelocity(relativeSpeed.Neg()));
            }

            //Connection A's contribution to the mass matrix
            Fix32 entryA;
            Vector3 transformedAxis;
            if (connectionA.isDynamic)
            {
                Matrix3x3.Transform(ref hingeAxis, ref connectionA.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref hingeAxis, out entryA);
            }
            else
                entryA = Fix32.Zero;

            //Connection B's contribution to the mass matrix
            Fix32 entryB;
            if (connectionB.isDynamic)
            {
                Matrix3x3.Transform(ref hingeAxis, ref connectionB.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref hingeAxis, out entryB);
            }
            else
                entryB = Fix32.Zero;

            //Compute the inverse mass matrix
            velocityToImpulse = F64.C1 .Div (softness .Add (entryA) .Add (entryB));


        }

        public override void ExclusiveUpdate()
        {
            //Apply accumulated impulse
            Vector3 impulse;
            Vector3.Multiply(ref hingeAxis, accumulatedImpulse, out impulse);
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Negate(ref impulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }
        }
    }
}