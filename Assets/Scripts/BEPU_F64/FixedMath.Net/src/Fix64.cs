#define CHECKMATH
//#define USE_DOUBLES // Used for testing

using System;
using System.Runtime.CompilerServices;

namespace FixMath.NET
{
    /// <summary>
    /// Represents a Q1.17.14 fixed-point number. TO DO: Rename to Fix32
    /// </summary>
    [Serializable]
    public partial struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>
    {
        [UnityEngine.SerializeField]
        private int RawValue;

        // Precision of this type is 2^-14, that is 6.103515625E-5
        public static readonly decimal Precision = (decimal) (double) new Fix64(1);
        public static readonly Fix64 MaxValue = new Fix64(MAX_VALUE);
        public static readonly Fix64 MinValue = new Fix64(MIN_VALUE);
        public static readonly Fix64 MinusOne = new Fix64(-ONE);
        public static readonly Fix64 One = new Fix64(ONE);
        public static readonly Fix64 Two = (Fix64)2;
        public static readonly Fix64 Three = (Fix64)3;
        public static readonly Fix64 Zero = new Fix64();
        public static readonly Fix64 C0p28 = (Fix64) 0.28m;
        /// <summary>
        /// The value of Pi
        /// </summary>
        public static readonly Fix64 Pi = new Fix64(PI);
        public static readonly Fix64 PiOver2 = new Fix64(PI_OVER_2);
        public static readonly Fix64 PiOver4 = new Fix64(PI_OVER_4);
        public static readonly Fix64 PiTimes2 = new Fix64(PI_TIMES_2);
        public static readonly Fix64 PiInv = new Fix64(PI_INV);
        public static readonly Fix64 PiOver2Inv = new Fix64(PI_OVER_2_INV);
        public static readonly Fix64 E = new Fix64(E_RAW);
        public static readonly Fix64 EPow4 = new Fix64(EPOW4);
        public static readonly Fix64 Ln2 = new Fix64(LN2);
        public static readonly Fix64 Log2Max = new Fix64(LOG2MAX);
        public static readonly Fix64 Log2Min = new Fix64(LOG2MIN);

		const int MAX_VALUE = int.MaxValue;
		const int MIN_VALUE = int.MinValue;
		const int MAX_INT_VALUE = int.MaxValue >> FRACTIONAL_PLACES;
		const int MIN_INT_VALUE = int.MinValue >> FRACTIONAL_PLACES;
		public const int NUM_BITS = 32;
		public const int FRACTIONAL_PLACES = 14;
		const int SIGNED_INTEGER_PLACES = NUM_BITS - FRACTIONAL_PLACES;

		const int NUM_BITS_MINUS_ONE = NUM_BITS - 1;
		const int ONE = 1 << FRACTIONAL_PLACES;
		const int FRACTIONAL_MASK = ONE - 1;
		const int INTEGER_MASK = ~FRACTIONAL_MASK;
		const int LOG2MAX = (NUM_BITS - 1) << FRACTIONAL_PLACES;
		const int LOG2MIN = -(NUM_BITS << FRACTIONAL_PLACES);
		const int LUT_SIZE_RS = FRACTIONAL_PLACES / 2 - 1;
		const int LUT_SIZE = PI_OVER_2 >> LUT_SIZE_RS;
		static readonly Fix64 LutInterval = (Fix64) (LUT_SIZE - 1) / PiOver2;

		// Const before rounding
		const decimal D_PI = ONE * (3.1415926535897932384626433832795028841971693993751058209749445923078164m);
		const decimal D_PI_TIMES_2 = ONE * (2m * 3.1415926535897932384626433832795028841971693993751058209749445923078164m);
		const decimal D_PI_OVER_2 = ONE * (3.1415926535897932384626433832795028841971693993751058209749445923078164m / 2m);
		const decimal D_PI_OVER_4 = ONE * (3.1415926535897932384626433832795028841971693993751058209749445923078164m / 4m);
		const decimal D_PI_INV = ONE * (1m / 3.1415926535897932384626433832795028841971693993751058209749445923078164m);
		const decimal D_PI_OVER_2_INV = ONE * (1m / (3.1415926535897932384626433832795028841971693993751058209749445923078164m / 2m));
		const decimal D_E_RAW = ONE * (2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200m);
		const decimal D_EPOW4 = ONE * (2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200m * 2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200m * 2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200m * 2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742746639193200m);
		const decimal D_LN2 = ONE * (0.693147180559945309417232121458m);

		// Const rounded to int instead of truncated
		const int PI = (int) (D_PI < 0 ? D_PI - 0.5m : D_PI + 0.5m);
		const int PI_TIMES_2 = (int) (D_PI_TIMES_2 < 0 ? D_PI_TIMES_2 - 0.5m : D_PI_TIMES_2 + 0.5m);
		const int PI_OVER_2 = (int) (D_PI_OVER_2 < 0 ? D_PI_OVER_2 - 0.5m : D_PI_OVER_2 + 0.5m);
		const int PI_OVER_4 = (int) (D_PI_OVER_4 < 0 ? D_PI_OVER_4 - 0.5m : D_PI_OVER_4 + 0.5m);
		const int PI_INV = (int) (D_PI_INV < 0 ? D_PI_INV - 0.5m : D_PI_INV + 0.5m);
		const int PI_OVER_2_INV = (int) (D_PI_OVER_2_INV < 0 ? D_PI_OVER_2_INV - 0.5m : D_PI_OVER_2_INV + 0.5m);
		const int E_RAW = (int) (D_E_RAW < 0 ? D_E_RAW - 0.5m : D_E_RAW + 0.5m);
		const int EPOW4 = (int) (D_EPOW4 < 0 ? D_EPOW4 - 0.5m : D_EPOW4 + 0.5m);
		const int LN2 = (int) (D_LN2 < 0 ? D_LN2 - 0.5m : D_LN2 + 0.5m);


		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 operator +(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
			return (Fix64) ((double) x + (double) y);
#endif
			int xRaw = x.RawValue;
			int yRaw = y.RawValue;

			// https://stackoverflow.com/questions/17580118/signed-saturated-add-of-64-bit-ints/17587197#17587197
			// determine the lower or upper bound of the result
			//int ret = (x.RawValue < 0) ? MIN_VALUE : MAX_VALUE;
			int ret = (int) ((((uint) xRaw >> NUM_BITS_MINUS_ONE) - 1U) ^ (1U << NUM_BITS_MINUS_ONE));
			// this is always well defined:
			// if x < 0 this adds a positive value to INT64_MIN
			// if x > 0 this subtracts a positive value from INT64_MAX
			//int comp = ret - xRaw;
			// the condition is equivalent to
			// ((x < 0) && (y > comp)) || ((x >=0) && (y <= comp))
			return new Fix64((xRaw < 0) != (yRaw > (ret - xRaw)) ? ret : xRaw + yRaw);
		}

		/// <summary>
		/// Adds x and y. Doesn't saturate, wraps around when overflows.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 FastAdd(in Fix64 x, in Fix64 y) {
			return new Fix64(x.RawValue + y.RawValue);
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 operator -(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
            return (Fix64) ((double) x - (double) y);
#endif
			int xRaw = x.RawValue;
			int yRaw = y.RawValue;
			long sub = (long) xRaw - (long) yRaw; // TO TEST: Shift and operate to check overflow
			return new Fix64(((int) sub) != sub ? (int) ((((uint) xRaw >> NUM_BITS_MINUS_ONE) - 1U) ^ (1U << NUM_BITS_MINUS_ONE)) : (int) sub);
		}

		/// <summary>
		/// Subtracts y from x. Doesn't saturate, wraps around when overflows.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 FastSub(in Fix64 x, in Fix64 y) {
			return new Fix64(x.RawValue - y.RawValue);
		}

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SignI(in Fix64 v) {
			//https://stackoverflow.com/questions/14579920/fast-sign-of-integer-in-c/14612418#14612418
			int vRaw = v.RawValue;
			return (vRaw >> NUM_BITS_MINUS_ONE) | (int) (((uint) -vRaw) >> NUM_BITS_MINUS_ONE);
        }

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Sign(in Fix64 v) {
			int vRaw = v.RawValue;
			const int RS = NUM_BITS_MINUS_ONE - FRACTIONAL_PLACES;
			return new Fix64(((vRaw >> RS) | (int) (((uint) -vRaw) >> RS)) & INTEGER_MASK);
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Abs(in Fix64 v) { 
#if USE_DOUBLES
			return (Fix64) Math.Abs((double) v);
#endif
            if (v.RawValue == MIN_VALUE) {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = v.RawValue >> NUM_BITS_MINUS_ONE;
            return new Fix64((v.RawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 FastAbs(in Fix64 v) {
#if USE_DOUBLES
            return (Fix64) Math.Abs((double) v);
#endif
            var mask = v.RawValue >> NUM_BITS_MINUS_ONE;
            return new Fix64((v.RawValue + mask) ^ mask);
        }

		/// <summary>
		/// Returns the largest integer less than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 Floor(in Fix64 v) {
#if USE_DOUBLES
            return (Fix64) Math.Floor((double) v);
#endif
            // Just zero out the fractional part
            return new Fix64(v.RawValue & INTEGER_MASK);
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 Ceiling(in Fix64 v) {
#if USE_DOUBLES
            return (Fix64) Math.Ceiling((double) v);
#endif
			int vRaw = v.RawValue;
			var hasFractionalPart = (vRaw & FRACTIONAL_MASK) != 0;
			return hasFractionalPart ? new Fix64(vRaw & INTEGER_MASK) + One : v;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		public static Fix64 Round(in Fix64 v) {
#if USE_DOUBLES
            return (Fix64) Math.Round((double) value);
#endif
			int vRaw = v.RawValue;
			var fractionalPart = vRaw & FRACTIONAL_MASK;
			var integralPart = new Fix64(vRaw & INTEGER_MASK);
			if (fractionalPart < (ONE >> 1)) {
				return integralPart;
			}
			if (fractionalPart > (ONE >> 1)) {
				return integralPart + One;
			}
			// if number is halfway between two values, round to the nearest even number
			// this is the method used by System.Math.Round().
			return (integralPart.RawValue & ONE) == 0 ?
				integralPart : 
				integralPart + One;
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// FastRount(Fix64.MaxValue) is undefined
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 FastRound(in Fix64 v) {
#if USE_DOUBLES
            return (Fix64) Math.Round((double) value);
#endif
			// https://sestevenson.wordpress.com/2009/08/19/rounding-in-fixed-point-number-conversions/
			int vRaw = v.RawValue;
			int odd = (vRaw & ONE) >> FRACTIONAL_PLACES;
			return new Fix64((vRaw + (ONE / 2 - 1) + odd) & INTEGER_MASK);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 operator *(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
            return (Fix64) ((double) x * (double) y);
#endif
			long mult = ((long) x.RawValue * (long) y.RawValue) >> FRACTIONAL_PLACES;

			//int ret = (x.RawValue < 0) ? MIN_VALUE : MAX_VALUE;
			int sat = (int) ((((ulong) mult >> 63) - 1U) ^ (1U << NUM_BITS_MINUS_ONE));

			return new Fix64(mult < MIN_VALUE || mult > MAX_VALUE ? sat : (int) mult);

			// Testing saturation. Didn't work
			/*if (mult >> NUM_BITS > 0)
				return new Fix64((int) ((((uint) (mult) >> 63) - 1U) ^ (1U << NUM_BITS_MINUS_ONE))); // Saturate
			return new Fix64((int) mult);
			*/
			return mult < MIN_VALUE ? MinValue :
				mult > MAX_VALUE ? MaxValue :
				new Fix64((int) mult);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining), Obsolete]
		public static Fix64 SafeMul(in Fix64 x, in Fix64 y) {
			return x * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 FastMul(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
			return (Fix64) ((double) x * (double) y);
#endif
			return new Fix64((int) (((long) x.RawValue * (long) y.RawValue) >> FRACTIONAL_PLACES));
		}
		
		/// <summary>
		/// Returns the base-2 logarithm of a specified number.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The argument was non-positive
		/// </exception>
		public static Fix64 Log2(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Log((double) x, 2);
#endif
            if (x.RawValue <= 0)
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            uint b = 1U << (FRACTIONAL_PLACES - 1);
			uint y = 0;

            var rawX = x.RawValue;
            while (rawX < ONE)
            {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= (ONE << 1))
            {
                rawX >>= 1;
                y += ONE;
            }

            Fix64 z = new Fix64(rawX);

            for (int i = 0; i < FRACTIONAL_PLACES; i++)
            {
                z = z * z;
                if (z.RawValue >= (ONE << 1))
                {
                    z = new Fix64(z.RawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return new Fix64((int) y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Ln(in Fix64 x)
        {
#if USE_DOUBLES
            return (Fix64) Math.Log((double) x);
#endif
            return Log2(x) * Ln2;
        }

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// </summary>
        public static Fix64 Pow2(Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Pow(2, (double) x);
#endif
            if (x.RawValue == 0) return One;

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.RawValue < 0;
            if (neg) x = -x;

            if (x == One)
                return neg ? One / Two : Two;
            if (x >= Log2Max) return neg ? One / MaxValue : MaxValue;
            if (x <= Log2Min) return neg ? MaxValue : Zero;

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */
            int integerPart = (int)Floor(x);
            // Take fractional part of exponent
            x = new Fix64((int) ((uint) x.RawValue & FRACTIONAL_MASK));

            Fix64 result = One;
            Fix64 term = One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = x * term * Ln2 / (Fix64)i;
                result += term;
                i++;
            }

            result = new Fix64(result.RawValue << integerPart);
            if (neg) result = One / result;

            return result;
        }

		/// <summary>
		/// Returns a specified number raised to the specified power.
		/// </summary>
		/// <exception cref="DivideByZeroException">
		/// The base was zero, with a negative exponent
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The base was negative, with a non-zero exponent
		/// </exception>
		public static Fix64 Pow(in Fix64 b, in Fix64 exp)
        {
#if USE_DOUBLES
            return (Fix64) Math.Pow((double) b, (double) exp);
#endif
            if (b == One)
                return One;
            if (exp.RawValue == 0)
                return One;
            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    throw new DivideByZeroException();
                }
                return Zero;
            }
            
            Fix64 log2 = Log2(b);
            return Pow2(exp * log2);
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// </summary>
        public static Fix64 Acos(in Fix64 x)
        {
#if USE_DOUBLES
            return (Fix64) Math.Acos((double) x);
#endif
            if (x < -One || x > One)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (x.RawValue == 0)
                return PiOver2;

            var result = Atan(Sqrt(One - x * x) / x);
            return x.RawValue < 0 ? result + Pi : result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountLeadingZeroes(ulong x) {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        public static Fix64 operator /(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
            return (Fix64) ((double) x / (double) y);
#endif
            long xl = x.RawValue;
            long yl = y.RawValue;

            if (yl == 0) {
                return new Fix64(unchecked((int) (((((uint) xl) >> NUM_BITS_MINUS_ONE) - 1U) ^ (1U << NUM_BITS_MINUS_ONE))));
                return xl >= 0 ? MaxValue : MinValue; // Branched version of the previous code, for clarity. Slower
            }

			long a = xl << FRACTIONAL_PLACES;
			long b = yl;
			long r = a / b;
			if (r > MAX_VALUE) return MaxValue;
			if (r < MIN_VALUE) return MinValue;
			return new Fix64((int) r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 operator %(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
            return (Fix64) ((double) x % (double) y);
#endif
            return new Fix64(
                x.RawValue == MIN_VALUE & y.RawValue == -1 ?
                0 :
                x.RawValue % y.RawValue);
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        public static Fix64 FastMod(in Fix64 x, in Fix64 y) {
#if USE_DOUBLES
            return (Fix64) ((double) x % (double) y);
#endif
            return new Fix64(x.RawValue % y.RawValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 operator -(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) (-(double) x);
#endif
        	//return new Fix64(-x.RawValue);
            return new Fix64(x.RawValue == MIN_VALUE ? MAX_VALUE : -x.RawValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Fix64 x, in Fix64 y) {
            return x.RawValue == y.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Fix64 x, in Fix64 y) {
            return x.RawValue != y.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in Fix64 x, in Fix64 y) {
            return x.RawValue > y.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in Fix64 x, in Fix64 y) {
            return x.RawValue < y.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in Fix64 x, in Fix64 y) {
            return x.RawValue >= y.RawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in Fix64 x, in Fix64 y) {
            return x.RawValue <= y.RawValue;
        }


        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static Fix64 Sqrt(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Sqrt((double) x);
#endif
            var xl = x.RawValue;
            if (xl < 0) {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }
			
            var num = (uint)xl;
            var result = 0U;

            // second-to-top bit
            var bit = 1U << (NUM_BITS - 2);

            while (bit > num) {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i) {
                // First we get the top 48 bits of the answer.
                while (bit != 0) {
                    if (num >= result + bit) {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else {
                        result = result >> 1;
                    }
                    bit >>= 2;
                }

                if (i == 0) {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1U << (NUM_BITS / 2)) - 1) {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - (1U << NUM_BITS_MINUS_ONE);
                        result = (result << (NUM_BITS / 2)) + (1U << NUM_BITS_MINUS_ONE);
                    }
                    else {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1U << (NUM_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result) {
                ++result;
            }
            return new Fix64((int)result);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// The relative error is less than 1E-10 for x in [-2PI, 2PI], and less than 1E-7 in the worst case.
        /// </summary>
        public static Fix64 Sin(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Sin((double) x);
#endif
			var clampedRaw = ClampSinValue(x.RawValue, out var flipHorizontal, out var flipVertical);
            var clamped = new Fix64((int) clampedRaw);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            var rawIndex = clamped * LutInterval;
            var roundedIndex = Round(rawIndex);
            var indexError = rawIndex - roundedIndex;

            var nearestValue = new Fix64((int) SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex :
                (int)roundedIndex]);
            var secondNearestValue = new Fix64((int) SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex - SignI(indexError) :
                (int)roundedIndex + SignI(indexError)]);

            var delta = (indexError * Abs(nearestValue - secondNearestValue)).RawValue;
            var interpolatedValue = nearestValue.RawValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return new Fix64(finalValue);
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static Fix64 FastSin(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Sin((double) x);
#endif
			var clampedL = ClampSinValue(x.RawValue, out bool flipHorizontal, out bool flipVertical);

			// Here we use the fact that the SinLut table has a number of entries
			// equal to (PI_OVER_2 >> LUT_SIZE_RS) to use the angle to index directly into it
			var rawIndex = (uint)(clampedL >> LUT_SIZE_RS);
            if (rawIndex >= LUT_SIZE) {
                rawIndex = LUT_SIZE - 1;
            }
            var nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            return new Fix64((int) (flipVertical ? -nearestValue : nearestValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical) {
#if CHECKMATH
            var clamped2Pi = angle;
            for (int i = 0; i < LARGE_PI_TIMES; ++i)
            {
                clamped2Pi %= (LARGE_PI_RAW >> i);
            }
#else
			// Clamp value to 0 - 2*PI using modulo; this is very slow but there's no better way AFAIK
			var clamped2Pi = angle % PI_TIMES_2;
#endif
            if (angle < 0) {
                clamped2Pi += PI_TIMES_2;
            }

            // The LUT contains values for 0 - PiOver2; every other value must be obtained by
            // vertical or horizontal mirroring
            flipVertical = clamped2Pi >= PI;
            // obtain (angle % PI) from (angle % 2PI) - much faster than doing another modulo
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI) {
                clampedPi -= PI;
            }
            flipHorizontal = clampedPi >= PI_OVER_2;
            // obtain (angle % PI_OVER_2) from (angle % PI) - much faster than doing another modulo
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2) {
                clampedPiOver2 -= PI_OVER_2;
            }
            return clampedPiOver2;
        }

        /// <summary>
        /// Returns the cosine of x.
        /// See Sin() for more details.
        /// </summary>
        public static Fix64 Cos(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Cos((double) x);
#endif
            var xl = x.RawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return Sin(new Fix64(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static Fix64 FastCos(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Cos((double) x);
#endif
            var xl = x.RawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new Fix64(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static Fix64 Tan(in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Tan((double) x);
#endif
			var clampedPi = x.RawValue % PI;
            var flip = false;
            if (clampedPi < 0) {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2) {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = new Fix64(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            var rawIndex = clamped * LutInterval;
            var roundedIndex = Round(rawIndex);
            var indexError = rawIndex - roundedIndex;

            var nearestValue = new Fix64((int) TanLut[(int)roundedIndex]);
            var secondNearestValue = new Fix64((int) TanLut[(int)roundedIndex + SignI(indexError)]);

            var delta = (indexError * Abs(nearestValue - secondNearestValue)).RawValue;
            var interpolatedValue = nearestValue.RawValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;
            return new Fix64(finalValue);
        }

        public static Fix64 FastAtan2(in Fix64 y, in Fix64 x) {
#if USE_DOUBLES
            return (Fix64) Math.Atan2((double) y, (double) x);
#endif
            var yl = y.RawValue;
            var xl = x.RawValue;
            if (xl == 0) {
                if (yl > 0) {
                    return PiOver2;
                }
                if (yl == 0) {
                    return Zero;
                }
                return -PiOver2;
            }
            Fix64 atan;
            var z = y / x;

            // Deal with overflow
			if (One + (C0p28 * z * z) == MaxValue) {
                return y.RawValue < 0 ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One) {
                atan = z / (One + C0p28 * z * z);
                if (xl < 0) {
                    if (yl < 0) {
                        return atan - Pi;
                    }
                    return atan + Pi;
                }
            }
            else {
                atan = PiOver2 - z / (z * z + C0p28);
                if (yl < 0) {
                    return atan - Pi;
                }
            }
            return atan;
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// </summary>
        public static Fix64 Atan(Fix64 z)
        {
#if USE_DOUBLES
            return (Fix64) Math.Atan((double) z);
#endif
            if (z.RawValue == 0)
                return Zero;

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            bool neg = (z.RawValue < 0);
            if (neg) z = -z;

            Fix64 result;

            if (z == One)
                result = PiOver4;
            else
            {
                bool invert = z > One;
                if (invert) z = One / z;

                result = One;
                Fix64 term = One;

                Fix64 zSq = z * z;
                Fix64 zSq2 = zSq * Two;
                Fix64 zSqPlusOne = zSq + One;
                Fix64 zSq12 = zSqPlusOne * Two;
                Fix64 dividend = zSq2;
                Fix64 divisor = zSqPlusOne * Three;

                for (int i = 2; i < 30; i++)
                {
                    term *= dividend / divisor;
                    result += term;

                    dividend += zSq2;
                    divisor += zSq12;

                    if (term.RawValue == 0)
                        break;
                }

                result = result * z / zSqPlusOne;

                if (invert)
                    result = PiOver2 - result;
            }

            if (neg) result = -result;
            return result;
        }

        public static Fix64 Atan2(in Fix64 y, in Fix64 x)
        {
#if USE_DOUBLES
            return (Fix64) Math.Atan2((double) y, (double) x);
#endif
            var yl = y.RawValue;
            var xl = x.RawValue;
            if (xl == 0)
            {
                if (yl > 0)
                    return PiOver2;
                if (yl == 0)
                    return Zero;
                return -PiOver2;
            }
            Fix64 atan;
            var z = y / x;

            // Deal with overflow
            if (One + ((Fix64)0.28d * z * z) == MaxValue)
            {
                return y.RawValue < 0 ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One)
            {
                atan = z / (One + (Fix64)0.28d * z * z);
                if (xl < 0)
                {
                    if (yl < 0)
                    {
                        return atan - Pi;
                    }
                    return atan + Pi;
                }
            }
            else
            {
                atan = PiOver2 - z / (z * z + (Fix64)0.28d);
                if (yl < 0)
                    return atan - Pi;
            }

            return atan;
        }
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Fix64(float value) {
            return new Fix64((int) Clamp(value * ONE, MIN_VALUE, MAX_VALUE));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(in Fix64 value) {
            return (float)value.RawValue / ONE;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Fix64(double value) {
			return new Fix64((int) Clamp(value * ONE, MIN_VALUE, MAX_VALUE));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double(in Fix64 value) {
			return (double) value.RawValue / ONE;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fix64(decimal value) {
			return new Fix64((int) Clamp(value * ONE, MIN_VALUE, MAX_VALUE));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator decimal(in Fix64 value) {
			return (decimal) value.RawValue / ONE;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Fix64(int value) {
			return new Fix64(value > MAX_INT_VALUE ? MAX_VALUE : value < MIN_INT_VALUE ? MIN_VALUE : value * ONE);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int(in Fix64 value) {
			return (int) value.RawValue / ONE;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp(float value, double min, double max) {
			return value > max ? max : value < min ? min : value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp(double value, double min, double max) {
			return value > max ? max : value < min ? min : value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Clamp(decimal value, decimal min, decimal max) {
			return value > max ? max : value < min ? min : value;
		}

		public override bool Equals(object obj) {
            return obj is Fix64 && ((Fix64)obj).RawValue == RawValue;
        }

        public override int GetHashCode() {
            return RawValue.GetHashCode();
        }

        public bool Equals(Fix64 other) {
            return RawValue == other.RawValue;
        }

        public int CompareTo(Fix64 other) {
            return RawValue.CompareTo(other.RawValue);
        }

        public override string ToString() {
            return ((double)this).ToString("0.##########");
        }

		/// <summary>
		/// Intentionally kept out of the way so this is never accidentally called
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix64 FromRaw(int rawValue) {
            return new Fix64(rawValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ToRaw() {
			return RawValue;
		}

		/// <summary>
		/// This is the constructor from raw value; it can only be used interally.
		/// </summary>
		/// <param name="rawValue"></param>
		private Fix64(int rawValue) {
			RawValue = rawValue;
		}


#if UNITY_EDITOR
		static void GenerateSinLut(string where) {
			CalculateLargePI(out Fix64 largePIF64, out int N);

			using (var writer = new System.IO.StreamWriter(where + "/Fix64SinLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
	public partial struct Fix64 {
		const int LARGE_PI_RAW = " + largePIF64.RawValue + @";
		const int LARGE_PI_TIMES = " + N + @";

		internal static readonly int[] SinLut = new int[" + LUT_SIZE + "] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * 3.1415926535897932384626433832795028841971693993751058209749445923078164m * 0.5m / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("			");
                    }
                    var sin = Math.Sin((double) angle);
                    var rawValue = ((Fix64)sin).RawValue;
                    writer.Write(string.Format("0x{0:X}U, ", rawValue));
                }
                writer.Write(
@"
		};
	}
}");
            }
        }

        static void GenerateTanLut(string where) {
            using (var writer = new System.IO.StreamWriter(where + "/Fix64TanLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
	public partial struct Fix64 {
		internal static readonly int[] TanLut = new int[" + LUT_SIZE + "] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * 3.1415926535897932384626433832795028841971693993751058209749445923078164m * 0.5m / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("			");
                    }
                    var tan = Math.Tan((double) angle);
                    if (tan > (double)MaxValue || tan < 0.0) {
                        tan = (double)MaxValue;
                    }
                    var rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : (Fix64)tan).RawValue;
                    writer.Write(string.Format("0x{0:X}U, ", rawValue));
                }
                writer.Write(
@"
		};
	}
}");
            }
        }

		// Obtained from ((Fix64)1686629713.065252369824872831112M).RawValue
		// This is (2^29)*PI, where 29 is the largest N such that (2^N)*PI < MaxValue.
		// The idea is that this number contains way more precision than PI_TIMES_2,
		// and (((x % (2^29*PI)) % (2^28*PI)) % ... (2^1*PI) = x % (2 * PI)
		// In practice this gives us an error of about 1,25e-9 in the worst case scenario (Sin(MaxValue))
		// Whereas simply doing x % PI_TIMES_2 is the 2e-3 range.
		static void CalculateLargePI(out Fix64 largePIF64, out int N) {
			int prevN = 0;
			N = 0;
			decimal largePI = 0;
			decimal prevLargePI = 0;

			do {
				prevN = N++;
				prevLargePI = largePI;

				decimal sqrd = 2;
				for (int i = 1; i < N; i++) {
					sqrd *= 2;
				}
				largePI = sqrd * (decimal) Math.PI;
			}
			while (largePI < (decimal) MaxValue);
			N = prevN;
			largePIF64 = prevLargePI;
		}

		// turn into a Console Application and use this to generate the look-up tables
		[UnityEditor.MenuItem("Tools/Fix64 Regenerate LUT")]
        static void GenerateLut() {
			string thisFile = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
			thisFile = thisFile.Replace('\\', '/');
			string path = thisFile.Substring(0, thisFile.LastIndexOf('/'));
            GenerateSinLut(path);
            GenerateTanLut(path);
			UnityEditor.AssetDatabase.Refresh();
        }
#endif
	}
}
