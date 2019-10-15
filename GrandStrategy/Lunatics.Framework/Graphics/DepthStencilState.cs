namespace Lunatics.Framework.Graphics
{
	public class DepthStencilState
	{
		public static readonly DepthStencilState Default = new DepthStencilState("DepthStencilState.Default", true, true);
		public static readonly DepthStencilState DepthRead = new DepthStencilState("DepthStencilState.DepthRead", true, false);
		public static readonly DepthStencilState None = new DepthStencilState("DepthStencilState.None", false, false);

		#region Public Properties

		public string Name { get; }

		public bool DepthBufferEnable { get; }

		public bool DepthBufferWriteEnable { get; }

		public StencilOperation CounterClockwiseStencilDepthBufferFail { get; }

		public StencilOperation CounterClockwiseStencilFail { get; }

		public CompareFunction CounterClockwiseStencilFunction { get; }

		public StencilOperation CounterClockwiseStencilPass { get; }

		public CompareFunction DepthBufferFunction { get; }

		public int ReferenceStencil { get; }

		public StencilOperation StencilDepthBufferFail { get; }

		public bool StencilEnable { get; }
		public StencilOperation StencilFail { get; }
		public CompareFunction StencilFunction { get; }

		public int StencilMask { get; }

		public StencilOperation StencilPass { get; }

		public int StencilWriteMask { get; }

		public bool TwoSidedStencilMode { get; }

		#endregion

		public DepthStencilState(string name,
		                         bool depthBufferEnable = true,
		                         bool depthBufferWriteEnable = true,
		                         CompareFunction depthBufferFunction = CompareFunction.LessEqual,
		                         bool stencilEnable = false,
		                         CompareFunction stencilFunction = CompareFunction.Always,
		                         StencilOperation stencilPass = StencilOperation.Keep,
		                         StencilOperation stencilFail = StencilOperation.Keep,
		                         StencilOperation stencilDepthBufferFail = StencilOperation.Keep,
		                         bool twoSidedStencilMode = false,
		                         CompareFunction counterClockwiseStencilFunction = CompareFunction.Always,
		                         StencilOperation counterClockwiseStencilFail = StencilOperation.Keep,
		                         StencilOperation counterClockwiseStencilPass = StencilOperation.Keep,
		                         StencilOperation counterClockwiseStencilDepthBufferFail = StencilOperation.Keep,
		                         int stencilMask = int.MaxValue,
		                         int stencilWriteMask = int.MaxValue,
		                         int referenceStencil = 0)
		{
			Name = name;

			DepthBufferEnable = depthBufferEnable;
			DepthBufferWriteEnable = depthBufferWriteEnable;
			DepthBufferFunction = depthBufferFunction;

			StencilEnable = stencilEnable;
			StencilFunction = stencilFunction;
			StencilPass = stencilPass;
			StencilFail = stencilFail;
			StencilDepthBufferFail = stencilDepthBufferFail;
			TwoSidedStencilMode = twoSidedStencilMode;
			CounterClockwiseStencilFunction = counterClockwiseStencilFunction;
			CounterClockwiseStencilFail = counterClockwiseStencilFail;
			CounterClockwiseStencilPass = counterClockwiseStencilPass;
			CounterClockwiseStencilDepthBufferFail = counterClockwiseStencilDepthBufferFail;
			StencilMask = stencilMask;
			StencilWriteMask = stencilWriteMask;
			ReferenceStencil = referenceStencil;
		}
	}
}