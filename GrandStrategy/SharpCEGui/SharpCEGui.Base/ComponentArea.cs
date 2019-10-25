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
    /// Class that represents a target area for a widget or imagery component.
    /// 
    /// This is essentially a Rect built out of Dimension objects.  Of note is that
    /// what would normally be the 'right' and 'bottom' edges may alternatively
    /// represent width and height depending upon what the assigned Dimension(s)
    /// represent.
    /// </summary>
    public class ComponentArea
    {
        public ComponentArea()
        {
            d_left = new Dimension(new AbsoluteDim(0.0f), DimensionType.LeftEdge);
            d_top = new Dimension(new AbsoluteDim(0.0f), DimensionType.TopEdge);
            d_right_or_width = new Dimension(new UnifiedDim(new UDim(1.0f, 0.0f), DimensionType.Width), DimensionType.RightEdge);
            d_bottom_or_height = new Dimension(new UnifiedDim(new UDim(1.0f, 0.0f), DimensionType.Height), DimensionType.BottomEdge);
        }

        /// <summary>
        /// Return a Rect describing the absolute pixel area represented by this
        /// ComponentArea.
        /// </summary>
        /// <param name="wnd">
        /// Window object to be used when calculating final pixel area.
        /// </param>
        /// <returns>
        /// Rectf object describing the pixels area represented by this ComponentArea
        /// when using \a wnd as a reference for calculating the final pixel
        /// dimensions.
        /// </returns>
        public Rectf GetPixelRect(Window wnd)
        {
            var pixelRect = new Rectf();

            // use a property?
            if (IsAreaFetchedFromProperty())
            {
                pixelRect = CoordConverter.AsAbsolute(wnd.GetProperty<URect>(_namedSource), wnd.GetPixelSize());
            }
            else if (IsAreaFetchedFromNamedArea())
            {
                return WidgetLookManager.GetSingleton()
                                        .GetWidgetLook(_namedAreaSourceLook)
                                        .GetNamedArea(_namedSource)
                                        .GetArea()
                                        .GetPixelRect(wnd);
            }
            else
            {
                // not via property or named area- calculate using Dimensions

                // sanity check, we mus be able to form a Rect from what we represent.
                global::System.Diagnostics.Debug.Assert(d_left.GetDimensionType() == DimensionType.LeftEdge ||
                                                        d_left.GetDimensionType() == DimensionType.XPosition);
                global::System.Diagnostics.Debug.Assert(d_top.GetDimensionType() == DimensionType.TopEdge ||
                                                        d_top.GetDimensionType() == DimensionType.YPosition);
                global::System.Diagnostics.Debug.Assert(d_right_or_width.GetDimensionType() == DimensionType.RightEdge ||
                                                        d_right_or_width.GetDimensionType() == DimensionType.Width);
                global::System.Diagnostics.Debug.Assert(d_bottom_or_height.GetDimensionType() == DimensionType.BottomEdge ||
                                                        d_bottom_or_height.GetDimensionType() == DimensionType.Height);

                pixelRect.Left = d_left.GetBaseDimension().GetValue(wnd);
                pixelRect.Top = d_top.GetBaseDimension().GetValue(wnd);

                if (d_right_or_width.GetDimensionType() == DimensionType.Width)
                    pixelRect.Width = d_right_or_width.GetBaseDimension().GetValue(wnd);
                else
                    pixelRect.Right = d_right_or_width.GetBaseDimension().GetValue(wnd);

                if (d_bottom_or_height.GetDimensionType() == DimensionType.Height)
                    pixelRect.Height = d_bottom_or_height.GetBaseDimension().GetValue(wnd);
                else
                    pixelRect.Bottom = d_bottom_or_height.GetBaseDimension().GetValue(wnd);
            }

            return pixelRect;
        }

        /// <summary>
        /// Return a Rectd describing the absolute pixel area represented by this
        /// ComponentArea.
        /// </summary>
        /// <param name="wnd">
        /// Window object to be used when calculating final pixel area.
        /// </param>
        /// <param name="container">
        /// Rect object to be used as a base or container when converting relative
        /// dimensions.
        /// </param>
        /// <returns>
        /// Rectf object describing the pixels area represented by this
        /// ComponentArea when using \a wnd and \a container as a reference for
        /// calculating the final pixel dimensions.
        /// </returns>
        public Rectf GetPixelRect(Window wnd, Rectf container)
        {
            var pixelRect = new Rectf();

            // use a property?
            if (IsAreaFetchedFromProperty())
            {
                pixelRect = CoordConverter.AsAbsolute(wnd.GetProperty<URect>(_namedSource), wnd.GetPixelSize());
            }
            else if (IsAreaFetchedFromNamedArea())
            {
                return WidgetLookManager.GetSingleton()
                    .GetWidgetLook(_namedAreaSourceLook)
                    .GetNamedArea(_namedSource)
                    .GetArea()
                    .GetPixelRect(wnd, container);
            }
            else
            {
                // not via property or named area- calculate using Dimensions

                // sanity check, we mus be able to form a Rect from what we represent.
                global::System.Diagnostics.Debug.Assert(d_left.GetDimensionType() == DimensionType.LeftEdge ||
                                                        d_left.GetDimensionType() == DimensionType.XPosition);
                global::System.Diagnostics.Debug.Assert(d_top.GetDimensionType() == DimensionType.TopEdge ||
                                                        d_top.GetDimensionType() == DimensionType.YPosition);
                global::System.Diagnostics.Debug.Assert(d_right_or_width.GetDimensionType() == DimensionType.RightEdge ||
                                                        d_right_or_width.GetDimensionType() == DimensionType.Width);
                global::System.Diagnostics.Debug.Assert(d_bottom_or_height.GetDimensionType() == DimensionType.BottomEdge ||
                                                        d_bottom_or_height.GetDimensionType() == DimensionType.Height);


                pixelRect.Left = d_left.GetBaseDimension().GetValue(wnd, container) + container.Left;
                pixelRect.Top = d_top.GetBaseDimension().GetValue(wnd, container) + container.Top;

                if (d_right_or_width.GetDimensionType() == DimensionType.Width)
                    pixelRect.Width = d_right_or_width.GetBaseDimension().GetValue(wnd, container);
                else
                    pixelRect.Right = d_right_or_width.GetBaseDimension().GetValue(wnd, container) + container.Left;

                if (d_bottom_or_height.GetDimensionType() == DimensionType.Height)
                    pixelRect.Height = d_bottom_or_height.GetBaseDimension().GetValue(wnd, container);
                else
                    pixelRect.Bottom = d_bottom_or_height.GetBaseDimension().GetValue(wnd, container) + container.Top;
            }

            return pixelRect;
        }

        /// <summary>
        /// Writes an xml representation of this ComponentArea to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// XMLSerializer where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("Area");

            // see if we should write an AreaProperty element
            if (IsAreaFetchedFromProperty())
            {
                xmlStream.OpenTag("AreaProperty")
                    .Attribute("name", _namedSource)
                    .CloseTag();
            }
            else if (IsAreaFetchedFromNamedArea())
            {
                xmlStream.OpenTag("NamedAreaSource")
                    .Attribute("look", _namedAreaSourceLook)
                    .Attribute("name", _namedSource)
                    .CloseTag();
            }            
            else
            {
                // not a property, write out individual dimensions explicitly.

                d_left.WriteXmlToStream(xmlStream);
                d_top.WriteXmlToStream(xmlStream);
                d_right_or_width.WriteXmlToStream(xmlStream);
                d_bottom_or_height.WriteXmlToStream(xmlStream);
            }
            xmlStream.CloseTag();
        }

        /// <summary>
        /// Return whether this ComponentArea fetches it's area via a property on
        /// the target window.
        /// </summary>
        /// <returns>
        /// - true if the area comes via a Propery.
        /// - false if the area is not sourced from a Property.
        /// </returns>
        public bool IsAreaFetchedFromProperty()
        {
            return !String.IsNullOrEmpty(_namedSource) && String.IsNullOrEmpty(_namedAreaSourceLook);
        }

        /// <summary>
        /// Return the name of the property that will be used to determine the pixel
        /// area for this ComponentArea.
        /// </summary>
        /// <returns>
        /// String object holding the name of a Propery.
        /// </returns>
        public string GetAreaPropertySource()
        {
            return _namedSource;
        }

        /// <summary>
        /// Set the name of the property that will be used to determine the pixel
        /// area for this ComponentArea.
        /// </summary>
        /// <remarks>
        /// Calling this will replace any existing souce, such as a named area.
        /// </remarks>
        /// <param name="property">
        /// String object holding the name of a Propery.  The property should access
        /// a URect type property.
        /// </param>
        public void SetAreaPropertySource(string property)
        {
            _namedSource = property;
            _namedAreaSourceLook = String.Empty;
        }

        /// <summary>
        /// Set the named area source of the ComponentArea.
        /// </summary>
        /// <param name="widgetLook"></param>
        /// <param name="areaName"></param>
        public void SetNamedAreaSouce(string widgetLook, string areaName)
        {
            _namedSource = areaName;
            _namedAreaSourceLook = widgetLook;
        }

        /// <summary>
        /// Return whether this ComponentArea fetches it's area via a named area
        /// defined.
        /// </summary>
        /// <returns>
        /// - true if the area comes via a named area defined in a WidgetLook.
        /// - false if the area is not sourced from a named area.
        /// </returns>
        public bool IsAreaFetchedFromNamedArea()
        {
            return !String.IsNullOrEmpty(_namedAreaSourceLook) && !String.IsNullOrEmpty(_namedSource);
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            if (IsAreaFetchedFromProperty())
                return false;

            if (IsAreaFetchedFromNamedArea())
            {
                return WidgetLookManager.GetSingleton()
                    .GetWidgetLook(_namedAreaSourceLook)
                    .GetNamedArea(_namedSource)
                    .HandleFontRenderSizeChange(window, font);
            }

            var result = d_left.HandleFontRenderSizeChange(window, font);
            result |= d_top.HandleFontRenderSizeChange(window, font);
            result |= d_right_or_width.HandleFontRenderSizeChange(window, font);
            result |= d_bottom_or_height.HandleFontRenderSizeChange(window, font);

            return result;
        }

        #region Fields

        /// <summary>
        /// Left edge of the area.
        /// </summary>
        public Dimension d_left;

        /// <summary>
        /// Top edge of the area.
        /// </summary>
        public Dimension d_top;

        /// <summary>
        /// Either the right edge or the width of the area.
        /// </summary>
        public Dimension d_right_or_width;
        
        /// <summary>
        /// Either the bototm edge or the height of the area.
        /// </summary>
        public Dimension d_bottom_or_height;

        /// <summary>
        /// name of property or named area: must access a URect style value.
        /// </summary>
        private string _namedSource;

        /// <summary>
        /// name of widget look holding the named area to fetch
        /// </summary>
        private string _namedAreaSourceLook;

        #endregion
    }
}