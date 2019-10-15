using System;
using System.Runtime.InteropServices;
using Lunatics.Mathematics;

namespace Lunatics.Framework
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Color : IEquatable<Color>
	{
		public static readonly Color Transparent = new Color(0);
		public static readonly Color White = new Color(1f, 1f, 1f, 1f);

		#region Properties

		public byte B
		{
			get
			{
				unchecked
				{
					return (byte) (PackedValue >> 16);
				}
			}
			set => PackedValue = (PackedValue & 0xff00ffff) | ((uint) value << 16);
		}

		public byte G
		{
			get
			{
				unchecked
				{
					return (byte) (PackedValue >> 8);
				}
			}
			set => PackedValue = (PackedValue & 0xffff00ff) | ((uint) value << 8);
		}

		public byte R
		{
			get
			{
				unchecked
				{
					return (byte) (PackedValue >> 0);
				}
			}
			set => PackedValue = (PackedValue & 0xffffff00) | ((uint) value << 0);
		}

		public byte A
		{
			get
			{
				unchecked
				{
					return (byte) (PackedValue >> 24);
				}
			}
			set => PackedValue = (PackedValue & 0x00ffffff) | ((uint) value << 24);
		}

		public uint PackedValue { get; set; }

		#endregion

		public Color(uint packedValue)
		{
			PackedValue = packedValue;
		}

		public Color(Vector4 color) : this(color.X, color.Y, color.Z, color.W)
		{
		}

		public Color(int r, int g, int b) : this(r, g, b, 255)
		{
		}

		public Color(int r, int g, int b, int alpha)
		{
			PackedValue = 0;
			R = (byte) MathUtils.Clamp(r, byte.MinValue, byte.MaxValue);
			G = (byte) MathUtils.Clamp(g, byte.MinValue, byte.MaxValue);
			B = (byte) MathUtils.Clamp(b, byte.MinValue, byte.MaxValue);
			A = (byte) MathUtils.Clamp(alpha, byte.MinValue, byte.MaxValue);
		}

		public Color(float r, float g, float b) : this(r, g, b, 1f)
		{
		}

		public Color(float r, float g, float b, float alpha)
		{
			PackedValue = 0;

			R = (byte) MathUtils.Clamp(r * 255, byte.MinValue, byte.MaxValue);
			G = (byte) MathUtils.Clamp(g * 255, byte.MinValue, byte.MaxValue);
			B = (byte) MathUtils.Clamp(b * 255, byte.MinValue, byte.MaxValue);
			A = (byte) MathUtils.Clamp(alpha * 255, byte.MinValue, byte.MaxValue);
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Color a, Color b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return PackedValue.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is Color color) && Equals(color);
		}

		public bool Equals(Color other)
		{
			return PackedValue == other.PackedValue;
		}
	}
}