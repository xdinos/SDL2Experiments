using System;
using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework.Sdl
{
	public sealed class SDLGameWindow : GameWindow
	{
		public override IntPtr Handle { get; }
		public override string DeviceName { get; }

		public override Rectangle ClientBounds => _platform.GetWindowBounds(Handle);

		internal SDLGameWindow(SDLPlatform platform, IntPtr handle, string deviceName)
		{
			_platform = platform;
			Handle = handle;
			DeviceName = deviceName;
		}

		internal void ClientSizeChangedInternal()
		{
			OnClientSizeChanged();
		}

		private string _displayName;
		private SDLPlatform _platform;
	}
}