namespace Lunatics.Framework.Graphics
{
	public class SamplerState
	{
		public static readonly SamplerState AnisotropicClamp = new SamplerState("SamplerState.AnisotropicClamp",
		                                                                        TextureFilter.Anisotropic,
		                                                                        TextureAddressMode.Clamp,
		                                                                        TextureAddressMode.Clamp,
		                                                                        TextureAddressMode.Clamp);

		public static readonly SamplerState AnisotropicWrap = new SamplerState("SamplerState.AnisotropicWrap",
		                                                                       TextureFilter.Anisotropic,
		                                                                       TextureAddressMode.Wrap,
		                                                                       TextureAddressMode.Wrap,
		                                                                       TextureAddressMode.Wrap);

		public static readonly SamplerState LinearClamp = new SamplerState("SamplerState.LinearClamp",
		                                                                   TextureFilter.Linear,
		                                                                   TextureAddressMode.Clamp,
		                                                                   TextureAddressMode.Clamp,
		                                                                   TextureAddressMode.Clamp);

		public static readonly SamplerState LinearWrap = new SamplerState("SamplerState.LinearWrap",
		                                                                  TextureFilter.Linear,
		                                                                  TextureAddressMode.Wrap,
		                                                                  TextureAddressMode.Wrap,
		                                                                  TextureAddressMode.Wrap);

		public static readonly SamplerState PointClamp = new SamplerState("SamplerState.PointClamp",
		                                                                  TextureFilter.Point,
		                                                                  TextureAddressMode.Clamp,
		                                                                  TextureAddressMode.Clamp,
		                                                                  TextureAddressMode.Clamp);

		public static readonly SamplerState PointWrap = new SamplerState("SamplerState.PointWrap",
		                                                                 TextureFilter.Point,
		                                                                 TextureAddressMode.Wrap,
		                                                                 TextureAddressMode.Wrap,
		                                                                 TextureAddressMode.Wrap);

		#region Public Properties

		public string Name { get; }

		public TextureAddressMode AddressU { get; }

		public TextureAddressMode AddressV { get; }

		public TextureAddressMode AddressW { get; }

		public TextureFilter Filter { get; }

		public int MaxAnisotropy { get; }

		public int MaxMipLevel { get; }

		public float MipMapLevelOfDetailBias { get; }

		#endregion

		public SamplerState(string name,
		                    TextureFilter filter = TextureFilter.Linear,
		                    TextureAddressMode addressU = TextureAddressMode.Wrap,
		                    TextureAddressMode addressV = TextureAddressMode.Wrap,
		                    TextureAddressMode addressW = TextureAddressMode.Wrap,
		                    int maxAnisotropy = 4,
		                    int maxMipLevel = 0,
		                    float mipMapLevelOfDetailBias = 0.0f)
		{
			Name = name;

			Filter = filter;
			AddressU = addressU;
			AddressV = addressV;
			AddressW = addressW;
			MaxAnisotropy = maxAnisotropy;
			MaxMipLevel = maxMipLevel;
			MipMapLevelOfDetailBias = mipMapLevelOfDetailBias;
		}
	}
}