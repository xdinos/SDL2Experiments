using System;
using System.Collections.Generic;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using Newtonsoft.Json.Linq;

namespace SDLGame.AdvEn
{
	public class Sprite
	{
		public string Name { get; }
		
		public TextureRegion2D Texture { get; }
		public Size Size { get; }
		 
		public Vector2 Offset { get; }
		public Vector2 Pivot { get; }

		public Sprite(string name, 
		              TextureRegion2D texture, 
					  Size size,
		              Vector2 offset, 
		              Vector2 pivot)
		{
			Name = name;
			Texture = texture;
			Size = size;
			Offset = offset;
			Pivot = pivot;
		}

		public void Draw(Renderer renderer, Vector2 position, Vector2 scale)
		{
			var scaledSize = new Size((int) (Size.Width * scale.X),
			                          (int) (Size.Height * scale.Y));
			var scaledOffset = new Vector2(Offset.X * scale.X, Offset.Y * scale.Y);
			var pos = new Vector2(
				(int) (position.X + scaledOffset.X - Pivot.X * scaledSize.Width),
				(int) (position.Y + scaledOffset.Y - Pivot.Y * scaledSize.Height));
			renderer.Draw(Texture,
			              pos,
			              Vector2.Zero, 
			              0, 
			              scale);
		}
	}

	public class SpriteSheet
	{
		public string Name { get; }

		public SpriteSheet(string name, Texture2D texture)
		{
			Name = name;
			_texture = texture;
		}

		public Sprite CreateSprite(string name, 
		                           Rectangle frame, 
								   Size size,
		                           Vector2 offset, 
		                           Vector2 pivot) 
		{
			if (_sprites.ContainsKey(name))
				throw new InvalidOperationException($"Sprite {name} already exists in the sprite sheet");

			var sprite = new Sprite(name,
			                        new TextureRegion2D(name, _texture, frame.X, frame.Y, frame.Width, frame.Height),
									size,
			                        offset,
			                        pivot);
			_sprites.Add(sprite.Name, sprite);
			return sprite;
		}

		private readonly Texture2D _texture;
		private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

		public Sprite GetSprite(string name)
		{
			return _sprites[name];
		}
	}
}