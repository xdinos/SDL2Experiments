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
    /// Abstract ImageLoader class. An image loader encapsulate the loading of a texture.
    /// 
    /// This class define the loading of an abstract 
    /// </summary>
    public abstract class ImageCodec
    {
        // TODO: virtual ~ImageCodec();
    
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="name">
        /// name of the codec
        /// </param>
        protected ImageCodec(string name)
        {
            d_identifierString = name;
        }

        /// <summary>
        /// Return the name of the image codec object 
        /// Return the name of the image codec 
        /// </summary>
        /// <returns>
        /// a string containing image codec name 
        /// </returns>
        public string GetIdentifierString()
        {
            return d_identifierString;
        }

        /// <summary>
        /// Return the list of image file format supported 
        /// Return a list of space separated image format supported by this
        /// codec
        /// </summary>
        /// <returns>
        /// list of supported image file format separated with space 
        /// </returns>
        public string GetSupportedFormat()
        {
            return d_supportedFormat;
        }
    
        /// <summary>
        /// Load an image from a memory buffer 
        /// </summary>
        /// <param name="data">
        /// data the image data 
        /// </param>
        /// <param name="result">
        /// the texture to use for storing the image data 
        /// </param>
        /// <returns>
        /// result on success or null if the load failed 
        /// </returns>
        public abstract Texture Load(RawDataContainer data, Texture result);

        private string d_identifierString;   //!< display the name of the codec 

        protected string d_supportedFormat;   //!< list all image file format supported 

        // TODO: 
    //private:
    //    ImageCodec(const ImageCodec& obj);
    //    ImageCodec& operator=(ImageCodec& obj);
    }
}