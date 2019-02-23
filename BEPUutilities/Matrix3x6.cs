
using System;

namespace BEPUutilities
{
	static class Matrix3x6
	{
		[ThreadStatic] private static Fix32[] Matrix;

		public static bool Gauss(Fix32[] M, int m, int n)
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < m; k++)
			{
				Fix32 maxValue = M[k + k * m].Abs();
				int iMax = k;
				for (int i = k+1; i < m; i++)
				{
					Fix32 value = M[i + k * m].Abs();
					if (value >= maxValue)
					{
						maxValue = value;
						iMax = i;
					}
				}
				if (maxValue == F64.C0)
					return false;
				// Swap rows k, iMax
				if (k != iMax)
				{
					for (int j = 0; j < n; j++)
					{
						Fix32 temp = M[k + j * m];
						M[k + j * m] = M[iMax + j * m];
						M[iMax + j * m] = temp;
					}
				}

				// Divide row by pivot
				Fix32 pivotInverse = F64.C1.Div(M[k + k * m]);

				M[k + k * m] = F64.C1;
				for (int j = k + 1; j < n; j++)
				{
					ref var t = ref M[k + j * m];
					t = t.Mul(pivotInverse);
				}

				// Subtract row k from other rows
				for (int i = 0; i < m; i++)
				{
					if (i == k)
						continue;
					ref Fix32 f = ref M[i + k * m];					
					for (int j = k + 1; j < n; j++)
					{
						ref var t = ref M[i + j * m];
						t = t.Sub(M[k + j * m].Mul(f));
					}
					f = F64.C0;
				}
			}
			return true;
		}
		
		public static bool Invert(ref Matrix3x3 m, out Matrix3x3 r)
		{
			if (Matrix == null)
				 Matrix = new Fix32[3 + 6 * 3];
			Fix32[] M = Matrix;

			// Initialize temporary matrix
			M[0 + 0 * 3] = m.M11;
			M[0 + 1 * 3] = m.M12;
			M[0 + 2 * 3] = m.M13;
			M[1 + 0 * 3] = m.M21;
			M[1 + 1 * 3] = m.M22;
			M[1 + 2 * 3] = m.M23;
			M[2 + 0 * 3] = m.M31;
			M[2 + 1 * 3] = m.M32;
			M[2 + 2 * 3] = m.M33;

			M[0 + 3 * 3] = Fix32.One;
			M[0 + 4 * 3] = Fix32.Zero;
			M[0 + 5 * 3] = Fix32.Zero;
			M[1 + 3 * 3] = Fix32.Zero;
			M[1 + 4 * 3] = Fix32.One;
			M[1 + 5 * 3] = Fix32.Zero;
			M[2 + 3 * 3] = Fix32.Zero;
			M[2 + 4 * 3] = Fix32.Zero;
			M[2 + 5 * 3] = Fix32.One;

			if (!Gauss(M, 3, 6))
			{
				r = new Matrix3x3();
				return false;
			}
			r = new Matrix3x3(
				// m11...m13
				M[0 + 3 * 3],
				M[0 + 4 * 3],
				M[0 + 5 * 3],

				// m21...m23
				M[1 + 3 * 3],
				M[1 + 4 * 3],
				M[1 + 5 * 3],

				// m31...m33
				M[2 + 3 * 3],
				M[2 + 4 * 3],
				M[2 + 5 * 3]
				);
			return true;
		}
	}
}
