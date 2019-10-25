using System;
using SharpCEGui.Base;

#if __MACOS__
using OpenGL;
using Icehole.OpenGL;
#else
using Lunatics.SDLGL;
using OpenTK.Graphics.OpenGL;
#endif


namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGLFBOTextureTarget - allows rendering to an OpenGL texture via FBO.
    /// </summary>
    public sealed class OpenGLFBOTextureTarget : OpenGLTextureTarget
    {
        public OpenGLFBOTextureTarget(OpenGLRenderer owner)
                : base(owner)
        {
            if (!OpenGLRenderer.GLEW_EXT_framebuffer_object)
                throw new InvalidRequestException("Hardware does not support FBO");

            // no need to initialise d_previousFrameBuffer here, it will be
            // initialised in activate()

            InitialiseRenderTexture();

            // setup area and cause the initial texture to be generated.
            DeclareRenderSize(new Sizef(DefaultSize, DefaultSize));
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
#if __MACOS__
					GL.DeleteFramebuffers(1, ref d_frameBuffer);
#else
                    OpenGL.GL.Ext.DeleteFramebuffers(1, ref d_frameBuffer);
#endif
                }
                base.Dispose(disposing);
            }
        }

        // overrides from OpenGLRenderTarget
        public override void Activate()
        {
            // remember previously bound FBO to make sure we set it back when deactivating
            GL.GetInteger(GetPName.FramebufferBindingExt, out d_previousFrameBuffer);

            // switch to rendering to the texture
#if __MACOS__
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);
#else
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);
#endif

            base.Activate();
        }
        
        public override void Deactivate()
        {
            base.Deactivate();

			// switch back to rendering to the previously bound framebuffer
#if __MACOS__
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, d_previousFrameBuffer);
#else
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, d_previousFrameBuffer);
#endif

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
            GL.GetFloat(GetPName.ColorClearValue, old_col);

            // remember previously bound FBO to make sure we set it back
            int previousFBO;
            GL.GetInteger(GetPName.FramebufferBindingExt, out previousFBO);

            // switch to our FBO
#if __MACOS__
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);
#else
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);
#endif
            
            // Clear it.
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            // switch back to rendering to the previously bound FBO
#if __MACOS__
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, previousFBO);
#else
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, previousFBO);
#endif

            // restore previous clear colour
            GL.ClearColor(old_col[0], old_col[1], old_col[2], old_col[3]);
        }

        public override void DeclareRenderSize(Sizef sz)
        {
            SetArea(new Rectf(d_area.Position, Owner.GetAdjustedTextureSize(sz)));
            ResizeRenderTexture();
        }

        // specialise functions from OpenGLTextureTarget
        public override void GrabTexture()
        {
#if __MACOS__
			GL.DeleteFramebuffers(1, ref d_frameBuffer);
#else
            GL.Ext.DeleteFramebuffers(1, ref d_frameBuffer);
#endif
            d_frameBuffer = 0;

            base.GrabTexture();
        }
        
        public override void RestoreTexture()
        {
            base.RestoreTexture();

            InitialiseRenderTexture();
            ResizeRenderTexture();
        }

        //! allocate and set up the texture used with the FBO.
        private void InitialiseRenderTexture()
        {
            // save old texture binding
            int old_tex;
            GL.GetInteger(GetPName.TextureBinding2D, out old_tex);

#if __MACOS__
			// create FBO
			GL.GenFramebuffers(1, out d_frameBuffer);

			// remember previously bound FBO to make sure we set it back
			int previousFBO = 0;
			GL.GetInteger(GetPName.FramebufferBindingExt, out previousFBO);

			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);

			// set up the texture the FBO will draw to
			GL.GenTextures(1, out d_texture);
            GL.BindTexture(TextureTarget.Texture2D, d_texture);
			GL.TexParameter(TextureTarget.Texture2D, 
                            TextureParameterName.TextureMagFilter, 
                            /*(int)All.Linear*/0x2601);
            GL.TexParameter(TextureTarget.Texture2D, 
                            TextureParameterName.TextureMinFilter, 
                            /*(int)All.Linear*/0x2601);
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
						   (int)DefaultSize, (int)DefaultSize, 0,
			               PixelFormat.Rgba,
			               PixelType.UnsignedByte,
			               IntPtr.Zero);
			GL.FramebufferTexture2D (FramebufferTarget.FramebufferExt,
			                         FramebufferAttachment.ColorAttachment0Ext, 
			                         TextureTarget.Texture2D, 
			                         d_texture, 0);

			// TODO: Check for completeness and then maybe try some alternative stuff?

			// switch from our frame buffer back to the previously bound buffer.
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, previousFBO);

			// ensure the CEGUI::Texture is wrapping the gl texture and has correct size
			d_CEGUITexture.SetOpenGLTexture(d_texture, d_area.Size);

			// restore previous texture binding.
			GL.BindTexture(TextureTarget.Texture2D, old_tex);
