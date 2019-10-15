using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	public interface IOpenGLBackBuffer
	{
		int Width { get; }
		int Height { get; }
		DepthFormat DepthFormat { get; }
		int MultiSampleCount { get; }
		void Reset(PresentationParameters presentationParameters);
	}
}