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
    /// 
    /// </summary>
    public interface IPropertyDefinition
    {
        string GetPropertyName();
        void SetPropertyName(string name);
        string GetInitialValue();
        void SetInitialValue(string value);
        string GetHelpString();
        void SetHelpString(string help);
        bool IsRedrawOnWrite();
        void SetRedrawOnWrite(bool value);
        bool IsLayoutOnWrite();
        void SetLayoutOnWrite(bool value);
        string GetEventFiredOnWrite();
        void SetEventFiredOnWrite(string eventName);
        string GetEventNamespace();
        void SetEventNamespace(string eventNamespace);

        /// <summary>
        /// Writes an xml representation of the PropertyDefinitionBase based
        /// object to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// XMLSerializer where xml data should be output.
        /// </param>
        void WriteDefinitionXMLToStream(XMLSerializer xml_stream);

        void WriteDefinitionXMLElementType(XMLSerializer xml_stream);
        void WriteDefinitionXMLAttributes(XMLSerializer xml_stream);
    }

    /// <summary>
    /// common base class used for types representing a new property to be
    /// available on all widgets that use the WidgetLook that the property
    /// definition is a part of.
    /// </summary>
    public abstract class PropertyDefinitionBase : IPropertyDefinition
    {
        protected PropertyDefinitionBase(string name, string help,
                                         string initialValue, bool redrawOnWrite, bool layoutOnWrite,
                                         string fireEvent, string eventNamespace)
        {
            d_propertyName = name;
            d_initialValue = initialValue;
            d_helpString = help;
            d_writeCausesRedraw = redrawOnWrite;
            d_writeCausesLayout = layoutOnWrite;
            d_eventFiredOnWrite = fireEvent;
            d_eventNamespace = eventNamespace;
        }

        // TODO: virtual ~PropertyDefinitionBase() { }

        public string GetPropertyName()
        {
            return d_propertyName;
        }

        public void SetPropertyName(string name)
        {
            d_propertyName = name;
        }

        public string GetInitialValue()
        {
            return d_initialValue;
        }

        public void SetInitialValue(string value)
        {
            d_initialValue = value;
        }

        public string GetHelpString()
        {
            return d_helpString;
        }

        public void SetHelpString(string help)
        {
            d_helpString = help;
        }

        public bool IsRedrawOnWrite()
        {
            return d_writeCausesRedraw;
        }

        public void SetRedrawOnWrite(bool value)
        {
            d_writeCausesRedraw = value;
        }

        public bool IsLayoutOnWrite()
        {
            return d_writeCausesLayout;
        }

        public void SetLayoutOnWrite(bool value)
        {
            d_writeCausesLayout = value;
        }

        public string GetEventFiredOnWrite()
        {
            return d_eventFiredOnWrite;
        }

        public void SetEventFiredOnWrite(string eventName)
        {
            d_eventFiredOnWrite = eventName;
        }

        public string GetEventNamespace()
        {
            return d_eventNamespace;
        }

        public void SetEventNamespace(string eventNamespace)
        {
            d_eventNamespace = eventNamespace;
        }
        
        /// <summary>
        /// Writes an xml representation of the PropertyDefinitionBase based
        /// object to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// XMLSerializer where xml data should be output.
        /// </param>
        public virtual void WriteDefinitionXMLToStream(XMLSerializer xml_stream)
        {
            WriteDefinitionXMLElementType(xml_stream);
            WriteDefinitionXMLAttributes(xml_stream);
            xml_stream.CloseTag();
        }

        /*!
        \brief
            Write out the text of the XML element type.  Note that you should
            not write the opening '<' character, nor any other information such
            as attributes in this function.  The writeExtraAttributes function
            can be used for writing attributes.

        \param xml_stream
            XMLSerializer where xml data should be output.
        */
        public abstract void WriteDefinitionXMLElementType(XMLSerializer xml_stream);

        /*!
        \brief
            Write out any xml attributes added in a sub-class.  Note that you
            should not write the closing '/>' character sequence, nor any other
            information in this function.  You should always call the base class
            implementation of this function when overriding.

        \param xml_stream
            XMLSerializer where xml data should be output.
        */

        public virtual void WriteDefinitionXMLAttributes(XMLSerializer xml_stream)
        {
            xml_stream.Attribute("name", d_propertyName);

            if (!String.IsNullOrEmpty(d_initialValue))
                xml_stream.Attribute("initialValue", d_initialValue);

            if (!String.IsNullOrEmpty(d_helpString))
                xml_stream.Attribute("help", d_helpString);

            if (d_writeCausesRedraw)
                xml_stream.Attribute("redrawOnWrite", "true");

            if (d_writeCausesLayout)
                xml_stream.Attribute("layoutOnWrite", "true");

            if (!String.IsNullOrEmpty(d_eventFiredOnWrite))
                xml_stream.Attribute("fireEvent", d_eventFiredOnWrite);
        }

        protected string d_propertyName;
        protected string d_initialValue;
        protected string d_helpString;
        protected bool d_writeCausesRedraw;
        protected bool d_writeCausesLayout;
        protected string d_eventFiredOnWrite;
        protected string d_eventNamespace;
    }
}