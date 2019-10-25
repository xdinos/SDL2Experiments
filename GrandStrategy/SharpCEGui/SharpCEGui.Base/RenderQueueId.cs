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
    /// Enumerated type for valid render queue Ids.
    /// </summary>
    public enum RenderQueueId
    {
        RQ_USER_0,

        /// <summary>
        /// Queue for rendering that appears beneath base imagery.
        /// </summary>
        RQ_UNDERLAY,

        RQ_USER_1,

        /// <summary>
        /// Queue for base level rendering by the surface owner.
        /// </summary>
        RQ_BASE,

        RQ_USER_2,

        /// <summary>
        /// Queue for first level of 'content' rendering.
        /// </summary>
        RQ_CONTENT_1,

        RQ_USER_3,

        /// <summary>
        /// Queue for second level of 'content' rendering.
        /// </summary>
        RQ_CONTENT_2,

        RQ_USER_4,

        /// <summary>
        /// Queue for overlay rendering that appears above other regular rendering.
        /// </summary>
        RQ_OVERLAY,

        RQ_USER_5
    };
}