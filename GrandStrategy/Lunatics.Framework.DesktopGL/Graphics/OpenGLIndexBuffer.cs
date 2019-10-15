using System;
using System.Runtime.InteropServices;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	internal class OpenGLIndexBuffer : IndexBuffer
	{
		internal OpenGLBuffer Buffer { get; }

		internal OpenGLIndexBuffer(GraphicsDevice graphicsDevice,
		                           OpenGLBuffer buffer,
		                           IndexElementSize indexElementSize,
		                           int indexCount,
		                           BufferUsage usage,
		                           bool dynamic)
			: base(graphicsDevice, indexElementSize, indexCount, usage, dynamic)
		{
			Buffer = buffer;
		}

		public override void SetData<T>(T[] data)
		{
			SetData(0, data, 0, data.Length);
			//var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			//((OpenGLGraphicsDevice) GraphicsDevice).SetIndexBufferData(_buffer,
			//                                                           0,
			//                                                           handle.AddrOfPinnedObject(),
			//                                                           data.Length * Marshal.SizeOf(typeof(T)),
			//                                                           SetDataOptions.None);
			//handle.Free(); 
		}

		public override void SetData<T>(T[] data, int startIndex, int elementCount)
		{
			SetData(0, data, startIndex, elementCount);
			//ErrorCheck(data, startIndex, elementCount);
			//var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			//((OpenGLGraphicsDevice) GraphicsDevice).SetIndexBufferData(_buffer,
			//                                                           0,
			//                                                           handle.AddrOfPinnedObject() + startIndex * Marshal.SizeOf(typeof(T)),
			//                                                           elementCount * Marshal.SizeOf(typeof(T)),
			//                                                           SetDataOptions.None);
			//handle.Free();
		}

		public override void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount)
		{
			ErrorCheck(data, startIndex, elementCount);

			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			((OpenGLGraphicsDevice) GraphicsDevice).SetIndexBufferData(Buffer,
			                                                           offsetInBytes,
			                                                           handle.AddrOfPinnedObject() + startIndex * Marshal.SizeOf(typeof(T)),
			                                                           elementCount * Marshal.SizeOf(typeof(T)),
			                                                           SetDataOptions.None);
			handle.Free();
		}

		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				((OpenGLGraphicsDevice)GraphicsDevice).AddDisposeIndexBuffer(Buffer);
			}
			base.Dispose(disposing);
		}


		[System.Diagnostics.Conditional("DEBUG")]
		private void ErrorCheck<T>(T[] data, int startIndex, int elementCount) where T : struct
		{
			if (data == null) 
				throw new ArgumentNullException(nameof(data));
			

			if (data.Length < startIndex + elementCount)
				throw new InvalidOperationException("The array specified in the data parameter is not the correct size for the amount of data requested.");
			
		}
	}
}