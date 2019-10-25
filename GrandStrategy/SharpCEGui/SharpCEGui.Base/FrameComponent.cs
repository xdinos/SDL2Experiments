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
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that encapsulates information for a frame with background
    /// (9 images in total)
    /// 
    /// Corner images are always drawn at their natural size, edges can be fomatted
    /// (stretched, tiled or aligned) between the corner pieces for a particular
    /// edge, the background image will cover the inner rectangle formed by the edge
    /// images and can be formatted in both dimensions.
    /// </summary>
    public class FrameComponent : ComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        public FrameComponent()
        {
            LeftEdgeFormatting = new FormattingSetting<VerticalFormatting>(VerticalFormatting.Stretched);
            RightEdgeFormatting = new FormattingSetting<VerticalFormatting>(VerticalFormatting.Stretched);
            TopEdgeFormatting = new FormattingSetting<HorizontalFormatting>(HorizontalFormatting.Stretched);
            BottomEdgeFormatting = new FormattingSetting<HorizontalFormatting>(HorizontalFormatting.Stretched);
            BackgroundVertFormatting = new FormattingSetting<VerticalFormatting>(VerticalFormatting.Stretched);
            BackgroundHorzFormatting = new FormattingSetting<HorizontalFormatting>(HorizontalFormatting.Stretched);
        }

        /// <summary>
        /// Set the formatting to be used for the left edge image.
        /// </summary>
        /// <param name="fmt">
        /// One of the VerticalFormatting enumerated values.
        /// </param>
        public void SetLeftEdgeFormatting(VerticalFormatting fmt)
        {
            LeftEdgeFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the formatting to be used for the right edge image.
        /// </summary>
        /// <param name="fmt">
        /// One of the VerticalFormatting enumerated values.
        /// </param>
        public void SetRightEdgeFormatting(VerticalFormatting fmt)
        {
            RightEdgeFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the formatting to be used for the top edge image.
        /// </summary>
        /// <param name="fmt">
        /// One of the HorizontalFormatting enumerated values.
        /// </param>
        public void SetTopEdgeFormatting(HorizontalFormatting fmt)
        {
            TopEdgeFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the formatting to be used for the bottom edge image.
        /// </summary>
        /// <param name="fmt">
        /// One of the HorizontalFormatting enumerated values.
        /// </param>
        public void SetBottomEdgeFormatting(HorizontalFormatting fmt)
        {
            BottomEdgeFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the vertical formatting to be used for the background image.
        /// </summary>
        /// <param name="fmt">
        /// One of the VerticalFormatting enumerated values.
        /// </param>
        public void SetBackgroundVerticalFormatting(VerticalFormatting fmt)
        {
            BackgroundVertFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the horizontal formatting to be used for the background image.
        /// </summary>
        /// <param name="fmt">
        /// One of the HorizontalFormatting enumerated values.
        /// </param>
        public void SetBackgroundHorizontalFormatting(HorizontalFormatting fmt)
        {
            BackgroundHorzFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the formatting
        /// to use for the left edge image.
        /// </summary>
        /// <param name="property_name"></param>
        public void SetLeftEdgeFormattingPropertySource(string property_name)
        {
            LeftEdgeFormatting.SetPropertySource(property_name);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the formatting
        /// to use for the right edge image.
        /// </summary>
        /// <param name="property_name"></param>
        public void SetRightEdgeFormattingPropertySource(string property_name)
        {
            RightEdgeFormatting.SetPropertySource(property_name);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the formatting
        /// to use for the top edge image.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetTopEdgeFormattingPropertySource(string propertyName)
        {
            TopEdgeFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the formatting
        /// to use for the bottom edge image.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetBottomEdgeFormattingPropertySource(string propertyName)
        {
            BottomEdgeFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the vertical
        /// formatting to use for the backdround image.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetBackgroundVerticalFormattingPropertySource(string propertyName)
        {
            BackgroundVertFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the horizontal
        /// formatting to use for the backdround image.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetBackgroundHorizontalFormattingPropertySource(string propertyName)
        {
            BackgroundHorzFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Return the formatting to be used for the left edge image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>One of the VerticalFormatting enumerated values.</returns>
        public VerticalFormatting GetLeftEdgeFormatting(Window wnd)
        {
            return LeftEdgeFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the formatting to be used for the right edge image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>One of the VerticalFormatting enumerated values.</returns>
        public VerticalFormatting GetRightEdgeFormatting(Window wnd)
        {
            return RightEdgeFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the formatting to be used for the top edge image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>One of the HorizontalFormatting enumerated values.</returns>
        public HorizontalFormatting GetTopEdgeFormatting(Window wnd)
        {
            return TopEdgeFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the formatting to be used for the bottom edge image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the HorizontalFormatting enumerated values.
        /// </returns>
        public HorizontalFormatting GetBottomEdgeFormatting(Window wnd)
        {
            return BottomEdgeFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the vertical formatting to be used for the background image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the VerticalFormatting enumerated values.
        /// </returns>
        public VerticalFormatting GetBackgroundVerticalFormatting(Window wnd)
        {
            return BackgroundVertFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the horizontal formatting to be used for the background image.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the HorizontalFormatting enumerated values.
        /// </returns>
        public HorizontalFormatting GetBackgroundHorizontalFormatting(Window wnd)
        {
            return BackgroundHorzFormatting.Get(wnd);
        }

        /// <summary>
        /// Return the Image object that will be drawn by this FrameComponent
        /// for a specified frame part.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to be accessed.
        /// </param>
        /// <param name="wnd">
        /// Reference to a Window object that will be accessed if the image
        /// component is fetched from a Property.
        /// </param>
        /// <returns>
        /// pointer to an Image object, or 0 if the image had not been set
        /// or if the image is sourced from a property that returns an empty
        /// image name.
        /// </returns>
        public Image GetImage(FrameImageComponent part, Window wnd)
        {
            Debug.Assert((int)part < (int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            if (!FrameImages[(int)part].d_specified)
                return null;

            if (String.IsNullOrEmpty(FrameImages[(int)part].d_propertyName))
                return FrameImages[(int)part].d_image;

            return wnd.GetProperty<Image>(FrameImages[(int)part].d_propertyName);
        }

        /// <summary>
        /// Set an Image that will be drawn by this FrameComponent.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to be set.
        /// </param>
        /// <param name="image">
        /// Pointer to the Image object to be drawn.  If this is 0 then drawing
        /// of the component image specified by \a part will be disabled.
        /// </param>
        public void SetImage(FrameImageComponent part, Image image)
        {
            Debug.Assert((int) part < (int) FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            FrameImages[(int) part].d_image = image;
            FrameImages[(int) part].d_specified = image != null;
            FrameImages[(int) part].d_propertyName = String.Empty;
        }

        /// <summary>
        /// Set an Image that will be drawn by this FrameComponent.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to be set.
        /// </param>
        /// <param name="name">
        /// String holding the name of an Image. The image should already exist,
        /// if the Image does not exist an Exception will be logged and
        /// drawing of the component image specified by \a part will be
        /// disabled.
        /// </param>
        public void SetImage(FrameImageComponent part, string name)
        {
            Image image;
            try
            {
                image = ImageManager.GetSingleton().Get(name);
            }
            catch (UnknownObjectException)
            {
                image = null;
            }

            SetImage(part, image);
        }

        /// <summary>
        /// Set the name of the Property that will be accesssed on the target
        /// Window to determine the Image that will be drawn by the
        /// FrameComponent.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to be set.
        /// </param>
        /// <param name="name">
        /// String holding the name of a property that will be accessed. If this
        /// is the empty string then drawing of the component image specified by
        /// \a part will be disabled.
        /// </param>
        public void SetImagePropertySource(FrameImageComponent part, string name)
        {
            Debug.Assert((int)part < (int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            FrameImages[(int)part].d_image = null;
            FrameImages[(int)part].d_specified = !String.IsNullOrEmpty(name);
            FrameImages[(int)part].d_propertyName = name;
        }

        /// <summary>
        /// Return whether the given component image has been specified.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to check.
        /// </param>
        /// <returns>
        /// - true if the image is specified and will be drawn.
        /// - false if the image is not specified.
        /// </returns>
        public bool IsImageSpecified(FrameImageComponent part)
        {
            Debug.Assert((int)part < (int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            return FrameImages[(int)part].d_specified;
        }

        /// <summary>
        /// Return whether the given component image is specified via a
        /// property.
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image to check.
        /// </param>
        /// <returns>
        /// - true if the image is specified and fetched via a property.
        /// - false if the image is not fetched via a property
        ///   (or is not specified)
        /// </returns>
        public bool IsImageFetchedFromProperty(FrameImageComponent part)
        {
            Debug.Assert((int)part < (int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            return FrameImages[(int)part].d_specified &&
                   !String.IsNullOrEmpty(FrameImages[(int)part].d_propertyName);
        }

        /// <summary>
        /// Return the name of the property that will be used to determine the
        /// image to use for the given component image.
        /// 
        /// If the returned String is empty, it indicates either that the
        /// component image is not specified or that the component image is not
        /// sourced from a property.
        /// 
        /// \see
        /// isImageFetchedFromProperty isImageSpecified
        /// </summary>
        /// <param name="part">
        /// One of the FrameImageComponent enumerated values specifying the
        /// component image property source to return.
        /// </param>
        /// <returns></returns>
        public string GetImagePropertySource(FrameImageComponent part)
        {
            Debug.Assert((int)part < (int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT);

            return FrameImages[(int)part].d_propertyName;

        }

        /*!
        \brief
            Writes an xml representation of this FrameComponent to \a out_stream.

        \param xml_stream
            Stream where xml data should be output.


        \return
            Nothing.
        */
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        // TODO: bool operator==(const FrameComponent& rhs) const;

        protected struct FrameImageSource
        {
            // TODO: ...
            //bool operator==(FrameImageSource rhs) const
            //{
            //    return d_specified == rhs.d_specified &&
            //            d_image == rhs.d_image &&
            //            d_propertyName == rhs.d_propertyName;
            //}

            //bool operator!=(const FrameImageSource& rhs) const
            //{
            //    return !operator==(rhs);
            //}
        
            public bool d_specified;
            public Image d_image;
            public string d_propertyName;
        };

        // implemets abstract from base
        protected override void AddImageRenderGeometryToWindowImpl(Window srcWindow, Rectf destRect, ColourRect modColours, Rectf? clipper,
                                           bool clipToDisplay)
        {
            var backgroundRect = destRect;
            Sizef imageSize;
            Lunatics.Mathematics.Vector2 imageOffsets;
            float leftfactor, rightfactor, topfactor, bottomfactor;
            bool calcColoursPerImage;

            // vars we use to track what to do with the side pieces.
            float topOffset = 0, bottomOffset = 0, leftOffset = 0, rightOffset = 0;
            float topWidth, bottomWidth, leftHeight, rightHeight;
            topWidth = bottomWidth = destRect.Width;
            leftHeight = rightHeight = destRect.Height;

            // calculate final overall colours to be used
            ColourRect renderSettingFinalColours;
            InitColoursRect(srcWindow, modColours, out renderSettingFinalColours);

            var renderSettings = new ImageRenderSettings(Rectf.Zero, clipper, !clipToDisplay, renderSettingFinalColours);
            //var renderSettingDestArea = renderSettings.destArea;
            //var renderSettingMultiplyColours = renderSettings.multiplyColours;

            calcColoursPerImage = !renderSettingFinalColours.IsMonochromatic();

            // top-left image
            var componentImage = GetImage(FrameImageComponent.TopLeftCorner, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                imageOffsets = componentImage.GetRenderedOffset();
                renderSettings.DestArea.d_min = destRect.d_min;
                renderSettings.DestArea.Size = imageSize;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // update adjustments required to edges do to presence of this element.
                topOffset += imageSize.Width + imageOffsets.X;
                leftOffset += imageSize.Height + imageOffsets.Y;
                topWidth -= topOffset;
                leftHeight -= leftOffset;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + imageOffsets.X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + imageOffsets.Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this element and append it to the Window's geometry
                var imageGeomBuffers = componentImage.CreateRenderGeometry(renderSettings);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // top-right image
            componentImage = GetImage(FrameImageComponent.TopRightCorner, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                imageOffsets = componentImage.GetRenderedOffset();
                renderSettings.DestArea.Left = destRect.Right - imageSize.Width;
                renderSettings.DestArea.Top = destRect.Top;
                renderSettings.DestArea.Size = imageSize;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // update adjustments required to edges do to presence of this element.
                rightOffset += imageSize.Height + imageOffsets.Y;
                topWidth -= imageSize.Width - imageOffsets.X;
                rightHeight -= rightOffset;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + imageOffsets.X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + imageOffsets.Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this element and append it to the Window's geometry
                var imageGeomBuffers = componentImage.CreateRenderGeometry(renderSettings);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // bottom-left image
            componentImage = GetImage(FrameImageComponent.BottomLeftCorner, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                imageOffsets = componentImage.GetRenderedOffset();
                renderSettings.DestArea.Left = destRect.Left;
                renderSettings.DestArea.Top = destRect.Bottom - imageSize.Height;
                renderSettings.DestArea.Size = imageSize;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // update adjustments required to edges do to presence of this element.
                bottomOffset += imageSize.Width + imageOffsets.X;
                bottomWidth -= bottomOffset;
                leftHeight -= imageSize.Height - imageOffsets.Y;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + imageOffsets.X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + imageOffsets.Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this element and append it to the Window's geometry
                var imageGeomBuffers = componentImage.CreateRenderGeometry(renderSettings);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // bottom-right image
            componentImage = GetImage(FrameImageComponent.BottomRightCorner, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                imageOffsets = componentImage.GetRenderedOffset();
                renderSettings.DestArea.Left = destRect.Right - imageSize.Width;
                renderSettings.DestArea.Top = destRect.Bottom - imageSize.Height;
                renderSettings.DestArea.Size = imageSize;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // update adjustments required to edges do to presence of this element.
                bottomWidth -= imageSize.Width - imageOffsets.X;
                rightHeight -= imageSize.Height - imageOffsets.Y;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + componentImage.GetRenderedOffset().X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + componentImage.GetRenderedOffset().Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this element and append it to the Window's geometry
                var imageGeomBuffers = componentImage.CreateRenderGeometry(renderSettings);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // top image
            componentImage = GetImage(FrameImageComponent.TopEdge, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                renderSettings.DestArea.Left = destRect.Left + topOffset;
                renderSettings.DestArea.Right = (renderSettings.DestArea.Left + topWidth);
                renderSettings.DestArea.Top = destRect.Top;
                renderSettings.DestArea.Bottom = renderSettings.DestArea.Top + imageSize.Height;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // adjust background area to miss this edge
                backgroundRect.d_min.Y += imageSize.Height + componentImage.GetRenderedOffset().Y;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + componentImage.GetRenderedOffset().X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + componentImage.GetRenderedOffset().Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this image and append it to the Window's geometry
                var imageGeomBuffers = CreateRenderGeometryForImage(componentImage,
                                                                    VerticalFormatting.TopAligned,
                                                                    TopEdgeFormatting.Get(srcWindow),
                                                                    renderSettings.DestArea,
                                                                    renderSettings.MultiplyColours,
                                                                    clipper,
                                                                    clipToDisplay);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // bottom image
            componentImage = GetImage(FrameImageComponent.BottomEdge, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                renderSettings.DestArea.Left = destRect.Left + bottomOffset;
                renderSettings.DestArea.Right = renderSettings.DestArea.Left + bottomWidth;
                renderSettings.DestArea.Bottom = destRect.Bottom;
                renderSettings.DestArea.Top = renderSettings.DestArea.Bottom - imageSize.Height;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // adjust background area to miss this edge
                backgroundRect.d_max.Y -= imageSize.Height - componentImage.GetRenderedOffset().Y;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + componentImage.GetRenderedOffset().X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + componentImage.GetRenderedOffset().Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this image and append it to the Window's geometry
                var imageGeomBuffers = CreateRenderGeometryForImage(componentImage,
                                                                    VerticalFormatting.BottomAligned,
                                                                    BottomEdgeFormatting.Get(srcWindow),
                                                                    renderSettings.DestArea,
                                                                    renderSettings.MultiplyColours,
                                                                    clipper,
                                                                    clipToDisplay);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // left image
            componentImage = GetImage(FrameImageComponent.LeftEdge, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                renderSettings.DestArea.Left = destRect.Left;
                renderSettings.DestArea.Right = renderSettings.DestArea.Left + imageSize.Width;
                renderSettings.DestArea.Top = destRect.Top + leftOffset;
                renderSettings.DestArea.Bottom = renderSettings.DestArea.Top + leftHeight;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // adjust background area to miss this edge
                backgroundRect.d_min.X += imageSize.Width + componentImage.GetRenderedOffset().X;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + componentImage.GetRenderedOffset().X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + componentImage.GetRenderedOffset().Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this image and append it to the Window's geometry
                var imageGeomBuffers = CreateRenderGeometryForImage(componentImage,
                                                                    LeftEdgeFormatting.Get(srcWindow),
                                                                    HorizontalFormatting.LeftAligned,
                                                                    renderSettings.DestArea,
                                                                    renderSettings.MultiplyColours,
                                                                    clipper,
                                                                    clipToDisplay);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            // right image
            componentImage = GetImage(FrameImageComponent.RightEdge, srcWindow);
            if (componentImage != null)
            {
                // calculate final destination area
                imageSize = componentImage.GetRenderedSize();
                renderSettings.DestArea.Top = destRect.Top + rightOffset;
                renderSettings.DestArea.Bottom = renderSettings.DestArea.Top + rightHeight;
                renderSettings.DestArea.Right = destRect.Right;
                renderSettings.DestArea.Left = renderSettings.DestArea.Right - imageSize.Width;
                renderSettings.DestArea = destRect.GetIntersection(renderSettings.DestArea);

                // adjust background area to miss this edge
                backgroundRect.d_max.X -= imageSize.Width - componentImage.GetRenderedOffset().X;

                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (renderSettings.DestArea.Left + componentImage.GetRenderedOffset().X) / destRect.Width;
                    rightfactor = leftfactor + renderSettings.DestArea.Width / destRect.Width;
                    topfactor = (renderSettings.DestArea.Top + componentImage.GetRenderedOffset().Y) / destRect.Height;
                    bottomfactor = topfactor + renderSettings.DestArea.Height / destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                // create render geometry for this image and append it to the Window's geometry
                var imageGeomBuffers = CreateRenderGeometryForImage(componentImage,
                                                                    RightEdgeFormatting.Get(srcWindow),
                                                                    HorizontalFormatting.RightAligned,
                                                                    renderSettings.DestArea,
                                                                    renderSettings.MultiplyColours,
                                                                    clipper,
                                                                    clipToDisplay);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }

            componentImage = GetImage(FrameImageComponent.Background, srcWindow);
            if (componentImage != null)
            {
                // calculate colours that are to be used to this component image
                if (calcColoursPerImage)
                {
                    leftfactor = (backgroundRect.Left + componentImage.GetRenderedOffset().X)/destRect.Width;
                    rightfactor = leftfactor + backgroundRect.Width/destRect.Width;
                    topfactor = (backgroundRect.Top + componentImage.GetRenderedOffset().Y)/destRect.Height;
                    bottomfactor = topfactor + backgroundRect.Height/destRect.Height;

                    renderSettings.MultiplyColours = renderSettingFinalColours.GetSubRectangle(leftfactor, rightfactor, topfactor, bottomfactor);
                }

                var horzFormatting = BackgroundHorzFormatting.Get(srcWindow);
                var vertFormatting = BackgroundVertFormatting.Get(srcWindow);

                var imageGeomBuffers = CreateRenderGeometryForImage(componentImage,
                                                                   vertFormatting,
                                                                    horzFormatting,
                                                                    backgroundRect,
                                                                    renderSettings.MultiplyColours,
                                                                    clipper,
                                                                    clipToDisplay);
                srcWindow.AppendGeometryBuffers(imageGeomBuffers);
            }
        }

        protected List<GeometryBuffer> CreateRenderGeometryForImage(Image image, VerticalFormatting vertFmt, HorizontalFormatting horzFmt, Rectf destRect, ColourRect colours, Rectf? clipper, bool clipToDisplay)
        {
            int horzTiles, vertTiles;
            float xpos, ypos;

            var imgSz = image.GetRenderedSize();

            // calculate initial x co-ordinate and horizontal tile count according to formatting options
            switch (horzFmt)
            {
                case HorizontalFormatting.Stretched:
                    imgSz.Width = destRect.Width;
                    xpos = destRect.Left;
                    horzTiles = 1;
                    break;

                case HorizontalFormatting.Tiled:
                    xpos = destRect.Left;
                    horzTiles = Math.Abs((int) ((destRect.Width + (imgSz.Width - 1))/imgSz.Width));
                    break;

                case HorizontalFormatting.LeftAligned:
                    xpos = destRect.Left;
                    horzTiles = 1;
                    break;

                case HorizontalFormatting.CentreAligned:
                    xpos = destRect.Left + CoordConverter.AlignToPixels((destRect.Width - imgSz.Width)*0.5f);
                    horzTiles = 1;
                    break;

                case HorizontalFormatting.RightAligned:
                    xpos = destRect.Right - imgSz.Width;
                    horzTiles = 1;
                    break;

                default:
                    throw new InvalidRequestException("An unknown HorizontalFormatting value was specified.");
            }

            // calculate initial y co-ordinate and vertical tile count according to formatting options
            switch (vertFmt)
            {
                case VerticalFormatting.Stretched:
                    imgSz.Height = destRect.Height;
                    ypos = destRect.Top;
                    vertTiles = 1;
                    break;

                case VerticalFormatting.Tiled:
                    ypos = destRect.Top;
                    vertTiles = Math.Abs((int)((destRect.Height + (imgSz.Height - 1))/imgSz.Height));
                    break;

                case VerticalFormatting.TopAligned:
                    ypos = destRect.Top;
                    vertTiles = 1;
                    break;

                case VerticalFormatting.CentreAligned:
                    ypos = destRect.Top + CoordConverter.AlignToPixels((destRect.Height - imgSz.Height)*0.5f);
                    vertTiles = 1;
                    break;

                case VerticalFormatting.BottomAligned:
                    ypos = destRect.Bottom - imgSz.Height;
                    vertTiles = 1;
                    break;

                default:
                    throw new InvalidRequestException("An unknown VerticalFormatting value was specified.");
            }

            // Create the render geometry
            var geomBuffers = new List<GeometryBuffer>();
            var renderSettings = new ImageRenderSettings(Rectf.Zero, null, !clipToDisplay, colours);
            renderSettings.DestArea.d_min.Y = ypos;
            renderSettings.DestArea.d_max.Y = ypos + imgSz.Height;

            for (uint row = 0; row < vertTiles; ++row)
            {
                renderSettings.DestArea.d_min.X = xpos;
                renderSettings.DestArea.d_max.X = xpos + imgSz.Width;

                for (uint col = 0; col < horzTiles; ++col)
                {
                    // use custom clipping for right and bottom edges when tiling the imagery
                    if (((vertFmt == VerticalFormatting.Tiled) && row == vertTiles - 1) ||
                        ((horzFmt == HorizontalFormatting.Tiled) && col == horzTiles - 1))
                    {
                        renderSettings.ClipArea = clipper.HasValue ? clipper.Value.GetIntersection(destRect) : destRect;
                    }
                    else
                    {
                        // not tiling, or not on far edges, just used passed in clipper (if any).
                        renderSettings.ClipArea = clipper;
                    }

                    geomBuffers.AddRange(image.CreateRenderGeometry(renderSettings));
                    
                    renderSettings.DestArea.d_min.X += imgSz.Width;
                    renderSettings.DestArea.d_max.X += imgSz.Width;
                }

                renderSettings.DestArea.d_min.Y += imgSz.Height;
                renderSettings.DestArea.d_max.Y += imgSz.Height;
            }

            return geomBuffers;
        }

        protected FormattingSetting<VerticalFormatting>   LeftEdgeFormatting;
        protected FormattingSetting<VerticalFormatting>   RightEdgeFormatting;
        protected FormattingSetting<HorizontalFormatting> TopEdgeFormatting;
        protected FormattingSetting<HorizontalFormatting> BottomEdgeFormatting;
        protected FormattingSetting<VerticalFormatting>   BackgroundVertFormatting;
        protected FormattingSetting<HorizontalFormatting> BackgroundHorzFormatting;

        /// <summary>
        /// FrameImageSource array describing images to be used.
        /// </summary>
        protected FrameImageSource[] FrameImages = new FrameImageSource[(int)FrameImageComponent.FIC_FRAME_IMAGE_COUNT];
    }
}