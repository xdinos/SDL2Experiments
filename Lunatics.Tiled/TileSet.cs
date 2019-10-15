namespace Lunatics.Tiled
{
	public class TileSet
	{
		public string Name { get; }
		public int FirstGlobalIdentifier { get; }
		public string ImagePath { get; }
		public int TileWidth { get; }
		public int TileHeight { get; }
		public int Spacing { get; }
		public int Margin { get; }
		public int TileCount { get; }
		public int Columns { get; }

		public TileSet(string name,
					   int firstGlobalIdentifier,
					   string imagePath,
		               int tileWidth,
		               int tileHeight,
		               int tileCount,
		               int spacing,
		               int margin,
		               int columns)
		{
			Name = name;
			FirstGlobalIdentifier = firstGlobalIdentifier;
			ImagePath = imagePath;
			TileWidth = tileWidth;
			TileHeight = tileHeight;
			TileCount = tileCount;
			Spacing = spacing;
			Margin = margin;
			Columns = columns;
		}
	}
}