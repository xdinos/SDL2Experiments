using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunatics.Tiled.Json;
using Lunatics.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lunatics.Tiled
{
	public partial class Map
	{
		public static Map Load(string filePath)
		{
			var rootPath = Path.GetDirectoryName(filePath);

			var json = File.ReadAllText(filePath);
			var mapContent = JsonConvert.DeserializeObject<MapContent>(
				json,
				new JsonSerializerSettings
				{
					Converters = new List<JsonConverter>
					             {
						             new JsonLayerContentConverter()
					             }
				});

			var map = new Map(filePath,
			                  mapContent.Width,
			                  mapContent.Height,
			                  mapContent.TileWidth,
			                  mapContent.TileHeight);

			foreach (var t in mapContent.TileSets)
			{
				var tileSetContent = t;
				if (!string.IsNullOrEmpty(tileSetContent.Source))
				{
					tileSetContent = JsonConvert.DeserializeObject<TileSetContent>(
						File.ReadAllText(Path.Combine(rootPath, tileSetContent.Source)));
				}

				map.AddTileSet(new TileSet(tileSetContent.Name,
				                           tileSetContent.FirstGlobalIdentifier,
										   Path.Combine(rootPath, tileSetContent.Image),
				                           tileSetContent.TileWidth,
				                           tileSetContent.TileHeight,
				                           tileSetContent.TileCount,
				                           tileSetContent.Spacing,
				                           tileSetContent.Margin,
				                           tileSetContent.Columns),
				               tileSetContent.FirstGlobalIdentifier);
			}

			foreach (var layerContent in mapContent.Layers)
			{
				map.AddLayer(LoadLayer(map, layerContent));
			}

			return map;
		}

		private static Layer LoadLayer(Map map, LayerContent layerContent)
		{
			switch (layerContent)
			{
				case TileLayerContent tileLayerContent:
					return LoadTileLayer(map, tileLayerContent);
				case ObjectLayerContent objectLayerContent:
					return LoadObjectLayer(map, objectLayerContent);
				case ImageLayerContent imageLayerContent:
					return LoadImageLayer(map, imageLayerContent);
				case GroupLayerContent groupLayerContent:
					return LoadGroupLayer(map, groupLayerContent);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static TileLayer LoadTileLayer(Map map, TileLayerContent content)
		{
			var chunks = new List<Chunk>();

			if (content.RawData != null)
			{

			}

			foreach (var chunkContent in content.Chunks)
			{
				if (chunkContent.RawData is JArray array)
				{
					var tiles = array.Select(x => x.Value<uint>()).ToArray();
					var chunk = new Chunk(chunkContent.X,
					                      chunkContent.Y,
					                      chunkContent.Width,
					                      chunkContent.Height,
					                      tiles);
					chunks.Add(chunk);
				}
			}

			return new TileLayer(content.Name,
			                     content.Width,
			                     content.Height,
			                     map.TileWidth,
			                     map.TileHeight,
			                     chunks,
			                     content.OffsetX,
			                     content.OffsetY,
			                     content.Opacity,
			                     content.Visible);
		}

		private static ObjectLayer LoadObjectLayer(Map map, ObjectLayerContent ctx)
		{
			return new ObjectLayer(ctx.Name,
			                       ctx.OffsetX,
			                       ctx.OffsetY,
			                       ctx.Opacity,
			                       ctx.Visible);
		}

		private static ImageLayer LoadImageLayer(Map map, ImageLayerContent ctx)
		{
			return new ImageLayer(ctx.Name,
			                      ctx.OffsetX,
			                      ctx.OffsetY,
			                      ctx.Opacity,
			                      ctx.Visible);
		}

		private static GroupLayer LoadGroupLayer(Map map, GroupLayerContent ctx)
		{
			return new GroupLayer(ctx.Name,
			                      ctx.Layers.Select(x => LoadLayer(map, x)),
			                      ctx.OffsetX,
			                      ctx.OffsetY,
			                      ctx.Opacity,
			                      ctx.Visible);
		}

		private class JsonLayerContentConverter : JsonCreationConverter<LayerContent>
		{
			private const string TiledMapImageLayerName = "imagelayer";
			private const string TiledMapObjectLayerName = "objectgroup";
			private const string TiledMapGroupLayerName = "group";


			protected override LayerContent Create(Type objectType, JObject jObject)
			{
				if (!FieldExists("type", jObject))
					throw new InvalidOperationException();

				var type = jObject["type"].ToString();

				if (string.CompareOrdinal(type, TileLayerContent.JsonTypeName) == 0)
					return new TileLayerContent();

				if (string.CompareOrdinal(type, TiledMapImageLayerName) == 0)
					return new ImageLayerContent();

				if (string.CompareOrdinal(type, TiledMapObjectLayerName) == 0)
					return new ObjectLayerContent();

				if (string.CompareOrdinal(type, TiledMapGroupLayerName) == 0)
					return new GroupLayerContent();

				throw new InvalidOperationException();
			}

			private bool FieldExists(string fieldName, JObject jObject)
			{
				return jObject[fieldName] != null;
			}
		}
	}
}