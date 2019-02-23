using BEPUbenchmark;
using BEPUbenchmark.Benchmarks;
using NUnit.Framework;
using System;
using System.Text;

namespace BEPUtests
{
	public class CrossPlatformDeterminismTests
    {
		private void GetExpectedHash(Benchmark b, StringBuilder result)
		{
			result.AppendFormat("{{\"{0}\", new string[] {{", b.GetName());

			b.Initialize();
			long startTime = DateTime.Now.Ticks;
			for (int i = 0; i < 50; i++)
			{
				string expectedHash = b.RunToNextHash();
				if (i != 0)
					result.Append(",\n");
				result.AppendFormat("\"{0}\"", expectedHash);

				if (i > 5 && DateTime.Now.Ticks - startTime > TimeSpan.TicksPerSecond * 5)
					break;
			}
			result.AppendLine("}}");
		}

		//[Test]
		public void OutputExpectedHashes()
		{
			StringBuilder result = new StringBuilder();

			result.AppendLine(@"
using System.Collections.Generic;
namespace BEPUtests
{
class CrossPlatformDeterminismExpectedHashes
{
public static readonly Dictionary<string, string[]> Hashes = new Dictionary<string, string[]>()
{");

			bool first = true;
			foreach (Benchmark b in AllBenchmarks.Benchmarks)
			{
				if (!first)
					result.Append(",\n");
				GetExpectedHash(b, result);
				first = false;
			}
			result.AppendLine("};\n}\n}\n");
			Console.WriteLine(result.ToString());
		}
		
		//[Test]
		public void OutputExpectedHashesForBenchmark()
		{
			Benchmark b = new PathFollowingBenchmark();

			StringBuilder result = new StringBuilder();

			GetExpectedHash(b, result);
			Console.WriteLine(result.ToString());
		}

		[Test]
		public void DiscreteVsContinuous()
		{
			TestDeterminism(new DiscreteVsContinuousBenchmark());
		}

		[Test]
		public void Pyramid()
		{
			TestDeterminism(new PyramidBenchmark());
		}

		[Test]
		public void PathFollowing()
		{
			TestDeterminism(new PathFollowingBenchmark());
		}

		[Test]
		public void SelfCollidingCloth()
		{
			TestDeterminism(new SelfCollidingClothBenchmark());
		}

		private void TestDeterminism(Benchmark b)
		{
			b.Initialize();
			string[] expectedHashes = CrossPlatformDeterminismExpectedHashes.Hashes[b.GetName()];
			int step = 0;
			foreach (string expectedHash in expectedHashes)
			{
				string actualHash = b.RunToNextHash();
				Assert.True(expectedHash == actualHash, string.Format("Expected {0}, actual {1} in step {2}", expectedHash, actualHash, step));
				step++;
			}
		}
	}
}
