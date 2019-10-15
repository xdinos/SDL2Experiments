using System;
using System.Collections.Generic;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public abstract class GraphicsDevice : IDisposable
	{
		public GraphicsAdapter Adapter { get; }

		public PresentationParameters PresentationParameters { get; }

		public BlendState BlendState { get; set; }

		public DepthStencilState DepthStencilState { get; set; }

		public RasterizerState RasterizerState { get; set; }

		public TextureCollection Textures { get; protected set; }

		public IndexBuffer Indices { get; set; }

		public Shader VertexShader { get; set; }
		public Shader PixelShader { get; set; }

		/* We have to store this internally because we flip the Viewport for
		 * when we aren't rendering to a target. I'd love to remove this.
		 * -flibit
		 */
		private Viewport INTERNAL_viewport;
		public Viewport Viewport
		{
			get => INTERNAL_viewport;
			set
			{
				INTERNAL_viewport = value;
				SetViewport(value);
			}
		}

		public abstract ISamplerStates SamplerStates { get; }

		public void SetVertexBuffer(VertexBuffer vertexBuffer) => SetVertexBuffer(vertexBuffer, 0);

		public abstract void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset);

		public VertexBuffer CreateVertexBuffer(Type type,
		                                       int vertexCount,
		                                       BufferUsage bufferUsage,
		                                       bool dynamic)
		{
			return CreateVertexBuffer(VertexDeclaration.FromType(type), vertexCount, bufferUsage, dynamic);
		}

		public abstract VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration,
		                                                int vertexCount,
		                                                BufferUsage bufferUsage,
		                                                bool dynamic);
		public abstract IndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize,
		                                              int indexCount,
		                                              BufferUsage usage,
		                                              bool dynamic);

		public abstract Shader CreateVertexShader(string vertexShaderCode);

		public abstract Shader CreatePixelShader(string pixelShaderCode);

		public abstract void Clear(ClearOptions options, Vector4 color, float depth, int stencil);

		public abstract void DrawPrimitives(PrimitiveType primitiveType,
		                                    int vertexStart,
		                                    int primitiveCount);

		public abstract void DrawIndexedPrimitives(PrimitiveType primitiveType,
		                                           int baseVertex,
		                                           int minVertexIndex,
		                                           int numVertices,
		                                           int startIndex,
		                                           int primitiveCount);

		public abstract void Present();

		internal Texture2D CreateTexture2D(int width, int height)
		{
			return CreateTexture2D(width, height, false, SurfaceFormat.Color);
		}

		protected GraphicsDevice(GraphicsAdapter adapter, PresentationParameters presentationParameters)
		{
			Adapter = adapter;
			PresentationParameters = presentationParameters ?? throw new ArgumentNullException(nameof(presentationParameters));
		}

		protected internal abstract Texture2D CreateTexture2D(int width, int height, bool mipMap, SurfaceFormat format);

		protected abstract void SetViewport(Viewport viewport);

		#region IDisposable

		public event EventHandler<EventArgs> Disposing;

		public bool IsDisposed { get; private set; }

		~GraphicsDevice()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed) return;

			if (disposing)
			{
				// We're about to dispose, notify the application.
				Disposing?.Invoke(this, EventArgs.Empty);

				// Dispose of all remaining graphics resources before disposing of the GraphicsDevice.
				lock (_resourcesLock)
				{
					foreach (var resource in _resources.ToArray())
					{
						var target = resource.Target;
						(target as IDisposable)?.Dispose();
					}
					_resources.Clear();
				}
			}

			IsDisposed = true;
		}

		#endregion
		
		internal void AddResourceReference(WeakReference resourceReference)
		{
			lock (_resourcesLock)
			{
				_resources.Add(resourceReference);
			}
		}
		internal void RemoveResourceReference(WeakReference resourceReference)
		{
			lock (_resourcesLock)
			{
				_resources.Remove(resourceReference);
			}
		}

		private readonly object _resourcesLock = new object();
		private readonly List<WeakReference> _resources = new List<WeakReference>();
	}
}