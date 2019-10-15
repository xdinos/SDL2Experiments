using System;
using Lunatics.Framework.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsDevice : IDisposable
	{
		#region Events
		
		public event EventHandler<EventArgs> DeviceReset;
		public event EventHandler<EventArgs> DeviceResetting;

		#endregion

		public GraphicsAdapter Adapter { get; protected set; }

		public PresentationParameters PresentationParameters { get; protected set; }

		public Rectangle ScissorRectangle
		{
			get => _scissorRectangle;
			set
			{
				_scissorRectangle = value;
				SetScissorRect(_scissorRectangle);
			}
		}
		public Viewport Viewport
		{
			get => _viewport;
			set
			{
				_viewport = value;
				SetViewport(_viewport);
			}
		}

		public abstract VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration,
		                                                int vertexCount,
		                                                BufferUsage bufferUsage);


		public void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
		{
			//DepthFormat dsFormat;
			//if (renderTargetCount == 0)
			//{
			//	/* FIXME: PresentationParameters.DepthStencilFormat is probably
			//	 * a more accurate value here, but the Backbuffer may disagree.
			//	 * -flibit
			//	 */
			//	dsFormat = GLDevice.Backbuffer.DepthFormat;
			//}
			//else
			//{
			//	dsFormat = (renderTargetBindings[0].RenderTarget as IRenderTarget).DepthStencilFormat;
			//}
			//if (dsFormat == DepthFormat.None)
			//{
			//	options &= ClearOptions.Target;
			//}
			//else if (dsFormat != DepthFormat.Depth24Stencil8)
			//{
			//	options &= ~ClearOptions.Stencil;
			//}
			ClearImpl(options, color, depth, stencil);
		}

		public void SetVertexBuffer(VertexBuffer vertexBuffer) => SetVertexBuffer(vertexBuffer, 0);
		public abstract void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset);
		public abstract void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount);

		//public abstract void DrawUserPrimitives<T>(PrimitiveType primitiveType,
		//                                           T[] vertexData,
		//                                           int vertexOffset,
		//                                           int primitiveCount) where T : struct, IVertexType;

		public abstract void DrawUserPrimitives<T>(PrimitiveType primitiveType,
		                                           T[] vertexData,
		                                           int vertexOffset,
		                                           int primitiveCount,
		                                           VertexDeclaration vertexDeclaration) where T : struct;

		public abstract void Present();

		public void Reset()
		{
			Reset(PresentationParameters, Adapter);
		}

		public void Reset(PresentationParameters presentationParameters)
		{
			Reset(presentationParameters, Adapter);
		}

		public abstract void Reset(PresentationParameters presentationParameters, GraphicsAdapter graphicsAdapter);

		protected void OnDeviceResetting()
		{
			DeviceResetting?.Invoke(this, EventArgs.Empty);
		}

		protected void OnDeviceReset()
		{
			DeviceReset?.Invoke(this, EventArgs.Empty);
		}

		protected abstract void SetViewport(Viewport viewport);

		protected abstract void SetScissorRect(Rectangle rectangle);

		#region Platform Implementation

		protected abstract void ClearImpl(ClearOptions options, Vector4 color, float depth, int stencil);

		#endregion

		#region IDisposable Implementation

		protected virtual void Dispose(bool disposing)
		{
			// TODO: release unmanaged resources here
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GraphicsDevice()
		{
			Dispose(false);
		}

		#endregion

		private Viewport _viewport;
		private Rectangle _scissorRectangle;
	}
}