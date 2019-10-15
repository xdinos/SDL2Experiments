using System;
using System.Diagnostics;
using System.Linq;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework
{
	public abstract class Game : IDisposable
	{
		#region Events
		
		#endregion

		internal GamePlatform Platform { get; }

		public GameWindow Window => Platform.Window;

		public GraphicsDevice GraphicsDevice => GetOrCreateGraphicsDevice();

		protected Game(Func<Game, GamePlatform> platformFactory,
		               Func<GraphicsAdapter, PresentationParameters, GraphicsDevice> graphicsDeviceFactory)
		{
			Platform = platformFactory(this);
			_graphicsDeviceFactory = graphicsDeviceFactory;
		}

		public void Run()
		{
			if (!_initialized)
			{
				DoInitialize();
				_initialized = true;
			}

			BeginRun();
			_gameTimer = Stopwatch.StartNew();

			Platform.RunLoop();

			EndRun();
		}

		public void Tick()
		{
			var currentTicks = _gameTimer.Elapsed.Ticks;
			_accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
			_previousTicks = currentTicks;

			Update(_accumulatedElapsedTime);

			_accumulatedElapsedTime = TimeSpan.Zero;

			if (BeginDraw())
			{
				Draw(_accumulatedElapsedTime);
				EndDraw();
			}

			_accumulatedElapsedTime = TimeSpan.Zero;
		}

		protected virtual void Initialize()
		{
			LoadContent();
		}

		protected virtual void LoadContent() { }
		protected virtual void UnloadContent() { }

		protected virtual void BeginRun() { }
		protected virtual void EndRun() { }

		protected virtual void Update(TimeSpan elapsedGameTime) { }

		protected virtual bool BeginDraw() { return true; }
		protected virtual void Draw(TimeSpan elapsedGameTime) { }
		protected virtual void EndDraw()
		{
			Platform.Present();
		}

		private void DoInitialize()
		{
			Platform.BeforeInitialize();
			Initialize();
		}

		private GraphicsDevice GetOrCreateGraphicsDevice()
		{
			if (_graphicsDevice != null)
				return _graphicsDevice;

			var defaultAdapter = Platform.GetGraphicsAdapters().First();

			// TODO: ??? OnPreparingDeviceSettings(this, new PreparingDeviceSettingsEventArgs(gdi));

			_graphicsDevice = _graphicsDeviceFactory(defaultAdapter,
			                                         new PresentationParameters
			                                         {
														 DeviceWindowHandle = Window.Handle,
														 IsFullScreen = false
			                                         });
			
			_graphicsDevice.Disposing += OnDeviceDisposing;
			// TODO: ...
			//_graphicsDevice.DeviceResetting += OnDeviceResetting;
			//_graphicsDevice.DeviceReset += OnDeviceReset;

			ApplyChanges();

			// TODO: ??? OnDeviceCreated(this, EventArgs.Empty);

			return _graphicsDevice;
		}

		private void ApplyChanges()
		{
			if (_graphicsDevice==null) return;
		}

		private void OnDeviceDisposing(object sender, EventArgs args)
		{
			UnloadContent();
		}

		#region IDisposable

		public event EventHandler<EventArgs> Disposed;

		~Game()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				Platform?.Dispose();
				GraphicsDevice?.Dispose();
			}

			_disposed = true;
		}

		#endregion

		private bool _disposed;
		private bool _initialized;
		private Stopwatch _gameTimer;
		private long _previousTicks = 0;
		private TimeSpan _accumulatedElapsedTime;
		private GraphicsDevice _graphicsDevice;
		private readonly Func<GraphicsAdapter, PresentationParameters, GraphicsDevice> _graphicsDeviceFactory;
	}
}