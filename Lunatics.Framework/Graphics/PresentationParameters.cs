using System;
using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public class PresentationParameters
	{
		public int BackBufferHeight { get; set; }

		public int BackBufferWidth { get; set; }

		public Rectangle Bounds => new Rectangle(0, 0, BackBufferWidth, BackBufferHeight);

		public bool IsFullScreen { get; set; } = false;

		public PresentInterval PresentationInterval { get; set; } = PresentInterval.Default;

		public IntPtr DeviceWindowHandle { get; set; } = IntPtr.Zero;
	}
}