using System;
using Lunatics.SDLGL;

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
	        d_glStateChanger = glStateChanger;
	        d_createdSuccessfully = false;
	        d_vertexShader = 0;
	        d_fragmentShader = 0;
	        d_geometryShader = 0;
	        d_program = 0;


			// Compile the shaders

			d_vertexShader = Compile(OpenGL.ShaderType.VertexShader, vertex_shader_source);
			if (d_vertexShader == 0)
				return;

			OpenGLRendererBase.CheckGLErrors();

			if (!string.IsNullOrWhiteSpace(fragment_shader_source))
			{
				d_fragmentShader = Compile(OpenGL.ShaderType.FragmentShader, fragment_shader_source);

				if (d_fragmentShader == 0)
					return;
			}

			OpenGLRendererBase.CheckGLErrors();

			d_program = OpenGL.GL.CreateProgram();
		}

		// TODO://OpenGLBaseShader::~OpenGLBaseShader()
		//{
		//	if (d_program != 0)
		//		glDeleteProgram(d_program);
		//	if (d_vertexShader != 0)
		//		glDeleteShader(d_vertexShader);
		//	if (d_fragmentShader != 0)
		//		glDeleteShader(d_fragmentShader);
		//	if (d_geometryShader != 0)
		//		glDeleteShader(d_geometryShader);
		//}

		/// <summary>
		/// Bind the shader to the OGL state-machine
		/// </summary>
		public void Bind()
        {
			d_glStateChanger.UseProgram(d_program);
		}

        /// <summary>
        /// Query the location of a vertex attribute inside the shader.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int /*GLint*/ GetAttribLocation(string name)
        {
			return OpenGL.GL.GetAttribLocation(d_program, name);
		}

        /// <summary>
        /// Query the location of a uniform variable inside the shader.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int /*GLint*/ GetUniformLocation(string name)
        {
			return OpenGL.GL.GetUniformLocation(d_program, name);
		}

        /// <summary>
        /// Defines the name of the variable inside the shader which represents the
        /// final color for each fragment.
        /// </summary>
        /// <param name="name"></param>
        public virtual void BindFragDataLocation(string name)
        {
			if (d_program > 0)
				Link();
		}

        public bool IsCreatedSuccessfully()
        {
			return d_createdSuccessfully;
		}

        public virtual void Link()
        {
			// Attach shaders and link
			OpenGL.GL.AttachShader(d_program, d_vertexShader);

			if (d_geometryShader != 0)
				OpenGL.GL.AttachShader(d_program, d_geometryShader);

			if (d_fragmentShader != 0)
				OpenGL.GL.AttachShader(d_program, d_fragmentShader);

			OpenGL.GL.LinkProgram(d_program);

			// Check for problems
			OpenGL.GL.GetProgram(d_program, OpenGL.GetProgramParameterName.LinkStatus, out var status);

			if (status == 0)
			{
				OutputProgramLog(d_program);

				OpenGL.GL.DeleteProgram(d_program);
				d_program = 0;
			}

			OpenGLRendererBase.CheckGLErrors();

			if (d_program == 0)
				return;

			d_createdSuccessfully = true;
			OpenGLRendererBase.CheckGLErrors();
		}

        private int /*GLuint*/ Compile(OpenGL.ShaderType type, string source)
        {
	        OpenGLRendererBase.CheckGLErrors();

			var shader = OpenGL.GL.CreateShader(type);

			if (shader == 0)
				throw new Exception/*RendererException*/($"Critical Error - Could not create shader object of type:{type}.");

			OpenGLRendererBase.CheckGLErrors();

			// Define shader source and compile
			OpenGL.GL.ShaderSource(shader, source);

			OpenGL.GL.CompileShader(shader);

			// Check for errors
			OpenGL.GL.GetShader(shader, OpenGL.ShaderParameter.CompileStatus, out var status);

			if (status ==0)
			{
				OutputShaderLog(shader);
				return 0;
			}

			OpenGLRendererBase.CheckGLErrors();

			return shader;
		}

        private void OutputShaderLog(int /*GLuint*/ shader)
        {
	        var log = OpenGL.GL.GetShaderInfoLog(shader);
	        if (!string.IsNullOrWhiteSpace(log))
		        throw new Exception/*RendererException*/($"OpenGLBaseShader linking has failed.\n{log}");
		}

        private void OutputProgramLog(int /*GLuint*/ program)
        {
			var log = OpenGL.GL.GetProgramInfoLog(program);
			if (!string.IsNullOrWhiteSpace(log))
				throw new Exception/*RendererException*/($"OpenGLBaseShader linking has failed.\n{log}");
		}

        #region Fields

        protected int /*GLuint*/ d_program;

        private OpenGLBaseStateChangeWrapper d_glStateChanger;

        private bool d_createdSuccessfully;

        private int /*GLuint*/ d_vertexShader;
        private int /*GLuint*/ d_fragmentShader;
        private int /*GLuint*/ d_geometryShader;

        #endregion
    }
}