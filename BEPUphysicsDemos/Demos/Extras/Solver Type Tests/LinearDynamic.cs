using System.Threading;
using BEPUutilities;
using BEPUutilities.DataStructures;

namespace BEPUphysicsDemos.Demos.Extras.SolverTypeTests
{
    class LinearDynamic
    {
        private static long DynamicId;

        private static long GetId()
        {
            return Interlocked.Increment(ref DynamicId);
        }


        public RawList<Constraint> Constraints = new RawList<Constraint>();

        private Fix mass, inverseMass;
        public Fix Mass
        {
            get { return mass; }
        }

        public Fix InverseMass
        {
            get { return inverseMass; }
        }

        internal int ConstraintCount;

        public Vector3 Velocity;

        public Vector3 Position;

        /// <summary>
        /// Used to determine lock order in the locking simulator type.
        /// </summary>
        public readonly long Id;

        public LinearDynamic(Fix mass)
        {
            this.mass = mass;
            this.inverseMass = 1.ToFix().Div(mass);

            Id = GetId();
        }

        public void UpdatePosition(Fix dt)
        {
            Vector3 displacement;
            Vector3.Multiply(ref Velocity, dt, out displacement);
            Vector3.Add(ref displacement, ref Position, out Position);
        }

        public void ApplyImpulse(ref Vector3 worldSpaceImpulse)
        {
            Vector3 velocityChange;
            Vector3.Multiply(ref worldSpaceImpulse, inverseMass, out velocityChange);
            Vector3.Add(ref velocityChange, ref Velocity, out Velocity);
        }
    }
}
