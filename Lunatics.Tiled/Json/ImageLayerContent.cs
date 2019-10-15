using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class ImageLayerContent : LayerContent
	{
		/// <summary>
		/// Image used by this layer. imagelayer only.
		/// </summary>
		[JsonProperty("image")]
		public string Image { get; set; }

		/// <summary>
		/// Hex-formatted color (#RRGGBB) (optional, imagelayer only
		/// </summary>
		[JsonProperty("transparentcolor")]
		public string TransparentColor { get; set; }

		public ImageLayerContent()
			: base(LayerType.ImageLayer)
		{
		}
	}
}