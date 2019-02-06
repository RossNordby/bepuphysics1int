using BEPUutilities;

using FloatMatrix3x3 = BEPUutilitiesFloat.Matrix3x3;
using FloatMatrix = BEPUutilitiesFloat.Matrix;
namespace BEPUtests.util
{
	static class MathConverter
	{
		public static FloatMatrix3x3 Convert(Matrix3x3 matrix)
		{
			FloatMatrix3x3 toReturn;
			Convert(ref matrix, out toReturn);
			return toReturn;
		}

		public static void Convert(ref Matrix3x3 matrix, out FloatMatrix3x3 floatMatrix)
		{
			floatMatrix.M11 = (float)matrix.M11.ToFloat();
			floatMatrix.M12 = (float)matrix.M12.ToFloat();
			floatMatrix.M13 = (float)matrix.M13.ToFloat();

			floatMatrix.M21 = (float)matrix.M21.ToFloat();
			floatMatrix.M22 = (float)matrix.M22.ToFloat();
			floatMatrix.M23 = (float)matrix.M23.ToFloat();

			floatMatrix.M31 = (float)matrix.M31.ToFloat();
			floatMatrix.M32 = (float)matrix.M32.ToFloat();
			floatMatrix.M33 = (float)matrix.M33.ToFloat();
		}

		public static FloatMatrix Convert(Matrix matrix)
		{
			FloatMatrix toReturn;
			Convert(ref matrix, out toReturn);
			return toReturn;
		}

		public static void Convert(ref Matrix matrix, out FloatMatrix floatMatrix)
		{
			floatMatrix.M11 = (float)matrix.M11.ToFloat();
			floatMatrix.M12 = (float)matrix.M12.ToFloat();
			floatMatrix.M13 = (float)matrix.M13.ToFloat();
			floatMatrix.M14 = (float)matrix.M14.ToFloat();

			floatMatrix.M21 = (float)matrix.M21.ToFloat();
			floatMatrix.M22 = (float)matrix.M22.ToFloat();
			floatMatrix.M23 = (float)matrix.M23.ToFloat();
			floatMatrix.M24 = (float)matrix.M24.ToFloat();

			floatMatrix.M31 = (float)matrix.M31.ToFloat();
			floatMatrix.M32 = (float)matrix.M32.ToFloat();
			floatMatrix.M33 = (float)matrix.M33.ToFloat();
			floatMatrix.M34 = (float)matrix.M34.ToFloat();

			floatMatrix.M41 = (float)matrix.M41.ToFloat();
			floatMatrix.M42 = (float)matrix.M42.ToFloat();
			floatMatrix.M43 = (float)matrix.M43.ToFloat();
			floatMatrix.M44 = (float)matrix.M44.ToFloat();
		}

	}
}
