using System.Collections.Generic;

namespace Lunatics.Tiled
{
	public class TileLayer : Layer
	{
		public int Width { get; }
		public int Height { get; }
		public int TileWidth { get; }
		public int TileHeight { get; }

		public IReadOnlyList<Chunk> Chunks => _chunks;

		public TileLayer(string name,
		                 int width, 
		                 int height, 
		                 int tileWidth, 
		                 int tileHeight,
						 IEnumerable<Chunk> chunks,
						 float offsetX = 0f,
		                 float offsetY = 0f,
		                 float opacity = 1f,
		                 bool isVisible = true)
			: base(name, offsetX, offsetY, opacity, isVisible)
		{
			Width = width;
			Height = height;
			TileWidth = tileWidth;
			TileHeight = tileHeight;

			_chunks.AddRange(chunks);
		}

		private readonly List<Chunk> _chunks = new List<Chunk>();
	}
}