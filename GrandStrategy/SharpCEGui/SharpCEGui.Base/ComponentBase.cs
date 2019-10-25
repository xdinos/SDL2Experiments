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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Common base class used for renderable components within an ImagerySection.
    /// </summary>
    public abstract class ComponentBase
    {
        protected ComponentBase()
        {
            d_colours = new ColourRect(0xFFFFFFFF);
        }

        // TODO: virtual ~FalagardComponentBase();

        /// <summary>
        /// Creates the render geometry for this component and adds it to the Window
        /// </summary>
        /// <param name="srcWindow">
        ///     Window to use as the base for translating the component's ComponentArea
        ///     into pixel values.
        /// </param>
        /// <param name="modColours">
        ///     ColourRect describing colours that are to be modulated with the
        ///     component's stored colour values to calculate a set of 'final' colour
        ///     values to be used.  May be 0.
        /// </param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void CreateRenderGeometryAndAddToWindow(Window srcWindow, ColourRect modColours = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            var destRect = d_area.GetPixelRect(srcWindow);

            if (!clipper.HasValue)
                clipper = destRect;

            var finalClipRect = destRect.GetIntersection(clipper.Value);
            AddImageRenderGeometryToWindowImpl(srcWindow, destRect, modColours, finalClipRect, clipToDisplay);
        }

        /// <summary>
        /// Creates the render geometry for this component and adds it to the Window
        /// </summary>
        /// <param name="srcWindow">
        /// Window to use as the base for translating the component's ComponentArea
        /// into pixel values.
        /// </param>
        /// <param name="baseRect">
        /// Rect to use as the base for translating the component's ComponentArea
        /// into pixel values.
        /// </param>
        /// <param name="modColours">
        /// ColourRect describing colours that are to be modulated with the
        /// component's stored colour values to calculate a set of 'final' colour
        /// values to be used.  May be 0.
        /// </param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void CreateRenderGeometryAndAddToWindow(Window srcWindow, Rectf baseRect, ColourRect modColours = null, Rectf? clipper = null,
                           bool clipToDisplay = false)
        {
            var destRect = d_area.GetPixelRect(srcWindow, baseRect);

            if (clipper == null)
                clipper = destRect;

            var finalClipRect = destRect.GetIntersection(clipper.Value);
            AddImageRenderGeometryToWindowImpl(srcWindow, destRect, modColours, finalClipRect, clipToDisplay);
        }

        /// <summary>
        /// Return the ComponentArea of this component.
        /// </summary>
        /// <returns>
        /// ComponentArea object describing the component's current target area.
        /// </returns>
        public ComponentArea GetComponentArea()
        {
            return d_area;
        }

        /// <summary>
        /// Set the conponent's ComponentArea.
        /// </summary>
        /// <param name="area">
        /// ComponentArea object describing a new target area for the component.
        /// </param>
        public void SetComponentArea(ComponentArea area)
        {
            d_area = area;
        }

        /*!
        \brief
            Return the ColourRect used by this component.

        \return
            ColourRect object holding the colours currently in use by this
            component.
        */
        public ColourRect GetColours()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the colours to be used by this component.
        /// </summary>
        /// <param name="cols">
        /// ColourRect object describing the colours to be used by this component.
        /// </param>
        public void SetColours(ColourRect cols)
        {
            d_colours = cols;
        }

        /// <summary>
        /// Set the name of the property where colour values can be obtained.
        /// </summary>
        /// <param name="property">
        /// String containing the name of the property.
        /// </param>
        public void SetColoursPropertySource(string property)
        {
            d_colourPropertyName = property;
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public virtual bool HandleFontRenderSizeChange(Window window, Font font)
        {
            return d_area.HandleFontRenderSizeChange(window, font);
        }

        /// <summary>
        /// Helper function to initialise a ColourRect with appropriate values
        /// according to the way the component is set up.
        /// 
        /// This will try and get values from multiple places:
        ///     - a property attached to \a wnd
        ///     - or the integral d_colours value.
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="modCols"></param>
        /// <param name="cr"></param>
        protected void InitColoursRect(Window wnd, ColourRect modCols, out ColourRect cr)
        {
            // if colours come via a colour property
            if (!String.IsNullOrEmpty(d_colourPropertyName))
            {
                // if property accesses a ColourRect or a colour
                cr = wnd.GetProperty<ColourRect>(d_colourPropertyName);
            }
            // use explicit ColourRect.
            else
                cr = d_colours;

            if (modCols != null)
                cr *= modCols;
        }

        /// <summary>
        /// Function to do main render caching work.
        /// </summary>
        /// <param name="srcWindow"></param>
        /// <param name="destRect"></param>
        /// <param name="modColours"></param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        protected abstract void AddImageRenderGeometryToWindowImpl(Window srcWindow, Rectf destRect, ColourRect modColours, Rectf? clipper, bool clipToDisplay);

        /*!
        \brief
            Writes xml for the colours to a OutStream.
            Will prefer property colours before explicit.

        \note
            This is intended as a helper function for sub-classes when outputting
            xml to a stream.

        \return
            - true if xml element was written.
            - false if nothing was output due to the formatting not being set
              (sub-class may then choose to do something else.)
        */
        protected bool WriteColoursXML(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        //! Destination area for this component.
        protected ComponentArea d_area = new ComponentArea();
        //! base colours to be applied when rendering the image component.
        protected ColourRect d_colours;
        //! name of property to fetch colours from.
        protected string d_colourPropertyName;
    }
}