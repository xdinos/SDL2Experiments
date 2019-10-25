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
using Lunatics.Mathematics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// String component that draws an image.
    /// </summary>
    public class RenderedStringImageComponent : RenderedStringComponent
    {
        //! Constructor
        public RenderedStringImageComponent()
        {
            throw new NotImplementedException();
        }
        
        public RenderedStringImageComponent(string name)
        {
            throw new NotImplementedException();
        }

        public RenderedStringImageComponent(Image image)
        {
            throw new NotImplementedException();
        }

        //! Set the image to be drawn by this component.
        public void SetImage(string name)
        {
            throw new NotImplementedException();
        }

        //! Set the image to be drawn by this component.
        public void SetImage(Image image)
        {
            throw new NotImplementedException();
        }

        //! return the current set image that will be drawn by this component
        public Image GetImage()
        {
            throw new NotImplementedException();
        }

        //! Set the colour values used when rendering this component.
        public void SetColours(ColourRect cr)
        {
            throw new NotImplementedException();
        }

        //! Set the colour values used when rendering this component.
        public void SetColours(Colour c)
        {
            throw new NotImplementedException();
        }

        //! return the ColourRect object used when drawing this component.
        public ColourRect GetColours()
        {
            throw new NotImplementedException();
        }

        //! set the size for rendering the image (0s mean 'normal' size)
        public void SetSize(Sizef sz)
        {
            throw new NotImplementedException();
        }

        //! return the size for rendering the image (0s mean 'normal' size)
        public Sizef GetSize()
        {
            throw new NotImplementedException();
        }

        #region Overrides of RenderedStringComponent

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Vector2 position, ColourRect modColours, Rectf? clipRect, float verticalSpace, float spaceExtra)
        {
            throw new global::System.NotImplementedException();
        }

        public override Sizef GetPixelSize(Window refWnd)
        {
            throw new global::System.NotImplementedException();
        }

        public override bool CanSplit()
        {
            throw new global::System.NotImplementedException();
        }

        public override RenderedStringComponent Split(Window refWnd, float splitPoint, bool firstComponent)
        {
            throw new global::System.NotImplementedException();
        }

        public override RenderedStringComponent Clone()
        {
            throw new global::System.NotImplementedException();
        }

        public override int GetSpaceCount()
        {
            throw new global::System.NotImplementedException();
        }

        public override void SetSelection(Window refWnd, float start, float end)
        {
            throw new global::System.NotImplementedException();
        }

        #endregion

        //! pointer to the image drawn by the component.
        protected Image d_image;
        //! ColourRect object describing the colours to use when rendering.
        protected ColourRect d_colours;
        //! target size to render the image at (0s mean natural size)
        protected Sizef d_size;
        // whether the image is marked as selected.
        protected bool d_selected;
    }
}