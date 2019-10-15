using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework.Mathematics
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4 : IEquatable<Vector4>
	{
		#region Static

		#region Fields

		/// <summary>
		/// Defines a unit-length Vector4 that points towards the X-axis.
		/// </summary>
		public static readonly Vector4 UnitX = new Vector4(1f, 0f, 0f, 0f);

		/// <summary>
		/// Defines a unit-length Vector4 that points towards the Y-axis.
		/// </summary>
		public static readonly Vector4 UnitY = new Vector4(0f, 1f, 0f, 0f);

		/// <summary>
		/// Defines a unit-length Vector4 that points towards the Z-axis.
		/// </summary>
		public static readonly Vector4 UnitZ = new Vector4(0f, 0f, 1f, 0f);

		/// <summary>
		/// Defines a unit-length Vector4 that points towards the W-axis.
		/// </summary>
		public static readonly Vector4 UnitW = new Vector4(0f, 0f, 0f, 1f);

		/// <summary>
		/// Defines a zero-length Vector4.
		/// </summary>
		public static readonly Vector4 Zero = new Vector4(0f, 0f, 0f, 0f);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector4 One = new Vector4(1f, 1f, 1f, 1f);

		/// <summary>
		/// Defines the size of the Vector4 struct in bytes.
		/// </summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(new Vector4());

		#endregion

		#endregion

		#region Fields

		/// <summary>
		/// The X component of the Vector4.
		/// </summary>
		public float X;

		/// <summary>
		/// The Y component of the Vector4.
		/// </summary>
		public float Y;

		/// <summary>
		/// The Z component of the Vector4.
		/// </summary>
		public float Z;

		/// <summary>
		/// The W component of the Vector4.
		/// </summary>
		public float W;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector4(float value)
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector4"/> struct.
		/// </summary>
		/// <param name="x">The x component of the Vector4.</param>
		/// <param name="y">The y component of the Vector4.</param>
		/// <param name="z">The z component of the Vector4.</param>
		/// <param name="w">The w component of the Vector4.</param>
		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector4"/> struct.
		/// </summary>
		/// <param name="v">The Vector2 to copy components from.</param>
		public Vector4(Vector2 v)
		{
			X = v.X;
			Y = v.Y;
			Z = 0.0f;
			W = 0.0f;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector4"/> struct.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		/// <remarks>
		///  .<seealso cref="Vector4(Vector3, float)"/>
		/// </remarks>
		public Vector4(Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = 0.0f;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector4"/> struct.
		/// </summary>
		/// <param name="v">The Vector3 to copy components from.</param>
		/// <param name="w">The w component of the new Vector4.</param>
		public Vector4(Vector3 v, float w)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = w;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector4"/> struct.
		/// </summary>
		/// <param name="v">The Vector4 to copy components from.</param>
		public Vector4(Vector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
			W = v.W;
		}

		#endregion

		#region Operators
		public static bool operator ==(Vector4 left, Vector4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector4 left, Vector4 right)
		{
			return !left.Equals(right);
		}

		#endregion

		#region IEquatable<Vector4> Members

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector4 other)
		{
			return X == other.X &&
			       Y == other.Y &&
			       Z == other.Z &&
			       W == other.W;
		}

		#endregion
	}
}