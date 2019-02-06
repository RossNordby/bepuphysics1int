using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.SolverTypeTests
{
    class DistanceConstraint : Constraint
    {
        public LinearDynamic A { get; private set; }
        public LinearDynamic B { get; private set; }


        private Vector3 jacobian;
        private Fix32 effectiveMass;


        private Fix32 biasVelocity;

        private Fix32 accumulatedImpulse;
        private Fix32 impulse;

        private Fix32 distance;

        public DistanceConstraint(LinearDynamic a, LinearDynamic b)
        {
            A = a;
            B = b;
            ++a.ConstraintCount;
            ++b.ConstraintCount;

            distance = (a.Position - b.Position).Length();
        }

        public override void Preupdate(Fix32 inverseDt, bool useConstraintCounts)
        {
            Vector3.Subtract(ref B.Position, ref A.Position, out jacobian);
            Fix32 currentDistance = jacobian.LengthSquared();
            if (currentDistance > Toolbox.Epsilon)
            {
                currentDistance = Fix32Ext.Sqrt(currentDistance);
                Vector3.Divide(ref jacobian, currentDistance, out jacobian);
            }
            else
            {
                currentDistance = 0.ToFix();
                jacobian = Toolbox.UpVector;
            }

            if (useConstraintCounts)
                effectiveMass = 1.ToFix().Div((((A.ConstraintCount.ToFix().Mul(A.InverseMass)).Add(B.ConstraintCount.ToFix().Mul(B.InverseMass))).Add(Softness)));
            else
                effectiveMass = 1.ToFix().Div(((A.InverseMass.Add(B.InverseMass)).Add(Softness)));
            accumulatedImpulse = 0.ToFix();
            biasVelocity = ((distance.Sub(currentDistance)).Mul(BiasFactor)).Mul(inverseDt);

        }

        public override void SolveIteration()
        {
            Vector3 relativeVelocity;
            Vector3.Subtract(ref B.Velocity, ref A.Velocity, out relativeVelocity);
            Fix32 relativeVelocityAlongJacobian;
            Vector3.Dot(ref relativeVelocity, ref jacobian, out relativeVelocityAlongJacobian);


            Fix32 changeInVelocity = (relativeVelocityAlongJacobian.Sub(biasVelocity)).Sub(Softness.Mul(accumulatedImpulse));

            impulse = changeInVelocity.Mul(effectiveMass);

			accumulatedImpulse =
accumulatedImpulse.Add(impulse);

        }

        public override void ApplyImpulse(LinearDynamic dynamic)
        {
            ApplyImpulse(dynamic, impulse);
        }

        public override void ApplyAccumulatedImpulse(LinearDynamic dynamic)
        {
            ApplyImpulse(dynamic, accumulatedImpulse);
        }

        private void ApplyImpulse(LinearDynamic dynamic, Fix32 impulseToApply)
        {
            Vector3 worldSpaceImpulse;
            if (A == dynamic)
            {
                Vector3.Multiply(ref jacobian, impulseToApply, out worldSpaceImpulse);
            }
            else
            {
                Vector3.Multiply(ref jacobian, impulseToApply.Neg(), out worldSpaceImpulse);
            }
            dynamic.ApplyImpulse(ref worldSpaceImpulse);

        }

        public override void ApplyImpulses()
        {
            Vector3 worldSpaceImpulse;
            Vector3.Multiply(ref jacobian, impulse, out worldSpaceImpulse);
            A.ApplyImpulse(ref worldSpaceImpulse);
            Vector3.Negate(ref worldSpaceImpulse, out worldSpaceImpulse);
            B.ApplyImpulse(ref worldSpaceImpulse);
        }

        public override void ApplyAccumulatedImpulses()
        {
            Vector3 worldSpaceImpulse;
            Vector3.Multiply(ref jacobian, accumulatedImpulse, out worldSpaceImpulse);
            A.ApplyImpulse(ref worldSpaceImpulse);
            Vector3.Negate(ref worldSpaceImpulse, out worldSpaceImpulse);
            B.ApplyImpulse(ref worldSpaceImpulse);
        }


        internal override void AddToConnections()
        {
            A.Constraints.Add(this);
            B.Constraints.Add(this);
        }

        public override void EnterLock()
        {
            if (A.Id <= B.Id)
            {
                A.SolverSpinLock.Enter();
                B.SolverSpinLock.Enter();
            }
            else
            {
                B.SolverSpinLock.Enter();
                A.SolverSpinLock.Enter();
            }
        }

        public override void ExitLock()
        {
            if (A.Id <= B.Id)
            {
                B.SolverSpinLock.Exit();
                A.SolverSpinLock.Exit();
            }
            else
            {
                A.SolverSpinLock.Exit();
                B.SolverSpinLock.Exit();
            }
        }
    }
}
