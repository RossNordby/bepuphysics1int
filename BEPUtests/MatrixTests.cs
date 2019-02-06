
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using BEPUutilities;
using FloatMatrix = BEPUutilitiesFloat.Matrix;
using BEPUtests.util;
using System.Linq;
using System.Diagnostics;
using System;

namespace BEPUtests
{
	public class MatrixTests
    {
		Matrix[] testCases = {
			Matrix.Identity,
			new Matrix(6770.833m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 13500.ToFix(), 0.ToFix(), 0.ToFix(),0.ToFix(), 0.ToFix(), 6770.833m.ToFix(), 0.ToFix(), 0.ToFix(),0.ToFix(),0.ToFix(), 20000m.ToFix()),
			new Matrix(0.6770833m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 1.35m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(),0.ToFix(), 0.6770833m.ToFix(),  0.ToFix(), 0.ToFix(),0.ToFix(),0.ToFix(), 2m.ToFix()),
			new Matrix(0.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(), 1.35m.ToFix(), 0.ToFix(), 0.ToFix(), 0.ToFix(),0.ToFix(), 0.6770833m.ToFix(),  0.ToFix(), 0.ToFix(),0.ToFix(),0.ToFix(), 2m.ToFix()),

			new Matrix(5.ToFix(), 135.ToFix(), (-5).ToFix(), 8.ToFix(), 13500.ToFix(), 20.ToFix(), (-5).ToFix(), 100.ToFix(), 6770.833m.ToFix(), 2.ToFix(), 5.ToFix(), (-3).ToFix(), 10.ToFix(), 0.1m.ToFix(), 15.ToFix(), 2000.ToFix()),
			new Matrix(0.1m.ToFix(), 3.ToFix(), 838.ToFix(), (-200).ToFix(), 13500.ToFix(), 0.001m.ToFix(), 22.ToFix(), 42.ToFix(), 6770.833m.ToFix(), 5.ToFix(), (-100).ToFix(), 3000.ToFix(), 0.001m.ToFix(), 10.ToFix(), 11.ToFix(), 42.ToFix()),
			new Matrix((-3).ToFix(), 3.ToFix(), 2.ToFix(), (-1).ToFix(), (-8).ToFix(), (-5).ToFix(), 63.ToFix(), 5.ToFix(), 0.833m.ToFix(), (-1).ToFix(), (-1).ToFix(), (-1).ToFix(), (-2).ToFix(), 5.ToFix(), 3.ToFix(), 3.14m.ToFix()),
			new Matrix(5.ToFix(), 3.ToFix(), 2.ToFix(), (-3).ToFix(), 11.ToFix(), 1900.ToFix(), 76.ToFix(), 96.ToFix(), 33.833m.ToFix(), 1.ToFix(), 2.ToFix(), 3.ToFix(), 4.ToFix(), 5.ToFix(),6.ToFix(), 7.ToFix()),

		};

		private readonly ITestOutputHelper output;
		
		public MatrixTests(ITestOutputHelper output)
		{
			if (output == null)
				output = new ConsoleTestOutputHelper();
			this.output = output;
		}


		[Fact]
		public void Invert()
		{
			var maxDelta = 0.00001m;

			var deltas = new List<decimal>();

			// Scalability and edge cases
			foreach (var m in testCases)
			{
				Matrix testCase = m;

				FloatMatrix floatMatrix = MathConverter.Convert(testCase);
				FloatMatrix expected;
				FloatMatrix.Invert(ref floatMatrix, out expected);

				Matrix actual;
				if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
					expected = new FloatMatrix();

				Matrix.Invert(ref testCase, out actual);
				bool success = true;
				foreach (decimal delta in GetDeltas(expected, actual))
				{
					deltas.Add(delta);
					success &= delta <= maxDelta;

				}
				Assert.True(success, string.Format("Precision: Matrix3x3Invert({0}): Expected {1} Actual {2}", testCase, expected, actual));
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (decimal) Fix32Ext.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (decimal) Fix32Ext.Precision);
		}

		[Fact]
		public void BenchmarkInvert()
		{
			var swf = new Stopwatch();
			var swd = new Stopwatch();

			var deltas = new List<decimal>();

			foreach (var m in testCases)
			{
				Matrix testCase = m;

				for (int i = 0; i < 10000; i++)
				{
					FloatMatrix floatMatrix = MathConverter.Convert(testCase);
					FloatMatrix expected;
					swf.Start();
					FloatMatrix.Invert(ref floatMatrix, out expected);
					swf.Stop();

					Matrix actual;
					swd.Start();
					Matrix.Invert(ref testCase, out actual);
					swd.Stop();

					if (float.IsInfinity(expected.M11) || float.IsNaN(expected.M11))
						expected = new FloatMatrix();

					foreach (decimal delta in GetDeltas(expected, actual))
						deltas.Add(delta);
				}
			}
			output.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (decimal) Fix32Ext.Precision);
			output.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (decimal) Fix32Ext.Precision);
			output.WriteLine("Fix32.Invert time = {0}ms, float.Invert time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
		}		

		decimal[] GetDeltas(FloatMatrix expected, Matrix actual)
		{
			decimal[] result = new decimal[16];
			int i = 0;
			result[i++] = (decimal)actual.M11.ToDouble() - (decimal)expected.M11;
			result[i++] = (decimal)actual.M12.ToDouble() - (decimal)expected.M12;
			result[i++] = (decimal)actual.M13.ToDouble() - (decimal)expected.M13;
			result[i++] = (decimal)actual.M14.ToDouble() - (decimal)expected.M14;

			result[i++] = (decimal)actual.M21.ToDouble() - (decimal)expected.M21;
			result[i++] = (decimal)actual.M22.ToDouble() - (decimal)expected.M22;
			result[i++] = (decimal)actual.M23.ToDouble() - (decimal)expected.M23;
			result[i++] = (decimal)actual.M24.ToDouble() - (decimal)expected.M24;

			result[i++] = (decimal)actual.M31.ToDouble() - (decimal)expected.M31;
			result[i++] = (decimal)actual.M32.ToDouble() - (decimal)expected.M32;
			result[i++] = (decimal)actual.M33.ToDouble() - (decimal)expected.M33;
			result[i++] = (decimal)actual.M34.ToDouble() - (decimal)expected.M34;

			result[i++] = (decimal)actual.M41.ToDouble() - (decimal)expected.M41;
			result[i++] = (decimal)actual.M42.ToDouble() - (decimal)expected.M42;
			result[i++] = (decimal)actual.M43.ToDouble() - (decimal)expected.M43;
			result[i++] = (decimal)actual.M44.ToDouble() - (decimal)expected.M44;

			return result;
		}
	}
}
