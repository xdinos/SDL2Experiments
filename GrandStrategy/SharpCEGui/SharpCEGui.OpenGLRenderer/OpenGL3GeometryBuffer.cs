using System;
using System.Collections.Generic;
using SharpCEGui.Base;
using System.Runtime.InteropServices;
using Lunatics.SDLGL;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// OpenGL3 based implementation of the GeometryBuffer interface.
    /// </summary>
    public class OpenGL3GeometryBuffer : OpenGLGeometryBufferBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="renderMaterial"></param>
        public OpenGL3GeometryBuffer(OpenGL3Renderer owner, RenderMaterial renderMaterial):
            base(owner, renderMaterial)
        {
            d_glStateChanger = owner.GetOpenGLStateChanger();
            d_bufferSize = 0;
            InitialiseVertexBuffers();
        }
        
        protected override void Dispose(bool disposing)
        {
            DeinitialiseOpenGLBuffers();
            base.Dispose(disposing);
        }
        
        public override void Draw()
        {
            if (d_vertexData.Count == 0)
                return;

            var viewPort = d_owner.GetActiveViewPort();

            if (d_clippingActive)
            {
                d_glStateChanger.Scissor((int) d_preparedClippingRegion.Left,
                                         (int) (viewPort.Height - d_preparedClippingRegion.Bottom),
                                         (int) d_preparedClippingRegion.Width,
                                         (int) d_preparedClippingRegion.Height);
                d_glStateChanger.Enable(OpenGL.EnableCap.ScissorTest);
            }
            else
                d_glStateChanger.Disable(OpenGL.EnableCap.ScissorTest);

            // Update the model view projection matrix
            UpdateMatrix();

            var shaderParameterBindings = d_renderMaterial.GetShaderParamBindings();

            // Set the uniform variables for this GeometryBuffer in the Shader
            shaderParameterBindings.SetParameter("modelViewProjMatrix", ref d_matrix);
            shaderParameterBindings.SetParameter("alphaFactor", d_alpha);

            // activate desired blending mode
            d_owner.SetupRenderingBlendMode(d_blendMode);

            // Bind our vao
            d_glStateChanger.BindVertexArray(d_verticesVAO);

            var passCount = d_effect != null ? d_effect.GetPassCount() : 1;
            for (int pass = 0; pass < passCount; ++pass)
            {
                // set up RenderEffect
                if (d_effect != null)
                    d_effect.PerformPreRenderFunctions(pass);

                d_renderMaterial.PrepareForRendering();

                // draw the geometry
                DrawDependingOnFillRule();
            }

            // clean up RenderEffect
            if (d_effect != null)
                d_effect.PerformPostRenderFunctions();

            UpdateRenderTargetData(d_owner.GetActiveRenderTarget());
        }

        public override void AppendGeometry(float[] vertexData, int vertexCount)
        {
            base.AppendGeometry(vertexData, vertexCount);
            UpdateOpenGLBuffers();
        }

        public override void Reset()
        {
            base.Reset();
            UpdateOpenGLBuffers();
        }

        public override void FinaliseVertexAttributes()
        {
            //We need to bind both of the following calls
            d_glStateChanger.BindVertexArray(d_verticesVAO);
            d_glStateChanger.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, d_verticesVBO);

            var stride = GetVertexAttributeElementCount() * sizeof(float);

            var gl3_shader_wrapper = (OpenGLBaseShaderWrapper)d_renderMaterial.GetShaderWrapper();

            //Update the vertex attrib pointers of the vertex array object depending on the saved attributes
            int dataOffset = 0;
            var attribute_count = d_vertexAttributes.Count;
            for (var i = 0; i < attribute_count; ++i)
            {
                switch(d_vertexAttributes[i])
                {
                case VertexAttributeType.Position0:
                    {
                        var shader_pos_loc = gl3_shader_wrapper.GetAttributeLocation("inPosition");
                        OpenGL.GL.VertexAttribPointer(shader_pos_loc, 
                                               3, 
                                               OpenGL.VertexAttribPointerType.Float, 
                                               false, 
                                               stride, 
                                               (IntPtr)(dataOffset * sizeof(float)));
                        OpenGL.GL.EnableVertexAttribArray(shader_pos_loc);
                        dataOffset += 3;
                    }
                    break;
                case VertexAttributeType.Color0:
                    {
                        var shader_colour_loc = gl3_shader_wrapper.GetAttributeLocation("inColour");
                        OpenGL.GL.VertexAttribPointer(shader_colour_loc, 
                                               4, 
                                               OpenGL.VertexAttribPointerType.Float, 
                                               false, 
                                               stride, 
                                               (IntPtr)(dataOffset * sizeof(float)));
                        OpenGL.GL.EnableVertexAttribArray(shader_colour_loc);
                        dataOffset += 4;
                    }
                    break;
                case VertexAttributeType.TexCoord0:
                    {
                        var texture_coord_loc = gl3_shader_wrapper.GetAttributeLocation("inTexCoord");
                        OpenGL.GL.VertexAttribPointer(texture_coord_loc, 
                                               2, 
                                               OpenGL.VertexAttribPointerType.Float, 
                                               false, 
                                               stride, 
                                               (IntPtr)(dataOffset * sizeof(float)));
                        OpenGL.GL.EnableVertexAttribArray(texture_coord_loc);
                        dataOffset += 2;
                    }
                    break;
                }
            }
        }

        protected void InitialiseVertexBuffers()
		{
#if __MACOS__
			throw new NotImplementedException();
#else
            OpenGL.GL.GenVertexArrays(1, out d_verticesVAO);
            d_glStateChanger.BindVertexArray(d_verticesVAO);

            // Generate and bind position vbo
            OpenGL.GL.GenBuffers(1, out d_verticesVBO);
            d_glStateChanger.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, d_verticesVBO);

            OpenGL.GL.BufferData(OpenGL.BufferTarget.ArrayBuffer, IntPtr.Zero, IntPtr.Zero, OpenGL.BufferUsageHint.StaticDraw);

            // Unbind Vertex Attribute Array (VAO)
            d_glStateChanger.BindVertexArray(0);

            // Unbind array and element array buffers
            d_glStateChanger.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, 0);
