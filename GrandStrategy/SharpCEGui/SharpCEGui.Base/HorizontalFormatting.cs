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
    /// Enumeration of possible values to indicate the horizontal formatting to be used for an image component.
    /// </summary>
    public enum HorizontalFormatting
    {
        /// <summary>
        /// Left of Image should be aligned with the left of the destination area.
        /// </summary>
        LeftAligned,    // HF_LEFT_ALIGNED
        
        /// <summary>
        /// Image should be horizontally centred within the destination area.
        /// </summary>
        CentreAligned,  // HF_CENTRE_ALIGNED,

        /// <summary>
        /// Right of Image should be aligned with the right of the destination area.
        /// </summary>
        RightAligned,   // HF_RIGHT_ALIGNED,

        /// <summary>
        /// Image should be stretched horizontally to fill the destination area.
        /// </summary>
        Stretched,      // HF_STRETCHED,

        /// <summary>
        /// Image should be tiled horizontally to fill the destination area (right-most tile may be clipped).
        /// </summary>
        Tiled           // HF_TILED
    };
}