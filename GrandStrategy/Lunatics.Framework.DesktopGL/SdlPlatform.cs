using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Lunatics.Framework.DesktopGL.Input;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Input;

namespace Lunatics.Framework.DesktopGL
{
	public sealed class SdlPlatform : GamePlatform
	{
		public SdlPlatform(Game game)
			: base(game)
		{
			var version = Sdl.GetVersion();
			var platform = Sdl.GetPlatform();

			var versionNumber = 1000 * version.Major + 100 * version.Minor + version.Patch;
			if (versionNumber >= 2005 &&
			    (string.CompareOrdinal(platform, "Windows") == 0 || string.CompareOrdinal(platform, "WinRT") == 0) &&
			    Debugger.IsAttached)
			{
				Sdl.SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");
			}

			Sdl.Init(Sdl.InitFlags.Video);
		}

		public override IReadOnlyCollection<GraphicsAdapter> GetGraphicsAdapters() => Adapters;

		protected override List<Keys> GetKeys()
		{
			return _keys;
		}

		protected override GameWindow CreateWindow()
		{
			var initFlags = Sdl.Window.Flags.Hidden |
							Sdl.Window.Flags.InputFocus |
							Sdl.Window.Flags.MouseFocus;

			Sdl.GL.SetAttribute(Sdl.GL.Attribute.RedSize, 8);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.GreenSize, 8);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.BlueSize, 8);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.AlphaSize, 8);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.DepthSize, 24/*depthSize*/);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.StencilSize, 8/*stencilSize*/);
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.DoubleBuffer, 1);

			if (Environment.GetEnvironmentVariable("OPENGL_FORCE_CORE_PROFILE") == "1")
			{
				Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextProfileMask, (int)Sdl.GL.ContextProfile.Core);
				Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextMajorVersion, 4);
				Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextMinorVersion, 1);
			}

#if DEBUG
			Sdl.GL.SetAttribute(Sdl.GL.Attribute.ContextFlags, (int)Sdl.GL.Context.Debug);
