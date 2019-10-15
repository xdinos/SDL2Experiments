using System;
using System.Collections.Generic;

namespace Lunatics.Tiled
{
	public partial class Map
	{
		public string Name { get; }
		public int Width { get; }
		public int Height { get; }
		public int TileWidth { get; }
		public int TileHeight { get; }

		public IReadOnlyList<Layer> Layers => _layers;
		public IReadOnlyList<TileSet> TileSets => _tileSets;

		public Map(string name, int width, int height, int tileWidth, int tileHeight)
		{
			Name = name;
			Width = width;
			Height = height;
			TileWidth = tileWidth;
			TileHeight = tileHeight;
		}

		public void AddTileSet(TileSet tileSet, int firstGlobalIdentifier)
		{
			_tileSets.Add(tileSet);
			_firstGlobalIdentifiers.Add(new Tuple<TileSet, int>(tileSet, firstGlobalIdentifier));
		}

		private void AddLayer(Layer layer, bool root = true)
		{
			if (root) _layers.Add(layer);

			//if (_layersByName.ContainsKey(layer.Name))
			//	throw new ArgumentException($"The TiledMap '{Name}' contains two or more layers named '{layer.Name}'. Please ensure all layers have unique names.");

			//_layersByName.Add(layer.Name, layer);


			switch (layer)
			{
				case ImageLayer imageLayer:
					_imageLayers.Add(imageLayer);
					break;
				case TileLayer tileLayer:
					_tileLayers.Add(tileLayer);
					break;
				case ObjectLayer objectLayer:
					_objectLayers.Add(objectLayer);
					break;
				case GroupLayer groupLayer:
					foreach (var subLayer in groupLayer.Layers)
						AddLayer(subLayer, false);
					break;
			}
		}
		
		private readonly List<Layer> _layers = new List<Layer>();
		private readonly List<TileSet> _tileSets = new List<TileSet>();
		private readonly List<TileLayer> _tileLayers = new List<TileLayer>();
		private readonly List<ImageLayer> _imageLayers = new List<ImageLayer>();
		private readonly List<ObjectLayer> _objectLayers = new List<ObjectLayer>();
		private readonly Dictionary<string, Layer> _layersByName = new Dictionary<string, Layer>();
		private readonly List<Tuple<TileSet, int>> _firstGlobalIdentifiers = new List<Tuple<TileSet, int>>();
	}
}