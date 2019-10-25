using System;
using Lunatics.SDLGL;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
	public sealed class OpenGL3FBOTextureTarget : OpenGLTextureTarget
	{
		public OpenGL3FBOTextureTarget(OpenGL3Renderer owner, bool addStencilBuffer)
			: base(owner, addStencilBuffer)
		{
			// no need to initialise d_previousFrameBuffer here, it will be
			// initialised in activate()

			InitialiseRenderTexture();
			
			// setup area and cause the initial texture to be generated.
			DeclareRenderSize(new Sizef(DEFAULT_SIZE, DEFAULT_SIZE));
		}
		
		// TODO: ...
		//OpenGL3FBOTextureTarget::~OpenGL3FBOTextureTarget()
		//{    
		//	glDeleteFramebuffers(1, &d_frameBuffer);
		//	
		//	if (d_usesStencil)
		//	{
		//		glDeleteRenderbuffers(1, &d_stencilBufferRBO);
		//	}
		//}

		// overrides from OpenGLRenderTarget
		public override void Activate()
		{
			// remember previously bound FBO to make sure we set it back
			// when deactivating
			OpenGL.GL.GetInteger(OpenGL.GetPName.GL_FRAMEBUFFER_BINDING, out d_previousFrameBuffer);

			// switch to rendering to the texture
			OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, d_frameBuffer);
			
			base.Activate();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			
			// switch back to rendering to the previously bound framebuffer
			OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, d_previousFrameBuffer);
		}

		// implementation of TextureTarget interface

		public override void Clear()
		{
			var sz = d_area.Size;
			// Some drivers crash when clearing a 0x0 RTT. This is a workaround for
			// those cases.
			if (sz.Width < 1.0f || sz.Height < 1.0f)
				return;

			// save old clear colour
			var old_col = new float[4];
			OpenGL.GL.GetFloat(OpenGL.GetPName.GL_COLOR_CLEAR_VALUE, old_col);

			// remember previously bound FBO to make sure we set it back
			OpenGL.GL.GetInteger(OpenGL.GetPName.GL_FRAMEBUFFER_BINDING, out var previousFBO);

			// switch to our FBO
			OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, d_frameBuffer);
			// Clear it.
			d_glStateChanger.Disable(OpenGL.EnableCap.ScissorTest);
			OpenGL.GL.ClearColor(0, 0, 0, 0);

			if (!d_usesStencil)
				OpenGL.GL.Clear(OpenGL.ClearBufferMask.ColorBufferBit);
			else
				OpenGL.GL.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.StencilBufferBit);

			// switch back to rendering to the previously bound FBO
			OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, previousFBO);

			// restore previous clear colour
			OpenGL.GL.ClearColor(old_col[0], old_col[1], old_col[2], old_col[3]);
		}

		public override void DeclareRenderSize(Sizef sz)
		{
			SetArea(new Rectf(d_area.Position, Owner.GetAdjustedTextureSize(sz)));
			ResizeRenderTexture();
		}

		// specialise functions from OpenGLTextureTarget
		public override void GrabTexture()
		{
			OpenGL.GL.DeleteFrameBuffers(1, ref d_frameBuffer);
			d_frameBuffer = 0;

			base.GrabTexture();
		}

		public override void RestoreTexture()
		{
			base.RestoreTexture();

			InitialiseRenderTexture();
			ResizeRenderTexture();
		}

		//! default size of created texture objects
		private const float DEFAULT_SIZE = 128.0f;

		//! allocate and set up the texture used with the FBO.
		private void InitialiseRenderTexture()
		{
			// save old texture binding
			OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out var old_tex);

			// remember previously bound FBO-s to make sure we set them back
			var previousFBO_read = -1;
			var previousFBO_draw = -1;
			var previousFBO = -1;
			
			//if (OpenGLInfo::getSingleton().isSeperateReadAndDrawFramebufferSupported())
			{
				OpenGL.GL.GetInteger(OpenGL.GetPName.GL_READ_FRAMEBUFFER_BINDING, out previousFBO_read);
				OpenGL.GL.GetInteger(OpenGL.GetPName.GL_DRAW_FRAMEBUFFER_BINDING, out previousFBO_draw);
			}
			//else
			//{
			//	var isUsingOpenglEs = false /*OpenGLInfo::getSingleton().isUsingOpenglEs()*/;
			//	OpenGL.GL.GetInteger(isUsingOpenglEs
			//		                     ? OpenGL.GetPName.GL_FRAMEBUFFER_BINDING
			//		                     : OpenGL.GetPName.GL_FRAMEBUFFER_BINDING_EXT,
			//	                     out previousFBO);
			//}

			// create FBO
			OpenGL.GL.GenFrameBuffers(1, out d_frameBuffer);

			OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, d_frameBuffer);

			// set up the texture the FBO will draw to
			OpenGL.GL.GenTextures(1, out d_texture);
			d_glStateChanger.BindTexture(OpenGL.TextureTarget.Texture2D, d_texture);
			OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, (int)OpenGL.TextureMagFilter.Linear);
			OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, (int)OpenGL.TextureMinFilter.Linear);
			OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, (int)OpenGL.TextureWrapMode.ClampToEdge);
			OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, (int) OpenGL.TextureWrapMode.ClampToEdge);

			OpenGL.GL.TexImage2D(OpenGL.TextureTarget.Texture2D,
			                     0,
			                     OpenGL.PixelInternalFormat.Rgba8,
								 //OpenGLInfo::getSingleton().isSizedInternalFormatSupported() ? OpenGL.PixelInternalFormat.Rgba8 : OpenGL.PixelInternalFormat.Rgba,
			                     (int) DEFAULT_SIZE,
			                     (int) DEFAULT_SIZE,
			                     0,
			                     OpenGL.PixelFormat.Rgba,
			                     OpenGL.PixelType.UnsignedByte,
			                     IntPtr.Zero);

			OpenGL.GL.FramebufferTexture2D(OpenGL.FramebufferTarget.Framebuffer,
			                               OpenGL.FramebufferAttachment.ColorAttachment0,
			                               OpenGL.TextureTarget.Texture2D,
			                               d_texture,
			                               0);

			if (d_usesStencil)
			{
				// Set up the stencil buffer for the FBO
				OpenGL.GL.GenRenderbuffers(1, out d_stencilBufferRBO);
				OpenGL.GL.BindRenderbuffer(OpenGL.RenderbufferTarget.Renderbuffer, d_stencilBufferRBO);
				OpenGL.GL.RenderbufferStorage(OpenGL.RenderbufferTarget.Renderbuffer,
				                              OpenGL.RenderbufferStorage.StencilIndex8,
				                              (int) DEFAULT_SIZE,
				                              (int) DEFAULT_SIZE);
				OpenGL.GL.FramebufferRenderbuffer(OpenGL.FramebufferTarget.Framebuffer,
				                                  OpenGL.FramebufferAttachment.StencilAttachment,
				                                  OpenGL.RenderbufferTarget.Renderbuffer,
				                                  d_stencilBufferRBO);
			}

			//Check for framebuffer completeness
			CheckFramebufferStatus();

			// switch from our frame buffers back to the previously bound buffers.
			//if (OpenGLInfo::getSingleton().isSeperateReadAndDrawFramebufferSupported())
			{
				OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.ReadFramebuffer, previousFBO_read);
				OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.DrawFrameBuffer, previousFBO_draw);
			}
			//else
			//{
			//	OpenGL.GL.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, previousFBO);
			//}

			// ensure the CEGUI::Texture is wrapping the gl texture and has correct size
			d_CEGUITexture.SetOpenGLTexture(d_texture, d_area.Size);

			// restore previous texture binding.
			d_glStateChanger.BindTexture(OpenGL.TextureTarget.Texture2D, old_tex);
		}

		//! resize the texture
		private void ResizeRenderTexture()
		{
			// save old texture binding
			OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out var old_tex);

			// Some drivers (hint: Intel) segfault when glTexImage2D is called with
			// any of the dimensions being 0. The downside of this workaround is very
			// small. We waste a tiny bit of VRAM on cards with sane drivers and
			// prevent insane drivers from crashing CEGUI.
			Sizef sz = d_area.Size;
			if (sz.Width < 1.0f || sz.Height < 1.0f)
			{
				sz.Width = 1.0f;
				sz.Height = 1.0f;
			}

			// set the texture to the required size
			d_glStateChanger.BindTexture(OpenGL.TextureTarget.Texture2D, d_texture);

			OpenGL.GL.TexImage2D(OpenGL.TextureTarget.Texture2D,
			                     0,
			                     OpenGL.PixelInternalFormat.Rgba8, //OpenGLInfo::getSingleton().isSizedInternalFormatSupported() ? GL_RGBA8 : GL_RGBA,
			                     (int)sz.Width,
			                     (int)sz.Height,
			                     0,
								 OpenGL.PixelFormat.Rgba,
								 OpenGL.PixelType.UnsignedByte,
			                     IntPtr.Zero);

			if (d_usesStencil)
			{
				OpenGL.GL.BindRenderbuffer(OpenGL.RenderbufferTarget.Renderbuffer, d_stencilBufferRBO);
				OpenGL.GL.RenderbufferStorage(OpenGL.RenderbufferTarget.Renderbuffer,
				                              OpenGL.RenderbufferStorage.StencilIndex8,
				                              (int) sz.Width,
				                              (int) sz.Height);
			}

			Clear();

			// ensure the CEGUI::Texture is wrapping the gl texture and has correct size
			d_CEGUITexture.SetOpenGLTexture(d_texture, sz);

			// restore previous texture binding.
			d_glStateChanger.BindTexture(OpenGL.TextureTarget.Texture2D, old_tex);
		}

		//! Checks for OpenGL framebuffer completeness
		private void CheckFramebufferStatus()
		{
			var status = OpenGL.GL.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer);

			// Check for completeness
			if (status != OpenGL.FramebufferErrorCode.FramebufferComplete)
			{
				var stringStream = "OpenGL3Renderer: Error - The Framebuffer is incomplete: ";

				switch (status)
				{
					case OpenGL.FramebufferErrorCode.FramebufferIncompleteAttachment:
						stringStream += "GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferUndefined:
						stringStream += "GL_FRAMEBUFFER_UNDEFINED\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
						stringStream += "GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
						stringStream += "GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferIncompleteReadBuffer:
						stringStream += "GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferIncompleteMultisample:
						stringStream += "GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE\n";
						break;
					case OpenGL.FramebufferErrorCode.FramebufferUnsupported:
						stringStream += "GL_FRAMEBUFFER_UNSUPPORTED\n";
						break;
					default:
						stringStream += "Undefined Framebuffer error\n";
						break;
				}

				global::System.Diagnostics.Debug.WriteLine(stringStream);
				//if (CEGUI::Logger * logger = CEGUI::Logger::getSingletonPtr())
				//	logger->logEvent(stringStream.str().c_str());
				//else
				//	std::cerr << stringStream.str() << std::endl;
			}
		}
		
		//! Frame buffer object.
		private int/*GLuint*/ d_frameBuffer;

		//! Stencil buffer renderbuffer object
		private int/*GLuint*/ d_stencilBufferRBO;

		//! Frame buffer object that was bound before we bound this one
		private int/*GLuint*/ d_previousFrameBuffer;

		//! OpenGL state changer
		private OpenGLBaseStateChangeWrapper d_glStateChanger;
	}
}