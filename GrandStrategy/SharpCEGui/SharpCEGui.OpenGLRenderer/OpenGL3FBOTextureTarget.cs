using System;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
	public sealed class OpenGL3FBOTextureTarget : OpenGLTextureTarget
	{
		private const float DEFAULT_SIZE = 128.0f;
		
		public OpenGL3FBOTextureTarget(OpenGL3Renderer owner, bool addStencilBuffer)
			: base(owner, addStencilBuffer)
		{
			// no need to initialise d_previousFrameBuffer here, it will be
			// initialised in activate()

			initialiseRenderTexture();
			
			// setup area and cause the initial texture to be generated.
			base.DeclareRenderSize(Sizef(DEFAULT_SIZE, DEFAULT_SIZE));
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
			glGetIntegerv(GL_FRAMEBUFFER_BINDING, reinterpret_cast<GLint*>(&d_previousFrameBuffer));
			
			// switch to rendering to the texture
			glBindFramebuffer(GL_FRAMEBUFFER, d_frameBuffer);
			
			OpenGLTextureTarget::activate();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		}

		// implementation of TextureTarget interface

		public override void Clear()
		{
			throw new NotImplementedException();
		}

		public override void DeclareRenderSize(Sizef sz)
		{
			SetArea(Rectf(d_area.getPosition(), d_owner.getAdjustedTextureSize(sz)));
			ResizeRenderTexture();
		}

		// specialise functions from OpenGLTextureTarget
		public override void GrabTexture()
		{
			base.GrabTexture();
		}

		public override void RestoreTexture()
		{
			base.RestoreTexture();
		}

		//protected:
		//	//! default size of created texture objects
		//	static const float DEFAULT_SIZE;

		//	//! allocate and set up the texture used with the FBO.
		//	void initialiseRenderTexture();
		//	//! resize the texture
		//	void resizeRenderTexture();
		//	//! Checks for OpenGL framebuffer completeness
		//	void checkFramebufferStatus();

		//	//! Frame buffer object.
		//	GLuint d_frameBuffer;
		//	//! Stencil buffer renderbuffer object
		//	GLuint d_stencilBufferRBO;
		//	//! Frame buffer object that was bound before we bound this one
		//	GLuint d_previousFrameBuffer;
		//	//! OpenGL state changer
		//	OpenGLBaseStateChangeWrapper* d_glStateChanger;
	}
}