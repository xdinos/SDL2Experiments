namespace Lunatics.Tiled
{
	public struct Tile
	{
		public readonly ushort X;
		public readonly ushort Y;
		public readonly uint GlobalTileIdentifierWithFlags;

		public int GlobalIdentifier => (int)(GlobalTileIdentifierWithFlags & ~(uint)TileFlipFlags.All);
		public bool IsFlippedHorizontally => (GlobalTileIdentifierWithFlags & (uint)TileFlipFlags.FlipHorizontally) != 0;
		public bool IsFlippedVertically => (GlobalTileIdentifierWithFlags & (uint)TileFlipFlags.FlipVertically) != 0;
		public bool IsFlippedDiagonally => (GlobalTileIdentifierWithFlags & (uint)TileFlipFlags.FlipDiagonally) != 0;
		public bool IsBlank => GlobalIdentifier == 0;
		public TileFlipFlags Flags => (TileFlipFlags)(GlobalTileIdentifierWithFlags & (uint)TileFlipFlags.All);

		public Tile(uint globalTileIdentifierWithFlags, ushort x, ushort y)
		{
			GlobalTileIdentifierWithFlags = globalTileIdentifierWithFlags;
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return $"GlobalIdentifier: {GlobalIdentifier}, Flags: {Flags}";
		}
	}
}