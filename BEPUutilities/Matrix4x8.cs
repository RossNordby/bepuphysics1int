
using System;

namespace BEPUutilities
{
	static class Matrix4x8
	{
		[ThreadStatic] private static Fix[] Matrix;

		public static bool Invert(ref Matrix m, out Matrix r)
		{
			if (Matrix == null)
				Matrix = new Fix[4 + 8 * 4];
			Fix[] M = Matrix;

			M[0 + 0 * 4] = m.M11;
			M[0 + 1 * 4] = m.M12;
			M[0 + 2 * 4] = m.M13;
			M[0 + 3 * 4] = m.M14;
			M[1 + 0 * 4] = m.M21;
			M[1 + 1 * 4] = m.M22;
			M[1 + 2 * 4] = m.M23;
			M[1 + 3 * 4] = m.M24;
			M[2 + 0 * 4] = m.M31;
			M[2 + 1 * 4] = m.M32;
			M[2 + 2 * 4] = m.M33;
			M[2 + 3 * 4] = m.M34;
			M[3 + 0 * 4] = m.M41;
			M[3 + 1 * 4] = m.M42;
			M[3 + 2 * 4] = m.M43;
			M[3 + 3 * 4] = m.M44;

			M[0 + 4 * 4] = Fix.One;
			M[0 + 5 * 4] = Fix.Zero;
			M[0 + 6 * 4] = Fix.Zero;
			M[0 + 7 * 4] = Fix.Zero;
			M[1 + 4 * 4] = Fix.Zero;
			M[1 + 5 * 4] = Fix.One;
			M[1 + 6 * 4] = Fix.Zero;
			M[1 + 7 * 4] = Fix.Zero;
			M[2 + 4 * 4] = Fix.Zero;
			M[2 + 5 * 4] = Fix.Zero;
			M[2 + 6 * 4] = Fix.One;
			M[2 + 7 * 4] = Fix.Zero;
			M[3 + 4 * 4] = Fix.Zero;
			M[3 + 5 * 4] = Fix.Zero;
			M[3 + 6 * 4] = Fix.Zero;
			M[3 + 7 * 4] = Fix.One;


			if (!Matrix3x6.Gauss(M, 4, 8))
			{
				r = new Matrix();
				return false;
			}
			r = new Matrix(
				// m11...m14
				M[0 + 4 * 4],
				M[0 + 5 * 4],
				M[0 + 6 * 4],
				M[0 + 7 * 4],

				// m21...m24				
				M[1 + 4 * 4],
				M[1 + 5 * 4],
				M[1 + 6 * 4],
				M[1 + 7 * 4],

				// m31...m34
				M[2 + 4 * 4],
				M[2 + 5 * 4],
				M[2 + 6 * 4],
				M[2 + 7 * 4],

				// m41...m44
				M[3 + 4 * 4],
				M[3 + 5 * 4],
				M[3 + 6 * 4],
				M[3 + 7 * 4]
				);
			return true;
		}
	}
}
