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
using System.Linq;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Abstract class defining the basic required interface for Renderer objects.
    /// 
    /// Objects derived from Renderer are the means by which the GUI system
    /// interfaces with specific rendering technologies.  To use a rendering system
    /// or API to draw CEGUI imagery requires that an appropriate Renderer object be
    /// available.
    /// </summary>
    public abstract class Renderer
    {
	    //! This is the DPI value that was assumed up to CEGUI version 0.8.X
	    public static int ReferenceDpiValue = 96;


		public abstract int TextureTargetsCount { get; }

        public int GeometryBuffersRendered { get; set; }
        public int GeometryBuffersVertexRendered { get; set; }

        public int GeometryBuffersCount { get { return d_geometryBuffers.Count; } }
        public int GeometryBuffersVertexCount { get { return d_geometryBuffers.Sum(x => x.GetVertexCount()); } }

        // TODO: Destructor.
        //virtual ~Renderer() {}

        /// <summary>
        /// Returns the default RenderTarget object.  The default render target is
        /// is typically one that targets the entire screen (or rendering window).
        /// </summary>
        /// <returns>
        /// Reference to a RenderTarget object.
        /// </returns>
        public abstract IRenderTarget GetDefaultRenderTarget();

        /// <summary>
        /// Create a GeometryBuffer for textured geometry and return a reference to it.
        /// You should remove the GeometryBuffer from any RenderQueues and call destroyGeometryBuffer
        /// when you want to destroy the GeometryBuffer.
        /// </summary>
        /// <param name="renderMaterial"></param>
        /// <returns>GeometryBuffer object.</returns>
        public abstract GeometryBuffer CreateGeometryBufferTextured(RenderMaterial renderMaterial);

		/// <summary>
		/// Creates a GeometryBuffer for textured geometry with its default RenderMaterial and return a
		/// reference to it.
		/// You should remove the GeometryBuffer from any RenderQueues and call destroyGeometryBuffer
		/// when you want to destroy the GeometryBuffer.
		/// </summary>
		/// <returns>
		/// GeometryBuffer object.
		/// </returns>
		public GeometryBuffer CreateGeometryBufferTextured()
        {
            return CreateGeometryBufferTextured(CreateRenderMaterial(DefaultShaderType.Textured));
        }

        /// <summary>
        /// Creates a GeometryBuffer for coloured geometry and return a reference to it.
        /// You should remove the GeometryBuffer from any RenderQueues and call destroyGeometryBuffer
        /// when you want to destroy the GeometryBuffer.
        /// </summary>
        /// <param name="renderMaterial"></param>
        /// <returns>
        /// GeometryBuffer object.
        /// </returns>
        public abstract GeometryBuffer CreateGeometryBufferColoured(RenderMaterial renderMaterial);

        /// <summary>
        /// Creates a GeometryBuffer for coloured geometry with its default RenderMaterial and return a
        /// reference to it.
        /// You should remove the GeometryBuffer from any RenderQueues and call destroyGeometryBuffer
        /// when you want to destroy the GeometryBuffer.
        /// </summary>
        /// <returns>
        /// GeometryBuffer object.
        /// </returns>
        public GeometryBuffer CreateGeometryBufferColoured()
        {
            return CreateGeometryBufferColoured(CreateRenderMaterial(DefaultShaderType.Solid));
        }

        ///// <summary>
        ///// Create a new GeometryBuffer and return a reference to it.  You should
        ///// remove the GeometryBuffer from any RenderQueues and call
        ///// destroyGeometryBuffer when you want to destroy the GeometryBuffer.
        ///// </summary>
        ///// <returns>
        ///// GeometryBuffer object.
        ///// </returns>
        //public abstract GeometryBuffer CreateGeometryBuffer();

        /// <summary>
        /// Destroy a GeometryBuffer that was returned when calling the
        /// createGeometryBuffer function.  Before destroying any GeometryBuffer
        /// you should ensure that it has been removed from any RenderQueue that
        /// was using it.
        /// </summary>
        /// <param name="buffer">
        /// The GeometryBuffer object to be destroyed.
        /// </param>
        public void DestroyGeometryBuffer(GeometryBuffer buffer)
        {
#if DEBUG
            if (!d_geometryBuffers.Contains(buffer)) 
                return;
#endif
            d_geometryBuffers.Remove(buffer);
            buffer.Dispose();
        }

        /// <summary>
        /// Destroy all GeometryBuffer objects created by this Renderer.
        /// </summary>
        public void DestroyAllGeometryBuffers()
        {
            while (d_geometryBuffers.Count!=0)
                DestroyGeometryBuffer(d_geometryBuffers[0]);
        }

        /// <summary>
        /// Goes through all geometry buffers and updates their texture
        /// coordinates if the texture matches the supplied texture.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="scaleFactor"></param>
        public void UpdateGeometryBufferTexCoords(Texture texture, float scaleFactor)
        {
			throw new NotImplementedException();
        }

		/// <summary>
		/// Create a TextureTarget that can be used to cache imagery; this is a
		/// RenderTarget that does not lose it's content from one frame to another.
		/// 
		/// If the renderer is unable to offer such a thing, null should be returned.
		/// </summary>
		/// <param name="addStencilBuffer"></param>
		/// <returns>
		/// Pointer to a TextureTarget object that is suitable for caching imagery,
		/// or null if the renderer is unable to offer such a thing.
		/// </returns>
		public abstract ITextureTarget CreateTextureTarget(bool addStencilBuffer);

        /// <summary>
        /// Function that cleans up TextureTarget objects created with the
        /// createTextureTarget function.
        /// </summary>
        /// <param name="target">
        /// A pointer to a TextureTarget object that was previously returned from a
        /// call to createTextureTarget.
        /// </param>
        public abstract void DestroyTextureTarget(ITextureTarget target);

        /// <summary>
        /// Destory all TextureTarget objects created by this Renderer.
        /// </summary>
        public abstract void DestroyAllTextureTargets();

        /// <summary>
        /// Create a 'null' Texture object.
        /// </summary>
        /// <param name="name">
        /// String holding the name for the new texture.
        /// Texture names must be unique within the Renderer.
        /// </param>
        /// <returns>
        /// A newly created Texture object.  
        /// The returned Texture object has no size or imagery associated with it.
        /// </returns>
        /// <exception cref="AlreadyExistsException">
        /// thrown if a Texture object named \a name already exists within the system.
        /// </exception>
        public abstract Texture CreateTexture(string name);

        /// <summary>
        /// Create a Texture object using the given image file.
        /// </summary>
        /// <param name="name">
        /// String holding the name for the new texture.
        /// Texture names must be unique within the Renderer.
        /// </param>
        /// <param name="filename">
        /// String object that specifies the path and filename of the image file to
        /// use when creating the texture.
        /// </param>
        /// <param name="resourceGroup">
        /// String objet that specifies the resource group identifier to be passed
        /// to the resource provider when loading the texture file \a filename.
        /// </param>
        /// <returns>
        /// A newly created Texture object.
        /// The initial content of the texture memory is the requested image file.
        /// </returns>
        /// <exception cref="AlreadyExistsException">
        /// thrown if a Texture object named \a name already exists within the system.
        /// </exception>
        /// <remarks>
        /// Due to possible limitations of the underlying hardware, API or engine,
        /// the final size of the texture may not match the size of the loaded file.
        /// You can check the ultimate sizes by querying the Texture object
        /// after creation.
        /// </remarks>
        public abstract Texture CreateTexture(string name, string filename, string resourceGroup);

        /// <summary>
        /// Create a Texture object with the given pixel dimensions as specified by
        /// \a size.
        /// </summary>
        /// <param name="name">
        /// String holding the name for the new texture.  Texture names must be
        /// unique within the Renderer.
        /// </param>
        /// <param name="size">
        /// Size object that describes the desired texture size.
        /// </param>
        /// <returns>
        /// A newly created Texture object.  The initial contents of the texture
        /// memory is undefined.
        /// </returns>
        /// <remarks>
        /// Due to possible limitations of the underlying hardware, API or engine,
        /// the final size of the texture may not match the requested size.  You can
        /// check the ultimate sizes by querying the Texture object after creation.
        /// </remarks>
        /// <exception cref="AlreadyExistsException">
        /// thrown if a Texture object named \a name already exists within the system.
        /// </exception>
        public abstract Texture CreateTexture(string name, Sizef size);

        /// <summary>
        /// Destroy a Texture object that was previously created by calling the
        /// createTexture functions.
        /// </summary>
        /// <param name="texture">
        /// Texture object to be destroyed.
        /// </param>
        public abstract void DestroyTexture(Texture texture);

        /// <summary>
        /// Destroy a Texture object that was previously created by calling the
        /// createTexture functions.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the texture to destroy.
        /// </param>
        public abstract void DestroyTexture(string name);

        /// <summary>
        /// Destroy all Texture objects created by this Renderer.
        /// </summary>
        public abstract void DestroyAllTextures();

        /// <summary>
        /// Return a Texture object that was previously created by calling the createTexture functions.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// String holding the name of the Texture object to be returned.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// thrown if no Texture object named \a name exists within the system.
        /// </exception>
        public abstract Texture GetTexture(string name);

        /// <summary>
        /// Return whether a texture with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract bool IsTextureDefined(string name);

        /// <summary>
        /// Perform any operations required to put the system into a state ready
        /// for rendering operations to begin.
        /// </summary>
        public abstract void BeginRendering();

        /// <summary>
        /// Perform any operations required to finalise rendering.
        /// </summary>
        public abstract void EndRendering();

        /// <summary>
        /// Set the size of the display or host window in pixels for this Renderer object.
        /// 
        /// This is intended to be called by the System as part of the notification process 
        /// when display size changes are notified to it via the System::notifyDisplaySizeChanged function.
        /// </summary>
        /// <param name="size">
        /// Size object describing the dimesions of the current or host window in pixels.
        /// </param>
        /// <remarks>
        /// The Renderer implementation should not use this function other than to
        /// perform internal state updates on the Renderer and related objects.
        /// </remarks>
        public abstract void SetDisplaySize(Sizef size);

        /// <summary>
        /// Return the size of the display or host window in pixels.
        /// </summary>
        /// <returns>
        /// Size object describing the pixel dimesntions of the current display or
        /// host window.
        /// </returns>
        public abstract Sizef GetDisplaySize();

        /// <summary>
        /// Return the resolution of the display or host window in dots per inch.
        /// </summary>
        /// <returns>
        /// Vector2 object that describes the resolution of the display or host window in DPI.
        /// </returns>
        [Obsolete("")]
		public abstract Lunatics.Mathematics.Vector2 GetDisplayDotsPerInch();

        /// <summary>
        /// Return the pixel size of the maximum supported texture.
        /// </summary>
        /// <returns>
        /// Size of the maximum supported texture in pixels.
        /// </returns>
        public abstract int GetMaxTextureSize();

        /// <summary>
        /// Return identification string for the renderer module.
        /// </summary>
        /// <returns>
        /// String object holding text that identifies the Renderer in use.
        /// </returns>
        public abstract string GetIdentifierString();

        /// <summary>
        /// Creates a copy of the specified default shader type.
        /// </summary>
        /// <param name="shaderType">
        /// Specifies the type of CEGUI shader that the RenderMaterial should be based on
        /// </param>
        /// <returns>
        /// A copy of the specified default shader type.
        /// </returns>
        public abstract /*RefCounted<RenderMaterial>*/ RenderMaterial CreateRenderMaterial(DefaultShaderType shaderType);

        /// <summary>
        /// Marks all matrices of all GeometryBuffers as dirty, so that they will be updated before their next usage.
        /// This is a special function that will only be used if a RenderTarget has been rendered more than the amount
        /// of numbers that can be stored in the counter, at which point the counter restarts at 0. This is necessary
        /// to ensure that no Matrix will be reused although it actually would need updating (for example in the case
        /// the Buffer was not rendered for exactly the amount of maximum countable times, and is updated again exactly at
        /// the same count)
        /// </summary>
        /// <param name="renderTarget"></param>
        public void InvalidateGeomBufferMatrices(IRenderTarget renderTarget)
        {
            foreach (var geometryBuffer in d_geometryBuffers)
            {
                if (geometryBuffer.GetLastRenderTarget() == renderTarget)
                    geometryBuffer.InvalidateMatrix();
            }
        }

        public virtual void UploadBuffers(RenderingSurface surface) { }
        public virtual void UploadBuffers(IEnumerable<GeometryBuffer> buffers) {}

		/// <summary>
		/// Sets the active render target.
		/// </summary>
		/// <param name="renderTarget">
		/// The active RenderTarget.
		/// </param>
		public void SetActiveRenderTarget(IRenderTarget renderTarget)
        {
            d_activeRenderTarget = renderTarget;
        }
      
        /// <summary>
        /// Retruns the active render target.
        /// </summary>
        /// <returns>
        /// The active RenderTarget.
        /// </returns>
        public IRenderTarget GetActiveRenderTarget()
        {
            return d_activeRenderTarget;
        }

        /// <summary>
        /// Sets the currently active view projection matrix.
        /// </summary>
        /// <param name="viewProjectionMatrix">
        /// The view projection matrix that should be set as the new active matrix.
        /// </param>
        public virtual void SetViewProjectionMatrix(Lunatics.Mathematics.Matrix viewProjectionMatrix)
        {
            d_viewProjectionMatrix = viewProjectionMatrix;
        }

        /// <summary>
        /// Returns the currently active view projection matrix.
        /// </summary>
        /// <returns>
        /// The currently active view projection matrix.
        /// </returns>
        public Lunatics.Mathematics.Matrix GetViewProjectionMatrix()
        {
            return d_viewProjectionMatrix;
        }


        /*!
   \brief
       Returns if the texture coordinate system is vertically flipped or not. The original of a
       texture coordinate system is typically located either at the the top-left or the bottom-left.
       CEGUI, Direct3D and most rendering engines assume it to be on the top-left. OpenGL assumes it to
       be at the bottom left.        

       This function is intended to be used when generating geometry for rendering the TextureTarget
       onto another surface. It is also intended to be used when trying to use a custom texture (RTT)
       inside CEGUI using the Image class, in order to determine the Image coordinates correctly.

   \return
       - true if flipping is required: the texture coordinate origin is at the bottom left
       - false if flipping is not required: the texture coordinate origin is at the top left
   */
        public abstract bool IsTexCoordSystemFlipped();

		/// <summary>
		/// Gets the Font scale factor to be used when rendering scalable Fonts.
		/// </summary>
		/// <returns>
		/// The currently set Font scale factor
		/// </returns>
		public float GetFontScale()
        {
	        return d_fontScale;
        }

		/*!
   \brief
	   Sets the Font scale factor to be used when rendering scalable Fonts.

	   This updates all Fonts but will not invalidate the Windows. If you
	   want to also invalidate all Windows to be affected by all Fonts,
	   you have to do this after setting the new Font scale manually.
   */
		void setFontScale(float fontScale)
		{
			throw new NotImplementedException();
		}

		/*!
\brief
	Calculates and returns the font scale factor, based on the 
	reference DPI value of 96, which was assumed as fixed DPI up to
	CEGUI 0.8.X. Point sizes are rendered in CEGUI based on this fixed
	DPI. If you want to render point sizes according to a higher DPI,
	use this function together with setFontScale.

\return
	The result of dpiValue / ReferenceDpiValue

\see Renderer::setFontScale
\see System::getSystemDPI
*/
		public static float DpiToFontScale(float dpiValue)
		{
			return dpiValue / ReferenceDpiValue;
		}

	/// <summary>
	/// Adds a created GeometryBuffer, which was returned when calling one of the
	/// createGeometryBuffer functions, to the list of GeometryBuffers.
	/// </summary>
	/// <param name="geometryBuffer">
	/// The GeometryBuffer object to be destroyed.
	/// </param>
	protected void AddGeometryBuffer(GeometryBuffer geometryBuffer)
        {
            d_geometryBuffers.Add(geometryBuffer);
        }

        /// <summary>
        /// The currently active RenderTarget
        /// </summary>
        protected IRenderTarget d_activeRenderTarget;

        /// <summary>
        /// The currently active view projection matrix 
        /// </summary>
        protected /*glm::mat4*/Lunatics.Mathematics.Matrix d_viewProjectionMatrix;

        //! The Font scale factor to be used when rendering Fonts (except Bitmap Fonts).
        private float d_fontScale;

	/// <summary>
	/// Container used to track geometry buffers.
	/// </summary>
	private readonly List<GeometryBuffer> d_geometryBuffers = new List<GeometryBuffer>();
    }
}