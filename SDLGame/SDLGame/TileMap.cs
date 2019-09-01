using System;
using System.Collections.Generic;
using System.IO;
using Lunatics.Graphics;
using Newtonsoft.Json;
using SDL2;

namespace SDLGame
{
	public class TileMap
	{
		public static TileMap Load(IntPtr rendererPtr, string rootPath, string filename)
		{
			var json = File.ReadAllText(Path.Combine(rootPath, filename));
			var tileMap = JsonConvert.DeserializeObject<TileMap>(json);

			tileMap.TileSets = new List<TileSet>(tileMap.TileSetInfos.Count);

			foreach (var tileSetInfo in tileMap.TileSetInfos)
			{
				tileMap.TileSets.Add(TileSet.Load(rendererPtr, rootPath, tileSetInfo));
			}

			return tileMap;
		}

		public int Height { get; set; }
		public int Width { get; set; }

		public int TileHeight { get; set; }
		public int TileWidth { get; set; }
		public bool Infinite { get; set; }

		public List<Layer> Layers { get; set; }

		[JsonProperty("tilesets")]
		public List<TileSetInfo> TileSetInfos { get; set; }

		public List<TileSet> TileSets { get; set; }

		public void Update()
		{

		}

		public void Render(IntPtr rendererPtr)
		{
			foreach (var layer in Layers)
			{
				layer.Render(rendererPtr, TileSets);
			}
		}
	}
    
    public class Layer
	{
		public string Name { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Opacity { get; set; }
        public bool Visible { get; set; }

        public List<Chunk> Chunks { get; set; }
        public List<Layer> Layers { get; set; }

		public void Update()
		{

		}

		public void Render(IntPtr rendererPtr, List<TileSet> tileSets)
        {
            switch (Type)
            {
                case "group":
                    RenderGroup(rendererPtr, tileSets);
                    break;
                case "tilelayer":
                    RenderLayer(rendererPtr, tileSets);
                    break;
                case "objectgroup":
                    default:
                    break;
            }
		}

        private void RenderGroup(IntPtr rendererPtr, List<TileSet> tileSets)
        {
            foreach (var subLayer in Layers)
                subLayer.Render(rendererPtr, tileSets);
        }

        private void RenderLayer(IntPtr rendererPtr, List<TileSet> tileSets)
        {
            var scale = 2f;
            var sx0 = 0;
            foreach (var chunk in Chunks)
            {
                chunk.Render(rendererPtr, tileSets, sx0, scale);
                sx0 += chunk.Width * (int)(tileSets[0].TileWidth * scale);
            }
        }
    }

	public class Chunk
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public uint[] Data { get; set; }

		public void Update()
		{

		}

		public void Render(IntPtr rendererPtr, List<TileSet> tileSets, int sx0, float scale = 1f)
		{
			var tileSet = tileSets[0];

			var sx = sx0;
			var sy = 0;

			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < Width; x++)
				{
					var tileId = Data[y * Width + x];
					if (tileId != 0)
					{
						tileSet.Render(rendererPtr, tileId, sx, sy, scale);
					}

					sx += (int) (tileSet.TileWidth * scale);
				}

				sx = sx0;
				sy += (int) (tileSet.TileHeight * scale);
			}
		}
	}

	public class TileSetInfo
	{
		public string FirstGId { get; set; }
		public string Source { get; set; }
	}

	public class TileSet
	{
		public static TileSet Load(IntPtr rendererPtr, string rootPath, TileSetInfo info)
		{
			var tileSet = JsonConvert.DeserializeObject<TileSet>(
				File.ReadAllText(Path.Combine(rootPath, info.Source)));
			
			tileSet._texture = Texture2D.Load(rendererPtr, Path.Combine(rootPath, tileSet.Image));

			return tileSet;
		}

		public int Columns { get; set; }

		public string Image { get; set; }
		public int ImageHeight { get; set; }
		public int ImageWidth { get; set; }
		public int Margin { get; set; }
		public string Name { get; set; }
		public int Spacing { get; set; }
		public int TileCount { get; set; }
		//"tiledversion":"1.2.4",
		public int TileHeight { get; set; }
		public int TileWidth { get; set; }

		//"type":"tileset",
		//"version":1.2

		public void Render(IntPtr rendererPtr, uint tileId, int sx, int sy, float scale = 1f)
		{
			const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
			const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
			const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

			var flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;

			flip |= (tileId & FLIPPED_HORIZONTALLY_FLAG) == FLIPPED_HORIZONTALLY_FLAG
				        ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL
				        : SDL.SDL_RendererFlip.SDL_FLIP_NONE;
			flip |= (tileId & FLIPPED_VERTICALLY_FLAG) == FLIPPED_VERTICALLY_FLAG
						? SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL
				        : SDL.SDL_RendererFlip.SDL_FLIP_NONE;
			//bool flipped_diagonally = (tileId & FLIPPED_DIAGONALLY_FLAG);

			// Clear the flags
			var cleanTileId = (int) (tileId & ~(FLIPPED_HORIZONTALLY_FLAG |
			                                    FLIPPED_VERTICALLY_FLAG |
			                                    FLIPPED_DIAGONALLY_FLAG));
			var ty = (cleanTileId - 1) / Columns;
			var tx = (cleanTileId - 1) - (ty * Columns);

			var src = new SDL.SDL_Rect {h = TileHeight, w = TileWidth, x = tx * TileWidth, y = ty * TileHeight};
			var dst = new SDL.SDL_Rect {h = (int) (TileHeight * scale), w = (int) (TileWidth * scale), x = sx, y = sy};

			SDL.SDL_RenderCopyEx(rendererPtr, _texture.Handle, ref src, ref dst, 0D, IntPtr.Zero, flip);
		}

		private Texture2D _texture;
	}
    }