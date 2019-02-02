using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FixedVsFloat : MonoBehaviour {
	public GUISkin skin;
	public int iterations = 20000;
	public int repeats = 2;

	Dictionary<Func<int, Fix32>, string> fResults = new Dictionary<Func<int, Fix32>, string>();
	Dictionary<Func<int, float>, string> dResults = new Dictionary<Func<int, float>, string>();
	
	private void Update() {
		fResults.Clear();
		dResults.Clear();
	}

	#region Fix32
	/// <summary>
	/// Get the number of nanoseconds of an iteration of <paramref name="f"/>, that must execute <see cref="iterations"/> iterations.
	/// </summary>
	string TestFix32(Func<int, Fix32> f) {
		if (fResults.ContainsKey(f)) return fResults[f];
		var sw = Stopwatch.StartNew();
		Fix32 tmp;
		for (int i = 0; i < repeats; i++)
			tmp = f(iterations);
		return fResults[f] = " = " + (sw.Elapsed.TotalMilliseconds).ToString("0.0000") + " ms";
	}

	static Fix32 FAdd(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum.Add(Fix32.One);
		return sum;
	}
	static Fix32 FAddFast(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum.AddFast(Fix32.One);
		return sum;
	}
	static Fix32 FSub(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum.Sub(Fix32.One);
		return sum;
	}
	static Fix32 FSubFast(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum.SubFast(Fix32.One);
		return sum;
	}
	static Fix32 FInv(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = Fix32.One.Neg();
		return sum;
	}
	static Fix32[] Fcoeffs = new Fix32[] {
		0f.ToFix32(), 0.0001f.ToFix32(), 0.001f.ToFix32(), 0.01f.ToFix32(),
		0.1f.ToFix32(), 1f.ToFix32(), 10f.ToFix32(), 100f.ToFix32(),
		1000f.ToFix32(), 10000f.ToFix32(),

		(-0f).ToFix32(), (-0.0001f).ToFix32(), (-0.001f).ToFix32(), (-0.01f).ToFix32(),
		(-0.1f).ToFix32(), (-1f).ToFix32(), (-10f).ToFix32(), (-100f).ToFix32(),
		(-1000f).ToFix32(), (-10000f).ToFix32()
	};
	static Fix32 FMul(int iterations) {
		iterations = iterations / (Fcoeffs.Length);
		iterations = iterations / (Fcoeffs.Length);
		Fix32 sum = 0;
		for (int k = 0; k < iterations; k++)
			for (int i = 0; i < Fcoeffs.Length; i++)
				for (int j = 0; j < Fcoeffs.Length; j++)
					sum = Fcoeffs[i].Mul(Fcoeffs[j]);
		return sum;
	}
	static Fix32 FMulFast(int iterations) {
		iterations = iterations / (Fcoeffs.Length);
		iterations = iterations / (Fcoeffs.Length);
		Fix32 sum = 0;
		for (int k = 0; k < iterations; k++)
			for (int i = 0; i < Fcoeffs.Length; i++)
				for (int j = 0; j < Fcoeffs.Length; j++)
					sum = Fcoeffs[i].MulFast(Fcoeffs[j]);
		return sum;
	}
	static Fix32 FDiv(int iterations) {
		iterations = iterations / (Fcoeffs.Length);
		iterations = iterations / (Fcoeffs.Length);
		Fix32 sum = 0;
		for (int k = 0; k < iterations; k++)
			for (int i = 0; i < Fcoeffs.Length; i++)
				for (int j = 0; j < Fcoeffs.Length; j++)
					sum = Fcoeffs[i].Div(Fcoeffs[j]);
		return sum;
	}
	/*
	static Fix32 FDivFast(int iterations) {
		iterations = iterations / (coeffs.Length);
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++)
			for (int j = 0; j < coeffs.Length; j++)
				sum = Fix32.FastDiv(i, coeffs[j]);
		return sum;
	}
	*/
	static Fix32 FDiv1(int iterations) {
		Fix32 sum = 0;
		Fix32 f1 = Fix32.One;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = f1.Div((Fix32) i);
		return sum;
	}
	static Fix32 FModulo(int iterations) {
		Fix32 sum = 0;
		Fix32 fMod = 11.ToFix32().Mul(5.ToFix32()).Mul(3.ToFix32());
		for (int i = -iterations / 2; i < 0; i++) sum = fMod.Mod((Fix32) i);
		for (int i = 1; i < iterations / 2; i++) sum = fMod.Mod((Fix32) i);
		return sum;
	}
	static Fix32 FSign(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Sign();
		return sum;
	}
	static Fix32 FSignI(int iterations) {
		int sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).SignI();
		return Fix32.Zero;
	}
	static Fix32 FAbs(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Abs();
		return sum;
	}
	static Fix32 FFastAbs(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).AbsFast();
		return sum;
	}
	static Fix32 FFloor(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Floor();
		return sum;
	}
	static Fix32 FLog2(int iterations) {
		Fix32 sum = 0;
		for (int i = 1; i <= iterations; i++) sum = ((Fix32) i).Log2();
		return sum;
	}
	static Fix32 FLn(int iterations) {
		Fix32 sum = 0;
		for (int i = 1; i <= iterations; i++) sum = ((Fix32) i).Ln();
		return sum;
	}
	static Fix32 FPow2(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Pow2();
		return sum;
	}
	static Fix32 FPow(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = ((Fix32) i).Pow(((Fix32) i));
		return sum;
	}
	static Fix32 FAcos(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Acos();
		return sum;
	}
	static Fix32 FCeiling(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Ceiling();
		return sum;
	}
	static Fix32 FRound(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Round();
		return sum;
	}
	static Fix32 FFastRound(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).RoundFast();
		return sum;
	}
	static Fix32 FEqualEqual(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum == ((Fix32) i);
		return sum;
	}
	static Fix32 FNotEqual(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum != ((Fix32) i);
		return sum;
	}
	static Fix32 FGreater(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum > ((Fix32) i);
		return sum;
	}
	static Fix32 FLess(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum < ((Fix32) i);
		return sum;
	}
	static Fix32 FGreaterOrEqual(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum >= ((Fix32) i);
		return sum;
	}
	static Fix32 FLessOrEqual(int iterations) {
		bool tmp;
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum <= ((Fix32) i);
		return sum;
	}
	static Fix32 FSqrt(int iterations) {
		Fix32 sum = 0;
		for (int i = 0; i < iterations; i++) sum = ((Fix32) i).Sqrt();
		return sum;
	}
	static Fix32 FSin(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Sin();
		return sum;
	}
	static Fix32 FFastSin(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).SinFast();
		return sum;
	}
	static Fix32 FCos(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Cos();
		return sum;
	}
	static Fix32 FFastCos(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).CosFast();
		return sum;
	}
	static Fix32 FTan(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Tan();
		return sum;
	}
	static Fix32 FAtan(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Atan();
		return sum;
	}
	static Fix32 FAtan2(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Atan2(((Fix32) i));
		return sum;
	}
	static Fix32 FFastAtan2(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).Atan2Fast(((Fix32) i));
		return sum;
	}
	static Fix32 FEquals(int iterations) {
		bool tmp;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = ((Fix32) i).Equals(((Fix32) i));
		return 0;
	}
	static Fix32 FCompareTo(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).CompareTo((Fix32) i).ToFix32();
		return sum;
	}

	static Fix32 FToInt(int iterations) {
		int sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).ToInt();
		return Fix32.Zero;
	}
	static Fix32 FFromInt(int iterations) {
		Fix32 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i.ToFix32();
		return sum;
	}
	static Fix32 FToFloat(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).ToFloat();
		return Fix32.Zero;
	}
	static Fix32 FFromFloat(int iterations) {
		Fix32 sum = 0;
		for (float i = -iterations / 2; i < iterations / 2; i++) sum = i.ToFix32();
		return sum;
	}
	static Fix32 FToDouble(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = ((Fix32) i).ToDouble();
		return Fix32.Zero;
	}
	static Fix32 FFromDouble(int iterations) {
		Fix32 sum = 0;
		for (double i = -iterations / 2; i < iterations / 2; i++) sum = i.ToFix32();
		return sum;
	}
	#endregion

	#region Double
	/// <summary>
	/// Get the number of nanoseconds of an iteration of <paramref name="f"/>, that must execute <see cref="iterations"/> iterations.
	/// </summary>
	string TestDouble(Func<int, float> f) {
		if (dResults.ContainsKey(f)) return dResults[f];
		var sw = Stopwatch.StartNew();
		float tmp;
		for (int i = 0; i < repeats; i++)
			tmp = f(iterations);
		return dResults[f] = " = " + (sw.Elapsed.TotalMilliseconds).ToString("0.0000") + " ms";
	}

	static float DAdd(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum + (1f);
		return sum;
	}
	static float DSub(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = sum - (1f);
		return sum;
	}
	static float DInv(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = -1f;
		return sum;
	}
	static float[] Dcoeffs = new float[] {
		0f, 0.0001f, 0.001f, 0.01f,
		0.1f, 1f, 10f, 100f,
		1000f, 10000f,

		(-0f), (-0.0001f), (-0.001f), (-0.01f),
		(-0.1f), (-1f), (-10f), (-100f),
		(-1000f), (-10000f)
	};
	static float DMul(int iterations) {
		iterations = iterations / (Fcoeffs.Length);
		iterations = iterations / (Fcoeffs.Length);
		float sum = 0;
		for (int k = 0; k < iterations; k++)
			for (int i = 0; i < Fcoeffs.Length; i++)
				for (int j = 0; j < Fcoeffs.Length; j++)
					sum = Dcoeffs[i] * Dcoeffs[j];
		return sum;
	}
	static float DDiv(int iterations) {
		iterations = iterations / (Fcoeffs.Length);
		iterations = iterations / (Fcoeffs.Length);
		float sum = 0;
		for (int k = 0; k < iterations; k++)
			for (int i = 0; i < Fcoeffs.Length; i++)
				for (int j = 0; j < Fcoeffs.Length; j++)
					sum = Dcoeffs[i] / Dcoeffs[j];
		return sum;
	}
	static float DDiv1(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = 1f / (i);
		return sum;
	}
	static float DModulo(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < 0; i++) sum = 11 * 5 * 3 % (i);
		for (int i = 1; i < iterations / 2; i++) sum = 11 * 5 * 3 % (i);
		return sum;
	}
	static float DSign(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Sign((i));
		return sum;
	}
	static float DAbs(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Abs((i));
		return sum;
	}
	static float DFloor(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Floor((float) (i));
		return sum;
	}
	static float DLog2(int iterations) {
		float sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Mathf.Log((float) (i), 2);
		return sum;
	}
	static float DLn(int iterations) {
		float sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Mathf.Log((float) (i));
		return sum;
	}
	static float DPow2(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Pow(2, i);
		return sum;
	}
	static float DPow(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = Mathf.Pow((i), (i));
		return sum;
	}
	static float DAcos(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Acos((i));
		return sum;
	}
	static float DCeiling(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Ceil((float) (i));
		return sum;
	}
	static float DRound(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Round((float) (i));
		return sum;
	}
	static float DEqualEqual(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum == (i);
		return sum;
	}
	static float DNotEqual(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum != (i);
		return sum;
	}
	static float DGreater(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum > (i);
		return sum;
	}
	static float DLess(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum < (i);
		return sum;
	}
	static float DGreaterOrEqual(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum >= (i);
		return sum;
	}
	static float DLessOrEqual(int iterations) {
		bool tmp;
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum <= (i);
		return sum;
	}
	static float DSqrt(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = Mathf.Sqrt((i));
		return sum;
	}
	static float DSin(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Sin((i));
		return sum;
	}
	static float DFastSin(int iterations) { return DSin(iterations); }
	static float DCos(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Cos((i));
		return sum;
	}
	static float DFastCos(int iterations) { return DCos(iterations); }
	static float DTan(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Tan((i));
		return sum;
	}
	static float DAtan(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Atan((i));
		return sum;
	}
	static float DAtan2(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Mathf.Atan2((i), (i));
		return sum;
	}
	static float DFastAtan2(int iterations) { return DAtan2(iterations); }
	static float DEquals(int iterations) {
		bool tmp;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = (i).Equals((i));
		return 0;
	}
	static float DCompareTo(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (i).CompareTo((i));
		return sum;
	}

	static float DToInt(int iterations) {
		int sum = 0;
		for (float i = -iterations / 2; i < iterations / 2; i++) sum = (int) i;
		return sum;
	}
	static float DFromInt(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (float) i;
		return sum;
	}
	static float DToFloat(int iterations) { return 0; }
	static float DFromFloat(int iterations) { return 0; }
	static float DToDouble(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (double) i;
		return (float) sum;
	}
	static float DFromDouble(int iterations) {
		float sum = 0;
		for (double i = -iterations / 2; i < iterations / 2; i++) sum = (float) i;
		return sum;
	}
	#endregion

	private void OnGUI() {
		GUI.skin = skin;
		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical("box", GUILayout.MinWidth(400));
		GUILayout.Label("Fix32 (" + (iterations * repeats) + " iterations)");
		GUILayout.Label("+ " + TestFix32(FAdd) + "  +(fast) " + TestFix32(FAddFast));
		GUILayout.Label("- " + TestFix32(FSub) + "  -(fast) " + TestFix32(FSubFast));
		GUILayout.Label("-(inv) " + TestFix32(FInv));
		GUILayout.Label("* " + TestFix32(FMul) + "  *(fast) " + TestFix32(FMulFast));
		GUILayout.Label("/ " + TestFix32(FDiv));
		GUILayout.Label("1/x " + TestFix32(FDiv1));
		GUILayout.Label("% " + TestFix32(FModulo));
		GUILayout.Label("Sign " + TestFix32(FSign) + "  Sign(I) " + TestFix32(FSignI));
		GUILayout.Label("Abs " + TestFix32(FAbs) + "  Abs(fast) " + TestFix32(FFastAbs));
		GUILayout.Label("Floor " + TestFix32(FFloor));
		GUILayout.Label("Log2 " + TestFix32(FLog2));
		GUILayout.Label("Ln " + TestFix32(FLn));
		GUILayout.Label("Pow2 " + TestFix32(FPow2));
		GUILayout.Label("Pow " + TestFix32(FPow));
		GUILayout.Label("Acos " + TestFix32(FAcos));
		GUILayout.Label("Ceiling " + TestFix32(FCeiling));
		GUILayout.Label("Round " + TestFix32(FRound) + "  Round(fast) " + TestFix32(FFastRound));
		GUILayout.Label("== " + TestFix32(FEqualEqual));
		GUILayout.Label("!= " + TestFix32(FNotEqual));
		GUILayout.Label("> " + TestFix32(FGreater));
		GUILayout.Label("< " + TestFix32(FLess));
		GUILayout.Label(">= " + TestFix32(FGreaterOrEqual));
		GUILayout.Label("<= " + TestFix32(FLessOrEqual));
		GUILayout.Label("Sqrt " + TestFix32(FSqrt));
		GUILayout.Label("Sin " + TestFix32(FSin));
		GUILayout.Label("FastSin " + TestFix32(FFastSin));
		GUILayout.Label("Cos " + TestFix32(FCos));
		GUILayout.Label("FastCos " + TestFix32(FFastCos));
		GUILayout.Label("Tan " + TestFix32(FTan));
		GUILayout.Label("Atan " + TestFix32(FAtan));
		GUILayout.Label("Atan2 " + TestFix32(FAtan2));
		GUILayout.Label("FastAtan2 " + TestFix32(FFastAtan2));
		GUILayout.Label("Equals " + TestFix32(FEquals));
		GUILayout.Label("CompareTo " + TestFix32(FCompareTo));

		GUILayout.Label("ToInt " + TestFix32(FToInt));
		GUILayout.Label("FromInt " + TestFix32(FFromInt));
		GUILayout.Label("ToFloat " + TestFix32(FToFloat));
		GUILayout.Label("FromFloat " + TestFix32(FFromFloat));
		GUILayout.Label("ToDouble " + TestFix32(FToDouble));
		GUILayout.Label("FromDouble " + TestFix32(FFromDouble));
		GUILayout.EndVertical();

		GUILayout.BeginVertical("box", GUILayout.MinWidth(400));
		GUILayout.Label("Float (" + (iterations * repeats) + " iterations)");
		GUILayout.Label("+ " + TestDouble(DAdd));
		GUILayout.Label("- " + TestDouble(DSub));
		GUILayout.Label("-(inv) " + TestDouble(DInv));
		GUILayout.Label("* " + TestDouble(DMul));
		GUILayout.Label("/ " + TestDouble(DDiv));
		GUILayout.Label("1/x " + TestDouble(DDiv1));
		GUILayout.Label("% " + TestDouble(DModulo));
		GUILayout.Label("Sign " + TestDouble(DSign));
		GUILayout.Label("Abs " + TestDouble(DAbs));
		GUILayout.Label("Floor " + TestDouble(DFloor));
		GUILayout.Label("Log2 " + TestDouble(DLog2));
		GUILayout.Label("Ln " + TestDouble(DLn));
		GUILayout.Label("Pow2 " + TestDouble(DPow2));
		GUILayout.Label("Pow " + TestDouble(DPow));
		GUILayout.Label("Acos " + TestDouble(DAcos));
		GUILayout.Label("Ceiling " + TestDouble(DCeiling));
		GUILayout.Label("Round " + TestDouble(DRound));
		GUILayout.Label("== " + TestDouble(DEqualEqual));
		GUILayout.Label("!= " + TestDouble(DNotEqual));
		GUILayout.Label("> " + TestDouble(DGreater));
		GUILayout.Label("< " + TestDouble(DLess));
		GUILayout.Label(">= " + TestDouble(DGreaterOrEqual));
		GUILayout.Label("<= " + TestDouble(DLessOrEqual));
		GUILayout.Label("Sqrt " + TestDouble(DSqrt));
		GUILayout.Label("Sin " + TestDouble(DSin));
		GUILayout.Label("FastSin " + TestDouble(DFastSin));
		GUILayout.Label("Cos " + TestDouble(DCos));
		GUILayout.Label("FastCos " + TestDouble(DFastCos));
		GUILayout.Label("Tan " + TestDouble(DTan));
		GUILayout.Label("Atan " + TestDouble(DAtan));
		GUILayout.Label("Atan2 " + TestDouble(DAtan2));
		GUILayout.Label("FastAtan2 " + TestDouble(DFastAtan2));
		GUILayout.Label("Equals " + TestDouble(DEquals));
		GUILayout.Label("CompareTo " + TestDouble(DCompareTo));

		GUILayout.Label("ToInt " + TestDouble(DToInt));
		GUILayout.Label("FromInt " + TestDouble(DFromInt));
		GUILayout.Label("ToFloat " + TestDouble(DToFloat));
		GUILayout.Label("FromFloat " + TestDouble(DFromFloat));
		GUILayout.Label("ToDouble " + TestDouble(DToDouble));
		GUILayout.Label("FromDouble " + TestDouble(DFromDouble));
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}
}
