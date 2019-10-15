using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsAdapter : IDisposable
	{
		public string DeviceName { get; }

		public string Description { get; }

		public abstract DisplayMode CurrentDisplayMode { get; }

		public abstract IReadOnlyCollection<DisplayMode> SupportedDisplayModes { get; }

		public void Dispose()
		{
		}

		protected GraphicsAdapter(string deviceName, string description)
		{
			DeviceName = deviceName;
			Description = description;
		}
	}
}