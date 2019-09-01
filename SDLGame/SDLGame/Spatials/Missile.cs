using System;
using Lunatics.Graphics;
using SDL2;
using SDLGame.Component;

namespace SDLGame.Spatials
{
	internal static class Missile
	{
		private static Texture2D _texture;

		public static void Render(IntPtr rendererPtr, TransformComponent transformComponent)
		{
			if(_texture == null)
				_texture = Texture2D.Load(rendererPtr, "assets/bullet.png");
			
			var dst = new SDL.SDL_Rect { h = _texture.Height, w = _texture.Width };
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