using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class PropertyContent
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string type { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }
	}
}