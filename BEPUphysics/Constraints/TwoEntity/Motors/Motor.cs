

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Superclass of constraints which do work and change the velocity of connected entities, but have no specific position target.
    /// </summary>
    public abstract class Motor : TwoEntityConstraint
    {
        protected Fix maxForceDt = Fix.MaxValue;
        protected Fix maxForceDtSquared = Fix.MaxValue;

        /// <summary>
        /// Softness divided by the timestep to maintain timestep independence.
        /// </summary>
        internal Fix usedSoftness;

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        protected void ComputeMaxForces(Fix maxForce, Fix dt)
        {
            //Determine maximum force
            if (maxForce < Fix.MaxValue)
            {
                maxForceDt = maxForce.Mul(dt);
                maxForceDtSquared = maxForceDt.Mul(maxForceDt);
            }
            else
            {
                maxForceDt = Fix.MaxValue;
                maxForceDtSquared = Fix.MaxValue;
            }
        }
    }
}