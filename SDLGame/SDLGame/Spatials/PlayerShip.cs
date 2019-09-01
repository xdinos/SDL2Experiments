using System;
using Lunatics.Graphics;
using SDLGame.Component;
using SDL2;

namespace SDLGame.Spatials
{
	/// <summary>The player ship.</summary>
	internal static class PlayerShip
	{
		private static Texture2D _texture;

		public static void Render(IntPtr rendererPtr, TransformComponent transformComponent)
		{
			if (_texture == null)
				_texture = Texture2D.Load(rendererPtr, "assets/player.png");

			var dst = new SDL.SDL_Rect {h = _texture.Height, w = _texture.Width};
			dst.x = (int)(transformComponent.X - (dst.w * 0.5f));
			dst.y = (int)(transformComponent.Y - (dst.h * 0.5f));

			SDL.SDL_RenderCopyEx(rendererPtr, 
			                     _texture.Handle,
			                     IntPtr.Zero, 
			                     ref dst, 
			                     0D, IntPtr.Zero, 
			                     SDL.SDL_RendererFlip.SDL_FLIP_NONE);
		}
	}
}