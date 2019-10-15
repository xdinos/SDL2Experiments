using System;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework
{
	public abstract class Platform : IDisposable
	{
		~Platform()
		{
			Dispose(false);
		}

		#region IDisposable Implementation

		protected virtual void Dispose(bool disposing)
		{
			// TODO: release unmanaged resources here 

			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		public abstract GameWindow CreateWindow(string title);
		public abstract void DisposeWindow(GameWindow gameWindow);

		public abstract GraphicsDevice CreateGraphicsDevice(PresentationParameters presentationParameters, GraphicsAdapter adapter);

		public abstract void RunLoop(Game game);

		#region Fields

		private bool _disposed;

		#endregion

		
	}
}