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
    /// Class that encapsulates information for a single image component.
    /// </summary>
    public class ImageryComponent : ComponentBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ImageryComponent()
        {
            Image = null;
            VerticalFormatting = new FormattingSetting<VerticalFormatting>(Base.VerticalFormatting.TopAligned);
            HorizontalFormatting = new FormattingSetting<HorizontalFormatting>(Base.HorizontalFormatting.LeftAligned);
        }

        /// <summary>
        /// Return the Image object that will be drawn by this ImageryComponent.
        /// </summary>
        /// <returns>
        /// Image object.
        /// </returns>
        public Image GetImage()
        {
            return Image;
        }

        /// <summary>
        /// Set the Image that will be drawn by this ImageryComponent.
        /// </summary>
        /// <param name="image">
        /// Pointer to the Image object to be drawn by this ImageryComponent.
        /// </param>
        public void SetImage(Image image)
        {
            this.Image = image;
        }

        /// <summary>
        /// Set the Image that will be drawn by this ImageryComponent.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Image to be rendered.
        /// </param>
        public void SetImage(string name)
        {
            try 
            {
                Image = ImageManager.GetSingleton().Get(name);
            }
            catch(UnknownObjectException)
            {
                Image = null;
            }
        }

        /// <summary>
        /// Return the current vertical formatting setting for this ImageryComponent.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the VerticalFormatting enumerated values.
        /// </returns>
        public VerticalFormatting GetVerticalFormatting(Window wnd)
        {
            return VerticalFormatting.Get(wnd);
        }

        /// <summary>
        /// Set the vertical formatting setting for this ImageryComponent.
        /// </summary>
        /// <param name="fmt">
        /// One of the VerticalFormatting enumerated values.
        /// </param>
        public void SetVerticalFormatting(VerticalFormatting fmt)
        {
            VerticalFormatting.Set(fmt);
        }

        /// <summary>
        /// Return the current horizontal formatting setting for this ImageryComponent.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the HorizontalFormatting enumerated values.
        /// </returns>
        public HorizontalFormatting GetHorizontalFormatting(Window wnd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the horizontal formatting setting for this ImageryComponent.
        /// </summary>
        /// <param name="fmt">
        /// One of the HorizontalFormatting enumerated values.
        /// </param>
        public void SetHorizontalFormatting(HorizontalFormatting fmt)
        {
            HorizontalFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the horizontal
        /// formatting to use for this ImageryComponent.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetHorizontalFormattingPropertySource(string propertyName)
        {
            HorizontalFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the vertical
        /// formatting to use for this ImageryComponent.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetVerticalFormattingPropertySource(string propertyName)
        {
            VerticalFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Writes an xml representation of this ImageryComponent to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xmlStream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether this ImageryComponent fetches it's image via a property on the target window.
        /// </summary>
        /// <returns>
        /// - true if the image comes via a Propery.
        /// - false if the image is defined explicitly.
        /// </returns>
        public bool IsImageFetchedFromProperty()
        {
            return !String.IsNullOrEmpty(ImagePropertyName);
        }

        /// <summary>
        /// Return the name of the property that will be used to determine the image for this ImageryComponent.
        /// </summary>
        /// <returns>
        /// String object holding the name of a Propery.
        /// </returns>
        public string GetImagePropertySource()
        {
            return ImagePropertyName;
        }

        /// <summary>
        /// Set the name of the property that will be used to determine the image for this ImageryComponent.
        /// </summary>
        /// <param name="property">
        /// String object holding the name of a Propery.  The property should access a imageset & image specification.
        /// </param>
        public void SetImagePropertySource(string property)
        {
            ImagePropertyName = property;
        }

        // implemets abstract from base
        protected override void AddImageRenderGeometryToWindowImpl(Window srcWindow, Rectf destRect, ColourRect modColours, Rectf? clipper, bool clipToDisplay)
        {
            // get final image to use.
            var img = IsImageFetchedFromProperty()
                          ? srcWindow.GetProperty<Image>(ImagePropertyName)
                          : Image;

            // do not draw anything if image is not set.
            if (img == null)
                return;

            var horzFormatting = HorizontalFormatting.Get(srcWindow);
            var vertFormatting = VerticalFormatting.Get(srcWindow);

            int horzTiles, vertTiles;
            float xpos, ypos;

            var imgSz = img.GetRenderedSize();

            // calculate final colours to be used
            ColourRect finalColours;
            InitColoursRect(srcWindow, modColours, out finalColours);

            // calculate initial x co-ordinate and horizontal tile count according to formatting options
            switch (horzFormatting)
            {
                case Base.HorizontalFormatting.Stretched:
                    imgSz.Width = destRect.Width;
                    xpos = destRect.Left;
                    horzTiles = 1;
                    break;

                case Base.HorizontalFormatting.Tiled:
                    xpos = destRect.Left;
                    horzTiles = Math.Abs((int) ((destRect.Width + (imgSz.Width - 1))/imgSz.Width));
                    break;

                case Base.HorizontalFormatting.LeftAligned:
                    xpos = destRect.Left;
                    horzTiles = 1;
                    break;

                case Base.HorizontalFormatting.CentreAligned:
                    xpos = destRect.Left + CoordConverter.AlignToPixels((destRect.Width - imgSz.Width)*0.5f);
                    horzTiles = 1;
                    break;

                case Base.HorizontalFormatting.RightAligned:
                    xpos = destRect.Right - imgSz.Width;
                    horzTiles = 1;
                    break;

                default:
                    throw new InvalidRequestException("An unknown HorizontalFormatting value was specified.");
            }

            // calculate initial y co-ordinate and vertical tile count according to formatting options
            switch (vertFormatting)
            {
                case Base.VerticalFormatting.Stretched:
                    imgSz.Height = destRect.Height;
                    ypos = destRect.Top;
                    vertTiles = 1;
                    break;

                case Base.VerticalFormatting.Tiled:
                    ypos = destRect.Top;
                    vertTiles = Math.Abs((int) ((destRect.Height + (imgSz.Height - 1))/imgSz.Height));
                    break;

                case Base.VerticalFormatting.TopAligned:
                    ypos = destRect.Top;
                    vertTiles = 1;
                    break;

                case Base.VerticalFormatting.CentreAligned:
                    ypos = destRect.Top + CoordConverter.AlignToPixels((destRect.Height - imgSz.Height)*0.5f);
                    vertTiles = 1;
                    break;

                case Base.VerticalFormatting.BottomAligned:
                    ypos = destRect.Bottom - imgSz.Height;
                    vertTiles = 1;
                    break;

                default:
                    throw new InvalidRequestException("An unknown VerticalFormatting value was specified.");
            }

            // perform final rendering (actually is now a caching of the images which will be drawn)
            var  imgRenderSettings = new ImageRenderSettings(Rectf.Zero, null, !clipToDisplay, finalColours);
            imgRenderSettings.DestArea.Top = ypos;
            imgRenderSettings.DestArea.Bottom = ypos + imgSz.Height;

            for (uint row = 0; row < vertTiles; ++row)
            {
                imgRenderSettings.DestArea.Left = xpos;
                imgRenderSettings.DestArea.Right = xpos + imgSz.Width;

                for (uint col = 0; col < horzTiles; ++col)
                {
                    // use custom clipping for right and bottom edges when tiling the imagery
                    if (((vertFormatting == Base.VerticalFormatting.Tiled) && row == vertTiles - 1) ||
                        ((horzFormatting == Base.HorizontalFormatting.Tiled) && col == horzTiles - 1))
                    {
                        imgRenderSettings.ClipArea = clipper.HasValue
                                                             ? clipper.Value.GetIntersection(destRect)
                                                             : destRect;
                    }
                        // not tiliing, or not on far edges, just used passed in clipper (if any).
                    else
                    {
                        imgRenderSettings.ClipArea = clipper;
                    }

                    // add geometry for image to the target window.
                    var geomBuffers = img.CreateRenderGeometry(imgRenderSettings);
                    srcWindow.AppendGeometryBuffers(geomBuffers);

                    imgRenderSettings.DestArea.d_min.X += imgSz.Width;
                    imgRenderSettings.DestArea.d_max.X += imgSz.Width;
                }

                imgRenderSettings.DestArea.d_min.Y += imgSz.Height;
                imgRenderSettings.DestArea.d_max.Y += imgSz.Height;
            }
        }

        /// <summary>
        /// Image to be drawn by this image component.
        /// </summary>
        protected Image Image;
        
        /// <summary>
        /// Vertical formatting to be applied when rendering the image component.
        /// </summary>
        protected FormattingSetting<VerticalFormatting> VerticalFormatting;
        
        /// <summary>
        /// Horizontal formatting to be applied when rendering the image component.
        /// </summary>
        protected FormattingSetting<HorizontalFormatting> HorizontalFormatting;
        
        /// <summary>
        /// Name of the property to access to obtain the image to be used.
        /// </summary>
        protected string ImagePropertyName;
    }
}