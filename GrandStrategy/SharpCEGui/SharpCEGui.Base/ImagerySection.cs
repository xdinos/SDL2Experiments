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
    /// Class that encapsulates a re-usable collection of imagery specifications.
    /// </summary>
    public class ImagerySection
    {
        // TODO: ???
        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public ImagerySection()
        //    : this(String.Empty)
        //{

        //}

        /*!
        \brief
            

        \param name
            
        */

        /// <summary>
        /// ImagerySection constructor.  Name must be supplied, masterColours are set to 0xFFFFFFFF by default.
        /// </summary>
        /// <param name="name">
        /// Name of the new ImagerySection.
        /// </param>
        public ImagerySection(string name)
        {
            d_name = name;
            d_masterColours = new ColourRect(new Colour(0xFFFFFFFF));
        }

        /// <summary>
        /// Render the ImagerySection.
        /// </summary>
        /// <param name="srcWindow">
        /// Window object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modColours">
        /// ColourRect specifying colours to be modulated with the ImagerySection's master colours.  May be 0.
        /// </param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, ColourRect modColours = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            // decide what to do as far as colours go
            ColourRect finalCols;
            InitMasterColourRect(srcWindow, out finalCols);

            if (modColours!=null)
                finalCols *= modColours;

            var finalColsPtr = (finalCols.IsMonochromatic() && finalCols.d_top_left.ToAlphaRGB() == 0xFFFFFFFF)
                                   ? null
                                   : finalCols;

            // render all frame components in this section
            foreach (var frame in d_frames)
                frame.CreateRenderGeometryAndAddToWindow(srcWindow, finalColsPtr, clipper, clipToDisplay);

            // render all image components in this section
            foreach (var image in d_images)
                image.CreateRenderGeometryAndAddToWindow(srcWindow, finalColsPtr, clipper, clipToDisplay);

            // render all text components in this section
            foreach (var text in d_texts)
                text.CreateRenderGeometryAndAddToWindow(srcWindow, finalColsPtr, clipper, clipToDisplay);
        }

        /// <summary>
        /// Render the ImagerySection.
        /// </summary>
        /// <param name="srcWindow">
        /// Window object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="baseRect">
        /// Rect object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modColours">
        ///  ColourRect specifying colours to be modulated with the ImagerySection's master colours.  May be null.
        /// </param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, Rectf baseRect, ColourRect modColours = null, Rectf? clipper = null,
                           bool clipToDisplay = false)
        {
            // decide what to do as far as colours go
            ColourRect finalCols;
            InitMasterColourRect(srcWindow, out finalCols);

            if (modColours != null)
                finalCols *= modColours;

            var finalColsPtr = (finalCols.IsMonochromatic() && finalCols.d_top_left.ToAlphaRGB() == 0xFFFFFFFF)
                                   ? null
                                   : finalCols;

            // render all frame components in this section
            foreach (var frame in d_frames)
                frame.CreateRenderGeometryAndAddToWindow(srcWindow, baseRect, finalColsPtr, clipper, clipToDisplay);

            // render all image components in this section
            foreach (var image in d_images)
                image.CreateRenderGeometryAndAddToWindow(srcWindow, baseRect, finalColsPtr, clipper, clipToDisplay);

            // render all text components in this section
            foreach (var text in d_texts)
                text.CreateRenderGeometryAndAddToWindow(srcWindow, baseRect, finalColsPtr, clipper, clipToDisplay);
        }

        /// <summary>
        /// Add an ImageryComponent to this ImagerySection.
        /// </summary>
        /// <param name="img">
        /// ImageryComponent to be added to the section (a copy is made)
        /// </param>
        public void AddImageryComponent(ImageryComponent img)
        {
            d_images.Add(img);
        }

        public void RemoveImageryComponent(ImageryComponent img)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear all ImageryComponents from this ImagerySection.
        /// </summary>
        public void ClearImageryComponents()
        {
            d_images.Clear();
        }

        /// <summary>
        /// Add a TextComponent to this ImagerySection.
        /// </summary>
        /// <param name="text">
        /// TextComponent to be added to the section (a copy is made)
        /// </param>
        public void AddTextComponent(TextComponent text)
        {
            d_texts.Add(text);
        }

        public void RemoveTextComponent(TextComponent text)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Clear all TextComponents from this ImagerySection.

        \return
            Nothing
        */

        public void ClearTextComponents()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Clear all FrameComponents from this ImagerySection.

        \return
            Nothing
        */

        public void ClearFrameComponents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a FrameComponent to this ImagerySection.
        /// </summary>
        /// <param name="frame">
        /// FrameComponent to be added to the section (a copy is made)
        /// </param>
        public void AddFrameComponent(FrameComponent frame)
        {
            d_frames.Add(frame);
        }

        public void RemoveFrameComponent(FrameComponent frame)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return the current master colours set for this ImagerySection.

        \return
            ColourRect describing the master colour values in use for this ImagerySection.
        */

        public ColourRect GetMasterColours()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the master colours to be used for this ImagerySection.
        /// </summary>
        /// <param name="cols">
        /// ColourRect describing the colours to be set as the master colours for this ImagerySection.
        /// </param>
        public void SetMasterColours(ColourRect cols)
        {
            d_masterColours = cols;
        }

        /// <summary>
        /// Return the name of this ImagerySection.
        /// </summary>
        /// <returns>
        /// String object holding the name of the ImagerySection.
        /// </returns>
        public string GetName()
        {
            return d_name;
        }

        /*!
        \brief
            Sets the name of this ImagerySection.

        \param name
            String object holding the name of the ImagerySection.

        \return
            Nothing.
        */

        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Get the name of the property where master colour values can be obtained.

        \return
            String containing the name of the property.
        */

        public string GetMasterColoursPropertySource()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the name of the property where master colour values can be obtained.

        \param property
            String containing the name of the property.

        \return
            Nothing.
        */

        public void SetMasterColoursPropertySource(string property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return smallest Rect that could contain all imagery within this section.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public Rectf GetBoundingRect(Window wnd)
        {
            Rectf compRect;
            var bounds = new Rectf(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

            // measure all frame components
            // measure all frame components
            foreach (var frame in d_frames)
            {
                compRect = frame.GetComponentArea().GetPixelRect(wnd);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            // measure all imagery components
            foreach (var image in d_images)
            {
                compRect = image.GetComponentArea().GetPixelRect(wnd);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            // measure all text components
            foreach (var text in d_texts)
            {
                compRect = text.GetComponentArea().GetPixelRect(wnd);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            return bounds;
        }

        /// <summary>
        /// Return smallest Rect that could contain all imagery within this section.
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectf GetBoundingRect(Window wnd, Rectf rect)
        {
            Rectf compRect;
            var bounds = new Rectf(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

            // measure all frame components
            foreach (var frame in d_frames)
            {
                compRect = frame.GetComponentArea().GetPixelRect(wnd, rect);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            // measure all imagery components
            foreach (var image in d_images)
            {
                compRect = image.GetComponentArea().GetPixelRect(wnd, rect);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            // measure all text components
            foreach (var text in d_texts)
            {
                compRect = text.GetComponentArea().GetPixelRect(wnd, rect);

                bounds.Left = Math.Min(bounds.Left, compRect.Left);
                bounds.Top = Math.Min(bounds.Top, compRect.Top);
                bounds.Right = Math.Max(bounds.Right, compRect.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, compRect.Bottom);
            }

            return bounds;
        }

        /*!
        \brief
            Writes an xml representation of this ImagerySection to \a out_stream.

        \param xml_stream
            Stream where xml data should be output.


        \return
            Nothing.
        */

        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            var result = false;

            foreach (var text in d_texts)
                result |= text.HandleFontRenderSizeChange(window, font);

            return result;
        }

        /// <summary>
        /// Helper method to initialise a ColourRect with appropriate values according to the way the
        /// ImagerySection is set up.
        /// 
        /// This will try and get values from multiple places:
        ///     - a property attached to \a wnd
        ///     - or the integral d_masterColours value.
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="cr"></param>
        protected void InitMasterColourRect(Window wnd, out ColourRect cr)
        {
            // if colours come via a colour property
            if (!String.IsNullOrEmpty(d_colourPropertyName))
            {
                // if property accesses a ColourRect or a colour
                cr = wnd.GetProperty<ColourRect>(d_colourPropertyName);
            }
            // use explicit ColourRect.
            else
            {
                cr = d_masterColours;
            }
        }

        //private:
        //    typedef std::vector<ImageryComponent
        //        CEGUI_VECTOR_ALLOC(ImageryComponent)> ImageryList;
        //    typedef std::vector<TextComponent
        //        CEGUI_VECTOR_ALLOC(TextComponent)> TextList;
        //    typedef std::vector<FrameComponent
        //        CEGUI_VECTOR_ALLOC(FrameComponent)> FrameList;

        private string d_name; //!< Holds the name of the ImagerySection.

        private ColourRect d_masterColours;
                           //!< Naster colours for the the ImagerySection (combined with colours of each ImageryComponent).

        /// <summary>
        /// Collection of FrameComponent objects to be drawn for this ImagerySection.
        /// </summary>
        private List<FrameComponent> d_frames = new List<FrameComponent>();
                                     
        /// <summary>
        /// Collection of ImageryComponent objects to be drawn for this ImagerySection.
        /// </summary>
        private List<ImageryComponent> d_images = new List<ImageryComponent>();

        private List<TextComponent> d_texts = new List<TextComponent>();
                                    //!< Collection of TextComponent objects to be drawn for this ImagerySection.

        private string d_colourPropertyName; //!< name of property to fetch colours from.

        //public:
        //    typedef ConstVectorIterator<ImageryList> ImageryComponentIterator;
        //    typedef ConstVectorIterator<TextList> TextComponentIterator;
        //    typedef ConstVectorIterator<FrameList> FrameComponentIterator;

        /*!
        \brief
            Return a ImagerySection::ImageryComponentIterator object to iterate
            over the ImageryComponent elements currently added to the
            ImagerySection.
        */

        public IEnumerable<ImageryComponent> GetImageryComponentIterator()
        {
            return d_images;
        }

        /// <summary>
        /// Return a ImagerySection::TextComponentIterator object to iterate
        /// over the TextComponent elements currently added to the
        /// ImagerySection.
        /// </summary>
        public IEnumerable<TextComponent> TextComponents { get { return d_texts; } }

        /// <summary>
        /// Return a ImagerySection::FrameComponentIterator object to iterate
        /// over the FrameComponent elements currently added to the
        /// ImagerySection.
        /// </summary>
        public IEnumerable<FrameComponent> FrameComponents { get { return d_frames; } }
    }
}