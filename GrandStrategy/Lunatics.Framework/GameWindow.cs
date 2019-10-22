﻿using System;

namespace Lunatics.Framework
{
	public abstract class GameWindow : IDisposable
	{
		public abstract IntPtr Handle { get; }
		public abstract void BeginScreenDeviceChange(bool willBeFullScreen);
		public abstract void EndScreenDeviceChange(string deviceName, int clientWidth, int clientHeight);

		#region IDisposable

		~GameWindow()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			
			_disposed = true;
		}

		private bool _disposed;

		#endregion
	}
}