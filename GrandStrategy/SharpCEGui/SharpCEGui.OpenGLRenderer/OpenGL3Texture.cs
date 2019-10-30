//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenGL = OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
	public class OpenGL3Texture : OpenGL1Texture
	{
		public OpenGL3Texture(OpenGLRendererBase owner, string name) 
			: base(owner, name)
		{
		}

		// TODO:
		// OpenGL3Texture::~OpenGL3Texture() { }

		protected override void SetTextureEnvironment()
		{
		}
	}
}