using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Lunatics.SDLGL;
using SharpCEGui.Base;

namespace SharpCEGui.OpenGLRenderer
{
    public class OpenGLBaseShaderWrapper : ShaderWrapper
    {
	    public OpenGLBaseShaderWrapper(OpenGLBaseShader shader,
	                                   OpenGLBaseStateChangeWrapper stateChangeWrapper)
	    {
		    d_shader = shader;
		    d_glStateChangeWrapper = stateChangeWrapper;
	    }

	    // TODO: ~OpenGLBaseShaderWrapper() {}

		// Implementation of ShaderWrapper interface
		public override void PrepareForRendering(ShaderParameterBindings shaderParameterBindings)
        {
			d_shader.Bind();

			var shader_parameter_bindings = shaderParameterBindings.GetShaderParameterBindings();

			foreach (var iter in shader_parameter_bindings)
			{
				var parameter = iter.Value;

				if (parameter.GetParamType() != ShaderParamType.SPT_TEXTURE)
				{
					if (d_shaderParameterStates.ContainsKey(iter.Key))
					{
						var last_shader_parameter = d_shaderParameterStates[iter.Key];
						if (parameter.Equal(last_shader_parameter))
						{
							continue;
						}
						else
						{
							if (parameter.GetParamType() == last_shader_parameter.GetParamType())
								last_shader_parameter.TakeOverParameterValue(parameter);
							else
							{
								// TODO: ... delete found_iterator->second;
								d_shaderParameterStates[iter.Key] = parameter.Clone();
							}
						}
					}
					else
					{
						d_shaderParameterStates[iter.Key] = parameter.Clone();
					}
				}

				var location = d_uniformVariables[iter.Key];


				var parameter_type = parameter.GetParamType();

				switch (parameter_type)
				{
					case ShaderParamType.SPT_INT:
					{
						var parameterInt = (ShaderParameterInt) parameter;
						OpenGL.GL.Uniform1i(location, parameterInt.d_parameterValue);
					}
						break;
					case ShaderParamType.SPT_FLOAT:
					{
						var parameterFloat = (ShaderParameterFloat) parameter;
						OpenGL.GL.Uniform1f(location, parameterFloat.d_parameterValue);
					}
						break;
					case ShaderParamType.SPT_MATRIX_4X4:
					{
						var parameterMatrix = (ShaderParameterMatrix) parameter;
						OpenGL.GL.UniformMatrix4(location, 1, false, ref parameterMatrix.d_parameterValue);
					}
						break;
					case ShaderParamType.SPT_TEXTURE:
					{
						var parameterTexture = (ShaderParameterTexture) parameter;
						var openglTexture = (OpenGLTexture) parameterTexture.d_parameterValue;

						d_glStateChangeWrapper.ActiveTexture(location);
						d_glStateChangeWrapper.BindTexture(OpenGL.TextureTarget.Texture2D,
						                                   openglTexture.GetOpenGLTexture());
					}
						break;
				}
			}
        }

        /// <summary>
        /// Adds a shader attribute variable to the list of variables
        /// </summary>
        /// <param name="attributeName"></param>
        public void AddAttributeVariable(string attributeName)
        {
			var variable_location = d_shader.GetAttribLocation(attributeName);

			if (variable_location == -1)
				throw new System.Exception /*RendererException*/(
					"OpenGLBaseShaderWrapper::addAttributeVariable- An attribute with the name \"" + attributeName +
					"\" was not found in the OpenGL shader.");

			d_attributeVariables.Add(attributeName, variable_location);
		}

        /// <summary>
        /// Adds a shader uniform variable to the list of variables
        /// </summary>
        /// <param name="uniformName"></param>
        public void AddUniformVariable(string uniformName)
        {
			var variable_location = d_shader.GetUniformLocation(uniformName);

			if (variable_location == -1)
				throw new System.Exception /*RendererException*/(
					"OpenGLBaseShaderWrapper::addUniformVariable - A uniform variable with the name \"" + uniformName +
					"\" was not found in the OpenGL shader.");

			d_uniformVariables.Add(uniformName, variable_location);
        }

        /// <summary>
        /// Adds a shader attribute variable to the list of variables
        /// </summary>
        /// <param name="uniformName"></param>
        /// <param name="textureUnitIndex"></param>
        public void AddTextureUniformVariable(string uniformName, int /*GLint*/ textureUnitIndex)
        {
			var variable_location = d_shader.GetUniformLocation(uniformName);

			if (variable_location == -1)
				throw new System.Exception /*RendererException*/("OpenGLBaseShaderWrapper::addTextureUniformVariable - A texture uniform variable with the name \"" + uniformName + "\" was not found in the OpenGL shader.");

			d_uniformVariables.Add(uniformName, textureUnitIndex);

			d_shader.Bind();
			OpenGL.GL.Uniform1i(variable_location, textureUnitIndex);
		}

        public int /*GLint*/ GetAttributeLocation(string attributeName)
        {
	        if (d_attributeVariables.ContainsKey(attributeName))
		        return d_attributeVariables[attributeName];

	        throw new System.Exception /*RendererException*/(
		        "OpenGLBaseShaderWrapper::getAttributeLocation: An attribute variable with the name \"" +
		        attributeName + "\" has not been added to the ShaderWrapper.");

        }

		public int /*GLint*/ GetUniformLocation(string uniformName)
		{
			if (d_uniformVariables.ContainsKey(uniformName))
				return d_uniformVariables[uniformName];

			throw new System.Exception /*RendererException*/(
				"OpenGLBaseShaderWrapper::getUniformLocation: An uniform variable with the name \"" + uniformName +
				"\" has not been added to the ShaderWrapper.");

        }

		//! The underlying GLSL shader that this class wraps the access to
		protected OpenGLBaseShader d_shader;
        
        //! A map of parameter names and the related uniform variable locations
        protected Dictionary<string, int > d_uniformVariables=new Dictionary<string, int>();
        
        //! A map of parameter names and the related attribute variable locations
        protected Dictionary<string, int> d_attributeVariables = new Dictionary<string, int>();
        
        //! OpenGL state change wrapper
        protected OpenGLBaseStateChangeWrapper d_glStateChangeWrapper;
        
        //! Last states of the set shader parameters
        protected Dictionary<string, ShaderParameter> d_shaderParameterStates = new Dictionary<string, ShaderParameter>();
    }
}