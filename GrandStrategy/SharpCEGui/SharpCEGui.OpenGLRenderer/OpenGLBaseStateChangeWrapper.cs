using System.Collections.Generic;
using Lunatics.SDLGL;
//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGLBaseStateChangeWrapper - wraps OpenGL calls and checks for redundant calls beforehand
    /// </summary>
    public abstract class OpenGLBaseStateChangeWrapper
    {
        /// <summary>
        /// This has to be used for both glBlendFunc and glBlendFuncSeperate, as the second call is
        /// just a more specific version of the first.
        /// </summary>
        public class BlendFuncSeperateParams
        {
            public BlendFuncSeperateParams()
            {
                Reset();
            }

            public void Reset()
            {
                d_sfactorRGB = -1;
                d_dfactorRGB = -1;
                d_sfactorAlpha = -1;
                d_dfactorAlpha = -1;
            }

            public bool Equal(int /*GLenum*/ sFactor, int /*GLenum*/ dFactor)
            {
                return Equal(sFactor, dFactor, sFactor, dFactor);
            }

            public bool Equal(int /*GLenum*/ sfactorRGB, int /*GLenum*/ dfactorRGB, int /*GLenum*/ sfactorAlpha,
                              int /*GLenum*/ dfactorAlpha)
            {
                var equal = (d_sfactorRGB == sfactorRGB) &&
                            (d_dfactorRGB == dfactorRGB) &&
                            (d_sfactorAlpha == sfactorAlpha) &&
                            (d_dfactorAlpha == dfactorAlpha);
                if (!equal)
                {
                    d_sfactorRGB = sfactorRGB;
                    d_dfactorRGB = dfactorRGB;
                    d_sfactorAlpha = sfactorAlpha;
                    d_dfactorAlpha = dfactorAlpha;
                }
                return equal;
            }

            public int /*GLenum*/ d_sfactorRGB, d_dfactorRGB, d_sfactorAlpha, d_dfactorAlpha;
        }

        public class PortParams
        {
            public PortParams()
            {
                Reset();
            }

            public void Reset()
            {
                d_x = -1;
                d_y = -1;
                d_width = -1;
                d_height = -1;
            }

            public bool Equal(int /*GLint*/ x, int /*GLint*/ y, int /*GLsizei*/ width, int /*GLsizei*/ height)
            {
                var equal = (d_x == x) && (d_y == y) && (d_width == width) && (d_height == height);

                if (!equal)
                {
                    d_x = x;
                    d_y = y;
                    d_width = width;
                    d_height = height;
                }

                return equal;
            }

            public int /*GLint*/ d_x, d_y;
            public int /*GLsizei*/ d_width, d_height;
        }

        public class BindBufferParams
        {
            public BindBufferParams()
            {
                Reset();
            }

            public void Reset()
            {
                d_target = -1;
                d_buffer = -1;
            }

            public bool Equal(int /*GLenum*/ target, int /*GLuint*/ buffer)
            {
                var equal = (d_target == target) && (d_buffer == buffer);
                if (!equal)
                {
                    d_target = target;
                    d_buffer = buffer;
                }
                return equal;
            }

            public int /*GLenum*/ d_target;
            public int /*GLuint*/ d_buffer;
        }

        public class BoundTexture
        {
            public BoundTexture()
            {
                d_target = -1;
                d_texture = -1;
            }

            public void BindTexture(int /*GLenum*/ target, int /*GLuint*/ texture)
            {
                d_target = target;
                d_texture = texture;
            }

            public int /*GLenum*/ d_target;
            public int /*GLuint*/ d_texture;
        }


        protected OpenGLBaseStateChangeWrapper()
        {
            Reset();
        }

        // TODO: virtual ~OpenGLBaseStateChangeWrapper() { }


        /// <summary>
        /// Due to unknown changes of states between each time CEGUI gets rendered, we will invalidate
        /// all states on CPU-side so that the following calls will definitely change the states on GPU
        /// </summary>
        public void Reset()
        {
            d_vertexArrayObject = -1;
            d_shaderProgram = -1;
            d_blendFuncSeperateParams.Reset();
            d_viewPortParams.Reset();
            d_scissorParams.Reset();
            d_bindBufferParams.Reset();
            d_activeTexturePosition = -1;
            d_boundTextures.Clear();
            d_enabledOpenGLStates.Clear();
        }

        /// <summary>
        /// Functions wrapping the gl* function calls to improve performance by storing the parameters and 
        /// only calling the OpenGL functions when actual state changes are taking place.
        /// </summary>
        /// <param name="vertexArray"></param>
        public abstract void BindVertexArray(int /*GLuint*/ vertexArray);

        public void UseProgram(int /*GLuint*/ program)
        {
            if (program != d_shaderProgram)
            {
                OpenGL.GL.UseProgram(program);
                d_shaderProgram = program;
            }
        }

        public void BlendFunc(OpenGL.BlendingFactorSrc /*GLenum*/ sfactor, OpenGL.BlendingFactorDest /*GLenum*/ dfactor)
        {
            var callIsRedundant = d_blendFuncSeperateParams.Equal((int)sfactor, (int)dfactor);
            if (!callIsRedundant)
                OpenGL.GL.BlendFunc(sfactor, dfactor);
        }

        public void BlendFuncSeparate(OpenGL.BlendingFactorSrc /*GLenum*/ sfactorRGB,
                                      OpenGL.BlendingFactorDest /*GLenum*/ dfactorRGB,
                                      OpenGL.BlendingFactorSrc /*GLenum*/ sfactorAlpha,
                                      OpenGL.BlendingFactorDest /*GLenum*/ dfactorAlpha)
        {
            var callIsRedundant = d_blendFuncSeperateParams.Equal((int) sfactorRGB, (int) dfactorRGB,
                                                                   (int) sfactorAlpha, (int) dfactorAlpha);
            if (!callIsRedundant)
                OpenGL.GL.BlendFuncSeparate(sfactorRGB, dfactorRGB, sfactorAlpha, dfactorAlpha);
        }

        public void Viewport(int /*GLint*/ x, int /*GLint*/ y, int /*GLsizei*/ width, int /*GLsizei*/ height)
        {
            var callIsRedundant = d_viewPortParams.Equal(x, y, width, height);
            if (!callIsRedundant)
                OpenGL.GL.Viewport(x, y, width, height);
        }

        public void Scissor(int /*GLint*/ x, int /*GLint*/ y, int /*GLsizei*/ width, int /*GLsizei*/ height)
        {
            var callIsRedundant = d_scissorParams.Equal(x, y, width, height);
            if (!callIsRedundant)
                OpenGL.GL.Scissor(x, y, width, height);
        }

        public void BindBuffer(OpenGL.BufferTarget /*GLenum*/ target, int /*GLuint*/ buffer)
        {
            var callIsRedundant = d_bindBufferParams.Equal((int)target, buffer);
            if (!callIsRedundant)
                OpenGL.GL.BindBuffer(target, buffer);
        }

        public void Enable(OpenGL.EnableCap /*GLenum*/ capability)
        {
            if(d_enabledOpenGLStates.ContainsKey(capability))
            {
                if (d_enabledOpenGLStates[capability] != true)
                {
                    OpenGL.GL.Enable(capability);
                    d_enabledOpenGLStates[capability] = true;
                }
            }
            else
            {
                d_enabledOpenGLStates[capability] = true;
                OpenGL.GL.Enable(capability);
            }
        }

        public void Disable(OpenGL.EnableCap /*GLenum*/ capability)
        {
            if(d_enabledOpenGLStates.ContainsKey(capability))
            {
                if (d_enabledOpenGLStates[capability] != false)
                {
                    OpenGL.GL.Disable(capability);
                    d_enabledOpenGLStates[capability] = false;
                }
            }
            else
            {
                d_enabledOpenGLStates[capability] = false;
                OpenGL.GL.Disable(capability);
            }
        }


        public void BindTexture(OpenGL.TextureTarget /*GLenum*/ target, int /*GLuint*/ texture)
        {
            if (d_activeTexturePosition == -1)
                return;

            var boundTexture = d_boundTextures[d_activeTexturePosition];
            if (boundTexture.d_target != (int)target || boundTexture.d_texture != texture)
            {
                OpenGL.GL.BindTexture(target, texture);
                boundTexture.BindTexture((int)target, texture);
            }
        }

        /// <summary>
        /// This function takes the number representing the texture position as unsigned integer, 
        /// not the actual OpenGL value for the position (GL_TEXTURE0, GL_TEXTURE1).
        /// </summary>
        /// <param name="texturePosition">
        /// Value representing the texture position as integer, it will be used in the following way to get the
        /// OpenGL Texture position: (GL_TEXTURE0 + texture_position)
        /// </param>
        public void ActiveTexture(int texturePosition)
        {
            if (d_activeTexturePosition != texturePosition)
            {
                while (texturePosition >= d_boundTextures.Count)
                    d_boundTextures.Add(new BoundTexture());


                OpenGL.GL.ActiveTexture(OpenGL.TextureUnit.Texture0 + texturePosition);
                d_activeTexturePosition = texturePosition;
            }
        }

        /// <summary>
        /// Returns the number representing the last active texture's position. This value is the one that was last
        /// set using this wrapper. No OpenGL getter function is called to retrieve the actual state of the variable,
        /// which means that changes resulting from OpenGL calls done outside this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// An unsigned int representing the currently active texture.
        /// </returns>
        public int GetActiveTexture()
        {
            return (int) (OpenGL.TextureUnit.Texture0 + d_activeTexturePosition);
        }

        /// <summary>
        /// Returns the ID of the bound vertex array. No OpenGL getter function is called to retrieve the actual state of 
        /// the variable, which means that changes resulting from OpenGL calls done outside this wrapper, will not be
        /// considered.
        /// </summary>
        /// <returns>
        /// An unsigned int representing the bound vertex array.
        /// </returns>
        public int /*GLuint*/ GetBoundVertexArray()
        {
            return d_vertexArrayObject;
        }

        /// <summary>
        /// Returns the ID of the OpenGL shader program that is set to be used . No OpenGL getter function is called to
        /// retrieve the actual state of the variable, which means that changes resulting from OpenGL calls done outside 
        /// this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// An unsigned int representing the ID of the OpenGL shader program.
        /// </returns>
        public int /*GLuint*/ GetUsedProgram()
        {
            return d_shaderProgram;
        }

        /// <summary>
        /// Returns a struct containing the parameters that were set for the blend function. No OpenGL getter function is
        /// called to retrieve the actual state of the variables, which means that changes resulting from OpenGL calls done
        /// outside this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// A struct containing the parameters for the blend function.
        /// </returns>
        public BlendFuncSeperateParams GetBlendFuncParams()
        {
            return d_blendFuncSeperateParams;
        }

        /// <summary>
        /// Returns a struct containing the parameters that were set for the viewport function. No OpenGL getter function is
        /// called to retrieve the actual state of the variables, which means that changes resulting from OpenGL calls done
        /// outside this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// A struct containing the parameters for the viewport function.
        /// </returns>
        public PortParams GetViewportParams()
        {
            return d_viewPortParams;
        }

        /// <summary>
        /// Returns a struct containing the parameters that were set for the scissor function. No OpenGL getter function is
        /// called to retrieve the actual state of the variables, which means that changes resulting from OpenGL calls done
        /// outside this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// A struct containing the parameters for the scissor function.
        /// </returns>
        public PortParams GetScissorParams()
        {
            return d_scissorParams;
        }

        /// <summary>
        /// Returns a struct containing the parameters that were set for the bindBuffer function. No OpenGL getter function is
        /// called to retrieve the actual state of the variables, which means that changes resulting from OpenGL calls done
        /// outside this wrapper, will not be considered.
        /// </summary>
        /// <returns>
        /// A struct containing the parameters for the bindBuffer function.
        /// </returns>
        public BindBufferParams GetBoundBuffer()
        {
            return d_bindBufferParams;
        }

        /// <summary>
        /// Returns an integers representing if an OpenGL state was enabled, disabled or not set. No OpenGL getter function is
        /// called to retrieve the actual state of the variables, which means that changes resulting from OpenGL calls done
        /// outside this wrapper, will not be considered.
        /// </summary>
        /// <param name="capability">
        /// The OpenGL state's OpenGL enum.
        /// </param>
        /// <returns>
        /// 0 if the requested state has been disabled,
        /// 1 if the requested state has been enabled,
        /// -1 if the requested state has never been set using this class.
        /// </returns>
        public int IsStateEnabled(OpenGL.EnableCap /*GLenum*/ capability)
        {
            if (!d_enabledOpenGLStates.ContainsKey(capability)) 
                return -1;
            
            var isEnabled = d_enabledOpenGLStates[capability];

            return isEnabled ? 1 : 0;
        }

        protected int /*GLuint*/ d_vertexArrayObject;
        protected int /*GLuint*/ d_shaderProgram;
        protected BlendFuncSeperateParams d_blendFuncSeperateParams = new BlendFuncSeperateParams();
        protected PortParams d_viewPortParams = new PortParams();
        protected PortParams d_scissorParams = new PortParams();
        protected BindBufferParams d_bindBufferParams = new BindBufferParams();

        //! List of enabled/disabled OpenGL states
        protected Dictionary<OpenGL.EnableCap, bool> /*std::map<GLenum, bool>*/ d_enabledOpenGLStates = new Dictionary<OpenGL.EnableCap, bool>();

        //! The active texture saved as integer and not as OpenGL enum
        protected int d_activeTexturePosition;

        //! List of bound textures, the position in the vector defines the active texture it is bound to
        protected List<BoundTexture> d_boundTextures = new List<BoundTexture>();
    }
}