#endif
        }

        protected void DeinitialiseOpenGLBuffers()
        {
#if __MACOS__
			throw new NotImplementedException();
#else
            OpenGL.GL.DeleteVertexArrays(1, ref d_verticesVAO);
            OpenGL.GL.DeleteBuffers(1, ref d_verticesVBO);
#endif
        }

        /// <summary>
        /// Update the OpenGL buffer objects containing the vertex data.
        /// </summary>
        protected void UpdateOpenGLBuffers()
        {
            var needNewBuffer = false;
            var vertexCount = d_vertexData.Count;

            if (d_bufferSize < vertexCount)
            {
                needNewBuffer = true;
                d_bufferSize = vertexCount;
            }

            d_glStateChanger.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, d_verticesVBO);

            //float* vertexData;
            //if (d_vertexData.empty())
            //    vertexData = 0;
            //else
            //    vertexData = &d_vertexData[0];

            var vertexData = d_vertexData.ToArray();
            var dataSize = new IntPtr(vertexCount * sizeof(float));

            var ptr = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            try
            {
                if (needNewBuffer)
                {
                    //GL.BufferData(BufferTarget.ArrayBuffer, dataSize, vertexData, BufferUsageHint.StaticDraw);
                    OpenGL.GL.BufferData(OpenGL.BufferTarget.ArrayBuffer, dataSize, ptr.AddrOfPinnedObject(), OpenGL.BufferUsageHint.StaticDraw);
                }
                else
                {
                    //GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, dataSize, vertexData);
                    OpenGL.GL.BufferSubData(OpenGL.BufferTarget.ArrayBuffer, IntPtr.Zero, dataSize, ptr.AddrOfPinnedObject());
                }
            }
            finally{
                ptr.Free();
            }
        }

        /// <summary>
        /// Draws the vertex data depending on the fill rule that was set for this object.
        /// </summary>
        protected void DrawDependingOnFillRule()
        {
            if(d_polygonFillRule == PolygonFillRule.None)
            {
                d_glStateChanger.Disable(OpenGL.EnableCap.CullFace);
                d_glStateChanger.Disable(OpenGL.EnableCap.StencilTest);

                OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, 0, d_vertexCount);
            }
            else if(d_polygonFillRule == PolygonFillRule.EvenOdd)
            {
				//We use a stencil buffer to determine the insideness
				//of a fragment. Every draw inverts the precious value
				//according to the even-odd rule.
				OpenGL.GL.ColorMask(false, false, false, false);

                d_glStateChanger.Disable(OpenGL.EnableCap.CullFace);
                d_glStateChanger.Enable(OpenGL.EnableCap.StencilTest);
                OpenGL.GL.StencilMask(0xFF);
                OpenGL.GL.Clear(OpenGL.ClearBufferMask.StencilBufferBit);
                OpenGL.GL.StencilFunc(OpenGL.StencilFunction.Always, 0x00, 0xFF);
                OpenGL.GL.StencilOp(OpenGL.StencilOp.Invert, OpenGL.StencilOp.Keep, OpenGL.StencilOp.Invert);
                OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, 0, d_vertexCount - d_postStencilVertexCount);

                var postStencilStart = d_vertexCount - d_postStencilVertexCount;
                OpenGL.GL.ColorMask(true, true, true, true);
                OpenGL.GL.StencilMask(0x00);
                OpenGL.GL.StencilFunc(OpenGL.StencilFunction.Equal, 0xFF, 0xFF);
                OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, postStencilStart, d_postStencilVertexCount);
            }
            else if(d_polygonFillRule == PolygonFillRule.NonZero)
            {
				//We use a stencil buffer to determine the insideness
				//of a fragment. We draw the front sides while increasing
				//stencil values and then draw backside while decreasing them.
				//A resulting 0 value means we are outside.
				OpenGL.GL.ColorMask(false, false, false, false);

                var solid_fill_count = d_vertexCount - d_postStencilVertexCount;
                var vertex_pos = 0;

                //Performing the back/front faces stencil incr and decr stencil op
                d_glStateChanger.Enable(OpenGL.EnableCap.CullFace);
                d_glStateChanger.Enable(OpenGL.EnableCap.StencilTest);
                OpenGL.GL.StencilMask(0xFF);
                OpenGL.GL.Clear(OpenGL.ClearBufferMask.StencilBufferBit);
                OpenGL.GL.StencilFunc(OpenGL.StencilFunction.Always, 0x00, 0xFF);

                OpenGL.GL.CullFace(OpenGL.CullFaceMode.Front);
                OpenGL.GL.StencilOp(OpenGL.StencilOp.Keep, OpenGL.StencilOp.Keep, OpenGL.StencilOp.IncrWrap);
                OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, vertex_pos, solid_fill_count);

                OpenGL.GL.CullFace(OpenGL.CullFaceMode.Back);
                OpenGL.GL.StencilOp(OpenGL.StencilOp.Keep, OpenGL.StencilOp.Keep, OpenGL.StencilOp.DecrWrap);
                OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, vertex_pos, solid_fill_count);

                vertex_pos += solid_fill_count;

                //Only needing culling for the back/front face stencil calculations
                d_glStateChanger.Disable(OpenGL.EnableCap.CullFace);

                OpenGL.GL.ColorMask(true, true, true, true);
                OpenGL.GL.StencilMask(0x00);

                if(d_postStencilVertexCount != 0)
                {
                    OpenGL.GL.StencilFunc(OpenGL.StencilFunction.NotEqual, 0x00, 0xFF);
                    OpenGL.GL.DrawArrays(OpenGL.PrimitiveType.Triangles, d_vertexCount - d_postStencilVertexCount, d_postStencilVertexCount);
                }
            }
        }
        
        #region Fields

        //! OpenGL vao used for the vertices
        protected int /*GLuint*/ d_verticesVAO;
        
        //! OpenGL vbo containing all vertex data
        protected int /*GLuint*/ d_verticesVBO;
        
        /// <summary>
        /// Pointer to the OpenGL state changer wrapper that was created inside the Renderer
        /// </summary>
        protected OpenGLBaseStateChangeWrapper d_glStateChanger;
        
        /// <summary>
        /// Size of the buffer that is currently in use
        /// </summary>
        protected int /*GLuint*/ d_bufferSize;

        #endregion
    };
}