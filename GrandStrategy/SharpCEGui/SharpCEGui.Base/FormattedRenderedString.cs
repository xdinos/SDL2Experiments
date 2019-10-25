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
    /// Root of a class hierarchy that wrap RenderedString objects and render them
    /// with additional formatting.
    /// </summary>
    public abstract class FormattedRenderedString
    {
        //! Destructor.
        // TODO: virtual ~FormattedRenderedString();

        public abstract void Format(Window refWnd, Sizef areaSize);

        public abstract List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect);

        public abstract int GetFormattedLineCount();

        public abstract float GetHorizontalExtent(Window refWnd);

        public abstract float GetVerticalExtent(Window refWnd);

        /// <summary>
        /// set the RenderedString.
        /// </summary>
        /// <param name="string"></param>
        public void SetRenderedString(RenderedString @string)
        {
            d_renderedString = @string;
        }

        public RenderedString GetRenderedString()
        {
            return d_renderedString;
        }
        
        //! Constructor.
        protected FormattedRenderedString()
        {
            
        }

        protected FormattedRenderedString(RenderedString @string)
        {
            d_renderedString = @string;
        }
        
        //! RenderedString that we handle formatting for.
        protected RenderedString d_renderedString;
    }
}