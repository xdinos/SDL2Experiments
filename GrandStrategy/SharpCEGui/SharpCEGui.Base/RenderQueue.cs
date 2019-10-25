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

using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that represents a queue of GeometryBuffer objects to be rendered.
    /// </summary>
    /// <remarks>
    /// The RenderQueue does not make copies of added GeometryBuffers, nor does it
    /// take ownership of them - it is up to other parts of the system to manage the
    /// lifetime of the GeometryBuffer objects (and to remove them from any
    /// RenderQueue to which they may be attached prior to destoying them).
    /// </remarks>
    public class RenderQueue
    {
        #region Methods

        /// <summary>
        /// Draw all GeometryBuffer objects currently listed in the RenderQueue.
        /// The GeometryBuffer objects remain in the queue after drawing has taken
        /// place.
        /// </summary>
        public void Draw()
        {
            foreach (var geometryBuffer in _buffers)
            {
                geometryBuffer.Draw();
            }
        }

        /// <summary>
        /// Add a list of GeometryBuffers to the RenderQueue. Ownership of the
        /// GeometryBuffer does not pass to the RenderQueue.
        /// </summary>
        /// <param name="geometryBuffers">
        /// List of GeometryBuffers that are to be added to the RenderQueue for later drawing.
        /// </param>
        public void AddGeometryBuffers(IEnumerable<GeometryBuffer> geometryBuffers)
        {
            _buffers.AddRange(geometryBuffers);
        }

        /// <summary>
        /// Add a GeometryBuffer to the RenderQueue. Ownership of the
        /// GeometryBuffer does not pass to the RenderQueue.
        /// </summary>
        /// <param name="buffer">
        /// GeometryBuffer that is to be added to the RenderQueue for later drawing.
        /// </param>
        public void AddGeometryBuffer(GeometryBuffer buffer)
        {
            _buffers.Add(buffer);
        }


        /// <summary>
        /// Remove a GeometryBuffer previously queued for drawing.  If the specified
        /// GeometryBuffer is not added to the queue, no action is taken. The
        /// removed GeometryBuffer is not destroyed or modified in any way.
        /// </summary>
        /// <param name="buffer">
        /// GeometryBuffer to be removed from the queue.
        /// </param>
        public void RemoveGeometryBuffer(GeometryBuffer buffer)
        {
            _buffers.Remove(buffer);
        }

        /// <summary>
        /// Remove any and all queued GeometryBuffer objects and restore the queue
        /// to the default state.  Any GeometryBuffer objects removed are not
        /// destroyed or modified in any way.
        /// </summary>
        public void Reset()
        {
            _buffers.Clear();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Collection of GeometryBuffer objects that comprise this RenderQueue.
        /// </summary>
        private readonly List<GeometryBuffer> _buffers = new List<GeometryBuffer>();

        #endregion
    }
}