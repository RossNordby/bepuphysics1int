using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
 
using BEPUphysics.Materials;
using BEPUutilities;


namespace BEPUphysics.Vehicle
{
    /// <summary>
    /// Attempts to resist rolling motion of a vehicle.
    /// </summary>
    public class WheelBrake : ISolverSettings
    {
        #region Static Stuff

        /// <summary>
        /// Default blender used by WheelRollingFriction constraints.
        /// </summary>
        public static WheelFrictionBlender DefaultRollingFrictionBlender;

        static WheelBrake()
        {
            DefaultRollingFrictionBlender = BlendFriction;
        }

        /// <summary>
        /// Function which takes the friction values from a wheel and a supporting material and computes the blended friction.
        /// </summary>
        /// <param name="wheelFriction">Friction coefficient associated with the wheel.</param>
        /// <param name="materialFriction">Friction coefficient associated with the support material.</param>
        /// <param name="usingKineticFriction">True if the friction coefficients passed into the blender are kinetic coefficients, false otherwise.</param>
        /// <param name="wheel">Wheel being blended.</param>
        /// <returns>Blended friction coefficient.</returns>
        public static Fix BlendFriction(Fix wheelFriction, Fix materialFriction, bool usingKineticFriction, Wheel wheel)
        {
            return wheelFriction.Mul(materialFriction);
        }

        #endregion

        internal Fix accumulatedImpulse;

        //Fix32 linearBX, linearBY, linearBZ;
        private Fix angularAX, angularAY, angularAZ;
        private Fix angularBX, angularBY, angularBZ;
        internal bool isActive = true;
        private Fix linearAX, linearAY, linearAZ;
        private Fix blendedCoefficient;
        private Fix kineticBrakingFrictionCoefficient;
        private WheelFrictionBlender frictionBlender = DefaultRollingFrictionBlender;
        private bool isBraking;
        private Fix rollingFrictionCoefficient;
        internal SolverSettings solverSettings = new SolverSettings();
        private Fix staticBrakingFrictionCoefficient;
        private Fix staticFrictionVelocityThreshold = F64.C5;
        private Wheel wheel;
        internal int numIterationsAtZeroImpulse;
        private Entity vehicleEntity, supportEntity;

        //Inverse effective mass matrix
        private Fix velocityToImpulse;
        private bool supportIsDynamic;


        /// <summary>
        /// Constructs a new rolling friction object for a wheel.
        /// </summary>
        /// <param name="dynamicBrakingFrictionCoefficient">Coefficient of dynamic friction of the wheel for friction when the brake is active.</param>
        /// <param name="staticBrakingFrictionCoefficient">Coefficient of static friction of the wheel for friction when the brake is active.</param>
        /// <param name="rollingFrictionCoefficient">Coefficient of friction of the wheel for rolling friction when the brake isn't active.</param>
        public WheelBrake(Fix dynamicBrakingFrictionCoefficient, Fix staticBrakingFrictionCoefficient, Fix rollingFrictionCoefficient)
        {
            KineticBrakingFrictionCoefficient = dynamicBrakingFrictionCoefficient;
            StaticBrakingFrictionCoefficient = staticBrakingFrictionCoefficient;
            RollingFrictionCoefficient = rollingFrictionCoefficient;
        }

        internal WheelBrake(Wheel wheel)
        {
            Wheel = wheel;
        }

        /// <summary>
        /// Gets the coefficient of rolling friction between the wheel and support.
        /// This coefficient is the blended result of the supporting entity's friction and the wheel's friction.
        /// </summary>
        public Fix BlendedCoefficient
        {
            get { return blendedCoefficient; }
        }

        /// <summary>
        /// Gets or sets the coefficient of braking dynamic friction for this wheel.
        /// This coefficient and the supporting entity's coefficient of friction will be 
        /// taken into account to determine the used coefficient at any given time.
        /// This coefficient is used instead of the rollingFrictionCoefficient when 
        /// isBraking is true.
        /// </summary>
        public Fix KineticBrakingFrictionCoefficient
        {
            get { return kineticBrakingFrictionCoefficient; }
            set { kineticBrakingFrictionCoefficient = MathHelper.Max(value, F64.C0); }
        }

