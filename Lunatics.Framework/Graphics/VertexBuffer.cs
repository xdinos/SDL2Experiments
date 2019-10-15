using System;

namespace Lunatics.Framework.Graphics
{
	public abstract class VertexBuffer : GraphicsResource
	{
		public BufferUsage BufferUsage { get; }

		public int VertexCount { get; }

		public VertexDeclaration VertexDeclaration { get; }

		public abstract void SetData<T>(T[] data) where T : struct;

		protected VertexBuffer(GraphicsDevice graphicsDevice,
		                       VertexDeclaration vertexDeclaration,
		                       int vertexCount,
		                       BufferUsage bufferUsage)
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
			VertexDeclaration = vertexDeclaration;
			VertexCount = vertexCount;
			BufferUsage = bufferUsage;
		}
	}
}