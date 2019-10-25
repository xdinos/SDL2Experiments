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
    /// Abstract interface for a generic 'dimension' class.
    /// </summary>
    public abstract class BaseDim
    {
        /// <summary>
        /// Return a value that represents this dimension as absolute pixels.
        /// </summary>
        /// <param name="wnd">
        /// Window object that may be used by the specialised class to aid in
        /// calculating the final value.
        /// </param>
        /// <returns>
        /// float value which represents, in pixels, the same value as this BaseDim.
        /// </returns>
        public abstract float GetValue(Window wnd);

        /// <summary>
        /// Return a value that represents this dimension as absolute pixels.
        /// </summary>
        /// <param name="wnd">
        /// Window object that may be used by the specialised class to aid in
        /// calculating the final value (typically would be used to obtain
        /// window/widget dimensions).
        /// </param>
        /// <param name="container">
        /// Rect object which describes an area to be considered as the base area
        /// when calculating the final value.  Basically this means that relative
        /// values are calculated from the dimensions of this Rect.
        /// </param>
        /// <returns>
        /// float value which represents, in pixels, the same value as this BaseDim.
        /// </returns>
        public abstract float GetValue(Window wnd, Rectf container);

        /// <summary>
        /// Create an exact copy of the specialised object and return it as a
        /// pointer to a BaseDim object.
        /// 
        /// Since the system needs to be able to copy objects derived from BaseDim,
        /// but only has knowledge of the BaseDim interface, this clone method is
        /// provided to prevent slicing issues.
        /// </summary>
        /// <returns></returns>
        public abstract BaseDim Clone();

        /// <summary>
        /// Writes an xml representation of this BaseDim to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// Stream where xml data should be output.
        /// </param>
        public virtual void WriteXmlToStream(XMLSerializer xmlStream)
        {
            // get sub-class to output the data for this single dimension
            WriteXmlElementNameImpl(xmlStream);
            WriteXmlElementAttributesImpl(xmlStream);
            xmlStream.CloseTag();
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public virtual bool HandleFontRenderSizeChange(Window window, Font font)
        {
            return false;
        }

        /// <summary>
        /// Implementataion method to output real xml element name.
        /// </summary>
        /// <param name="xmlStream"></param>
        protected abstract void WriteXmlElementNameImpl(XMLSerializer xmlStream);

        /// <summary>
        /// Implementataion method to create the element attributes
        /// </summary>
        /// <param name="xmlStream"></param>
        protected abstract void WriteXmlElementAttributesImpl(XMLSerializer xmlStream);
    };
}