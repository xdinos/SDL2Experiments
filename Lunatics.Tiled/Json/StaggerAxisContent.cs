using System.Runtime.Serialization;

namespace Lunatics.Tiled.Json
{
	public enum StaggerAxisContent : byte
	{
		[EnumMember(Value = "x")] X,
		[EnumMember(Value = "y")] Y
	}
}