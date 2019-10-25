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
    /// Dimension type that represents some dimension of a Window/widget.
    /// Implements BaseDim interface.
    /// 
    /// When calculating the final pixel value for the dimension, a target widget
    /// name is built by appending the name specified in the WidgetDim to the name
    /// path of the window passed to getValue, we then use the window/widget with
    /// that name to obtain the value for the dimension.
    /// </summary>
    public class WidgetDim : BaseDim
    {
        // TODO: do we really need this one.
        // public WidgetDim() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">
        /// String object holding the name suffix for a window/widget.
        /// </param>
        /// <param name="dim">
        /// DimensionType value indicating which dimension of the described image that this ImageDim
        /// is to represent.
        /// </param>
        public WidgetDim(string name, DimensionType dim)
        {
            d_widgetName = name;
            d_what = dim;
        }

        /// <summary>
        /// Get the name suffix to use for this WidgetDim.
        /// </summary>
        /// <returns>
        /// String object holding the name suffix for a window/widget.
        /// </returns>
        public string GetWidgetName()
        {
            return d_widgetName;
        }

        /// <summary>
        /// Set the name suffix to use for this WidgetDim.
        /// </summary>
        /// <param name="name">
        /// String object holding the name suffix for a window/widget.
        /// </param>
        public void SetWidgetName(string name)
        {
            d_widgetName = name;
        }

        /// <summary>
        /// Gets the source dimension type for this WidgetDim.
        /// </summary>
        /// <returns>
        /// DimensionType value indicating which dimension of the described image
        /// that this WidgetDim is to represent.
        /// </returns>
        public DimensionType GetSourceDimension()
        {
            return d_what;
        }

        /// <summary>
        /// Sets the source dimension type for this WidgetDim.
        /// </summary>
        /// <param name="dim">
        /// DimensionType value indicating which dimension of the described image
        /// that this WidgetDim is to represent.
        /// </param>
        public void SetSourceDimension(DimensionType dim)
        {
            d_what = dim;
        }

        // Implementation of the base class interface
        public override float GetValue(Window wnd)
        {
            Window widget;

            // if target widget name is empty, then use the input window.
            if (String.IsNullOrEmpty(d_widgetName))
            {
                widget = wnd;
            }
            // name not empty, so find window with required name
            else
            {
                widget = wnd.GetChild(d_widgetName);
            }

            // get size of parent; required to extract pixel values
            var parentSize = widget.GetParentPixelSize();

            switch (d_what)
            {
                case DimensionType.Width:
                    return widget.GetPixelSize().Width;

                case DimensionType.Height:
                    return widget.GetPixelSize().Height;

                case DimensionType.XOffset:
                    System.GetSingleton().Logger
                        .LogEvent("WigetDim.GetValue - Nonsensical DimensionType of XOffset specified! returning 0.0f");
                    return 0.0f;
                    
                case DimensionType.YOffset:
                    System.GetSingleton().Logger
                        .LogEvent("WigetDim.GetValue - Nonsensical DimensionType of YOffset specified! returning 0.0f");
                    return 0.0f;

                case DimensionType.LeftEdge:
                case DimensionType.XPosition:
                    return CoordConverter.AsAbsolute(widget.GetPosition().d_x, parentSize.Width);

                case DimensionType.TopEdge:
                case DimensionType.YPosition:
                    return CoordConverter.AsAbsolute(widget.GetPosition().d_y, parentSize.Height);

                case DimensionType.RightEdge:
                    return CoordConverter.AsAbsolute(widget.GetArea().d_max.d_x, parentSize.Width);

                case DimensionType.BottomEdge:
                    return CoordConverter.AsAbsolute(widget.GetArea().d_max.d_y, parentSize.Height);

                default:
                    throw new InvalidRequestException("unknown or unsupported DimensionType encountered.");
            }
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            // This dimension type does not alter when whithin a container Rect.
            return GetValue(wnd);
        }

        public override BaseDim Clone()
        {
            return new WidgetDim(d_widgetName, d_what);
        }

        // Implementation of the base class interface
        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("WidgetDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            if (!String.IsNullOrEmpty(d_widgetName))
                xmlStream.Attribute("widget", d_widgetName);
            
            xmlStream.Attribute("dimension", /*FalagardXMLHelper*/PropertyHelper.ToString<DimensionType>(d_what));
        }

        #region Fields

        /// <summary>
        /// Holds target window name suffix.
        /// </summary>
        private string d_widgetName;

        /// <summary>
        /// the dimension of the target window that we are to represent.
        /// </summary>
        private DimensionType d_what;

        #endregion
    };
}