using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public class DisplayMode
	{
		public float AspectRatio => (float)Width / (float)Height;

		public SurfaceFormat Format { get; }

		public int Height { get; }

		public int Width { get; } 

		public Rectangle TitleSafeArea => new Rectangle(0, 0, Width, Height);

		public DisplayMode(int width, int height, SurfaceFormat format)
		{
			Width = width;
			Height = height;
			Format = format;
		}

		public static bool operator !=(DisplayMode left, DisplayMode right)
		{
			return !(left == right);
		}

		public static bool operator ==(DisplayMode left, DisplayMode right)
		{
			if (ReferenceEquals(left, right)) // Same object or both are null
			{
				return true;
			}
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
			{
				return false;
			}
			return ((left.Format == right.Format) &&
			        (left.Height == right.Height) &&
			        (left.Width == right.Width));
		}

		public override bool Equals(object obj)
		{
			return (obj as DisplayMode) == this;
		}

		public override int GetHashCode()
		{
			return (Width.GetHashCode() ^ Height.GetHashCode() ^ Format.GetHashCode());
		}

		public override string ToString()
		{
			return $"{{Width: {Width} Height:{Height} Format:{Format}}}";
		}
	}
}