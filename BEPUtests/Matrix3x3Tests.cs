using System.Collections.Generic;
using BEPUutilities;
using FloatMatrix3x3 = BEPUutilitiesFloat.Matrix3x3;
using BEPUtests.util;
using System.Linq;
using System.Diagnostics;
using NUnit.Framework;
using System;

namespace BEPUtests
{
	public class Matrix3x3Tests
    {
		Matrix3x3[] testCases = {
			Matrix3x3.Identity,
			new Matrix3x3(6770.833m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 13500.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 6770.833m.ToFix()),
			new Matrix3x3(0.6770833m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 1.35m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 0.6770833m.ToFix()),
			new Matrix3x3(0.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 13500.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 6770.833m.ToFix()),

			new Matrix3x3(5.ToFix(), 135.ToFix(), (-5).ToFix(), 8.ToFix(), 13500.ToFix(), 20.ToFix(), (-5).ToFix(), 100.ToFix(), 6770.833m.ToFix()),
			new Matrix3x3(0.1m.ToFix(), 3.ToFix(), 838.ToFix(), (-200).ToFix(), 13500.ToFix(), 0.001m.ToFix(), 22.ToFix(), 42.ToFix(), 6770.833m.ToFix()),
			new Matrix3x3((-3).ToFix(), 3.ToFix(), 2.ToFix(), (-1).ToFix(), (-8).ToFix(), (-5).ToFix(), 63.ToFix(), 5.ToFix(), 0.833m.ToFix()),
			new Matrix3x3(5.ToFix(), 3.ToFix(), 2.ToFix(), (-3).ToFix(), 11.ToFix(), 1900.ToFix(), 76.ToFix(), 96.ToFix(), 33.833m.ToFix()),

		};

		[Test]
		public void Invert()
		{
			var maxDelta = 0.001m;

			var deltas = new List<decimal>();

			// Scalability and edge cases
			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
				FloatMatrix3x3 expected;
				FloatMatrix3x3.Invert(ref floatMatrix, out expected);

				Matrix3x3 actual;
				if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
					expected = new FloatMatrix3x3();

				Matrix3x3.Invert(ref testCase, out actual);
				bool success = true;
				foreach (decimal delta in GetDeltas(expected, actual))
				{
					deltas.Add(delta);
					success &= delta <= maxDelta;

				}
				Assert.True(success, string.Format("Precision: Matrix3x3Invert({0}): Expected {1} Actual {2}", testCase, expected, actual));
			}
			Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (decimal) Fix32Ext.Precision);
			Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (decimal) Fix32Ext.Precision);
		}

		[Test]
		public void BenchmarkInvert()
		{
			Matrix3x3.Invert(ref testCases[0], out var _); // Prepare
			Fix32Tests.PrepareStatistics(out var deltas, out Stopwatch swFixed, out Stopwatch swFloat);
			
			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				for (int i = 0; i < 10000; i++)
				{
					FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
					FloatMatrix3x3 expected;
					swFloat.Start();
					FloatMatrix3x3.Invert(ref floatMatrix, out expected);
					swFloat.Stop();

					Matrix3x3 actual;
					swFixed.Start();
					Matrix3x3.Invert(ref testCase, out actual);
					swFixed.Stop();

					if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
						expected = new FloatMatrix3x3();

					foreach (var delta in GetDeltas(expected, actual))
						deltas.Add(delta);
				}
			}
			Console.WriteLine(Fix32Tests.GetStatisticsString(deltas, swFixed, swFloat));
		}

		[Test]
		public void AdaptiveInvert()
		{
			var maxDelta = 0.001m;

			var deltas = new List<decimal>();

			// Scalability and edge cases
			for (int i = 0; i < testCases.Length; i++)
			{
				Matrix3x3 m = testCases[i];
				Matrix3x3 testCase = m;

				FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
				FloatMatrix3x3 expected;
				FloatMatrix3x3.AdaptiveInvert(ref floatMatrix, out expected);
				
				Matrix3x3 actual;
				Matrix3x3.AdaptiveInvert(ref testCase, out actual);

				bool success = true;
				foreach (decimal delta in GetDeltas(expected, actual))
				{
					deltas.Add(delta);
					success &= delta <= maxDelta;
					
				}
				Assert.True(success, string.Format("Test case {0}\nPrecision: Matrix3x3Invert({1}):\nExpected:\n{2}\nActual:\n{3}", i, testCase, expected, actual));
			}
			Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (decimal) Fix32Ext.Precision);
			Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (decimal) Fix32Ext.Precision);
		}

		[Test]
		public void BenchmarkAdaptiveInvert()
		{
			Matrix3x3.AdaptiveInvert(ref testCases[0], out var _); // Prepare
			Fix32Tests.PrepareStatistics(out var deltas, out Stopwatch swFixed, out Stopwatch swFloat);

			foreach (var m in testCases)
			{
				Matrix3x3 testCase = m;

				for (int i = 0; i < 10000; i++)
				{
					FloatMatrix3x3 floatMatrix = MathConverter.Convert(testCase);
					swFloat.Start();
					FloatMatrix3x3.AdaptiveInvert(ref floatMatrix, out var expected);
					swFloat.Stop();


					swFixed.Start();
					Matrix3x3.AdaptiveInvert(ref testCase, out var actual);
					swFixed.Stop();

					foreach (var delta in GetDeltas(expected, actual))
						deltas.Add(delta);
				}
			}
			Console.WriteLine(Fix32Tests.GetStatisticsString(deltas, swFixed, swFloat));
		}

		double[] GetDeltas(FloatMatrix3x3 expected, Matrix3x3 actual)
		{
			double[] result = new double[9];
			int i = 0;
			result[i++] = actual.M11.ToDouble() - expected.M11;
			result[i++] = actual.M12.ToDouble() - expected.M12;
			result[i++] = actual.M13.ToDouble() - expected.M13;

			result[i++] = actual.M21.ToDouble() - expected.M21;
			result[i++] = actual.M22.ToDouble() - expected.M22;
			result[i++] = actual.M23.ToDouble() - expected.M23;

			result[i++] = actual.M31.ToDouble() - expected.M31;
			result[i++] = actual.M32.ToDouble() - expected.M32;
			result[i++] = actual.M33.ToDouble() - expected.M33;

			return result;
		}
	}
}
