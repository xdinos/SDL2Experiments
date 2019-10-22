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
			_wantsFullscreen = false;
		}

		public override void BeginScreenDeviceChange(bool willBeFullScreen)
		{
			_wantsFullscreen = willBeFullScreen;
		}

		public override void EndScreenDeviceChange(string deviceName, int clientWidth, int clientHeight)
		{
			string prevName = deviceName;
			ApplyWindowChanges(_handle,
			                   clientWidth,
			                   clientHeight,
			                   _wantsFullscreen,
			                   deviceName,
			                   ref _deviceName);
			// TODO: ???
			//if (deviceName != prevName)
			//	OnScreenDeviceNameChanged();
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

		private static void ApplyWindowChanges(IntPtr window,
		                                       int clientWidth,
		                                       int clientHeight,
		                                       bool wantsFullscreen,
		                                       string screenDeviceName,
		                                       ref string resultDeviceName)
		{
			bool center = false;
			
			// TODO: ...
			//if (Environment.GetEnvironmentVariable("GRAPHICS_ENABLE_HIGHDPI") == "1" && OSVersion.Equals("Mac OS X"))
			//{
			//	/* For high-DPI windows, halve the size!
			//	 * The drawable size is now the primary width/height, so
			//	 * the window needs to accommodate the GL viewport.
			//	 * -flibit
			//	 */
			//	clientWidth /= 2;
			//	clientHeight /= 2;
			//}

			// When windowed, set the size before moving
			if (!wantsFullscreen)
			{
				bool resize = false;
				if ((Sdl.Window.GetWindowFlags(window) & (uint)Sdl.Window.Flags.Fullscreen) != 0)
				{
					Sdl.Window.SetFullscreen(window, 0);
					resize = true;
				}
				else
				{
					Sdl.Window.GetSize(window, out var w, out var h);
					resize = (clientWidth != w || clientHeight != h);
				}
				if (resize)
				{
					Sdl.Window.SetSize(window, clientWidth, clientHeight);
					center = true;
				}
			}

			// Get on the right display!
			var displayIndex = 0;
			for (var i = 0; i < SdlPlatform.Adapters.Count; i += 1)
			{
				if (screenDeviceName != SdlPlatform.Adapters[i].DeviceName) continue;
				displayIndex = i;
				break;
			}

			// Just to be sure, become a window first before changing displays
			if (resultDeviceName != screenDeviceName)
			{
				Sdl.Window.SetFullscreen(window, 0);
				resultDeviceName = screenDeviceName;
				center = true;
			}

			// Window always gets centered on changes, per XNA behavior
			if (center)
			{
				var pos = Sdl.Window.PosCentered | displayIndex;
				Sdl.Window.SetPosition(window, pos, pos);
			}

			// Set fullscreen after we've done all the ugly stuff.
			if (wantsFullscreen)
			{
				if ((Sdl.Window.GetWindowFlags(window) & (uint)Sdl.Window.Flags.Shown) == 0)
				{
					/* If we're still hidden, we can't actually go fullscreen yet.
					 * But, we can at least set the hidden window size to match
					 * what the window/drawable sizes will eventually be later.
					 * -flibit
					 */
					Sdl.Display.GetCurrentDisplayMode(displayIndex, out var mode);
					Sdl.Window.SetSize(window, mode.Width, mode.Height);
				}

				Sdl.Window.SetFullscreen(window,(int)Sdl.Window.Flags.FullscreenDesktop);
			}
		}

		private IntPtr _handle;
		private string _deviceName;
		private bool _wantsFullscreen;
	}
}