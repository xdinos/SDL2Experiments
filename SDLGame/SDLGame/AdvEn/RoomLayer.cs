using System.Collections.Generic;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;

namespace SDLGame.AdvEn
{
	public class RoomLayer
	{
		public int ZSort { get; set; }
		public Vector2 Parallax { get; set; }

		public Sprite Sprite => _sprites[0];

		public void Draw(Renderer renderer, Vector2 position, Vector2 scale)
		{
			if (_sprites.Count == 0)
			{
				_sprites[0].Draw(renderer, position, scale);
			}
			else
			{
				//_sprites[0].Draw(renderer, position, scale);

				var scaledSize = new Size((int) (_size.Width * scale.X),
				                          (int) (_size.Height * scale.Y));
				var pos = position;
				foreach (var sprite in _sprites)
				{
					var scaledOffset = new Vector2(sprite.Offset.X * scale.X, sprite.Offset.Y * scale.Y);
					renderer.Draw(sprite.Texture, pos + scaledOffset, Vector2.Zero, 0, scale);
					pos.X += sprite.Size.Width * scale.X;

					//	var scaledOffset = new Vector2(Offset.X * scale.X, Offset.Y * scale.Y);
					//	var pos = new Vector2(
					//		(int)(position.X + scaledOffset.X - Pivot.X * scaledSize.Width),
					//		(int)(position.Y + scaledOffset.Y - Pivot.Y * scaledSize.Height));
					//	renderer.Draw(Texture,
					//	              pos,
					//	              Vector2.Zero,
					//	              0,
					//	              scale);
				}
			}
		}

		public void AddSprite(Sprite sprite)
		{
			_sprites.Add(sprite);
			_size = new Size(_size.Width + sprite.Size.Width,
			                 _size.Height + sprite.Size.Height);
		}

		private Size _size = Size.Zero;
		private readonly List<Sprite> _sprites = new List<Sprite>();
	}
}