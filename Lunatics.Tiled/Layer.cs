namespace Lunatics.Tiled
{
	public abstract class Layer
	{
		public string Name { get; }
		public bool IsVisible { get; set; }
		public float Opacity { get; set; }
		public float OffsetX { get; set; }
		public float OffsetY { get; set; }

		// TODO: public TiledMapProperties Properties { get; }

		protected Layer(string name,
		                float offsetX = 0f,
		                float offsetY = 0f,
		                float opacity = 1f,
		                bool isVisible = true)
		{
			Name = name;
			OffsetX = offsetX;
			OffsetY = offsetY;
			Opacity = opacity;
			IsVisible = isVisible;
		}
	}
}