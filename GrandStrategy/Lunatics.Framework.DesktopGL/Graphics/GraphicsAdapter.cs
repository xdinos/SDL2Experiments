using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	public sealed class GraphicsAdapter : Framework.Graphics.GraphicsAdapter
	{
		public override DisplayMode CurrentDisplayMode
		{
			get
			{
				Sdl.Display.GetCurrentDisplayMode(_adapterIndex, out var mode);
				return new DisplayMode(mode.Width, mode.Height, SurfaceFormat.Color);
			}
		}

		public override IReadOnlyCollection<DisplayMode> SupportedDisplayModes
		{
			get
			{
				if (_displayModes == null)
				{
					var modeCount = Sdl.Display.GetNumDisplayModes(_adapterIndex);
					var modes = new List<DisplayMode>(modeCount);

					for (var i = 0; i < modeCount; i++)
					{
						Sdl.Display.GetDisplayMode(_adapterIndex, i, out var mode);

						// We are only using one format, Color
						// mode.Format gets the Color format from SDL
						var displayMode = new DisplayMode(mode.Width, mode.Height, SurfaceFormat.Color);
						if (!modes.Contains(displayMode))
							modes.Add(displayMode);
					}

					modes.Sort(delegate (DisplayMode a, DisplayMode b)
					           {
						           if (a == b) return 0;
						           if (a.Format <= b.Format && a.Width <= b.Width && a.Height <= b.Height) return -1;

						           return 1;
					           });

					_displayModes = modes.ToList();
				}

				return _displayModes;
			}
		}

		public GraphicsAdapter(int index, string name, string description)
			: base(name, description)
		{
			_adapterIndex = index;
		}

		private readonly int _adapterIndex;
		private List<DisplayMode> _displayModes;
	}
}