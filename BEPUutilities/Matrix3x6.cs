
using System;

namespace BEPUutilities
{
	static class Matrix3x6
	{
		[ThreadStatic] private static Fix[] Matrix;

		public static bool Gauss(Fix[] M, int m, int n)
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < m; k++)
			{
				Fix maxValue = M[k + k * m].Abs();
				int iMax = k;
				for (int i = k+1; i < m; i++)
				{
					Fix value = M[i + k * m].Abs();
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
						Fix temp = M[k + j * m];
						M[k + j * m] = M[iMax + j * m];
						M[iMax + j * m] = temp;
					}
				}

				// Divide row by pivot
				Fix pivotInverse = F64.C1.Div(M[k + k * m]);

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
					ref Fix f = ref M[i + k * m];					
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
				 Matrix = new Fix[3 + 6 * 3];
			Fix[] M = Matrix;

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

			M[0 + 3 * 3] = Fix.One;
			M[0 + 4 * 3] = Fix.Zero;
			M[0 + 5 * 3] = Fix.Zero;
			M[1 + 3 * 3] = Fix.Zero;
			M[1 + 4 * 3] = Fix.One;
			M[1 + 5 * 3] = Fix.Zero;
			M[2 + 3 * 3] = Fix.Zero;
			M[2 + 4 * 3] = Fix.Zero;
			M[2 + 5 * 3] = Fix.One;

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
