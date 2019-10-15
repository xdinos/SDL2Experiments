using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Lunatics.Input;

namespace Lunatics
{
	public abstract class GameOld : IDisposable
	{
		public Graphics.Renderer Renderer { get; private set; }
		public IntPtr WindowHandle { get; private set; }
		public bool IsRunning { get; private set; }

		protected GameOld()
		{
			IsRunning = false;

			

			Keyboard.SetKeys(_keys);
		}
		
		public void Initialize(string title, int width, int height, bool fullscreen)
		{
			var OSVersion = SDL.SDL_GetPlatform();

			SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

			var drivers = SDL.SDL_GetNumVideoDrivers();
			for (var i = 0; i < drivers; i++)
				Console.WriteLine($"Driver ({i}): {SDL.SDL_GetVideoDriver(i)}");

			SDL.SDL_GetVersion(out var version);
			Console.WriteLine($"{SDL.SDL_GetPlatform()} {version.major}.{version.minor}.{version.patch}");

			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) == 0)
			{
				var flags = fullscreen ? SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0;

				WindowHandle = IntPtr.Zero;
				WindowHandle = SDL.SDL_CreateWindow(title,
				                                  SDL.SDL_WINDOWPOS_CENTERED,
				                                  SDL.SDL_WINDOWPOS_CENTERED,
				                                  width,
				                                  height,
				                                  flags /*| SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL*/);
				if (WindowHandle != IntPtr.Zero)
				{
					//var glCtx = SDL.SDL_GL_CreateContext(_windowPtr);
					SDL.SDL_SysWMinfo sysWMinfo = new SDL.SDL_SysWMinfo();
					SDL.SDL_GetWindowWMInfo(WindowHandle, ref sysWMinfo);


					Renderer = Graphics.Renderer.CreateRenderer(WindowHandle);
					if (Renderer != null)
					{
						IsRunning = true;
					}
				}

				Framework.Sdl.Graphics.GraphicsAdapter.GetGraphicsAdapters();
			}
		}

		public void Run()
		{
			Initialize("SDL2 Game Test", 1280, 800, false);
			LoadResources();

			var _accumulatedElapsedTime = TimeSpan.Zero;
			long _previousTicks = 0;
			var _gameTimer = Stopwatch.StartNew();

			while (IsRunning)
			{
				var currentTicks = _gameTimer.Elapsed.Ticks;
				_accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
				_previousTicks = currentTicks;

				HandleEvents();
				Update(_accumulatedElapsedTime);
				Render();

				_accumulatedElapsedTime = TimeSpan.Zero;
			}

			Shutdown();
		}
		
		protected virtual void LoadResources() { }

		protected virtual void Update(TimeSpan elapsedGameTime)
		{

		}

		protected virtual void Draw()
		{

		}

		protected virtual void Shutdown()
		{
			//SDL.SDL_DestroyTexture(_texturePtr);

			if (Renderer != null)
			{
				Renderer.Dispose();
				Renderer = null;
			}
			
			SDL.SDL_DestroyWindow(WindowHandle);
			SDL.SDL_Quit();
		}

		private void HandleEvents()
		{
			Mouse.ScrollX = 0;
			Mouse.ScrollY = 0;
			while (SDL.SDL_PollEvent(out var e) != 0)
			{
				switch (e.type)
				{
					case SDL.SDL_EventType.SDL_QUIT:
						IsRunning = false;
						break;
						
					case SDL.SDL_EventType.SDL_KEYDOWN:
					{
						var key = e.key.keysym.sym;
						if (!_keys.Contains(key))
							_keys.Add(key);
						//var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
						//if (!_keys.Contains(key))
						//	_keys.Add(key);
						//char character = (char) ev.Key.Keysym.Sym;
						//_view.OnKeyDown(new InputKeyEventArgs(key));
						//if (char.IsControl(character))
						//	_view.OnTextInput(new TextInputEventArgs(character, key));
					}

						//switch (e.key.keysym.sym)
						//{
						//	case SDL.SDL_Keycode.SDLK_q:
						//		IsRunning = false;
						//		break;
						//	case SDL.SDL_Keycode.SDLK_a:
						//	case SDL.SDL_Keycode.SDLK_LEFT:
						//		moveLeft = true;
						//		break;
						//	case SDL.SDL_Keycode.SDLK_d:
						//	case SDL.SDL_Keycode.SDLK_RIGHT:
						//		moveRight = true;
						//		break;
						//	case SDL.SDL_Keycode.SDLK_SPACE:
						//	case SDL.SDL_Keycode.SDLK_RETURN:
						//		fire = true;
						//		break;
						//}
						break;

					case SDL.SDL_EventType.SDL_KEYUP:
					{
						_keys.Remove(e.key.keysym.sym);
						//var key = KeyboardUtil.ToXna(ev.Key.Keysym.Sym);
						//_keys.Remove(key);
						//_view.OnKeyUp(new InputKeyEventArgs(key));
					}
						//switch (e.key.keysym.sym)
						//{
						//	case SDL.SDL_Keycode.SDLK_a:
						//	case SDL.SDL_Keycode.SDLK_LEFT:
						//		moveLeft = false;
						//		break;
						//	case SDL.SDL_Keycode.SDLK_d:
						//	case SDL.SDL_Keycode.SDLK_RIGHT:
						//		moveRight = false;
						//		break;
						//	case SDL.SDL_Keycode.SDLK_SPACE:
						//	case SDL.SDL_Keycode.SDLK_RETURN:
						//		fire = false;
						//		break;
						//}
						break;

					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
						Mouse.LeftButton = e.button.button == SDL.SDL_BUTTON_LEFT
							                   ? Mouse.ButtonState.Pressed
							                   : Mouse.LeftButton;
						Mouse.RightButton = e.button.button == SDL.SDL_BUTTON_RIGHT
							                   ? Mouse.ButtonState.Pressed
							                   : Mouse.RightButton;
						break;

					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
						Mouse.LeftButton = e.button.button == SDL.SDL_BUTTON_LEFT
							                   ? Mouse.ButtonState.Released
							                   : Mouse.LeftButton;
						Mouse.RightButton = e.button.button == SDL.SDL_BUTTON_RIGHT
							                    ? Mouse.ButtonState.Released
												: Mouse.RightButton;
						break;

					case SDL.SDL_EventType.SDL_MOUSEWHEEL:
						const int wheelDelta = 120;
						Mouse.ScrollX += e.wheel.x * wheelDelta;
						Mouse.ScrollY += e.wheel.y * wheelDelta;
						break;
					case SDL.SDL_EventType.SDL_MOUSEMOTION:
						Mouse.X = e.motion.x;
						Mouse.Y = e.motion.y;
						break;
				}
			}

			var currentKeyStatesPtr = SDL.SDL_GetKeyboardState(out var size);
			var currentKeyStates = new byte[size];
			Marshal.Copy(currentKeyStatesPtr, currentKeyStates, 0, currentKeyStates.Length);

		}

		private void Render()
		{
			Renderer.Clear(0x00, 0xff, 0xff, 0xff);
			

			Draw();

			Renderer.Present();
			
		}

		private readonly List<SDL.SDL_Keycode> _keys = new List<SDL.SDL_Keycode>();

		#region Implementation of IDisposable

		~GameOld()
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
			if (!_disposed)
			{
				if (disposing)
				{
					Shutdown();
				}

				_disposed = true;
			}
		}

		[System.Diagnostics.DebuggerNonUserCode]
		private void ThrowIfDisposed()
		{
			if (!_disposed)
				return;

			var name = GetType().Name;
			throw new ObjectDisposedException(name, $"The {name} object was used after being Disposed.");
		}

		private bool _disposed;

		#endregion
	}
}