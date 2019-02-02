using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
 
using BEPUphysics.Materials;
using BEPUutilities;

namespace BEPUphysics.Vehicle
{
    /// <summary>
    /// Attempts to resist sliding motion of a vehicle.
    /// </summary>
    public class WheelSlidingFriction : ISolverSettings
    {
        #region Static Stuff

        /// <summary>
        /// Default blender used by WheelSlidingFriction constraints.
        /// </summary>
        public static WheelFrictionBlender DefaultSlidingFrictionBlender;

        static WheelSlidingFriction()
        {
            DefaultSlidingFrictionBlender = BlendFriction;
        }

        /// <summary>
        /// Function which takes the friction values from a wheel and a supporting material and computes the blended friction.
        /// </summary>
        /// <param name="wheelFriction">Friction coefficient associated with the wheel.</param>
        /// <param name="materialFriction">Friction coefficient associated with the support material.</param>
        /// <param name="usingKineticFriction">True if the friction coefficients passed into the blender are kinetic coefficients, false otherwise.</param>
        /// <param name="wheel">Wheel being blended.</param>
        /// <returns>Blended friction coefficient.</returns>
        public static Fix32 BlendFriction(Fix32 wheelFriction, Fix32 materialFriction, bool usingKineticFriction, Wheel wheel)
        {
            return wheelFriction.Mul(materialFriction);
        }

        #endregion

        internal Fix32 accumulatedImpulse;

        //Fix32 linearBX, linearBY, linearBZ;
        private Fix32 angularAX, angularAY, angularAZ;
        private Fix32 angularBX, angularBY, angularBZ;
        internal bool isActive = true;
        private Fix32 linearAX, linearAY, linearAZ;
        private Fix32 blendedCoefficient;
        private Fix32 kineticCoefficient;
        private WheelFrictionBlender frictionBlender = DefaultSlidingFrictionBlender;
        internal Vector3 slidingFrictionAxis;
        internal SolverSettings solverSettings = new SolverSettings();
        private Fix32 staticCoefficient;
        private Fix32 staticFrictionVelocityThreshold = F64.C5;
        private Wheel wheel;
        internal int numIterationsAtZeroImpulse;
        private Entity vehicleEntity, supportEntity;

        //Inverse effective mass matrix
        private Fix32 velocityToImpulse;

        /// <summary>
        /// Constructs a new sliding friction object for a wheel.
        /// </summary>
        /// <param name="dynamicCoefficient">Coefficient of dynamic sliding friction to be blended with the supporting entity's friction.</param>
        /// <param name="staticCoefficient">Coefficient of static sliding friction to be blended with the supporting entity's friction.</param>
        public WheelSlidingFriction(Fix32 dynamicCoefficient, Fix32 staticCoefficient)
        {
            KineticCoefficient = dynamicCoefficient;
            StaticCoefficient = staticCoefficient;
        }

        internal WheelSlidingFriction(Wheel wheel)
        {
            Wheel = wheel;
        }

        /// <summary>
        /// Gets the coefficient of sliding friction between the wheel and support.
        /// This coefficient is the blended result of the supporting entity's friction and the wheel's friction.
        /// </summary>
        public Fix32 BlendedCoefficient
        {
            get { return blendedCoefficient; }
        }

        /// <summary>
        /// Gets or sets the coefficient of dynamic horizontal sliding friction for this wheel.
        /// This coefficient and the supporting entity's coefficient of friction will be 
        /// taken into account to determine the used coefficient at any given time.
        /// </summary>
        public Fix32 KineticCoefficient
        {
            get { return kineticCoefficient; }
            set { kineticCoefficient = MathHelper.Max(value, Fix32.Zero); }
        }

        /// <summary>
        /// Gets or sets the function used to blend the supporting entity's friction and the wheel's friction.
        /// </summary>
        public WheelFrictionBlender FrictionBlender
        {
            get { return frictionBlender; }
            set { frictionBlender = value; }
        }

        /// <summary>
        /// Gets the axis along which sliding friction is applied.
        /// </summary>
        public Vector3 SlidingFrictionAxis
        {
            get { return slidingFrictionAxis; }
        }

        /// <summary>
        /// Gets or sets the coefficient of static horizontal sliding friction for this wheel.
        /// This coefficient and the supporting entity's coefficient of friction will be 
        /// taken into account to determine the used coefficient at any given time.
        /// </summary>
        public Fix32 StaticCoefficient
        {
            get { return staticCoefficient; }
            set { staticCoefficient = MathHelper.Max(value, Fix32.Zero); }
        }

