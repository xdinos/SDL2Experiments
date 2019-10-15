using System;
using System.Runtime.InteropServices;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	internal class OpenGLVertexBuffer : VertexBuffer
	{
		internal OpenGLBuffer Buffer { get; }

		internal OpenGLVertexBuffer(GraphicsDevice graphicsDevice,
		                            OpenGLBuffer buffer,
		                            VertexDeclaration vertexDeclaration,
		                            int vertexCount,
		                            BufferUsage bufferUsage,
		                            bool dynamic)
			: base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage, dynamic)
		{
			Buffer = buffer;
		}

		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				((OpenGLGraphicsDevice) GraphicsDevice).AddDisposeVertexBuffer(Buffer);
			}

			base.Dispose(disposing);
		}

		public override void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (data.Length < (startIndex + elementCount))
				throw new ArgumentOutOfRangeException(nameof(elementCount),
				                                      "This parameter must be a valid index within the array.");

			if (BufferUsage == BufferUsage.WriteOnly)
				throw new NotSupportedException(
					"Calling GetData on a resource that was created with BufferUsage.WriteOnly is not supported.");

			var elementSizeInBytes = Marshal.SizeOf(typeof(T));
			if (vertexStride == 0)
			{
				vertexStride = elementSizeInBytes;
			}
			else if (vertexStride < elementSizeInBytes)
			{
				throw new ArgumentOutOfRangeException(nameof(vertexStride),
				                                      "The vertex stride is too small for the type of data requested. This is not allowed.");

			}

			if (elementCount > 1 && (elementCount * vertexStride) > (VertexCount * VertexDeclaration.VertexStride))
				throw new InvalidOperationException(
					"The array is not the correct size for the amount of data requested.");

			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			((OpenGLGraphicsDevice) GraphicsDevice).GetVertexBufferData(Buffer,
			                                                            offsetInBytes,
			                                                            handle.AddrOfPinnedObject(),
			                                                            startIndex,
			                                                            elementCount,
			                                                            elementSizeInBytes,
			                                                            vertexStride);
			handle.Free();
		}

		public override void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
		{
			ErrorCheck(data, startIndex, elementCount, vertexStride);

			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			SetDataPointer(offsetInBytes,
			               handle.AddrOfPinnedObject() + (startIndex * Marshal.SizeOf(typeof(T))),
			               elementCount * Marshal.SizeOf(typeof(T)),
			               SetDataOptions.None);
			handle.Free();
		}

		public override void SetDataPointer(int offsetInBytes, IntPtr data, int dataLength, SetDataOptions options)
		{
			((OpenGLGraphicsDevice)GraphicsDevice).SetVertexBufferData(Buffer, offsetInBytes, data, dataLength, options);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		private void ErrorCheck<T>(T[] data, int startIndex, int elementCount, int vertexStride) where T : struct
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			if ((startIndex + elementCount > data.Length) || elementCount <= 0)
			{
				throw new InvalidOperationException(
					"The array specified in the data parameter is not the correct size for the amount of data requested.");
			}

			if (elementCount > 1 && (elementCount * vertexStride > (int) Buffer.BufferSize))
				throw new InvalidOperationException("The vertex stride is larger than the vertex buffer.");

			var elementSizeInBytes = Marshal.SizeOf(typeof(T));
			if (vertexStride == 0)
				vertexStride = elementSizeInBytes;

			if (vertexStride < elementSizeInBytes)
			{
				throw new ArgumentOutOfRangeException(
					$"The vertex stride must be greater than or equal to the size of the specified data ({elementSizeInBytes}).");
			}
		}
	}
}