using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework
{
	/// <summary>
	/// Describes a 32-bit packed color.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Color : IEquatable<Color>
	{
		/// <summary>
		/// Gets or sets the blue component.
		/// </summary>
		public byte B
		{
			get
			{
				unchecked
				{
					return (byte)(packedValue >> 16);
				}
			}
			set => packedValue = (packedValue & 0xff00ffff) | ((uint)value << 16);
		}

		/// <summary>
		/// Gets or sets the green component.
		/// </summary>
		public byte G
		{
			get
			{
				unchecked
				{
					return (byte)(packedValue >> 8);
				}
			}
			set => packedValue = (packedValue & 0xffff00ff) | ((uint)value << 8);
		}

		/// <summary>
		/// Gets or sets the red component.
		/// </summary>
		public byte R
		{
			get
			{
				unchecked
				{
					return (byte)(packedValue);
				}
			}
			set => packedValue = (packedValue & 0xffffff00) | value;
		}

		/// <summary>
		/// Gets or sets the alpha component.
		/// </summary>
		public byte A
		{
			get
			{
				unchecked
				{
					return (byte)(packedValue >> 24);
				}
			}
			set => packedValue = (packedValue & 0x00ffffff) | ((uint)value << 24);
		}

		public uint Value
		{
			get => packedValue;
			set => packedValue = value;
		}

		public Color(byte r, byte g, byte b)
			: this(r, g, b, 255)
		{
		}

		public Color(byte r, byte g, byte b, byte alpha)
		{
			packedValue = 0;
			R = r;
			G = g;
			B = b;
			A = alpha;
		}


		private Color(uint packedValue)
		{
			this.packedValue = packedValue;
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Color a, Color b)
		{
			return !(a == b);
		}

		public bool Equals(Color other)
		{
			return packedValue == other.packedValue;
		}

		private uint packedValue;
	}
}