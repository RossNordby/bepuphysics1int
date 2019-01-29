using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FixedVsFloat : MonoBehaviour {
	public GUISkin skin;
	public int iterations = 20000;
	public int repeats = 2;

	Dictionary<Func<int, Fix64>, string> fResults = new Dictionary<Func<int, Fix64>, string>();
	Dictionary<Func<int, float>, string> dResults = new Dictionary<Func<int, float>, string>();
	
	private void Update() {
		fResults.Clear();
		dResults.Clear();
	}

	#region Fix64
	/// <summary>
	/// Get the number of nanoseconds of an iteration of <paramref name="f"/>, that must execute <see cref="iterations"/> iterations.
	/// </summary>
	string TestFix64(Func<int, Fix64> f) {
		if (fResults.ContainsKey(f)) return fResults[f];
		var sw = Stopwatch.StartNew();
		Fix64 tmp;
		for (int i = 0; i < repeats; i++)
			tmp = f(iterations);
		return fResults[f] = " = " + sw.Elapsed.TotalMilliseconds / iterations / repeats * 1000000 + " ns";
	}

	static Fix64 FAdd(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum += Fix64.One;
		return sum;
	}
	static Fix64 FAddFast(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum = Fix64.FastAdd(sum, Fix64.One);
		return sum;
	}
	static Fix64 FSub(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum -= Fix64.One;
		return sum;
	}
	static Fix64 FSubFast(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum = Fix64.FastSub(sum, Fix64.One);
		return sum;
	}
	static Fix64 FInv(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum = -Fix64.One;
		return sum;
	}
	static Fix64 FMul2(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * Fix64.Two;
		return sum;
	}
	static Fix64 FDiv2(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / Fix64.Two;
		return sum;
	}
	static Fix64 FMul3(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * Fix64.Three;
		return sum;
	}
	static Fix64 FDiv3(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / Fix64.Three;
		return sum;
	}
	static Fix64 FMul12345(int iterations) {
		Fix64 f12345 = Fix64.FromRaw(12345);
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * f12345;
		return sum;
	}
	static Fix64 FDiv12345(int iterations) {
		Fix64 f12345 = Fix64.FromRaw(12345);
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / f12345;
		return sum;
	}
	static Fix64 FDiv1(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = 1 / Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FModulo(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < 0; i++) sum = 11 * 5 * 3 % Fix64.FromRaw(i);
		for (int i = 1; i < iterations / 2; i++) sum = 11 * 5 * 3 % Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FSign(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Sign(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FSignI(int iterations) {
		int sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.SignI(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FAbs(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Abs(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FFastAbs(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.FastAbs(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FFloor(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Floor(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FLog2(int iterations) {
		Fix64 sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Fix64.Log2(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FLn(int iterations) {
		Fix64 sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Fix64.Ln(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FPow2(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Pow2(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FPow(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum = Fix64.Pow(Fix64.FromRaw(i), Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FAcos(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Acos(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FCeiling(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Ceiling(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FRound(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Round(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FEqualEqual(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum == Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FNotEqual(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum != Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FGreater(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum > Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FLess(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum < Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FGreaterOrEqual(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum >= Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FLessOrEqual(int iterations) {
		bool tmp;
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum <= Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FSqrt(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum = Fix64.Sqrt(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FSin(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Sin(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FFastSin(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.FastSin(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FCos(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Cos(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FFastCos(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.FastCos(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FTan(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Tan(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FAtan(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Atan(Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FAtan2(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Atan2(Fix64.FromRaw(i), Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FFastAtan2(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.FastAtan2(Fix64.FromRaw(i), Fix64.FromRaw(i));
		return sum;
	}
	static Fix64 FEquals(int iterations) {
		bool tmp;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = Fix64.FromRaw(i).Equals(Fix64.FromRaw(i));
		return 0;
	}
	static Fix64 FCompareTo(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.FromRaw(i).CompareTo(Fix64.FromRaw(i));
		return sum;
	}

	static Fix64 FToInt(int iterations) {
		int sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (int) Fix64.FromRaw(i);
		return sum;
	}
	static Fix64 FFromInt(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (Fix64) i;
		return sum;
	}
	static Fix64 FToFloat(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (float) Fix64.FromRaw(i);
		return (Fix64) sum;
	}
	static Fix64 FFromFloat(int iterations) {
		Fix64 sum = 0;
		for (float i = -iterations / 2; i < iterations / 2; i++) sum = (Fix64) i;
		return sum;
	}
	static Fix64 FToDouble(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (double) Fix64.FromRaw(i);
		return (Fix64) sum;
	}
	static Fix64 FFromDouble(int iterations) {
		Fix64 sum = 0;
		for (double i = -iterations / 2; i < iterations / 2; i++) sum = (Fix64) i;
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
		return dResults[f] = " = " + sw.Elapsed.TotalMilliseconds / iterations / repeats * 1000000 + " ns";
	}

	static float DAdd(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum += 1f;
		return sum;
	}
	static float DSub(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum -= 1f;
		return sum;
	}
	static float DInv(int iterations) {
		float sum = 0;
		for (int i = 0; i < iterations; i++) sum = -1f;
		return sum;
	}
	static float DMul2(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * 2f;
		return sum;
	}
	static float DDiv2(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / 2f;
		return sum;
	}
	static float DMul3(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * 3f;
		return sum;
	}
	static float DDiv3(int iterations) {
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / 3f;
		return sum;
	}
	static float DMul12345(int iterations) {
		float f12345 = (12345);
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * f12345;
		return sum;
	}
	static float DDiv12345(int iterations) {
		float f12345 = (12345);
		float sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / f12345;
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

		GUILayout.BeginVertical("box");
		GUILayout.Label("Fix64");
		GUILayout.Label("+ " + TestFix64(FAdd) + "  +(fast) " + TestFix64(FAddFast));
		GUILayout.Label("- " + TestFix64(FSub) + "  -(fast) " + TestFix64(FSubFast));
		GUILayout.Label("-(inv) " + TestFix64(FInv));
		GUILayout.Label("*2 " + TestFix64(FMul2));
		GUILayout.Label("/2 " + TestFix64(FDiv2));
		GUILayout.Label("*3 " + TestFix64(FMul3));
		GUILayout.Label("/3 " + TestFix64(FDiv3));
		GUILayout.Label("*12345 " + TestFix64(FMul12345));
		GUILayout.Label("/12345 " + TestFix64(FDiv12345));
		GUILayout.Label("1/i " + TestFix64(FDiv1));
		GUILayout.Label("% " + TestFix64(FModulo));
		GUILayout.Label("Sign " + TestFix64(FSign) + "  Sign(I) " + TestFix64(FSignI));
		GUILayout.Label("Abs " + TestFix64(FAbs) + "  Abs(fast) " + TestFix64(FFastAbs));
		GUILayout.Label("Floor " + TestFix64(FFloor));
		GUILayout.Label("Log2 " + TestFix64(FLog2));
		GUILayout.Label("Ln " + TestFix64(FLn));
		GUILayout.Label("Pow2 " + TestFix64(FPow2));
		GUILayout.Label("Pow " + TestFix64(FPow));
		GUILayout.Label("Acos " + TestFix64(FAcos));
		GUILayout.Label("Ceiling " + TestFix64(FCeiling));
		GUILayout.Label("Round " + TestFix64(FRound));
		GUILayout.Label("== " + TestFix64(FEqualEqual));
		GUILayout.Label("!= " + TestFix64(FNotEqual));
		GUILayout.Label("> " + TestFix64(FGreater));
		GUILayout.Label("< " + TestFix64(FLess));
		GUILayout.Label(">= " + TestFix64(FGreaterOrEqual));
		GUILayout.Label("<= " + TestFix64(FLessOrEqual));
		GUILayout.Label("Sqrt " + TestFix64(FSqrt));
		GUILayout.Label("Sin " + TestFix64(FSin));
		GUILayout.Label("FastSin " + TestFix64(FFastSin));
		GUILayout.Label("Cos " + TestFix64(FCos));
		GUILayout.Label("FastCos " + TestFix64(FFastCos));
		GUILayout.Label("Tan " + TestFix64(FTan));
		GUILayout.Label("Atan " + TestFix64(FAtan));
		GUILayout.Label("Atan2 " + TestFix64(FAtan2));
		GUILayout.Label("FastAtan2 " + TestFix64(FFastAtan2));
		GUILayout.Label("Equals " + TestFix64(FEquals));
		GUILayout.Label("CompareTo " + TestFix64(FCompareTo));

		GUILayout.Label("ToInt " + TestFix64(FToInt));
		GUILayout.Label("FromInt " + TestFix64(FFromInt));
		GUILayout.Label("ToFloat " + TestFix64(FToFloat));
		GUILayout.Label("FromFloat " + TestFix64(FFromFloat));
		GUILayout.Label("ToDouble " + TestFix64(FToDouble));
		GUILayout.Label("FromDouble " + TestFix64(FFromDouble));
		GUILayout.EndVertical();

		GUILayout.BeginVertical("box");
		GUILayout.Label("Float");
		GUILayout.Label("+ " + TestDouble(DAdd));
		GUILayout.Label("- " + TestDouble(DSub));
		GUILayout.Label("-(inv) " + TestDouble(DInv));
		GUILayout.Label("*2 " + TestDouble(DMul2));
		GUILayout.Label("/2 " + TestDouble(DDiv2));
		GUILayout.Label("*3 " + TestDouble(DMul3));
		GUILayout.Label("/3 " + TestDouble(DDiv3));
		GUILayout.Label("*12345 " + TestDouble(DMul12345));
		GUILayout.Label("/12345 " + TestDouble(DDiv12345));
		GUILayout.Label("1/i " + TestDouble(DDiv1));
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
