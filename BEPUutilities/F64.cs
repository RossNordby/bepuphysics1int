

namespace BEPUutilities
{
#pragma warning disable F64_NUM, CS1591
	public static class F64
	{
		public static readonly Fix32 C0 = 0.ToFix();
		public static readonly Fix32 C1 = 1.ToFix();
		public static readonly Fix32 C180 = 180.ToFix();
		public static readonly Fix32 C2 = 2.ToFix();
		public static readonly Fix32 C3 = 3.ToFix();
		public static readonly Fix32 C5 = 5.ToFix();
		public static readonly Fix32 C6 = 6.ToFix();
		public static readonly Fix32 C16 = 16.ToFix();
		public static readonly Fix32 C24 = 24.ToFix();
		public static readonly Fix32 C50 = 50.ToFix();
		public static readonly Fix32 C60 = 60.ToFix();
		public static readonly Fix32 C120 = 120.ToFix();
		public static readonly Fix32 C0p001 = 0.001m.ToFix();
		public static readonly Fix32 C0p5 = 0.5m.ToFix();
		public static readonly Fix32 C0p25 = 0.25m.ToFix();
		public static readonly Fix32 C1em09 = 1e-9m.ToFix();
		public static readonly Fix32 C1em9 = 1e-9m.ToFix();
		public static readonly Fix32 Cm1em9 = (-1e-9m).ToFix();
		public static readonly Fix32 C1em14 = (1e-14m).ToFix();		
		public static readonly Fix32 C0p1 = 0.1m.ToFix();
		public static readonly Fix32 OneThird = (1.ToFix()).Div(3.ToFix());
		public static readonly Fix32 C0p75 = 0.75m.ToFix();
		public static readonly Fix32 C0p15 = 0.15m.ToFix();
		public static readonly Fix32 C0p3 = 0.3m.ToFix();
		public static readonly Fix32 C0p0625 = 0.0625m.ToFix();
		public static readonly Fix32 C0p99 = .99m.ToFix();
		public static readonly Fix32 C0p9 = .9m.ToFix();
		public static readonly Fix32 C1p5 = 1.5m.ToFix();
		public static readonly Fix32 C1p1 = 1.1m.ToFix();
		public static readonly Fix32 OneEighth = Fix32.One.Div(8.ToFix());
		public static readonly Fix32 FourThirds = (4.ToFix()).Div(3.ToFix());
		public static readonly Fix32 TwoFifths = (2.ToFix()).Div(5.ToFix());
		public static readonly Fix32 C0p2 = 0.2m.ToFix();
		public static readonly Fix32 C0p8 = 0.8m.ToFix();
		public static readonly Fix32 C0p01 = 0.01m.ToFix();
		public static readonly Fix32 C1em7 = 1e-7m.ToFix();
		public static readonly Fix32 C1em5 = 1e-5m.ToFix();
		public static readonly Fix32 C1em4 = 1e-4m.ToFix();
		public static readonly Fix32 C1em10 = 1e-10m.ToFix();
		public static readonly Fix32 Cm0p25 = (-0.25m).ToFix();
		public static readonly Fix32 Cm0p9999 = (-0.9999m).ToFix();
		public static readonly Fix32 C1m1em12 = Fix32.One.Sub(1e-12m.ToFix());
		public static readonly Fix32 GoldenRatio = Fix32.One.Add(Fix32Ext.Sqrt(5.ToFix()).Div(2.ToFix()));
		public static readonly Fix32 OneTwelfth = Fix32.One.Div(12.ToFix());
		public static readonly Fix32 C0p0833333333 = .0833333333m.ToFix();
		public static readonly Fix32 C90000 = 90000.ToFix();
		public static readonly Fix32 C600000 = 600000.ToFix();
	}
}
