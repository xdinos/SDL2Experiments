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

namespace SharpCEGui.Base
{
    /// <summary>
    /// EventArgs based class that is passed to handlers subcribed to hear about
    /// begin/end events on rendering queues that are part of a RenderingSurface
    /// object.
    /// </summary>
    public class RenderQueueEventArgs : EventArgs
    {
        /// <summary>
        /// Id of the queue that this event has been fired for.
        /// </summary>
        public RenderQueueId RenderQueueId { get; set; }

        /// <summary>
        /// Constructor for RenderQueueEventArgs objects.
        /// </summary>
        /// <param name="renderQueueId">
        /// RenderQueueId value indicating the queue that the event is being
        /// generated for.
        /// </param>
        public RenderQueueEventArgs(RenderQueueId renderQueueId)
        {
            RenderQueueId = renderQueueId;
        }
    }
}