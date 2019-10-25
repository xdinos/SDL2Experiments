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
    /// Base class representing a part of a rendered string.  The 'part' represented
    /// may be a text string, an image or some other entity.
    /// </summary>
    public abstract class RenderedStringComponent
    {
        // TODO: Destructor.
        // virtual ~RenderedStringComponent();

        protected RenderedStringComponent()
        {
        }

        protected RenderedStringComponent(RenderedStringComponent other)
        {
            d_aspectLock = other.d_aspectLock;
            d_padding = other.d_padding;
            d_selectionImage = other.d_selectionImage;
            d_verticalFormatting = other.d_verticalFormatting;
        }

        /// <summary>
        /// Set the VerticalFormatting option for this component.
        /// </summary>
        /// <param name="fmt"></param>
        public void SetVerticalFormatting(VerticalFormatting fmt)
        {
            d_verticalFormatting = fmt;
        }

        //! return the current VerticalFormatting option.
        public VerticalFormatting GetVerticalFormatting()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// set the padding values.
        /// </summary>
        /// <param name="padding"></param>
        public void SetPadding(Rectf padding)
        {
            d_padding = padding;
        }

        /// <summary>
        /// set the left padding value.
        /// </summary>
        /// <param name="padding"></param>
        public void SetLeftPadding(float padding)
        {
            d_padding.d_min.X = padding;
        }

        /// <summary>
        /// set the right padding value.
        /// </summary>
        /// <param name="padding"></param>
        public void SetRightPadding(float padding)
        {
            d_padding.d_max.X = padding;
        }

        //! set the top padding value.
        public void SetTopPadding(float padding)
        {
            throw new NotImplementedException();
        }

        //! set the Bottom padding value.
        public void SetBottomPadding(float padding)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return the current padding value Rect.
        /// </summary>
        /// <returns></returns>
        public Rectf GetPadding()
        {
            throw new NotImplementedException();
        }

        //! return the left padding value.
        public float GetLeftPadding()
        {
            throw new NotImplementedException();
        }

        //! return the right padding value.
        public float GetRightPadding()
        {
            throw new NotImplementedException();
        }

        //! return the top padding value.
        public float GetTopPadding()
        {
            throw new NotImplementedException();
        }

        //! return the bottom padding value.
        public float GetBottomPadding()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// set the aspect-lock state
        /// </summary>
        /// <param name="setting"></param>
        public void SetAspectLock(bool setting)
        {
            d_aspectLock = setting;
        }

        /// <summary>
        /// return the aspect-lock state
        /// </summary>
        /// <returns></returns>
        public bool GetAspectLock()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// draw the component.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <param name="position"></param>
        /// <param name="modColours"></param>
        /// <param name="clipRect"></param>
        /// <param name="verticalSpace"></param>
        /// <param name="spaceExtra"></param>
        public abstract List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect, float verticalSpace, float spaceExtra);

        /// <summary>
        /// return the pixel size of the rendered component.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <returns></returns>
        public abstract Sizef GetPixelSize(Window refWnd);

        /// <summary>
        /// return whether the component can be split
        /// </summary>
        /// <returns></returns>
        public abstract bool CanSplit();

        /// <summary>
        /// split the component as close to split_point as possible, returning a
        /// new RenderedStringComponent of the same type as '*this' holding the
        /// left side of the split, and leaving the right side of the split in
        /// this object.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <param name="splitPoint"></param>
        /// <param name="firstComponent"></param>
        /// <returns></returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if the RenderedStringComponent does not support being split.
        /// </exception>
        public abstract RenderedStringComponent Split(Window refWnd, float splitPoint, bool firstComponent);

        /// <summary>
        /// clone this component.
        /// </summary>
        /// <returns></returns>
        public abstract RenderedStringComponent Clone();

        /// <summary>
        /// return the total number of spacing characters in the string.
        /// </summary>
        /// <returns></returns>
        public abstract int GetSpaceCount();

        /// <summary>
        /// mark some region appropriate given /a start and /a end as selected.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public abstract void SetSelection(Window refWnd, float start, float end);

        #region Fields

        //! Rect object holding the padding values for this component.
        protected Rectf d_padding = Rectf.Zero;

        //! Vertical formatting to be used for this component.
        protected VerticalFormatting d_verticalFormatting = VerticalFormatting.BottomAligned;

        //! true if the aspect ratio should be maintained where possible.
        protected bool d_aspectLock = false;

        //! Image to draw for selection
        protected Image d_selectionImage = null;

        #endregion
    }
}