using System;
using System.Threading;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.SolverTypeTests
{
    class PlaneCollisionConstraint : Constraint
    {
        public LinearDynamic Dynamic { get; private set; }

        private Plane plane;

        private Fix effectiveMass;


        private Fix biasVelocity;

        private Fix accumulatedImpulse;
        private Fix impulse;


        public Plane Plane
        {
            get { return plane; }
        }

        /// <summary>
        /// Gets the distance from the plane to the dynamic.
        /// </summary>
        public Fix Distance
        {
            get
            {
                Fix d;
                Vector3.Dot(ref plane.Normal, ref Dynamic.Position, out d);
                return d.Add(plane.D);
            }
        }

        public PlaneCollisionConstraint(LinearDynamic dynamic, Plane plane)
        {
            Dynamic = dynamic;
            ++dynamic.ConstraintCount;
            this.plane = plane;
        }


        public override void Preupdate(Fix inverseDt, bool useConstraintCounts)
        {
            Fix d;
            Vector3.Dot(ref plane.Normal, ref Dynamic.Position, out d);

            if (useConstraintCounts)
                effectiveMass = 1.ToFix().Div(((Dynamic.ConstraintCount.ToFix().Mul(Dynamic.InverseMass)).Add(Softness)));
            else
                effectiveMass = 1.ToFix().Div((Dynamic.InverseMass.Add(Softness)));

            Fix error = d.Add(plane.D);
            if (error > 0.ToFix())
            {
                //Allow the dynamic to approach the plane, but no closer.
                biasVelocity = error.Mul(inverseDt);
            }
            else
            {
                biasVelocity = (error.Mul(BiasFactor)).Mul(inverseDt);
            }
        }

        public override void SolveIteration()
        {
            Fix velocityAlongJacobian;
            Vector3.Dot(ref Dynamic.Velocity, ref plane.Normal, out velocityAlongJacobian);


            Fix changeInVelocity = (velocityAlongJacobian.Neg().Sub(biasVelocity)).Sub(Softness.Mul(accumulatedImpulse));

            Fix newImpulse = changeInVelocity.Mul(effectiveMass);

            Fix newAccumulatedImpulse = accumulatedImpulse.Add(newImpulse);
            newAccumulatedImpulse = MathHelper.Max(newAccumulatedImpulse, 0.ToFix());

            impulse = newAccumulatedImpulse.Sub(accumulatedImpulse);
            accumulatedImpulse = newAccumulatedImpulse;

        }

        public override void ApplyImpulse(LinearDynamic dynamic)
        {
            ApplyImpulses();
        }

        public override void ApplyAccumulatedImpulse(LinearDynamic dynamic)
        {
            ApplyAccumulatedImpulses();
        }

        public override void ApplyImpulses()
        {
            Vector3 worldSpaceImpulse;
            Vector3.Multiply(ref plane.Normal, impulse, out worldSpaceImpulse);
            Dynamic.ApplyImpulse(ref worldSpaceImpulse);
        }

        public override void ApplyAccumulatedImpulses()
        {
            Vector3 worldSpaceImpulse;
            Vector3.Multiply(ref plane.Normal, accumulatedImpulse, out worldSpaceImpulse);
            Dynamic.ApplyImpulse(ref worldSpaceImpulse);
        }

        internal override void AddToConnections()
        {
            Dynamic.Constraints.Add(this);
        }
    }
}
