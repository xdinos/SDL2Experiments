using System;
using System.Text;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Mathematics;
using Lunatics.Input;
using SDL2;

namespace Lunatics.Framework.Sdl
{
	public sealed class SDLPlatform : Platform
	{
		public string OSVersion { get; }

		public SDLPlatform()
		{
			OSVersion = SDL.SDL_GetPlatform();

			if (string.CompareOrdinal(OSVersion, "Windows") == 0)
			{
				if (System.Diagnostics.Debugger.IsAttached)
				{
					SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
				}

				// TODO: SDL.SDL_SetEventFilter(win32OnPaint, IntPtr.Zero);
			}

			// TODO: storage ...

			SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
		}

		public override GameWindow CreateWindow(string title)
		{
			SDL.SDL_WindowFlags initFlags = SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN |
			                                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
			                                SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;
			                                
			bool useOpenGL = false;
			if (useOpenGL = PrepareGLAttributes())
				initFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;

			var handle = SDL.SDL_CreateWindow(title,
			                                  SDL.SDL_WINDOWPOS_CENTERED,
			                                  SDL.SDL_WINDOWPOS_CENTERED,
			                                  800/*GraphicsDeviceManager.DefaultBackBufferWidth*/,
			                                  600/*GraphicsDeviceManager.DefaultBackBufferHeight*/,
			                                  initFlags);

			if (handle == IntPtr.Zero)
			{
				// TODO: NoSuitableGraphicsDeviceException
				throw new Exception($"NoSuitableGraphicsDeviceException => {SDL.SDL_GetError()}");
			}

			return new SDLGameWindow(this, handle, $@"\\.\DISPLAY{SDL.SDL_GetWindowDisplayIndex(handle) + 1}");
		}

		public override GraphicsDevice CreateGraphicsDevice(PresentationParameters presentationParameters,
		                                                    GraphicsAdapter adapter)
		{
			return new Graphics.GraphicsDevice(presentationParameters, adapter);
		}

		public override void DisposeWindow(GameWindow gameWindow)
		{
			/* Some window managers might try to minimize the window as we're
			 * destroying it. This looks pretty stupid and could cause problems,
			 * so set this hint right before we destroy everything.
			 * -flibit
			 */
			SDL.SDL_SetHintWithPriority(SDL.SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, 
			                            "0",
			                            SDL.SDL_HintPriority.SDL_HINT_OVERRIDE);

			//if (Mouse.WindowHandle == window.Handle)
			//{
			//	Mouse.WindowHandle = IntPtr.Zero;
			//}

			//if (TouchPanel.WindowHandle == window.Handle)
			//{
			//	TouchPanel.WindowHandle = IntPtr.Zero;
			//}

			SDL.SDL_DestroyWindow(gameWindow.Handle);
		}
		public override void RunLoop(Game game)
		{
			SDL.SDL_ShowWindow(game.Window.Handle);

			game.IsActive = true;

			Rectangle windowBounds = game.Window.ClientBounds;
			// TODO: Mouse.INTERNAL_WindowWidth = windowBounds.Width;
			// TODO: Mouse.INTERNAL_WindowHeight = windowBounds.Height;

			// Which display did we end up on?
			var displayIndex = SDL.SDL_GetWindowDisplayIndex(game.Window.Handle);

			// OSX has some fancy fullscreen features, let's use them!
			bool osxUseSpaces;
			if (OSVersion.Equals("Mac OS X"))
			{
				string hint = SDL.SDL_GetHint(SDL.SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES);
				osxUseSpaces = (String.IsNullOrEmpty(hint) || hint.Equals("1"));
			}
			else
			{
				osxUseSpaces = false;
			}

			while (game.RunApplication)
			{
				HandleEvents(game, displayIndex, osxUseSpaces);
				game.Tick();
			}

			game.Exit();
		}

