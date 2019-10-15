using System.Runtime.Serialization;

namespace Lunatics.Tiled.Json
{
	public enum StaggerIndexContent : byte
	{
		[EnumMember(Value = "even")] Even,
		[EnumMember(Value = "odd")] Odd
	}
}