        /// <summary>
        /// Gets or sets the velocity under which the coefficient of static friction will be used instead of the dynamic one.
        /// </summary>
        public Fix32 StaticFrictionVelocityThreshold
        {
            get { return staticFrictionVelocityThreshold; }
            set { staticFrictionVelocityThreshold = value.Abs(); }
        }

        /// <summary>
        /// Gets the force 
        /// </summary>
        public Fix32 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the wheel that this sliding friction applies to.
        /// </summary>
        public Wheel Wheel
        {
            get { return wheel; }
            internal set { wheel = value; }
        }

        #region ISolverSettings Members

        /// <summary>
        /// Gets the solver settings used by this wheel constraint.
        /// </summary>
        public SolverSettings SolverSettings
        {
            get { return solverSettings; }
        }

        #endregion

        bool supportIsDynamic;

        ///<summary>
        /// Gets the relative velocity along the sliding direction at the wheel contact.
        ///</summary>
        public Fix32 RelativeVelocity
        {
            get
            {
                Fix32 velocity = vehicleEntity.linearVelocity.X.Mul(linearAX) .Add( vehicleEntity.linearVelocity.Y.Mul(linearAY) ).Add( vehicleEntity.linearVelocity.Z.Mul(linearAZ) ).Add(
                            vehicleEntity.angularVelocity.X.Mul(angularAX) ).Add( vehicleEntity.angularVelocity.Y.Mul(angularAY) ).Add( vehicleEntity.angularVelocity.Z.Mul(angularAZ) );
                if (supportEntity != null)
					velocity = velocity.Add( supportEntity.linearVelocity.X.Mul(linearAX).Neg() .Sub( supportEntity.linearVelocity.Y.Mul(linearAY) ).Sub( supportEntity.linearVelocity.Z.Mul(linearAZ) ).Add(
                                supportEntity.angularVelocity.X.Mul(angularBX) ).Add( supportEntity.angularVelocity.Y.Mul(angularBY) ).Add( supportEntity.angularVelocity.Z.Mul(angularBZ) ) );
                return velocity;
            }
        }

        internal Fix32 ApplyImpulse()
        {
            //Compute relative velocity and convert to an impulse
            Fix32 lambda = RelativeVelocity.Mul(velocityToImpulse);


            //Clamp accumulated impulse
            Fix32 previousAccumulatedImpulse = accumulatedImpulse;
            Fix32 maxForce = blendedCoefficient.Neg().Mul(wheel.suspension.accumulatedImpulse);
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
            if (vehicleEntity.isDynamic)
            {
                angular.X = lambda.Mul(angularAX);
                angular.Y = lambda.Mul(angularAY);
                angular.Z = lambda.Mul(angularAZ);
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }
            if (supportIsDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                angular.X = lambda.Mul(angularBX);
                angular.Y = lambda.Mul(angularBY);
                angular.Z = lambda.Mul(angularBZ);
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }

            return lambda;
        }

