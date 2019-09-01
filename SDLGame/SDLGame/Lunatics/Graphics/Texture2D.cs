using System;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;

namespace Lunatics.Graphics
{
	public class Texture2D : IDisposable
	{
		public static Texture2D Load(IntPtr renderer, string path)
		{
			var handle = new GCHandle();
			var surfacePtr = IntPtr.Zero;

			try
			{
				var buffer = File.ReadAllBytes(path);
				var imgResult = StbImageSharp.ImageResult.FromMemory(buffer, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

				handle = GCHandle.Alloc(imgResult.Data, GCHandleType.Pinned);
				surfacePtr = SDL.SDL_CreateRGBSurfaceFrom(handle.AddrOfPinnedObject(),
				                                          imgResult.Width, imgResult.Height,
				                                          32, imgResult.Width * 4,
				                                          0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

				return new Texture2D(SDL.SDL_CreateTextureFromSurface(renderer, surfacePtr), imgResult.Width, imgResult.Height);

			}
			finally
			{
				if (handle.IsAllocated)
					handle.Free();

				if (surfacePtr != IntPtr.Zero)
					SDL.SDL_FreeSurface(surfacePtr);
			}
		}

		public int Width { get; }
		public int Height { get; }

		internal IntPtr Handle { get; private set; }

		internal Texture2D(IntPtr handle, int width,int height)
		{
			Handle = handle;
			Width = width;
			Height = height;
		}

		public void Dispose()
		{
			if (Handle == IntPtr.Zero) return;
			SDL.SDL_DestroyTexture(Handle);
			Handle = IntPtr.Zero;
		}
	}
}