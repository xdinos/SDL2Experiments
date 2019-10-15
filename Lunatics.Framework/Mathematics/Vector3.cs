using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework.Mathematics
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3 : IEquatable<Vector3>
	{
		#region Fields

		/// <summary>
		/// The X component of the Vector3.
		/// </summary>
		public float X;

		/// <summary>
		/// The Y component of the Vector3.
		/// </summary>
		public float Y;

		/// <summary>
		/// The Z component of the Vector3.
		/// </summary>
		public float Z;

		#endregion

		public Vector3(float value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		#region IEquatable<Vector3> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector3 other)
		{
			return X == other.X &&
			       Y == other.Y &&
			       Z == other.Z;
		}

		#endregion
	}
}