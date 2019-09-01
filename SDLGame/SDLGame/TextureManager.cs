using System;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;

namespace SDLGame
{
	//public class TextureManager
	//{
	//	public static IntPtr LoadTexture(IntPtr rendererPtr, string filePath)
	//	{
	//		var handle = new GCHandle();
	//		var surfacePtr = IntPtr.Zero;

	//		try
	//		{
	//			byte[] buffer = File.ReadAllBytes(filePath);
	//			var imgResult = StbImageSharp.ImageResult.FromMemory(buffer, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
				
	//			handle = GCHandle.Alloc(imgResult.Data, GCHandleType.Pinned);
	//			surfacePtr = SDL.SDL_CreateRGBSurfaceFrom(handle.AddrOfPinnedObject(),
	//			                                          imgResult.Width, imgResult.Height,
	//			                                          32, imgResult.Width * 4,
	//			                                          0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

	//			return SDL.SDL_CreateTextureFromSurface(rendererPtr, surfacePtr);
				
	//		}
	//		finally
	//		{
	//			if (handle.IsAllocated)
	//				handle.Free();

	//			if (surfacePtr != IntPtr.Zero)
	//				SDL.SDL_FreeSurface(surfacePtr);
	//		}
	//	}
	//}
}