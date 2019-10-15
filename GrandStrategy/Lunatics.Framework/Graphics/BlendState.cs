namespace Lunatics.Framework.Graphics
{
	public class BlendState
	{
		#region Properties

		public string Name { get; }

		public BlendFunction AlphaBlendFunction { get; }

		public Blend AlphaDestinationBlend { get; }

		public Blend AlphaSourceBlend { get; }

		public BlendFunction ColorBlendFunction { get; }

		public Blend ColorDestinationBlend { get; }

		public Blend ColorSourceBlend { get; }

		public ColorWriteChannels ColorWriteChannels { get; }

		public ColorWriteChannels ColorWriteChannels1 { get; }

		public ColorWriteChannels ColorWriteChannels2 { get; }

		public ColorWriteChannels ColorWriteChannels3 { get; }

		public Color BlendFactor { get; set; }

		public int MultiSampleMask { get; set; }

		#endregion

		public static readonly BlendState Additive = new BlendState("BlendState.Additive", 
		                                                            Blend.SourceAlpha,
		                                                            Blend.SourceAlpha,
		                                                            Blend.One,
		                                                            Blend.One);

		public static readonly BlendState AlphaBlend = new BlendState("BlendState.AlphaBlend", 
		                                                              Blend.One, 
		                                                              Blend.One,
		                                                              Blend.InverseSourceAlpha,
		                                                              Blend.InverseSourceAlpha);

		public static readonly BlendState NonPreMultiplied = new BlendState("BlendState.NonPreMultiplied",
		                                                                    Blend.SourceAlpha,
		                                                                    Blend.SourceAlpha,
		                                                                    Blend.InverseSourceAlpha,
		                                                                    Blend.InverseSourceAlpha);

		public static readonly BlendState Opaque = new BlendState("BlendState.Opaque",
		                                                          Blend.One,
		                                                          Blend.One,
		                                                          Blend.Zero,
		                                                          Blend.Zero);

		public BlendState(string name,
		                  Blend colorSourceBlend = Blend.One,
		                  Blend alphaSourceBlend = Blend.One,
		                  Blend colorDestinationBlend = Blend.Zero,
		                  Blend alphaDestinationBlend = Blend.Zero,
		                  BlendFunction colorBlendFunction = BlendFunction.Add,
		                  BlendFunction alphaBlendFunction = BlendFunction.Add,
		                  ColorWriteChannels colorWriteChannels = ColorWriteChannels.All,
		                  ColorWriteChannels colorWriteChannels1 = ColorWriteChannels.All,
		                  ColorWriteChannels colorWriteChannels2 = ColorWriteChannels.All,
		                  ColorWriteChannels colorWriteChannels3 = ColorWriteChannels.All)
		{
			Name = name;

			ColorSourceBlend = colorSourceBlend;
			AlphaSourceBlend = alphaSourceBlend;
			ColorDestinationBlend = colorDestinationBlend;
			AlphaDestinationBlend = alphaDestinationBlend;

			ColorBlendFunction = colorBlendFunction;
			AlphaBlendFunction = alphaBlendFunction;

			ColorWriteChannels = colorWriteChannels;
			ColorWriteChannels1 = colorWriteChannels1;
			ColorWriteChannels2 = colorWriteChannels2;
			ColorWriteChannels3 = colorWriteChannels3;

			BlendFactor = Color.White;

			MultiSampleMask = unchecked((int) 0xffffffff);
		}
	}
}