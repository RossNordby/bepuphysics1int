
using System;
using System.Threading;

namespace BEPUutilities
{
	static class Matrix3x6
	{
		[ThreadStatic] private static Fix32[,] Matrix;

		public static bool Gauss(Fix32[,] M, int m, int n)
		{
			// Perform Gauss-Jordan elimination
			for (int k = 0; k < m; k++)
			{
				Fix32 maxValue = Fix32Ext.Abs(M[k, k]);
				int iMax = k;
				for (int i = k+1; i < m; i++)
				{
					Fix32 value = Fix32Ext.Abs(M[i, k]);
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
						Fix32 temp = M[k, j];
						M[k, j] = M[iMax, j];
						M[iMax, j] = temp;
					}
				}

				// Divide row by pivot
				Fix32 pivot = M[k, k];

				M[k, k] = F64.C1;
				for (int j = k + 1; j < n; j++)
				{
					M[k, j] = M[k, j].Div(pivot);
				}

				// Subtract row k from other rows
				for (int i = 0; i < m; i++)
				{
					if (i == k)
						continue;
					Fix32 f = M[i, k];					
					for (int j = k + 1; j < n; j++)
					{
						M[i, j] = M[i, j].Sub(M[k, j].Mul(f));
					}
					M[i, k] = F64.C0;
				}
			}
			return true;
		}
		
		public static bool Invert(ref Matrix3x3 m, out Matrix3x3 r)
		{
			if (Matrix == null)
				 Matrix = new Fix32[3, 6];
			Fix32[,] M = Matrix;

			// Initialize temporary matrix
			M[0, 0] = m.M11;
			M[0, 1] = m.M12;
			M[0, 2] = m.M13;
			M[1, 0] = m.M21;
			M[1, 1] = m.M22;
			M[1, 2] = m.M23;
			M[2, 0] = m.M31;
			M[2, 1] = m.M32;
			M[2, 2] = m.M33;

			M[0, 3] = Fix32.One;
			M[0, 4] = Fix32.Zero;
			M[0, 5] = Fix32.Zero;
			M[1, 3] = Fix32.Zero;
			M[1, 4] = Fix32.One;
			M[1, 5] = Fix32.Zero;
			M[2, 3] = Fix32.Zero;
			M[2, 4] = Fix32.Zero;
			M[2, 5] = Fix32.One;

			if (!Gauss(M, 3, 6))
			{
				r = new Matrix3x3();
				return false;
			}
			r = new Matrix3x3(
				// m11...m13
				M[0, 3],
				M[0, 4],
				M[0, 5],

				// m21...m23
				M[1, 3],
				M[1, 4],
				M[1, 5],

				// m31...m33
				M[2, 3],
				M[2, 4],
				M[2, 5]
				);
			return true;
		}
	}
}
