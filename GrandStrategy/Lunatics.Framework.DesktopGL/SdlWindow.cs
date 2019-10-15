using System;

namespace Lunatics.Framework.DesktopGL
{
	public sealed class SdlWindow : GameWindow
	{
		public override IntPtr Handle => _handle;

		public SdlWindow(IntPtr handle, string deviceName)
		{
			_handle = handle;
			_deviceName = deviceName;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Handle != IntPtr.Zero)
				{
					Sdl.Window.Destroy(Handle);
					_handle = IntPtr.Zero;
				}
			}

			base.Dispose(disposing);
		}

		private IntPtr _handle;
		private string _deviceName;
	}
}