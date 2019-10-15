using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class TileLayerContent : LayerContent
	{
		public const string JsonTypeName = "tilelayer";

		public TileLayerContent() 
			: base(LayerType.TileLayer)
		{
		}

		/// <summary>
		/// csv (default) or base64.
		/// </summary>
		[JsonProperty("encoding")]
		public string Encoding { get; set; }

		/// <summary>
		/// zlib, gzip or empty(default).
		/// </summary>
		[JsonProperty("compression")]
		public string Compression { get; set; }

		/// <summary>
		/// Array of chunks(optional).
		/// </summary>
		[JsonProperty("chunks")]
		public List<ChunkContent> Chunks { get; set; }

		//data array or string Array of unsigned int (GIDs) or base64-encoded data.tilelayer only.
		[JsonProperty("data")]
		public object RawData { get; set; }

		[JsonIgnore]
		public List<uint> Tiles { get; set; }
	}
}