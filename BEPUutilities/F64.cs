namespace BEPUutilities
{
	public static class F64
	{
		public const Fix32 C180 = (Fix32) ((int) Fix32.One * 180);
		public const Fix32 C100 = (Fix32) ((int) Fix32.One * 100);
		public const Fix32 C0 = Fix32.Zero;
		public const Fix32 C1 = Fix32.One;
		public const Fix32 C2 = (Fix32) ((int) Fix32.One * 2);
		public const Fix32 C3 = (Fix32) ((int) Fix32.One * 3);
		public const Fix32 C5 = (Fix32) ((int) Fix32.One * 5);
		public const Fix32 C6 = (Fix32) ((int) Fix32.One * 6);
		public const Fix32 C16 = (Fix32) ((int) Fix32.One * 16);
		public const Fix32 C24 = (Fix32) ((int) Fix32.One * 24);
		public const Fix32 C50 = (Fix32) ((int) Fix32.One * 50);
		public const Fix32 C60 = (Fix32) ((int) Fix32.One * 60);
		public const Fix32 C120 = (Fix32) ((int) Fix32.One * 120);
		public static readonly Fix32 C0p001 = 0.001m.ToFix();
		public const Fix32 C0p5 = Fix32.Half;
		public const Fix32 C0p25 = Fix32.Fourth;
		public static readonly Fix32 C1em9 = (1e-4m).ToFix();
		public static readonly Fix32 Cm1em9 = (-1e-4m).ToFix();
		public static readonly Fix32 C1em14 = (1e-4m).ToFix();		
		public static readonly Fix32 C0p1 = 0.1m.ToFix();
		public const Fix32 OneThird = Fix32.Third;
		public const Fix32 C0p75 = (Fix32) (((int) Fix32.One * 3) / 4);
		public static readonly Fix32 C0p15 = 0.15m.ToFix();
		public static readonly Fix32 C0p3 = 0.3m.ToFix();
		public const Fix32 C0p0625 = (Fix32) ((int) Fix32.One / 16);
		public static readonly Fix32 C0p99 = .99m.ToFix();
		public static readonly Fix32 C0p9 = .9m.ToFix();
		public const Fix32 C1p5 = (Fix32) ((int) Fix32.One + (int) Fix32.Half);
		public static readonly Fix32 C1p1 = 1.1m.ToFix();
        public const Fix32 Half = Fix32.Half;
        public const Fix32 OneEighth = (Fix32) ((int) Fix32.One / 8);
		public static readonly Fix32 FourThirds = (4.0 / 3).ToFix();
		public static readonly Fix32 FourThirdsTimesPI = FourThirds.Mul(MathHelper.Pi);
		public static readonly Fix32 TwoFifths = (2.0 / 5).ToFix();
		public static readonly Fix32 C0p2 = 0.2m.ToFix();
		public static readonly Fix32 C0p8 = 0.8m.ToFix();
		public static readonly Fix32 C0p01 = 0.01m.ToFix();
		public static readonly Fix32 C1em7 = 1e-4m.ToFix();
		public static readonly Fix32 C1em5 = 1e-4m.ToFix();
		public static readonly Fix32 C1em4 = 1e-4m.ToFix();
		public static readonly Fix32 C1em10 = 1e-4m.ToFix();
		public static readonly Fix32 Cm0p25 = (-0.25m).ToFix();
		public static readonly Fix32 Cm0p9999 = (-0.9999m).ToFix();
		public static readonly Fix32 C1m1em12 = Fix32.One.Sub(1e-4m.ToFix());
		public static readonly Fix32 GoldenRatio = Fix32.One.Add(5.ToFix().Div(2.ToFix()).SqrtSlow());
		public static readonly Fix32 OneTwelfth = Fix32.One.Div(12.ToFix());
		public static readonly Fix32 C0p0833333333 = .0833333333m.ToFix();
		public static readonly Fix32 C90000 = 16000.ToFix();
		public static readonly Fix32 C600000 = 16000.ToFix();
	}
}
