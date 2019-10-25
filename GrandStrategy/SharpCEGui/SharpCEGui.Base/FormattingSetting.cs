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
    public class FormattingSetting<T> : IEquatable<FormattingSetting<T>>
    {
        public FormattingSetting()
        {
            Value = default(T); //FalagardXMLHelper<T>.fromString("");
        }
        
        public FormattingSetting(string propertyName)
        {
            Value = default(T); //FalagardXMLHelper<T>.fromString("");
            PropertySource = propertyName;
        }

        public FormattingSetting(T val)
        {
            Value = val;
        }

        public T Get(Window wnd)
        {
            if (String.IsNullOrEmpty(PropertySource))
                return Value;

            return PropertyHelper.FromString<T>(wnd.GetProperty(PropertySource));
        }

        public void Set(T val)
        {
            Value = val;
            PropertySource = String.Empty;
        }

        public void SetPropertySource(string propertyName)
        {
            PropertySource = propertyName;
        }

        public bool IsFetchedFromProperty()
        {
            return !String.IsNullOrEmpty(PropertySource);
        }

        public void WriteXmlToStream(XMLSerializer xmlStream)
        {
            WriteXmlTagToStream(xmlStream);
            WriteXmlAttributesToStream(xmlStream);
            xmlStream.CloseTag();
        }

        public virtual void WriteXmlTagToStream(XMLSerializer xmlStream)
        {
            // This does nothing and needs to be specialised or overridden
        }

        public virtual void WriteXmlAttributesToStream(XMLSerializer xmlStream)
        {
            // This does nothing and needs to be specialised or overridden
        }

        public static bool operator==(FormattingSetting<T> lhs,FormattingSetting<T> rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            return !ReferenceEquals(lhs, null) && lhs.Equals(rhs);
        }

        public static bool operator!=(FormattingSetting<T> lhs,FormattingSetting<T> rhs)
        {
            return !(lhs == rhs);
        }

        protected internal T Value;
        protected internal string PropertySource;

        public bool Equals(FormattingSetting<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Value, Value) && 
                Equals(other.PropertySource, PropertySource);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (FormattingSetting<T>) && 
                Equals((FormattingSetting<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (PropertySource != null ? PropertySource.GetHashCode() : 0);
            }
        }
    }

    internal static class FormattingSetting
    {
        public static void WriteXmlTagToStream(this FormattingSetting<VerticalFormatting> s, XMLSerializer xmlStream)
        {
            if (String.IsNullOrEmpty(s.PropertySource))
                xmlStream.OpenTag("VertFormat");
            else
                xmlStream.OpenTag("VertFormatProperty");
        }

        public static void WriteXmlAttributesToStream(this FormattingSetting<VerticalFormatting> s, XMLSerializer xmlStream)
        {
            if (String.IsNullOrEmpty(s.PropertySource))
                xmlStream.Attribute("type", s.Value.ToString());
            else
                xmlStream.Attribute("name", s.PropertySource);
        }
    }
}