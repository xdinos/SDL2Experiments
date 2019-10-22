using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Lunatics.Framework.Graphics;

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

			Window = CreateWindow();
		}

		public override IReadOnlyCollection<GraphicsAdapter> GetGraphicsAdapters() => Adapters;

		protected override void RunLoop()
		{
			Sdl.Window.Show(Window.Handle);

			_displayIndex = Sdl.Window.GetDisplayIndex(Window.Handle);

			while (!_isExiting)
			{
				HandleEvents();
				Game.Tick();
			}
		}

		protected override void Present()
		{
			Game.GraphicsDevice.Present();
		}

		protected override void Dispose(bool disposing)
		{
			if (Window != null)
			{
				Window.Dispose();
				Window = null;

				Sdl.Quit();
			}
			base.Dispose(disposing);
		}

		private GameWindow CreateWindow()
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

		private void HandleEvents()
		{
			while (Sdl.PollEvent(out var ev) == 1)
			{
				if (ev.Type == Sdl.EventType.Quit)
				{
					_isExiting = true;
				}
				else if (ev.Type == Sdl.EventType.WindowEvent /*&& ev.Window.WindowID == ((Window)Window).Id*/)
				{
					if (ev.Window.EventID == Sdl.Window.EventId.FocusGained)
					{
						IsActive = true;
						Sdl.DisableScreenSaver();
					}
					else if (ev.Window.EventID==Sdl.Window.EventId.FocusLost)
					{
						IsActive = false;
						Sdl.EnableScreenSaver();
					}
					else if (ev.Window.EventID == Sdl.Window.EventId.Enter)
					{
						Sdl.DisableScreenSaver();
					}
					else if (ev.Window.EventID == Sdl.Window.EventId.Leave)
					{
						Sdl.EnableScreenSaver();
					}
					else if (ev.Window.EventID == Sdl.Window.EventId.Moved)
					{
						var newDisplayIndex = Sdl.Window.GetDisplayIndex(Window.Handle);
						if (newDisplayIndex != _displayIndex)
						{
							_displayIndex = newDisplayIndex;
							// TODO: Game.GraphicsDevice.Reset(Game.GraphicsDevice.PresentationParameters, GraphicsAdapter.Adapters[newDisplayIndex]);
						}
					}
					else if (ev.Window.EventID == Sdl.Window.EventId.Close)
					{
						_isExiting = true;
					}
				}
			}
		}

		private int _displayIndex;
		private bool _isExiting = false;

		internal static ReadOnlyCollection<GraphicsAdapter> Adapters => _adapters ?? (_adapters = new ReadOnlyCollection<GraphicsAdapter>(GetGraphicsAdaptersImpl()));

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