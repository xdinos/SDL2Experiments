using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public struct VertexAttribute
	{
		public VertexElementUsage usage;
		public int index;
		public string name;
		public int location;
	}

	public abstract class Shader : GraphicsResource
	{
		public ShaderStage Stage { get; }

		public VertexAttribute[] Attributes { get; private set; }

		public abstract void SetMatrix4(string name, ref Matrix matrix);
		
		protected Shader(GraphicsDevice graphicsDevice, ShaderStage stage)
		{
			GraphicsDevice = graphicsDevice;
			Stage = stage;
		}
	}
}