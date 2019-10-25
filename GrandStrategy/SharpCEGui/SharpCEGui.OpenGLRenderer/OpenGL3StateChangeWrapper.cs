using System;
using Lunatics.SDLGL;

//#if __MACOS__
//using OpenGL;
//#else
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGL3StateChangeWrapper - wraps OpenGL calls and checks for redundant calls beforehand
    /// </summary>
    public class OpenGL3StateChangeWrapper : OpenGLBaseStateChangeWrapper
    {
        public override void BindVertexArray(int vertexArray)
        {
            if (vertexArray != d_vertexArrayObject)
            {
#if __MACOS__
				throw new NotImplementedException();
#else
                OpenGL.GL.BindVertexArray(vertexArray);
                d_vertexArrayObject = vertexArray;
#endif
            }
        }
    }
}