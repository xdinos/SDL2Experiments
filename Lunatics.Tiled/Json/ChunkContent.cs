using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	/// <summary>
	/// Chunks are used to store the tile layer data for infinite maps.
	/// </summary>
	public class ChunkContent
	{
		[JsonProperty("x")]
		public int X { get; set; }

		[JsonProperty("y")]
		public int Y { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("data")]
		public object RawData { get; set; }
	}
}