        internal void PreStep(Fix32 dt)
        {
            vehicleEntity = wheel.Vehicle.Body;
            supportEntity = wheel.SupportingEntity;
            supportIsDynamic = supportEntity != null && supportEntity.isDynamic;
            Vector3.Cross(ref wheel.worldForwardDirection, ref wheel.normal, out slidingFrictionAxis);
            Fix32 axisLength = slidingFrictionAxis.LengthSquared();
            //Safety against bad cross product
            if (axisLength < Toolbox.BigEpsilon)
            {
                Vector3.Cross(ref wheel.worldForwardDirection, ref Toolbox.UpVector, out slidingFrictionAxis);
                axisLength = slidingFrictionAxis.LengthSquared();
                if (axisLength < Toolbox.BigEpsilon)
                {
                    Vector3.Cross(ref wheel.worldForwardDirection, ref Toolbox.RightVector, out slidingFrictionAxis);
                }
            }
            slidingFrictionAxis.Normalize();

            linearAX = slidingFrictionAxis.X;
            linearAY = slidingFrictionAxis.Y;
            linearAZ = slidingFrictionAxis.Z;

            //angular A = Ra x N
            angularAX = wheel.ra.Y.Mul(linearAZ) .Sub( wheel.ra.Z.Mul(linearAY) );
            angularAY = wheel.ra.Z.Mul(linearAX) .Sub( wheel.ra.X.Mul(linearAZ) );
            angularAZ = wheel.ra.X.Mul(linearAY) .Sub( wheel.ra.Y.Mul(linearAX) );

            //Angular B = N x Rb
            angularBX = linearAY.Mul(wheel.rb.Z) .Sub( linearAZ.Mul(wheel.rb.Y) );
            angularBY = linearAZ.Mul(wheel.rb.X) .Sub( linearAX.Mul(wheel.rb.Z) );
            angularBZ = linearAX.Mul(wheel.rb.Y) .Sub( linearAY.Mul(wheel.rb.X) );

            //Compute inverse effective mass matrix
            Fix32 entryA, entryB;

            //these are the transformed coordinates
            Fix32 tX, tY, tZ;
            if (vehicleEntity.isDynamic)
            {
                tX = angularAX.Mul(vehicleEntity.inertiaTensorInverse.M11) .Add( angularAY.Mul(vehicleEntity.inertiaTensorInverse.M21) ).Add( angularAZ.Mul(vehicleEntity.inertiaTensorInverse.M31) );
                tY = angularAX.Mul(vehicleEntity.inertiaTensorInverse.M12) .Add( angularAY.Mul(vehicleEntity.inertiaTensorInverse.M22) ).Add( angularAZ.Mul(vehicleEntity.inertiaTensorInverse.M32) );
                tZ = angularAX.Mul(vehicleEntity.inertiaTensorInverse.M13) .Add( angularAY.Mul(vehicleEntity.inertiaTensorInverse.M23) ).Add( angularAZ.Mul(vehicleEntity.inertiaTensorInverse.M33) );
                entryA = tX.Mul(angularAX) .Add( tY.Mul(angularAY) ).Add( tZ.Mul(angularAZ) ).Add( vehicleEntity.inverseMass );
            }
            else
                entryA = Fix32.Zero;

            if (supportIsDynamic)
            {
                tX = angularBX.Mul(supportEntity.inertiaTensorInverse.M11) .Add( angularBY.Mul(supportEntity.inertiaTensorInverse.M21) ).Add( angularBZ.Mul(supportEntity.inertiaTensorInverse.M31) );
                tY = angularBX.Mul(supportEntity.inertiaTensorInverse.M12) .Add( angularBY.Mul(supportEntity.inertiaTensorInverse.M22) ).Add( angularBZ.Mul(supportEntity.inertiaTensorInverse.M32) );
                tZ = angularBX.Mul(supportEntity.inertiaTensorInverse.M13) .Add( angularBY.Mul(supportEntity.inertiaTensorInverse.M23) ).Add( angularBZ.Mul(supportEntity.inertiaTensorInverse.M33) );
                entryB = tX.Mul(angularBX) .Add( tY.Mul(angularBY) ).Add( tZ.Mul(angularBZ) ).Add( supportEntity.inverseMass );
            }
            else
                entryB = Fix32.Zero;

            velocityToImpulse = Fix32.MinusOne.Div( entryA.Add(entryB) ); //Softness?

            //Compute friction.
            //Which coefficient? Check velocity.
            if (RelativeVelocity.Abs() < staticFrictionVelocityThreshold)
                blendedCoefficient = frictionBlender(staticCoefficient, wheel.supportMaterial.staticFriction, false, wheel);
            else
                blendedCoefficient = frictionBlender(kineticCoefficient, wheel.supportMaterial.kineticFriction, true, wheel);



        }

        internal void ExclusiveUpdate()
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
            if (vehicleEntity.isDynamic)
            {
                angular.X = accumulatedImpulse.Mul(angularAX);
				angular.Y = accumulatedImpulse.Mul(angularAY);
                angular.Z = accumulatedImpulse.Mul(angularAZ);
                vehicleEntity.ApplyLinearImpulse(ref linear);
                vehicleEntity.ApplyAngularImpulse(ref angular);
            }
            if (supportIsDynamic)
            {
                linear.X = linear.X.Neg();
                linear.Y = linear.Y.Neg();
                linear.Z = linear.Z.Neg();
                angular.X = accumulatedImpulse.Mul(angularBX);
                angular.Y = accumulatedImpulse.Mul(angularBY);
                angular.Z = accumulatedImpulse.Mul(angularBZ);
                supportEntity.ApplyLinearImpulse(ref linear);
                supportEntity.ApplyAngularImpulse(ref angular);
            }
        }
    }
}
