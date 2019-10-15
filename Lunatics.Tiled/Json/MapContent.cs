using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class MapContent
	{
		/// <summary>
		/// map (since 1.0)
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; set; }

		/// <summary>
		/// The JSON format version
		/// </summary>
		[JsonProperty("version")]
		public string Version { get; set; }

		/// <summary>
		/// The Tiled version used to save the file
		/// </summary>
		[JsonProperty("tiledversion")]
		public string TiledVersion { get; set; }

		/// <summary>
		/// Whether the map has infinite dimensions
		/// </summary>
		[JsonProperty("infinite")]
		public bool Infinite { get; set; }

		/// <summary>
		/// Number of tile columns
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; set; }

		/// <summary>
		/// Number of tile rows
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; set; }
		
		/// <summary>
		/// Map grid width
		/// </summary>
		[JsonProperty("tilewidth")]
		public int TileWidth { get; set; }

		/// <summary>
		/// Map grid height
		/// </summary>
		[JsonProperty("tileheight")]
		public int TileHeight { get; set; }

		/// <summary>
		/// orthogonal, isometric, staggered or hexagonal
		/// </summary>
		[JsonProperty("orientation")]
		public OrientationContent Orientation { get; set; }

		/// <summary>
		/// Rendering direction(orthogonal maps only)
		/// </summary>
		[JsonProperty("renderorder")]
		public RenderOrderContent RenderOrder { get; set; }

		/// <summary>
		/// x or y(staggered / hexagonal maps only)
		/// </summary>
		[JsonProperty("staggeraxis")]
		public StaggerAxisContent StaggerAxis { get; set; }

		/// <summary>
		/// odd or even (staggered / hexagonal maps only)
		/// </summary>
		[JsonProperty("staggerindex")]
		public StaggerIndexContent StaggerIndex { get; set; }

		/// <summary>
		/// Length of the side of a hex tile in pixels
		/// </summary>
		[JsonProperty("hexsidelength")]
		public int HexSideLength { get; set; }

		/// <summary>
		/// Hex-formatted color (#RRGGBB or #AARRGGBB) (optional)
		/// </summary>
		[JsonProperty("backgroundcolor")]
		public string BackgroundColor { get; set; }

		/// <summary>
		/// Auto-increments for each layer
		/// </summary>
		[JsonProperty("nextlayerid")]
		public int NextLayerId { get; set; }

		/// <summary>
		/// Auto-increments for each placed object
		/// </summary>
		[JsonProperty("nextobjectid")]
		public int NextObjectId { get; set; }

		/// <summary>
		/// Array of Layers
		/// </summary>
		[JsonProperty("layers")]
		public List<LayerContent> Layers { get; set; }

		/// <summary>
		/// Array of TileSets
		/// </summary>
		[JsonProperty("tilesets")]
		public List<TileSetContent> TileSets { get; set; }

		/// <summary>
		/// A list of properties(name, value, type).
		/// </summary>
		[JsonProperty("properties")]
		public List<PropertyContent> Properties { get; set; }
	}
}