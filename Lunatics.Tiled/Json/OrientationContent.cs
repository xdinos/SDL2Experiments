using System.Runtime.Serialization;

namespace Lunatics.Tiled.Json
{
	public enum OrientationContent : byte
	{
		[EnumMember(Value = "orthogonal")] Orthogonal,

		[EnumMember(Value = "isometric")] Isometric,

		[EnumMember(Value = "staggered")] Staggered,

		[EnumMember(Value = "hexagonal")] Hexagonal
	}
}