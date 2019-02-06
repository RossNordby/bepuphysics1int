using BEPUphysics.Entities.Prefabs;
using BEPUphysics.PositionUpdating;
using BEPUutilities;
using FixMath.NET;

namespace BEPUbenchmark.Benchmarks
{
	public class DiscreteVsContinuousBenchmark : Benchmark
	{
		protected override void InitializeSpace()
		{
			//Create the discretely updated spheres.  These will fly through the ground due to their high speed.
			var toAdd = new Sphere(new Vector3((-6).ToFix(), 150.ToFix(), 0.ToFix()), .5m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			Space.Add(toAdd);

			toAdd = new Sphere(new Vector3((-4).ToFix(), 150.ToFix(), 0.ToFix()), .25m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			Space.Add(toAdd);

			toAdd = new Sphere(new Vector3((-2).ToFix(), 150.ToFix(), 0.ToFix()), .1m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			Space.Add(toAdd);

			Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 20m.ToFix(), .1m.ToFix(), 20.ToFix()));


			//Create the continuously updated spheres.  These will hit the ground and stop.
			toAdd = new Sphere(new Vector3(6.ToFix(), 150.ToFix(), 0.ToFix()), .5m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			toAdd.PositionUpdateMode = PositionUpdateMode.Continuous;
			Space.Add(toAdd);

			toAdd = new Sphere(new Vector3(4.ToFix(), 150.ToFix(), 0.ToFix()), .25m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			toAdd.PositionUpdateMode = PositionUpdateMode.Continuous;
			Space.Add(toAdd);

			toAdd = new Sphere(new Vector3(2.ToFix(), 150.ToFix(), 0.ToFix()), .1m.ToFix(), 1.ToFix());
			toAdd.LinearVelocity = new Vector3(0.ToFix(), (-100).ToFix(), 0.ToFix());
			toAdd.PositionUpdateMode = PositionUpdateMode.Continuous;
			Space.Add(toAdd);
		}
	}
}
