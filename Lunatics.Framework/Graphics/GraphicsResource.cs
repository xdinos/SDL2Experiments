using System;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsResource : IDisposable
	{
		public GraphicsDevice GraphicsDevice { get; protected set; }

		protected virtual void GraphicsDeviceResetting()
		{
		}

		#region IDisposable

		public bool IsDisposed { get; private set; }

		public event EventHandler<EventArgs> Disposing;

		public void Dispose()
		{
			// Dispose of managed objects as well
			Dispose(true);
			// Since we have been manually disposed, do not call the finalizer on this object
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Do not trigger the event if called from the finalizer
				if (disposing)
				{
					Disposing?.Invoke(this, EventArgs.Empty);
				}

				//// Remove from the list of graphics resources
				//if (GraphicsDevice != null)
				//{
				//	GraphicsDevice.RemoveResourceReference(selfReference);
				//}

				//selfReference = null;
				GraphicsDevice = null;
				IsDisposed = true;
			}
		}

		#endregion
	}
}