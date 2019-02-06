//#define CHECKMATH
#pragma warning disable CastToFix
#pragma warning disable CastFromFix

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FixMath.NET
{

	public enum Fix32 : long {
		MaxValue = Fix32Ext.MAX_VALUE,
		MinValue = Fix32Ext.MIN_VALUE,
		MinusOne = -Fix32Ext.ONE,
		One = Fix32Ext.ONE,
		Two = One * 2,
		Three = One * 3,
		Zero = 0,
		Pi = Fix32Ext.PI,
		PiOver2 = Fix32Ext.PI_OVER_2,
		PiOver4 = Fix32Ext.PI_OVER_4,
		PiTimes2 = Fix32Ext.PI_TIMES_2,
	}

    /// <summary>
    /// Represents a Q31.32 fixed-point number.
    /// </summary>
    public static partial class Fix32Ext {
		// Field is public and mutable to allow serialization by XNA Content Pipeline

        // Precision of this type is 2^-32, that is 2,3283064365386962890625E-10
        public static readonly decimal Precision = ((Fix32)(1L)).ToDecimal();//0.00000000023283064365386962890625m;
        public static readonly Fix32 MaxValue = (Fix32)(MAX_VALUE);
        public static readonly Fix32 MinValue = (Fix32)(MIN_VALUE);
		public static readonly Fix32 MinusOne = (Fix32)(-ONE);
		public static readonly Fix32 One = (Fix32)(ONE);
		public static readonly Fix32 Two = 2.ToFix();
		public static readonly Fix32 Three = 3.ToFix();
		public static readonly Fix32 Zero = (Fix32)(0);
		public static readonly Fix32 C0p28 = 0.28m.ToFix();
		/// <summary>
		/// The value of Pi
		/// </summary>
		public static readonly Fix32 Pi = (Fix32)(PI);
        public static readonly Fix32 PiOver2 = (Fix32)(PI_OVER_2);
		public static readonly Fix32 PiOver4 = (Fix32)(PI_OVER_4);
		public static readonly Fix32 PiTimes2 = (Fix32)(PI_TIMES_2);
        public static readonly Fix32 PiInv = 0.3183098861837906715377675267M.ToFix();
        public static readonly Fix32 PiOver2Inv = 0.6366197723675813430755350535M.ToFix();
		public static readonly Fix32 E = (Fix32)(E_RAW);
		public static readonly Fix32 EPow4 = (Fix32)(EPOW4);
		public static readonly Fix32 Ln2 = (Fix32)(LN2);
		public static readonly Fix32 Log2Max = (Fix32)(LOG2MAX);
		public static readonly Fix32 Log2Min = (Fix32)(LOG2MIN);

		static readonly Fix32 LutInterval = ((LUT_SIZE - 1).ToFix()).Div(PiOver2);
		public const long MAX_VALUE = long.MaxValue;
		public const long MIN_VALUE = long.MinValue;
		public const long MAX_INT_VALUE = long.MaxValue >> FRACTIONAL_BITS;
		public const long MIN_INT_VALUE = long.MinValue >> FRACTIONAL_BITS;
		public const int NUM_BITS = 64;
		public const int FRACTIONAL_BITS = 32;
        public const long ONE = 1L << FRACTIONAL_BITS;
        public const long PI_TIMES_2 = 0x6487ED511;
        public const long PI = 0x3243F6A88;
        public const long PI_OVER_2 = 0x1921FB544;
		public const long PI_OVER_4 = 0xC90FDAA2;
		public const long E_RAW = 0x2B7E15162;
		public const long EPOW4 = 0x3699205C4E;
		public const long LN2 = 0xB17217F7;
		public const long LOG2MAX = 0x1F00000000;
		public const long LOG2MIN = -0x2000000000;
		public const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

		/// <summary>
		/// Returns a number indicating the sign of a Fix64 number.
		/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
		/// </summary>
		public static int SignI(this Fix32 value) {
            return
                value < 0 ? -1 :
                value > 0 ? 1 :
                0;
        }

		public static Fix32 Sign(this Fix32 v)
		{
			long raw = (long) v;
			return
				raw < 0 ? MinusOne :
				raw > 0 ? One :
				Fix32Ext.Zero;
		}


		/// <summary>
		/// Returns the absolute value of a Fix64 number.
		/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 Abs(this Fix32 value) {
            if ((long)value == MIN_VALUE) {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = (long) value >> 63;
            return (Fix32)(((long) value + mask) ^ mask);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fix32 Floor(this Fix32 value) {
            // Just zero out the fractional part
            return (Fix32)((long)((ulong) (long) value & 0xFFFFFFFF00000000));
        }

		public static Fix32 Log2(this Fix32 x)
		{
			if ((long) x <= 0)
				throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");

			// This implementation is based on Clay. S. Turner's fast binary logarithm
			// algorithm[1].

			long b = 1U << (FRACTIONAL_BITS - 1);
			long y = 0;

			long rawX = (long) x;
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

			Fix32 z = (Fix32)(rawX);

			for (int i = 0; i < FRACTIONAL_BITS; i++)
			{
				z = z.Mul(z);
				if ((long) z >= (ONE << 1))
				{
					z = (Fix32)((long) z >> 1);
					y += b;
				}
				b >>= 1;
			}

			return (Fix32)(y);
		}

		public static Fix32 Ln(this Fix32 x)
		{
			return Log2(x).Mul(Ln2);
		}

		public static Fix32 Pow2(this Fix32 x)
		{
			if (x == 0) return One;

			// Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
			bool neg = (x < 0);
			if (neg) x = x.Neg();

			if (x == One)
				return neg ? One.Div(Two) : Two;
			if (x >= Log2Max) return neg ? One.Div(MaxValue) : MaxValue;
			if (x <= Log2Min) return neg ? MaxValue : Zero;

			/* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

			int integerPart = Floor(x).ToInt();
			x = FractionalPart(x);

			Fix32 result = One;
			Fix32 term = One;
			int i = 1;
			while (term != 0)
			{
				term = ((x.Mul(term)).Mul(Ln2)).Div(i.ToFix());
				result = result.Add(term);
				i++;
			}

			result = (Fix32)(((long) result) << integerPart);
			if (neg) result = One.Div(result);

			return result;
		}

		public static Fix32 Pow(this Fix32 b, Fix32 exp)
		{
			if (b == One)
				return One;
			if (exp == 0)
				return One;
			if (b == 0)
				return Zero;

			Fix32 log2 = Log2(b);
			return Pow2(SafeMul(exp, log2));
		}

		/// <summary>
		/// Returns the arccos of of the specified number, calculated using Atan and Sqrt
		/// </summary>
		public static Fix32 Acos(this Fix32 x)
		{
			if (x == 0)
				return Fix32Ext.PiOver2;

			Fix32 result = Fix32Ext.Atan(Sqrt(One.Sub(x.Mul(x))).Div(x));
			if (x < 0)
				return result.Add(Fix32Ext.Pi);
			else
				return result;
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified number.
		/// </summary>
		public static Fix32 Ceiling(this Fix32 value) {
            var hasFractionalPart = (((long) value) & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value).Add(One) : value;
        }

		/// <summary>
		/// Returns the fractional part of the specified number.
		/// </summary>
		public static Fix32 FractionalPart(this Fix32 value)
		{
			return (Fix32) (((long)value) & 0x00000000FFFFFFFF);
		}

		/// <summary>
		/// Rounds a value to the nearest integral value.
		/// If the value is halfway between an even and an uneven value, returns the even value.
		/// </summary>
		public static Fix32 Round(this Fix32 value) {
            var fractionalPart = ((long)value) & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);
            if (fractionalPart < 0x80000000) {
                return integralPart;
            }
            if (fractionalPart > 0x80000000) {
                return integralPart.Add(One);
            }
            // if number is halfway between two values, round to the nearest even number
            // this is the method used by System.Math.Round().
            return (((long)integralPart) & ONE) == 0
                       ? integralPart
                       : integralPart.Add(One);
        }

		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 Add(this Fix32 x, Fix32 y) {
#if CHECKMATH
			var xl = RawValue;
            var yl = y.RawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            return new Fix64(sum);
#else
			return (Fix32)((long)x + (long)y);
#endif
		}

		public static Fix32 SafeAdd(this Fix32 x, Fix32 y)
		{
			var xl = (long) x;
			var yl = (long) y;
			var sum = xl + yl;
			// if signs of operands are equal and signs of sum and x are different
			if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
			{
				sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
			}
			return (Fix32) sum;
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 Sub(this Fix32 x, Fix32 y) {
#if CHECKMATH
			var xl = RawValue;
            var yl = y.RawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            return new Fix64(diff);
#else
			return (Fix32)((long)x - (long) y);
#endif
		}

		public static Fix32 SafeSub(this Fix32 x, in Fix32 y)
		{
			var xl = (long)x;
			var yl = (long)y;
			var diff = xl - yl;
			// if signs of operands are different and signs of sum and x are different
			if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
			{
				diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
			}
			return (Fix32)(diff);
		}

        static long AddOverflowHelper(long x, long y, ref bool overflow) {
            var sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 Mul(this Fix32 x, Fix32 y) {
#if CHECKMATH
			var xl = RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual) {
                if (sum < 0 || (overflow && xl > 0)) {
					throw new OverflowException();
                    return MaxValue;
                }
            }
            else {
                if (sum > 0) {
					throw new OverflowException();
					return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/) {
				throw new OverflowException();
				return opSignsEqual ? MaxValue : MinValue; 
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual) {
                long posOp, negOp;
                if (xl > yl) {
                    posOp = xl;
                    negOp = yl;
                }
                else {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE) {
					throw new OverflowException();
					return MinValue;
                }
            }

            return new Fix64(sum);
#else
			var xl = (long)x;
			var yl = (long)y;

			var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
			var xhi = xl >> FRACTIONAL_BITS;
			var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
			var yhi = yl >> FRACTIONAL_BITS;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FRACTIONAL_BITS;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FRACTIONAL_BITS;

			var sum = (long)loResult + midResult1 + midResult2 + hiResult;
			return (Fix32)(sum);
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 Neg(this Fix32 x) {
			return (long) x == MIN_VALUE ? MaxValue : (Fix32)(-(long)x);
		}

		public static Fix32 SafeMul(this Fix32 x, Fix32 y)
		{
			var xl = (long)x;
			var yl = (long)y;

			var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
			var xhi = xl >> FRACTIONAL_BITS;
			var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
			var yhi = yl >> FRACTIONAL_BITS;

			var lolo = xlo * ylo;
			var lohi = (long)xlo * yhi;
			var hilo = xhi * (long)ylo;
			var hihi = xhi * yhi;

			var loResult = lolo >> FRACTIONAL_BITS;
			var midResult1 = lohi;
			var midResult2 = hilo;
			var hiResult = hihi << FRACTIONAL_BITS;

			bool overflow = false;
			var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
			sum = AddOverflowHelper(sum, midResult2, ref overflow);
			sum = AddOverflowHelper(sum, hiResult, ref overflow);

			bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

			// if signs of operands are equal and sign of result is negative,
			// then multiplication overflowed positively
			// the reverse is also true
			if (opSignsEqual)
			{
				if (sum < 0 || (overflow && xl > 0))
				{
					return MaxValue;
				}
			}
			else
			{
				if (sum > 0)
				{
					return MinValue;
				}
			}

			// if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
			// then this means the result overflowed.
			var topCarry = hihi >> FRACTIONAL_BITS;
			if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
			{
				return opSignsEqual ? MaxValue : MinValue;
			}

			// If signs differ, both operands' magnitudes are greater than 1,
			// and the result is greater than the negative operand, then there was negative overflow.
			if (!opSignsEqual)
			{
				long posOp, negOp;
				if (xl > yl)
				{
					posOp = xl;
					negOp = yl;
				}
				else
				{
					posOp = yl;
					negOp = xl;
				}
				if (sum > negOp && negOp < -ONE && posOp > ONE)
				{
					return MinValue;
				}
			}

			return (Fix32)(sum);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        static int CountLeadingZeroes(ulong x) {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        public static Fix32 Div(this Fix32 x, Fix32 y) {
            var xl = (long)x;
            var yl = (long)y;

            if (yl == 0) {
				return Fix32.MaxValue;
                //throw new DivideByZeroException();
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = NUM_BITS / 2 + 1;


            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4) {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0) {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos) {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0) {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0) {
                result = -result;
            }

            return (Fix32)(result);
        }

        public static Fix32 Mod(this Fix32 x, Fix32 y) {
            return (Fix32)(
				(long) x == MIN_VALUE & (long)y == -1 ? 
                0 :
                (long)x % (long)y);
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        public static Fix32 FastMod(this Fix32 x, Fix32 y) {
            return (Fix32)((long)x % (long)y);
        }


        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static Fix32 Sqrt(this Fix32 x) {
            var xl = (long)x;
            if (xl < 0) {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (NUM_BITS - 2);

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
                    if (num > (1UL << (NUM_BITS / 2)) - 1) {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result) {
                ++result;
            }
            return (Fix32)((long)result);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// This function has about 9 decimals of accuracy for small values of x.
        /// It may lose accuracy as the value of x grows.
        /// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
        /// </summary>
        public static Fix32 Sin(this Fix32 x) {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue((long) x, out flipHorizontal, out flipVertical);
            var clamped = (Fix32)(clampedL);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            var rawIndex = clamped.Mul(LutInterval);
            var roundedIndex = Round(rawIndex); 
            var indexError = rawIndex.Sub(roundedIndex);

            var nearestValue = (Fix32)(SinLut[flipHorizontal ? 
                SinLut.Length - 1 - roundedIndex.ToInt() : 
                roundedIndex.ToInt()]);
            var secondNearestValue = (Fix32)(SinLut[flipHorizontal ? 
                SinLut.Length - 1 - roundedIndex.ToInt() - SignI(indexError) : 
                roundedIndex.ToInt() + SignI(indexError)]);

            var delta = (long) (indexError.Mul(Abs(nearestValue.Sub(secondNearestValue))));
            var interpolatedValue = (long) nearestValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return (Fix32)(finalValue);
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static Fix32 FastSin(this Fix32 x) {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue((long) x, out flipHorizontal, out flipVertical);

            // Here we use the fact that the SinLut table has a number of entries
            // equal to (PI_OVER_2 >> 15) to use the angle to index directly into it
            var rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE) {
                rawIndex = LUT_SIZE - 1;
            }
            var nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            return (Fix32)(flipVertical ? -nearestValue : nearestValue);
        }



        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
        static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical) {
            // Clamp value to 0 - 2*PI using modulo; this is very slow but there's no better way AFAIK
            var clamped2Pi = angle % PI_TIMES_2;
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
        public static Fix32 Cos(this Fix32 x) {
            var xl = (long) x;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return Sin((Fix32)(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static Fix32 FastCos(this Fix32 x) {
            var xl = (long)x;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin((Fix32)(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static Fix32 Tan(this Fix32 x) {
            var clampedPi = ((long)x) % PI;
            var flip = false;
            if (clampedPi < 0) {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2) {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = (Fix32)(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            var rawIndex = clamped.Mul(LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = rawIndex.Sub(roundedIndex);

            var nearestValue = (Fix32)(TanLut[roundedIndex.ToInt()]);
            var secondNearestValue = (Fix32) (TanLut[roundedIndex.ToInt() + SignI(indexError)]);

            var delta = (long) (indexError.Mul(Abs(nearestValue.Sub(secondNearestValue))));
            var interpolatedValue = (long) nearestValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;
            return (Fix32) (finalValue);
        }

        public static Fix32 FastAtan2(this Fix32 y, Fix32 x) {
            var yl = (long) y;
            var xl = (long) x;
            if (xl == 0) {
                if (yl > 0) {
                    return PiOver2;
                }
                if (yl == 0) {
                    return Zero;
                }
                return PiOver2.Neg();
            }
            Fix32 atan;
            var z = y.Div(x);

			// Deal with overflow
			if (SafeAdd(One, SafeMul(SafeMul(C0p28, z), z)) == MaxValue) {
				return y < 0 ? PiOver2.Neg() : PiOver2;
            }

            if (Abs(z) < One) {
                atan = z.Div((One.Add((C0p28.Mul(z)).Mul(z))));
                if (xl < 0) {
                    if (yl < 0) {
                        return atan.Sub(Pi);
                    }
                    return atan.Add(Pi);
                }
            }
            else {
                atan = PiOver2.Sub(z.Div(((z.Mul(z)).Add(C0p28))));
                if (yl < 0) {
                    return atan.Sub(Pi);
                }
            }
            return atan;
        }

		/// <summary>
		/// Returns the arctan of of the specified number, calculated using Euler series
		/// </summary>
		public static Fix32 Atan(Fix32 z)
		{
			if (z == 0)
				return Zero;

			// Force positive values for argument
			// Atan(-z) = -Atan(z).
			bool neg = (z < 0);
			if (neg) z = z.Neg();

			Fix32 result;

			if (z == One)
				result = PiOver4;
			else
			{
				bool invert = z > One;
				if (invert) z = One.Div(z);
				
				result = One;
				Fix32 term = One;
				
				Fix32 zSq = z.Mul(z);
				Fix32 zSq2 = zSq.Mul(Two);
				Fix32 zSqPlusOne = zSq.Add(One);
				Fix32 zSq12 = zSqPlusOne.Mul(Two);
				Fix32 dividend = zSq2;
				Fix32 divisor = zSqPlusOne.Mul(Three);

				for (int i = 2; i < 30; i++)
				{
					term = term.Mul(dividend.Div(divisor));
					result = result.Add(term);

					dividend =
dividend.Add(zSq2);
					divisor = divisor.Add(zSq12);

					if (term == 0)
						break;
				}

				result = (result.Mul(z)).Div(zSqPlusOne);

				if (invert)
					result = PiOver2.Sub(result);
			}

			if (neg) result = result.Neg();
			return result;
		}

		public static Fix32 Atan2(this Fix32 y, Fix32 x)
		{
			var yl = (long) y;
			var xl = (long) x;
			if (xl == 0)
			{
				if (yl > 0)
					return PiOver2;
				if (yl == 0)
					return Zero;
				return PiOver2.Neg();
			}

			var z = y.Div(x);

			// Deal with overflow
			if (SafeAdd(One, SafeMul(SafeMul(0.28M.ToFixFast(), z), z)) == MaxValue)
			{
				return y < 0 ? PiOver2.Neg() : PiOver2;
			}
			Fix32 atan = Atan(z);

			if (xl < 0)
			{
				if (yl < 0)
					return atan.Sub(Pi);
				return atan.Add(Pi);
			}

			return atan;
		}

		
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(this Fix32 RawValue) {
			return (float)((long)RawValue) / ONE;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this Fix32 RawValue) {
			return (double) ((long) RawValue) / ONE;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(this Fix32 RawValue) {
			return (int) (((long) RawValue) / ONE);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal ToDecimal(this Fix32 RawValue) {
			return (decimal) ((long) RawValue) / ONE;
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

		

        public static string ToStringFull(this Fix32 v) {
            return (v.ToDecimal()).ToString();
        }

        internal static void GenerateSinLut() {
            using (var writer = new StreamWriter("Fix64SinLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] SinLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var sin = Math.Sin(angle);
                    var rawValue = (long) (sin.ToFix());
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        internal static void GenerateTanLut() {
            using (var writer = new StreamWriter("Fix64TanLut.cs")) {
                writer.Write(
@"namespace FixMath.NET {
    partial struct Fix64 {
        public static readonly long[] TanLut = new[] {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i) {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var tan = Math.Tan(angle);
                    if (tan > MaxValue.ToDouble() || tan < 0.0) {
                        tan = MaxValue.ToDouble();
                    }
                    var rawValue = (long) (((decimal)tan > MaxValue.ToDecimal() || tan < 0.0) ? MaxValue : tan.ToFix());
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFix(this float value) {
			return (Fix32) (long)Clamp(value * Fix32Ext.ONE, Fix32Ext.MIN_VALUE, Fix32Ext.MAX_VALUE);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFixFast(this float value) {
			return (Fix32)(long)(value * Fix32Ext.ONE);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFix(this double value) {
			return (Fix32)(long)Clamp(value * Fix32Ext.ONE, Fix32Ext.MIN_VALUE, Fix32Ext.MAX_VALUE);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFixFast(this double value) {
			return (Fix32)(long)(value * Fix32Ext.ONE);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFix(this int value) {
			return ToFix((long) value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFixFast(this int value) {
			return ToFixFast((long) value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFix(this long value) {
			long v = (value > Fix32Ext.MAX_INT_VALUE ? Fix32Ext.MAX_VALUE : value < Fix32Ext.MIN_INT_VALUE ? Fix32Ext.MIN_VALUE : value * Fix32Ext.ONE);
			return (Fix32)v ;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFixFast(this long value) {
			return (Fix32)(value * Fix32Ext.ONE);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFix(this decimal value) {
			return (Fix32)(long)Clamp(value * Fix32Ext.ONE, Fix32Ext.MIN_VALUE, Fix32Ext.MAX_VALUE);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fix32 ToFixFast(this decimal value) {
			return (Fix32)(long)(value * Fix32Ext.ONE);
		}
	}
}
#pragma warning restore CastToFix
#pragma warning restore CastFromFix
