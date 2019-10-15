using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using SDLGame.Component;

namespace SDLGame.Spatials
{
	internal static class Explosion
	{
		private static Sprite _sprite;

		public static void Render(Renderer renderer, TransformComponent transformComponent, int radius)
		{
			if (_sprite == null)
				_sprite = new Sprite(Texture2D.Load(renderer, "assets/explosion.png"));

			var position = new Vector2(transformComponent.X - radius, transformComponent.Y - radius);
			renderer.Draw(_sprite, position, Vector2.Zero, 0f, new Vector2(0.3f));
		}
	}
}