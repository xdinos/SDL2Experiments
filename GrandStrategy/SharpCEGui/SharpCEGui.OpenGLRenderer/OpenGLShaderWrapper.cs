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
    public class OpenGLShaderWrapper : ShaderWrapper
    {
        public override void PrepareForRendering(ShaderParameterBindings shaderParameterBindings)
        {
            var shader_parameter_bindings = shaderParameterBindings.GetShaderParameterBindings();
            
            bool isTextured = false;
            //shaderParameterBinding
            foreach (var iter in shader_parameter_bindings)
            {
                var parameter = iter.Value;

                var parameterType = parameter.GetParamType();

                switch (parameterType)
                {
                    case ShaderParamType.SPT_TEXTURE:
                        var parameterTexture = (ShaderParameterTexture) parameter;
                        var openglTexture = (OpenGLTexture) parameterTexture.d_parameterValue;

                        OpenGL.GL.ActiveTexture(OpenGL.TextureUnit.Texture0);
                        OpenGL.GL.ClientActiveTexture(OpenGL.TextureUnit.Texture0);
                        OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, openglTexture.GetOpenGLTexture());

                        isTextured = true;
                        break;
                }
            }

            if(isTextured)
            {
                OpenGL.GL.EnableClientState(OpenGL.ArrayCap.TextureCoordArray);
                OpenGL.GL.Enable(OpenGL.EnableCap.Texture2D);
            }
            else
            {
                OpenGL.GL.DisableClientState(OpenGL.ArrayCap.TextureCoordArray);
                OpenGL.GL.Disable(OpenGL.EnableCap.Texture2D);
            }
        }
    }
}