using System;
using System.Runtime.InteropServices;
using Lunatics.Framework.Graphics;
using Lunatics.Mathematics;
using Lunatics.SDLGL;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	internal class OpenGLTexture
	{
		public static readonly OpenGLTexture NullTexture = new OpenGLTexture(0, OpenGL.TextureTarget.Texture2D, 0);

		public int Handle { get; }
		public bool HasMipMaps { get; }

		public OpenGL.TextureTarget Target { get; }

		public TextureAddressMode WrapS;
		public TextureAddressMode WrapT;
		public TextureAddressMode WrapR;
		public TextureFilter Filter;
		public float Anistropy;
		public int MaxMipmapLevel;
		public float LODBias;

		internal OpenGLTexture(int handle, OpenGL.TextureTarget target, int levelCount)
		{
			Handle = handle;
			Target = target;
			HasMipMaps = levelCount > 1;

			// TODO: ...

			WrapS = TextureAddressMode.Wrap;
			WrapT = TextureAddressMode.Wrap;
			WrapR = TextureAddressMode.Wrap;
			Filter = TextureFilter.Linear;
			Anistropy = 4.0f;
			MaxMipmapLevel = 0;
			LODBias = 0.0f;
			
		}
	}

	internal sealed class OpenGLTexture2D : Texture2D
	{
		internal OpenGLTexture Texture { get; }

		internal OpenGLTexture2D(GraphicsDevice graphicsDevice,
		                         int width,
		                         int height,
		                         int levelCount,
		                         SurfaceFormat format,
		                         OpenGLTexture texture)
			: base(graphicsDevice, width, height, levelCount, format)
		{
			Texture = texture;
		}

		public override void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			int x, y, w, h;
			if (rect.HasValue)
			{
				x = rect.Value.X;
				y = rect.Value.Y;
				w = rect.Value.Width;
				h = rect.Value.Height;
			}
			else
			{
				x = 0;
				y = 0;
				w = Math.Max(Width >> level, 1);
				h = Math.Max(Height >> level, 1);
			}
			var elementSize = Marshal.SizeOf(typeof(T));
			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

			((OpenGLGraphicsDevice) GraphicsDevice).SetTextureData2D(Texture,
			                                                         Format,
			                                                         x,
			                                                         y,
			                                                         w,
			                                                         h,
			                                                         level,
			                                                         handle.AddrOfPinnedObject() +
			                                                         startIndex * elementSize,
			                                                         elementCount * elementSize);
			handle.Free();
		}

		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				((OpenGLGraphicsDevice) GraphicsDevice).DeleteTexture(Texture);
			}

			base.Dispose(disposing);
		}
	}
}