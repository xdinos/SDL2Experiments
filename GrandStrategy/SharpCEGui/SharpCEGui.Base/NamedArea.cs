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
    /// NamedArea defines an area for a component which may later be obtained
    /// and referenced by a name unique to the WidgetLook holding the NamedArea.
    /// </summary>
    public class NamedArea
    {
        // TODO: ??? public NamedArea() {}

        public NamedArea(string name)
        {
            d_name = name;
        }

        /// <summary>
        /// Return the name of this NamedArea.
        /// </summary>
        /// <returns>
        /// String object holding the name of this NamedArea.
        /// </returns>
        public string GetName()
        { 
            return d_name; 
        }

        /// <summary>
        /// set the name for this NamedArea.
        /// </summary>
        /// <param name="name">
        /// String object holding the name of this NamedArea.
        /// </param>
        public void SetName(string name)
        {
            d_name = name;
        }

        /// <summary>
        /// Return the ComponentArea of this NamedArea
        /// </summary>
        /// <returns>
        /// ComponentArea object describing the NamedArea's current target area.
        /// </returns>
        public ComponentArea GetArea()
        {
            return d_area;
        }

        /// <summary>
        /// Set the Area for this NamedArea.
        /// </summary>
        /// <param name="area">
        /// ComponentArea object describing a new target area for the NamedArea..
        /// </param>
        public void SetArea(ComponentArea area)
        {
            d_area = area;
        }

        /// <summary>
        /// Writes an xml representation of this NamedArea to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            xml_stream.OpenTag("NamedArea")
                      .Attribute("name", d_name);
            d_area.WriteXMLToStream(xml_stream);
            xml_stream.CloseTag();
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            return d_area.HandleFontRenderSizeChange(window, font);
        }

        private String d_name;
        private ComponentArea d_area = new ComponentArea();
    }
}