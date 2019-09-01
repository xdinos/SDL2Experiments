using System;
using SDL2;

namespace SDL2Test
{
	public class GameObject
	{
		public GameObject(IntPtr rendererPtr, IntPtr texturePtr, int tx, int ty, int tw, int th)
		{
			_rendererPtr = rendererPtr;
			_texturePtr = texturePtr;

			_srcRect.x = tx;
			_srcRect.y = ty;
			_srcRect.w = tw;
			_srcRect.h = th;

			_dstRect.x = 0;
			_dstRect.y = 0;
			_dstRect.w = 2 * tw;
			_dstRect.h = 2 * th;
		}

		public void Update()
		{
			_dstRect.x = 0;
			_dstRect.y = 0;
		}

		public void Render()
		{
			SDL.SDL_RenderCopy(_rendererPtr, _texturePtr, ref _srcRect, ref _dstRect);
		}

		private SDL.SDL_Rect _srcRect;
		private SDL.SDL_Rect _dstRect;
		private readonly IntPtr _texturePtr;
		private readonly IntPtr _rendererPtr;
	}
}