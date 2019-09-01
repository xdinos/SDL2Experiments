using System;
using System.Drawing;
using Lunatics.Math;

namespace Lunatics.Graphics
{
	public class TextureRegion2D
	{
		public Texture2D Texture { get; protected set; }
		public int X { get; }
		public int Y { get; }
		public int Width { get; }
		public int Height { get; }

		public TextureRegion2D(Texture2D texture)
			: this(texture, 0, 0, texture.Width, texture.Height)
		{
		}

		public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
		{
			Texture = texture;
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
	}

	public class Sprite
	{
		public TextureRegion2D TextureRegion => _textureRegion;

		public Sprite(TextureRegion2D textureRegion)
		{
			_textureRegion = textureRegion ?? throw new ArgumentNullException(nameof(textureRegion));

			//Alpha = 1.0f;
			//Color = Color.White;
			//IsVisible = true;
			//Effect = SpriteEffects.None;
			//OriginNormalized = new Vector2(0.5f, 0.5f);
			//Depth = 0.0f;
		}


		public Sprite(Texture2D texture)
			: this(new TextureRegion2D(texture))
		{
		}

		private TextureRegion2D _textureRegion;
	}
}