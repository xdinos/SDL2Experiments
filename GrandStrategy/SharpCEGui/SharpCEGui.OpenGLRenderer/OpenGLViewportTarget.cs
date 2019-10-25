using Lunatics.Mathematics;
using Lunatics.SDLGL;
using SharpCEGui.Base;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGL implementation of a RenderTarget that represents an on-scren viewport.
    /// </summary>
    internal class OpenGLViewportTarget : OpenGLRenderTarget
    {
        /// <summary>
        /// Construct a default OpenGLViewportTarget that uses the currently
        /// defined OpenGL viewport as it's initial area.
        /// </summary>
        /// <param name="owner"></param>
        internal OpenGLViewportTarget(OpenGLRendererBase owner)
            : base(owner)
        {
            // viewport area defaults to whatever the current OpenGL viewport is set to
            var vp = new int[4];
            OpenGL.GL.GetInteger(OpenGL.GetPName.Viewport, vp);

            var initArea = new Rectf(new Vector2(vp[0], vp[1]),
                                     new Sizef(vp[2], vp[3]));

            SetArea(initArea);
        }

        /// <summary>
        /// Construct a OpenGLViewportTarget that uses the specified Rect as it's initial area.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="area">
        /// Rect object describing the initial viewport area that should be used for the RenderTarget.
        /// </param>
        internal OpenGLViewportTarget(OpenGLRendererBase owner, Rectf area)
            : base(owner)
        {
            SetArea(area);
        }

        // implementations of RenderTarget interface
        public override bool IsImageryCache()
        {
            return false;
        }
    }
}
