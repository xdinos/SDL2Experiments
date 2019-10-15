using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework.Graphics
{
	public abstract class IndexBuffer : GraphicsResource
	{
		public BufferUsage BufferUsage { get; private set; }
		public int IndexCount { get; private set; }
		public IndexElementSize IndexElementSize { get; private set; }

		protected IndexBuffer(GraphicsDevice graphicsDevice, 
		                      Type indexType, 
		                      int indexCount, 
		                      BufferUsage usage, 
		                      bool dynamic)
			: this(graphicsDevice, 
			       SizeForType(graphicsDevice, indexType), 
			       indexCount, 
			       usage, 
			       dynamic)
		{
		}

		protected IndexBuffer(GraphicsDevice graphicsDevice, 
		                      IndexElementSize indexElementSize, 
		                      int indexCount, 
		                      BufferUsage usage, 
		                      bool dynamic)
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice),
			                                                                   "The GraphicsDevice must not be null when creating new resources.");
			IndexElementSize = indexElementSize;
			IndexCount = indexCount;
			BufferUsage = usage;

			_isDynamic = dynamic;
		}

		public abstract void SetData<T>(T[] data) where T : struct;

		public abstract void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct;

		public abstract void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct;

		private static IndexElementSize SizeForType(GraphicsDevice graphicsDevice, Type type)
		{
			switch (Marshal.SizeOf(type))
			{
				case 2:
					return IndexElementSize.SixteenBits;
				case 4:
					return IndexElementSize.ThirtyTwoBits;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), "Index buffers can only be created for type that are sixteen or thirty two bits in length.");
			}
		}

		private readonly bool _isDynamic;
	}

	//public abstract class DynamicIndexBuffer : IndexBuffer
	//{
	//	/// <summary>
	//	/// Special offset used internally by GraphicsDevice.DrawUserXXX() methods.
	//	/// </summary>
	//	internal int UserOffset;

	//	public bool IsContentLost => false;

	//	protected DynamicIndexBuffer(GraphicsDevice graphicsDevice, 
	//	                          IndexElementSize indexElementSize, 
	//	                          int indexCount, 
	//	                          BufferUsage usage) 
	//		: base(graphicsDevice, indexElementSize, indexCount, usage, true)
	//	{
	//	}

	//	protected DynamicIndexBuffer(GraphicsDevice graphicsDevice, 
	//	                          Type indexType, 
	//	                          int indexCount, 
	//	                          BufferUsage usage) 
	//		: base(graphicsDevice, indexType, indexCount, usage, true)
	//	{
	//	}

	//	public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, SetDataOptions options) where T : struct
	//	{
	//		base.SetDataInternal<T>(offsetInBytes, data, startIndex, elementCount, options);
	//	}

	//	public void SetData<T>(T[] data, int startIndex, int elementCount, SetDataOptions options) where T : struct
	//	{
	//		base.SetDataInternal<T>(0, data, startIndex, elementCount, options);
	//	}
	//}
}