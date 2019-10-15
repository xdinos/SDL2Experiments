namespace Lunatics.Framework.Graphics
{
	public class RasterizerState
	{
		public static readonly RasterizerState CullClockwise = new RasterizerState("RasterizerState.CullClockwise", CullMode.CullClockwiseFace);
		public static readonly RasterizerState CullCounterClockwise = new RasterizerState("RasterizerState.CullCounterClockwise", CullMode.CullCounterClockwiseFace);
		public static readonly RasterizerState CullNone = new RasterizerState("RasterizerState.CullNone", CullMode.None);

		#region Public Properties

		public string Name { get; }

		public CullMode CullMode { get; }

		public float DepthBias { get; }

		public FillMode FillMode { get; }

		public bool MultiSampleAntiAlias { get; }

		public bool ScissorTestEnable { get; }

		public float SlopeScaleDepthBias { get; }

		#endregion

		public RasterizerState(string name,
		                       CullMode cullMode = CullMode.CullCounterClockwiseFace,
		                       FillMode fillMode = FillMode.Solid,
		                       float depthBias = 0f,
		                       bool multiSampleAntiAlias = true,
		                       bool scissorTestEnable = false,
		                       float slopeScaleDepthBias = 0f)
		{
			Name = name;

			CullMode = cullMode;
			FillMode = fillMode;
			DepthBias = depthBias;
			MultiSampleAntiAlias = multiSampleAntiAlias;
			ScissorTestEnable = scissorTestEnable;
			SlopeScaleDepthBias = slopeScaleDepthBias;
		}
	}
}