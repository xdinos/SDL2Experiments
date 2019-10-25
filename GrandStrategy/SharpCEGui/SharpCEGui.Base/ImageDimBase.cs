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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Dimension type that represents some dimension of an Image.
    /// Implements BaseDim interface.
    /// </summary>
    public abstract class ImageDimBase : BaseDim
    {
        protected ImageDimBase() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dim">
        /// DimensionType value indicating which dimension of an Image that
        /// this ImageDim is to represent.
        /// </param>
        protected ImageDimBase(DimensionType dim)
        {
            d_what = dim;
        }

        /// <summary>
        /// Gets the source dimension type for this WidgetDim.
        /// </summary>
        /// <returns>
        /// DimensionType value indicating which dimension of the described
        /// image that this WidgetDim is to represent.
        /// </returns>
        public DimensionType GetSourceDimension()
        {
            return d_what;
        }

        /// <summary>
        /// Sets the source dimension type for this ImageDim.
        /// </summary>
        /// <param name="dim">
        /// DimensionType value indicating which dimension of the described
        /// image that this ImageDim is to represent.
        /// </param>
        public void SetSourceDimension(DimensionType dim)
        {
            d_what = dim;
        }

        // Implementation of the base class interface
        public override float GetValue(Window wnd)
        {
            var img = GetSourceImage(wnd);

            if (img== null)
                return 0.0f;

            switch (d_what)
            {
                case DimensionType.Width:
                    return img.GetRenderedSize().Width;

                case DimensionType.Height:
                    return img.GetRenderedSize().Height;

                case DimensionType.XOffset:
                    return img.GetRenderedOffset().X;

                case DimensionType.YOffset:
                    return img.GetRenderedOffset().Y;

                // these other options will not be particularly useful for most people 
                // since they return the edges of the image on the source texture.
                //case DT_LEFT_EDGE:
                //case DT_X_POSITION:
                //    return img->getSourceTextureArea().d_left;
                
                //case DT_TOP_EDGE:
                //case DT_Y_POSITION:
                //    return img->getSourceTextureArea().d_top;
                
                //case DT_RIGHT_EDGE:
                //    return img->getSourceTextureArea().d_right;
                
                //case DT_BOTTOM_EDGE:
                //    return img->getSourceTextureArea().d_bottom;
        
                default:
                    throw new InvalidRequestException("unknown or unsupported DimensionType encountered.");
            }
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            // This dimension type does not alter when whithin a container Rect.
            return GetValue(wnd);
        }

        //! return the image instance to access
        protected abstract Image GetSourceImage(Window wnd);

        // Implementation of the base class interface
        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            xmlStream.Attribute("dimension", /*FalagardXMLHelper*/ PropertyHelper.ToString(d_what));
        }

        /// <summary>
        /// the dimension of the image that we are to represent.
        /// </summary>
        protected DimensionType d_what;
    }
}