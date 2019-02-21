using BEPUfloatBenchmark.Benchmarks;

namespace BEPUfloatBenchmark
{
	public static class AllBenchmarks
	{
		public static Benchmark[] Benchmarks = { new DiscreteVsContinuousBenchmark(), new PathFollowingBenchmark(), new SelfCollidingClothBenchmark(), new PyramidBenchmark() };
	}
}
