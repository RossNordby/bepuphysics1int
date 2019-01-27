using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FixedVsFloat : MonoBehaviour {
	public GUISkin skin;
	public int iterations = 20000;

	Dictionary<Func<int, Fix64>, string> fResults = new Dictionary<Func<int, Fix64>, string>();
	Dictionary<Func<int, double>, string> dResults = new Dictionary<Func<int, double>, string>();
	
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
		var tmp = f(iterations);
		return fResults[f] = " = " + sw.Elapsed.TotalMilliseconds / iterations * 1000000 + " ns";
	}

	static Fix64 FAdd(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum += Fix64.One;
		return sum;
	}
	static Fix64 FSub(int iterations) {
		Fix64 sum = 0;
		for (int i = 0; i < iterations; i++) sum -= Fix64.One;
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
	static Fix64 FAbs(int iterations) {
		Fix64 sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Fix64.Abs(Fix64.FromRaw(i));
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
	#endregion

	#region Double
	/// <summary>
	/// Get the number of nanoseconds of an iteration of <paramref name="f"/>, that must execute <see cref="iterations"/> iterations.
	/// </summary>
	string TestDouble(Func<int, double> f) {
		if (dResults.ContainsKey(f)) return dResults[f];
		var sw = Stopwatch.StartNew();
		var tmp = f(iterations);
		return dResults[f] = " = " + sw.Elapsed.TotalMilliseconds / iterations * 1000000 + " ns";
	}

	static double DAdd(int iterations) {
		double sum = 0;
		for (int i = 0; i < iterations; i++) sum += 1d;
		return sum;
	}
	static double DSub(int iterations) {
		double sum = 0;
		for (int i = 0; i < iterations; i++) sum -= 1d;
		return sum;
	}
	static double DInv(int iterations) {
		double sum = 0;
		for (int i = 0; i < iterations; i++) sum = -1d;
		return sum;
	}
	static double DMul2(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * 2d;
		return sum;
	}
	static double DDiv2(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / 2d;
		return sum;
	}
	static double DMul3(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * 3d;
		return sum;
	}
	static double DDiv3(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / 3d;
		return sum;
	}
	static double DMul12345(int iterations) {
		double f12345 = (12345);
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i * f12345;
		return sum;
	}
	static double DDiv12345(int iterations) {
		double f12345 = (12345);
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = i / f12345;
		return sum;
	}
	static double DDiv1(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = 1d / (i);
		return sum;
	}
	static double DModulo(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < 0; i++) sum = 11 * 5 * 3 % (i);
		for (int i = 1; i < iterations / 2; i++) sum = 11 * 5 * 3 % (i);
		return sum;
	}
	static double DSign(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Sign((i));
		return sum;
	}
	static double DAbs(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Abs((i));
		return sum;
	}
	static double DFloor(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Floor((double) (i));
		return sum;
	}
	static double DLog2(int iterations) {
		double sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Math.Log((double) (i), 2);
		return sum;
	}
	static double DLn(int iterations) {
		double sum = 0;
		for (int i = 1; i <= iterations; i++) sum = Math.Log((double) (i));
		return sum;
	}
	static double DPow2(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Pow(2, i);
		return sum;
	}
	static double DPow(int iterations) {
		double sum = 0;
		for (int i = 0; i < iterations; i++) sum = Math.Pow((i), (i));
		return sum;
	}
	static double DAcos(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Acos((i));
		return sum;
	}
	static double DCeiling(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Ceiling((double) (i));
		return sum;
	}
	static double DRound(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Round((double) (i));
		return sum;
	}
	static double DEqualEqual(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum == (i);
		return sum;
	}
	static double DNotEqual(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum != (i);
		return sum;
	}
	static double DGreater(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum > (i);
		return sum;
	}
	static double DLess(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum < (i);
		return sum;
	}
	static double DGreaterOrEqual(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum >= (i);
		return sum;
	}
	static double DLessOrEqual(int iterations) {
		bool tmp;
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = sum <= (i);
		return sum;
	}
	static double DSqrt(int iterations) {
		double sum = 0;
		for (int i = 0; i < iterations; i++) sum = Math.Sqrt((i));
		return sum;
	}
	static double DSin(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Sin((i));
		return sum;
	}
	static double DFastSin(int iterations) { return DSin(iterations); }
	static double DCos(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Cos((i));
		return sum;
	}
	static double DFastCos(int iterations) { return DCos(iterations); }
	static double DTan(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Tan((i));
		return sum;
	}
	static double DAtan(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Atan((i));
		return sum;
	}
	static double DAtan2(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = Math.Atan2((i), (i));
		return sum;
	}
	static double DFastAtan2(int iterations) { return DAtan2(iterations); }
	static double DEquals(int iterations) {
		bool tmp;
		for (int i = -iterations / 2; i < iterations / 2; i++) tmp = (i).Equals((i));
		return 0;
	}
	static double DCompareTo(int iterations) {
		double sum = 0;
		for (int i = -iterations / 2; i < iterations / 2; i++) sum = (i).CompareTo((i));
		return sum;
	}
	#endregion

	private void OnGUI() {
		GUI.skin = skin;
		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical("box");
		GUILayout.Label("Fix64");
		GUILayout.Label("+ " + TestFix64(FAdd));
		GUILayout.Label("- " + TestFix64(FSub));
		GUILayout.Label("-(inv) " + TestFix64(FInv));
		GUILayout.Label("*2 " + TestFix64(FMul2));
		GUILayout.Label("/2 " + TestFix64(FDiv2));
		GUILayout.Label("*3 " + TestFix64(FMul3));
		GUILayout.Label("/3 " + TestFix64(FDiv3));
		GUILayout.Label("*12345 " + TestFix64(FMul12345));
		GUILayout.Label("/12345 " + TestFix64(FDiv12345));
		GUILayout.Label("1/i " + TestFix64(FDiv1));
		GUILayout.Label("% " + TestFix64(FModulo));
		GUILayout.Label("Sign " + TestFix64(FSign));
		GUILayout.Label("Abs " + TestFix64(FAbs));
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
		/*GUILayout.Label("ToLong " + TestFix64());
		GUILayout.Label("FromLong " + TestFix64());
		GUILayout.Label("ToFloat " + TestFix64());
		GUILayout.Label("FromFloat " + TestFix64());
		GUILayout.Label("ToDouble " + TestFix64());
		GUILayout.Label("FromDouble " + TestFix64());*/
		GUILayout.Label("Equals " + TestFix64(FEquals));
		GUILayout.Label("CompareTo " + TestFix64(FCompareTo));
		GUILayout.EndVertical();

		GUILayout.BeginVertical("box");
		GUILayout.Label("Double");
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
		/*GUILayout.Label("ToLong " + TestDouble());
		GUILayout.Label("FromLong " + TestDouble());
		GUILayout.Label("ToFloat " + TestDouble());
		GUILayout.Label("FromFloat " + TestDouble());
		GUILayout.Label("ToDouble " + TestDouble());
		GUILayout.Label("FromDouble " + TestDouble());*/
		GUILayout.Label("Equals " + TestDouble(DEquals));
		GUILayout.Label("CompareTo " + TestDouble(DCompareTo));

		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}
}
