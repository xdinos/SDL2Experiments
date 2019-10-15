using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public struct Viewport
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public Rectangle Bounds
		{
			get => new Rectangle(X, Y, Width, Height);
			set
			{
				X = value.X;
				Y = value.Y;
				Width = value.Width;
				Height = value.Height;
			}
		}

		public Viewport(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			minDepth = 0.0f;
			maxDepth = 1.0f;
		}

		public Viewport(Rectangle bounds)
		{
			X = bounds.X;
			Y = bounds.Y;
			Width = bounds.Width;
			Height = bounds.Height;
			minDepth = 0.0f;
			maxDepth = 1.0f;
		}

		private float minDepth;
		private float maxDepth;
	}
}