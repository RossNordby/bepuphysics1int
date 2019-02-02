

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Superclass of constraints which do work and change the velocity of connected entities, but have no specific position target.
    /// </summary>
    public abstract class Motor : TwoEntityConstraint
    {
        protected Fix32 maxForceDt = Fix32.MaxValue;
        protected Fix32 maxForceDtSquared = Fix32.MaxValue;

        /// <summary>
        /// Softness divided by the timestep to maintain timestep independence.
        /// </summary>
        internal Fix32 usedSoftness;

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        protected void ComputeMaxForces(Fix32 maxForce, Fix32 dt)
        {
            //Determine maximum force
            if (maxForce < Fix32.MaxValue)
            {
                maxForceDt = maxForce .Mul (dt);
                maxForceDtSquared = maxForceDt .Mul (maxForceDt);
            }
            else
            {
                maxForceDt = Fix32.MaxValue;
                maxForceDtSquared = Fix32.MaxValue;
            }
        }
    }
}