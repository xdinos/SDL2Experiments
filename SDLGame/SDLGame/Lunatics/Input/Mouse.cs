namespace Lunatics.Input
{
	public static class Mouse
	{
		public enum ButtonState
		{
			Released,
			Pressed
		}

		public static ButtonState LeftButton { get; internal set; }
		public static ButtonState RightButton { get; internal set; }

		public static int X { get; internal set; }
		public static int Y { get; internal set; }
		public static int ScrollX { get; internal set; }
		public static int ScrollY { get; internal set; }
	}
}