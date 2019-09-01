using System;
using System.Runtime.InteropServices;
using Lunatics.Math;
using SDL2;

namespace Lunatics.Graphics
{
	public class Renderer : IDisposable
	{
		internal IntPtr Handle => _handle;

		internal static Renderer CreateRenderer(IntPtr windowHandle)
		{
			return new Renderer(SDL.SDL_CreateRenderer(windowHandle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED));

		}

		internal Renderer(IntPtr handle)
		{
			_handle = handle;

			SDL.SDL_GetRendererInfo(_handle, out var rendererInfo);
			Console.WriteLine($"Renderer: {Marshal.PtrToStringAnsi(rendererInfo.name)}");
		}

		public void Dispose()
		{
			if (_handle == IntPtr.Zero) return;

			SDL.SDL_DestroyRenderer(_handle);
			_handle = IntPtr.Zero;
		}

		public void Clear(byte r, byte g, byte b, byte alpha)
		{
			SDL.SDL_SetRenderDrawColor(_handle, r, g, b, alpha);
			SDL.SDL_RenderClear(_handle);
		}

		public void Present()
		{
			SDL.SDL_RenderPresent(_handle);
		}

		public void Draw(Sprite sprite, Vector2 position, float rotation, Vector2 scale)
		{
			if (sprite == null) throw new ArgumentNullException(nameof(sprite));

			//if (!sprite.IsVisible) return;

			var src = new SDL.SDL_Rect
			          {
				          x = sprite.TextureRegion.X,
				          y = sprite.TextureRegion.Y,
				          w = sprite.TextureRegion.Width,
				          h = sprite.TextureRegion.Height,
			          };

			var dst = new SDL.SDL_Rect
			          {
				          x = (int) position.X,
				          y = (int) position.Y,
				          w = (int) (sprite.TextureRegion.Width * scale.X),
				          h = (int) (sprite.TextureRegion.Height * scale.Y)
			          };

			SDL.SDL_RenderCopyEx(_handle,
			                     sprite.TextureRegion.Texture.Handle,
			                     ref src,
			                     ref dst,
			                     0D,
			                     IntPtr.Zero,
			                     SDL.SDL_RendererFlip.SDL_FLIP_NONE);
		}
		

		private IntPtr _handle;
	}
}