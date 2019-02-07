
using System;

namespace BEPUutilities
{
    /// <summary>
    /// Contains helper math methods.
    /// </summary>
    public static class MathHelper
    {
		/// <summary>
		/// Approximate value of Pi.
		/// </summary>
		public const Fix32 Pi = Fix32.Pi;
		public const Fix32 MinusPi = (Fix32) (-(int) Fix32.Pi);

		/// <summary>
		/// Approximate value of Pi multiplied by two.
		/// </summary>
		public const Fix32 TwoPi = Fix32.PiTimes2;

		/// <summary>
		/// Approximate value of Pi divided by two.
		/// </summary>
		public const Fix32 PiOver2 = Fix32.PiOver2;

		/// <summary>
		/// Approximate value of Pi divided by four.
		/// </summary>
		public const Fix32 PiOver4 = Fix32.PiOver4;

		/// <summary>
		/// Calculate remainder of of Fix32 division using same algorithm
		/// as Math.IEEERemainder
		/// </summary>
		/// <param name="dividend">Dividend</param>
		/// <param name="divisor">Divisor</param>
		/// <returns>Remainder</returns>
		public static Fix32 IEEERemainder(Fix32 dividend, Fix32 divisor)
		{
			return dividend .Sub (divisor .Mul ((dividend .Div (divisor)).Round()));
		}

        /// <summary>
        /// Reduces the angle into a range from -Pi to Pi.
        /// </summary>
        /// <param name="angle">Angle to wrap.</param>
        /// <returns>Wrapped angle.</returns>
        public static Fix32 WrapAngle(Fix32 angle)
        {
            angle = IEEERemainder(angle, TwoPi);
            if (angle < MinusPi)
            {
                angle = angle.Add(TwoPi);
                return angle;
            }
            if (angle >= Pi)
            {
                angle = angle .Sub (TwoPi);
            }
            return angle;

        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.  If the value is less than this, the minimum is returned instead.</param>
        /// <param name="max">Maximum value.  If the value is more than this, the maximum is returned instead.</param>
        /// <returns>Clamped value.</returns>
        public static Fix32 Clamp(Fix32 value, Fix32 min, Fix32 max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            return value;
        }


        /// <summary>
        /// Returns the higher value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Higher value of the two parameters.</returns>
        public static Fix32 Max(Fix32 a, Fix32 b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Returns the lower value of the two parameters.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <returns>Lower value of the two parameters.</returns>
        public static Fix32 Min(Fix32 a, Fix32 b)
        {
            return a < b ? a : b;
        }

		static readonly Fix32 PiOverC180 = Pi.Div(F64.C180);

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees to convert.</param>
        /// <returns>Radians equivalent to the input degrees.</returns>
        public static Fix32 ToRadians(Fix32 degrees)
        {
            return degrees.Mul(PiOverC180);
        }

		static readonly Fix32 C180OverPi = F64.C180.Div(Pi);

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <param name="radians">Radians to convert.</param>
		/// <returns>Degrees equivalent to the input radians.</returns>
		public static Fix32 ToDegrees(Fix32 radians)
        {
            return radians.Mul(C180OverPi);
        }
    }
}
