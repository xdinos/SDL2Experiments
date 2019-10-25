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
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Defines interface to some surface that can be rendered to. Concrete 
    /// instances of objects that implement the RenderTarget interface are
    /// normally created via the Renderer object.
    /// </summary>
    public interface IRenderTarget : IDisposable
    {
        /// <summary>
        /// Event to be fired when the RenderTarget object's area has changed.
        /// Handlers are passed a const RenderTargetEventArgs reference with
        /// RenderTargetEventArgs::target set to the RenderTarget whose area changed.
        /// </summary>
        event EventHandler<RenderTargetEventArgs> AreaChanged;

        /// <summary>
        /// Draw geometry from the given GeometryBuffer onto the surface that
        /// this RenderTarget represents.
        /// </summary>
        /// <param name="buffer">
        /// GeometryBuffer object holding the geometry that should be drawn to the
        /// RenderTarget.
        /// </param>
        void Draw(GeometryBuffer buffer);

        /// <summary>
        /// Draw geometry from the given RenderQueue onto the surface that this RenderTarget represents.
        /// </summary>
        /// <param name="queue">
        /// RenderQueue object holding the geometry that should be drawn to the RenderTarget.
        /// </param>
        void Draw(RenderQueue queue);

        /// <summary>
        /// Set the area for this RenderTarget.  The exact action this function
        /// will take depends upon what the concrete class is representing.  For
        /// example, with a 'view port' style RenderTarget, this should set the area
        /// that the view port occupies on the display (or rendering window).
        /// </summary>
        /// <param name="area">
        /// Rect object describing the new area to be assigned to the RenderTarget.
        /// </param>
        /// <remarks>
        /// When implementing this function, you should be sure to fire the event
        /// RenderTarget::EventAreaChanged so that interested parties can know that
        /// the change has occurred.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// May be thrown if the RenderTarget does not support setting or changing
        /// its area, or if the area change can not be satisfied for some reason.
        /// </exception>
        void SetArea(Rectf area);

        /// <summary>
        /// Return the area defined for this RenderTarget.
        /// </summary>
        /// <returns>
        /// Rect object describing the currently defined area for this RenderTarget.
        /// </returns>
        Rectf GetArea();

        /// <summary>
        /// Return whether the RenderTarget is an implementation that caches
        /// actual rendered imagery.
        /// <para>
        /// Typically it is expected that texture based RenderTargets would return
        /// true in response to this call.  Other types of RenderTarget, like
        /// view port based targets, will more likely return false.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the RenderTarget does cache rendered imagery.
        /// - false if the RenderTarget does not cache rendered imagery.
        /// </returns>
        bool IsImageryCache();

        /// <summary>
        /// Activate the render target and put it in a state ready to be drawn to.
        /// </summary>
        /// <remarks>
        /// You MUST call this before doing any rendering - if you do not call this,
        /// in the unlikely event that your application actually works, it will
        /// likely stop working in some future version.
        /// </remarks>
        void Activate();

        /// <summary>
        /// Deactivate the render target after having completed rendering.
        /// </summary>
        /// <remarks>
        /// You MUST call this after you finish rendering to the target - if you do
        /// not call this, in the unlikely event that your application actually
        /// works, it will likely stop working in some future version.
        /// </remarks>
        void Deactivate();

        /// <summary>
        /// Take point \a p_in unproject it and put the result in \a p_out.
        /// Resulting point is local to GeometryBuffer \a buff.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        void UnprojectPoint(GeometryBuffer buff,
                            Lunatics.Mathematics.Vector2 pIn,
                            out Lunatics.Mathematics.Vector2 pOut);

        /// <summary>
        /// Returns a reference to the Renderer that is the owner/creator of this RenderTarget instance.
        /// </summary>
        /// <returns>
        /// A reference to the Renderer that is the owner/creator of this RenderTarget instance.
        /// </returns>
        Renderer GetOwner();

        /// <summary>
        /// Returns the current count of activations for this RenderTarget
        /// </summary>
        /// <returns>
        /// The current count of activations.
        /// </returns>
        int GetActivationCounter();

        /// <summary>
        /// Creates a view projection matrix for the OpenGL graphics library (Depth Range from -1 to 1) based
        /// on this RenderTarget's current settings.
        /// </summary>
        /// <returns>
        /// A freshly created OpenGL view projection matrix for this RenderTarget.
        /// </returns>
        Lunatics.Mathematics.Matrix CreateViewProjMatrixForOpenGL();

        /// <summary>
        /// Creates a view projection matrix for the Direct3D graphics library (Depth Range from 0 to 1) based
        /// on this RenderTarget's current settings.
        /// </summary>
        /// <returns>
        /// A freshly created Direct3D view projection matrix for this RenderTarget.
        /// </returns>
        Lunatics.Mathematics.Matrix CreateViewProjMatrixForDirect3D();


        /// <summary>
        /// Updates the view projection matrix of this Rendertarget.
        /// </summary>
        /// <param name="matrix"></param>
        void UpdateMatrix(Lunatics.Mathematics.Matrix matrix);

    }

    /// <summary>
    /// Defines interface to some surface that can be rendered to. Concrete 
    /// instances of objects that implement the RenderTarget interface are
    /// normally created via the Renderer object.
    /// </summary>
    public abstract class RenderTarget : /*EventSet,*/IRenderTarget
    {
        protected RenderTarget()
        {
            d_activationCounter = 0;
            d_area = Rectf.Zero;
            d_matrixValid = false;
            d_viewDistance = 0f;
            d_matrix = Lunatics.Mathematics.Matrix.Identity;
        }

        #region Implementation of IDisposable

        // TODO: virtual ~RenderTarget();

        ~RenderTarget()
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
            Disposed = true;
        }

        [Conditional("DEBUG")]
        protected void ThrowIfDisposed()
        {
            if (Disposed)
            {
                var objectName = GetType().Name;
                throw new ObjectDisposedException(objectName);
            }
        }

        protected bool Disposed;

        #endregion

        #region Implemantation of IRenderTarget

        /// <summary>
        /// Event to be fired when the RenderTarget object's area has changed.
        /// Handlers are passed a const RenderTargetEventArgs reference with
        /// RenderTargetEventArgs::target set to the RenderTarget whose area changed.
        /// </summary>
        public event EventHandler<RenderTargetEventArgs> AreaChanged;
        
        /// <summary>
        /// Draw geometry from the given GeometryBuffer onto the surface that
        /// this RenderTarget represents.
        /// </summary>
        /// <param name="buffer">
        /// GeometryBuffer object holding the geometry that should be drawn to the
        /// RenderTarget.
        /// </param>
        public virtual void Draw(GeometryBuffer buffer)
        {
            buffer.Draw();
        }

        /// <summary>
        /// Draw geometry from the given RenderQueue onto the surface that this RenderTarget represents.
        /// </summary>
        /// <param name="queue">
        /// RenderQueue object holding the geometry that should be drawn to the RenderTarget.
        /// </param>
        public virtual void Draw(RenderQueue queue)
        {
            queue.Draw();
        }

        /// <summary>
        /// Set the area for this RenderTarget.  The exact action this function
        /// will take depends upon what the concrete class is representing.  For
        /// example, with a 'view port' style RenderTarget, this should set the area
        /// that the view port occupies on the display (or rendering window).
        /// </summary>
        /// <param name="area">
        /// Rect object describing the new area to be assigned to the RenderTarget.
        /// </param>
        /// <remarks>
        /// When implementing this function, you should be sure to fire the event
        /// RenderTarget::EventAreaChanged so that interested parties can know that
        /// the change has occurred.
        /// </remarks>
        /// <exception cref="InvalidRequestException">
        /// May be thrown if the RenderTarget does not support setting or changing
        /// its area, or if the area change can not be satisfied for some reason.
        /// </exception>
        public virtual void SetArea(Rectf area)
        {
            d_area = area;
            d_matrixValid = false;
            
            FireEvent(AreaChanged, new RenderTargetEventArgs(this));
        }

        /// <summary>
        /// Return the area defined for this RenderTarget.
        /// </summary>
        /// <returns>
        /// Rect object describing the currently defined area for this RenderTarget.
        /// </returns>
        public Rectf GetArea()
        {
            return d_area;
        }

        /// <summary>
        /// Return whether the RenderTarget is an implementation that caches
        /// actual rendered imagery.
        /// <para>
        /// Typically it is expected that texture based RenderTargets would return
        /// true in response to this call.  Other types of RenderTarget, like
        /// view port based targets, will more likely return false.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the RenderTarget does cache rendered imagery.
        /// - false if the RenderTarget does not cache rendered imagery.
        /// </returns>
        public abstract bool IsImageryCache();

        /// <summary>
        /// Activate the render target and put it in a state ready to be drawn to.
        /// </summary>
        /// <remarks>
        /// You MUST call this before doing any rendering - if you do not call this,
        /// in the unlikely event that your application actually works, it will
        /// likely stop working in some future version.
        /// </remarks>
        public virtual void Activate()
        {
            var owner = GetOwner();

            owner.SetActiveRenderTarget(this);

            ++d_activationCounter;

            if (d_activationCounter == 0)
                owner.InvalidateGeomBufferMatrices(this);
        }

        /// <summary>
        /// Deactivate the render target after having completed rendering.
        /// </summary>
        /// <remarks>
        /// You MUST call this after you finish rendering to the target - if you do
        /// not call this, in the unlikely event that your application actually
        /// works, it will likely stop working in some future version.
        /// </remarks>
        public virtual void Deactivate()
        {
            
        }

        /// <summary>
        /// Take point \a p_in unproject it and put the result in \a p_out.
        /// Resulting point is local to GeometryBuffer \a buff.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        public abstract void UnprojectPoint(GeometryBuffer buff,
                                            Lunatics.Mathematics.Vector2 pIn,
                                            out Lunatics.Mathematics.Vector2 pOut);

        /// <summary>
        /// Returns a reference to the Renderer that is the owner/creator of this RenderTarget instance.
        /// </summary>
        /// <returns>
        /// A reference to the Renderer that is the owner/creator of this RenderTarget instance.
        /// </returns>
        public abstract Renderer GetOwner();

        /// <summary>
        /// Returns the current count of activations for this RenderTarget
        /// </summary>
        /// <returns>
        /// The current count of activations.
        /// </returns>
        public int GetActivationCounter()
        {
            return d_activationCounter;
        }

        /// <summary>
        /// Creates a view projection matrix for the OpenGL graphics library (Depth Range from -1 to 1) based
        /// on this RenderTarget's current settings.
        /// </summary>
        /// <returns>
        /// A freshly created OpenGL view projection matrix for this RenderTarget.
        /// </returns>
        public Lunatics.Mathematics.Matrix CreateViewProjMatrixForOpenGL()
        {
            var w = d_area.Width;
            var h = d_area.Height;

            // We need to check if width or height are zero and act accordingly to prevent running into issues
            // with divisions by zero which would lead to undefined values, as well as faulty clipping planes
            // This is mostly important for avoiding asserts
            var widthAndHeightNotZero = ( w != 0.0f ) && ( h != 0.0f);

            var fov = 30f;
            var fovInRad = fov * Lunatics.Mathematics.MathUtils.Pi / 180f;
            var fovY_halftan = (float)Math.Tan(fovInRad * 0.5f);

            var aspect = widthAndHeightNotZero ? w / h : 1.0f;
            var midx = widthAndHeightNotZero ? w * 0.5f : 0.5f;
            var midy = widthAndHeightNotZero ? h * 0.5f : 0.5f;
            d_viewDistance = midx / (aspect * /*d_yfov_tan*/fovY_halftan);

            

            var eye = new Lunatics.Mathematics.Vector3(midx, midy, -d_viewDistance);
            var center = new Lunatics.Mathematics.Vector3(midx, midy, 1f);
            var up = new Lunatics.Mathematics.Vector3(0f, -1f, 0f);

            var projectionMatrix = Lunatics.Mathematics.Matrix.PerspectiveFovRH(fovInRad, aspect, d_viewDistance * 0.5f, d_viewDistance * 2.0f);
            // Projection matrix abuse!
            var viewMatrix = Lunatics.Mathematics.Matrix.LookAtRH(eye, center, up);
            var tm = projectionMatrix*viewMatrix;
            return  Lunatics.Mathematics.Matrix.Multiply(
                    Lunatics.Mathematics.Matrix.LookAtRH(eye, center, up),
                    Lunatics.Mathematics.Matrix.PerspectiveFovRH(fovInRad, aspect, d_viewDistance * 0.5f, d_viewDistance * 2.0f));
            //return projectionMatrix * viewMatrix;
        }

        /// <summary>
        /// Creates a view projection matrix for the Direct3D graphics library (Depth Range from 0 to 1) based
        /// on this RenderTarget's current settings.
        /// </summary>
        /// <returns>
        /// A freshly created Direct3D view projection matrix for this RenderTarget.
        /// </returns>
        public Lunatics.Mathematics.Matrix CreateViewProjMatrixForDirect3D()
        {
            var w = d_area.Width;
            var h = d_area.Height;

            // We need to check if width or height are zero and act accordingly to prevent running into issues
            // with divisions by zero which would lead to undefined values, as well as faulty clipping planes
            // This is mostly important for avoiding asserts
            var widthAndHeightNotZero = ( w != 0.0f ) && ( h != 0.0f);

            var aspect = widthAndHeightNotZero ? w / h : 1.0f;
            var midx = widthAndHeightNotZero ? w * 0.5f : 0.5f;
            var midy = widthAndHeightNotZero ? h * 0.5f : 0.5f;
            d_viewDistance = midx / (aspect * d_yfov_tan);

            var eye = new Lunatics.Mathematics.Vector3(midx, midy, -d_viewDistance);
            var center = new Lunatics.Mathematics.Vector3(midx, midy, 1f);
            var up = new Lunatics.Mathematics.Vector3(0, -1, 0);

            // We need to have a projection matrix with its depth in clip space ranging from 0 to 1 for nearclip to farclip.
            // The regular OpenGL projection matrix would work too, but we would lose 1 bit of depth precision, which the following
            // manually filled matrix should fix:
            var fovy = 30f;
            var zNear = d_viewDistance * 0.5f;
            var zFar = d_viewDistance * 2.0f;
            var f = 1.0f / (float)Math.Tan(fovy * (float)Math.PI * 0.5f / 180.0f);
            var Q = zFar / (zNear - zFar);

            var projectionMatrixFloat = new[] 
            {
                f/aspect,           0.0f,               0.0f,           0.0f,
                0.0f,               f,                  0.0f,           0.0f,
                0.0f,               0.0f,               Q,              -1.0f,
                0.0f,               0.0f,               Q * zNear,      0.0f
            };

            var projectionMatrix = new Lunatics.Mathematics.Matrix(projectionMatrixFloat);

            // Projection matrix abuse!
            var viewMatrix = Lunatics.Mathematics.Matrix.LookAtRH(eye, center, up);

            //return projectionMatrix * viewMatrix;

            return Lunatics.Mathematics.Matrix.Multiply(
                    Lunatics.Mathematics.Matrix.LookAtRH(eye, center, up),
                    Lunatics.Mathematics.Matrix.PerspectiveFovRH(0.523598776f, aspect, d_viewDistance*0.5f, d_viewDistance*2.0f));

            //return  Lunatics.Mathematics.Matrix.Multiply(
            //    Lunatics.Mathematics.Matrix.LookAtRH(eye, center, up),
            //    Lunatics.Mathematics.Matrix.PerspectiveFovRH(30f, aspect, d_viewDistance * 0.5f, d_viewDistance * 2.0f));

            //return Lunatics.Mathematics.Matrix.Multiply(
            //    Lunatics.Mathematics.Matrix.PerspectiveFovLH(30f, aspect, d_viewDistance * 0.5f, d_viewDistance * 2.0f),
            //    Lunatics.Mathematics.Matrix.LookAtLH(eye, center, up));

        }


        /// <summary>
        /// Updates the view projection matrix of this Rendertarget.
        /// </summary>
        /// <param name="matrix"></param>
        public void UpdateMatrix(Lunatics.Mathematics.Matrix matrix)
        {
            d_matrix = matrix;

            d_matrixValid = true;
            //! This will trigger the RenderTarget to notify all of its GeometryBuffers to regenerate their matrices
            d_activationCounter = -1;
        }



        #endregion

        protected void FireEvent<T>(EventHandler<T> eventHandler, T args) where T : EventArgs
        {
            var handler = eventHandler;
            if (handler != null)
            {
                foreach (var @delegate in handler.GetInvocationList())
                    @delegate.DynamicInvoke(this, args);
                // handler(this, args);
            }
        }

        #region Fields

        /// <summary>
        /// The current number of activation of this RenderTarget. This is increased on every call to activate() and
        /// will in turn be used to remove the most common redundant matrix updates of GeometryBuffers.
        /// </summary>
        protected int d_activationCounter;

        //! holds defined area for the RenderTarget
        protected Rectf d_area;

        //! Determines if the matrix is up to date
        protected /*mutable*/ bool d_matrixValid;

        //! The view projection matrix
        protected /*mutable*/ /*glm::mat4*/ Lunatics.Mathematics.Matrix d_matrix;

        //! tracks viewing distance (this is set up at the same time as d_matrix)
        protected /*mutable*/ float d_viewDistance;
        
        //! The tangent of the y-axis FOV half-angle; used to calculate viewing distance.
        protected const float d_yfov_tan = 0.267949192431123f;

        #endregion
    }
}