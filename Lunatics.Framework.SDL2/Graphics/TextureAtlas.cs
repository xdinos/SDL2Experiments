using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lunatics.Graphics
{
	public class TextureAtlas
	{
		public string Name { get; }

		public Texture2D Texture { get; }

		public IEnumerable<TextureRegion2D> Regions => _regionsByIndex;
		public int RegionCount => _regionsByIndex.Count;

		public TextureRegion2D this[string name] => GetRegion(name);
		public TextureRegion2D this[int index] => GetRegion(index);

		public static TextureAtlas Create(string name,
		                                  Texture2D texture,
		                                  int regionWidth,
		                                  int regionHeight,
		                                  int maxRegionCount = int.MaxValue,
		                                  int margin = 0,
		                                  int spacing = 0)
		{
			var textureAtlas = new TextureAtlas(name, texture);
			var count = 0;
			var width = texture.Width - margin;
			var height = texture.Height - margin;
			var xIncrement = regionWidth + spacing;
			var yIncrement = regionHeight + spacing;

			for (var y = margin; y < height; y += yIncrement)
			{
				for (var x = margin; x < width; x += xIncrement)
				{
					var regionName = $"{name ?? "region"}{count}";
					textureAtlas.CreateRegion(regionName, x, y, regionWidth, regionHeight);
					count++;

					if (count >= maxRegionCount)
						return textureAtlas;
				}
			}

			return textureAtlas;
		}

		public TextureAtlas(string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;

			_regionsByName = new Dictionary<string, TextureRegion2D>();
			_regionsByIndex = new List<TextureRegion2D>();
		}

		public TextureAtlas(string name, Texture2D texture, Dictionary<string, Rectangle> regions)
			: this(name, texture)
		{
			foreach (var region in regions)
				CreateRegion(region.Key, region.Value.X, region.Value.Y, region.Value.Width, region.Value.Height);
		}

		public bool ContainsRegion(string name)
		{
			return _regionsByName.ContainsKey(name);
		}

		private void AddRegion(TextureRegion2D region)
		{
			_regionsByIndex.Add(region);
			_regionsByName.Add(region.Name, region);
		}

		public TextureRegion2D CreateRegion(string name, int x, int y, int width, int height)
		{
			if (_regionsByName.ContainsKey(name))
				throw new InvalidOperationException($"Region {name} already exists in the texture atlas");

			var region = new TextureRegion2D(name, Texture, x, y, width, height);
			AddRegion(region);
			return region;
		}

		public void RemoveRegion(int index)
		{
			var region = _regionsByIndex[index];
			_regionsByIndex.RemoveAt(index);

			if (region.Name != null)
				_regionsByName.Remove(region.Name);
		}

		public void RemoveRegion(string name)
		{
			if (_regionsByName.TryGetValue(name, out var region))
			{
				_regionsByName.Remove(name);
				_regionsByIndex.Remove(region);
			}
		}

		public TextureRegion2D GetRegion(int index)
		{
			if (index < 0 || index >= _regionsByIndex.Count)
				throw new IndexOutOfRangeException();

			return _regionsByIndex[index];
		}

		public TextureRegion2D GetRegion(string name)
		{
			return GetRegion<TextureRegion2D>(name);
		}

		public T GetRegion<T>(string name) where T : TextureRegion2D
		{
			if (_regionsByName.TryGetValue(name, out var region))
				return (T)region;

			throw new KeyNotFoundException(name);
		}

		private readonly List<TextureRegion2D> _regionsByIndex;
		private readonly Dictionary<string, TextureRegion2D> _regionsByName;
	}
}