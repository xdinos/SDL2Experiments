using System;
using System.Diagnostics;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsResource : IDisposable
	{
		public event EventHandler<EventArgs> Disposing;

		public GraphicsDevice GraphicsDevice
		{
			get => _graphicsDevice;
			internal set
			{
				Debug.Assert(value != null);

				if (_graphicsDevice == value)
					return;

				// VertexDeclaration objects can be bound to multiple GraphicsDevice objects
				// during their lifetime. But only one GraphicsDevice should retain ownership.
				if (_graphicsDevice != null)
				{
					_graphicsDevice.RemoveResourceReference(_selfReference);
					_selfReference = null;
				}

				_graphicsDevice = value;

				_selfReference = new WeakReference(this);
				_graphicsDevice.AddResourceReference(_selfReference);
			}
		}

		protected internal virtual void GraphicsDeviceResetting()
		{
		}

		#region IDisposable

		public bool IsDisposed { get; private set; }

		~GraphicsResource()
		{
			// Pass false so the managed objects are not released
			Dispose(false);
		}

		public void Dispose()
		{
			// Dispose of managed objects as well
			Dispose(true);
			// Since we have been manually disposed, do not call the finalizer on this object
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed) return;

			if (disposing)
			{
				// Release managed objects
				// ...
			}

			// Release native objects
			// ...

			// Do not trigger the event if called from the finalizer
			if (disposing)
				Disposing?.Invoke(this, EventArgs.Empty);

			// Remove from the global list of graphics resources
			_graphicsDevice?.RemoveResourceReference(_selfReference);

			_selfReference = null;
			_graphicsDevice = null;
			IsDisposed = true;
		}

		#endregion

		private WeakReference _selfReference;

		// The GraphicsDevice property should only be accessed in Dispose(bool) if the disposing
		// parameter is true. If disposing is false, the GraphicsDevice may or may not be
		// disposed yet.
		private GraphicsDevice _graphicsDevice;
		
	}
}