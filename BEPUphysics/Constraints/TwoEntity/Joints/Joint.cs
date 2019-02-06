using BEPUutilities;

using System;

namespace BEPUphysics.Constraints.TwoEntity.Joints
{
    /// <summary>
    /// Superclass of position-based constraints.
    /// </summary>
    public abstract class Joint : TwoEntityConstraint, ISpringSettings
    {
        /// <summary>
        /// Maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected Fix32 maxCorrectiveVelocity = Fix32.MaxValue;

        /// <summary>
        /// Squared maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected Fix32 maxCorrectiveVelocitySquared = Fix32.MaxValue;

        protected Fix32 softness;

        /// <summary>
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        protected SpringSettings springSettings = new SpringSettings();

        /// <summary>
        /// Gets or sets the maximum extra velocity that the constraint will apply in an effort to correct any constraint error.
        /// </summary>
        public Fix32 MaxCorrectiveVelocity
        {
            get { return maxCorrectiveVelocity; }
            set
            {
                maxCorrectiveVelocity = MathHelper.Max(F64.C0, value);
                if (maxCorrectiveVelocity >= Fix32.MaxValue)
                {
                    maxCorrectiveVelocitySquared = Fix32.MaxValue;
                }
                else
                {
                    maxCorrectiveVelocitySquared = maxCorrectiveVelocity.Mul(maxCorrectiveVelocity);
                }
            }
        }

        #region ISpringSettings Members

        /// <summary>
        /// Gets the spring settings used by the constraint.
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        public SpringSettings SpringSettings
        {
            get { return springSettings; }
        }

        #endregion
    }
}