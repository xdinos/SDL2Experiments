using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;

namespace SDLGame.AdvEn
{
	public class Object
	{
		public string Name { get; set; }
		public Vector2 Position { get; set; }
		public Rectangle HotSpot { get; set; }
		public bool Prop { get; set; }
		public bool Spot { get; set; }
		public bool Trigger { get; set; }
		public int ZSort { get; set; }
		public Vector2 UsePosition { get; set; }
		public Sprite Sprite { get; set; }
	}
}