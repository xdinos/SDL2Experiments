using System;

namespace SharpCEGui.OpenGLRenderer
{
    public class OpenGLBaseShader
    {
        /// <summary>
        /// Creates and loads shader programs from the two strings supplied to it
        /// </summary>
        /// <param name="vertex_shader_source"></param>
        /// <param name="fragment_shader_source"></param>
        /// <param name="glStateChanger"></param>
        public OpenGLBaseShader(string vertex_shader_source,
                                string fragment_shader_source,
                                OpenGLBaseStateChangeWrapper glStateChanger)
        {
            throw new NotImplementedException();
        }

        // TODO: ~OpenGLBaseShader();

        /// <summary>
        /// Bind the shader to the OGL state-machine
        /// </summary>
        public void Bind()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query the location of a vertex attribute inside the shader.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int /*GLint*/ GetAttribLocation(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query the location of a uniform variable inside the shader.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int /*GLint*/ GetUniformLocation(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the name of the variable inside the shader which represents the
        /// final color for each fragment.
        /// </summary>
        /// <param name="name"></param>
        public virtual void BindFragDataLocation(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsCreatedSuccessfully()
        {
            throw new NotImplementedException();
        }

        public virtual void Link()
        {
            throw new NotImplementedException();
        }

        private uint /*GLuint*/ Compile(uint /*GLuint*/ type, string source)
        {
            throw new NotImplementedException();
        }

        private void OutputShaderLog(uint /*GLuint*/ shader)
        {
            throw new NotImplementedException();
        }

        private void OutputProgramLog(uint /*GLuint*/ program)
        {
            throw new NotImplementedException();
        }

        #region Fields

        protected uint /*GLuint*/ d_program;

        private OpenGLBaseStateChangeWrapper d_glStateChanger;

        private bool d_createdSuccessfully;

        private uint /*GLuint*/ d_vertexShader;
        private uint /*GLuint*/ d_fragmentShader;
        private uint /*GLuint*/ d_geometryShader;

        #endregion
    }
}