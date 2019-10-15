using System;

namespace Lunatics.Framework.Graphics
{
	public class PresentationParameters
	{
		public int BackBufferHeight
		{
			get;
			set;
		}

		public int BackBufferWidth
		{
			get;
			set;
		}

		public IntPtr DeviceWindowHandle
		{
			get;
			set;
		}

		public bool IsFullScreen
		{
			get;
			set;
		}

		public PresentationParameters()
		{
			//BackBufferFormat = SurfaceFormat.Color;
			BackBufferWidth = 800/*GraphicsDeviceManager.DefaultBackBufferWidth*/;
			BackBufferHeight = 600/*GraphicsDeviceManager.DefaultBackBufferHeight*/;
			DeviceWindowHandle = IntPtr.Zero;
			IsFullScreen = false; // FIXME: Is this the default?
			//DepthStencilFormat = DepthFormat.None;
			//MultiSampleCount = 0;
			//PresentationInterval = PresentInterval.Default;
			//DisplayOrientation = DisplayOrientation.Default;
			//RenderTargetUsage = RenderTargetUsage.DiscardContents;
		}
	}
}