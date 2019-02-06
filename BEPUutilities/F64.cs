using FixMath.NET;

namespace BEPUutilities
{
#pragma warning disable F64_NUM, CS1591
	public static class F64
	{
		public static readonly Fix64 C0 = (Fix64)0.ToFix();
		public static readonly Fix64 C1 = (Fix64)1.ToFix();
		public static readonly Fix64 C180 = (Fix64)180.ToFix();
		public static readonly Fix64 C2 = (Fix64)2.ToFix();
		public static readonly Fix64 C3 = (Fix64)3.ToFix();
		public static readonly Fix64 C5 = (Fix64)5.ToFix();
		public static readonly Fix64 C6 = (Fix64)6.ToFix();
		public static readonly Fix64 C16 = (Fix64)16.ToFix();
		public static readonly Fix64 C24 = (Fix64)24.ToFix();
		public static readonly Fix64 C50 = (Fix64)50.ToFix();
		public static readonly Fix64 C60 = (Fix64)60.ToFix();
		public static readonly Fix64 C120 = (Fix64)120.ToFix();
		public static readonly Fix64 C0p001 = (Fix64)0.001m.ToFix();
		public static readonly Fix64 C0p5 = (Fix64)0.5m.ToFix();
		public static readonly Fix64 C0p25 = (Fix64)0.25m.ToFix();
		public static readonly Fix64 C1em09 = (Fix64)1e-9m.ToFix();
		public static readonly Fix64 C1em9 = (Fix64)1e-9m.ToFix();
		public static readonly Fix64 Cm1em9 = (Fix64)(-1e-9m).ToFix();
		public static readonly Fix64 C1em14 = (Fix64)(1e-14m).ToFix();		
		public static readonly Fix64 C0p1 = (Fix64)0.1m.ToFix();
		public static readonly Fix64 OneThird = ((Fix64)1.ToFix()).Div((Fix64)3.ToFix());
		public static readonly Fix64 C0p75 = (Fix64)0.75m.ToFix();
		public static readonly Fix64 C0p15 = (Fix64)0.15m.ToFix();
		public static readonly Fix64 C0p3 = (Fix64)0.3m.ToFix();
		public static readonly Fix64 C0p0625 = (Fix64)0.0625m.ToFix();
		public static readonly Fix64 C0p99 = (Fix64).99m.ToFix();
		public static readonly Fix64 C0p9 = (Fix64).9m.ToFix();
		public static readonly Fix64 C1p5 = (Fix64)1.5m.ToFix();
		public static readonly Fix64 C1p1 = (Fix64)1.1m.ToFix();
		public static readonly Fix64 OneEighth = Fix64.One.Div(8.ToFix());
		public static readonly Fix64 FourThirds = new Fix64(4).Div(3.ToFix());
		public static readonly Fix64 TwoFifths = new Fix64(2).Div(5.ToFix());
		public static readonly Fix64 C0p2 = (Fix64)0.2m.ToFix();
		public static readonly Fix64 C0p8 = (Fix64)0.8m.ToFix();
		public static readonly Fix64 C0p01 = (Fix64)0.01m.ToFix();
		public static readonly Fix64 C1em7 = (Fix64)1e-7m.ToFix();
		public static readonly Fix64 C1em5 = (Fix64)1e-5m.ToFix();
		public static readonly Fix64 C1em4 = (Fix64)1e-4m.ToFix();
		public static readonly Fix64 C1em10 = (Fix64)1e-10m.ToFix();
		public static readonly Fix64 Cm0p25 = (Fix64)(-0.25m).ToFix();
		public static readonly Fix64 Cm0p9999 = (Fix64)(-0.9999m).ToFix();
		public static readonly Fix64 C1m1em12 = Fix64.One.Sub((Fix64)1e-12m.ToFix());
		public static readonly Fix64 GoldenRatio = Fix64.One.Add(Fix64.Sqrt((Fix64)5.ToFix()).Div((Fix64)2.ToFix()));
		public static readonly Fix64 OneTwelfth = Fix64.One.Div((Fix64)12.ToFix());
		public static readonly Fix64 C0p0833333333 = (Fix64).0833333333m.ToFix();
		public static readonly Fix64 C90000 = (Fix64)90000.ToFix();
		public static readonly Fix64 C600000 = (Fix64)600000.ToFix();
	}
}
