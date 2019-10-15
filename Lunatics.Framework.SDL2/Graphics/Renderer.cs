using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Lunatics.Framework.Mathematics;
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

		public void SetViewPort(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			var result = SDL.SDL_RenderSetViewport(_handle, ref rect);
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

		public void SetDrawColor(byte r, byte g, byte b, byte alpha)
		{
			SDL.SDL_SetRenderDrawColor(_handle, r, g, b, alpha);
		}

		public void DrawLine(Vector2 p0, Vector2 p1)
		{
			DrawLine(p0, p1, Vector2.One);
		}

		public void DrawLine(Vector2 p0, Vector2 p1, Vector2 scale)
		{
			SDL.SDL_RenderDrawLine(_handle,
			                       (int) (p0.X * scale.X),
			                       (int) (p0.Y * scale.Y),
			                       (int) (p1.X * scale.X),
			                       (int) (p1.Y * scale.Y));
		}



		public void DrawLines(IEnumerable<Vector2> points)
		{
			DrawLines(points, Vector2.One);
		}

		public void DrawLines(IEnumerable<Vector2> points, Vector2 scale)
		{
			var pts = points.Select(x => new SDL.SDL_FPoint {x = x.X * scale.X, y = x.Y * scale.Y}).ToArray();
			SDL.SDL_RenderDrawLinesF(_handle, pts, pts.Length);
		}

		public void DrawRect(float x, float y, float w, float h)
		{
			DrawRect(x, y, w, h, Vector2.One);
		}

		public void DrawRect(float x, float y, float w, float h, Vector2 scale)
		{
			var rect = new SDL.SDL_Rect
			           {
				           x = (int) (x * scale.X),
				           y = (int) (y * scale.Y),
				           w = (int) (w * scale.X),
				           h = (int) (h * scale.Y),
			           };
			SDL.SDL_RenderDrawRect(_handle, ref rect);
		}

		public void Draw(TextureRegion2D textureRegion, 
		                 Vector2 position, 
		                 Vector2 origin,
		                 float rotation,
		                 Vector2 scale)
		{
			var src = new SDL.SDL_Rect
			          {
				          x = textureRegion.X,
				          y = textureRegion.Y,
				          w = textureRegion.Width,
				          h = textureRegion.Height,
			          };

			var scaledWidth = src.w * scale.X;
			var scaledHeight = src.h * scale.Y;
			var dst = new SDL.SDL_Rect
			          {
				          x = (int)(position.X - origin.X * scaledWidth),
				          y = (int)(position.Y - origin.Y * scaledHeight),
				          w = (int)scaledWidth,
				          h = (int)scaledHeight
			          };

			SDL.SDL_RenderCopyEx(_handle,
			                     textureRegion.Texture.Handle,
			                     ref src,
			                     ref dst,
			                     0D,
			                     IntPtr.Zero,
			                     SDL.SDL_RendererFlip.SDL_FLIP_NONE);
		}

		public void Draw(Sprite sprite,
		                 Vector2 position)
		{
			Draw(sprite, position, Vector2.Zero, 0f, Vector2.One);
		}

		public void Draw(Sprite sprite,
		                 Vector2 position,
		                 Vector2 origin,
		                 float rotation,
						 Vector2 scale)
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

			var scaledWidth = src.w * scale.X;
			var scaledHeight = src.h * scale.Y;
			var dst = new SDL.SDL_Rect
			          {
				          x = (int) (position.X - origin.X * scaledWidth),
				          y = (int) (position.Y - origin.Y * scaledHeight),
				          w = (int) scaledWidth,
				          h = (int) scaledHeight
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