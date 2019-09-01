using System;

namespace Lunatics.Graphics
{
	public class Sprite
	{
		public TextureRegion2D TextureRegion
		{
			get => _textureRegion;
			protected set
			{
				if (value == null)
					throw new InvalidOperationException("TextureRegion cannot be null");

				// preserve the origin if the texture size changes
				//TODO: var originNormalized = OriginNormalized;
				_textureRegion = value;
				//TODO: OriginNormalized = originNormalized;
			}
		}

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