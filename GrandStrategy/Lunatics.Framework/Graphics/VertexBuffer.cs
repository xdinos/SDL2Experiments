using System;
using System.Runtime.InteropServices;

namespace Lunatics.Framework.Graphics
{
	public abstract class VertexBuffer : GraphicsResource
	{
		public BufferUsage BufferUsage { get; private set; }

		public int VertexCount { get; private set; }

		public VertexDeclaration VertexDeclaration { get; private set; }

		protected VertexBuffer(GraphicsDevice graphicsDevice, 
		                       VertexDeclaration vertexDeclaration,
		                       int vertexCount,
		                       BufferUsage bufferUsage,
		                       bool dynamic)
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

			VertexDeclaration = vertexDeclaration;
			VertexCount = vertexCount;
			BufferUsage = bufferUsage;

			// Make sure the graphics device is assigned in the vertex declaration.
			if (vertexDeclaration.GraphicsDevice != graphicsDevice)
				vertexDeclaration.GraphicsDevice = graphicsDevice;
		}

		public void GetData<T>(T[] data) where T : struct
		{
			GetData(0, data, 0, data.Length, Marshal.SizeOf(typeof(T)));
		}

		public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
		{
			GetData(0, data, startIndex, elementCount, Marshal.SizeOf(typeof(T)));
		}

		public abstract void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
			where T : struct;
		
		public void SetData<T>(T[] data) where T : struct
		{
			SetData(0, data, 0, data.Length, Marshal.SizeOf(typeof(T)));
			//ErrorCheck(data, 0, data.Length, Marshal.SizeOf(typeof(T)));

			//GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			//GraphicsDevice.GLDevice.SetVertexBufferData(
			//	buffer,
			//	0,
			//	handle.AddrOfPinnedObject(),
			//	data.Length * Marshal.SizeOf(typeof(T)),
			//	SetDataOptions.None
			//);
			//handle.Free();
		}

		public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
		{
			SetData(0, data, startIndex, elementCount, Marshal.SizeOf(typeof(T)));
			//ErrorCheck(data, startIndex, elementCount, Marshal.SizeOf(typeof(T)));

			//GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			//GraphicsDevice.GLDevice.SetVertexBufferData(
			//	buffer,
			//	0,
			//	handle.AddrOfPinnedObject() + (startIndex * Marshal.SizeOf(typeof(T))),
			//	elementCount * Marshal.SizeOf(typeof(T)),
			//	SetDataOptions.None
			//);
			//handle.Free();
		}

		public abstract void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
			where T : struct;

		public abstract void SetDataPointer(int offsetInBytes, IntPtr data, int dataLength, SetDataOptions options);

		/// <summary>
		/// The GraphicsDevice is resetting, so GPU resources must be recreated.
		/// </summary>
		protected internal override void GraphicsDeviceResetting()
		{
			// FIXME: Do we even want to bother with DeviceResetting for GL? -flibit
		}
	}
}