using Lunatics.Framework.Mathematics;
using Rectangle = System.Drawing.Rectangle;

namespace Lunatics.Graphics
{
	public class TextureRegion2D
	{
		public string Name { get; }
		public int X { get; }
		public int Y { get; }
		public int Width { get; }
		public int Height { get; }
		public Texture2D Texture { get; protected set; }

		public TextureRegion2D(Texture2D texture)
			: this(null, texture, 0, 0, texture.Width, texture.Height)
		{
		}

		public TextureRegion2D(Texture2D texture, Rectangle region)
			: this(null, texture, region)
		{
		}

		public TextureRegion2D(string name, Texture2D texture, Rectangle region)
			: this(name, texture, region.X, region.Y, region.Width, region.Height)
		{
		}

		public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
			: this(null, texture, x, y, width, height)
		{
		}

		public TextureRegion2D(string name, Texture2D texture, int x, int y, int width, int height)
		{
			Name = name;
			Texture = texture;
			X = x;	
			Y = y;
			Width = width;
			Height = height;
		}
	}
}