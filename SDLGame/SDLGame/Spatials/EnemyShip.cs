using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using SDLGame.Component;

namespace SDLGame.Spatials
{
	internal static class EnemyShip
	{
		private static Sprite _sprite;

		public static void Render(Renderer renderer, TransformComponent transformComponent)
		{
			if (_sprite == null)
				_sprite =new Sprite(Texture2D.Load(renderer, "assets/enemy.png"));
			
			renderer.Draw(_sprite, transformComponent.Position, new Vector2(0.5f), 0f, Vector2.One);
		}
	}
}