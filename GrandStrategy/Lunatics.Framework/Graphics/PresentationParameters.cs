using System;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public class PresentationParameters
	{
		public const int DefaultBackBufferWidth = 800;
		public const int DefaultBackBufferHeight = 600;

		public SurfaceFormat BackBufferFormat { get; set; }
		public int BackBufferHeight { get; set; }

		public int BackBufferWidth { get; set; }

		public Rectangle Bounds => new Rectangle(0, 0, BackBufferWidth, BackBufferHeight);

		public IntPtr DeviceWindowHandle { get; set; }

		public DepthFormat DepthStencilFormat { get; set; }

		public bool IsFullScreen { get; set; }

		public PresentInterval PresentationInterval { get; set; }

		public PresentationParameters()
		{
			BackBufferFormat = SurfaceFormat.Color;
			BackBufferWidth = DefaultBackBufferWidth /*GraphicsDeviceManager.DefaultBackBufferWidth*/;
			BackBufferHeight = DefaultBackBufferHeight /*GraphicsDeviceManager.DefaultBackBufferHeight*/;
			DeviceWindowHandle = IntPtr.Zero;
			IsFullScreen = false; // FIXME: Is this the default?
			DepthStencilFormat = DepthFormat.None;
			//MultiSampleCount = 0;
			PresentationInterval = PresentInterval.Default;
			//DisplayOrientation = DisplayOrientation.Default;
			//RenderTargetUsage = RenderTargetUsage.DiscardContents;
		}

		public PresentationParameters Clone()
		{
			return new PresentationParameters
			       {
				       BackBufferFormat = BackBufferFormat,
				       BackBufferHeight = BackBufferHeight,
				       BackBufferWidth = BackBufferWidth,
				       DeviceWindowHandle = DeviceWindowHandle,
				       IsFullScreen = IsFullScreen,
				       DepthStencilFormat = DepthStencilFormat,
				       //clone.MultiSampleCount = MultiSampleCount;
				       PresentationInterval = PresentationInterval,
				       //clone.DisplayOrientation = DisplayOrientation;
				       //clone.RenderTargetUsage = RenderTargetUsage;
			       };
		}
	}
}