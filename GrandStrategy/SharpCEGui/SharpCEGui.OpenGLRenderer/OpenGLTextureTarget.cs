using System;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGLTextureTarget - Common base class for all OpenGL render targets
    /// based on some form of RTT support.
    /// </summary>
    public abstract class OpenGLTextureTarget : OpenGLRenderTarget, ITextureTarget
    {
        protected OpenGLTextureTarget(OpenGLRendererBase owner, bool addStencilBuffer) 
	        : base(owner)
        {
            d_texture = 0;
            d_usesStencil = addStencilBuffer;
            CreateCEGUITexture();
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Owner.DestroyTexture(d_CEGUITexture);
                }
                base.Dispose(disposing);
            }
        }
        
        public override bool IsImageryCache()
        {
            return true;
        }
        
        public Texture GetTexture()
        {
            return d_CEGUITexture;
        }
        
        public bool IsRenderingInverted()
        {
            return true;
        }

        public abstract void Clear();

        public abstract void DeclareRenderSize(Sizef sz);

        public bool GetUsesStencil()
        {
            return d_usesStencil;
        }
        
        public virtual void GrabTexture()
        {
            if (d_CEGUITexture!=null)
            {
                Owner.DestroyTexture(d_CEGUITexture);
                d_texture = 0;
                d_CEGUITexture = null;
            }
        }

        public virtual void RestoreTexture()
        {
            if (d_CEGUITexture==null)
                CreateCEGUITexture();
        }

        /// <summary>
        /// helper to generate unique texture names
        /// </summary>
        /// <returns></returns>
        protected static String GenerateTextureName()
        {
            return "_ogl_tt_tex_" + (TextureNumber++);
        }
        
        /// <summary>
        /// helper to create CEGUI::Texture d_CEGUITexture;
        /// </summary>
        protected void CreateCEGUITexture()
        {
            d_CEGUITexture = (OpenGLTexture) Owner.CreateTexture(GenerateTextureName(), d_texture, d_area.Size);
        }

        #region Fields

        /// <summary>
        /// Determines if the instance has a stencil buffer attached or not
        /// </summary>
        protected bool d_usesStencil;

        /// <summary>
        /// Associated OpenGL texture ID
        /// </summary>
        protected int d_texture;
        
        /// <summary>
        /// we use this to wrap d_texture so it can be used by the core CEGUI lib.
        /// </summary>
        protected OpenGLTexture d_CEGUITexture;

        /// <summary>
        /// static data used for creating texture names
        /// </summary>
        protected static uint TextureNumber;

        #endregion
    }
}
