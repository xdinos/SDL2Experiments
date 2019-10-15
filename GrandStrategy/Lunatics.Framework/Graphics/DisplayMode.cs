namespace Lunatics.Framework.Graphics
{
	public class DisplayMode
	{
		public float AspectRatio => (float) Width / (float) Height;

		public SurfaceFormat Format { get; }

		public int Height { get; }

		public int Width { get; }

		public DisplayMode(int width, int height, SurfaceFormat format)
		{
			Width = width;
			Height = height;
			Format = format;
		}

		public static bool operator ==(DisplayMode left, DisplayMode right)
		{
			if (ReferenceEquals(left, right)) //Same object or both are null
			{
				return true;
			}
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
			{
				return false;
			}
			return left.Format == right.Format &&
			       left.Height == right.Height &&
			       left.Width == right.Width;
		}

		public static bool operator !=(DisplayMode left, DisplayMode right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			var mode = obj as DisplayMode;
			return mode != null && this == mode;
		}

		public override int GetHashCode()
		{
			return Width.GetHashCode() ^ Height.GetHashCode() ^ Format.GetHashCode();
		}
	}
}