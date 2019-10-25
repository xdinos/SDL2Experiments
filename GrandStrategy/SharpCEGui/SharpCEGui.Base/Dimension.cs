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
    /// Class representing some kind of dimension.
    /// 
    /// The key thing to understand about Dimension is that it contains not just a
    /// dimensional value, but also a record of what the dimension value is supposed
    /// to represent. (e.g. a co-ordinate on the x axis, or the height of something).
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// 
        /// </summary>
        public Dimension()
        {
            _value = null;
            _type = DimensionType.Invalid;
        }

        // TODO: ...
        //~Dimension()
        //{
        //    if (d_value)
        //        CEGUI_DELETE_AO d_value;
        //}

        // TODO: ...
        //Dimension(const Dimension& other)
        //{
        //    d_value = other.d_value ? other.d_value->clone() : 0;
        //    d_type = other.d_type;
        //}

        // TODO: ...
        //Dimension& operator=(const Dimension& other)
        //{
        //    // release old value, if any.
        //    if (d_value)
        //        CEGUI_DELETE_AO d_value;

        //    d_value = other.d_value ? other.d_value->clone() : 0;
        //    d_type = other.d_type;

        //    return *this;
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dim">
        /// object based on subclass of BaseDim which holds the dimensional value.
        /// </param>
        /// <param name="type">
        /// DimensionType value indicating what dimension this object is to represent.
        /// </param>
        public Dimension(BaseDim dim, DimensionType type)
        {
            _value = dim.Clone();
            _type = type;
        }

        /// <summary>
        /// const reference to the BaseDim sub-class object which contains the value
        /// for this Dimension.
        /// </summary>
        /// <returns>
        /// const reference to the BaseDim sub-class object which contains the value
        /// for this Dimension.
        /// </returns>
        public BaseDim GetBaseDimension()
        {
            global::System.Diagnostics.Debug.Assert(_value != null);
            return _value;
        }

        /// <summary>
        /// set the current value for this Dimension.
        /// </summary>
        /// <param name="dim">
        /// object based on a subclass of BaseDim which holds the dimensional value.
        /// </param>
        public void SetBaseDimension(BaseDim dim)
        {
            // TODO: ...
            // release old value, if any.
            //if (d_value)
            //    CEGUI_DELETE_AO d_value;

            _value = dim.Clone();
        }

        /// <summary>
        /// Return a DimensionType value indicating what this Dimension represents.
        /// </summary>
        /// <returns>
        /// one of the DimensionType enumerated values.
        /// </returns>
        public DimensionType GetDimensionType()
        {
            return _type;
        }

        /// <summary>
        /// Sets what this Dimension represents.
        /// </summary>
        /// <param name="type">
        /// one of the DimensionType enumerated values.
        /// </param>
        public void SetDimensionType(DimensionType type)
        {
            _type = type;
        }

        /// <summary>
        /// Writes an xml representation of this Dimension to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXmlToStream(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("Dim")
                .Attribute("type", _type.ToString());
            
            if (_value!=null)
                _value.WriteXmlToStream(xmlStream);
            xmlStream.CloseTag();
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            return _value != null && _value.HandleFontRenderSizeChange(window, font);
        }

        #region Fields
        
        /// <summary>
        /// Pointer to the value for this Dimension.
        /// </summary>
        private BaseDim _value;

        /// <summary>
        /// What we represent.
        /// </summary>
        private DimensionType _type;

        #endregion
    };
}