#else
            // create FBO
            GL.Ext.GenFramebuffers(1, out d_frameBuffer);

            // remember previously bound FBO to make sure we set it back
            int previousFbo = 0;
            GL.GetInteger(GetPName.FramebufferBindingExt, out previousFbo);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, d_frameBuffer);

            // set up the texture the FBO will draw to
            GL.GenTextures(1, out d_texture);
            GL.BindTexture(TextureTarget.Texture2D, d_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                          (int) DefaultSize, (int) DefaultSize, 0,
                          PixelFormat.Rgba,
                          PixelType.UnsignedByte,
                          IntPtr.Zero);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                                        FramebufferAttachment.ColorAttachment0Ext,
                                        TextureTarget.Texture2D,
                                        d_texture, 0);

            // TODO: Check for completeness and then maybe try some alternative stuff?

            // switch from our frame buffer back to the previously bound buffer.
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, previousFbo);

            // ensure the CEGUI::Texture is wrapping the gl texture and has correct size
            d_CEGUITexture.SetOpenGLTexture(d_texture, d_area.Size);

            // restore previous texture binding.
            GL.BindTexture(TextureTarget.Texture2D, old_tex);
#endif
        }
        
        //! resize the texture
        private void ResizeRenderTexture()
        {
            // save old texture binding
            int oldTex;
            GL.GetInteger(GetPName.TextureBinding2D, out oldTex);

            // Some drivers (hint: Intel) segfault when glTexImage2D is called with
            // any of the dimensions being 0. The downside of this workaround is very
            // small. We waste a tiny bit of VRAM on cards with sane drivers and
            // prevent insane drivers from crashing CEGUI.
            var sz = d_area.Size;
            if (sz.Width < 1.0f || sz.Height < 1.0f)
            {
                sz.Width = 1.0f;
                sz.Height = 1.0f;
            }

#if __MACOS__
				// set the texture to the required size
			GL.BindTexture(TextureTarget.Texture2D, d_texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0,
			              PixelInternalFormat.Rgba,
			              (int) sz.Width, (int) sz.Height, 0,
			              PixelFormat.Rgba,
			              PixelType.UnsignedByte, 
			              IntPtr.Zero);
			Clear();

			// ensure the CEGUI::Texture is wrapping the gl texture and has correct size
			d_CEGUITexture.SetOpenGLTexture(d_texture, sz);

			// restore previous texture binding.
			GL.BindTexture(TextureTarget.Texture2D, oldTex);
#else
            // set the texture to the required size
            GL.BindTexture(TextureTarget.Texture2D, d_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                          PixelInternalFormat.Rgba,
                          (int) sz.Width, (int) sz.Height, 0,
                          PixelFormat.Rgba,
                          PixelType.UnsignedByte,
                          IntPtr.Zero);
            Clear();

            // ensure the CEGUI::Texture is wrapping the gl texture and has correct size
            d_CEGUITexture.SetOpenGLTexture(d_texture, sz);

            // restore previous texture binding.
            GL.BindTexture(TextureTarget.Texture2D, oldTex);
#endif
        }

        #region Fields

        /// <summary>
        /// Frame buffer object.
        /// </summary>
        private int d_frameBuffer;
        
        /// <summary>
        /// Frame buffer object that was bound before we bound this one
        /// </summary>
        private int d_previousFrameBuffer;

        /// <summary>
        /// default size of created texture objects
        /// </summary>
        private const float DefaultSize = 128.0f;

        #endregion
    }
}