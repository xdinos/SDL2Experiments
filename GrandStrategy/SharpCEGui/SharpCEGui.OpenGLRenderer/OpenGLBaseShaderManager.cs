using System;
using System.Collections.Generic;

namespace SharpCEGui.OpenGLRenderer
{
	public enum ShaderVersion
	{
		Glsl,
		GlslEs1,
		GlslEs3
	};

	public enum OpenGLBaseShaderID
	{
		StandardTextured,
		StandardSolid,

		Count
	};

	public class OpenGLBaseShaderManager
    {
	    public OpenGLBaseShaderManager(OpenGLBaseStateChangeWrapper glStateChanger, ShaderVersion shaderVersion)
	    {
		    d_glStateChanger = glStateChanger;
		    d_shaderVersion = shaderVersion;

			d_shadersInitialised = false;
		}

		// TODO:
		//virtual ~OpenGLBaseShaderManager()
		//{
		//	deinitialiseShaders();
		//	d_shadersInitialised = false;
		//}

		public OpenGLBaseShader GetShader(OpenGLBaseShaderID id)
		{
			return d_shaders.ContainsKey(id)
				       ? d_shaders[id]
				       : null;
		}

	    public void LoadShader(OpenGLBaseShaderID id, string vertexShader, string fragmentShader)
	    {
		    if (d_shaders.ContainsKey(id)) 
			    return;

		    d_shaders[id] = new OpenGLBaseShader(vertexShader, fragmentShader, d_glStateChanger);
			d_shaders[id].Link();
	    }

	    public void InitialiseShaders()
	    {
			if (!d_shadersInitialised)
			{
				//if (OpenGLInfo::getSingleton().isUsingDesktopOpengl())
				//{
					LoadShader(OpenGLBaseShaderID.StandardTextured, StandardShaderTexturedVertDesktopOpengl3, StandardShaderTexturedFragDesktopOpengl3);
					LoadShader(OpenGLBaseShaderID.StandardSolid, StandardShaderSolidVertDesktopOpengl3, StandardShaderSolidFragDesktopOpengl3);
				//}
				//else if (OpenGLInfo::getSingleton().verMajor() <= 2) // Open GL ES < 3
				//{
				//	LoadShader(OpenGLBaseShaderID::StandardTextured, StandardShaderTexturedVertOpenglEs2, StandardShaderTexturedFragOpenglEs2);
				//	LoadShader(OpenGLBaseShaderID::StandardSolid, StandardShaderSolidVertOpenglEs2, StandardShaderSolidFragOpenglEs2);
				//}
				//else // OpenGL ES >= 3
				//{
				//	LoadShader(OpenGLBaseShaderID::StandardTextured, StandardShaderTexturedVertOpenglEs3, StandardShaderTexturedFragOpenglEs3);
				//	LoadShader(OpenGLBaseShaderID::StandardSolid, StandardShaderSolidVertOpenglEs3, StandardShaderSolidFragOpenglEs3);
				//}


				if (!GetShader(OpenGLBaseShaderID.StandardTextured).IsCreatedSuccessfully() ||
				    !GetShader(OpenGLBaseShaderID.StandardSolid).IsCreatedSuccessfully())
				{
					throw new /*RendererException*/
						Exception("Critical Error - One or multiple shader programs weren't created successfully");
				}

				//const CEGUI::String notify("OpenGL3Renderer: Notification - Successfully initialised OpenGL3Renderer shader programs.");
				//if (CEGUI::Logger * logger = CEGUI::Logger::getSingletonPtr())
				//	logger->logEvent(notify);
			}
		}

	    public void DeinitialiseShaders()
	    {
		    foreach (var currentShader in d_shaders)
		    {
			    // TODO: currentShader.Value.Dispose();
				//delete currentShader.second;
			}

			d_shaders.Clear();
		}

	    private bool d_shadersInitialised;
		private ShaderVersion d_shaderVersion;
	    private OpenGLBaseStateChangeWrapper d_glStateChanger;
	    private readonly Dictionary<OpenGLBaseShaderID, OpenGLBaseShader> d_shaders = new Dictionary<OpenGLBaseShaderID, OpenGLBaseShader>();

		// A string containing a desktop OpenGL 3.2 vertex shader for solid colouring of a polygon.
		private const string StandardShaderSolidVertDesktopOpengl3 =
		    "#version 150 core\n" +
		    "uniform mat4 modelViewProjMatrix;\n" +
		    "in vec3 inPosition;\n" +
		    "in vec4 inColour;\n" +
		    "out vec4 exColour;\n" +
		    "void main(void)\n" +
		    "{\n" +
		    "exColour = inColour;\n" +
		    "gl_Position = modelViewProjMatrix * vec4(inPosition, 1.0);\n" +
		    "}";

	    // A string containing a desktop OpenGL 3.2 fragment shader for solid colouring of a polygon.
	    private const string StandardShaderSolidFragDesktopOpengl3 =
		    "#version 150 core\n" +
		    "in vec4 exColour;\n" +
		    "out vec4 out0;\n" +
		    "uniform float alphaFactor;\n" +
		    "void main(void)\n" +
		    "{\n" +
		    "out0 = exColour;\n" +
		    "out0.a *= alphaFactor;\n" +
		    "}";

		/*! A string containing an OpenGL3 vertex shader for polygons that should be
	     coloured based on a texture. The fetched texture colour will be multiplied
		 by a colour supplied to the shader, resulting in the final colour.
		 */
		private const string StandardShaderTexturedVertDesktopOpengl3 =
		    "#version 150 core\n" +
		    "uniform mat4 modelViewProjMatrix;\n" +
		    "in vec3 inPosition;\n" +
		    "in vec2 inTexCoord;\n" +
		    "in vec4 inColour;\n" +
		    "out vec2 exTexCoord;\n" +
		    "out vec4 exColour;\n" +
		    "void main(void)\n" +
		    "{\n" +
		    "exTexCoord = inTexCoord;\n" +
		    "exColour = inColour;\n" +
		    "gl_Position = modelViewProjMatrix * vec4(inPosition, 1.0);\n" +
		    "}";

	    // A string containing a desktop OpenGL 3.2 fragment shader for polygons that
	    // should be coloured based on a texture. The fetched texture colour will be
	    // multiplied by a colour supplied to the shader, resulting in the final colour. 
	    private const string StandardShaderTexturedFragDesktopOpengl3 =
		    "#version 150 core\n" +
		    "uniform sampler2D texture0;\n" +
		    "in vec2 exTexCoord;\n" +
		    "in vec4 exColour;\n" +
		    "out vec4 out0;\n" +
		    "uniform float alphaFactor;\n" +
		    "void main(void)\n" +
		    "{\n" +
		    "out0 = texture(texture0, exTexCoord) * exColour;\n" +
		    "out0.a *= alphaFactor;\n" +
		    "}";
    }
}