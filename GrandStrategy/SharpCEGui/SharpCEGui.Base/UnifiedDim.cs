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
using System.Globalization;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Dimension type that represents an Unified dimension.
    /// Implements BaseDim interface.
    /// </summary>
    public class UnifiedDim : BaseDim
    {
        //public UnifiedDim(){}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">
        /// UDim holding the value to assign to this UnifiedDim.
        /// </param>
        /// <param name="dim">
        /// DimensionType value indicating what this UnifiedDim is to represent.  This is required
        /// because we need to know what part of the base Window that the UDim scale component is
        /// to operate against.
        /// </param>
        public UnifiedDim(UDim value, DimensionType dim)
        {
            d_value = value;
            d_what = dim;
        }

        /// <summary>
        /// Get the current value of the UnifiedDim.
        /// </summary>
        /// <returns></returns>
        public UDim GetBaseValue()
        {
            return d_value;
        }

        /// <summary>
        /// Set the current value of the UnifiedDim.
        /// </summary>
        /// <param name="val"></param>
        public void SetBaseValue(UDim val)
        {
            d_value = val;
        }

        /// <summary>
        /// Gets the source dimension type for this WidgetDim.
        /// </summary>
        /// <returns>
        /// DimensionType value indicating which dimension should be used as the
        /// scale reference / base value when calculating the pixel value of this
        /// dimension.
        /// </returns>
        public DimensionType GetSourceDimension()
        {
            return d_what;
        }

        /// <summary>
        /// Sets the source dimension type for this WidgetDim.
        /// </summary>
        /// <param name="value">
        /// DimensionType value indicating which dimension should be used as the
        /// scale reference / base value when calculating the pixel value of this
        /// dimension.
        /// </param>
        public void SetSourceDimension(DimensionType value)
        {
            d_what = value;
        }

        // Implementation of the base class interface
        public override float GetValue(Window wnd)
        {
            switch (d_what)
            {
                case DimensionType.LeftEdge:
                case DimensionType.RightEdge:
                case DimensionType.XPosition:
                case DimensionType.XOffset:
                case DimensionType.Width:
                    return CoordConverter.AsAbsolute(d_value, wnd.GetPixelSize().Width);

                case DimensionType.TopEdge:
                case DimensionType.BottomEdge:
                case DimensionType.YPosition:
                case DimensionType.YOffset:
                case DimensionType.Height:
                    return CoordConverter.AsAbsolute(d_value, wnd.GetPixelSize().Height);

                default:
                    throw new InvalidRequestException("unknown or unsupported DimensionType encountered.");
            }
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            switch (d_what)
            {
                case DimensionType.LeftEdge:
                case DimensionType.RightEdge:
                case DimensionType.XPosition:
                case DimensionType.XOffset:
                case DimensionType.Width:
                    return CoordConverter.AsAbsolute(d_value, container.Width);

                case DimensionType.TopEdge:
                case DimensionType.BottomEdge:
                case DimensionType.YPosition:
                case DimensionType.YOffset:
                case DimensionType.Height:
                    return CoordConverter.AsAbsolute(d_value, container.Height);

                default:
                    throw new InvalidRequestException("unknown or unsupported DimensionType encountered.");
            }
        }

        public override BaseDim Clone()
        {
            return new UnifiedDim(d_value, d_what);
        }

        // Implementation of the base class interface
        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("UnifiedDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            if (Math.Abs(d_value.d_scale - 0) > float.Epsilon)
                xmlStream.Attribute("scale", d_value.d_scale.ToString(CultureInfo.InvariantCulture));

            if (Math.Abs(d_value.d_offset - 0) > float.Epsilon)
                xmlStream.Attribute("offset", d_value.d_offset.ToString(CultureInfo.InvariantCulture));

            xmlStream.Attribute("type", d_what.ToString());
        }

        #region Fields

        /// <summary>
        /// The UDim value.
        /// </summary>
        private UDim d_value;
        
        /// <summary>
        /// what to use as the base / reference for scale.
        /// </summary>
        private DimensionType d_what;

        #endregion
    };
}