		public Rectangle GetWindowBounds(IntPtr window)
		{
			if ((SDL.SDL_GetWindowFlags(window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0)
			{
				/* FIXME: SDL2 bug!
				 * SDL's a little weird about SDL_GetWindowSize.
				 * If you call it early enough (for example,
				 * Game.Initialize()), it reports outdated ints.
				 * So you know what, let's just use this.
				 * -flibit
				 */
				SDL.SDL_GetCurrentDisplayMode(SDL.SDL_GetWindowDisplayIndex(window), out var mode);
				return new Rectangle(0, 0, mode.w, mode.h);
			}
			else
			{
				
				SDL.SDL_GetWindowPosition(window, out var x, out var y);
				SDL.SDL_GetWindowSize(window, out var w, out var h);
				return new Rectangle(x, y, w, h);
			}
		}

		public static bool GetWindowResizable(IntPtr window)
		{
			return ((SDL.SDL_GetWindowFlags(window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0);
		}

		public void Shutdown()
		{
			SDL.SDL_Quit();
		}

		private void HandleEvents(Game game, int displayIndex, bool osxUseSpaces)
		{
			while (SDL.SDL_PollEvent(out var evt) == 1)
			{
				// Keyboard
				if (evt.type == SDL.SDL_EventType.SDL_KEYDOWN)
				{
					//Keys key = ToXNAKey(ref evt.key.keysym);
					//if (!keys.Contains(key))
					//{
					//	keys.Add(key);
					//	int textIndex;
					//	if (textInputBindings.TryGetValue(key, out textIndex))
					//	{
					//		textInputControlDown[textIndex] = true;
					//		textInputControlRepeat[textIndex] = Environment.TickCount + 400;
					//		TextInputEXT.OnTextInput(textInputCharacters[textIndex]);
					//	}
					//	else if (keys.Contains(Keys.LeftControl) && key == Keys.V)
					//	{
					//		textInputControlDown[6] = true;
					//		textInputControlRepeat[6] = Environment.TickCount + 400;
					//		TextInputEXT.OnTextInput(textInputCharacters[6]);
					//		textInputSuppress = true;
					//	}
					//}
				}
				else if (evt.type == SDL.SDL_EventType.SDL_KEYUP)
				{
					//Keys key = ToXNAKey(ref evt.key.keysym);
					//if (keys.Remove(key))
					//{
					//	int value;
					//	if (textInputBindings.TryGetValue(key, out value))
					//	{
					//		textInputControlDown[value] = false;
					//	}
					//	else if ((!keys.Contains(Keys.LeftControl) && textInputControlDown[3]) || key == Keys.V)
					//	{
					//		textInputControlDown[6] = false;
					//		textInputSuppress = false;
					//	}
					//}
				}

				// Mouse Input
				else if (evt.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
				{
					//Mouse.INTERNAL_onClicked(evt.button.button - 1);
				}
				else if (evt.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
				{
					// 120 units per notch. Because reasons.
					//Mouse.INTERNAL_MouseWheel += evt.wheel.y * 120;
				}

				//// Touch Input
				//else if (evt.type == SDL.SDL_EventType.SDL_FINGERDOWN)
				//{
				//	// Windows only notices a touch screen once it's touched
				//	TouchPanel.TouchDeviceExists = true;

				//	TouchPanel.INTERNAL_onTouchEvent(
				//		(int)evt.tfinger.fingerId,
				//		TouchLocationState.Pressed,
				//		evt.tfinger.x,
				//		evt.tfinger.y,
				//		0,
				//		0
				//	);
				//}
				//else if (evt.type == SDL.SDL_EventType.SDL_FINGERMOTION)
				//{
				//	TouchPanel.INTERNAL_onTouchEvent(
				//		(int)evt.tfinger.fingerId,
				//		TouchLocationState.Moved,
				//		evt.tfinger.x,
				//		evt.tfinger.y,
				//		evt.tfinger.dx,
				//		evt.tfinger.dy
				//	);
				//}
				//else if (evt.type == SDL.SDL_EventType.SDL_FINGERUP)
				//{
				//	TouchPanel.INTERNAL_onTouchEvent(
				//		(int)evt.tfinger.fingerId,
				//		TouchLocationState.Released,
				//		evt.tfinger.x,
				//		evt.tfinger.y,
				//		0,
				//		0
				//	);
				//}

				// Various Window Events...
				else if (evt.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
				{
					// Window Focus
					if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
					{
						game.IsActive = true;

						if (!osxUseSpaces)
						{
							// If we alt-tab away, we lose the 'fullscreen desktop' flag on some WMs
							SDL.SDL_SetWindowFullscreen(
								game.Window.Handle,
								game.GraphicsDevice.PresentationParameters.IsFullScreen
									? (uint) SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP
									: 0);
						}

						// Disable the screen saver when we're back.
						SDL.SDL_DisableScreenSaver();
					}
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
					{
						game.IsActive = false;

						if (!osxUseSpaces)
						{
							SDL.SDL_SetWindowFullscreen(game.Window.Handle, 0);
						}

						// Give the screen saver back, we're not that important now.
						SDL.SDL_EnableScreenSaver();
					}

					// Window Resize
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
					{
						// This is called on both API and WM resizes
						//Mouse.INTERNAL_WindowWidth = evt.window.data1;
						//Mouse.INTERNAL_WindowHeight = evt.window.data2;
					}
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
					{
						/* This should be called on user resize only, NOT ApplyChanges!
						 * Sadly some window managers are idiots and fire events anyway.
						 * Also ignore any other "resizes" (alt-tab, fullscreen, etc.)
						 * -flibit
						 */
						if (GetWindowResizable(game.Window.Handle))
						{
							((SDLGameWindow)game.Window).ClientSizeChangedInternal();
						}
					}
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED)
					{
						// This is typically called when the window is made bigger
						//game.RedrawWindow();
					}

					// Window Move
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MOVED)
					{
						/* Apparently if you move the window to a new
						 * display, a GraphicsDevice Reset occurs.
						 * -flibit
						 */
						int newIndex = SDL.SDL_GetWindowDisplayIndex(game.Window.Handle);
						if (newIndex != displayIndex)
						{
							displayIndex = newIndex;
							game.GraphicsDevice.Reset(game.GraphicsDevice.PresentationParameters,
							                          Graphics.GraphicsAdapter.GetGraphicsAdapters()[displayIndex]);
						}
					}

					// Mouse Focus
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER)
					{
						SDL.SDL_DisableScreenSaver();
					}
					else if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE)
					{
						SDL.SDL_EnableScreenSaver();
					}
				}

				//// Display Events
				//else if (evt.type == SDL.SDL_EventType.SDL_DISPLAYEVENT)
				//{
				//	// Orientation Change
				//	if (evt.display.displayEvent == SDL.SDL_DisplayEventID.SDL_DISPLAYEVENT_ORIENTATION)
				//	{
				//		DisplayOrientation orientation = INTERNAL_ConvertOrientation(
				//			(SDL.SDL_DisplayOrientation)evt.display.data1
				//		);

				//		INTERNAL_HandleOrientationChange(
				//			orientation,
				//			game.GraphicsDevice,
				//			(FNAWindow)game.Window
				//		);
				//	}
				//}

				//// Controller device management
				//else if (evt.type == SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED)
				//{
				//	INTERNAL_AddInstance(evt.cdevice.which);
				//}
				//else if (evt.type == SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED)
				//{
				//	INTERNAL_RemoveInstance(evt.cdevice.which);
				//}

				//// Text Input
				//else if (evt.type == SDL.SDL_EventType.SDL_TEXTINPUT && !textInputSuppress)
				//{
				//	// Based on the SDL2# LPUtf8StrMarshaler
				//	unsafe
				//	{
				//		byte* endPtr = evt.text.text;
				//		if (*endPtr != 0)
				//		{
				//			int bytes = 0;
				//			while (*endPtr != 0)
				//			{
				//				endPtr++;
				//				bytes += 1;
				//			}

				//			/* UTF8 will never encode more characters
				//			 * than bytes in a string, so bytes is a
				//			 * suitable upper estimate of size needed
				//			 */
				//			char* charsBuffer = stackalloc char[bytes];
				//			int chars = Encoding.UTF8.GetChars(
				//				evt.text.text,
				//				bytes,
				//				charsBuffer,
				//				bytes
				//			);

				//			for (int i = 0; i < chars; i += 1)
				//			{
				//				TextInputEXT.OnTextInput(charsBuffer[i]);
				//			}
				//		}
				//	}
				//}

				// Quit
				else if (evt.type == SDL.SDL_EventType.SDL_QUIT)
				{
					game.RunApplication = false;
					break;
				}
			}
		}

		private bool PrepareGLAttributes()
		{
			var depthSize = 24;
			var stencilSize = 8;

			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, depthSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_STENCIL_SIZE, stencilSize);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
#if DEBUG
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_FLAGS,
			                        (int) SDL.SDL_GLcontext.SDL_GL_CONTEXT_DEBUG_FLAG);
#endif
			return true;
		}

		//static SDLPlatform()
		//{
		//	AppDomain.CurrentDomain.ProcessExit += ProcessExit;
		//	Initialize();
		//}

		//private static void ProcessExit(object sender, EventArgs e)
		//{
		//	//AudioEngine.ProgramExiting = true;

		//	//if (SoundEffect.FAudioContext.Context != null)
		//	//{
		//	//	SoundEffect.FAudioContext.Context.Dispose();
		//	//}
		//	//Media.MediaPlayer.DisposeIfNecessary();

		//	// This _should_ be the last SDL call we make...
		//	SDL.SDL_Quit();
		//}
	}
}