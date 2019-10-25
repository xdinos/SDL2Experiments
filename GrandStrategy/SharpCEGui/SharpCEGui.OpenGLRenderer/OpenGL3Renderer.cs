using System;
using Lunatics.SDLGL;
using SharpCEGui.Base;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
	//! ~OpenGL3FBOTextureTarget - allows rendering to an OpenGL texture via FBO.

	/// <summary>
	/// Renderer class to interface with OpenGL
	/// </summary>
	public class OpenGL3Renderer : OpenGLRendererBase
    {
        #region Bootstraping Methods

        /*!
        \brief
            Convenience function that creates the required objects to initialise the
            CEGUI system.

            The created Renderer will use the current OpenGL viewport as it's
            default surface size.

            This will create and initialise the following objects for you:
            - CEGUI::OpenGL3Renderer
            - CEGUI::DefaultResourceProvider
            - CEGUI::System

        \param abi
            This must be set to CEGUI_VERSION_ABI

        \return
            Reference to the CEGUI::OpenGL3Renderer object that was created.
        */

        public static OpenGL3Renderer BootstrapSystem(/*const int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            if (Base.System.GetSingleton() == null)
                throw new InvalidRequestException("CEGUI::System object is already initialised.");

            var renderer = Create();
            Base.System.Create(renderer, new DefaultResourceProvider());

            return renderer;
        }

        /// <summary>
        /// Convenience function that creates the required objects to initialise the
        /// CEGUI system.
        /// 
        /// The created Renderer will use the current OpenGL viewport as it's
        /// default surface size.
        /// 
        /// This will create and initialise the following objects for you:
        /// - CEGUI::OpenGL3Renderer
        /// - CEGUI::DefaultResourceProvider
        /// - CEGUI::System
        /// </summary>
        /// <param name="displaySize">
        /// Size object describing the initial display resolution.
        /// </param>
        /// <param name="abi">
        /// This must be set to CEGUI_VERSION_ABI
        /// </param>
        /// <returns>
        /// Reference to the CEGUI::OpenGL3Renderer object that was created.
        /// </returns>
        public static OpenGL3Renderer BootstrapSystem(Sizef displaySize /*, const int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            if (Base.System.GetSingleton() == null)
                throw new InvalidRequestException("CEGUI::System object is already initialised.");

            var renderer = Create(displaySize);
            Base.System.Create(renderer, new DefaultResourceProvider());

            return renderer;
        }

        /// <summary>
        /// Convenience function to cleanup the CEGUI system and related objects
        /// that were created by calling the bootstrapSystem function.
        /// 
        /// This function will destroy the following objects for you:
        /// - CEGUI::System
        /// - CEGUI::DefaultResourceProvider
        /// - CEGUI::OpenGL3Renderer
        /// </summary>
        /// <remarks>
        /// If you did not initialise CEGUI by calling the bootstrapSystem function,
        /// you should \e not call this, but rather delete any objects you created
        /// manually.
        /// </remarks>
        public static void DestroySystem()
        {
            var sys = Base.System.GetSingleton();
            if (sys == null)
                throw new InvalidRequestException("CEGUI::System object is not created or was already destroyed.");

            var renderer = (OpenGL3Renderer) sys.GetRenderer();
            var resourceProvider = (DefaultResourceProvider)sys.GetResourceProvider();

            Base.System.Destroy();
            // TODO: delete resourceProvider;
            Destroy(renderer);
        }
        
        /// <summary>
        /// Create an OpenGL3Renderer object.
        /// </summary>
        /// <param name="abi">
        /// This must be set to CEGUI_VERSION_ABI
        /// </param>
        /// <returns></returns>
        public static OpenGL3Renderer Create(/*const int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);
            return new OpenGL3Renderer();
        }
        
        /// <summary>
        /// Create an OpenGL3Renderer object.
        /// </summary>
        /// <param name="displaySize">
        /// Size object describing the initial display resolution.
        /// </param>
        /// <param name="abi">
        /// This must be set to CEGUI_VERSION_ABI
        /// </param>
        /// <returns></returns>
        public static OpenGL3Renderer Create(Sizef displaySize /*,const int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);
            return new OpenGL3Renderer(displaySize);
        }

        /// <summary>
        /// Destroy an OpenGL3Renderer object.
        /// </summary>
        /// <param name="renderer">
        /// The OpenGL3Renderer object to be destroyed.
        /// </param>
        public static void Destroy(OpenGL3Renderer renderer)
        {
            renderer.Dispose();
        }

        #endregion

        /// <summary>
        /// Helper to get the wrapper used to check for redundant OpenGL state changes.
        /// </summary>
        /// <returns>
        /// The active OpenGL state change wrapper object.
        /// </returns>
        public OpenGLBaseStateChangeWrapper GetOpenGLStateChanger()
        {
            return d_openGLStateChanger;
        }

        public override void BeginRendering()
        {
            // if enabled, restores a subset of the GL state back to default values.
            if (d_initExtraStates)
                SetupExtraStates();

            d_openGLStateChanger.Reset();

            // Setup initial states
            d_openGLStateChanger.Enable(OpenGL.EnableCap.Blend);

            // force set blending ops to get to a known state.
            SetupRenderingBlendMode(BlendMode.Normal, true);
        }

        public override void EndRendering()
        {
            
        }

        public override Sizef GetAdjustedTextureSize(Sizef sz)
        {
            return sz;
        }

        public override bool IsS3TCSupported()
        {
            throw new NotImplementedException();
        }

        public override void SetupRenderingBlendMode(BlendMode mode, bool force = false)
        {
            // exit if mode is already set up (and update not forced)
            if ((d_activeBlendMode == mode) && !force)
                return;

            d_activeBlendMode = mode;

            if (d_activeBlendMode == BlendMode.RttPremultiplied)
            {
                d_openGLStateChanger.BlendFunc(OpenGL.BlendingFactorSrc.One, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
            }
            else
            {
                d_openGLStateChanger.BlendFuncSeparate(OpenGL.BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha,
                                                       OpenGL.BlendingFactorSrc.OneMinusDstAlpha, OpenGL.BlendingFactorDest.One);
            }
        }

        public override RenderMaterial CreateRenderMaterial(DefaultShaderType shaderType)
        {
            throw new NotImplementedException();
        }

        protected override OpenGLGeometryBufferBase CreateGeometryBufferImpl(RenderMaterial renderMaterial)
        {
            return new OpenGL3GeometryBuffer(this, renderMaterial);
        }

        protected override ITextureTarget CreateTextureTargetImpl(bool addStencilBuffer)
        {
            throw new NotImplementedException();
            //return d_textureTargetFactory.Create(this);
        }

        protected override OpenGLTexture CreateTextureImpl(string name)
        {
            throw new NotImplementedException();
        }

        protected void InitialiseRendererIDString()
        {
            d_rendererID = "CEGUI::OpenGL3Renderer - Official OpenGL 3.2 core based renderer module.";
        }

        /// <summary>
        /// Constructor for OpenGL Renderer objects
        /// </summary>
        protected OpenGL3Renderer()
        {
            d_shaderWrapperTextured = null;
            d_openGLStateChanger = null;
            d_shaderManager = null;

            InitialiseRendererIDString();
            d_openGLStateChanger = new OpenGL3StateChangeWrapper();
            InitialiseTextureTargetFactory();
            InitialiseOpenGLShaders();
        }

        /// <summary>
        /// Constructor for OpenGL Renderer objects.
        /// </summary>
        /// <param name="displaySize">
        /// Size object describing the initial display resolution.
        /// </param>
        protected OpenGL3Renderer(Sizef displaySize)
                : base(displaySize)
        {
            d_shaderWrapperTextured = null;
            d_openGLStateChanger = null;
            d_shaderManager = null;

            InitialiseRendererIDString();
            d_openGLStateChanger = new OpenGL3StateChangeWrapper();
            InitialiseTextureTargetFactory();
            InitialiseOpenGLShaders();
        }

        /// <summary>
        /// Initialises the ShaderManager and the required OpenGL shaders
        /// </summary>
        protected void InitialiseOpenGLShaders()
        {
            // TODO: checkGLErrors();
            d_shaderManager = new OpenGLBaseShaderManager(d_openGLStateChanger, ShaderVersion.Glsl);
            d_shaderManager.InitialiseShaders();

            InitialiseStandardTexturedShaderWrapper();
            InitialiseStandardColouredShaderWrapper();
        }

        //! Initialises the OpenGL ShaderWrapper for textured objects
        protected void InitialiseStandardTexturedShaderWrapper()
        {
            throw new NotImplementedException();
        }

        //! Initialises the OpenGL ShaderWrapper for coloured objects
        protected void InitialiseStandardColouredShaderWrapper()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Destructor for OpenGL3Renderer objects
        */
        // TODO: virtual ~OpenGL3Renderer();

        /// <summary>
        /// initialise OGL3TextureTargetFactory that will generate TextureTargets
        /// </summary>
        protected void InitialiseTextureTargetFactory()
        {
			//Use OGL core implementation for FBOs
			d_rendererID += "  TextureTarget support enabled via FBO OGL 3.2 core implementation.";
			d_textureTargetFactory = (renderer, stencil) => new OpenGL3FBOTextureTarget((OpenGL3Renderer)renderer, stencil);
		}

        /// <summary>
        /// init the extra GL states enabled via enableExtraStateSettings
        /// </summary>
        protected void SetupExtraStates()
        {
            OpenGL.GL.ActiveTexture(OpenGL.TextureUnit.Texture0);

            OpenGL.GL.PolygonMode(OpenGL.MaterialFace.FrontAndBack, OpenGL.PolygonMode.Fill);

            OpenGL.GL.Disable(OpenGL.EnableCap.CullFace);
            OpenGL.GL.Disable(OpenGL.EnableCap.DepthTest);

            OpenGL.GL.UseProgram(0);
            OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ElementArrayBuffer, 0);
            OpenGL.GL.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, 0);
        }

        #region Fields

        //! Wrapper of the OpenGL shader we will use for textured geometry
        protected OpenGLBaseShaderWrapper d_shaderWrapperTextured;
        
        //! Wrapper of the OpenGL shader we will use for solid geometry
        protected OpenGLBaseShaderWrapper d_shaderWrapperSolid;

        //! The wrapper we use for OpenGL calls, to detect redundant state changes and prevent them
        protected OpenGLBaseStateChangeWrapper d_openGLStateChanger;
        
        //! The ShaderManager  takes care of the creation of standard OpenGL Shaders and their deletion
        protected OpenGLBaseShaderManager d_shaderManager;
        
        //! whether S3TC texture compression is supported by the context
        protected bool d_s3tcSupported;

		//! pointer to a helper that creates TextureTargets supported by the system.
		// TODO: protected OGLTextureTargetFactory d_textureTargetFactory;
		private Func<OpenGLRendererBase, bool, OpenGLTextureTarget> d_textureTargetFactory;

		#endregion
    }
}
