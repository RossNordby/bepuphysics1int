using System;

public class Fix32Random {
	private Random random;

	public Fix32Random(int seed) {
		random = new Random(seed);
	}

	public Fix Next() {
		return (Fix) random.Next(int.MinValue, int.MaxValue);
	}

	public Fix NextInt(int maxValue) {
		return random.Next(maxValue).ToFix();
	}
}