        /// <summary>
        /// Gets the axis along which rolling friction is applied.
        /// </summary>
        public Vector3 FrictionAxis
        {
            get { return wheel.drivingMotor.ForceAxis; }
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
        /// Gets or sets whether or not the wheel is braking.
        /// When set to true, the brakingFrictionCoefficient is used.
        /// When false, the rollingFrictionCoefficient is used.
        /// </summary>
        public bool IsBraking
        {
            get { return isBraking; }
            set { isBraking = value; }
        }

        /// <summary>
        /// Gets or sets the coefficient of rolling friction for this wheel.
        /// This coefficient and the supporting entity's coefficient of friction will be 
        /// taken into account to determine the used coefficient at any given time.
        /// This coefficient is used instead of the brakingFrictionCoefficient when 
        /// isBraking is false.
        /// </summary>
        public Fix RollingFrictionCoefficient
        {
            get { return rollingFrictionCoefficient; }
            set { rollingFrictionCoefficient = MathHelper.Max(value, F64.C0); }
        }

        /// <summary>
        /// Gets or sets the coefficient of static dynamic friction for this wheel.
        /// This coefficient and the supporting entity's coefficient of friction will be 
        /// taken into account to determine the used coefficient at any given time.
        /// This coefficient is used instead of the rollingFrictionCoefficient when 
        /// isBraking is true.
        /// </summary>
        public Fix StaticBrakingFrictionCoefficient
        {
            get { return staticBrakingFrictionCoefficient; }
            set { staticBrakingFrictionCoefficient = MathHelper.Max(value, F64.C0); }
        }

        /// <summary>
        /// Gets or sets the velocity under which the coefficient of static friction will be used instead of the dynamic one.
        /// </summary>
        public Fix StaticFrictionVelocityThreshold
        {
            get { return staticFrictionVelocityThreshold; }
            set { staticFrictionVelocityThreshold = Fix32Ext.Abs(value); }
        }

        /// <summary>
        /// Gets the force 
        /// </summary>
        public Fix TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the wheel that this rolling friction applies to.
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

        ///<summary>
        /// Gets the relative velocity along the braking direction at the wheel contact.
        ///</summary>
        public Fix RelativeVelocity
        {
            get
            {
                Fix velocity = (((((vehicleEntity.linearVelocity.X.Mul(linearAX)).Add(vehicleEntity.linearVelocity.Y.Mul(linearAY))).Add(vehicleEntity.linearVelocity.Z.Mul(linearAZ))).Add(vehicleEntity.angularVelocity.X.Mul(angularAX))).Add(vehicleEntity.angularVelocity.Y.Mul(angularAY))).Add(vehicleEntity.angularVelocity.Z.Mul(angularAZ));
                if (supportEntity != null)
					velocity = velocity.Add((((((supportEntity.linearVelocity.X.Neg().Mul(linearAX)).Sub(supportEntity.linearVelocity.Y.Mul(linearAY))).Sub(supportEntity.linearVelocity.Z.Mul(linearAZ))).Add(supportEntity.angularVelocity.X.Mul(angularBX))).Add(supportEntity.angularVelocity.Y.Mul(angularBY))).Add(supportEntity.angularVelocity.Z.Mul(angularBZ)));
                return velocity;
            }
        }

        internal Fix ApplyImpulse()
        {
            //Compute relative velocity and convert to impulse
            Fix lambda = RelativeVelocity.Mul(velocityToImpulse);


            //Clamp accumulated impulse
            Fix previousAccumulatedImpulse = accumulatedImpulse;
            Fix maxForce = (blendedCoefficient.Neg()).Mul(wheel.suspension.accumulatedImpulse);
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

        internal void PreStep(Fix dt)
        {
            vehicleEntity = wheel.Vehicle.Body;
            supportEntity = wheel.SupportingEntity;
            supportIsDynamic = supportEntity != null && supportEntity.isDynamic;

            //Grab jacobian and mass matrix from the driving motor!
            linearAX = wheel.drivingMotor.linearAX;
            linearAY = wheel.drivingMotor.linearAY;
            linearAZ = wheel.drivingMotor.linearAZ;

            angularAX = wheel.drivingMotor.angularAX;
            angularAY = wheel.drivingMotor.angularAY;
            angularAZ = wheel.drivingMotor.angularAZ;
            angularBX = wheel.drivingMotor.angularBX;
            angularBY = wheel.drivingMotor.angularBY;
            angularBZ = wheel.drivingMotor.angularBZ;

            velocityToImpulse = wheel.drivingMotor.velocityToImpulse;

            //Friction
            //Which coefficient? Check velocity.
            if (isBraking)
                if (Fix32Ext.Abs(RelativeVelocity) < staticFrictionVelocityThreshold)
                    blendedCoefficient = frictionBlender(staticBrakingFrictionCoefficient, wheel.supportMaterial.staticFriction, false, wheel);
                else
                    blendedCoefficient = frictionBlender(kineticBrakingFrictionCoefficient, wheel.supportMaterial.kineticFriction, true, wheel);
            else
                blendedCoefficient = rollingFrictionCoefficient;


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