using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lunatics.Tiled.Json
{
	public abstract class LayerContent
	{
		[JsonIgnore]
		public LayerType Type { get; }

		/// <summary>
		/// Incremental id - unique across all layers
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; set; }

		/// <summary>
		/// Name assigned to this layer
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Whether layer is shown or hidden in editor
		/// </summary>
		[JsonProperty("visible")]
		public bool Visible { get; set; }

		/// <summary>
		/// Horizontal layer offset in tiles. Always 0.
		/// </summary>
		[JsonProperty("x")]
		public int X { get; set; }

		/// <summary>
		/// Vertical layer offset in tiles.Always 0.
		/// </summary>
		[JsonProperty("y")]
		public int Y { get; set; }

		/// <summary>
		/// Column count.Same as map width for fixed-size maps.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; set; }

		/// <summary>
		/// Row count. Same as map height for fixed-size maps.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; set; }

		/// <summary>
		/// Horizontal layer offset in pixels(default: 0)
		/// </summary>
		[JsonProperty("offsetx")]
		public float OffsetX { get; set; }

		/// <summary>
		/// Vertical layer offset in pixels (default: 0)
		/// </summary>
		[JsonProperty("offsety")]
		public float OffsetY { get; set; }

		/// <summary>
		/// Value between 0 and 1
		/// </summary>
		[JsonProperty("opacity")]
		public float Opacity { get; set; }

		/// <summary>
		/// A list of properties(name, value, type).
		/// </summary>
		[JsonProperty("properties")]
		public List<PropertyContent> Properties { get; set; }

		protected LayerContent(LayerType type)
		{
			Type = type;
			Opacity = 1.0f;
			Visible = true;
			Properties = new List<PropertyContent>();
		}
	}
}