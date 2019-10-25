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
    /// Enumeration of possible values to indicate what a given dimension represents.
    /// </summary>
    public enum DimensionType
    {
        /// <summary>
        /// Invalid / uninitialised DimensionType.
        /// </summary>
        Invalid,    // DT_INVALID

        /// <summary>
        /// Dimension represents the left edge of some entity (same as DT_X_POSITION).
        /// </summary>
        LeftEdge,   // DT_LEFT_EDGE

        /// <summary>
        /// Dimension represents the x position of some entity (same as DT_LEFT_EDGE).
        /// </summary>
        XPosition,  // DT_X_POSITION,

        /// <summary>
        /// Dimension represents the top edge of some entity (same as DT_Y_POSITION).
        /// </summary>
        TopEdge,    // DT_TOP_EDGE,

        /// <summary>
        /// Dimension represents the y position of some entity (same as DT_TOP_EDGE).
        /// </summary>
        YPosition,  // DT_Y_POSITION,

        /// <summary>
        /// Dimension represents the right edge of some entity.
        /// </summary>
        RightEdge,  // DT_RIGHT_EDGE,

        /// <summary>
        /// Dimension represents the bottom edge of some entity.
        /// </summary>
        BottomEdge, // DT_BOTTOM_EDGE,

        /// <summary>
        /// Dimension represents the width of some entity.
        /// </summary>
        Width,      // DT_WIDTH,

        /// <summary>
        /// Dimension represents the height of some entity.
        /// </summary>
        Height,     // DT_HEIGHT,

        /// <summary>
        /// Dimension represents the x offset of some entity (usually only applies to an Image entity).
        /// </summary>
        XOffset,    // DT_X_OFFSET,

        /// <summary>
        /// Dimension represents the y offset of some entity (usually only applies to an Image entity).
        /// </summary>
        YOffset,    // DT_Y_OFFSET,
    };
}