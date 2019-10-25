﻿#region Copyright
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
    /// Specifies interface for classes that parse text into RenderedString objects.
    /// </summary>
    public interface RenderedStringParser
    {
        /// <summary>
        /// parse a text string and return a RenderedString representation.
        /// </summary>
        /// <param name="inputString">
        /// String object holding the text that is to be parsed.
        /// </param>
        /// <param name="initialFont">
        /// Pointer to the initial font to be used for text (0 for system default).
        /// </param>
        /// <param name="initialColours">
        /// Pointer to the initial colours to be used (0 for default).
        /// </param>
        /// <returns>
        /// RenderedString object holding the result of the parse operation.
        /// </returns>
        RenderedString Parse(string inputString, Font initialFont, ColourRect initialColours);
    }
}