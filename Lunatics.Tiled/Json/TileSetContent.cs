using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class TileOffsetContent
	{
		/// <summary>
		/// Horizontal offset in pixels
		/// </summary>
		[JsonProperty("x")]
		public int X { get; set; }

		/// <summary>
		/// Vertical offset in pixels(positive is down)
		/// </summary>
		[JsonProperty("y")]
		public int Y { get; set; }
	}

	public class TileSetContent
	{
		/// <summary>
		/// GID corresponding to the first tile in the set
		/// </summary>
		[JsonProperty("firstgid")]
		public int FirstGlobalIdentifier { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }

		/// <summary>
		/// tileset (for tileset files, since 1.0)
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; set; }

		/// <summary>
		/// Name given to this tileset
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		
		/// <summary>
		/// The number of tiles in this tileset
		/// </summary>
		[JsonProperty("tilecount")]
		public int TileCount { get; set; }

		/// <summary>
		/// Maximum width of tiles in this set
		/// </summary>
		[JsonProperty("tilewidth")]
		public int TileWidth { get; set; }

		/// <summary>
		/// Maximum height of tiles in this set
		/// </summary>
		[JsonProperty("tileheight")]
		public int TileHeight { get; set; }

		/// <summary>
		/// Buffer between image edge and first tile(pixels)
		/// </summary>
		[JsonProperty("margin")]
		public int Margin { get; set; }

		/// <summary>
		/// Spacing between adjacent tiles in image(pixels)
		/// </summary>
		[JsonProperty("spacing")]
		public int Spacing { get; set; }

		/// <summary>
		/// The number of tile columns in the tileset
		/// </summary>
		[JsonProperty("columns")]
		public int Columns { get; set; }

		/// <summary>
		/// Image used for tiles in this set
		/// </summary>
		[JsonProperty("image")]
		public string Image { get; set; }

		/// <summary>
		/// Width of source image in pixels
		/// </summary>
		[JsonProperty("imagewidth")]
		public int ImageWidth { get; set; }

		/// <summary>
		/// Height of source image in pixels
		/// </summary>
		[JsonProperty("imageheight")]
		public int ImageHeight { get; set; }

		/// <summary>
		/// TileOffset (optional)
		/// This element is used to specify an offset in pixels,
		/// to be applied when drawing a tile from the related tileset.
		/// When not present, no offset is applied.
		/// </summary>
		[JsonProperty("tileoffset")]
		public TileOffsetContent TileOffset { get; set; }

		//transparentcolor    string Hex-formatted color(#RRGGBB) (optional)

		//properties  array A list of properties(name, value, type).
		//terrains array   Array of Terrains(optional)
		//tiles array   Array of Tiles(optional)
		//grid object See<grid>(optional)
		//wangsets array   Array of Wang sets(since 1.1.5)
	}
}