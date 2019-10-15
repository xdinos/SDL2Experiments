using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsAdapter
	{
		public string DeviceName { get; }

		public string Description { get; }

		public IReadOnlyCollection<DisplayMode> SupportedDisplayModes { get; }

		protected GraphicsAdapter(IEnumerable<DisplayMode> modes, string name, string description)
		{
			SupportedDisplayModes = modes.ToList();
			DeviceName = name;
			Description = description;;
		}

		public static ReadOnlyCollection<GraphicsAdapter> Adapters { get; protected set; }
	}
}