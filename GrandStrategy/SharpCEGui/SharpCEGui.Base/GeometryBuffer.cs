#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Enumerated type that contains the valid options to specify a vertex attribute
    /// of a vertex used in CEGUI
    /// </summary>
    public enum VertexAttributeType
    {
        /// <summary>
        /// Position 0 attribute
        /// </summary>
        Position0,

        /// <summary>
        /// Colour 0 attribute
        /// </summary>
        Color0,

        /// <summary>
        /// Texture coordinate 0 attribute
        /// </summary>
        TexCoord0
    }

    /// <summary>
    /// Enumerated type that contains the valid options for the fill rule mode. The
    /// fill rule defines how overlaps of the polygon defined by this GeometryBuffer
    /// should be rendered. For further information see "fill-rule" in the SVG
    /// standard: http://www.w3.org/TR/SVGTiny12/painting.html#FillRuleProperty
    /// </summary>
    public enum PolygonFillRule
    {
        /// <summary>
        /// Draw the polygon normally - without a fill-rule.
        /// </summary>
        None,

        /// <summary>
        /// Uses the nonzero rule to determine how the polygon is to be filled.
        /// </summary>
        NonZero,
        
        /// <summary>
        /// Uses the  evenodd rule to determine how the polygon is to be filled.
        /// </summary>
        EvenOdd
    }

    /// <summary>
    /// Abstract class defining the interface for objects that buffer geometry for
    /// later rendering.
    /// </summary>
    public abstract class GeometryBuffer : IDisposable
    {
        #region Implementation of IDisposable

        // TODO: virtual ~GeometryBuffer();

        ~GeometryBuffer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        [Conditional("DEBUG")]
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                var objectName = GetType().Name;
                throw new ObjectDisposedException(objectName);
            }
        }

        private bool _disposed;

        #endregion

        /// <summary>
        /// Draw the geometry buffered within this GeometryBuffer object.
        /// </summary>
        public abstract void Draw();
        
        /// <summary>
        /// Set the translation to be applied to the geometry in the buffer when it
        /// is subsequently rendered.
        /// </summary>
        /// <param name="translation">
        /// Vector3 describing the three axis translation vector to be used.
        /// </param>
        public virtual void SetTranslation(Lunatics.Mathematics.Vector3 translation)
        {
            if (d_translation != translation)
            {
                d_translation = translation;
                d_matrixValid = false;
            }
        }

        /// <summary>
        /// Set the rotations to be applied to the geometry in the buffer when it is
        /// subsequently rendered.
        /// </summary>
        /// <param name="rotationQuat">
        /// Quaternion describing the rotation to be used.
        /// </param>
        public virtual void SetRotation(Lunatics.Mathematics.Quaternion rotationQuat)
        {
            if (d_rotation != rotationQuat)
            {
                d_rotation = rotationQuat;
                d_matrixValid = false;
            }
        }

        /// <summary>
        /// Set the scaling to be applied to the geometry in the buffer when it is
        /// subsequently rendered.
        /// </summary>
        /// <param name="scale">
        /// Vector3 describing the scale to be used.
        /// </param>
        public virtual void SetScale(Lunatics.Mathematics.Vector3 scale)
        {
            if (d_scale != scale)
            {
                d_scale = scale;
                d_matrixValid = false;
            }
        }

        /// <summary>
        /// Set the scaling to be applied to the geometry in the buffer when it is
        /// subsequently rendered.
        /// </summary>
        /// <param name="scale">
        /// Vector2 describing the x and y scale to be used.
        /// </param>
        public void SetScale(Lunatics.Mathematics.Vector2 scale)
        {
            SetScale(new Lunatics.Mathematics.Vector3(scale, 0f));
        }

        /// <summary>
        /// Set the pivot point to be used when applying the rotations.
        /// </summary>
        /// <param name="p">
        /// Vector3 describing the location of the pivot point to be used when
        /// applying the rotation to the geometry.
        /// </param>
        public virtual void SetPivot(Lunatics.Mathematics.Vector3 p)
        {
            if (d_pivot != p)
            {
                d_pivot = p;
                d_matrixValid = false;
            }
        }

        /// <summary>
        /// Set a custom transformation matrix that will be applied to the
        /// geometry in the buffer after all the other transformations.
        /// </summary>
        /// <param name="transformation">
        /// 4x4 Matrix that describes the transformation.
        /// </param>
        public void SetCustomTransform(Lunatics.Mathematics.Matrix transformation)
        {
            if (d_customTransform != transformation)
            {
                d_customTransform = transformation;
                d_matrixValid = false;
            }
        }

        /// <summary>
        /// Set the clipping region to be used when rendering this buffer. 
        /// The clipping region for actual rendering will be stored extra after 
        /// clamping the input region.
        /// </summary>
        /// <param name="region"></param>
        public virtual void SetClippingRegion(Rectf region)
        {
            d_clippingRegion = region;

            d_preparedClippingRegion.Top = Math.Max(0.0f, d_clippingRegion.Top);
            d_preparedClippingRegion.Bottom = Math.Max(0.0f, d_clippingRegion.Bottom);
            d_preparedClippingRegion.Left = Math.Max(0.0f, d_clippingRegion.Left);
            d_preparedClippingRegion.Right = Math.Max(0.0f, d_clippingRegion.Right);
        }

        /// <summary>
        /// Gets the set clipping region for this buffer.
        /// </summary>
        /// <returns></returns>
        public Rectf GetClippingRegion()
        {
            return d_clippingRegion;
        }

        /// <summary>
        /// Gets the prepared clipping region to be used when rendering this buffer.
        /// </summary>
        /// <returns></returns>
        public Rectf GetPreparedClippingRegion()
        {
            return d_preparedClippingRegion;
        }

        /// <summary>
        /// Sets the fill rule that should be used when rendering the geometry.
        /// </summary>
        /// <param name="fillRule">
        /// The fill rule that should be used when rendering the geometry.
        /// </param>
        public void SetStencilRenderingActive(PolygonFillRule fillRule)
        {
            d_polygonFillRule = fillRule;
        }

        /// <summary>
        /// Sets the number of vertices that should be rendered after the stencil buffer was filled.
        /// </summary>
        /// <param name="vertexCount">
        /// The number of vertices that should be rendered after the stencil buffer was filled.
        /// </param>
        public void SetStencilPostRenderingVertexCount(int vertexCount)
        {
            d_postStencilVertexCount = vertexCount;
        }

        /// <summary>
        /// Append a number of vertices from an array to the GeometryBuffer.
        /// </summary>
        /// <param name="vertexData">
        /// Pointer to an array of Vertex objects that describe the vertices that
        /// are to be added to the GeometryBuffer.
        /// </param>
        /// <param name="vertexCount">
        /// The number of Vertex objects from the array \a vbuff that are to be
        /// added to the GeometryBuffer.
        /// </param>
        public virtual void AppendGeometry(float[] vertexData, int vertexCount)
        {
            //d_vertexData.reserve( d_vertexData.size() + array_size);
            //std::copy(vertex_data, vertex_data + array_size, std::back_inserter(d_vertexData));
            d_vertexData.AddRange(vertexData);

            // Update size of geometry buffer
            d_vertexCount = d_vertexData.Count/GetVertexAttributeElementCount();
        }

        /// <summary>
        /// Append a single vertex to the buffer.
        /// </summary>
        /// <param name="vertex">
        /// Vertex object describing the vertex to be added to the GeometryBuffer.
        /// </param>
        public virtual void AppendVertex(TexturedColouredVertex vertex)
        {
            AppendGeometry(new[]
                           {
                                   vertex.Position.X,
                                   vertex.Position.Y,
                                   vertex.Position.Z,
                                   vertex.Colour.X,
                                   vertex.Colour.Y,
                                   vertex.Colour.Z,
                                   vertex.Colour.W,
                                   vertex.TextureCoordinates.X,
                                   vertex.TextureCoordinates.Y
                           }, 9);
        }

        /// <summary>
        /// Append a single vertex to the buffer.
        /// </summary>
        /// <param name="vertex">
        /// Vertex object describing the vertex to be added to the GeometryBuffer.
        /// </param>
        public virtual void AppendVertex(ColouredVertex vertex)
        {
            AppendGeometry(new[]
                           {
                                   vertex.Position.X,
                                   vertex.Position.Y,
                                   vertex.Position.Z,
                                   vertex.Colour.X,
                                   vertex.Colour.Y,
                                   vertex.Colour.Z,
                                   vertex.Colour.W
                           }, 7);
        }

        /// <summary>
        /// Appends vertices with colour attributes from an std::vector to the GeometryBuffer.
        /// </summary>
        /// <param name="colouredVertices">
        /// The vector of ColouredVertices.
        /// </param>
        public virtual void AppendGeometry(ColouredVertex[] colouredVertices)
        {
            // Create a temporary array to contain our data
            const int vertexDataSize = 7;
            var fullArraySize = vertexDataSize*colouredVertices.Length;
            var vertexData = new float[fullArraySize];

            // Add the vertex data in their default order into an array
            var currentIndex = 0;
            foreach (var vs in colouredVertices)
            {
                // Add all the elements in the default order for textured and coloured
                // geometry into the vector
                vertexData[currentIndex + 0] = vs.Position.X;
                vertexData[currentIndex + 1] = vs.Position.Y;
                vertexData[currentIndex + 2] = vs.Position.Z;
                vertexData[currentIndex + 3] = vs.Colour.X;
                vertexData[currentIndex + 4] = vs.Colour.Y;
                vertexData[currentIndex + 5] = vs.Colour.Z;
                vertexData[currentIndex + 6] = vs.Colour.W;

                currentIndex += vertexDataSize;
            }

            // Append the prepared geometry data
            AppendGeometry(vertexData, fullArraySize);
        }

        /// <summary>
        /// Appends vertices with texture coordinate and colour attributes from an std::vector to 
        /// the GeometryBuffer.
        /// </summary>
        /// <param name="texturedVertices">
        /// The vector of TexturedColouredVertices.
        /// </param>
        public virtual void AppendGeometry(TexturedColouredVertex[] texturedVertices)
        {
            // Create a temporary array to contain our data
            const int vertexDataSize = 9;
            var fullArraySize = vertexDataSize*texturedVertices.Length;
            var vertexData = new float[fullArraySize];

            // Add the vertex data in their default order into an array
            var currentIndex = 0;
            foreach (var vs in texturedVertices)
            {
                // Add all the elements in the default order for textured and coloured
                // geometry into the vector
                vertexData[currentIndex + 0] = vs.Position.X;
                vertexData[currentIndex + 1] = vs.Position.Y;
                vertexData[currentIndex + 2] = vs.Position.Z;
                vertexData[currentIndex + 3] = vs.Colour.X;
                vertexData[currentIndex + 4] = vs.Colour.Y;
                vertexData[currentIndex + 5] = vs.Colour.Z;
                vertexData[currentIndex + 6] = vs.Colour.W;
                vertexData[currentIndex + 7] = vs.TextureCoordinates.X;
                vertexData[currentIndex + 8] = vs.TextureCoordinates.Y;

                currentIndex += vertexDataSize;
            }

            // Append the prepared geometry data
            AppendGeometry(vertexData, fullArraySize);
        }

        /// <summary>
        /// A helper function that sets a texture parameter of the RenderMaterial of this
        /// Geometrybuffer.
        /// </summary>
        /// <param name="parameterName">
        /// Name of the parameter as used inside the shader program. The regular CEGUI
        /// texture-parameter that is used inside CEGUI's materials is called "texture0".
        /// </param>
        /// <param name="texture">
        /// Pointer to a Texture object that shall be used for subsequently added
        /// vertices.  This may be 0, in which case texturing will be disabled for
        /// subsequently added vertices.
        /// </param>
        public virtual void SetTexture(string parameterName, Texture texture)
        {
            var shaderParameterBindings = d_renderMaterial.GetShaderParamBindings();
            shaderParameterBindings.SetParameter(parameterName, texture);
        }

        /// <summary>
        /// Clear all buffered data and reset the GeometryBuffer to the default
        /// state. This excludes resettings the vertex attributes.
        /// </summary>
        public virtual void Reset()
        {
            d_vertexData.Clear();
            d_clippingActive = true;
        }

        /// <summary>
        /// Returns the vertex count of this GeometryBuffer, which is determined based
        /// on the the used vertex layout and the size of the vertex data
        /// </summary>
        /// <returns>
        /// The number of vertices that have been appended to this GeometryBuffer.
        /// </returns>
        public virtual int GetVertexCount()
        {
            return d_vertexCount;
        }

        /// <summary>
        /// Returns the total number of floats used by the attributes of the
        /// current vertex layout.
        /// </summary>
        /// <returns>
        /// The total number of floats used by the attributes of the current vertex layout.
        /// </returns>
        public virtual int GetVertexAttributeElementCount()
        {
            var count = 0;

            var attributeCount = d_vertexAttributes.Count;
            for (var i = 0; i < attributeCount; ++i)
            {
                switch (d_vertexAttributes[i])
                {
                    case VertexAttributeType.Position0:
                        count += 3;
                        break;
                    case VertexAttributeType.Color0:
                        count += 4;
                        break;
                    case VertexAttributeType.TexCoord0:
                        count += 2;
                        break;
                }
            }

            return count;
        }

        /// <summary>
        /// Set the RenderEffect to be used by this GeometryBuffer.
        /// </summary>
        /// <param name="effect">
        /// Pointer to the RenderEffect to be used during renderng of the
        /// GeometryBuffer.  May be 0 to remove a previously added RenderEffect.
        /// </param>
        /// <remarks>
        /// When adding a RenderEffect, the GeometryBuffer <em>does not</em> take
        /// ownership of, nor make a copy of, the passed RenderEffect - this means
        /// you need to be careful not to delete the RenderEffect if it might still
        /// be in use!
        /// </remarks>
        public virtual void SetRenderEffect(RenderEffect effect)
        {
            d_effect = effect;
        }

        /// <summary>
        /// Return the RenderEffect object that is assigned to this GeometryBuffer or null if none.
        /// </summary>
        /// <returns></returns>
        public virtual RenderEffect GetRenderEffect()
        {
            return d_effect;
        }

        /// <summary>
        /// The blend mode setting is not a 'state' setting, but is used for \e all
        /// geometry added to the buffer regardless of when the blend mode is set.
        /// </summary>
        /// <param name="mode">
        /// One of the BlendMode enumerated values indicating the blending mode to be used.
        /// </param>
        public virtual void SetBlendMode(BlendMode mode)
        {
            d_blendMode = mode;
        }

        /// <summary>
        /// Return the blend mode that is set to be used for this GeometryBuffer.
        /// </summary>
        /// <returns>
        /// One of the BlendMode enumerated values indicating the blending mode
        /// that will be used when rendering all geometry added to this
        /// GeometryBuffer object.
        /// </returns>
        public virtual BlendMode GetBlendMode()
        {
            return d_blendMode;
        }

        /// <summary>
        /// Set whether clipping will be active for subsequently added vertices.
        /// </summary>
        /// <param name="value">
        /// - true if vertices added after this call should be clipped to the
        ///   clipping region defined for this GeometryBuffer.
        /// - false if vertices added after this call should not be clipped
        ///   (other than to the edges of rendering target.
        /// </param>
        public virtual void SetClippingActive(bool value)
        {
            d_clippingActive = value;
        }

        /// <summary>
        /// Return whether clipping will be used for the current batch
        /// of vertices being defined.
        /// </summary>
        /// <returns>
        /// - true if vertices subsequently added to the GeometryBuffer will
        ///   be clipped to the clipping region defined for this GeometryBuffer.
        /// - false if vertices subsequently added will not be clippled (other than
        ///   to the edges of the rendering target).
        /// </returns>
        public virtual bool IsClippingActive()
        {
            return d_clippingActive;
        }

        /// <summary>
        /// Resets the vertex attributes that were set for the vertices of this
        /// GeometryBuffer.
        /// </summary>
        public void ResetVertexAttributes()
        {
            d_vertexAttributes.Clear();
        }

        /// <summary>
        /// Adds a vertex attributes to the list of vertex attributes. The vertex
        /// attributes are used to describe the layout of the verrex data. The
        /// order in which the attributes are added is the same order in which the
        /// data has to be aligned for the vertex. This has be done before adding
        /// actual vertex data to the GeometryBuffer.
        /// </summary>
        /// <param name="attribute">
        /// The attribute that should be added to the list of vertex attributes
        /// describing the vertices of this GeometryBuffer.
        /// </param>
        public void AddVertexAttribute(VertexAttributeType attribute)
        {
            d_vertexAttributes.Add(attribute);
        }

        /*
        \brief
            Returns the RenderMaterial that is currently used by this GeometryBuffer.

        \return
            A reference to the RenderMaterial that is used by this GeometryBuffer.
        */

        public /*RefCounted<RenderMaterial>*/ RenderMaterial GetRenderMaterial()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a new RenderMaterial to be used by this GeometryBuffer.
        /// </summary>
        /// <param name="render_material">
        /// A reference to the RenderMaterial that will be set to be used by this
        /// GeometryBuffer.
        /// </param>
        public void SetRenderMaterial( /*RefCounted<RenderMaterial>*/ RenderMaterial render_material)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the alpha for this window
        /// </summary>
        /// <param name="alpha">
        /// The new alpha value in the range 0.f-1.f
        /// </param>
        public void SetAlpha(float alpha)
        {
            d_alpha = alpha;
        }

        /// <summary>
        /// Gets the current alpha value
        /// </summary>
        /// <returns></returns>
        public float GetAlpha()
        {
            return d_alpha;
        }

        /// <summary>
        /// Invalidates the local matrix. This should be called whenever anything extraordinary
        /// that requires to recalculate the matrix has occured
        /// </summary>
        public void InvalidateMatrix()
        {
            d_matrixValid = false;
        }

        //TODO DOCU
        public IRenderTarget GetLastRenderTarget()
        {
            return d_lastRenderTarget;
        }

        /// <summary>
        /// Returns if the data (matrix etc) from the RenderTarget that was last used is still valid or not.
        /// </summary>
        /// <param name="activeRenderTarget"></param>
        /// <returns>
        /// True if still valid. False if invalid.
        /// </returns>
        public bool IsRenderTargetDataValid(IRenderTarget activeRenderTarget)
        {
            //! The data received from the RenderTarget is only valid if:
            //! 1. The RenderTarget is the same as the one used the last time
            //! 2. The GeometryBuffer never skipped a RenderTarget activation (Checked via counter)
            return (d_lastRenderTarget == activeRenderTarget) &&
                   (d_lastRenderTargetActivationCount + 1 == activeRenderTarget.GetActivationCounter());
        }

        //TODO DOCU
        public void UpdateRenderTargetData(IRenderTarget activeRenderTarget)
        {
            d_lastRenderTarget = activeRenderTarget;
            d_lastRenderTargetActivationCount = activeRenderTarget.GetActivationCounter();
        }

        /// <summary>
        /// Calculates and returns the model matrix for this GeometryBuffer.
        /// </summary>
        /// <returns>
        /// The model matrix for this GeometryBuffer.
        /// </returns>
        public Lunatics.Mathematics.Matrix GetModelMatrix()
        {
            #region Original

            //glm::mat4 modelMatrix = glm::translate(glm::mat4(1.0f), d_translation + d_pivot);
            var modelMatrix = Lunatics.Mathematics.Matrix.Translation(d_translation + d_pivot);

            //const glm::mat4 scale_matrix(glm::scale(glm::mat4(1.0f), d_scale));
            var scaleMatrix = Lunatics.Mathematics.Matrix.Scaling(d_scale);
            //modelMatrix *= glm::mat4_cast(d_rotation) * scale_matrix;
            //modelMatrix *= Lunatics.Mathematics.Matrix.RotationQuaternion(d_rotation) * scaleMatrix;

            //const glm::mat4 translMatrix = glm::translate(glm::mat4(1.0f), -d_pivot);
            var translMatrix = Lunatics.Mathematics.Matrix.Translation(-d_pivot);
            
            modelMatrix *= translMatrix * d_customTransform;
            
            #endregion

            //var modelMatrix = Lunatics.Mathematics.Matrix.Translation(d_pivot);
            //var scaleMatrix = Lunatics.Mathematics.Matrix.Scaling(d_scale);
            //modelMatrix *= Lunatics.Mathematics.Matrix.RotationQuaternion(d_rotation) * scaleMatrix;
            //var translMatrix = Lunatics.Mathematics.Matrix.Translation(-d_pivot);
            //modelMatrix *= translMatrix * d_customTransform;
            //modelMatrix *= Lunatics.Mathematics.Matrix.Translation(d_translation);

            return modelMatrix;
        }

        protected GeometryBuffer( /*RefCounted<RenderMaterial>*/ RenderMaterial renderMaterial)
        {
            d_renderMaterial = renderMaterial;
            d_translation = Lunatics.Mathematics.Vector3.Zero;
            d_rotation = Lunatics.Mathematics.Quaternion.Identity;
            d_scale = Lunatics.Mathematics.Vector3.One;
            d_pivot = Lunatics.Mathematics.Vector3.Zero;
            d_customTransform = Lunatics.Mathematics.Matrix.Identity;
            d_matrixValid = false;
            d_lastRenderTarget = null;
            d_lastRenderTargetActivationCount = 0;
            d_blendMode = BlendMode.Normal;
            d_polygonFillRule = PolygonFillRule.None;
            d_postStencilVertexCount = 0;
            d_effect = null;
            d_alpha = 1.0f;
            d_clippingActive = false;
            d_clippingRegion = Rectf.Zero;
            d_preparedClippingRegion = Rectf.Zero;
        }

        #region Fields

        //! Reference to the RenderMaterial used for this GeometryBuffer
        protected /*RefCounted<RenderMaterial>*/ RenderMaterial d_renderMaterial;

        //! type of container used to store the geometry's vertex data
        // typedef std::vector<float>  VertexData;

        //! The container in which the vertex data is stored.
        protected List<float> d_vertexData = new List<float>();

        //! The vertex count which is determined based on the used vertex layout
        protected int d_vertexCount;

        /// <summary>
        /// A vector of the attributes of the vertices of this GeometryBuffer. The order
        /// in which they were added to the vector is used to define the alignment of the
        /// vertex data.
        /// </summary>
        protected List<VertexAttributeType> d_vertexAttributes = new List<VertexAttributeType>();

        //! translation vector
        protected Lunatics.Mathematics.Vector3 d_translation;

        //! rotation quaternion
        protected Lunatics.Mathematics.Quaternion d_rotation;

        //! scaling vector
        protected Lunatics.Mathematics.Vector3 d_scale;

        //! pivot point for rotation
        protected Lunatics.Mathematics.Vector3 d_pivot;

        //! custom transformation matrix
        protected Lunatics.Mathematics.Matrix d_customTransform;

        /// <summary>
        /// true, when there have been no translations, rotations or other transformations applied to the GeometryBuffer,
        /// as well as when it is guaranteed that the view projection matrix of the RenderTarget has been unchanged
        /// since the last update.
        /// </summary>
        protected /*mutable*/ bool d_matrixValid;

        //! The RenderTarget that this GeometryBuffer's matrix was last updated for
        protected /*mutable*/ IRenderTarget d_lastRenderTarget;

        //! The activation number of the RenderTarget that this GeometryBuffer's matrix was last updated for
        protected /*mutable*/ int d_lastRenderTargetActivationCount;

        //! The BlendMode to use when rendering this GeometryBuffer.
        protected BlendMode d_blendMode;

        //! The fill rule that should be used when rendering the geometry.
        protected PolygonFillRule d_polygonFillRule;

        //! The amount of vertices that need to be rendered after rendering to the stencil buffer.
        protected int d_postStencilVertexCount;

        //! RenderEffect that will be used by the GeometryBuffer
        protected RenderEffect d_effect;

        /// <summary>
        /// The last clipping region that was set. Negative values in this one are
        /// not clamped to 0.
        /// </summary>
        protected Rectf d_clippingRegion;

        /// <summary>
        /// Clipping region clamped to 0, for usage in rendering
        /// </summary>
        protected Rectf d_preparedClippingRegion;

        //! True if clipping will be active for the current batch
        protected bool d_clippingActive;

        //! The alpha value which will be applied to the whole buffer when rendering
        protected float d_alpha;

        #endregion
    }
}