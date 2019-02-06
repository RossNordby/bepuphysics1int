using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.SolverTypeTests
{
    abstract class Constraint
    {

        public Fix32 Softness = 0.ToFix();
        public Fix32 BiasFactor = 0.2m.ToFix();

        public abstract void Preupdate(Fix32 inverseDt, bool useConstraintCounts);

        public abstract void SolveIteration();

        public abstract void ApplyImpulse(LinearDynamic dynamic);

        public abstract void ApplyAccumulatedImpulse(LinearDynamic dynamic);

        public abstract void ApplyImpulses();

        public abstract void ApplyAccumulatedImpulses();

        internal abstract void AddToConnections();

        public abstract void EnterLock();
        public abstract void ExitLock();
    }
}
