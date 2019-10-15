using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Lunatics.Framework
{
	/// <summary>
	/// Represents a color with 4 floating-point components (R, G, B, A).
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Color4 : IEquatable<Color4>
	{
		/// <summary>
		/// The red component of this Color4 structure.
		/// </summary>
		public float R;

		/// <summary>
		/// The green component of this Color4 structure.
		/// </summary>
		public float G;

		/// <summary>
		/// The blue component of this Color4 structure.
		/// </summary>
		public float B;

		/// <summary>
		/// The alpha component of this Color4 structure.
		/// </summary>
		public float A;

		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> struct.
		/// </summary>
		/// <param name="r">The red component of the new Color4 structure.</param>
		/// <param name="g">The green component of the new Color4 structure.</param>
		/// <param name="b">The blue component of the new Color4 structure.</param>
		/// <param name="a">The alpha component of the new Color4 structure.</param>
		public Color4(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> struct.
		/// </summary>
		/// <param name="r">The red component of the new Color4 structure.</param>
		/// <param name="g">The green component of the new Color4 structure.</param>
		/// <param name="b">The blue component of the new Color4 structure.</param>
		/// <param name="a">The alpha component of the new Color4 structure.</param>
		public Color4(byte r, byte g, byte b, byte a)
		{
			R = r / (float)byte.MaxValue;
			G = g / (float)byte.MaxValue;
			B = b / (float)byte.MaxValue;
			A = a / (float)byte.MaxValue;
		}

		/// <summary>
		/// Converts this color to an integer representation with 8 bits per channel.
		/// </summary>
		/// <returns>A <see cref="int"/> that represents this instance.</returns>
		/// <remarks>
		/// This method is intended only for compatibility with System.Drawing. It compresses the color into 8 bits per
		/// channel, which means color information is lost.
		/// </remarks>
		public int ToArgb()
		{
			var value =
				((uint)(A * byte.MaxValue) << 24) |
				((uint)(R * byte.MaxValue) << 16) |
				((uint)(G * byte.MaxValue) << 8) |
				(uint)(B * byte.MaxValue);

			return unchecked((int)value);
		}

		/// <summary>
		/// Compares whether this Color4 structure is equal to the specified Color4.
		/// </summary>
		/// <param name="other">The Color4 structure to compare to.</param>
		/// <returns>True if both Color4 structures contain the same components; false otherwise.</returns>
		[Pure]
		public bool Equals(Color4 other)
		{
			return R == other.R &&
			       G == other.G &&
			       B == other.B &&
			       A == other.A;
		}
	}
}