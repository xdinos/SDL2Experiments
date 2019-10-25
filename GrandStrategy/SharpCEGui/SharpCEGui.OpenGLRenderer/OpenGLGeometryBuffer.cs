using System;
using Lunatics.Mathematics;
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
    /// <summary>
    /// OpenGL based implementation of the GeometryBuffer interface.
    /// </summary>
    public class OpenGLGeometryBuffer : OpenGLGeometryBufferBase
    {
        public OpenGLGeometryBuffer(OpenGLRenderer owner, RenderMaterial renderMaterial)
            : base(owner, renderMaterial)
        {
        }

        public override void Draw()
        {
            if (d_vertexData.Count == 0)
                return;

            var viewPort = d_owner.GetActiveViewPort();

            if (d_clippingActive)
            {
                OpenGL.GL.Scissor((int) d_preparedClippingRegion.Left,
                           (int) (viewPort.Height - d_preparedClippingRegion.Bottom),
                           (int) d_preparedClippingRegion.Width,
                           (int) d_preparedClippingRegion.Height);
                GL.Enable(EnableCap.ScissorTest);
            }
            else
            {
                GL.Disable(EnableCap.ScissorTest);
            }

            // Update the model view projection matrix
            UpdateMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            var matrix = d_matrix.ToOpenTK();
            GL.LoadMatrix(ref matrix);
            
            // activate desired blending mode
            d_owner.SetupRenderingBlendMode(d_blendMode);

            var passCount = d_effect!=null ? d_effect.GetPassCount() : 1;
            for (var pass = 0; pass < passCount; ++pass)
            {
                // set up RenderEffect
                if (d_effect!=null)
                    d_effect.PerformPreRenderFunctions(pass);

                d_renderMaterial.PrepareForRendering();
                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                DoDraw();
            }

            // clean up RenderEffect
            if (d_effect != null)
                d_effect.PerformPostRenderFunctions();

            UpdateRenderTargetData(d_owner.GetActiveRenderTarget());
        }

        public override void FinaliseVertexAttributes()
        {
        }

        private void DoDraw()
        {
            unsafe
            {
                fixed (float* pvertices = d_vertexData.ToArray())
                {
                    SetupVertexDataPointers((IntPtr)pvertices);

                    // draw the geometry
#if __MACOS__
                    GL.DrawArrays(GLPrimitiveType.Triangles, 0, d_vertexCount);
#else
                    GL.DrawArrays(BeginMode.Triangles, 0, d_vertexCount);
#endif
                    //GL.Flush();
                }
            }
            
        }

        /// <summary>
        /// Updates the fixed-function vertex data pointers based on the defined vertex attributes
        /// </summary>
        private void SetupVertexDataPointers(IntPtr pointer)
        {
            var dataOffset = 0;
            var stride = GetVertexAttributeElementCount() * sizeof(float);

            var attributeCount = d_vertexAttributes.Count;
            for (var i = 0; i < attributeCount; ++i)
            {
                switch (d_vertexAttributes[i])
                {
                    case VertexAttributeType.Position0:
                        GL.VertexPointer(3, VertexPointerType.Float, stride, pointer + dataOffset);
                        dataOffset += 3*sizeof(float);
                        break;
                    case VertexAttributeType.Color0:
                        GL.ColorPointer(4, ColorPointerType.Float, stride, pointer + dataOffset);
                        dataOffset += 4 * sizeof(float);
                        break;
                    case VertexAttributeType.TexCoord0:
                        GL.TexCoordPointer(2, TexCoordPointerType.Float, stride, pointer + dataOffset);
                        dataOffset += 2 * sizeof(float);
                        break;
                }
            }
        }
    }
}
