namespace BEPUutilities
{
	public static class F64
	{
		public const Fix C180 = (Fix) ((int) Fix.One * 180);
		public const Fix C100 = (Fix) ((int) Fix.One * 100);
		public const Fix C0 = Fix.Zero;
		public const Fix C1 = Fix.One;
		public const Fix C2 = (Fix) ((int) Fix.One * 2);
		public const Fix C3 = (Fix) ((int) Fix.One * 3);
		public const Fix C5 = (Fix) ((int) Fix.One * 5);
		public const Fix C6 = (Fix) ((int) Fix.One * 6);
		public const Fix C16 = (Fix) ((int) Fix.One * 16);
		public const Fix C24 = (Fix) ((int) Fix.One * 24);
		public const Fix C50 = (Fix) ((int) Fix.One * 50);
		public const Fix C60 = (Fix) ((int) Fix.One * 60);
		public const Fix C120 = (Fix) ((int) Fix.One * 120);
		public static readonly Fix C0p001 = 0.001m.ToFix();
		public const Fix C0p5 = Fix.Half;
		public const Fix C0p25 = Fix.Fourth;
		public static readonly Fix C1em9 = (1e-4m).ToFix();
		public static readonly Fix Cm1em9 = (-1e-4m).ToFix();
		public static readonly Fix C1em14 = (1e-4m).ToFix();		
		public static readonly Fix C0p1 = 0.1m.ToFix();
		public const Fix OneThird = Fix.Third;
		public const Fix C0p75 = (Fix) (((int) Fix.One * 3) / 4);
		public static readonly Fix C0p15 = 0.15m.ToFix();
		public static readonly Fix C0p3 = 0.3m.ToFix();
		public const Fix C0p0625 = (Fix) ((int) Fix.One / 16);
		public static readonly Fix C0p99 = .99m.ToFix();
		public static readonly Fix C0p9 = .9m.ToFix();
		public const Fix C1p5 = (Fix) ((int) Fix.One + (int) Fix.Half);
		public static readonly Fix C1p1 = 1.1m.ToFix();
        public const Fix Half = Fix.Half;
        public const Fix OneEighth = (Fix) ((int) Fix.One / 8);
		public static readonly Fix FourThirds = (4.0 / 3).ToFix();
		public static readonly Fix FourThirdsTimesPI = FourThirds.Mul(MathHelper.Pi);
		public static readonly Fix TwoFifths = (2.0 / 5).ToFix();
		public static readonly Fix C0p2 = 0.2m.ToFix();
		public static readonly Fix C0p8 = 0.8m.ToFix();
		public static readonly Fix C0p01 = 0.01m.ToFix();
		public static readonly Fix C1em7 = 1e-4m.ToFix();
		public static readonly Fix C1em5 = 1e-4m.ToFix();
		public static readonly Fix C1em4 = 1e-4m.ToFix();
		public static readonly Fix C1em10 = 1e-4m.ToFix();
		public static readonly Fix Cm0p25 = (-0.25m).ToFix();
		public static readonly Fix Cm0p9999 = (-0.9999m).ToFix();
		public static readonly Fix C1m1em12 = Fix.One.Sub(1e-4m.ToFix());
		public static readonly Fix GoldenRatio = Fix.One.Add(5.ToFix().Div(2.ToFix()).SqrtSlow());
		public static readonly Fix OneTwelfth = Fix.One.Div(12.ToFix());
		public static readonly Fix C0p0833333333 = .0833333333m.ToFix();
		public static readonly Fix C90000 = 16000.ToFix();
		public static readonly Fix C600000 = 16000.ToFix();
	}
}
