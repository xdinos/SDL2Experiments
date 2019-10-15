using System.Runtime.Serialization;

namespace Lunatics.Tiled.Json
{
	public enum RenderOrderContent : byte
	{
		[EnumMember(Value = "right-down")] RightDown,

		[EnumMember(Value = "right-up")] RightUp,

		[EnumMember(Value = "left-down")] LeftDown,

		[EnumMember(Value = "left-up")] LeftUp
	}
}