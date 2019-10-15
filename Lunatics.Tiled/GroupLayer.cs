using System.Collections.Generic;
using System.Linq;

namespace Lunatics.Tiled
{
	public class GroupLayer : Layer
	{
		public List<Layer> Layers { get; }

		public GroupLayer(string name,
		                  IEnumerable<Layer> layers,
		                  float offsetX = 0f,
		                  float offsetY = 0f,
		                  float opacity = 1f,
		                  bool isVisible = true)
			: base(name, offsetX, offsetY, opacity, isVisible)
		{
			Layers = layers.ToList();
		}
	}

	public class ObjectLayer : Layer
	{
		public ObjectLayer(string name,
		                   float offsetX = 0f,
		                   float offsetY = 0f,
		                   float opacity = 1f,
		                   bool isVisible = true)
			: base(name, offsetX, offsetY, opacity, isVisible)
		{
		}
	}

	public class ImageLayer : Layer
	{
		public ImageLayer(string name,
		                  float offsetX = 0f,
		                  float offsetY = 0f,
		                  float opacity = 1f,
		                  bool isVisible = true)
			: base(name, offsetX, offsetY, opacity, isVisible)
		{
		}
	}
}