using System;

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
}