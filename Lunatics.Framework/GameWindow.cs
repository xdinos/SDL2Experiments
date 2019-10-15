using System;
using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework
{
	public abstract class GameWindow
	{
		#region Events

		public event EventHandler<EventArgs> ClientSizeChanged;
		public event EventHandler<EventArgs> OrientationChanged;
		public event EventHandler<EventArgs> ScreenDeviceNameChanged;

		#endregion

		public abstract IntPtr Handle { get; }
		public abstract string DeviceName { get; }

		public abstract Rectangle ClientBounds { get; }

		protected void OnClientSizeChanged()
		{
			ClientSizeChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}