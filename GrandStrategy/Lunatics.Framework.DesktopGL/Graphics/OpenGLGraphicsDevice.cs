using System;
using System.Runtime.InteropServices;
using Lunatics.Framework.Graphics;
using Lunatics.Mathematics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	public sealed class OpenGLGraphicsDevice : GraphicsDevice
	{
		public override ISamplerStates SamplerStates { get; }

		internal IOpenGLBackBuffer BackBuffer { get; private set; }

		internal int MaxTextureSlots { get; }

		public OpenGLGraphicsDevice(Framework.Graphics.GraphicsAdapter adapter,
		                            PresentationParameters presentationParameters)
			: base(adapter, presentationParameters)
		{
			_glContext = Sdl.GL.CreateContext(presentationParameters.DeviceWindowHandle);

			Sdl.GL.GetAttribute(Sdl.GL.Attribute.DepthSize, out var depthSize);
			Sdl.GL.GetAttribute(Sdl.GL.Attribute.StencilSize, out var stencilSize);

			if (depthSize == 0 && stencilSize == 0)
			{
				_windowDepthFormat = DepthFormat.None;
			}
			else if (depthSize == 16 && stencilSize == 0)
			{
				_windowDepthFormat = DepthFormat.Depth16;
			}
			else if (depthSize == 24 && stencilSize == 0)
			{
				_windowDepthFormat = DepthFormat.Depth24;
			}
			else if (depthSize == 24 && stencilSize == 8)
			{
				_windowDepthFormat = DepthFormat.Depth24Stencil8;
			}
			else
			{
				throw new NotSupportedException("Unrecognized window depth/stencil format!");
			}

			OpenGL.GL.LoadEntryPoints();

			string renderer = OpenGL.GL.GetString(OpenGL.StringName.Renderer);
			string version = OpenGL.GL.GetString(OpenGL.StringName.Version);
			string vendor = OpenGL.GL.GetString(OpenGL.StringName.Vendor);
			string shadingLanguageVersion = OpenGL.GL.GetString(OpenGL.StringName.ShadingLanguageVersion);
			var extensions = OpenGL.GL.GetString(OpenGL.StringName.Extensions);

			BlendState = BlendState.Opaque;
			DepthStencilState = DepthStencilState.Default;
			RasterizerState = RasterizerState.CullCounterClockwise;

			BackBuffer = new NullBackBuffer(presentationParameters.BackBufferWidth,
			                                presentationParameters.BackBufferHeight,
			                                _windowDepthFormat);

			OpenGL.GL.GetInteger(OpenGL.GetPName.MaxTextureImageUnits, out var numSamplers);

			_textures = new OpenGLTexture[numSamplers];
			for (var i = 0; i < numSamplers; i += 1)
				_textures[i] = OpenGLTexture.NullTexture;
			MaxTextureSlots = numSamplers;

			var maxTextures = Math.Min(MaxTextureSlots, MaxTextureSamplers);
			var maxVertexTextures = MathUtils.Clamp(MaxTextureSlots - MaxTextureSamplers,
			                                        0, MaxVertexTextureSamplers);
			// TODO: vertexSamplerStart = MaxTextureSlots - maxVertexTextures;
			Textures = new TextureCollection(maxTextures, modifiedSamplers);
			SamplerStates = new SamplerStateCollection(maxTextures, modifiedSamplers);

			// Initialize vertex attribute state arrays
			int numAttributes;
			OpenGL.GL.GetInteger(OpenGL.GetPName.MaxVertexAttribs, out numAttributes);
			numAttributes = Math.Min(numAttributes, MaxVertexAttributes);
			attributes = new VertexAttribute[numAttributes];
			attributeEnabled = new bool[numAttributes];
			previousAttributeEnabled = new bool[numAttributes];
			attributeDivisor = new int[numAttributes];
			previousAttributeDivisor = new int[numAttributes];
			for (int i = 0; i < numAttributes; i += 1)
			{
				attributes[i] = new VertexAttribute();
				attributeEnabled[i] = false;
				previousAttributeEnabled[i] = false;
				attributeDivisor[i] = 0;
				previousAttributeDivisor[i] = 0;
			}

			// Initialize render target FBO and state arrays
			OpenGL.GL.GetInteger(OpenGL.GetPName.MaxDrawBuffers, out var numAttachments);
			numAttachments = Math.Min(numAttachments, MaxRenderTargetBindings);
			// TODO: attachments = new uint[numAttachments];
			// TODO: attachmentTypes = new GLenum[numAttachments];
			currentAttachments = new uint[numAttachments];
			// TODO: currentAttachmentTypes = new OpenGL.TextureTarget[numAttachments];
			// TODO: drawBuffersArray = Marshal.AllocHGlobal(sizeof(GLenum) * (numAttachments + 2));
			unsafe
			{
				// TODO: GLenum* dba = (GLenum*)drawBuffersArray;
				for (var i = 0; i < numAttachments; i += 1)
				{
					currentAttachments[i] = 0;
					// TODO: currentAttachmentTypes[i] = OpenGL.TextureTarget.Texture2D;
					// TODO: dba[i] = GLenum.GL_COLOR_ATTACHMENT0 + i;
				}

				// TODO: dba[numAttachments] = GLenum.GL_DEPTH_ATTACHMENT;
				// TODO: dba[numAttachments + 1] = GLenum.GL_STENCIL_ATTACHMENT;
			}

			Viewport = new Viewport(PresentationParameters.Bounds);
			//ScissorRectangle = Viewport.Bounds;

			_programCache = new ShaderProgramCache(this);
		}
		
		public override void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset)
		{
			//BindVertexBuffer(((OpenGLVertexBuffer)vertexBuffer).Buffer);

			if (_currentVertexBuffer != vertexBuffer)
			{
				_currentVertexBuffer = vertexBuffer;
				vertexBuffersUpdated = true;
			}

			//if (vertexBuffer == null)
			//{
			//	if (vertexBufferCount == 0)
			//		return;

			//	for (var i = 0; i < vertexBufferCount; i += 1)
			//		vertexBufferBindings[i] = VertexBufferBinding.None;

			//	vertexBufferCount = 0;
			//	vertexBuffersUpdated = true;
			//	return;
			//}

			//if (!ReferenceEquals(vertexBufferBindings[0].VertexBuffer, vertexBuffer) ||
			//	vertexBufferBindings[0].VertexOffset != vertexOffset)
			//{
			//	vertexBufferBindings[0] = new VertexBufferBinding(vertexBuffer, vertexOffset);
			//	vertexBuffersUpdated = true;
			//}

			//if (vertexBufferCount > 1)
			//{
			//	for (var i = 1; i < vertexBufferCount; i += 1)
			//		vertexBufferBindings[i] = VertexBufferBinding.None;
			//	vertexBuffersUpdated = true;
			//}

			//vertexBufferCount = 1;
		}

		public override VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration,
		                                                int vertexCount,
		                                                BufferUsage bufferUsage,
		                                                bool dynamic)
		{
			OpenGL.GL.GenBuffers(1, out var handle);
			var buffer = new OpenGLBuffer(handle,
			                              (IntPtr) (vertexDeclaration.VertexStride * vertexCount),
			                              dynamic
				                              ? OpenGL.BufferUsageHint.StreamDraw
				                              : OpenGL.BufferUsageHint.StaticDraw);
			BindVertexBuffer(buffer);

			OpenGL.GL.BufferData(OpenGL.BufferTarget.ArrayBuffer, 
			                     buffer.BufferSize, 
			                     IntPtr.Zero, 
			                     buffer.UsageHint);

			return new OpenGLVertexBuffer(this, buffer, vertexDeclaration, vertexCount, bufferUsage, dynamic);
		}

		public override IndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize,
		                                              int indexCount,
		                                              BufferUsage usage,
		                                              bool dynamic)
		{
			OpenGL.GL.GenBuffers(1, out int handle);

			var buffer = new OpenGLBuffer(handle,
			                              (IntPtr) (indexCount * IndexSize[(int) indexElementSize]),
			                              dynamic
				                              ? OpenGL.BufferUsageHint.StreamDraw
				                              : OpenGL.BufferUsageHint.StaticDraw);

			BindIndexBuffer(buffer);
			OpenGL.GL.BufferData(OpenGL.BufferTarget.ElementArrayBuffer,
			                     buffer.BufferSize,
			                     IntPtr.Zero,
			                     buffer.UsageHint);

			return new OpenGLIndexBuffer(this, buffer, indexElementSize, indexCount, usage, dynamic);
		}

		public override Shader CreateVertexShader(string vertexShaderCode)
		{
			var shader = new OpenGLShader(this, ShaderStage.Vertex, vertexShaderCode);
			shader.GetShaderHandle();
			return shader;
		}

		public override Shader CreatePixelShader(string pixelShaderCode)
		{
			var shader = new OpenGLShader(this, ShaderStage.Pixel, pixelShaderCode);
			shader.GetShaderHandle();
			return shader;
		}

		public override void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
		{
			// glClear depends on the scissor rectangle!
			if (_scissorTestEnable)
			{
				OpenGL.GL.Disable(OpenGL.EnableCap.ScissorTest);
			}

			var clearTarget = (options & ClearOptions.Target) == ClearOptions.Target;
			var clearDepth = (options & ClearOptions.DepthBuffer) == ClearOptions.DepthBuffer;
			var clearStencil = (options & ClearOptions.Stencil) == ClearOptions.Stencil;

			// Get the clear mask, set the clear properties if needed
			OpenGL.ClearBufferMask clearMask = 0;
			if (clearTarget)
			{
				clearMask |= OpenGL.ClearBufferMask.ColorBufferBit;
				if (!color.Equals(_clearColor))
				{
					OpenGL.GL.ClearColor(color.X, color.Y, color.Z, color.W);
					_clearColor = color;
				}

				// TODO: ???
				//// glClear depends on the color write mask!
				//if (colorWriteEnable != ColorWriteChannels.All)
				//{
				//	// FIXME: ColorWriteChannels1/2/3? -flibit
				//	GL.ColorMask(true, true, true, true);
				//}
			}

			if (clearDepth)
			{
				clearMask |= OpenGL.ClearBufferMask.DepthBufferBit;
				if (depth != _clearDepth)
				{
					OpenGL.GL.ClearDepth(depth);
					_clearDepth = depth;
				}

				// TODO: ???
				//// glClear depends on the depth write mask!
				//if (!zWriteEnable)
				//{
				//	GL.DepthMask(true);
				//}
			}

			if (clearStencil)
			{
				clearMask |= OpenGL.ClearBufferMask.StencilBufferBit;
				if (stencil != _clearStencil)
				{
					OpenGL.GL.ClearStencil(stencil);
					_clearStencil = stencil;
				}

				// TODO: ???
				//// glClear depends on the stencil write mask!
				//if (stencilWriteMask != -1)
				//{
				//	// AKA 0xFFFFFFFF, ugh -flibit
				//	GL.StencilMask(-1);
				//}
			}

			// CLEAR!
			OpenGL.GL.Clear(clearMask);

			// Clean up after ourselves.
			if (_scissorTestEnable)
			{
				OpenGL.GL.Enable(OpenGL.EnableCap.ScissorTest);
			}

			// TODO: ???
			//if (clearTarget && colorWriteEnable != ColorWriteChannels.All)
			//{
			//	// FIXME: ColorWriteChannels1/2/3? -flibit
			//	GL.ColorMask(
			//		(colorWriteEnable & ColorWriteChannels.Red) != 0,
			//		(colorWriteEnable & ColorWriteChannels.Blue) != 0,
			//		(colorWriteEnable & ColorWriteChannels.Green) != 0,
			//		(colorWriteEnable & ColorWriteChannels.Alpha) != 0
			//	);
			//}

			// TODO: ???
			//if (clearDepth && !zWriteEnable)
			//{
			//	GL.DepthMask(false);
			//}

			// TODO: ???
			//if (clearStencil && stencilWriteMask != -1) // AKA 0xFFFFFFFF, ugh -flibit
			//{
			//	GL.StencilMask(stencilWriteMask);
			//}
		}

		public override void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
		{
			ApplyState();

			// Set up the vertex buffers
			ApplyVertexAttributes(vertexBuffersUpdated, 0);
			//ApplyVertexAttributes(vertexBufferBindings, vertexBufferCount, vertexBuffersUpdated, 0);
			vertexBuffersUpdated = false;

			//var format = VertexPosition.VertexDeclaration.elements[0].VertexElementFormat;
			//OpenGL.GL.VertexAttribPointer(
			//	0,								// attribute 0. No particular reason for 0, but must match the layout in the shader.
			//	VertexAttribSize[(int)format],  // size
			//	VertexAttribType[(int)format],  // type
			//	false,							// normalized?
			//	0,								// stride
			//	IntPtr.Zero						// array buffer offset
			//);
			
			////if (OpenGL.GL.GetError() != OpenGL.ErrorCode.NoError)
			////	throw new Exception(OpenGL.GL.OnError);

			//OpenGL.GL.EnableVertexAttribArray(0);

			OpenGL.GL.DrawArrays(Primitive[(int) primitiveType],
			                     vertexStart,
			                     PrimitiveVerts(primitiveType, primitiveCount));

			//OpenGL.GL.DisableVertexAttribArray(0);
		}

		public override void DrawIndexedPrimitives(PrimitiveType primitiveType,
		                                           int baseVertex,
		                                           int minVertexIndex,
		                                           int numVertices,
		                                           int startIndex,
		                                           int primitiveCount)
		{
			ApplyState();

			// Set up the vertex buffers

			// TODO: ApplyVertexAttributes(vertexBufferBindings, vertexBufferCount, vertexBuffersUpdated, baseVertex);
			ApplyVertexAttributes(vertexBuffersUpdated, baseVertex);

			DrawIndexedPrimitives(primitiveType, 
			                      baseVertex, 
			                      minVertexIndex, 
			                      numVertices, 
			                      startIndex, 
			                      primitiveCount,
			                      Indices);
		}

		public override void Present()
		{
			Sdl.GL.SwapWindow(PresentationParameters.DeviceWindowHandle);
		}

		internal void GetVertexBufferData(OpenGLBuffer buffer,
		                                  int offsetInBytes,
		                                  IntPtr data,
		                                  int startIndex,
		                                  int elementCount,
		                                  int elementSizeInBytes,
		                                  int vertexStride)
		{
			IntPtr cpy;
			bool useStagingBuffer = elementSizeInBytes < vertexStride;
			if (useStagingBuffer)
			{
				cpy = Marshal.AllocHGlobal(elementCount * vertexStride);
			}
			else
			{
				cpy = data + (startIndex * elementSizeInBytes);
			}


			BindVertexBuffer(buffer);

			OpenGL.GL.GetBufferSubData(OpenGL.BufferTarget.ArrayBuffer,
			                           (IntPtr) offsetInBytes,
			                           (IntPtr) (elementCount * vertexStride),
			                           cpy);


			if (useStagingBuffer)
			{
				IntPtr src = cpy;
				IntPtr dst = data + (startIndex * elementSizeInBytes);
				for (int i = 0; i < elementCount; i += 1)
				{
					memcpy(dst, src, (IntPtr) elementSizeInBytes);
					dst += elementSizeInBytes;
					src += vertexStride;
				}

				Marshal.FreeHGlobal(cpy);
			}
		}

		internal void GetIndexBufferData(OpenGLBuffer buffer,
		                                 int offsetInBytes,
		                                 IntPtr data,
		                                 int startIndex,
		                                 int elementCount,
		                                 int elementSizeInBytes)
		{

			BindIndexBuffer(buffer);

			OpenGL.GL.GetBufferSubData(OpenGL.BufferTarget.ElementArrayBuffer,
			                           (IntPtr) offsetInBytes,
			                           (IntPtr) (elementCount * elementSizeInBytes),
			                           data + (startIndex * elementSizeInBytes));
		}

		internal void SetVertexBufferData(OpenGLBuffer buffer,
		                                  int offsetInBytes,
		                                  IntPtr data,
		                                  int dataLength,
		                                  SetDataOptions options)
		{
			BindVertexBuffer(buffer);

			if (options == SetDataOptions.Discard)
			{
				OpenGL.GL.BufferData(OpenGL.BufferTarget.ArrayBuffer,
				                     buffer.BufferSize,
				                     IntPtr.Zero,
				                     buffer.UsageHint);
			}

			OpenGL.GL.BufferSubData(OpenGL.BufferTarget.ArrayBuffer,
			                        (IntPtr) offsetInBytes,
			                        (IntPtr) dataLength,
			                        data);
		}

		internal void SetIndexBufferData(OpenGLBuffer buffer,
		                                 int offsetInBytes,
		                                 IntPtr data,
		                                 int dataLength,
		                                 SetDataOptions options)
		{
			BindIndexBuffer(buffer);

			if (options == SetDataOptions.Discard)
			{
				OpenGL.GL.BufferData(OpenGL.BufferTarget.ElementArrayBuffer,
				                     buffer.BufferSize,
				                     IntPtr.Zero,
				                     buffer.UsageHint);
			}

			OpenGL.GL.BufferSubData(OpenGL.BufferTarget.ElementArrayBuffer,
			                        (IntPtr) offsetInBytes,
			                        (IntPtr) dataLength,
			                        data);
		}

		internal void AddDisposeVertexBuffer(OpenGLBuffer buffer)
		{
			// TODO: ...
			//if (IsOnMainThread())
			//{
			DeleteVertexBuffer(buffer);
			//}
			//else
			//{
			//	GCVertexBuffers.Enqueue(buffer);
			//}
		}

		internal void AddDisposeIndexBuffer(OpenGLBuffer buffer)
		{
			// TODO: ...
			//if (IsOnMainThread())
			//{
			DeleteIndexBuffer(buffer);
			//}
			//else
			//{
			//	GCIndexBuffers.Enqueue(buffer);
			//}
		}

		internal void SetTextureData2D(OpenGLTexture texture,
		                               SurfaceFormat format,
		                               int x,
		                               int y,
		                               int w,
		                               int h,
		                               int level,
		                               IntPtr data,
		                               int dataLength)
		{
			BindTexture(texture);

			var glFormat = TextureFormat[(int) format];
			if (glFormat == OpenGL.PixelFormat.CompressedTextureFormats)
			{
				/* Note that we're using glInternalFormat, not glFormat.
				 * In this case, they should actually be the same thing,
				 * but we use glFormat somewhat differently for
				 * compressed textures.
				 * -flibit
				 */
				OpenGL.GL.CompressedTexSubImage2D(OpenGL.TextureTarget.Texture2D,
				                                  level,
				                                  x,
				                                  y,
				                                  w,
				                                  h,
				                                  TextureInternalFormat[(int) format],
				                                  dataLength,
				                                  data);
			}
			else
			{
				// Set pixel alignment to match texel size in bytes.
				int packSize = Texture.GetPixelStoreAlignment(format);
				if (packSize != 4)
				{
					OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.UnpackAlignment, packSize);
				}

				OpenGL.GL.TexSubImage2D(OpenGL.TextureTarget.Texture2D,
				                        level,
				                        x,
				                        y,
				                        w,
				                        h,
				                        glFormat,
				                        TextureDataType[(int) format],
				                        data);

				// Keep this state sane -flibit
				if (packSize != 4)
					OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.UnpackAlignment, 4);
			}
		}

		internal void DeleteTexture(OpenGLTexture texture)
		{
			var handle = texture.Handle;
			for (var i = 0; i < currentAttachments.Length; i += 1)
			{
				if (handle == currentAttachments[i])
				{
					// Force an attachment update, this no longer exists!
					currentAttachments[i] = uint.MaxValue;
				}
			}

			OpenGL.GL.DeleteTextures(1, ref handle);
		}

		protected override Texture2D CreateTexture2D(int width, int height, bool mipMap, SurfaceFormat format)
		{
			var levelCount = /*mipMap ? CalculateMipLevels(width, height) :*/ 1;
			var texture = CreateTexture(OpenGL.TextureTarget.Texture2D, levelCount);

			var glFormat = TextureFormat[(int) format];
			var glInternalFormat = TextureInternalFormat[(int) format];
			if (glFormat == OpenGL.PixelFormat.CompressedTextureFormats)
			{
				for (int i = 0; i < levelCount; i += 1)
				{
					int levelWidth = Math.Max(width >> i, 1);
					int levelHeight = Math.Max(height >> i, 1);
					OpenGL.GL.CompressedTexImage2D(OpenGL.TextureTarget.Texture2D,
					                               i,
					                               glInternalFormat,
					                               levelWidth,
					                               levelHeight,
					                               0,
					                               ((levelWidth + 3) / 4) * ((levelHeight + 3) / 4) *
					                               Texture.GetFormatSize(format),
					                               IntPtr.Zero);
				}
			}
			else
			{
				var glType = TextureDataType[(int) format];
				for (int i = 0; i < levelCount; i += 1)
				{
					OpenGL.GL.TexImage2D(OpenGL.TextureTarget.Texture2D,
					                     i,
					                     glInternalFormat,
					                     Math.Max(width >> i, 1),
					                     Math.Max(height >> i, 1),
					                     0,
					                     glFormat,
					                     glType,
					                     IntPtr.Zero);
				}
			}

			return new OpenGLTexture2D(this, width, height, levelCount, format, texture);
		}

		protected override void SetViewport(Viewport vp)
		{
			// Flip viewport when target is not bound
			if (!renderTargetBound)
			{
				vp.Y = BackBuffer.Height - vp.Y - vp.Height;
			}

			if (vp.Bounds != viewport)
			{
				viewport = vp.Bounds;
				OpenGL.GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
			}

			if (vp.MinDepth != depthRangeMin || vp.MaxDepth != depthRangeMax)
			{
				depthRangeMin = vp.MinDepth;
				depthRangeMax = vp.MaxDepth;
				OpenGL.GL.DepthRange((double) depthRangeMin, (double) depthRangeMax);
			}
		}

		private float depthRangeMin = 0.0f;
		private float depthRangeMax = 1.0f;
		private Rectangle viewport = new Rectangle(0, 0, 0, 0);


		protected override void Dispose(bool disposing)
		{
			if (IsDisposed) return;

			if (disposing)
			{
				Sdl.GL.DeleteContext(_glContext);
			}

			base.Dispose(disposing);
		}

		private void ApplyState()
		{
			if (_currentBlendState != BlendState)
			{
				SetBlendState(BlendState);
				_currentBlendState = BlendState;
			}

			if (_currentDepthStencilState != DepthStencilState)
			{
				SetDepthStencilState(DepthStencilState);
				_currentDepthStencilState = DepthStencilState;
			}

			// Always update RasterizerState, as it depends on other device states
			ApplyRasterizerState(RasterizerState);

			if (VertexShader != null && PixelShader != null)
			{
				ActivateShaderProgram();
			}
		}

		private void SetBlendState(BlendState blendState)
		{
			var blendEnabled = !(blendState.ColorSourceBlend == Blend.One &&
			                     blendState.ColorDestinationBlend == Blend.Zero &&
			                     blendState.AlphaSourceBlend == Blend.One &&
			                     blendState.AlphaDestinationBlend == Blend.Zero);

			if (blendEnabled != _alphaBlendEnable)
			{
				ToggleGLState(OpenGL.EnableCap.Blend, blendEnabled);
				_alphaBlendEnable = blendEnabled;
			}

			if (_alphaBlendEnable)
			{
				if (blendState.BlendFactor != _blendFactor)
				{
					OpenGL.GL.BlendColor(blendState.BlendFactor.R / 255.0f,
					                     blendState.BlendFactor.G / 255.0f,
					                     blendState.BlendFactor.B / 255.0f,
					                     blendState.BlendFactor.A / 255.0f);
					_blendFactor = blendState.BlendFactor;
				}

				if (blendState.ColorSourceBlend != _colorSourceBlend ||
				    blendState.ColorDestinationBlend != _colorDestinationBlend ||
				    blendState.AlphaSourceBlend != _alphaSourceBlend ||
				    blendState.AlphaDestinationBlend != _alphaDestinationBlend)
				{
					OpenGL.GL.BlendFuncSeparate(BlendSrcMode[(int) blendState.ColorSourceBlend],
					                            BlendDstMode[(int) blendState.ColorDestinationBlend],
					                            BlendSrcMode[(int) blendState.AlphaSourceBlend],
					                            BlendDstMode[(int) blendState.AlphaDestinationBlend]);

					_colorSourceBlend = blendState.ColorSourceBlend;
					_colorDestinationBlend = blendState.ColorDestinationBlend;
					_alphaSourceBlend = blendState.AlphaSourceBlend;
					_alphaDestinationBlend = blendState.AlphaDestinationBlend;
				}

				if (blendState.ColorBlendFunction != _colorBlendFunction ||
				    blendState.AlphaBlendFunction != _alphaBlendFunction)
				{
					OpenGL.GL.BlendEquationSeparate(BlendEquation[(int) blendState.ColorBlendFunction],
					                                BlendEquation[(int) blendState.AlphaBlendFunction]);

					_colorBlendFunction = blendState.ColorBlendFunction;
					_alphaBlendFunction = blendState.AlphaBlendFunction;
				}
			}

			if (blendState.ColorWriteChannels != _colorWriteEnable)
			{
				_colorWriteEnable = blendState.ColorWriteChannels;
				OpenGL.GL.ColorMask((_colorWriteEnable & ColorWriteChannels.Red) != 0,
				                    (_colorWriteEnable & ColorWriteChannels.Green) != 0,
				                    (_colorWriteEnable & ColorWriteChannels.Blue) != 0,
				                    (_colorWriteEnable & ColorWriteChannels.Alpha) != 0);
			}

			/* FIXME: So how exactly do we factor in
			 * COLORWRITEENABLE for buffer 0? Do we just assume that
			 * the default is just buffer 0, and all other calls
			 * update the other write masks afterward? Or do we
			 * assume that COLORWRITEENABLE only touches 0, and the
			 * other 3 buffers are left alone unless we don't have
			 * EXT_draw_buffers2?
			 * -flibit
			 */
			if (blendState.ColorWriteChannels1 != colorWriteEnable1)
			{
				colorWriteEnable1 = blendState.ColorWriteChannels1;
				OpenGL.GL.ColorMaskI(1,
				                     (colorWriteEnable1 & ColorWriteChannels.Red) != 0,
				                     (colorWriteEnable1 & ColorWriteChannels.Green) != 0,
				                     (colorWriteEnable1 & ColorWriteChannels.Blue) != 0,
				                     (colorWriteEnable1 & ColorWriteChannels.Alpha) != 0);
			}

			if (blendState.ColorWriteChannels2 != colorWriteEnable2)
			{
				colorWriteEnable2 = blendState.ColorWriteChannels2;
				OpenGL.GL.ColorMaskI(2,
				                     (colorWriteEnable2 & ColorWriteChannels.Red) != 0,
				                     (colorWriteEnable2 & ColorWriteChannels.Green) != 0,
				                     (colorWriteEnable2 & ColorWriteChannels.Blue) != 0,
				                     (colorWriteEnable2 & ColorWriteChannels.Alpha) != 0);
			}

			if (blendState.ColorWriteChannels3 != colorWriteEnable3)
			{
				colorWriteEnable3 = blendState.ColorWriteChannels3;
				OpenGL.GL.ColorMaskI(3,
				                     (colorWriteEnable3 & ColorWriteChannels.Red) != 0,
				                     (colorWriteEnable3 & ColorWriteChannels.Green) != 0,
				                     (colorWriteEnable3 & ColorWriteChannels.Blue) != 0,
				                     (colorWriteEnable3 & ColorWriteChannels.Alpha) != 0);
			}

			if (blendState.MultiSampleMask != multisampleMask)
			{
				if (blendState.MultiSampleMask == -1)
				{
					OpenGL.GL.Disable(OpenGL.EnableCap.SampleMask);
				}
				else
				{
					if (multisampleMask == -1)
					{
						OpenGL.GL.Enable(OpenGL.EnableCap.SampleMask);
					}

					// FIXME: index...? -flibit
					OpenGL.GL.SampleMaskI(0, (uint) blendState.MultiSampleMask);
				}

				multisampleMask = blendState.MultiSampleMask;
			}
		}

		private void SetDepthStencilState(DepthStencilState depthStencilState)
		{
			if (depthStencilState.DepthBufferEnable != _zEnable)
			{
				_zEnable = depthStencilState.DepthBufferEnable;
				ToggleGLState(OpenGL.EnableCap.DepthTest, _zEnable);
			}

			if (_zEnable)
			{
				if (depthStencilState.DepthBufferWriteEnable != _zWriteEnable)
				{
					_zWriteEnable = depthStencilState.DepthBufferWriteEnable;
					OpenGL.GL.DepthMask(_zWriteEnable);
				}

				if (depthStencilState.DepthBufferFunction != _depthFunc)
				{
					_depthFunc = depthStencilState.DepthBufferFunction;
					OpenGL.GL.DepthFunc(DepthFunc[(int) _depthFunc]);
				}
			}

			if (depthStencilState.StencilEnable != stencilEnable)
			{
				stencilEnable = depthStencilState.StencilEnable;
				ToggleGLState(OpenGL.EnableCap.StencilTest, stencilEnable);
			}

			if (stencilEnable)
			{
				if (depthStencilState.StencilWriteMask != stencilWriteMask)
				{
					OpenGL.GL.StencilMask(depthStencilState.StencilWriteMask);
					stencilWriteMask = depthStencilState.StencilWriteMask;
				}

				// TODO: Can we split StencilFunc/StencilOp up nicely? -flibit
				if (depthStencilState.TwoSidedStencilMode != separateStencilEnable ||
				    depthStencilState.ReferenceStencil != stencilRef ||
				    depthStencilState.StencilMask != stencilMask ||
				    depthStencilState.StencilFunction != stencilFunc ||
				    depthStencilState.CounterClockwiseStencilFunction != ccwStencilFunc ||
				    depthStencilState.StencilFail != stencilFail ||
				    depthStencilState.StencilDepthBufferFail != stencilZFail ||
				    depthStencilState.StencilPass != stencilPass ||
				    depthStencilState.CounterClockwiseStencilFail != ccwStencilFail ||
				    depthStencilState.CounterClockwiseStencilDepthBufferFail != ccwStencilZFail ||
				    depthStencilState.CounterClockwiseStencilPass != ccwStencilPass)
				{
					separateStencilEnable = depthStencilState.TwoSidedStencilMode;
					stencilRef = depthStencilState.ReferenceStencil;
					stencilMask = depthStencilState.StencilMask;
					stencilFunc = depthStencilState.StencilFunction;
					stencilFail = depthStencilState.StencilFail;
					stencilZFail = depthStencilState.StencilDepthBufferFail;
					stencilPass = depthStencilState.StencilPass;
					if (separateStencilEnable)
					{
						ccwStencilFunc = depthStencilState.CounterClockwiseStencilFunction;
						ccwStencilFail = depthStencilState.CounterClockwiseStencilFail;
						ccwStencilZFail = depthStencilState.CounterClockwiseStencilDepthBufferFail;
						ccwStencilPass = depthStencilState.CounterClockwiseStencilPass;
						OpenGL.GL.StencilFuncSeparate(OpenGL.StencilFace.Front,
						                              StencilFunc[(int) stencilFunc],
						                              stencilRef,
						                              stencilMask);
						OpenGL.GL.StencilFuncSeparate(OpenGL.StencilFace.Back,
						                              StencilFunc[(int) ccwStencilFunc],
						                              stencilRef,
						                              stencilMask);
						OpenGL.GL.StencilOpSeparate(OpenGL.StencilFace.Front,
						                            StencilOp[(int) stencilFail],
						                            StencilOp[(int) stencilZFail],
						                            StencilOp[(int) stencilPass]);
						OpenGL.GL.StencilOpSeparate(OpenGL.StencilFace.Back,
						                            StencilOp[(int) ccwStencilFail],
						                            StencilOp[(int) ccwStencilZFail],
						                            StencilOp[(int) ccwStencilPass]);
					}
					else
					{
						OpenGL.GL.StencilFunc(StencilFunc[(int) stencilFunc],
						                      stencilRef,
						                      stencilMask);
						OpenGL.GL.StencilOp(StencilOp[(int) stencilFail],
						                    StencilOp[(int) stencilZFail],
						                    StencilOp[(int) stencilPass]);
					}
				}
			}
		}

		private void ApplyRasterizerState(RasterizerState rasterizerState)
		{
			if (rasterizerState.ScissorTestEnable != _scissorTestEnable)
			{
				_scissorTestEnable = rasterizerState.ScissorTestEnable;
				ToggleGLState(OpenGL.EnableCap.ScissorTest, _scissorTestEnable);
			}

			CullMode actualMode;
			if (renderTargetBound)
			{
				actualMode = rasterizerState.CullMode;
			}
			else
			{
				// When not rendering offscreen the faces change order.
				if (rasterizerState.CullMode == CullMode.None)
				{
					actualMode = rasterizerState.CullMode;
				}
				else
				{
					actualMode = rasterizerState.CullMode == CullMode.CullClockwiseFace
						             ? CullMode.CullCounterClockwiseFace
						             : CullMode.CullClockwiseFace;
				}
			}

			if (actualMode != cullFrontFace)
			{
				if ((actualMode == CullMode.None) != (cullFrontFace == CullMode.None))
				{
					ToggleGLState(OpenGL.EnableCap.CullFace, actualMode != CullMode.None);
				}

				cullFrontFace = actualMode;
				if (cullFrontFace != CullMode.None)
				{
					OpenGL.GL.FrontFace(FrontFace[(int)cullFrontFace]);
				}
			}

			if (rasterizerState.FillMode != fillMode)
			{
				fillMode = rasterizerState.FillMode;
				OpenGL.GL.PolygonMode(OpenGL.MaterialFace.FrontAndBack, PolygonMode[(int) fillMode]);
			}

			// FIXME: Floating point equality comparisons used for speed -flibit
			float realDepthBias = rasterizerState.DepthBias *
			                      DepthBiasScale[renderTargetBound
				                                     ? (int) currentDepthStencilFormat
				                                     : (int) /*Backbuffer.DepthFormat*/_windowDepthFormat];
			if (realDepthBias != depthBias ||
			    rasterizerState.SlopeScaleDepthBias != slopeScaleDepthBias)
			{
				if (realDepthBias == 0.0f &&
				    rasterizerState.SlopeScaleDepthBias == 0.0f)
				{
					// We're changing to disabled bias, disable!
					OpenGL.GL.Disable(OpenGL.EnableCap.PolygonOffsetFill);
				}
				else
				{
					if (depthBias == 0.0f && slopeScaleDepthBias == 0.0f)
					{
						// We're changing away from disabled bias, enable!
						OpenGL.GL.Enable(OpenGL.EnableCap.PolygonOffsetFill);
					}

					OpenGL.GL.PolygonOffset(rasterizerState.SlopeScaleDepthBias, realDepthBias);
				}

				depthBias = realDepthBias;
				slopeScaleDepthBias = rasterizerState.SlopeScaleDepthBias;
			}

			/* If you're reading this, you have a user with broken MSAA!
			 * Here's the deal: On all modern drivers this should work,
			 * but there was a period of time where, for some reason,
			 * IHVs all took a nap and decided that they didn't have to
			 * respect GL_MULTISAMPLE toggles. A couple sources:
			 *
			 * https://developer.apple.com/library/content/documentation/GraphicsImaging/Conceptual/OpenGL-MacProgGuide/opengl_fsaa/opengl_fsaa.html
			 *
			 * https://www.opengl.org/discussion_boards/showthread.php/172025-glDisable(GL_MULTISAMPLE)-has-no-effect
			 *
			 * So yeah. Have em update their driver. If they're on Intel,
			 * tell them to install Linux. Yes, really.
			 * -flibit
			 */
			if (rasterizerState.MultiSampleAntiAlias != multiSampleEnable)
			{
				multiSampleEnable = rasterizerState.MultiSampleAntiAlias;
				ToggleGLState(OpenGL.EnableCap.Multisample, multiSampleEnable);
			}
		}

		private VertexBuffer _currentVertexBuffer;

		private void ApplyVertexAttributes(bool bindingsUpdated, int baseVertex)
		{
			if (bindingsUpdated)
			{
				BindVertexBuffer(((OpenGLVertexBuffer)_currentVertexBuffer).Buffer);
				VertexDeclaration vertexDeclaration = _currentVertexBuffer.VertexDeclaration;
				var basePtr = (IntPtr)(vertexDeclaration.VertexStride * (/*bindings[i].VertexOffset*/0 + baseVertex));
				
				var attribLoc = 0;
				foreach (VertexElement element in vertexDeclaration.elements)
				{

					attributeEnabled[attribLoc] = true;
					var ptr = basePtr + element.Offset;
					var format = element.VertexElementFormat;
					var normalized = VertexAttribNormalized(element);
					OpenGL.GL.VertexAttribPointer(attribLoc,
					                              VertexAttribSize[(int) format],
					                              VertexAttribType[(int) format],
					                              normalized,
					                              vertexDeclaration.VertexStride,
					                              ptr);
					// ???
					attribLoc++;
				}

				FlushGLVertexAttributes();
			}
		}

		private void FlushGLVertexAttributes()
		{
			for (int i = 0; i < attributes.Length; i += 1)
			{
				if (attributeEnabled[i])
				{
					attributeEnabled[i] = false;
					if (!previousAttributeEnabled[i])
					{
						OpenGL.GL.EnableVertexAttribArray(i);
						previousAttributeEnabled[i] = true;
					}
				}
				else if (previousAttributeEnabled[i])
				{
					OpenGL.GL.DisableVertexAttribArray(i);
					previousAttributeEnabled[i] = false;
				}

				// TODO: ???
				//int divisor = attributeDivisor[i];
				//if (divisor != previousAttributeDivisor[i])
				//{
				//	OpenGL.GL.VertexAttribDivisor(i, divisor);
				//	previousAttributeDivisor[i] = divisor;
				//}
			}
		}

		//private void ApplyVertexAttributes(VertexBufferBinding[] bindings,
		//                                   int numBindings,
		//                                   bool bindingsUpdated,
		//                                   int baseVertex)
		//{
		//	if (OpenGL.GL.SupportsBaseVertex)
		//		baseVertex = 0;

		//	if (bindingsUpdated ||
		//		baseVertex != ldBaseVertex ||
		//		currentEffect != ldEffect ||
		//		currentTechnique != ldTechnique ||
		//		currentPass != ldPass ||
		//		effectApplied)
		//	{
		//		/* There's this weird case where you can have overlapping
		//		 * vertex usage/index combinations. It seems like the first
		//		 * attrib gets priority, so whenever a duplicate attribute
		//		 * exists, give it the next available index. If that fails, we
		//		 * have to crash :/
		//		 * -flibit
		//		 */
		//		Array.Clear(attrUse, 0, attrUse.Length);
		//		for (int i = 0; i < numBindings; i += 1)
		//		{
		//			BindVertexBuffer(bindings[i].VertexBuffer.buffer);
		//			VertexDeclaration vertexDeclaration = bindings[i].VertexBuffer.VertexDeclaration;
		//			IntPtr basePtr = (IntPtr)(vertexDeclaration.VertexStride * (bindings[i].VertexOffset + baseVertex));
		//			foreach (VertexElement element in vertexDeclaration.elements)
		//			{
		//				int usage = (int)element.VertexElementUsage;
		//				int index = element.UsageIndex;
		//				if (attrUse[usage, index])
		//				{
		//					index = -1;
		//					for (int j = 0; j < 10; j += 1)
		//					{
		//						if (!attrUse[usage, j])
		//						{
		//							index = j;
		//							break;
		//						}
		//					}

		//					if (index < 0)
		//					{
		//						throw new InvalidOperationException("Vertex usage collision!");
		//					}
		//				}

		//				attrUse[usage, index] = true;
		//				int attribLoc = MojoShader.MOJOSHADER_glGetVertexAttribLocation(VertexAttribUsage[usage], index);
		//				if (attribLoc == -1)
		//				{
		//					// Stream not in use!
		//					continue;
		//				}

		//				attributeEnabled[attribLoc] = true;
		//				VertexAttribute attr = attributes[attribLoc];
		//				var buffer = (bindings[i].VertexBuffer.buffer as OpenGLBuffer).Handle;
		//				IntPtr ptr = basePtr + element.Offset;
		//				VertexElementFormat format = element.VertexElementFormat;
		//				bool normalized = VertexAttribNormalized(element);
		//				if (attr.CurrentBuffer != buffer ||
		//					attr.CurrentPointer != ptr ||
		//					attr.CurrentFormat != element.VertexElementFormat ||
		//					attr.CurrentNormalized != normalized ||
		//					attr.CurrentStride != vertexDeclaration.VertexStride)
		//				{
		//					OpenGL.GL.VertexAttribPointer(attribLoc,
		//												  VertexAttribSize[(int)format],
		//												  VertexAttribType[(int)format],
		//												  normalized,
		//												  vertexDeclaration.VertexStride,
		//												  ptr);
		//					attr.CurrentBuffer = buffer;
		//					attr.CurrentPointer = ptr;
		//					attr.CurrentFormat = format;
		//					attr.CurrentNormalized = normalized;
		//					attr.CurrentStride = vertexDeclaration.VertexStride;
		//				}

		//				if (SupportsHardwareInstancing)
		//				{
		//					attributeDivisor[attribLoc] = bindings[i].InstanceFrequency;
		//				}
		//			}
		//		}

		//		FlushGLVertexAttributes();

		//		ldBaseVertex = baseVertex;
		//		ldEffect = currentEffect;
		//		ldTechnique = currentTechnique;
		//		ldPass = currentPass;
		//		effectApplied = false;
		//		ldVertexDeclaration = null;
		//		ldPointer = IntPtr.Zero;
		//	}

		//	MojoShader.MOJOSHADER_glProgramReady();
		//	MojoShader.MOJOSHADER_glProgramViewportInfo(viewport.Width, viewport.Height,
		//												BackBuffer.Width, BackBuffer.Height,
		//												renderTargetBound ? 1 : 0);
		//}

		public void DrawIndexedPrimitives(PrimitiveType primitiveType,
		                                  int baseVertex,
		                                  int minVertexIndex,
		                                  int numVertices,
		                                  int startIndex,
		                                  int primitiveCount,
		                                  IndexBuffer indices
		)
		{
			// Bind the index buffer
			BindIndexBuffer(((OpenGLIndexBuffer) indices).Buffer);

			// Draw!
			OpenGL.GL.DrawRangeElementsBaseVertex(Primitive[(int) primitiveType],
			                                      minVertexIndex,
			                                      minVertexIndex + numVertices - 1,
			                                      PrimitiveVerts(primitiveType, primitiveCount),
			                                      IndexType[(int) indices.IndexElementSize],
			                                      (IntPtr) (startIndex * IndexSize[(int) indices.IndexElementSize]),
			                                      baseVertex);
		}

		private OpenGLTexture CreateTexture(OpenGL.TextureTarget target, int levelCount)
		{
			OpenGL.GL.GenTextures(1, out var handle);
			var result = new OpenGLTexture(handle, target, levelCount);

			BindTexture(result);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureWrapS, Wrap[(int) result.WrapS]);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureWrapT, Wrap[(int) result.WrapT]);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureWrapR, Wrap[(int) result.WrapR]);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureMagFilter,
			                       MagFilter[(int) result.Filter]);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureMinFilter,
			                       result.HasMipMaps
				                       ? MinMipFilter[(int) result.Filter]
				                       : MinFilter[(int) result.Filter]);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureMaxAnisotropyExt,
			                       (result.Filter == TextureFilter.Anisotropic)
				                       ? Math.Max(result.Anistropy, 1.0f)
				                       : 1.0f);
			OpenGL.GL.TexParameter(result.Target, OpenGL.TextureParameterName.TextureBaseLevel, result.MaxMipmapLevel);

			return result;
		}

		private void BindTexture(OpenGLTexture texture)
		{
			if (texture.Target != _textures[0].Target)
				OpenGL.GL.BindTexture(_textures[0].Target, 0);

			if (texture != _textures[0])
				OpenGL.GL.BindTexture(texture.Target, texture.Handle);

			_textures[0] = texture;
		}

		private void BindVertexBuffer(OpenGLBuffer buffer)
		{
			if (buffer.Handle != _currentVertexBufferHandle)
			{
				OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, buffer.Handle);
				_currentVertexBufferHandle = buffer.Handle;
			}
		}

		private void BindIndexBuffer(OpenGLBuffer buffer)
		{
			if (buffer.Handle != _currentIndexBuffer)
			{
				OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ElementArrayBuffer, buffer.Handle);
				_currentIndexBuffer = buffer.Handle;
			}
		}

		private void DeleteVertexBuffer(OpenGLBuffer buffer)
		{
			var handle = buffer.Handle;
			if (handle == _currentVertexBufferHandle)
			{
				OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, 0);
				_currentVertexBufferHandle = 0;
			}

			for (int i = 0; i < attributes.Length; i += 1)
			{
				if (handle == attributes[i].CurrentBuffer)
				{
					// Force the next vertex attrib update!
					attributes[i].CurrentBuffer = int.MaxValue;
				}
			}

			OpenGL.GL.DeleteBuffers(1, ref handle);
		}

		private void DeleteIndexBuffer(OpenGLBuffer buffer)
		{
			var handle = buffer.Handle;
			if (handle == _currentIndexBuffer)
			{
				OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ElementArrayBuffer, 0);
				_currentIndexBuffer = 0;
			}

			OpenGL.GL.DeleteBuffers(1, ref handle);
		}

		private static void ToggleGLState(OpenGL.EnableCap feature, bool enable)
		{
			if (enable)
			{
				OpenGL.GL.Enable(feature);
			}
			else
			{
				OpenGL.GL.Disable(feature);
			}
		}

		internal unsafe void ActivateShaderProgram()
		{
			// Lookup the shader program.
			var shaderProgram = _programCache.GetProgram(VertexShader, PixelShader);
			if (shaderProgram.Program == -1)
				return;
			// Set the new program if it has changed.
			if (_shaderProgram != shaderProgram)
			{
				OpenGL.GL.UseProgram(shaderProgram.Program);
				OpenGL.GL.CheckError();
				_shaderProgram = shaderProgram;
			}

			var posFixUpLocation = shaderProgram.GetUniformLocation("posFixup");
			if (posFixUpLocation == -1)
				return;

			//// Apply vertex shader fix:
			//// The following two lines are appended to the end of vertex shaders
			//// to account for rendering differences between OpenGL and DirectX:
			////
			//// gl_Position.y = gl_Position.y * posFixup.y;
			//// gl_Position.xy += posFixup.zw * gl_Position.ww;
			////
			//// (the following paraphrased from wine, wined3d/state.c and wined3d/glsl_shader.c)
			////
			//// - We need to flip along the y-axis in case of offscreen rendering.
			//// - D3D coordinates refer to pixel centers while GL coordinates refer
			////   to pixel corners.
			//// - D3D has a top-left filling convention. We need to maintain this
			////   even after the y-flip mentioned above.
			//// In order to handle the last two points, we translate by
			//// (63.0 / 128.0) / VPw and (63.0 / 128.0) / VPh. This is equivalent to
			//// translating slightly less than half a pixel. We want the difference to
			//// be large enough that it doesn't get lost due to rounding inside the
			//// driver, but small enough to prevent it from interfering with any
			//// anti-aliasing.
			////
			//// OpenGL coordinates specify the center of the pixel while d3d coords specify
			//// the corner. The offsets are stored in z and w in posFixup. posFixup.y contains
			//// 1.0 or -1.0 to turn the rendering upside down for offscreen rendering. PosFixup.x
			//// contains 1.0 to allow a mad.

			//_posFixup[0] = 1.0f;
			//_posFixup[1] = 1.0f;
			//if (UseHalfPixelOffset)
			//{
			//	_posFixup[2] = (63.0f / 64.0f) / Viewport.Width;
			//	_posFixup[3] = -(63.0f / 64.0f) / Viewport.Height;
			//}
			//else
			//{
			//	_posFixup[2] = 0f;
			//	_posFixup[3] = 0f;
			//}

			////If we have a render target bound (rendering offscreen)
			//if (IsRenderTargetBound)
			//{
			//	//flip vertically
			//	_posFixup[1] *= -1.0f;
			//	_posFixup[3] *= -1.0f;
			//}

			//fixed (float* floatPtr = _posFixup)
			//{
			//	OpenGL.GL.Uniform4(posFixupLoc, 1, floatPtr);
			//}
			//OpenGL.GL.CheckError();
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		private static extern void memcpy(IntPtr dst, IntPtr src, IntPtr len);

		#region Fields

		private IntPtr _glContext;

		private int _clearStencil = 0;
		private float _clearDepth = 1.0f;
		private Vector4 _clearColor = Vector4.Zero;

		private BlendState _currentBlendState;
		private DepthStencilState _currentDepthStencilState;

		private bool _alphaBlendEnable = false;
		private Color _blendFactor = Color.Transparent;
		private BlendFunction _colorBlendFunction = BlendFunction.Add;
		private BlendFunction _alphaBlendFunction = BlendFunction.Add;
		private Blend _colorSourceBlend = Blend.One;
		private Blend _colorDestinationBlend = Blend.Zero;
		private Blend _alphaSourceBlend = Blend.One;
		private Blend _alphaDestinationBlend = Blend.Zero;
		private ColorWriteChannels _colorWriteEnable = ColorWriteChannels.All;
		private ColorWriteChannels colorWriteEnable1 = ColorWriteChannels.All;
		private ColorWriteChannels colorWriteEnable2 = ColorWriteChannels.All;
		private ColorWriteChannels colorWriteEnable3 = ColorWriteChannels.All;
		private int multisampleMask = -1; // AKA 0xFFFFFFFF

		private bool _zEnable = false;
		private bool _zWriteEnable = false;
		private CompareFunction _depthFunc = CompareFunction.Less;

		private bool stencilEnable = false;
		private int stencilWriteMask = -1; // AKA 0xFFFFFFFF, ugh -flibit
		private bool separateStencilEnable = false;
		private int stencilRef = 0;
		private int stencilMask = -1; // AKA 0xFFFFFFFF, ugh -flibit
		private CompareFunction stencilFunc = CompareFunction.Always;
		private StencilOperation stencilFail = StencilOperation.Keep;
		private StencilOperation stencilZFail = StencilOperation.Keep;
		private StencilOperation stencilPass = StencilOperation.Keep;
		private CompareFunction ccwStencilFunc = CompareFunction.Always;
		private StencilOperation ccwStencilFail = StencilOperation.Keep;
		private StencilOperation ccwStencilZFail = StencilOperation.Keep;
		private StencilOperation ccwStencilPass = StencilOperation.Keep;

		private bool _scissorTestEnable = false;
		private CullMode cullFrontFace = CullMode.None;
		private FillMode fillMode = FillMode.Solid;
		private float depthBias = 0.0f;
		private float slopeScaleDepthBias = 0.0f;
		private bool multiSampleEnable = true;
		private bool renderTargetBound = false;

		private DepthFormat currentDepthStencilFormat;
		private DepthFormat _windowDepthFormat;

		private int _currentIndexBuffer = 0;
		private int _currentVertexBufferHandle = 0;

		private OpenGLTexture[] _textures;

		private class VertexAttribute
		{
			public int CurrentBuffer;
			public IntPtr CurrentPointer;
			public VertexElementFormat CurrentFormat;
			public bool CurrentNormalized;
			public int CurrentStride;

			public VertexAttribute()
			{
				CurrentBuffer = 0;
				CurrentPointer = IntPtr.Zero;
				CurrentFormat = VertexElementFormat.Single;
				CurrentNormalized = false;
				CurrentStride = 0;
			}
		}

		private VertexAttribute[] attributes;
		private bool[] attributeEnabled;
		private bool[] previousAttributeEnabled;
		private int[] attributeDivisor;
		private int[] previousAttributeDivisor;

		// TODO: private readonly VertexBufferBinding[] vertexBufferBindings = new VertexBufferBinding[MaxVertexAttributes];
		private int vertexBufferCount = 0;
		private bool vertexBuffersUpdated = false;

		private ShaderProgramCache _programCache;
		private ShaderProgram _shaderProgram = null;

		private readonly uint[] currentAttachments;
		private readonly bool[] modifiedSamplers = new bool[MaxTextureSamplers];
		private readonly bool[] modifiedVertexSamplers = new bool[MaxVertexTextureSamplers];

		private const int MaxTextureSamplers = 16;
		internal const int MaxVertexAttributes = 16;
		internal const int MaxRenderTargetBindings = 4;
		private const int MaxVertexTextureSamplers = 4;

		#endregion

		#region Convert Maps

		private static readonly OpenGL.PixelFormat[] TextureFormat
			=
			{
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.Color
				OpenGL.PixelFormat.Rgb, // GLenum.GL_RGB, // SurfaceFormat.Bgr565
				OpenGL.PixelFormat.Bgra, // GLenum.GL_BGRA, // SurfaceFormat.Bgra5551
				OpenGL.PixelFormat.Bgra, // GLenum.GL_BGRA, // SurfaceFormat.Bgra4444
				OpenGL.PixelFormat
				      .CompressedTextureFormats, // GLenum.GL_COMPRESSED_TEXTURE_FORMATS, // SurfaceFormat.Dxt1
				OpenGL.PixelFormat
				      .CompressedTextureFormats, // GLenum.GL_COMPRESSED_TEXTURE_FORMATS, // SurfaceFormat.Dxt3
				OpenGL.PixelFormat
				      .CompressedTextureFormats, // GLenum.GL_COMPRESSED_TEXTURE_FORMATS, // SurfaceFormat.Dxt5
				OpenGL.PixelFormat.Rg, // GLenum.GL_RG, // SurfaceFormat.NormalizedByte2
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.NormalizedByte4
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.Rgba1010102
				OpenGL.PixelFormat.Rg, // GLenum.GL_RG, // SurfaceFormat.Rg32
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.Rgba64
				OpenGL.PixelFormat.Luminance, // GLenum.GL_LUMINANCE, // SurfaceFormat.Alpha8
				OpenGL.PixelFormat.Red, // GLenum.GL_RED, // SurfaceFormat.Single
				OpenGL.PixelFormat.Rg, // GLenum.GL_RG, // SurfaceFormat.Vector2
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.Vector4
				OpenGL.PixelFormat.Red, // GLenum.GL_RED, // SurfaceFormat.HalfSingle
				OpenGL.PixelFormat.Rg, // GLenum.GL_RG, // SurfaceFormat.HalfVector2
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.HalfVector4
				OpenGL.PixelFormat.Rgba, // GLenum.GL_RGBA, // SurfaceFormat.HdrBlendable
				OpenGL.PixelFormat.Bgra, // GLenum.GL_BGRA, // SurfaceFormat.ColorBgraEXT
			};

		private static readonly OpenGL.PixelInternalFormat[] TextureInternalFormat
			=
			{
				OpenGL.PixelInternalFormat.Rgba8, // GLenum.GL_RGBA8, // SurfaceFormat.Color
				OpenGL.PixelInternalFormat.Rgb8, // GLenum.GL_RGB8, // SurfaceFormat.Bgr565
				OpenGL.PixelInternalFormat.Rgb5A1, // GLenum.GL_RGB5_A1, // SurfaceFormat.Bgra5551
				OpenGL.PixelInternalFormat.Rgba4, // GLenum.GL_RGBA4, // SurfaceFormat.Bgra4444
				OpenGL.PixelInternalFormat
				      .CompressedRgbS3tcDxt1Ext, // GLenum.GL_COMPRESSED_RGBA_S3TC_DXT1_EXT, // SurfaceFormat.Dxt1
				OpenGL.PixelInternalFormat
				      .CompressedRgbaS3tcDxt3Ext, // GLenum.GL_COMPRESSED_RGBA_S3TC_DXT3_EXT, // SurfaceFormat.Dxt3
				OpenGL.PixelInternalFormat
				      .CompressedRgbaS3tcDxt5Ext, // GLenum.GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, // SurfaceFormat.Dxt5
				OpenGL.PixelInternalFormat.Rg8, // GLenum.GL_RG8, // SurfaceFormat.NormalizedByte2
				OpenGL.PixelInternalFormat.Rgba8, // GLenum.GL_RGBA8, // SurfaceFormat.NormalizedByte4
				OpenGL.PixelInternalFormat.Rgb10A2Ext, // GLenum.GL_RGB10_A2_EXT, // SurfaceFormat.Rgba1010102
				OpenGL.PixelInternalFormat.Rg16, // GLenum.GL_RG16, // SurfaceFormat.Rg32
				OpenGL.PixelInternalFormat.Rgba16, // GLenum.GL_RGBA16, // SurfaceFormat.Rgba64
				OpenGL.PixelInternalFormat.Luminance, // GLenum.GL_LUMINANCE, // SurfaceFormat.Alpha8
				OpenGL.PixelInternalFormat.R32f, // GLenum.GL_R32F, // SurfaceFormat.Single
				OpenGL.PixelInternalFormat.Rgba32f, // GLenum.GL_RG32F, // SurfaceFormat.Vector2
				OpenGL.PixelInternalFormat.Rgba32f, // GLenum.GL_RGBA32F, // SurfaceFormat.Vector4
				OpenGL.PixelInternalFormat.R16f, // GLenum.GL_R16F, // SurfaceFormat.HalfSingle
				OpenGL.PixelInternalFormat.Rg16f, // GLenum.GL_RG16F, // SurfaceFormat.HalfVector2
				OpenGL.PixelInternalFormat.Rgba16f, // GLenum.GL_RGBA16F, // SurfaceFormat.HalfVector4
				OpenGL.PixelInternalFormat.Rgba16f, // GLenum.GL_RGBA16F, // SurfaceFormat.HdrBlendable
				OpenGL.PixelInternalFormat.Rgba8, // GLenum.GL_RGBA8 // SurfaceFormat.ColorBgraEXT
			};

		private static readonly OpenGL.PixelType[] TextureDataType
			=
			{
				OpenGL.PixelType.UnsignedByte, // GLenum.GL_UNSIGNED_BYTE, // SurfaceFormat.Color
				OpenGL.PixelType.UnsignedShort565, // GLenum.GL_UNSIGNED_SHORT_5_6_5, // SurfaceFormat.Bgr565
				OpenGL.PixelType.UnsignedShort5551, // GLenum.GL_UNSIGNED_SHORT_5_5_5_1_REV, // SurfaceFormat.Bgra5551
				OpenGL.PixelType.UnsignedShort4444, // GLenum.GL_UNSIGNED_SHORT_4_4_4_4_REV, // SurfaceFormat.Bgra4444
				(OpenGL.PixelType) 0x0000, // GLenum.GL_ZERO, // NOPE
				(OpenGL.PixelType) 0x0000, // GLenum.GL_ZERO, // NOPE
				(OpenGL.PixelType) 0x0000, // GLenum.GL_ZERO, // NOPE
				OpenGL.PixelType.Byte, // GLenum.GL_BYTE, // SurfaceFormat.NormalizedByte2
				OpenGL.PixelType.Byte, // GLenum.GL_BYTE, // SurfaceFormat.NormalizedByte4
				OpenGL.PixelType
				      .UnsignedInt1010102, // GLenum.GL_UNSIGNED_INT_2_10_10_10_REV, // SurfaceFormat.Rgba1010102
				OpenGL.PixelType.UnsignedShort, // GLenum.GL_UNSIGNED_SHORT, // SurfaceFormat.Rg32
				OpenGL.PixelType.UnsignedShort, // GLenum.GL_UNSIGNED_SHORT, // SurfaceFormat.Rgba64
				OpenGL.PixelType.UnsignedByte, // GLenum.GL_UNSIGNED_BYTE, // SurfaceFormat.Alpha8
				OpenGL.PixelType.Float, // GLenum.GL_FLOAT, // SurfaceFormat.Single
				OpenGL.PixelType.Float, // GLenum.GL_FLOAT, // SurfaceFormat.Vector2
				OpenGL.PixelType.Float, // GLenum.GL_FLOAT, // SurfaceFormat.Vector4
				OpenGL.PixelType.HalfFloat, // GLenum.GL_HALF_FLOAT, // SurfaceFormat.HalfSingle
				OpenGL.PixelType.HalfFloat, // GLenum.GL_HALF_FLOAT, // SurfaceFormat.HalfVector2
				OpenGL.PixelType.HalfFloat, // GLenum.GL_HALF_FLOAT, // SurfaceFormat.HalfVector4
				OpenGL.PixelType.HalfFloat, // GLenum.GL_HALF_FLOAT, // SurfaceFormat.HdrBlendable
				OpenGL.PixelType.Byte, // GLenum.GL_UNSIGNED_BYTE
			};

		private static readonly int[] Wrap
			=
			{
				(int) OpenGL.TextureWrapMode.Repeat, // GLenum.GL_REPEAT, // TextureAddressMode.Wrap
				(int) OpenGL.TextureWrapMode.ClampToEdge, // GLenum.GL_CLAMP_TO_EDGE, // TextureAddressMode.Clamp
				(int) OpenGL.TextureWrapMode.MirroredRepeat, // GLenum.GL_MIRRORED_REPEAT // TextureAddressMode.Mirror
			};

		private static readonly int[] MagFilter
			=
			{
				(int) OpenGL.TextureMagFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.Linear
				(int) OpenGL.TextureMagFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.Point
				(int) OpenGL.TextureMagFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.Anisotropic
				(int) OpenGL.TextureMagFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.LinearMipPoint
				(int) OpenGL.TextureMagFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.PointMipLinear
				(int) OpenGL.TextureMagFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.MinLinearMagPointMipLinear
				(int) OpenGL.TextureMagFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.MinLinearMagPointMipPoint
				(int) OpenGL.TextureMagFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.MinPointMagLinearMipLinear
				(int) OpenGL.TextureMagFilter.Linear, // GLenum.GL_LINEAR // TextureFilter.MinPointMagLinearMipPoint
			};

		private static readonly int[] MinMipFilter
			=
			{
				(int) OpenGL.TextureMinFilter
				            .LinearMipmapLinear, // GLenum.GL_LINEAR_MIPMAP_LINEAR, // TextureFilter.Linear
				(int) OpenGL.TextureMinFilter
				            .NearestMipmapNearest, // GLenum.GL_NEAREST_MIPMAP_NEAREST, // TextureFilter.Point
				(int) OpenGL.TextureMinFilter
				            .LinearMipmapLinear, // GLenum.GL_LINEAR_MIPMAP_LINEAR, // TextureFilter.Anisotropic
				(int) OpenGL.TextureMinFilter
				            .LinearMipmapNearest, // GLenum.GL_LINEAR_MIPMAP_NEAREST, // TextureFilter.LinearMipPoint
				(int) OpenGL.TextureMinFilter
				            .NearestMipmapLinear, // GLenum.GL_NEAREST_MIPMAP_LINEAR, // TextureFilter.PointMipLinear
				(int) OpenGL.TextureMinFilter
				            .LinearMipmapLinear, // GLenum.GL_LINEAR_MIPMAP_LINEAR, // TextureFilter.MinLinearMagPointMipLinear
				(int) OpenGL.TextureMinFilter
				            .LinearMipmapNearest, // GLenum.GL_LINEAR_MIPMAP_NEAREST, // TextureFilter.MinLinearMagPointMipPoint
				(int) OpenGL.TextureMinFilter
				            .NearestMipmapLinear, // GLenum.GL_NEAREST_MIPMAP_LINEAR, // TextureFilter.MinPointMagLinearMipLinear
				(int) OpenGL.TextureMinFilter
				            .NearestMipmapNearest, // GLenum.GL_NEAREST_MIPMAP_NEAREST // TextureFilter.MinPointMagLinearMipPoint
			};

		private static readonly int[] MinFilter
			=
			{
				(int) OpenGL.TextureMinFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.Linear
				(int) OpenGL.TextureMinFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.Point
				(int) OpenGL.TextureMinFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.Anisotropic
				(int) OpenGL.TextureMinFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.LinearMipPoint
				(int) OpenGL.TextureMinFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.PointMipLinear
				(int) OpenGL.TextureMinFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.MinLinearMagPointMipLinear
				(int) OpenGL.TextureMinFilter.Linear, // GLenum.GL_LINEAR, // TextureFilter.MinLinearMagPointMipPoint
				(int) OpenGL.TextureMinFilter.Nearest, // GLenum.GL_NEAREST, // TextureFilter.MinPointMagLinearMipLinear
				(int) OpenGL.TextureMinFilter.Nearest, // GLenum.GL_NEAREST // TextureFilter.MinPointMagLinearMipPoint
			};

		private static readonly OpenGL.BlendingFactorSrc[] BlendSrcMode
			=
			{
				OpenGL.BlendingFactorSrc.One, // GLenum.GL_ONE, Blend.One
				OpenGL.BlendingFactorSrc.Zero, // GLenum.GL_ZERO, Blend.Zero
				OpenGL.BlendingFactorSrc.SrcColor, // GLenum.GL_SRC_COLOR, Blend.SourceColor
				OpenGL.BlendingFactorSrc.OneMinusSrcColor, // GLenum.GL_ONE_MINUS_SRC_COLOR, Blend.InverseSourceColor
				OpenGL.BlendingFactorSrc.SrcAlpha, // GLenum.GL_SRC_ALPHA, Blend.SourceAlpha
				OpenGL.BlendingFactorSrc.OneMinusSrcAlpha, // GLenum.GL_ONE_MINUS_SRC_ALPHA, Blend.InverseSourceAlpha
				OpenGL.BlendingFactorSrc.DstColor, // GLenum.GL_DST_COLOR, Blend.DestinationColor
				OpenGL.BlendingFactorSrc.OneMinusDstColor, // GLenum.GL_ONE_MINUS_DST_COLOR, Blend.InverseDestinationColor
				OpenGL.BlendingFactorSrc.DstAlpha, // GLenum.GL_DST_ALPHA, Blend.DestinationAlpha
				OpenGL.BlendingFactorSrc.OneMinusDstAlpha, // GLenum.GL_ONE_MINUS_DST_ALPHA, Blend.InverseDestinationAlpha
				OpenGL.BlendingFactorSrc.ConstantColor, // GLenum.GL_CONSTANT_COLOR, Blend.BlendFactor
				OpenGL.BlendingFactorSrc.OneMinusConstantColor, // GLenum.GL_ONE_MINUS_CONSTANT_COLOR, Blend.InverseBlendFactor
				OpenGL.BlendingFactorSrc.SrcAlphaSaturate // GLenum.GL_SRC_ALPHA_SATURATE Blend.SourceAlphaSaturation
			};

		private static readonly OpenGL.BlendingFactorDest[] BlendDstMode
			=
			{
				OpenGL.BlendingFactorDest.One, // GLenum.GL_ONE, Blend.One
				OpenGL.BlendingFactorDest.Zero, // GLenum.GL_ZERO, Blend.Zero
				OpenGL.BlendingFactorDest.SrcColor, // GLenum.GL_SRC_COLOR, Blend.SourceColor
				OpenGL.BlendingFactorDest.OneMinusSrcColor, // GLenum.GL_ONE_MINUS_SRC_COLOR, Blend.InverseSourceColor
				OpenGL.BlendingFactorDest.SrcAlpha, // GLenum.GL_SRC_ALPHA, Blend.SourceAlpha
				OpenGL.BlendingFactorDest.OneMinusSrcAlpha, // GLenum.GL_ONE_MINUS_SRC_ALPHA, Blend.InverseSourceAlpha
				OpenGL.BlendingFactorDest.DstColor, // GLenum.GL_DST_COLOR, Blend.DestinationColor
				OpenGL.BlendingFactorDest.OneMinusDstColor, // GLenum.GL_ONE_MINUS_DST_COLOR, Blend.InverseDestinationColor
				OpenGL.BlendingFactorDest.DstAlpha, // GLenum.GL_DST_ALPHA, Blend.DestinationAlpha
				OpenGL.BlendingFactorDest.OneMinusDstAlpha, // GLenum.GL_ONE_MINUS_DST_ALPHA, Blend.InverseDestinationAlpha
				OpenGL.BlendingFactorDest.ConstantColor, // GLenum.GL_CONSTANT_COLOR, Blend.BlendFactor
				OpenGL.BlendingFactorDest.OneMinusConstantColor, // GLenum.GL_ONE_MINUS_CONSTANT_COLOR, Blend.InverseBlendFactor
				OpenGL.BlendingFactorDest.SrcAlphaSaturate // GLenum.GL_SRC_ALPHA_SATURATE Blend.SourceAlphaSaturation
			};

		private static readonly OpenGL.BlendEquationMode[] BlendEquation
			=
			{
				OpenGL.BlendEquationMode.FuncAdd, // GLenum.GL_FUNC_ADD, BlendFunction.Add
				OpenGL.BlendEquationMode.FuncSubtract, // GLenum.GL_FUNC_SUBTRACT, BlendFunction.Subtract
				OpenGL.BlendEquationMode
				      .FuncReverseSubtract, // GLenum.GL_FUNC_REVERSE_SUBTRACT, BlendFunction.ReverseSubtract
				OpenGL.BlendEquationMode.Max, // GLenum.GL_MAX, BlendFunction.Max
				OpenGL.BlendEquationMode.Min // GLenum.GL_MIN, BlendFunction.Min
			};

		private static readonly OpenGL.DepthFunction[] DepthFunc
			=
			{
				OpenGL.DepthFunction.Always, // GLenum.GL_ALWAYS, CompareFunction.Always
				OpenGL.DepthFunction.Never, // GLenum.GL_NEVER, CompareFunction.Never
				OpenGL.DepthFunction.Less, // GLenum.GL_LESS, CompareFunction.Less
				OpenGL.DepthFunction.LessEqual, // GLenum.GL_LEQUAL, CompareFunction.LessEqual
				OpenGL.DepthFunction.Equal, // GLenum.GL_EQUAL, CompareFunction.Equal
				OpenGL.DepthFunction.GreaterEqual, // GLenum.GL_GEQUAL, CompareFunction.GreaterEqual
				OpenGL.DepthFunction.Greater, // GLenum.GL_GREATER, CompareFunction.Greater
				OpenGL.DepthFunction.NotEqual // GLenum.GL_NOTEQUAL, CompareFunction.NotEqual
			};

		private static readonly OpenGL.StencilFunction[] StencilFunc
			=
			{
				OpenGL.StencilFunction.Always, // GLenum.GL_ALWAYS, CompareFunction.Always
				OpenGL.StencilFunction.Never, // GLenum.GL_NEVER, CompareFunction.Never
				OpenGL.StencilFunction.Less, // GLenum.GL_LESS, CompareFunction.Less
				OpenGL.StencilFunction.LessEqual, // GLenum.GL_LEQUAL, CompareFunction.LessEqual
				OpenGL.StencilFunction.Equal, // GLenum.GL_EQUAL, CompareFunction.Equal
				OpenGL.StencilFunction.GreaterEqual, // GLenum.GL_GEQUAL, CompareFunction.GreaterEqual
				OpenGL.StencilFunction.Greater, // GLenum.GL_GREATER, CompareFunction.Greater
				OpenGL.StencilFunction.NotEqual // GLenum.GL_NOTEQUAL, CompareFunction.NotEqual
			};

		private static readonly OpenGL.StencilOp[] StencilOp
			=
			{
				OpenGL.StencilOp.Keep, // GLenum.GL_KEEP, StencilOperation.Keep
				OpenGL.StencilOp.Zero, // GLenum.GL_ZERO, StencilOperation.Zero
				OpenGL.StencilOp.Replace, // GLenum.GL_REPLACE, StencilOperation.Replace
				OpenGL.StencilOp.IncrWrap, // GLenum.GL_INCR_WRAP, StencilOperation.Increment
				OpenGL.StencilOp.DecrWrap, // GLenum.GL_DECR_WRAP,	StencilOperation.Decrement
				OpenGL.StencilOp.Incr, // GLenum.GL_INCR, StencilOperation.IncrementSaturation
				OpenGL.StencilOp.Decr, // GLenum.GL_DECR, StencilOperation.DecrementSaturation
				OpenGL.StencilOp.Invert // GLenum.GL_INVERT, StencilOperation.Invert
			};

		private static readonly OpenGL.FrontFaceDirection[] FrontFace
			=
			{
				0, // None
				OpenGL.FrontFaceDirection.Cw,
				OpenGL.FrontFaceDirection.Ccw
			};

		private static readonly OpenGL.PolygonMode[] PolygonMode
			=
			{
				OpenGL.PolygonMode.Fill, // FillMode.Solid
				OpenGL.PolygonMode.Line // FillMode.WireFrame
			};

		private static readonly float[] DepthBiasScale
			=
			{
				0.0f, // DepthFormat.None
				(float) ((1 << 16) - 1), // DepthFormat.Depth16
				(float) ((1 << 24) - 1), // DepthFormat.Depth24
				(float) ((1 << 24) - 1) // DepthFormat.Depth24Stencil8
			};

		private static readonly OpenGL.DrawElementsType[] IndexType
			=
			{
				OpenGL.DrawElementsType.UnsignedShort, // GLenum.GL_UNSIGNED_SHORT, // IndexElementSize.SixteenBits
				OpenGL.DrawElementsType.UnsignedInt, // GLenum.GL_UNSIGNED_INT // IndexElementSize.ThirtyTwoBits
			};

		private static readonly int[] IndexSize
			=
			{
				2, // IndexElementSize.SixteenBits
				4 // IndexElementSize.ThirtyTwoBits
			};

		private static readonly OpenGL.PrimitiveType[] Primitive
			=
			{
				OpenGL.PrimitiveType.Triangles, // GLenum.GL_TRIANGLES, // PrimitiveType.TriangleList
				OpenGL.PrimitiveType.TriangleStrip, // GLenum.GL_TRIANGLE_STRIP, // PrimitiveType.TriangleStrip
				OpenGL.PrimitiveType.Lines, // GLenum.GL_LINES, // PrimitiveType.LineList
				OpenGL.PrimitiveType.LineStrip, // GLenum.GL_LINE_STRIP, // PrimitiveType.LineStrip
				// TODO: ... GLenum.GL_POINTS // PrimitiveType.PointListEXT
			};

		private static readonly int[] VertexAttribSize
			=
			{
				1, // VertexElementFormat.Single
				2, // VertexElementFormat.Vector2
				3, // VertexElementFormat.Vector3
				4, // VertexElementFormat.Vector4
				4, // VertexElementFormat.Color
				4, // VertexElementFormat.Byte4
				2, // VertexElementFormat.Short2
				4, // VertexElementFormat.Short4
				2, // VertexElementFormat.NormalizedShort2
				4, // VertexElementFormat.NormalizedShort4
				2, // VertexElementFormat.HalfVector2
				4 // VertexElementFormat.HalfVector4
			};

		private static readonly OpenGL.VertexAttribPointerType[] VertexAttribType
			=
			{
				OpenGL.VertexAttribPointerType.Float, // VertexElementFormat.Single
				OpenGL.VertexAttribPointerType.Float, // VertexElementFormat.Vector2
				OpenGL.VertexAttribPointerType.Float, // VertexElementFormat.Vector3
				OpenGL.VertexAttribPointerType.Float, // VertexElementFormat.Vector4
				OpenGL.VertexAttribPointerType.UnsignedByte, // VertexElementFormat.Color
				OpenGL.VertexAttribPointerType.UnsignedByte, // VertexElementFormat.Byte4
				OpenGL.VertexAttribPointerType.Short, // VertexElementFormat.Short2
				OpenGL.VertexAttribPointerType.Short, // VertexElementFormat.Short4
				OpenGL.VertexAttribPointerType.Short, // VertexElementFormat.NormalizedShort2
				OpenGL.VertexAttribPointerType.Short, // VertexElementFormat.NormalizedShort4
				OpenGL.VertexAttribPointerType.HalfFloat, // VertexElementFormat.HalfVector2
				OpenGL.VertexAttribPointerType.HalfFloat, // VertexElementFormat.HalfVector4
			};

		private static bool VertexAttribNormalized(VertexElement element)
		{
			return element.VertexElementUsage == VertexElementUsage.Color ||
			       element.VertexElementFormat == VertexElementFormat.NormalizedShort2 ||
			       element.VertexElementFormat == VertexElementFormat.NormalizedShort4;
		}

		private static int PrimitiveVerts(PrimitiveType primitiveType, int primitiveCount)
		{
			switch (primitiveType)
			{
				case PrimitiveType.TriangleList:
					return primitiveCount * 3;
				case PrimitiveType.TriangleStrip:
					return primitiveCount + 2;
				case PrimitiveType.LineList:
					return primitiveCount * 2;
				case PrimitiveType.LineStrip:
					return primitiveCount + 1;
				case PrimitiveType.PointListEXT:
					return primitiveCount;
			}

			throw new NotSupportedException();
		}
	}

	#endregion
}
