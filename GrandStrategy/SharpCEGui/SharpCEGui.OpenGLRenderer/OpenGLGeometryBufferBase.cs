using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunatics.Mathematics;
using SharpCEGui.Base;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGL based implementation of the GeometryBuffer interface.
    /// </summary>
    public abstract class OpenGLGeometryBufferBase : GeometryBuffer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="renderMaterial"></param>
        protected OpenGLGeometryBufferBase(OpenGLRendererBase owner, RenderMaterial renderMaterial)
                : base(renderMaterial)
        {
            d_owner = owner;
            d_clippingActive = true;
            d_matrix = new Matrix();
            d_matrixValid = false;
        }


        /// <summary>
        /// The update function that is to be called when all the vertex attributes are set.
        /// </summary>
        public abstract void FinaliseVertexAttributes();

        /// <summary>
        /// Update the cached matrices
        /// </summary>
        protected void UpdateMatrix()
        {
            if (!d_matrixValid || !IsRenderTargetDataValid(d_owner.GetActiveRenderTarget()))
            {
                // Apply the view projection matrix to the model matrix and save the result as cached matrix
                d_matrix = GetModelMatrix() * d_owner.GetViewProjectionMatrix();

                d_matrixValid = true;
            }
        }

        /// <summary>
        /// OpenGLRendererBase that owns the GeometryBuffer.
        /// </summary>
        protected OpenGLRendererBase d_owner;

        /// <summary>
        /// Cache of the model view projection matrix
        /// </summary>
        protected Matrix d_matrix;
    }
}
