using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Lunatics.Tiled.Json
{
	public class ObjectLayerContent : LayerContent
	{
		public enum DrawOrderContent : byte
		{
			[EnumMember(Value = "topdown")]
			TopDown,

			[EnumMember(Value = "index")]
			Manual
		}

		/// <summary>
		/// topdown (default) or index. objectgroup only.
		/// </summary>
		[JsonProperty("draworder")]
		public DrawOrderContent DrawOrder { get; set; }

		/// <summary>
		/// Array of objects. objectgroup only.
		/// </summary>
		[JsonProperty("objects")]
		public List<ObjectContent> Objects { get; set; }

		public ObjectLayerContent()
			: base(LayerType.ObjectLayer)
		{
		}
	}

	public class ObjectContent
	{
		//ellipse bool Used to mark an object as an ellipse
		//gid int GID, only if object comes from a Tilemap
		//height  double Height in pixels.Ignored if using a gid.
		//id  int Incremental id - unique across all objects
		//name    string String assigned to name field in editor
		//point   bool Used to mark an object as a point
		//polygon array   A list of x, y coordinates in pixels
		//polyline    array A list of x, y coordinates in pixels
		//properties  array A list of properties (name, value, type)
		//rotation    double Angle in degrees clockwise
		//template    string Reference to a template file, in case object is a template instance
		//text    object String key-value pairs
		//type    string String assigned to type field in editor
		//visible bool Whether object is shown in editor.
		//width   double Width in pixels.Ignored if using a gid.
		//x double X coordinate in pixels
		//y   double Y coordinate in pixels
}
}