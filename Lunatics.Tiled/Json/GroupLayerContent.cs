using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class GroupLayerContent : LayerContent
	{
		/// <summary>
		/// Array of Layers
		/// </summary>
		[JsonProperty("layers")]
		public List<LayerContent> Layers { get; set; }

		public GroupLayerContent()
			: base(LayerType.GroupLayer)
		{
		}
	}
}