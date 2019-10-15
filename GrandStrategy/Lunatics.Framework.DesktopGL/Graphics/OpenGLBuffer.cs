using System;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	internal class OpenGLBuffer
	{
		public int Handle { get; }
		public IntPtr BufferSize { get; }

		public OpenGL.BufferUsageHint UsageHint { get; }

		internal OpenGLBuffer(int handle, IntPtr bufferSize, OpenGL.BufferUsageHint usageHint)
		{
			Handle = handle;
			BufferSize = bufferSize;
			UsageHint = usageHint;
		}
	}
}