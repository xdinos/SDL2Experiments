using System;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
    public class OpenGLBaseShaderWrapper : ShaderWrapper
    {
        public OpenGLBaseShaderWrapper(OpenGLBaseShader shader, OpenGLBaseStateChangeWrapper stateChangeWrapper)
        {
            throw new NotImplementedException();
        }

        // TODO: ~OpenGLBaseShaderWrapper();

        public override void PrepareForRendering(ShaderParameterBindings shaderParameterBindings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a shader attribute variable to the list of variables
        /// </summary>
        /// <param name="attributeName"></param>
        public void AddAttributeVariable(string attributeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a shader uniform variable to the list of variables
        /// </summary>
        /// <param name="uniformName"></param>
        public void AddUniformVariable(string uniformName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a shader attribute variable to the list of variables
        /// </summary>
        /// <param name="uniformName"></param>
        /// <param name="textureUnitIndex"></param>
        public void AddTextureUniformVariable(string uniformName, int /*GLint*/ textureUnitIndex)
        {
            throw new NotImplementedException();
        }

        public int /*GLint*/ GetAttributeLocation(string attributeName)
        {
            throw new NotImplementedException();
        }

        public int /*GLint*/ GetUniformLocation(string uniformName)
        {
            throw new NotImplementedException();
        }
    }
}