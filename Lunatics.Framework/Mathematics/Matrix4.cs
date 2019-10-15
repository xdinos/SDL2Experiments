using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework.Mathematics
{
	/// <summary>
	/// Represents a 4x4 matrix containing 3D rotation, scale, transform, and projection.
	/// </summary>
	/// <seealso cref="Matrix4d"/>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix4 : IEquatable<Matrix4>
	{
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix4 Identity = new Matrix4(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);

		/// <summary>
		/// The zero matrix.
		/// </summary>
		public static readonly Matrix4 Zero = new Matrix4(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

		/// <summary>
		/// Top row of the matrix.
		/// </summary>
		public Vector4 Row0;

		/// <summary>
		/// 2nd row of the matrix.
		/// </summary>
		public Vector4 Row1;

		/// <summary>
		/// 3rd row of the matrix.
		/// </summary>
		public Vector4 Row2;

		/// <summary>
		/// Bottom row of the matrix.
		/// </summary>
		public Vector4 Row3;

		public Matrix4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
		{
			Row0 = row0;
			Row1 = row1;
			Row2 = row2;
			Row3 = row3;
		}

		public Matrix4(float m00, float m01, float m02, float m03,
		               float m10, float m11, float m12, float m13,
		               float m20, float m21, float m22, float m23,
		               float m30, float m31, float m32, float m33)
		{
			Row0 = new Vector4(m00, m01, m02, m03);
			Row1 = new Vector4(m10, m11, m12, m13);
			Row2 = new Vector4(m20, m21, m22, m23);
			Row3 = new Vector4(m30, m31, m32, m33);
		}

		//public Matrix4(Matrix3 topLeft)
		//{
		//	Row0.X = topLeft.Row0.X;
		//	Row0.Y = topLeft.Row0.Y;
		//	Row0.Z = topLeft.Row0.Z;
		//	Row0.W = 0;
		//	Row1.X = topLeft.Row1.X;
		//	Row1.Y = topLeft.Row1.Y;
		//	Row1.Z = topLeft.Row1.Z;
		//	Row1.W = 0;
		//	Row2.X = topLeft.Row2.X;
		//	Row2.Y = topLeft.Row2.Y;
		//	Row2.Z = topLeft.Row2.Z;
		//	Row2.W = 0;
		//	Row3.X = 0;
		//	Row3.Y = 0;
		//	Row3.Z = 0;
		//	Row3.W = 1;
		//}


		public static bool operator ==(Matrix4 left, Matrix4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Matrix4 left, Matrix4 right)
		{
			return !left.Equals(right);
		}

		//public override bool Equals(object obj)
		//{
		//	if (!(obj is Matrix4))
		//	{
		//		return false;
		//	}

		//	return Equals((Matrix4)obj);
		//}


		public bool Equals(Matrix4 other)
		{
			return
				Row0 == other.Row0 &&
				Row1 == other.Row1 &&
				Row2 == other.Row2 &&
				Row3 == other.Row3;
		}

	}
}