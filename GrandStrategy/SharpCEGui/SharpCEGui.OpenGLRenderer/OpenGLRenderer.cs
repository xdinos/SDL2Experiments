using System;
using Lunatics.SDLGL;
using SharpCEGui.Base;

#if __MACOS__
using OpenGL;
using Icehole.OpenGL;
#else
using OpenTK;
using OpenTK.Graphics.OpenGL;
#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// Renderer class to interface with OpenGL
    /// </summary>
    public sealed class OpenGLRenderer : OpenGLRendererBase
    {
        /// <summary>
        /// Enumeration of valid texture target types.
        /// </summary>
        public enum TextureTargetType
        {
            /// <summary>
            /// Automatically choose the best type available.
            /// </summary>
            TTT_AUTO,

            /// <summary>
            /// Use targets based on frame buffer objects if available, else none.
            /// </summary>
            TTT_FBO,
            
            /// <summary>
            /// Use targets based on pbuffer support if available, else none.
            /// </summary>
            TTT_PBUFFER,

            /// <summary>
            /// Disable texture targets.
            /// </summary>
            TTT_NONE
        }

        #region Bootstraping

        /// <summary>
        /// Convenience function that creates the required objects to initialise the
        /// CEGUI system.
        /// 
        /// The created Renderer will use the current OpenGL viewport as it's
        /// default surface size.
        /// 
        /// This will create and initialise the following objects for you:
        /// - CEGUI::OpenGLRenderer
        /// - CEGUI::DefaultResourceProvider
        /// - CEGUI::System
        /// </summary>
        /// <param name="textureTargetType">
        /// Specifies one of the TextureTargetType enumerated values indicating the
        /// desired TextureTarget type to be used.  Defaults to TTT_AUTO.
        /// </param>
        /// <param name="abi">This must be set to CEGUI_VERSION_ABI</param>
        /// <returns>
        /// Reference to the CEGUI::OpenGLRenderer object that was created.
        /// </returns>
        public static OpenGLRenderer BootstrapSystem(TextureTargetType textureTargetType = TextureTargetType.TTT_AUTO/*, int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            if (Base.System.GetSingleton() != null)
                throw new InvalidRequestException("CEGUI::System object is already initialised.");

            var renderer = Create(textureTargetType);
            Base.System.Create(renderer, new DefaultResourceProvider());

            return renderer;
        }

        /*!
        \brief
            Convenience function that creates the required objects to initialise the
            CEGUI system.

            The created Renderer will use the current OpenGL viewport as it's
            default surface size.

            This will create and initialise the following objects for you:
            - CEGUI::OpenGLRenderer
            - CEGUI::DefaultResourceProvider
            - CEGUI::System

        \param display_size
            Size object describing the initial display resolution.

        \param tt_type
            Specifies one of the TextureTargetType enumerated values indicating the
            desired TextureTarget type to be used.  Defaults to TTT_AUTO.

        \param abi
            This must be set to CEGUI_VERSION_ABI

        \return
            Reference to the CEGUI::OpenGLRenderer object that was created.
        */

        public static OpenGLRenderer BootstrapSystem(Sizef displaySize, TextureTargetType textureTargetType = TextureTargetType.TTT_AUTO/*, int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            if (Base.System.GetSingleton() != null)
                throw new InvalidRequestException("CEGUI::System object is already initialised.");

            var renderer = Create(displaySize, textureTargetType);
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
        /// - CEGUI::OpenGLRenderer
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

            var renderer = (OpenGLRenderer) sys.GetRenderer();
            var resourceProvider = (DefaultResourceProvider)sys.GetResourceProvider();

            Base.System.Destroy();
            // TODO: delete resourceProvider; resourceProvider.Dispose();
            Destroy(renderer);
        }

        /*!
        \brief
            Create an OpenGLRenderer object.

        \param tt_type
            Specifies one of the TextureTargetType enumerated values indicating the
            desired TextureTarget type to be used.

        \param abi
            This must be set to CEGUI_VERSION_ABI
        */
        public static OpenGLRenderer Create(TextureTargetType textureTargetType = TextureTargetType.TTT_AUTO/*, int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            return new OpenGLRenderer(textureTargetType);
        }

        /*!
        \brief
            Create an OpenGLRenderer object.

        \param display_size
            Size object describing the initial display resolution.

        \param tt_type
            Specifies one of the TextureTargetType enumerated values indicating the
            desired TextureTarget type to be used.

        \param abi
            This must be set to CEGUI_VERSION_ABI
        */
        public static OpenGLRenderer Create(Sizef displaySize, TextureTargetType textureTargetType = TextureTargetType.TTT_AUTO/*, int abi = CEGUI_VERSION_ABI*/)
        {
            // TODO: System::performVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

            return new OpenGLRenderer(displaySize, textureTargetType);
        }

        /// <summary>
        /// Destroy an OpenGLRenderer object.
        /// </summary>
        /// <param name="renderer">
        /// The OpenGLRenderer object to be destroyed.
        /// </param>
        public static void Destroy(OpenGLRenderer renderer)
        {
            renderer.Dispose();
        }

        #endregion

        public override void BeginRendering()
        {
            //save current attributes
            GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            // do required set-up.  yes, it really is this minimal ;)
            GL.Enable(OpenGL.EnableCap.ScissorTest);
            GL.Enable(OpenGL.EnableCap.Texture2D);
            GL.Enable(OpenGL.EnableCap.Blend);

            // force set blending ops to get to a known state.
            SetupRenderingBlendMode(BlendMode.Normal, true);

            // enable arrays that we'll be using in the batches
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.SecondaryColorArray);
            GL.DisableClientState(ArrayCap.IndexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.FogCoordArray);
            GL.DisableClientState(ArrayCap.EdgeFlagArray);

            // if enabled, restores a subset of the GL state back to default values.
            if (d_initExtraStates)
                SetupExtraStates();
        }

        public override void EndRendering()
        {
            if (d_initExtraStates)
                CleanupExtraStates();

            // restore former matrices
            // FIXME: If the push ops failed, the following could mess things up!
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            //restore former attributes
            GL.PopAttrib();
            GL.PopClientAttrib();
        }

        public override Sizef GetAdjustedTextureSize(Sizef sz)
        {
            var @out = sz;

            // if we can't support non power of two sizes, get appropriate POT values.
            if (!GLEW_ARB_texture_non_power_of_two)
            {
                @out.Width = GetNextPowerOfTwoSize(@out.Width);
                @out.Height = GetNextPowerOfTwoSize(@out.Height);
            }

            return @out;
        }

        public override bool IsS3TCSupported()
        {
            return GLEW_EXT_texture_compression_s3tc;
        }

        public override void SetupRenderingBlendMode(BlendMode mode,bool force = false)
        {
            // exit if mode is already set up (and update not forced)
            if ((d_activeBlendMode == mode) && !force)
                return;

            d_activeBlendMode = mode;

            if (d_activeBlendMode == BlendMode.RttPremultiplied)
            {
                GL.BlendFunc(OpenGL.BlendingFactorSrc.One, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
            }
            else
            {
                if (GLEW_VERSION_1_4)
                {
                    GL.BlendFuncSeparate(OpenGL.BlendingFactorSrc.SrcAlpha,
					                     OpenGL.BlendingFactorDest.OneMinusSrcAlpha,
					                     OpenGL.BlendingFactorSrc.OneMinusDstAlpha,
					                     OpenGL.BlendingFactorDest.One);
                }
                else if (GLEW_EXT_blend_func_separate)
                {
#if __MACOS__
					GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha,
					                     BlendingFactorDest.OneMinusSrcAlpha,
					                     BlendingFactorSrc.OneMinusDstAlpha,
					                     BlendingFactorDest.One);
#else
                    GL.Ext.BlendFuncSeparate((ExtBlendFuncSeparate)OpenGL.BlendingFactorSrc.SrcAlpha,
					                         (ExtBlendFuncSeparate)OpenGL.BlendingFactorDest.OneMinusSrcAlpha,
					                         (ExtBlendFuncSeparate)OpenGL.BlendingFactorSrc.OneMinusDstAlpha,
					                         (ExtBlendFuncSeparate)OpenGL.BlendingFactorDest.One);
#endif
                    
                }
                else
                {
                    GL.BlendFunc(OpenGL.BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
                }
            }
        }

        public override RenderMaterial CreateRenderMaterial(DefaultShaderType shaderType)
        {
            if(shaderType == DefaultShaderType.Textured)
                return new RenderMaterial(d_shaderWrapperTextured);
            
            if(shaderType == DefaultShaderType.Solid)
                return new RenderMaterial(d_shaderWrapperSolid);

            throw new InvalidOperationException("A default shader of this type does not exist.");
        }

        protected override OpenGLGeometryBufferBase CreateGeometryBufferImpl(RenderMaterial renderMaterial)
        {
            return new OpenGLGeometryBuffer(this, renderMaterial);
        }

        protected override ITextureTarget CreateTextureTargetImpl()
        {
            return _textureTargetFactory(this);
        }

        protected override OpenGLTexture CreateTextureImpl(string name)
        {
            return new OpenGL1Texture(this, name);
        }

        /// <summary>
        /// set up renderer id string.
        /// </summary>
        private void InitialiseRendererIDString()
        {
            d_rendererID = "CEGUI::OpenGLRenderer - Official OpenGL based 2nd generation renderer module.";
        }

        /// <summary>
        /// Constructor for OpenGL Renderer objects
        /// </summary>
        /// <param name="textureTargetType">
        /// Specifies one of the TextureTargetType enumerated values indicating the
        /// desired TextureTarget type to be used.
        /// </param>
        private OpenGLRenderer(TextureTargetType textureTargetType)
        {
            InitialiseRendererIDString();
            InitialiseGLExtensions();
            InitialiseTextureTargetFactory(textureTargetType);
            InitialiseShaderWrappers();

            // we _really_ need separate rgb/alpha blend modes, if this support is not
            // available, add a note to the renderer ID string so that this fact is
            // logged.
            if (!GLEW_VERSION_1_4 && !GLEW_EXT_blend_func_separate)
                d_rendererID += "  No glBlendFuncSeparate(EXT) support.";
        }

        /// <summary>
        /// Constructor for OpenGL Renderer objects.
        /// </summary>
        /// <param name="displaySize">
        /// Size object describing the initial display resolution.
        /// </param>
        /// <param name="textureTargetType">
        /// Specifies one of the TextureTargetType enumerated values indicating the
        /// desired TextureTarget type to be used.
        /// </param>
        private OpenGLRenderer(Sizef displaySize, TextureTargetType textureTargetType)
            : base(displaySize)
        {
            InitialiseRendererIDString();
            InitialiseGLExtensions();
            InitialiseTextureTargetFactory(textureTargetType);
            InitialiseShaderWrappers();

            // we _really_ need separate rgb/alpha blend modes, if this support is not
            // available, add a note to the renderer ID string so that this fact is
            // logged.
            if (!GLEW_VERSION_1_4 && !GLEW_EXT_blend_func_separate)
                d_rendererID += "  No glBlendFuncSeparate(EXT) support.";
        }

        private void InitialiseShaderWrappers()
        {
            d_shaderWrapperTextured = new OpenGLShaderWrapper();
            d_shaderWrapperSolid = new OpenGLShaderWrapper();
        }

        /*!
        \brief
            Destructor for OpenGLRenderer objects
        */
        // TODO: virtual ~OpenGLRenderer();

        protected override void Dispose(bool disposing)
        {
            _textureTargetFactory = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// init the extra GL states enabled via enableExtraStateSettings
        /// </summary>
        private void SetupExtraStates()
        {
            CEGUI_activeTexture(OpenGL.TextureUnit.Texture0);
            CEGUI_clientActiveTexture(OpenGL.TextureUnit.Texture0);

            GL.MatrixMode(MatrixMode.Texture);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.PolygonMode(OpenGL.MaterialFace.Front, OpenGL.PolygonMode.Fill);
            GL.PolygonMode(OpenGL.MaterialFace.Back, OpenGL.PolygonMode.Fill);

            GL.Disable(OpenGL.EnableCap.Lighting);
            GL.Disable(OpenGL.EnableCap.Fog);
            GL.Disable(OpenGL.EnableCap.CullFace);
            GL.Disable(OpenGL.EnableCap.DepthTest);
            GL.Disable(OpenGL.EnableCap.AlphaTest);

            GL.Disable(OpenGL.EnableCap.TextureGenS);
            GL.Disable(OpenGL.EnableCap.TextureGenT);
            GL.Disable(OpenGL.EnableCap.TextureGenR);

            GL.TexEnv(TextureEnvTarget.TextureEnv, 
                      TextureEnvParameter.TextureEnvMode,
                      (int)/*All.Modulate*/0x2100);
		}

        /// <summary>
        /// cleanup the extra GL states enabled via enableExtraStateSettings
        /// </summary>
        private void CleanupExtraStates()
        {
            GL.MatrixMode(MatrixMode.Texture);
            GL.PopMatrix();
        }

        //! initialise OGLTextureTargetFactory that will generate TextureTargets
        private void InitialiseTextureTargetFactory(TextureTargetType textureTargetType)
        {
            // prefer FBO

            if (((textureTargetType == TextureTargetType.TTT_AUTO) || (textureTargetType == TextureTargetType.TTT_FBO)) && GLEW_EXT_framebuffer_object)
            {
                d_rendererID += "  TextureTarget support enabled via FBO extension.";
                _textureTargetFactory = x => new OpenGLFBOTextureTarget((OpenGLRenderer)x);
            }

        //#if defined(__linux__) || defined(__FreeBSD__) || defined(__NetBSD__) || defined(__HAIKU__)
        //    // on linux (etc), we can try for GLX pbuffer support
        //    else if (((tt_type == TTT_AUTO) || (tt_type == TTT_PBUFFER)) &&
        //             GLXEW_VERSION_1_3)
        //    {
        //        d_rendererID += "  TextureTarget support enabled via GLX pbuffers.";
        //        d_textureTargetFactory =
        //            CEGUI_NEW_AO OGLTemplateTargetFactory<OpenGLGLXPBTextureTarget>;
        //    }
        //#elif defined(_WIN32) || defined(__WIN32__)
            else if (((textureTargetType == TextureTargetType.TTT_AUTO) || (textureTargetType == TextureTargetType.TTT_PBUFFER)) && WGLEW_ARB_pbuffer)
            {
                // on Windows, we can try for WGL based pbuffer support

                d_rendererID += "  TextureTarget support enabled via WGL_ARB_pbuffer.";
                _textureTargetFactory = x => new OpenGLWGLPBTextureTarget((OpenGLRenderer)x);
            }
        //#elif defined(__APPLE__)
        //    // on Apple Mac, we can try for Apple's pbuffer support
        //    else if (((tt_type == TTT_AUTO) || (tt_type == TTT_PBUFFER)) &&
        //             GLEW_APPLE_pixel_buffer)
        //    {
        //        d_rendererID += "  TextureTarget support enabled via "
        //                        "GL_APPLE_pixel_buffer.";
        //        d_textureTargetFactory =
        //            CEGUI_NEW_AO OGLTemplateTargetFactory<OpenGLApplePBTextureTarget>;
        //    }
        //#endif
            // Nothing suitable available, try to carry on without TextureTargets
            else
            {
                d_rendererID += "  TextureTarget support is not available :(";
                _textureTargetFactory = x => null;
            }
        }

        private void InitialiseGLExtensions()
        {
            // TODO: initialise GLEW
            //GLenum err = glewInit();
            //if (GLEW_OK != err)
            //{
            //    std::ostringstream err_string;
            //    err_string << "OpenGLRenderer failed to initialise the GLEW library. "
            //    << glewGetErrorString(err);

            //    throw new InvalidOperationException(err_string.str().c_str()));
            //}
            

            if (GLEW_VERSION_1_3)
            {
                // GL 1.3 has multi-texture support natively
                CEGUI_activeTexture = x => OpenGL.GL.ActiveTexture(x);
                CEGUI_clientActiveTexture = x => OpenGL.GL.ClientActiveTexture(x);
            }
            else if (GLEW_ARB_multitexture)
            {
                // Maybe there is the ARB_multitexture extension version?
#if __MACOS__
                CEGUI_activeTexture = x => GL.ActiveTexture(x);
                CEGUI_clientActiveTexture = x => GL.ClientActiveTexture(x);
#else
				CEGUI_activeTexture = OpenGL.GL.Arb.ActiveTexture;
                CEGUI_clientActiveTexture = OpenGL.GL.Arb.ClientActiveTexture;
#endif
            }
            else
            {
                // assign dummy functions if no multitexture possibilities
                CEGUI_activeTexture = x => { };
                CEGUI_clientActiveTexture = x => { };
            }
        }

        //! pointer to a helper that creates TextureTargets supported by the system.
        private Func<OpenGLRendererBase, OpenGLTextureTarget> _textureTargetFactory;

        /// <summary>
        /// Shaderwrapper for textured & coloured vertices
        /// </summary>
        private OpenGLShaderWrapper d_shaderWrapperTextured;

        /// <summary>
        /// Shaderwrapper for coloured vertices
        /// </summary>
        private OpenGLShaderWrapper d_shaderWrapperSolid;

        private Action<OpenGL.TextureUnit> CEGUI_activeTexture;
        private Action<OpenGL.TextureUnit> CEGUI_clientActiveTexture;

        static OpenGLRenderer()
        {
            var version = new Version(OpenGL.GL.GetString(OpenGL.StringName.Version).Substring(0, 3));
            GLEW_VERSION_1_3 = version >= new Version(1, 3);
            GLEW_VERSION_1_4 = version >= new Version(1, 4);
            var extensions = OpenGL.GL.GetString(OpenGL.StringName.Extensions);

            WGLEW_ARB_pbuffer = extensions.Contains("ARB_pbuffer");
            GLEW_EXT_framebuffer_object = extensions.Contains("EXT_framebuffer_object");
            GLEW_EXT_blend_func_separate = extensions.Contains("EXT_blend_func_separate");
            GLEW_EXT_texture_compression_s3tc = extensions.Contains("EXT_texture_compression_s3tc");
            GLEW_ARB_texture_non_power_of_two = extensions.Contains("ARB_texture_non_power_of_two");

        }

        private static readonly bool GLEW_VERSION_1_3;
		private static readonly bool GLEW_VERSION_1_4;
		private static readonly bool GLEW_ARB_multitexture;
        private static readonly bool WGLEW_ARB_pbuffer;
        internal static readonly bool GLEW_EXT_framebuffer_object;
        private static readonly bool GLEW_EXT_blend_func_separate;
        private static readonly bool GLEW_EXT_texture_compression_s3tc;
        private static readonly bool GLEW_ARB_texture_non_power_of_two;
    }
}