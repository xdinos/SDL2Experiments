using System.Collections.Generic;
using System.Collections.ObjectModel;
using Lunatics.Framework.Graphics;
using SDL2;

namespace Lunatics.Framework.Sdl.Graphics
{
	public sealed class GraphicsAdapter : Framework.Graphics.GraphicsAdapter
	{
		internal GraphicsAdapter(IEnumerable<DisplayMode> modes, string name, string description) 
			: base(modes, name, description)
		{
		}

		public static Framework.Graphics.GraphicsAdapter[] GetGraphicsAdapters()
		{
			if (_adapters != null)
				return _adapters;

			_adapters = new Framework.Graphics.GraphicsAdapter[SDL.SDL_GetNumVideoDisplays()];
			for (int i = 0; i < _adapters.Length; i += 1)
			{
				List<DisplayMode> modes = new List<DisplayMode>();
				int numModes = SDL.SDL_GetNumDisplayModes(i);
				for (int j = numModes - 1; j >= 0; j -= 1)
				{
					SDL.SDL_GetDisplayMode(i, j, out var filler);

					// Check for dupes caused by varying refresh rates.
					bool dupe = false;
					foreach (DisplayMode mode in modes)
					{
						if (filler.w == mode.Width && filler.h == mode.Height)
						{
							dupe = true;
						}
					}
					if (!dupe)
					{
						modes.Add(
							new DisplayMode(
								filler.w,
								filler.h,
								SurfaceFormat.Color // FIXME: Assumption!
							)
						);
					}
				}

				_adapters[i] = new GraphicsAdapter(modes, $@"\\.\DISPLAY{(i + 1)}", SDL.SDL_GetDisplayName(i));
			}
			return _adapters;
		}

		static GraphicsAdapter()
		{
			Adapters = new ReadOnlyCollection<Framework.Graphics.GraphicsAdapter>(GetGraphicsAdapters());
		}

		private static Framework.Graphics.GraphicsAdapter[] _adapters;
	}
}