#endif
			initFlags |= Sdl.Window.Flags.OpenGL;

			if (Environment.GetEnvironmentVariable("GRAPHICS_ENABLE_HIGHDPI") == "1")
				initFlags |= Sdl.Window.Flags.AllowHighDPI;

			var handle = Sdl.Window.Create(GetDefaultWindowTitle(),
										   Sdl.Window.PosCentered,
										   Sdl.Window.PosCentered,
										   PresentationParameters.DefaultBackBufferWidth/*GraphicsDeviceManager.DefaultBackBufferWidth*/,
										   PresentationParameters.DefaultBackBufferHeight/*GraphicsDeviceManager.DefaultBackBufferHeight*/,
										   initFlags);
			if (handle == IntPtr.Zero)
				throw new Exception/*NoSuitableGraphicsDeviceException*/(Sdl.GetError());


			Sdl.DisableScreenSaver();

			Sdl.GL.GetDrawableSize(handle, out var drawX, out var drawY);

			if (drawX == PresentationParameters.DefaultBackBufferWidth/*GraphicsDeviceManager.DefaultBackBufferWidth*/ &&
				drawY == PresentationParameters.DefaultBackBufferHeight/*GraphicsDeviceManager.DefaultBackBufferHeight*/)
			{
				Environment.SetEnvironmentVariable("GRAPHICS_ENABLE_HIGHDPI", "0");
			}
			else
			{

				// Store the full retina resolution of the display
				// TODO ...
				//RetinaWidth = drawX;
				//RetinaHeight = drawY;
			}

			return new SdlWindow(handle, $@"\\.\DISPLAY{Sdl.Window.GetDisplayIndex(handle) + 1}");
		}

		protected override void RunLoop()
		{
			Sdl.Window.Show(Game.Window.Handle);

			_displayIndex = Sdl.Window.GetDisplayIndex(Game.Window.Handle);
			
			while (!_isExiting)
			{
				HandleEvents();
				
				Game.Tick();
			}
			
			// TODO: ???
			// Game.Exit();
		}

		protected override void Present()
		{
			Game.GraphicsDevice.Present();
		}

		protected override MouseState GetMouseState(IntPtr windowHandle)
		{
			var winFlags = Sdl.Window.GetWindowFlags(windowHandle);
			var hasFocus = (winFlags & Sdl.Window.State.MouseFocus) != 0;
			int x, y;
			Sdl.Mouse.Button flags;

			if (Sdl.Mouse.GetRelativeMode() == 1)
			{
				flags = Sdl.Mouse.GetRelativeState(out x, out y);
			}
			else if ( /*SupportsGlobalMouse*/true)
			{
				flags = Sdl.Mouse.GetGlobalState(out x, out y);
				Sdl.Window.GetPosition(windowHandle, out var wx, out var wy);
				x -= wx;
				y -= wy;
			}
			else
			{
				/* This is inaccurate, but what can you do... */
				flags = Sdl.Mouse.GetState(out x, out y);
			}

			return new MouseState(x, y,
			                      _scrollX, _scrollY,
			                      (flags & Sdl.Mouse.Button.Left) != 0 ? ButtonState.Pressed : ButtonState.Released,
			                      (flags & Sdl.Mouse.Button.Middle) != 0 ? ButtonState.Pressed : ButtonState.Released,
			                      (flags & Sdl.Mouse.Button.Right) != 0 ? ButtonState.Pressed : ButtonState.Released);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{

			}

			base.Dispose(disposing);
		}


		private void HandleEvents()
		{
			while (Sdl.PollEvent(out var @event) == 1)
			{
				if (@event.Type == Sdl.EventType.Quit)
				{
					_isExiting = true;
				}
				else if (@event.Type == Sdl.EventType.KeyDown)
				{
					var key = ToKey(ref @event.Key.Keysym);
					if (!_keys.Contains(key))
					{
						_keys.Add(key);
						//int textIndex;
						//if (textInputBindings.TryGetValue(key, out textIndex))
						//{
						//	textInputControlDown[textIndex] = true;
						//	textInputControlRepeat[textIndex] = Environment.TickCount + 400;
						//	TextInputEXT.OnTextInput(textInputCharacters[textIndex]);
						//}
						//else if (keys.Contains(Keys.LeftControl) && key == Keys.V)
						//{
						//	textInputControlDown[6] = true;
						//	textInputControlRepeat[6] = Environment.TickCount + 400;
						//	TextInputEXT.OnTextInput(textInputCharacters[6]);
						//	textInputSuppress = true;
						//}
					}
				}
				else if (@event.Type == Sdl.EventType.KeyUp)
				{
					var key = ToKey(ref @event.Key.Keysym);
					if (_keys.Remove(key))
					{
						//int value;
						//if (textInputBindings.TryGetValue(key, out value))
						//{
						//	textInputControlDown[value] = false;
						//}
						//else if ((!keys.Contains(Keys.LeftControl) && textInputControlDown[3]) || key == Keys.V)
						//{
						//	textInputControlDown[6] = false;
						//	textInputSuppress = false;
						//}
					}
				}
				else if (@event.Type == Sdl.EventType.MouseButtonDown)
				{
					// TODO: Mouse.INTERNAL_onClicked(@event.Button.Button - 1);
				}
				else if (@event.Type == Sdl.EventType.MouseWheel)
				{
					const int wheelDelta = 120;

					_scrollX += @event.Wheel.X * wheelDelta;
					_scrollY += @event.Wheel.Y * 120;
				}
				else if (@event.Type == Sdl.EventType.WindowEvent /*&& ev.Window.WindowID == ((Window)Window).Id*/)
				{
					if (@event.Window.EventID == Sdl.Window.EventId.FocusGained)
					{
						IsActive = true;
						Sdl.DisableScreenSaver();
					}
					else if (@event.Window.EventID == Sdl.Window.EventId.FocusLost)
					{
						IsActive = false;
						Sdl.EnableScreenSaver();
					}
					else if (@event.Window.EventID == Sdl.Window.EventId.Enter)
					{
						Sdl.DisableScreenSaver();
					}
					else if (@event.Window.EventID == Sdl.Window.EventId.Leave)
					{
						Sdl.EnableScreenSaver();
					}
					else if (@event.Window.EventID == Sdl.Window.EventId.Moved)
					{
						var newDisplayIndex = Sdl.Window.GetDisplayIndex(Game.Window.Handle);
						if (newDisplayIndex != _displayIndex)
						{
							_displayIndex = newDisplayIndex;
							// TODO:...
							//Game.GraphicsDevice.Reset(Game.GraphicsDevice.PresentationParameters,
							//						  GraphicsAdapter.Adapters[newDisplayIndex]);
						}
					}
					else if (@event.Window.EventID == Sdl.Window.EventId.Close)
					{
						_isExiting = true;
					}
				}
			}
		}

		private int _scrollX;
		private int _scrollY;
		private int _displayIndex;
		private bool _isExiting = false;
		private readonly List<Keys> _keys = new List<Keys>();

		internal static ReadOnlyCollection<GraphicsAdapter> Adapters => _adapters ?? (_adapters = new ReadOnlyCollection<GraphicsAdapter>(GetGraphicsAdaptersImpl()));

		private static Keys ToKey(ref Sdl.Keyboard.Keysym key)
		{
			Keys retVal;
			//if (UseScancodes)
			//{
			//	if (INTERNAL_scanMap.TryGetValue((int)key.scancode, out retVal))
			//	{
			//		return retVal;
			//	}
			//}
			//else
			{
				retVal = KeyboardUtil.ToKey((int) key.Sym);
				if (retVal != Keys.None)
				{
					return retVal;
				}
			}
			Debug.WriteLine("KEY/SCANCODE MISSING FROM SDL2->XNA DICTIONARY: " +
			                key.Sym.ToString() + " " +
			                key.Scancode.ToString());
			return Keys.None;
		}

		private static GraphicsAdapter[] GetGraphicsAdaptersImpl()
		{
			var adapters = new GraphicsAdapter[Sdl.Display.GetNumVideoDisplays()];
			for (var i = 0; i < adapters.Length; i += 1)
			{
				adapters[i] = new Graphics.GraphicsAdapter(i,
				                                           $@"\\.\DISPLAY{i + 1}",
				                                           Sdl.Display.GetDisplayName(i));
			}

			return adapters;
		}

		private static ReadOnlyCollection<GraphicsAdapter> _adapters;
	}
}