using System;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;

namespace Lunatics.Graphics
{
	public class Texture2D : IDisposable
	{
		public static Texture2D Load(Renderer renderer, string path)
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

				return new Texture2D(path,
				                     SDL.SDL_CreateTextureFromSurface(renderer.Handle, surfacePtr),
				                     imgResult.Width,
				                     imgResult.Height);

			}
			finally
			{
				if (handle.IsAllocated)
					handle.Free();

				if (surfacePtr != IntPtr.Zero)
					SDL.SDL_FreeSurface(surfacePtr);
			}
		}

		public string Name { get; }

		public int Width { get; }
		public int Height { get; }

		internal IntPtr Handle { get; private set; }

		internal Texture2D(string name, IntPtr handle, int width,int height)
		{
			Name = name;
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