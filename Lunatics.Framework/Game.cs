using System;
using System.Diagnostics;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework
{
	public abstract class Game : IDisposable
	{
		#region Events
		
		public event EventHandler<EventArgs> Activated;
		public event EventHandler<EventArgs> Deactivated;
		public event EventHandler<EventArgs> Disposed;
		public event EventHandler<EventArgs> Exiting;

		#endregion

		public GameWindow Window { get; }

		public GraphicsDevice GraphicsDevice { get; }
		
		public bool IsActive
		{
			get => _isActive;
			/*internal*/ set
			{
				if (_isActive == value) 
					return;

				_isActive = value;
				if (_isActive)
				{
					OnActivated(this, EventArgs.Empty);
				}
				else
				{
					OnDeactivated(this, EventArgs.Empty);
				}
			}
		}

		public bool RunApplication { get; /*private*/ set; }

		protected Game(string title, Platform platform)
		{
			//AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			_platform = platform;
			
			Window = _platform.CreateWindow(title);

			GraphicsDevice = _platform.CreateGraphicsDevice(
				new PresentationParameters
				{
					DeviceWindowHandle = Window.Handle
				},
				null);

			RunApplication = true;
		}

		~Game()
		{
			Dispose(false);
		}

		#region IDisposable Implementation

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					//// Dispose loaded game components.
					//for (int i = 0; i < Components.Count; i += 1)
					//{
					//	IDisposable disposable = Components[i] as IDisposable;
					//	if (disposable != null)
					//	{
					//		disposable.Dispose();
					//	}
					//}

					//if (Content != null)
					//{
					//	Content.Dispose();
					//}

					//if (graphicsDeviceService != null)
					//{
					//	// FIXME: Does XNA4 require the GDM to be disposable? -flibit
					//	(graphicsDeviceService as IDisposable).Dispose();
					//}

					if (Window != null)
						_platform.DisposeWindow(Window);

					//ContentTypeReaderManager.ClearTypeCreators();
				}

				//AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;

				_isDisposed = true;
			}
		}

		#endregion

		public void Run()
		{
			if (!_isInitialized)
			{
				DoInitialize();
				_isInitialized = true;
			}

			BeginRun();
			_gameTimer = Stopwatch.StartNew();
			_platform.RunLoop(this);
			EndRun();

			OnExiting(this, EventArgs.Empty);
		}

		public void Tick()
		{
			var currentTicks = _gameTimer.Elapsed.Ticks;
			_accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
			_previousTicks = currentTicks;

			Update(_accumulatedElapsedTime);

			_accumulatedElapsedTime = TimeSpan.Zero;

			// Draw unless the update suppressed it.
			//if (suppressDraw)
			//{
			//	suppressDraw = false;
			//}
			//else
			{
				/* Draw/EndDraw should not be called if BeginDraw returns false.
				 * http://stackoverflow.com/questions/4054936/manual-control-over-when-to-redraw-the-screen/4057180#4057180
				 * http://stackoverflow.com/questions/4235439/xna-3-1-to-4-0-requires-constant-redraw-or-will-display-a-purple-screen
				 */
				if (BeginDraw())
				{
					Draw(_accumulatedElapsedTime);
					EndDraw();
				}
			}

			_accumulatedElapsedTime = TimeSpan.Zero;
		}

		public void Exit()
		{
			RunApplication = false;
			// TODO: suppressDraw = true;
		}
		
		private void DoInitialize()
		{
			// InitializeGraphicsService

			Initialize();
		}

		protected virtual void Initialize()
		{
			///* FIXME: If this test fails, is LoadContent ever called?
			// * This seems like a condition that warrants an exception more
			// * than a silent failure.
			// */
			//if (graphicsDeviceService != null &&
			//    graphicsDeviceService.GraphicsDevice != null)
			//{
			//	graphicsDeviceService.DeviceDisposing += (o, e) => UnloadContent();
			//	LoadContent();
			//}

			LoadContent();
		}

		protected virtual void LoadContent()
		{
		}

		protected virtual void UnloadContent()
		{
		}

		protected virtual void BeginRun()
		{
		}

		protected virtual void EndRun()
		{
		}

		protected virtual void Update(TimeSpan elapsedGameTime)
		{
		}

		protected virtual bool BeginDraw()
		{
			return true;
		}

		protected virtual void Draw(TimeSpan elapsedGameTime)
		{

		}

		protected virtual void EndDraw()
		{
			GraphicsDevice?.Present();
		}

		protected virtual void OnActivated(object sender, EventArgs args)
		{
			ThrowIfDisposed();
			Activated?.Invoke(this, args);
		}

		protected virtual void OnDeactivated(object sender, EventArgs args)
		{
			ThrowIfDisposed();
			Deactivated?.Invoke(this, args);
		}

		protected virtual void OnExiting(object sender, EventArgs args)
		{
			ThrowIfDisposed();
			Exiting?.Invoke(this, args);
		}

		[DebuggerNonUserCode]
		private void ThrowIfDisposed()
		{
			if (!_isDisposed) return;

			var name = GetType().Name;
			throw new ObjectDisposedException(name, $"The {name} object was used after being Disposed.");
		}

		private bool _isDisposed;
		private bool _isInitialized;
		private bool _isActive;
		private long _previousTicks = 0;
		private TimeSpan _accumulatedElapsedTime;
		private Stopwatch _gameTimer;
		private readonly Platform _platform;
	}
}