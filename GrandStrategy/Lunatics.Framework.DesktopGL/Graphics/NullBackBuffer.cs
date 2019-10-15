using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	public class NullBackBuffer : IOpenGLBackBuffer
	{
		public int Width { get; private set; }

		public int Height { get; private set; }

		public DepthFormat DepthFormat { get; private set; }

		public int MultiSampleCount
		{
			get
			{
				// Constant, per SDL2_GameWindow
				return 0;
			}
		}

		public NullBackBuffer(int width, int height, DepthFormat depthFormat)
		{
			Width = width;
			Height = height;
			DepthFormat = depthFormat;
		}

		public void Reset(PresentationParameters presentationParameters)
		{
			Width = presentationParameters.BackBufferWidth;
			Height = presentationParameters.BackBufferHeight;
		}
	}
}