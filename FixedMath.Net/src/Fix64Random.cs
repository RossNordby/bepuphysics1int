using System;

namespace FixMath.NET
{
	public class Fix64Random
    {
        private Random random;

        public Fix64Random(int seed)
        {
            random = new Random(seed);
        }

        public Fix32 Next()
        {
            Fix32 result = new Fix32();
            result = (Fix32)(uint)random.Next(int.MinValue, int.MaxValue);
            return result;
        }

        public Fix32 NextInt(int maxValue)
        {
            return random.Next(maxValue).ToFixFast();
        }
    }
}
