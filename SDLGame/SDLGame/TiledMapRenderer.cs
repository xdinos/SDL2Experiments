using System;
using System.Collections.Generic;
using System.Linq;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using Lunatics.Tiled;

namespace SDLGame
{
	public class TiledMapRenderer : IDisposable
	{
		public TiledMapRenderer(Renderer renderer, Map map)
		{
			_renderer = renderer;
			_map = map;

			_tileSets.AddRange(_map.TileSets.Select(x=>new Tuple<TileSet, int, Texture2D>(x, x.FirstGlobalIdentifier, Texture2D.Load(_renderer, x.ImagePath))));
		}

		public void Dispose()
		{
			foreach (var item in _tileSets)
			{
				item.Item3.Dispose();
			}
		}

		public void Update(long deltaTicks)
		{

		}

		public void Draw()
		{
			DrawLayers(_map.Layers.Where(x => x.IsVisible));
		}

		private void DrawLayers(IEnumerable<Layer> layers)
		{
			foreach (var layer in layers)
			{
				switch (layer)
				{
					case TileLayer tileLayer:
						DrawTileLayer(tileLayer);
						break;
					case GroupLayer groupLayer:
						DrawLayers(groupLayer.Layers.Where(x=>x.IsVisible));
						break;
				}
			}
		}

		private void DrawTileLayer(TileLayer tileLayer)
		{
			var scale =new Vector2(2f);
			var offset = Vector2.Zero;
			foreach (var chunk in tileLayer.Chunks)
			{
				DrawChunk(chunk, offset, scale);
				offset.X += chunk.Width * _map.TileWidth * scale.X;
			}
		}

		private void DrawChunk(Chunk chunk, Vector2 offset, Vector2 scale)
		{
			var sx = offset.X;
			var sy = 0;

			for (var y = 0; y < chunk.Height; y++)
			{
				for (var x = 0; x < chunk.Width; x++)
				{
					var tile = chunk.Tiles[y * chunk.Width + x];
					if (!tile.IsBlank)
					{
						var textureRegion = GeTextureRegion(tile.GlobalIdentifier);
						if (textureRegion != null)
						{
							_renderer.Draw(textureRegion, new Vector2(sx, sy), Vector2.Zero, 0f, scale);
						}
					}

					sx += (int)(_map.TileWidth * scale.X);
				}

				sx = offset.X;
				sy += (int) (_map.TileHeight * scale.Y);
			}
		}

		//public TextureRegion2D GetRegion(int column, int row)
		//{
		//	var x = Margin + column * (TileWidth + Spacing);
		//	var y = Margin + row * (TileHeight + Spacing);
		//	return new TextureRegion2D(Texture, x, y, TileWidth, TileHeight);
		//}

		public TextureRegion2D GeTextureRegion(int tileIdentifier)
		{
			foreach (var item in _tileSets)
			{
				if (tileIdentifier >= item.Item2 && tileIdentifier < item.Item2 + item.Item1.TileCount)
					return new TextureRegion2D(item.Item3, GetTileSourceRectangle(item.Item1, tileIdentifier));
			}

			return null;
		}

		public Tuple<TileSet, Texture2D> GetTileSetByTileGlobalIdentifier(int tileIdentifier)
		{
			foreach (var tileSet in _tileSets)
			{
				if (tileIdentifier >= tileSet.Item2 && tileIdentifier < tileSet.Item2 + tileSet.Item1.TileCount)
					return new Tuple<TileSet, Texture2D>(tileSet.Item1, tileSet.Item3);
			}

			return null;
		}

		internal static Rectangle GetTileSourceRectangle(TileSet tileSet, int localTileIdentifier)
		{
			var x = tileSet.Margin + ((localTileIdentifier % tileSet.Columns) -1)* (tileSet.TileWidth + tileSet.Spacing);
			var y = tileSet.Margin + (localTileIdentifier / tileSet.Columns)* (tileSet.TileHeight + tileSet.Spacing);
			return new Rectangle(x, y, tileSet.TileWidth, tileSet.TileHeight);
		}

		internal static Rectangle GetTileSourceRectangle(int localTileIdentifier, int tileWidth, int tileHeight, int columns, int margin, int spacing)
		{
			var x = margin + localTileIdentifier % columns * (tileWidth + spacing);
			var y = margin + localTileIdentifier / columns * (tileHeight + spacing);
			return new Rectangle(x, y, tileWidth, tileHeight);
		}

		private readonly Map _map;
		private readonly Renderer _renderer;
		private readonly List<Tuple<TileSet, int, Texture2D>> _tileSets = new List<Tuple<TileSet, int, Texture2D>>();
	}
}