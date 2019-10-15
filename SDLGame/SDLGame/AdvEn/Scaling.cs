namespace SDLGame.AdvEn
{
	public struct Scaling
	{
		public float Scale { get; set; }
		public int Y { get; set; }

		public static float GetScaling(Scaling scaling, int y)
		{
			//for (size_t i = 0; i < _scalings.size(); i++)
			//{
			//	const auto &scaling = _scalings[i];
			//	if (yPos < scaling.yPos)
			//	{
			//		if (i == 0)
			//			return _scalings[i].scale;
			//		auto prevScaling = _scalings[i - 1];
			//		auto dY = scaling.yPos - prevScaling.yPos;
			//		auto dScale = scaling.scale - prevScaling.scale;
			//		auto p = (yPos - prevScaling.yPos) / dY;
			//		auto scale = prevScaling.scale + (p * dScale);
			//		return scale;
			//	}
			//}
			return 1f;
		}
	}
}