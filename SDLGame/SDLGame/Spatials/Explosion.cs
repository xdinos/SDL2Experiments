using System;
using Lunatics.Graphics;
using SDL2;
using SDLGame.Component;

namespace SDLGame.Spatials
{
	internal static class Explosion
	{
		private static Texture2D _texture;

		public static void Render(IntPtr rendererPtr, TransformComponent transformComponent, int radius)
		{
			if (_texture == null)
				_texture = Texture2D.Load(rendererPtr, "assets/explosion.png");

			//var src = new SDL.SDL_Rect { h = TileHeight, w = TileWidth, x = tx * TileWidth, y = ty * TileHeight };
			var dst = new SDL.SDL_Rect {h = (int) (_texture.Height * 0.3f), w = (int) (_texture.Width* 0.3f)};
			dst.x = (int)(transformComponent.X - radius);
			dst.y = (int)(transformComponent.Y - radius);

			SDL.SDL_RenderCopyEx(rendererPtr, 
			                     _texture.Handle,
			                     IntPtr.Zero,
			                     ref dst,
			                     0D, IntPtr.Zero,
			                     SDL.SDL_RendererFlip.SDL_FLIP_NONE);
		}
	}
}