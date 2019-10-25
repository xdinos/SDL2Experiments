﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lunatics.SDLGL;
using SharpCEGui.Base;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK;
//using OpenTK.Graphics.OpenGL;
//#endif


namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// Common base class used for other OpenGL based renderer modules.
    /// </summary>
    public abstract class OpenGLRendererBase : Renderer, IDisposable
    {
        // implement Renderer interface

        public override int TextureTargetsCount
        {
            get { return d_textureTargets.Count; }
        }

        public override IRenderTarget GetDefaultRenderTarget()
        {
            ThrowIfDisposed();

            return d_defaultTarget;
        }

        public override GeometryBuffer CreateGeometryBufferTextured(RenderMaterial renderMaterial)
        {
            var geometryBuffer = CreateGeometryBufferImpl(renderMaterial);

            geometryBuffer.AddVertexAttribute(VertexAttributeType.Position0);
            geometryBuffer.AddVertexAttribute(VertexAttributeType.Color0);
            geometryBuffer.AddVertexAttribute(VertexAttributeType.TexCoord0);
            geometryBuffer.FinaliseVertexAttributes();

            AddGeometryBuffer(geometryBuffer);
            return geometryBuffer;
        }

        public override GeometryBuffer CreateGeometryBufferColoured(RenderMaterial renderMaterial)
        {
            var geometryBuffer = CreateGeometryBufferImpl(renderMaterial);

            geometryBuffer.AddVertexAttribute(VertexAttributeType.Position0);
            geometryBuffer.AddVertexAttribute(VertexAttributeType.Color0);
            geometryBuffer.FinaliseVertexAttributes();

            AddGeometryBuffer(geometryBuffer);
            return geometryBuffer;
        }

        public override ITextureTarget CreateTextureTarget(bool addStencilBuffer)
        {
            ThrowIfDisposed();

            var t = CreateTextureTargetImpl(addStencilBuffer);
            if (t != null)
                d_textureTargets.Add(t);

            return t;
        }

        public override void DestroyTextureTarget(ITextureTarget target)
        {
            ThrowIfDisposed();

            if (d_textureTargets.Contains(target))
            {
                d_textureTargets.Remove(target);
                target.Dispose();
            }
        }

        public override void DestroyAllTextureTargets()
        {
            ThrowIfDisposed();

            while (d_textureTargets.Count != 0)
                DestroyTextureTarget(d_textureTargets[0]);
        }

        public override Texture CreateTexture(string name)
        {
            ThrowIfDisposed();
            ThrowIfTextureExists(name);

            var tex = CreateTextureImpl(name);
            tex.Initialise();
            d_textures[name] = tex;

            LogTextureCreation(name);

            return tex;
        }

        public override Texture CreateTexture(string name, string filename, string resourceGroup)
        {
            ThrowIfDisposed();
            ThrowIfTextureExists(name);

            var tex = CreateTextureImpl(name);
            tex.Initialise(filename, resourceGroup);
            d_textures[name] = tex;

            LogTextureCreation(name);

            return tex;
        }

        public override Texture CreateTexture(string name, Sizef size)
        {
            ThrowIfDisposed();
            ThrowIfTextureExists(name);

            var tex = CreateTextureImpl(name);
            tex.Initialise(size);
            d_textures[name] = tex;
            
            LogTextureCreation(name);
            
            return tex;
        }

        public override void DestroyTexture(Texture texture)
        {
            DestroyTexture(texture.GetName());
        }

        public override void DestroyTexture(string name)
        {
            ThrowIfDisposed();

            if (d_textures.ContainsKey(name))
            {
                LogTextureDestruction(name);
                d_textures[name].Dispose();
                d_textures.Remove(name);
            }
        }

        public override void DestroyAllTextures()
        {
            ThrowIfDisposed();

            while (d_textures.Count != 0)
                DestroyTexture(d_textures.First().Key);
        }

        public override Texture GetTexture(string name)
        {
            ThrowIfDisposed();

            if (!d_textures.ContainsKey(name))
                throw new UnknownObjectException("No texture named '" + name + "' is available.");

            return d_textures[name];
        }

        public override bool IsTextureDefined(string name)
        {
            ThrowIfDisposed();

            return d_textures.ContainsKey(name);
        }

        public override void SetDisplaySize(Sizef size)
        {
            ThrowIfDisposed();

            if (size != d_displaySize)
            {
                d_displaySize = size;

                // update the default target's area
                var area = d_defaultTarget.GetArea();
                area.Size = size;
                d_defaultTarget.SetArea(area);
            }
        }

        public override Sizef GetDisplaySize()
        {
            ThrowIfDisposed();

            return d_displaySize;
        }

        public override Lunatics.Mathematics.Vector2 GetDisplayDotsPerInch()
        {
            ThrowIfDisposed();

            return d_displayDPI;
        }

        public override int GetMaxTextureSize() 
        {
            ThrowIfDisposed();

            return d_maxTextureSize;
        }

        public override string GetIdentifierString()
        {
            ThrowIfDisposed();

            return d_rendererID;
        }

        /*!
        \brief
            Create a texture that uses an existing OpenGL texture with the specified
            size.  Note that it is your responsibility to ensure that the OpenGL
            texture is valid and that the specified size is accurate.

        \param sz
            Size object that describes the pixel size of the OpenGL texture
            identified by \a tex.

        \param name
            String holding the name for the new texture.  Texture names must be
            unique within the Renderer.

        \return
            Texture object that wraps the OpenGL texture \a tex, and whose size is
            specified to be \a sz.

        \exceptions
            - AlreadyExistsException - thrown if a Texture object named \a name
              already exists within the system.
        */
        public Texture CreateTexture(string name, int tex, Sizef sz)
        {
            ThrowIfDisposed();
            ThrowIfTextureExists(name);

            var t = CreateTextureImpl(name);
            t.Initialise(tex, sz);
            d_textures[name] = t;
            
            LogTextureCreation(name);
            
            return t;
        }

        /// <summary>
        /// Tells the renderer to initialise some extra states beyond what it
        /// directly needs itself for CEGUI.
        /// 
        /// This option is useful in cases where you've made changes to the default
        /// OpenGL state and do not want to save/restore those between CEGUI
        /// rendering calls.  Note that this option will not deal with every
        /// possible state or extension - if you want a state added here, make a
        /// request and we'll consider it ;)
        /// </summary>
        /// <param name="setting"></param>
        public void EnableExtraStateSettings(bool setting)
        {
            ThrowIfDisposed();
            d_initExtraStates = setting;
        }

        /// <summary>
        /// Grabs all the loaded textures from Texture RAM and stores them in a
        /// local data buffer.  This function invalidates all textures, and
        /// restoreTextures must be called before any CEGUI rendering is done for
        /// predictable results.
        /// </summary>
        public void GrabTextures()
        {
            ThrowIfDisposed();

            // perform grab operations for texture targets
            foreach (var target in d_textureTargets)
                ((OpenGLTextureTarget)target).GrabTexture();

            // perform grab on regular textures
            foreach(var texture in d_textures)
                texture.Value.GrabTexture();
        }

        /// <summary>
        /// Restores all the loaded textures from the local data buffers previously
        /// created by 'grabTextures'
        /// </summary>
        public void RestoreTextures()
        {
            // perform restore on textures
            foreach (var texture in d_textures)
                texture.Value.RestoreTexture();

            // perform restore operations for texture targets
            foreach (var target in d_textureTargets)
                ((OpenGLTextureTarget)target).RestoreTexture();
        }

        /// <summary>
        /// Helper to return a valid texture size according to reported OpenGL capabilities.
        /// </summary>
        /// <param name="sz">
        /// Size object containing input size.
        /// </param>
        /// <returns>
        /// Size object containing - possibly different - output size.
        /// </returns>
        public abstract Sizef GetAdjustedTextureSize(Sizef sz);

        /// <summary>
        /// Utility function that will return \a f if it's a power of two, or the
        /// next power of two up from \a f if it's not.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float GetNextPowerOfTwoSize(float f)
        {
            var size = (uint) f;

            // if not power of 2
            if ((size & (size - 1))==(size-1) || size==0)
            {
                int log = 0;

                // get integer log of 'size' to base 2
                while ((size >>= 1)!=0)
                    ++log;

                // use log to calculate value to use as size.
                size = (uint) (2 << log);
            }

            return (float)(size);
        }

        //! set the render states for the specified BlendMode.
        public abstract void SetupRenderingBlendMode(BlendMode mode, bool force = false);
        
        /// <summary>
        /// Return whether EXT_texture_compression_s3tc is supported
        /// </summary>
        /// <returns></returns>
        public abstract bool IsS3TCSupported();

        /// <summary>
        /// Helper to get the viewport.
        /// </summary>
        /// <returns>
        /// The viewport.
        /// </returns>
        public Rectf GetActiveViewPort()
        {
            return d_activeRenderTarget.GetArea();
        }
       
        protected OpenGLRendererBase()
        {
            d_displayDPI = new Lunatics.Mathematics.Vector2(96, 96);
            d_initExtraStates = false;
            d_activeBlendMode = BlendMode.Invalid;

            InitialiseMaxTextureSize();
            InitialiseDisplaySizeWithViewportSize();
            
            d_defaultTarget = new OpenGLViewportTarget(this);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="displaySize">
        /// Size object describing the initial display resolution.
        /// </param>
        protected OpenGLRendererBase(Sizef displaySize)
        {
            d_displaySize = displaySize;
            d_displayDPI = new Lunatics.Mathematics.Vector2(96f, 96f);
            d_initExtraStates = false;
            d_activeBlendMode = BlendMode.Invalid;

            InitialiseMaxTextureSize();
            
            d_defaultTarget = new OpenGLViewportTarget(this);
        }

        #region Implementation of IDisposable

        ~OpenGLRendererBase()
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
            if (!_disposed)
            {
                if (disposing)
                {
                    DestroyAllGeometryBuffers();
                    DestroyAllTextureTargets();
                    DestroyAllTextures();

                    d_defaultTarget.Dispose();
                }

                _disposed = true;
            }
        }

        private bool _disposed;

        #endregion
        
        /// <summary>
        /// helper to set (rough) max texture size.
        /// </summary>
        protected void InitialiseMaxTextureSize()
        {
            ThrowIfDisposed();
            
            OpenGL.GL.GetInteger(OpenGL.GetPName.MaxTextureSize, out d_maxTextureSize);
        }

        /// <summary>
        /// helper to set display size with current viewport size.
        /// </summary>
        protected void InitialiseDisplaySizeWithViewportSize()
        {
            ThrowIfDisposed();

            var vp = new int[4];
            OpenGL.GL.GetInteger(OpenGL.GetPName.Viewport, vp);
            d_displaySize = new Sizef((float) vp[2], (float) vp[3]);
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        protected void ThrowIfTextureExists(string name)
        {
            if (d_textures.ContainsKey(name))
                throw new AlreadyExistsException("A texture named '" + name + "' already exists.");
        }

        /// <summary>
        /// helper to safely log the creation of a named textured
        /// </summary>
        /// <param name="name"></param>
        protected static void LogTextureCreation(string name)
        {
            var logger = Base.System.GetSingleton().Logger;
            if (logger != null)
                logger.LogEvent("[OpenGLRenderer] Created texture: " + name);
        }

        /// <summary>
        /// helper to safely log the destruction of a named texture
        /// </summary>
        /// <param name="name"></param>
        protected static void LogTextureDestruction(string name)
        {
            var logger = Base.System.GetSingleton().Logger;
            if (logger != null)
                logger.LogEvent("[OpenGLRenderer] Destroyed texture: " + name);
        }

        /// <summary>
        /// return some appropriate OpenGLGeometryBufferBase subclass instance.
        /// </summary>
        /// <param name="renderMaterial"></param>
        /// <returns></returns>
        protected abstract OpenGLGeometryBufferBase CreateGeometryBufferImpl(RenderMaterial renderMaterial);

        /// <summary>
        /// return some appropriate TextureTarget subclass instance.
        /// </summary>
        /// <param name="addStencilBuffer"></param>
        /// <returns></returns>
        protected abstract ITextureTarget CreateTextureTargetImpl(bool addStencilBuffer);

        /// <summary>
        /// return some appropriate Texture subclass instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract OpenGLTexture CreateTextureImpl(string name);

        #region Fields

        /// <summary>
        /// String holding the renderer identification text.
        /// </summary>
        protected static String d_rendererID = "--- subclass did not set ID: Fix this!";
        
        /// <summary>
        /// What the renderer considers to be the current display size.
        /// </summary>
        protected Sizef d_displaySize;
        
        /// <summary>
        /// What the renderer considers to be the current display DPI resolution.
        /// </summary>
        protected Lunatics.Mathematics.Vector2 d_displayDPI;
        
        /// <summary>
        /// The default RenderTarget
        /// </summary>
        protected RenderTarget d_defaultTarget;
        
        ////! container type used to hold TextureTargets we create.
        //typedef std::vector<TextureTarget*> TextureTargetList;
        
        /// <summary>
        /// Container used to track texture targets.
        /// </summary>
        private readonly List<ITextureTarget> d_textureTargets = new List<ITextureTarget>();
        
        ////! container type used to hold GeometryBuffers created.
        //typedef std::vector<OpenGLGeometryBufferBase*> GeometryBufferList;
        
        //! Container used to track geometry buffers.
        private readonly List<OpenGLGeometryBufferBase> d_geometryBuffers = new List<OpenGLGeometryBufferBase>();
        
        //! container type used to hold Textures we create.
        //typedef std::map<String, OpenGLTexture*, StringFastLessCompare
        //                 CEGUI_MAP_ALLOC(String, OpenGLTexture*)> TextureMap;
        
        /// <summary>
        /// Container used to track textures.
        /// </summary>
        private readonly Dictionary<string, OpenGLTexture> d_textures=new Dictionary<string, OpenGLTexture>();
        
        /// <summary>
        /// What the renderer thinks the max texture size is.
        /// </summary>
        protected int d_maxTextureSize;
        
        /// <summary>
        /// option of whether to initialise extra states that may not be at default
        /// </summary>
        protected bool d_initExtraStates;
        
        /// <summary>
        /// What blend mode we think is active.
        /// </summary>
        protected BlendMode d_activeBlendMode;

        #endregion
    }
}