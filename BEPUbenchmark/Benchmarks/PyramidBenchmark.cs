using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUbenchmark.Benchmarks
{
	public class PyramidBenchmark : Benchmark
	{
		protected override void InitializeSpace()
		{
			Fix64 boxSize = 2.ToFix();
			int boxCount = 20;
			Fix64 platformLength = MathHelper.Min(50.ToFix(), (boxCount.ToFix().Mul(boxSize)).Add(10.ToFix()));
			Space.Add(new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), (boxCount.ToFix().Mul(boxSize)).Add(20.ToFix()), 1.ToFix(),
							  platformLength));

			for (int i = 0; i < boxCount; i++)
			{
				for (int j = 0; j < boxCount - i; j++)
				{
					Space.Add(new Box(
								  new Vector3(
((((-boxCount).ToFix().Mul(boxSize)).Div(2.ToFix())).Add((boxSize.Div(2.ToFix())).Mul(i.ToFix()))).Add(j.ToFix().Mul((boxSize))),
(boxSize.Div(2.ToFix())).Add(i.ToFix().Mul(boxSize)),
									  0.ToFix()),
								  boxSize, boxSize, boxSize, 20.ToFix()));
				}
			}
			//Down here are the 'destructors' used to blow up the pyramid.

			Sphere pow = new Sphere(new Vector3((-25).ToFix(), 5.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
			pow = new Sphere(new Vector3((-15).ToFix(), 10.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
			pow = new Sphere(new Vector3((-5).ToFix(), 15.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
			pow = new Sphere(new Vector3(5.ToFix(), 15.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
			pow = new Sphere(new Vector3(15.ToFix(), 10.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
			pow = new Sphere(new Vector3(25.ToFix(), 5.ToFix(), 70.ToFix()), 2.ToFix(), 40.ToFix());
			pow.LinearVelocity = new Vector3(0.ToFix(), 10.ToFix(), (-100).ToFix());
			Space.Add(pow);
		}
	}
}
