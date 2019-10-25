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
    /// Enumeration of possible values to indicate the horizontal formatting to be used for a text component.
    /// </summary>
    public enum HorizontalTextFormatting
    {
        /// <summary>
        /// Left of text should be aligned with the left of the destination area (single line of text only).
        /// </summary>
        LeftAligned,            // HTF_LEFT_ALIGNED

        /// <summary>
        /// Right of text should be aligned with the right of the destination area  (single line of text only).
        /// </summary>
        RightAligned,           // HTF_RIGHT_ALIGNED

        /// <summary>
        /// text should be horizontally centred within the destination area  (single line of text only).
        /// </summary>
        CentreAligned,          // HTF_CENTRE_ALIGNED

        /// <summary>
        /// text should be spaced so that it takes the full width of the destination area (single line of text only).
        /// </summary>
        Justified,              // HTF_JUSTIFIED

        /// <summary>
        /// Left of text should be aligned with the left of the destination area (word wrapped to multiple lines as needed).
        /// </summary>
        WordWrapLeftAligned,    // HTF_WORDWRAP_LEFT_ALIGNED

        /// <summary>
        /// Right of text should be aligned with the right of the destination area  (word wrapped to multiple lines as needed).
        /// </summary>
        WordWrapRightAligned,   // HTF_WORDWRAP_RIGHT_ALIGNED

        /// <summary>
        /// text should be horizontally centred within the destination area  (word wrapped to multiple lines as needed).
        /// </summary>
        WordWrapCentreAligned,  // HTF_WORDWRAP_CENTRE_ALIGNED

        /// <summary>
        /// text should be spaced so that it takes the full width of the destination area (word wrapped to multiple lines as needed).
        /// </summary>
        WordWrapJustified,      // HTF_WORDWRAP_JUSTIFIED
    };
}