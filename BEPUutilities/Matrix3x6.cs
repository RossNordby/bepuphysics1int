
using System;
using System.Runtime.InteropServices;

namespace BEPUutilities
{
	static class Matrix3x6
	{
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

		public static bool Invert(ref Matrix3x3 m, out Matrix3x3 r) {
			unsafe {
				var M = new Fix32_18();

				// Initialize temporary matrix
				M.f0 = m.M11;
				M.f3 = m.M12;
				M.f6 = m.M13;
				M.f1 = m.M21;
				M.f4 = m.M22;
				M.f7 = m.M23;
				M.f2 = m.M31;
				M.f5 = m.M32;
				M.f8 = m.M33;

				M.f9 = Fix32.One;
				M.f12 = Fix32.Zero;
				M.f15 = Fix32.Zero;
				M.f10 = Fix32.Zero;
				M.f13 = Fix32.One;
				M.f16 = Fix32.Zero;
				M.f11 = Fix32.Zero;
				M.f14 = Fix32.Zero;
				M.f17 = Fix32.One;

				if (!Gauss_3_6(ref M)) {
					r = new Matrix3x3();
					return false;
				}
				r = new Matrix3x3(
					// m11...m13
					M.f9,
					M.f12,
					M.f15,

					// m21...m23
					M.f10,
					M.f13,
					M.f16,

					// m31...m33
					M.f11,
					M.f14,
					M.f17
					);
				return true;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		unsafe struct Fix32_18 {
			[FieldOffset(0)]
			public fixed int f[18];
			[FieldOffset(sizeof(int) * 0)]
			public Fix32 f0;
			[FieldOffset(sizeof(int) * 1)]
			public Fix32 f1;
			[FieldOffset(sizeof(int) * 2)]
			public Fix32 f2;
			[FieldOffset(sizeof(int) * 3)]
			public Fix32 f3;
			[FieldOffset(sizeof(int) * 4)]
			public Fix32 f4;
			[FieldOffset(sizeof(int) * 5)]
			public Fix32 f5;
			[FieldOffset(sizeof(int) * 6)]
			public Fix32 f6;
			[FieldOffset(sizeof(int) * 7)]
			public Fix32 f7;
			[FieldOffset(sizeof(int) * 8)]
			public Fix32 f8;
			[FieldOffset(sizeof(int) * 9)]
			public Fix32 f9;
			[FieldOffset(sizeof(int) * 10)]
			public Fix32 f10;
			[FieldOffset(sizeof(int) * 11)]
			public Fix32 f11;
			[FieldOffset(sizeof(int) * 12)]
			public Fix32 f12;
			[FieldOffset(sizeof(int) * 13)]
			public Fix32 f13;
			[FieldOffset(sizeof(int) * 14)]
			public Fix32 f14;
			[FieldOffset(sizeof(int) * 15)]
			public Fix32 f15;
			[FieldOffset(sizeof(int) * 16)]
			public Fix32 f16;
			[FieldOffset(sizeof(int) * 17)]
			public Fix32 f17;
		}

		/// <summary>
		/// Unrolled gauss for m=3 and n=6
		/// </summary>
		unsafe static bool Gauss_3_6(ref Fix32_18 M) {
			// Perform Gauss-Jordan elimination
			
			Fix32 maxValue = M.f0.Abs();
			int iMax = 0;
			
			Fix32 v1 = M.f1.Abs();
			Fix32 v2 = M.f2.Abs();

			Fix32 maxValue2;
			int iMax2;
			if (v1 > v2) {
				maxValue2 = v1;
				iMax2 = 1;
			}
			else {
				maxValue2 = v2;
				iMax2 = 2;
			}
			
			if (maxValue2 > maxValue) {
				maxValue = maxValue2;
				iMax = iMax2;


				if (maxValue == F64.C0) return false;

				var copy = M;
				// Swap rows 0, iMax
				M.f0 = (Fix32) copy.f[iMax];
				M.f[iMax] = (int) copy.f0;
				
				M.f3 = (Fix32) copy.f[iMax + 3];
				M.f[iMax + 3] = (int) copy.f3;

				M.f6 = (Fix32) copy.f[iMax + 6];
				M.f[iMax + 6] = (int) copy.f6;

				M.f9 = (Fix32) copy.f[iMax + 9];
				M.f[iMax + 9] = (int) copy.f9;

				M.f12 = (Fix32) copy.f[iMax + 12];
				M.f[iMax + 12] = (int) copy.f12;

				M.f15 = (Fix32) copy.f[iMax + 15];
				M.f[iMax + 15] = (int) copy.f15;
			}
			else {
				if (maxValue == F64.C0) return false;
			}


			// Divide row by pivot
			Fix32 pivotInverse = F64.C1.Div(M.f0);

			M.f[0] = (int) F64.C1;
			for (int j = 0 + 1; j < 6; j++) {
				ref var t = ref M.f[0 + j * 3];
				t = (int) (((Fix32) t).Mul(pivotInverse));
			}

			// Subtract row 0 from other rows
			for (int i = 1; i < 3; i++) {
				ref var f = ref M.f[i];
				for (int j = 0 + 1; j < 6; j++) {
					ref var t = ref M.f[i + j * 3];
					t = (int) ((Fix32) t).Sub(((Fix32) M.f[0 + j * 3]).Mul((Fix32) f));
				}
				f = (int) F64.C0;
			}














			int k = 1;
			maxValue = ((Fix32) M.f[k + k * 3]).Abs();
			iMax = k;
			for (int i = k + 1; i < 3; i++) {
				Fix32 value = ((Fix32) M.f[i + k * 3]).Abs();
				if (value >= maxValue) {
					maxValue = value;
					iMax = i;
				}
			}
			if (maxValue == F64.C0)
				return false;
			// Swap rows k, iMax
			if (k != iMax) {
				for (int j = 0; j < 6; j++) {
					var temp = M.f[k + j * 3];
					M.f[k + j * 3] = M.f[iMax + j * 3];
					M.f[iMax + j * 3] = temp;
				}
			}

			// Divide row by pivot
			pivotInverse = F64.C1.Div((Fix32) M.f[k + k * 3]);

			M.f[k + k * 3] = (int) F64.C1;
			for (int j = k + 1; j < 6; j++) {
				ref var t = ref M.f[k + j * 3];
				t = (int) ((Fix32) t).Mul(pivotInverse);
			}

			// Subtract row k from other rows
			for (int i = 0; i < 3; i++) {
				if (i == k)
					continue;
				ref var f = ref M.f[i + k * 3];
				for (int j = k + 1; j < 6; j++) {
					ref var t = ref M.f[i + j * 3];
					t = (int) ((Fix32) t).Sub(((Fix32) M.f[k + j * 3]).Mul((Fix32) f));
				}
				f = (int) F64.C0;
			}






			k = 2;
			maxValue = ((Fix32) M.f[k + k * 3]).Abs();
			iMax = k;
			for (int i = k + 1; i < 3; i++) {
				Fix32 value = ((Fix32) M.f[i + k * 3]).Abs();
				if (value >= maxValue) {
					maxValue = value;
					iMax = i;
				}
			}
			if (maxValue == F64.C0)
				return false;
			// Swap rows k, iMax
			if (k != iMax) {
				for (int j = 0; j < 6; j++) {
					var temp = M.f[k + j * 3];
					M.f[k + j * 3] = M.f[iMax + j * 3];
					M.f[iMax + j * 3] = temp;
				}
			}

			// Divide row by pivot
			pivotInverse = F64.C1.Div((Fix32) M.f[k + k * 3]);

			M.f[k + k * 3] = (int) F64.C1;
			for (int j = k + 1; j < 6; j++) {
				ref var t = ref M.f[k + j * 3];
				t = (int) ((Fix32) t).Mul(pivotInverse);
			}

			// Subtract row k from other rows
			for (int i = 0; i < 3; i++) {
				if (i == k)
					continue;
				ref var f = ref M.f[i + k * 3];
				for (int j = k + 1; j < 6; j++) {
					ref var t = ref M.f[i + j * 3];
					t = (int) ((Fix32) t).Sub(((Fix32) M.f[k + j * 3]).Mul((Fix32) f));
				}
				f = (int) F64.C0;
			}

			return true;
		}

	}
}
