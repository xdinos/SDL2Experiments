using System;

namespace Lunatics.Tiled
{
	[Flags]
	public enum TileFlipFlags : uint
	{
		None = 0,
		FlipDiagonally = 0x20000000,
		FlipVertically = 0x40000000,
		FlipHorizontally = 0x80000000,
		All = FlipDiagonally | FlipVertically | FlipHorizontally
	}
}