using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGLWGLPBTextureTarget - allows rendering to an OpenGL texture via the
    /// pbuffer WGL extension.
    /// </summary>
    public class OpenGLWGLPBTextureTarget : OpenGLTextureTarget
    {
        public OpenGLWGLPBTextureTarget(OpenGLRenderer owner) : base(owner)
        {
            throw new NotImplementedException();
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void DeclareRenderSize(Sizef sz)
        {
            throw new NotImplementedException();
        }

        public override void GrabTexture()
        {
            throw new NotImplementedException();
        }

        public override void RestoreTexture()
        {
            throw new NotImplementedException();
        }

        //! default size of created texture objects
        protected const float DEFAULT_SIZE = 128f;

        //! Initialise the PBuffer with the needed size
        protected void InitialisePBuffer()
        {
            throw new NotImplementedException();
        }

        //! Cleans up the pbuffer resources.
        protected void ReleasePBuffer()
        {
            throw new NotImplementedException();
        }

        //! Switch rendering so it targets the pbuffer
        protected void EnablePBuffer()
        {
            throw new NotImplementedException();
        }

        //! Switch rendering to target what was active before the pbuffer was used.
        protected void DisablePBuffer()
        {
            throw new NotImplementedException();
        }

        //! Perform basic initialisation of the texture we're going to use.
        protected void InitialiseTexture()
        {
            throw new NotImplementedException();
        }

        //! Holds the pixel format we use when creating the pbuffer.
        protected int d_pixfmt;
        
        //! Handle to the pbuffer itself.
        protected IntPtr /*HPBUFFERARB*/ d_pbuffer;
        
        //! Handle to the rendering context for the pbuffer.
        protected IntPtr /*HGLRC*/ d_context;
        
        //! Handle to the Windows device context for the pbuffer.
        protected IntPtr /*HDC*/ d_hdc;
        
        //! Handle to the rendering context in use when we switched to the pbuffer.
        protected IntPtr /*HGLRC*/ d_prevContext;
     
        //! Handle to the device context in use when we switched to the pbuffer.
        protected IntPtr /*HDC*/ d_prevDC;
    }
}
