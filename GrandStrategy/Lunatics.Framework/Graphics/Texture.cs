using System;
using System.IO;
using Lunatics.Mathematics;
using StbImageSharp;

namespace Lunatics.Framework.Graphics
{
	public interface ITexture : IDisposable
	{
		bool IsDisposed { get; }

		SurfaceFormat Format { get; }
		int LevelCount { get; }
	}

	public static class Texture
	{
		public static int GetFormatSize(SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Dxt1:
					return 8;
				case SurfaceFormat.Dxt3:
				case SurfaceFormat.Dxt5:
					return 16;
				case SurfaceFormat.Alpha8:
					return 1;
				case SurfaceFormat.Bgr565:
				case SurfaceFormat.Bgra4444:
				case SurfaceFormat.Bgra5551:
				case SurfaceFormat.HalfSingle:
				case SurfaceFormat.NormalizedByte2:
					return 2;
				case SurfaceFormat.Color:
				case SurfaceFormat.Single:
				case SurfaceFormat.Rg32:
				case SurfaceFormat.HalfVector2:
				case SurfaceFormat.NormalizedByte4:
				case SurfaceFormat.Rgba1010102:
				// TODO: case SurfaceFormat.ColorBgraEXT:
					return 4;
				case SurfaceFormat.HalfVector4:
				case SurfaceFormat.Rgba64:
				case SurfaceFormat.Vector2:
					return 8;
				case SurfaceFormat.Vector4:
					return 16;
				default:
					throw new ArgumentException($"Should be a value defined in {nameof(SurfaceFormat)}", nameof(format));
			}
		}

		public static int GetPixelStoreAlignment(SurfaceFormat format)
		{
			/*
			 * https://github.com/FNA-XNA/FNA/pull/238
			 * https://www.khronos.org/registry/OpenGL/specs/gl/glspec21.pdf
			 * OpenGL 2.1 Specification, section 3.6.1, table 3.1 specifies that the pixelstorei alignment cannot exceed 8
			 */
			return Math.Min(8, GetFormatSize(format));
		}
	}

	public abstract class Texture2D : GraphicsResource, ITexture
	{
		public int Width { get; }
		public int Height { get; }

		public SurfaceFormat Format { get; protected set; }
		public int LevelCount { get; protected set; }

		protected Texture2D(GraphicsDevice graphicsDevice, int width, int height, int levelCount, SurfaceFormat format)
		{
			GraphicsDevice = graphicsDevice;
			Width = width;
			Height = height;
			LevelCount = levelCount;
			Format = format;
		}

		public void SetData<T>(T[] data) where T : struct
		{
			SetData(0, null, data, 0, data.Length);
		}

		public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
		{
			SetData(0, null, data, startIndex, elementCount);
		}

		public abstract void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount)
			where T : struct;


		private void ValidateParams<T>(int level,
		                               int arraySlice,
		                               Rectangle? rect,
		                               T[] data,
		                               int startIndex,
		                               int elementCount,
		                               out Rectangle checkedRect) where T : struct
		{
			var textureBounds = new Rectangle(0, 0, Math.Max(Width >> level, 1), Math.Max(Height >> level, 1));
			checkedRect = rect ?? textureBounds;
//			if (level < 0 || level >= LevelCount)
//				throw new ArgumentException("level must be smaller than the number of levels in this texture.",
//				                            "level");
//			if (arraySlice > 0 && !GraphicsDevice.GraphicsCapabilities.SupportsTextureArrays)
//				throw new ArgumentException("Texture arrays are not supported on this graphics device", "arraySlice");
//			if (arraySlice < 0 || arraySlice >= ArraySize)
//				throw new ArgumentException(
//					"arraySlice must be smaller than the ArraySize of this texture and larger than 0.", "arraySlice");
//			if (!textureBounds.Contains(checkedRect) || checkedRect.Width <= 0 || checkedRect.Height <= 0)
//				throw new ArgumentException("Rectangle must be inside the texture bounds", "rect");
//			if (data == null)
//				throw new ArgumentNullException("data");
//			var tSize = ReflectionHelpers.SizeOf<T>.Get();
//			var fSize = Format.GetSize();
//			if (tSize > fSize || fSize % tSize != 0)
//				throw new ArgumentException("Type T is of an invalid size for the format of this texture.", "T");
//			if (startIndex < 0 || startIndex >= data.Length)
//				throw new ArgumentException("startIndex must be at least zero and smaller than data.Length.",
//				                            "startIndex");
//			if (data.Length < startIndex + elementCount)
//				throw new ArgumentException("The data array is too small.");

//			int dataByteSize;
//			if (Format.IsCompressedFormat())
//			{
//				int blockWidth, blockHeight;
//				Format.GetBlockSize(out blockWidth, out blockHeight);
//				int blockWidthMinusOne = blockWidth - 1;
//				int blockHeightMinusOne = blockHeight - 1;
//				// round x and y down to next multiple of block size; width and height up to next multiple of block size
//				var roundedWidth = (checkedRect.Width + blockWidthMinusOne) & ~blockWidthMinusOne;
//				var roundedHeight = (checkedRect.Height + blockHeightMinusOne) & ~blockHeightMinusOne;
//				checkedRect = new Rectangle(checkedRect.X & ~blockWidthMinusOne, checkedRect.Y & ~blockHeightMinusOne,
//#if OPENGL
//                    // OpenGL only: The last two mip levels require the width and height to be
//                    // passed as 2x2 and 1x1, but there needs to be enough data passed to occupy
//                    // a full block.
//                    checkedRect.Width < blockWidth && textureBounds.Width < blockWidth ? textureBounds.Width : roundedWidth,
//                    checkedRect.Height < blockHeight && textureBounds.Height < blockHeight ? textureBounds.Height : roundedHeight);
//#else
//				                            roundedWidth, roundedHeight);
//#endif
//				if (Format == SurfaceFormat.RgbPvrtc2Bpp || Format == SurfaceFormat.RgbaPvrtc2Bpp)
//				{
//					dataByteSize = (Math.Max(checkedRect.Width, 16) * Math.Max(checkedRect.Height, 8) * 2 + 7) / 8;
//				}
//				else if (Format == SurfaceFormat.RgbPvrtc4Bpp || Format == SurfaceFormat.RgbaPvrtc4Bpp)
//				{
//					dataByteSize = (Math.Max(checkedRect.Width, 8) * Math.Max(checkedRect.Height, 8) * 4 + 7) / 8;
//				}
//				else
//				{
//					dataByteSize = roundedWidth * roundedHeight * fSize / (blockWidth * blockHeight);
//				}
//			}
//			else
//			{
//				dataByteSize = checkedRect.Width * checkedRect.Height * fSize;
//			}

//			if (elementCount * tSize != dataByteSize)
//				throw new ArgumentException(
//					"elementCount is not the right size, " +
//					$"elementCount * sizeof(T) is {elementCount * tSize}, but data size is {dataByteSize}.", nameof(elementCount));
		}

		public static Texture2D Load(GraphicsDevice graphicsDevice, string path)
		{
			var bytes = File.ReadAllBytes(path);
			var result = ImageResult.FromMemory(bytes, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
			
			var texture = graphicsDevice.CreateTexture2D(result.Width, result.Height);
			texture.SetData(result.Data);
			return texture;
		}
	}
}