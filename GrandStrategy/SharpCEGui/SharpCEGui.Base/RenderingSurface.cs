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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that represents a surface that can have geometry based imagery drawn
    /// to it.
    ///  
    /// A RenderingSurface has a number of queues that can be used for rendering;
    /// normal window rendering will typically be done on <see cref="RenderQueueId.RQ_BASE"/> queue, things that
    /// are overlaid everything else are rendered to <see cref="RenderQueueId.RQ_OVERLAY"/>.
    /// \par
    /// The event <see cref="RenderQueueStarted"/> is fired before each queue is rendered and
    /// the event <see cref="RenderQueueEnded"/> is fired after each queue is rendered.
    /// \note
    /// For performance reasons, events are only fired for queues that are in use;
    /// these are queues that have had some interaction - such as clearing or adding
    /// geometry.
    /// </summary>
    public class RenderingSurface
    {
        /// <summary>
        /// Event fired when rendering of a RenderQueue begins for the RenderingSurface.
        /// Handlers are passed a const RenderQueueEventArgs reference with
        /// RenderQueueEventArgs::queueID set to one of the ::RenderQueueID
        /// enumerated values indicating the queue that is about to start
        /// rendering.
        /// </summary>
        public event EventHandler<RenderQueueEventArgs> RenderQueueStarted;

        /// <summary>
        /// Event fired when rendering of a RenderQueue completes for the
        /// RenderingSurface.
        /// Handlers are passed a const RenderQueueEventArgs reference with
        /// RenderQueueEventArgs::queueID set to one of the ::RenderQueueID
        /// enumerated values indicating the queue that has completed rendering.
        /// </summary>
        public event EventHandler<RenderQueueEventArgs> RenderQueueEnded;

        /// <summary>
        /// Constructor for RenderingSurface objects.
        /// </summary>
        /// <param name="target">
        /// RenderTarget object that will receive rendered output from the
        /// RenderingSurface being created.
        /// </param>
        /// <remarks>
        /// The RenderingSurface does not take ownership of \a target. When the
        /// RenderingSurface is finally destroyed, the RenderTarget will not have
        /// been destroyed, and it should be destoyed by whover created it, if that
        /// is desired. One reason for this is that there is not an exclusive one
        /// to one mapping from RenderingSurface to RenderTarget objects; it's
        /// entirely feasable that multiple RenderingSurface objects could be
        /// targetting a shared RenderTarget).
        /// </remarks>
        public RenderingSurface(IRenderTarget target)
        {
            d_target = target;
            d_invalidated = true;

            foreach (RenderQueueId value in Enum.GetValues(typeof(RenderQueueId)))
                d_queues[value]=new RenderQueue();
        }

        // TODO:...
        ////! Destructor for RenderingSurface objects.
        //virtual ~RenderingSurface();

        /// <summary>
        /// Add the specified GeometryBuffers to the specified queue for rendering
        /// when the RenderingSurface is drawn.
        /// </summary>
        /// <param name="queue">
        /// One of the RenderQueueID enumerated values indicating which prioritised
        /// queue the GeometryBuffer should be added to.
        /// </param>
        /// <param name="geometryBuffers">
        /// List of GeometryBuffers to be added to the specified rendering queue.
        /// </param>
        /// <remarks>
        /// The RenderingSurface does not take ownership of the GeometryBuffers, and
        /// does not destroy it when the RenderingSurface geometry is cleared.
        /// Rather, the RenderingSurface is just maintaining a list of things to be
        /// drawn; the actual GeometryBuffers can be re-used by whichever object
        /// does own them, and even changed or updated while still "attached" to
        /// a RenderingSurface.
        /// </remarks>
        public void AddGeometryBuffers(RenderQueueId queue, IEnumerable<GeometryBuffer> geometryBuffers)
        {
            d_queues[queue].AddGeometryBuffers(geometryBuffers);
        }

        /// <summary>
        /// Add the specified GeometryBuffer to the specified queue for rendering
        /// when the RenderingSurface is drawn.
        /// </summary>
        /// <param name="queue">
        /// One of the RenderQueueID enumerated values indicating which prioritised
        /// queue the GeometryBuffer should be added to.
        /// </param>
        /// <param name="buffer">
        /// GeometryBuffer object to be added to the specified rendering queue.
        /// </param>
        /// <remarks>
        /// The RenderingSurface does not take ownership of the <see cref="GeometryBuffer"/>, and
        /// does not destroy it when the RenderingSurface geometry is cleared. Rather, the 
        /// RenderingSurface is just maintaining a list of thigs to be  drawn; the actual 
        /// GeometryBuffers can be re-used by whichever object \e does own them, and even changed 
        /// or updated while still "attached" to a RenderingSurface.
        /// </remarks>
        public void AddGeometryBuffer(RenderQueueId queue, GeometryBuffer buffer)
        {
            d_queues[queue].AddGeometryBuffer(buffer);
        }

        /// <summary>
        /// Remove the specified GeometryBuffer from the specified queue.
        /// </summary>
        /// <param name="queue">
        /// One of the RenderQueueID enumerated values indicating which prioritised
        /// queue the GeometryBuffer should be removed from.
        /// </param>
        /// <param name="buffer">
        /// GeometryBuffer object to be removed from the specified rendering queue.
        /// </param>
        public void RemoveGeometryBuffer(RenderQueueId queue, GeometryBuffer buffer)
        {
            d_queues[queue].RemoveGeometryBuffer(buffer);
        }

        /// <summary>
        /// Clears all GeometryBuffers from the specified rendering queue.
        /// </summary>
        /// <param name="queue">
        /// One of the RenderQueueID enumerated values indicating which prioritised
        /// queue is to to be cleared.
        /// </param>
        /// <remarks>
        /// Clearing a rendering queue does not destroy the attached GeometryBuffers,
        /// which remain under thier original ownership.
        /// </remarks>
        public void ClearGeometry(RenderQueueId queue)
        {
            d_queues[queue].Reset();
        }

        /// <summary>
        /// Clears all GeometryBuffers from all rendering queues.
        /// </summary>
        /// <remarks>
        /// Clearing the rendering queues does not destroy the attached GeometryBuffers,
        /// which remain under their original ownership.
        /// </remarks>
        public void ClearGeometry()
        {
            foreach (var i in d_queues)
                i.Value.Reset();
        }
        
        /// <summary>
        /// Draw the GeometryBuffers for all rendering queues to the RenderTarget
        /// that this RenderingSurface is targetting.
        /// 
        /// The GeometryBuffers remain in the rendering queues after the draw
        /// operation is complete.  This allows the next draw operation to occur
        /// without needing to requeue all the GeometryBuffers (if for instance the
        /// sequence of buffers to be drawn remains unchanged).
        /// </summary>
        public virtual void Draw()
        {
            d_target.Activate();
            DrawContent();
            d_target.Deactivate();
        }
        
        /// <summary>
        /// Marks the RenderingSurface as invalid, causing the geometry to be
        /// rerendered to the RenderTarget next time draw is called.
        /// </summary>
        /// <remarks>
        /// Note that some surface types can never be in a 'valid' state and so
        /// rerendering occurs whenever draw is called.  This function mainly exists
        /// as a means to hint to other surface types - those that physically cache
        /// the rendered output - that geometry content has changed and the cached
        /// imagery should be cleared and redrawn.
        /// </remarks>
        public virtual void Invalidate()
        {
            d_invalidated = true;
        }

        /// <summary>
        /// Return whether this RenderingSurface is invalidated.
        /// </summary>
        /// <returns>
        /// - true to indicate the RenderingSurface is invalidated and will be
        ///   rerendered the next time the draw member function is called.
        /// - false to indicate the RenderingSurface is valid, and will not be
        ///   rerendered the next time the draw member function is called, since it's
        ///   cached imagery is up-to-date.
        /// </returns>
        /// <remarks>
        /// Note that some surface types can never be in a 'valid' state and so
        /// will always return true.
        /// </remarks>
        public bool IsInvalidated()
        {
            return d_invalidated || !d_target.IsImageryCache();
        }

        /// <summary>
        /// Return whether this RenderingSurface is actually an instance of the
        /// RenderingWindow subclass.
        /// </summary>
        /// <returns>
        /// - true to indicate the RenderingSurface is a RenderingWindow instance.
        /// - false to indicate the RenderingSurface is not a RenderingWindow.
        /// </returns>
        public virtual bool IsRenderingWindow()
        {
            return false;
        }

        /// <summary>
        /// Create and return a reference to a child RenderingWindow object that
        /// will render back onto this RenderingSurface when it's draw member
        /// function is called.
        /// 
        /// The RenderingWindow returned is initially owned by the RenderingSurface
        /// that created it.
        /// </summary>
        /// <param name="target">
        ///     TextureTarget object that will receive rendered output from the
        ///     RenderingWindow being creatd.
        /// </param>
        /// <returns>
        /// Reference to a RenderingWindow object.
        /// </returns>
        /// <remarks>
        /// Since RenderingWindow is a RenderingSurface, the same note from the
        /// constructor applies here, and that is the passed in TextureTarget
        /// remains under the ownership of whichever part of the system created
        /// it.
        /// </remarks>
        public virtual RenderingWindow CreateRenderingWindow(ITextureTarget target)
        {
            var w = new RenderingWindow(target, this);
            AttachWindow(w);
            return w;
        }

        /// <summary>
        /// Destroy a RenderingWindow we own.  If we are not the present owner of
        /// the given RenderingWindow, nothing happens.
        /// </summary>
        /// <param name="window">
        /// RenderingWindow object that is to be destroyed.
        /// </param>
        /// <remarks>
        /// Destroying a RenderingWindow will not also destroy the TextureTarget
        /// that was given when the RenderingWindow was created.  The TextureTarget
        /// should be destoyed elsewhere.
        /// </remarks>
        public virtual void DestroyRenderingWindow(RenderingWindow window)
        {
            if (window.GetOwner() == this)
            {
                DetatchWindow(window);
                // TODO: CEGUI_DELETE_AO & window;
            }
        }
        
        /// <summary>
        /// Transfer ownership of the given RenderingWindow to this
        /// RenderingSurface.  The result is \e generally the same as if this
        /// RenderingSurface had created the RenderingWindow in the first place.
        /// </summary>
        /// <param name="window">
        /// RenderingWindow object that this RenderingSurface is to take ownership of.
        /// </param>
        public virtual void TransferRenderingWindow(RenderingWindow window)
        {
            if (window.GetOwner() != this)
            {
                // detach window from it's current owner
                window.GetOwner().DetatchWindow(window);
                // add window to this surface.
                AttachWindow(window);

                window.SetOwner(this);
            }
        }

        /// <summary>
        /// Return the RenderTarget object that this RenderingSurface is drawing to.
        /// </summary>
        /// <returns>
        /// RenderTarget object that the RenderingSurface is using to draw it's output.
        /// </returns>
        public IRenderTarget GetRenderTarget()
        {
            return d_target;
        }
        
        /// <summary>
        /// draw the surface content. Default impl draws the render queues.
        /// NB: Called between RenderTarget activate and deactivate calls.
        /// </summary>
        protected virtual void DrawContent()
        {
            var evt_args = new RenderQueueEventArgs(RenderQueueId.RQ_USER_0);

            foreach (var i in d_queues)
            {
                evt_args.handled = 0;
                evt_args.RenderQueueId = i.Key;
                Draw(i.Value, evt_args);
            }
        }

        /// <summary>
        /// draw a rendering queue, firing events before and after.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="args"></param>
        protected void Draw(RenderQueue queue, RenderQueueEventArgs args)
        {
            FireEvent(RenderQueueStarted, args);

            d_target.Draw(queue);
            args.handled = 0;

            FireEvent(RenderQueueEnded, args);
        }

        /// <summary>
        /// detatch ReneringWindow from this RenderingSurface
        /// </summary>
        /// <param name="w"></param>
        protected void DetatchWindow(RenderingWindow w)
        {
            if (d_windows.Remove(w))
            {
                Invalidate();
            }
        }

        /// <summary>
        /// attach ReneringWindow from this RenderingSurface
        /// </summary>
        /// <param name="w"></param>
        protected void AttachWindow(RenderingWindow w)
        {
            d_windows.Add(w);
            Invalidate();
        }

        private void FireEvent(EventHandler<RenderQueueEventArgs> eventHandler, RenderQueueEventArgs args)
        {
            var handler = eventHandler;
            if (handler != null)
                handler(this, args);
        }

        // collection type for the queues
        // typedef std::map<RenderQueueID, RenderQueue/*CEGUI_MAP_ALLOC(RenderQueueID, RenderQueue)*/> RenderQueueList;
        //! the collection of RenderQueue objects.
        protected Dictionary<RenderQueueId, RenderQueue> d_queues =new Dictionary<RenderQueueId, RenderQueue>();

        /// <summary>
        /// collection of RenderingWindow object we own
        /// </summary>
        protected List<RenderingWindow> d_windows = new List<RenderingWindow>();

        /// <summary>
        /// RenderTarget that this surface actually draws to.
        /// </summary>
        protected IRenderTarget d_target;

        /// <summary>
        /// holds invalidated state of target (as far as we are concerned)
        /// </summary>
        protected bool d_invalidated;
    }
}