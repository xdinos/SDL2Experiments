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
    /// ImageDimBase subclass that accesses an image by its name.
    /// </summary>
    public class ImageDim : ImageDimBase
    {
        public ImageDim()
        {
        }

        public ImageDim(string imageName, DimensionType dim)
            : base(dim)
        {
            d_imageName = imageName;
        }

        /// <summary>
        /// return the name of the image accessed by this ImageDim.
        /// </summary>
        /// <returns></returns>
        public string GetSourceImage()
        {
            return d_imageName;
        }

        /// <summary>
        /// set the name of the image accessed by this ImageDim.
        /// </summary>
        /// <param name="image_name"></param>
        public void SetSourceImage(string image_name)
        {
            d_imageName = image_name;
        }

        public override BaseDim Clone()
        {
            return new ImageDim(d_imageName, d_what);
        }

        protected override Image GetSourceImage(Window wnd)
        {
            return ImageManager.GetSingleton().Get(d_imageName);
        }

        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("ImageDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            base.WriteXmlElementAttributesImpl(xmlStream);
            xmlStream.Attribute("name", d_imageName);
        }

        #region Fields

        /// <summary>
        /// name of the Image.
        /// </summary>
        protected string d_imageName;

        #endregion
    }
}