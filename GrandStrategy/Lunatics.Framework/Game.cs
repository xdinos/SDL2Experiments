using System;
using System.Diagnostics;
using System.Linq;
using Lunatics.Framework.Graphics;
using Lunatics.Mathematics;

namespace Lunatics.Framework
{
	public abstract class Game : IDisposable
	{
		#region Events
		
		#endregion

		internal GamePlatform Platform { get; }

		public GameWindow Window => Platform.Window;

		public GraphicsDevice GraphicsDevice => _graphicsDevice;

		public bool IsFullScreen { get; set; }
		public int PreferredBackBufferHeight { get; set; }
		public int PreferredBackBufferWidth { get; set; }
		public SurfaceFormat PreferredBackBufferFormat {get; set;}
		public DepthFormat PreferredDepthStencilFormat { get; set; }
		public bool SynchronizeWithVerticalRetrace { get; set; }
		

		protected Game(Func<Game, GamePlatform> platformFactory,
		               Func<GraphicsAdapter, PresentationParameters, GraphicsDevice> graphicsDeviceFactory)
		{
			PreferredBackBufferWidth = PresentationParameters.DefaultBackBufferWidth;
			PreferredBackBufferHeight = PresentationParameters.DefaultBackBufferHeight;
			
			PreferredBackBufferFormat = SurfaceFormat.Color;
			PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			SynchronizeWithVerticalRetrace = true;
			// TODO: PreferMultiSampling = false;

			Platform = platformFactory(this);
			_graphicsDeviceFactory = graphicsDeviceFactory;

			// TODO:...
			// Window.ClientSizeChanged += INTERNAL_OnClientSizeChanged;
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
			if (_graphicsDevice == null)
				CreateGraphicsDevice();

			Platform.BeforeInitialize();
			Initialize();
		}

		private void CreateGraphicsDevice()
		{
			if (_graphicsDevice != null)
				return;

			var gdi = new GraphicsDeviceInfo
			          {
				          Adapter = Platform.GetGraphicsAdapters().First(),
				          PresentationParameters = new PresentationParameters
				                                   {
					                                   DeviceWindowHandle = Window.Handle,
					                                   DepthStencilFormat = PreferredDepthStencilFormat,
					                                   IsFullScreen = false
				                                   }
			          };

			// TODO: ??? OnPreparingDeviceSettings(this, new PreparingDeviceSettingsEventArgs(gdi));

			PreferredBackBufferFormat = gdi.PresentationParameters.BackBufferFormat;
			PreferredDepthStencilFormat = gdi.PresentationParameters.DepthStencilFormat;

			_graphicsDevice = _graphicsDeviceFactory(gdi.Adapter, gdi.PresentationParameters);
			
			_graphicsDevice.Disposing += OnDeviceDisposing;
			// TODO: ...
			//_graphicsDevice.DeviceResetting += OnDeviceResetting;
			//_graphicsDevice.DeviceReset += OnDeviceReset;

			ApplyChanges();

			// TODO: ??? OnDeviceCreated(this, EventArgs.Empty);
		}

		private void ApplyChanges()
		{
			if (_graphicsDevice == null) return;

			var gdi = new GraphicsDeviceInfo
			          {
				          Adapter = _graphicsDevice.Adapter,
				          PresentationParameters = _graphicsDevice.PresentationParameters.Clone()
			          };

			gdi.PresentationParameters.BackBufferFormat = PreferredBackBufferFormat;

			gdi.PresentationParameters.BackBufferWidth = PreferredBackBufferWidth;
			gdi.PresentationParameters.BackBufferHeight = PreferredBackBufferHeight;

			gdi.PresentationParameters.DepthStencilFormat = PreferredDepthStencilFormat;
			gdi.PresentationParameters.IsFullScreen = IsFullScreen;
			gdi.PresentationParameters.PresentationInterval = SynchronizeWithVerticalRetrace
				                                                  ? PresentInterval.One
				                                                  : PresentInterval.Immediate;

			// TODO: ??? OnPreparingDeviceSettings(this, new PreparingDeviceSettingsEventArgs(gdi));

			Window.BeginScreenDeviceChange(gdi.PresentationParameters.IsFullScreen);
			Window.EndScreenDeviceChange(gdi.Adapter.DeviceName,
			                             gdi.PresentationParameters.BackBufferWidth,
			                             gdi.PresentationParameters.BackBufferHeight);

			// FIXME: This should be before EndScreenDeviceChange! -flibit
			_graphicsDevice.Reset(gdi.PresentationParameters, gdi.Adapter);
		}

		private void OnDeviceDisposing(object sender, EventArgs args)
		{
			UnloadContent();
		}

		// TODO: ...
		//private void INTERNAL_OnClientSizeChanged(object sender, EventArgs e)
		//{
		//	Rectangle size = (sender as GameWindow).ClientBounds;
		//	resizedBackBufferWidth = size.Width;
		//	resizedBackBufferHeight = size.Height;
		//	if (Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1")
		//	{
		//		resizedBackBufferWidth *= 2;
		//		resizedBackBufferHeight *= 2;
		//	}
		//	useResizedBackBuffer = true;
		//	ApplyChanges();
		//}

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