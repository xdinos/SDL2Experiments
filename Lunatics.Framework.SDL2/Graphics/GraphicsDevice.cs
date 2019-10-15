using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Mathematics;
using Lunatics.Framework.Sdl.OpenGL;
using SDL2;

namespace Lunatics.Framework.Sdl.Graphics
{
	internal sealed class VertexBuffer : Lunatics.Framework.Graphics.VertexBuffer
	{
		internal int Handle { get; }
		internal IntPtr Size { get; }
		internal GL.BufferUsageHint UsageHint { get; }

		internal VertexBuffer(GraphicsDevice graphicsDevice,
		                      VertexDeclaration vertexDeclaration,
		                      int vertexCount, 
		                      BufferUsage bufferUsage,
							  int handle,
		                      IntPtr size,
							  GL.BufferUsageHint usageHint)
			: base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage)
		{
			Handle = handle;
			Size = size;
			UsageHint = usageHint;
		}

		public override void SetData<T>(T[] data)
		{
			// TODO: ErrorCheck(data, 0, data.Length, Marshal.SizeOf(typeof(T)));

			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			((GraphicsDevice) GraphicsDevice).SetVertexBufferData(this,
			                                                      0,
			                                                      handle.AddrOfPinnedObject(),
			                                                      data.Length * Marshal.SizeOf(typeof(T))/*,SetDataOptions.None*/);
			handle.Free();
		}
	}

	public sealed class GraphicsDevice : Framework.Graphics.GraphicsDevice
	{
		public GraphicsDevice(PresentationParameters presentationParameters, Framework.Graphics.GraphicsAdapter adapter)
		{
			PresentationParameters = presentationParameters;
			Adapter = adapter;

			_glContext = SDL.SDL_GL_CreateContext(presentationParameters.DeviceWindowHandle);

			SDL.SDL_GL_GetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, out var flags);

			// Check for a possible Core context
			_useCoreProfile = IsSet(flags, (int) SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

			// UIKit needs special treatment for back buffer behavior
			var wmInfo = new SDL.SDL_SysWMinfo();
			SDL.SDL_VERSION(out wmInfo.version);
			SDL.SDL_GetWindowWMInfo(presentationParameters.DeviceWindowHandle, ref wmInfo);

			if (wmInfo.subsystem == SDL.SDL_SYSWM_TYPE.SDL_SYSWM_UIKIT)
			{
				_realBackbufferFBO = wmInfo.info.uikit.framebuffer;
				_realBackbufferRBO = wmInfo.info.uikit.colorbuffer;
			}
			else
			{
				_realBackbufferFBO = 0;
				_realBackbufferRBO = 0;
			}


			GL.LoadEntryPoints();

			string renderer = GL.GetString(GL.StringName.Renderer);
			string version = GL.GetString(GL.StringName.Version);
			string vendor = GL.GetString(GL.StringName.Vendor);
			string shadingLanguageVersion = GL.GetString(GL.StringName.ShadingLanguageVersion);
			var extensions = GL.GetString(GL.StringName.Extensions);
		}

		public Shader CreateShader(string vertexSource, string fragmentSource, string geometrySource)
		{
			var vertexShader = GL.CreateShader(GL.ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexSource);
			GL.CompileShader(vertexShader);
			GL.GetShader
		}

		public override Framework.Graphics.VertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration,
		                                                                   int vertexCount,
		                                                                   BufferUsage bufferUsage)
		{
			var dynamic = false;

			var bufferUsageHint = dynamic
				                      ? GL.BufferUsageHint.StreamDraw
				                      : GL.BufferUsageHint.StaticDraw;
			var bufferSize = (IntPtr) (vertexDeclaration.VertexStride * vertexCount);
			
			GL.GenBuffers(1, out var handle);
			BindVertexBuffer(handle);
			GL.BufferData(GL.BufferTarget.ArrayBuffer, bufferSize, IntPtr.Zero, bufferUsageHint);
			
			return new VertexBuffer(this, vertexDeclaration, vertexCount, bufferUsage, 
									handle, bufferSize, bufferUsageHint);
		}

		internal void SetVertexBufferData(VertexBuffer buffer,
		                                  int offsetInBytes,
		                                  IntPtr data,
		                                  int dataLength/*,SetDataOptions options*/)
		{
			BindVertexBuffer(buffer.Handle);

			//if (options == SetDataOptions.Discard)
			//{
			//	GL.BufferData(GL.BufferTarget.ArrayBuffer,
			//	              buffer.Size,
			//	              IntPtr.Zero,
			//	              buffer.UsageHint);
			//}

			GL.BufferSubData(GL.BufferTarget.ArrayBuffer,
			                 (IntPtr) offsetInBytes,
			                 (IntPtr) dataLength,
			                 data);
		}

		private void BindVertexBuffer(int vertexBufferHandle)
		{
			if (vertexBufferHandle == _currentVertexBufferHandle) return;

			GL.BindBuffer(GL.BufferTarget.ArrayBuffer, vertexBufferHandle);
			_currentVertexBufferHandle = vertexBufferHandle;
		}

		private int _currentVertexBufferHandle;

		public override void SetVertexBuffer(Framework.Graphics.VertexBuffer vertexBuffer, int vertexOffset)
		{
			BindVertexBuffer(((VertexBuffer) vertexBuffer).Handle);
		}

		public override void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
		{
			// TODO: ApplyState();

			// TODO: ...
			// Set up the vertex buffers

			//ApplyVertexAttributes(
			//	vertexBufferBindings,
			//	vertexBufferCount,
			//	vertexBuffersUpdated,
			//	0
			//);

			// TODO: vertexBuffersUpdated = false;

			GL.EnableVertexAttribArray(0);

			GL.VertexAttribPointer(0,
			                       VertexAttribSize[(int) VertexElementFormat.Vector3],
			                       VertexAttribType[(int) VertexElementFormat.Vector3],
			                       false,
			                       12,
			                       IntPtr.Zero);

			DrawPrimitivesImpl(primitiveType, vertexStart, primitiveCount);

			GL.DisableVertexAttribArray(0);
		}

		//public override void DrawUserPrimitives<T>(PrimitiveType primitiveType,
		//                                           T[] vertexData,
		//                                           int vertexOffset,
		//                                           int primitiveCount)
		//{
		//	// TODO: ApplyState();

		//	// Pin the buffers.
		//	GCHandle vbHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		//	IntPtr vbPtr = vbHandle.AddrOfPinnedObject();

		//	// Setup the vertex declaration to point at the vertex data.
		//	VertexDeclaration vertexDeclaration = VertexDeclarationCache<T>.VertexDeclaration;
		//	vertexDeclaration.GraphicsDevice = this;
		//	GLDevice.ApplyVertexAttributes(
		//		vertexDeclaration,
		//		vbPtr,
		//		0
		//	);

		//	GLDevice.DrawUserPrimitives(
		//		primitiveType,
		//		vbPtr,
		//		vertexOffset,
		//		primitiveCount
		//	);

		//	// Release the handles.
		//	vbHandle.Free();
		//}

		public override void DrawUserPrimitives<T>(PrimitiveType primitiveType,
		                                           T[] vertexData,
		                                           int vertexOffset,
		                                           int primitiveCount,
		                                           VertexDeclaration vertexDeclaration)
		{
			// TODO: ApplyState();

			// Pin the buffers.
			GCHandle vbHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
			IntPtr vbPtr = vbHandle.AddrOfPinnedObject();

			// Setup the vertex declaration to point at the vertex data.
			// TODO: ??? vertexDeclaration.GraphicsDevice = this;
			ApplyVertexAttributes(vertexDeclaration, vbPtr, 0);

			DrawUserPrimitivesImpl(primitiveType, vbPtr, vertexOffset, primitiveCount);

			// Release the handles.
			vbHandle.Free();
		}

		public override void Reset(PresentationParameters presentationParameters,
		                           Framework.Graphics.GraphicsAdapter graphicsAdapter)
		{
			PresentationParameters =
				presentationParameters ?? throw new ArgumentNullException(nameof(presentationParameters));
			Adapter = graphicsAdapter;

			// Verify MSAA before we really start...
			//PresentationParameters.MultiSampleCount = Math.Min(
			//	MathHelper.ClosestMSAAPower(
			//		PresentationParameters.MultiSampleCount
			//	),
			//	GLDevice.MaxMultiSampleCount
			//);

			// We're about to reset, let the application know.
			OnDeviceResetting();

			/* FIXME: Why are we not doing this...? -flibit
			lock (resourcesLock)
			{
				foreach (WeakReference resource in resources)
				{
					object target = resource.Target;
					if (target != null)
					{
						(target as GraphicsResource).GraphicsDeviceResetting();
					}
				}

				// Remove references to resources that have been garbage collected.
				resources.RemoveAll(wr => !wr.IsAlive);
			}
			*/

			/* Reset the backbuffer first, before doing anything else.
			 * The GLDevice needs to know what we're up to right away.
			 * -flibit
			 */
			ResetBackBuffer(PresentationParameters, Adapter);

			// The mouse needs to know this for faux-backbuffer mouse scaling.
			//Input.Mouse.INTERNAL_BackBufferWidth = PresentationParameters.BackBufferWidth;
			//Input.Mouse.INTERNAL_BackBufferHeight = PresentationParameters.BackBufferHeight;

			//// The Touch Panel needs this too, for the same reason.
			//Input.Touch.TouchPanel.DisplayWidth = PresentationParameters.BackBufferWidth;
			//Input.Touch.TouchPanel.DisplayHeight = PresentationParameters.BackBufferHeight;

			// Now, update the viewport
			Viewport = new Viewport(0, 0, PresentationParameters.BackBufferWidth,
			                        PresentationParameters.BackBufferHeight);

			// Update the scissor rectangle to our new default target size
			ScissorRectangle = new Rectangle(0, 0,
			                                 PresentationParameters.BackBufferWidth,
			                                 PresentationParameters.BackBufferHeight);

			// Finally, update the swap interval
			SetPresentationInterval(PresentationParameters.PresentationInterval);

			// We just reset, let the application know.
			OnDeviceReset();
		}

		public override void Present()
		{
			//GLDevice.SwapBuffers(null, null, PresentationParameters.DeviceWindowHandle);
			SDL.SDL_GL_SwapWindow(PresentationParameters.DeviceWindowHandle);
		}

		protected override void SetViewport(Viewport vp)
		{
			//// Flip viewport when target is not bound
			//if (!renderTargetBound)
			//{
			//	vp.Y = Backbuffer.Height - vp.Y - vp.Height;
			//}

			//if (vp.Bounds != _viewport)
			//{
			//	_viewport = vp.Bounds;
			//	glViewport(
			//		viewport.X,
			//		viewport.Y,
			//		viewport.Width,
			//		viewport.Height
			//	);
			//}

			//if (vp.MinDepth != depthRangeMin || vp.MaxDepth != depthRangeMax)
			//{
			//	depthRangeMin = vp.MinDepth;
			//	depthRangeMax = vp.MaxDepth;
			//	glDepthRange((double)depthRangeMin, (double)depthRangeMax);
			//}
		}

		protected override void SetScissorRect(Rectangle rectangle)
		{
			// Flip rectangle when target is not bound
			//if (!renderTargetBound)
			//{
			//	rectangle.Y = Backbuffer.Height - rectangle.Y - rectangle.Height;
			//}

			//if (rectangle != _scissorRectangle)
			//{
			//	_scissorRectangle = rectangle;
			//	GL.Scissor(_scissorRectangle.X,
			//	           _scissorRectangle.Y,
			//	           _scissorRectangle.Width,
			//	           _scissorRectangle.Height);
			//}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			SDL.SDL_GL_DeleteContext(_glContext);
		}

		public void SetPresentationInterval(PresentInterval interval)
		{
			if (interval == PresentInterval.Default || interval == PresentInterval.One)
			{
				string OSVersion = SDL.SDL_GetPlatform();
				bool disableLateSwapTear = (
					                           OSVersion.Equals("Mac OS X") ||
					                           OSVersion.Equals("WinRT") ||
					                           Environment.GetEnvironmentVariable("FNA_OPENGL_DISABLE_LATESWAPTEAR") ==
					                           "1"
				                           );
				if (disableLateSwapTear)
				{
					SDL.SDL_GL_SetSwapInterval(1);
				}
				else
				{
					if (SDL.SDL_GL_SetSwapInterval(-1) != -1)
					{
						//FNALoggerEXT.LogInfo("Using EXT_swap_control_tear VSync!");
					}
					else
					{
						//FNALoggerEXT.LogInfo("EXT_swap_control_tear unsupported. Fall back to standard VSync.");
						SDL.SDL_ClearError();
						SDL.SDL_GL_SetSwapInterval(1);
					}
				}
			}
			else if (interval == PresentInterval.Immediate)
			{
				SDL.SDL_GL_SetSwapInterval(0);
			}
			else if (interval == PresentInterval.Two)
			{
				SDL.SDL_GL_SetSwapInterval(2);
			}
			else
			{
				throw new NotSupportedException("Unrecognized PresentInterval!");
			}
		}

		public void ResetBackBuffer(PresentationParameters presentationParameters,
		                            Framework.Graphics.GraphicsAdapter adapter)
		{
			//if (UseFauxBackbuffer(presentationParameters, adapter.CurrentDisplayMode))
			//{
			//	if (Backbuffer is NullBackbuffer)
			//	{
			//		if (!supportsFauxBackbuffer)
			//		{
			//			throw new NoSuitableGraphicsDeviceException(
			//				"Your hardware does not support the faux-backbuffer!" +
			//				"\n\nKeep the window/backbuffer resolution the same."
			//			);
			//		}
			//		Backbuffer = new OpenGLBackbuffer(
			//			this,
			//			presentationParameters.BackBufferWidth,
			//			presentationParameters.BackBufferHeight,
			//			presentationParameters.DepthStencilFormat,
			//			presentationParameters.MultiSampleCount
			//		);
			//	}
			//	else
			//	{
			//		Backbuffer.ResetFramebuffer(
			//			presentationParameters
			//		);
			//	}
			//}
			//else
			//{
			//	if (Backbuffer is OpenGLBackbuffer)
			//	{
			//		(Backbuffer as OpenGLBackbuffer).Dispose();
			//		Backbuffer = new NullBackbuffer(
			//			presentationParameters.BackBufferWidth,
			//			presentationParameters.BackBufferHeight,
			//			windowDepthFormat
			//		);
			//	}
			//	else
			//	{
			//		Backbuffer.ResetFramebuffer(presentationParameters);
			//	}
			//}
		}

		protected override void ClearImpl(ClearOptions options, Vector4 color, float depth, int stencil)
		{
			// glClear depends on the scissor rectangle!
			if (_isScissorTestEnable)
			{
				GL.Disable(GL.EnableCap.ScissorTest);
			}

			bool clearTarget = (options & ClearOptions.Target) == ClearOptions.Target;
			bool clearDepth = (options & ClearOptions.DepthBuffer) == ClearOptions.DepthBuffer;
			bool clearStencil = (options & ClearOptions.Stencil) == ClearOptions.Stencil;

			// Get the clear mask, set the clear properties if needed
			GL.ClearBufferMask clearMask = 0;
			if (clearTarget)
			{
				clearMask |= GL.ClearBufferMask.ColorBufferBit;
				if (!color.Equals(_clearColor))
				{
					GL.ClearColor(color.X, color.Y, color.Z, color.W);
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
				clearMask |= GL.ClearBufferMask.DepthBufferBit;
				if (depth != _clearDepth)
				{
					GL.ClearDepth(depth);
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
				clearMask |= GL.ClearBufferMask.StencilBufferBit;
				if (stencil != _clearStencil)
				{
					GL.ClearStencil(stencil);
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
			GL.Clear(clearMask);

			// Clean up after ourselves.
			if (_isScissorTestEnable)
			{
				GL.Enable(GL.EnableCap.ScissorTest);
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

		private void ApplyVertexAttributes(VertexDeclaration vertexDeclaration, IntPtr ptr, int vertexOffset)
		{
			//BindVertexBuffer(OpenGLBuffer.NullBuffer);
			//IntPtr basePtr = ptr + (vertexDeclaration.VertexStride * vertexOffset);

			//if (vertexDeclaration != ldVertexDeclaration ||
			//    basePtr != ldPointer ||
			//    currentEffect != ldEffect ||
			//    currentTechnique != ldTechnique ||
			//    currentPass != ldPass ||
			//    effectApplied)
			//{
			//	/* There's this weird case where you can have overlapping
			//	 * vertex usage/index combinations. It seems like the first
			//	 * attrib gets priority, so whenever a duplicate attribute
			//	 * exists, give it the next available index. If that fails, we
			//	 * have to crash :/
			//	 * -flibit
			//	 */
			//	Array.Clear(attrUse, 0, attrUse.Length);
			//	foreach (VertexElement element in vertexDeclaration.elements)
			//	{
			//		int usage = (int) element.VertexElementUsage;
			//		int index = element.UsageIndex;
			//		if (attrUse[usage, index])
			//		{
			//			index = -1;
			//			for (int j = 0; j < 10; j += 1)
			//			{
			//				if (!attrUse[usage, j])
			//				{
			//					index = j;
			//					break;
			//				}
			//			}

			//			if (index < 0)
			//			{
			//				throw new InvalidOperationException("Vertex usage collision!");
			//			}
			//		}

			//		attrUse[usage, index] = true;
			//		int attribLoc = MojoShader.MOJOSHADER_glGetVertexAttribLocation(
			//			XNAToGL.VertexAttribUsage[usage],
			//			index
			//		);
			//		if (attribLoc == -1)
			//		{
			//			// Stream not used!
			//			continue;
			//		}

			//		attributeEnabled[attribLoc] = true;
			//		VertexAttribute attr = attributes[attribLoc];
			//		IntPtr finalPtr = basePtr + element.Offset;
			//		bool normalized = XNAToGL.VertexAttribNormalized(element);
			//		if (attr.CurrentBuffer != 0 ||
			//		    attr.CurrentPointer != finalPtr ||
			//		    attr.CurrentFormat != element.VertexElementFormat ||
			//		    attr.CurrentNormalized != normalized ||
			//		    attr.CurrentStride != vertexDeclaration.VertexStride)
			//		{
			//			glVertexAttribPointer(
			//				attribLoc,
			//				XNAToGL.VertexAttribSize[(int) element.VertexElementFormat],
			//				XNAToGL.VertexAttribType[(int) element.VertexElementFormat],
			//				normalized,
			//				vertexDeclaration.VertexStride,
			//				finalPtr
			//			);
			//			attr.CurrentBuffer = 0;
			//			attr.CurrentPointer = finalPtr;
			//			attr.CurrentFormat = element.VertexElementFormat;
			//			attr.CurrentNormalized = normalized;
			//			attr.CurrentStride = vertexDeclaration.VertexStride;
			//		}

			//		attributeDivisor[attribLoc] = 0;
			//	}

			//	FlushGLVertexAttributes();

			//	ldVertexDeclaration = vertexDeclaration;
			//	ldPointer = ptr;
			//	ldEffect = currentEffect;
			//	ldTechnique = currentTechnique;
			//	ldPass = currentPass;
			//	effectApplied = false;
			//	ldBaseVertex = -1;
			//}

			//MojoShader.MOJOSHADER_glProgramReady();
			//MojoShader.MOJOSHADER_glProgramViewportInfo(_viewport.Width, _viewport.Height,
			//                                            Backbuffer.Width, Backbuffer.Height,
			//                                            renderTargetBound ? 1 : 0 // lol C#
			//);
		}

		private void DrawPrimitivesImpl(PrimitiveType primitiveType, int vertexStart,int primitiveCount)
		{
			GL.DrawArrays(
				Primitive[(int) primitiveType],
				vertexStart,
				PrimitiveVertices(primitiveType, primitiveCount));
		}

		private void DrawUserPrimitivesImpl(PrimitiveType primitiveType,
		                                IntPtr vertexData,
		                                int vertexOffset,
		                                int primitiveCount)
		{
			GL.DrawArrays(Primitive[(int) primitiveType],
			              vertexOffset,
			              PrimitiveVertices(primitiveType, primitiveCount));
		}

		//private void BindVertexBuffer(IGLBuffer buffer)
		//{
		//	//uint handle = (buffer as OpenGLBuffer).Handle;
		//	//if (handle != currentVertexBuffer)
		//	//{
		//	//	glBindBuffer(GLenum.GL_ARRAY_BUFFER, handle);
		//	//	currentVertexBuffer = handle;
		//	//}
		//}

		private static bool IsSet(int flags, int mask)
		{
			return (flags & mask) == mask;
		}

		public static int PrimitiveVertices(PrimitiveType primitiveType, int primitiveCount)
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

		public static bool VertexAttribNormalized(VertexElement element)
		{
			return (element.VertexElementUsage == VertexElementUsage.Color ||
			        element.VertexElementFormat == VertexElementFormat.NormalizedShort2 ||
			        element.VertexElementFormat == VertexElementFormat.NormalizedShort4);
		}

		private Rectangle _viewport;
		private bool _isScissorTestEnable;
		private Rectangle _scissorRectangle;

		private float _clearDepth;
		private int _clearStencil;
		private Vector4 _clearColor = Vector4.Zero;

		private uint _realBackbufferFBO;
		private uint _realBackbufferRBO;
		private bool renderTargetBound = false;
		private readonly bool _useCoreProfile;
		private readonly IntPtr _glContext;

		private static readonly GL.PrimitiveType[] Primitive =
		{
			GL.PrimitiveType.Triangles,
			GL.PrimitiveType.TriangleStrip,
			GL.PrimitiveType.Lines,
			GL.PrimitiveType.LineStrip,
			GL.PrimitiveType.Points,
		};

		private static readonly int[] VertexAttribSize =
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

		private static readonly GL.VertexAttribPointerType[] VertexAttribType =
		{
			GL.VertexAttribPointerType.Float, // VertexElementFormat.Single
			GL.VertexAttribPointerType.Float, // VertexElementFormat.Vector2
			GL.VertexAttribPointerType.Float, // VertexElementFormat.Vector3
			GL.VertexAttribPointerType.Float, // VertexElementFormat.Vector4
			GL.VertexAttribPointerType.UnsignedByte, // VertexElementFormat.Color
			GL.VertexAttribPointerType.UnsignedByte, // VertexElementFormat.Byte4
			GL.VertexAttribPointerType.Short, // VertexElementFormat.Short2
			GL.VertexAttribPointerType.Short, // VertexElementFormat.Short4
			GL.VertexAttribPointerType.Short, // VertexElementFormat.NormalizedShort2
			GL.VertexAttribPointerType.Short, // VertexElementFormat.NormalizedShort4
			GL.VertexAttribPointerType.HalfFloat, // VertexElementFormat.HalfVector2
			GL.VertexAttribPointerType.HalfFloat, // VertexElementFormat.HalfVector4
		};
	}
}