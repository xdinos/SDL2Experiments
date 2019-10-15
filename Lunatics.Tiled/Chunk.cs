using System.Collections.Generic;

namespace Lunatics.Tiled
{
	public class Chunk
	{
		public int X { get; }

		public int Y { get; }

		public int Width { get; }

		public int Height { get; }

		public IReadOnlyList<Tile> Tiles => _tiles;

		public Chunk(int x, int y, int width, int height, uint[] tiles)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			_tiles = new Tile[Width * Height];

			for (ushort iy = 0; iy < Height; iy++)
			{
				for (ushort ix = 0; ix < Width; ix++)
				{
					var idx = iy * Width + ix;
					_tiles[idx] = new Tile(tiles[idx], ix, iy);
				}
			}
		}

		private readonly Tile[] _tiles;
	}
}