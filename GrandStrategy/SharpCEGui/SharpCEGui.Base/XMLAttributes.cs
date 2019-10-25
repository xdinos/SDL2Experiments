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
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class representing a block of attributes associated with an XML element.
    /// </summary>
    public class XMLAttributes
    {
        /// <summary>
        /// XMLAttributes constructor.
        /// </summary>
        public XMLAttributes()
        {
        }

        /*!
        \brief
            XMLAttributes Destructor
            */
        // TODO: virtual ~XMLAttributes(void);

        /// <summary>
        /// Adds an attribute to the attribute block.  If the attribute value already exists, it is replaced with
        /// the new value.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute to be added.
        /// </param>
        /// <param name="attrValue">
        /// String object holding a string representation of the attribute value.
        /// </param>
        public void Add(string attrName, string attrValue)
        {
            d_attrs[attrName] = attrValue;
        }

        /// <summary>
        /// Removes an attribute from the attribute block.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute to be removed.
        /// </param>
        public void Remove(string attrName)
        {
            d_attrs.Remove(attrName);
        }

        /// <summary>
        /// Return whether the named attribute exists within the attribute block.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute to be checked.
        /// </param>
        /// <returns>
        /// - true if an attribute with the name \a attrName is present in the attribute block.
        /// - false if no attribute named \a attrName is present in the attribute block.
        /// </returns>
        public bool Exists(string attrName)
        {
            return d_attrs.ContainsKey(attrName);
        }

        /// <summary>
        /// Return the number of attributes in the attribute block.
        /// </summary>
        /// <returns>
        /// value specifying the number of attributes in this attribute block.
        /// </returns>
        public int GetCount()
        {
            return d_attrs.Count;
        }

        /*!
        \brief
            Return the name of an attribute based upon its index within the attribute block.

        \note
            Nothing is specified about the order of elements within the attribute block.  Elements
            may not, for example, appear in the order they were specified in the XML file.

        \param index
            zero based index of the attribute whos name is to be returned.

        \return
            String object holding the name of the attribute at the requested index.

        \exception IllegalRequestException  thrown if \a index is out of range for this attribute block.
        */

        public string GetName(int index)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return the value string of an attribute based upon its index within the attribute block.

        \note
            Nothing is specified about the order of elements within the attribute block.  Elements
            may not, for example, appear in the order they were specified in the XML file.
        
        \param index
            zero based index of the attribute whos value string is to be returned.

        \return
            String object holding the string value of the attribute at the requested index.

        \exception IllegalRequestException  thrown if \a index is out of range for this attribute block.
        */

        public string GetValue(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the value string for attribute \a attrName.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute whos value string is to be returned
        /// </param>
        /// <returns>
        /// String object hilding the value string for attribute \a attrName.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// thrown if no attribute named \a attrName is present in the attribute block.
        /// </exception>
        public string GetValue(string attrName)
        {
            string value;
            if (d_attrs.TryGetValue(attrName, out value))
                return value;

            throw new UnknownObjectException("no value exists for an attribute named '" + attrName + "'.");
        }

        /// <summary>
        /// Return the value of attribute \a attrName as a string.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute whos value is to be returned.
        /// </param>
        /// <param name="def">
        /// String object holding the default value to be returned if \a attrName does not exist in the attribute block.
        /// For some parsers, defaults can be gotten from schemas and such like, though for others this may not be desired
        /// or possible, so this parameter is used to ensure a default is available in the abscence of other mechanisms.
        /// </param>
        /// <returns>
        /// String object containing the value of attribute \a attrName if present, or \a def if not.
        /// </returns>
        public string GetValueAsString(string attrName, string def = "")
        {
            return (Exists(attrName)) ? GetValue(attrName) : def;
        }

        /// <summary>
        /// Return the value of attribute \a attrName as a boolean value.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute whos value is to be returned.
        /// </param>
        /// <param name="def">
        /// bool value specifying the default value to be returned if \a attrName does not exist in the attribute block.
        /// For some parsers, defaults can be gotten from schemas and such like, though for others this may not be desired
        /// or possible, so this parameter is used to ensure a default is available in the abscence of other mechanisms.
        /// </param>
        /// <returns>
        /// bool value equal to the value of attribute \a attrName if present, or \a def if not.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if the attribute value string coul dnot be converted to the requested type.
        /// </exception>
        public bool GetValueAsBool(string attrName, bool def = false)
        {
            return !Exists(attrName)
                       ? def
                       : Convert.ToBoolean(GetValue(attrName), global::System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Return the value of attribute \a attrName as a integer value.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute whos value is to be returned.
        /// </param>
        /// <param name="def">
        /// integer value specifying the default value to be returned if \a attrName does not exist in the attribute block.
        /// For some parsers, defaults can be gotten from schemas and such like, though for others this may not be desired
        /// or possible, so this parameter is used to ensure a default is available in the abscence of other mechanisms.</param>
        /// <returns>
        /// integer value equal to the value of attribute \a attrName if present, or \a def if not.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if the attribute value string coul dnot be converted to the requested type.
        /// </exception>
        public int GetValueAsInteger(string attrName, int def = 0)
        {
            if (!Exists(attrName))
                return def;

            return Convert.ToInt32(GetValue(attrName), global::System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Return the value of attribute \a attrName as a floating point value.
        /// </summary>
        /// <param name="attrName">
        /// String object holding the name of the attribute whos value is to be returned.
        /// </param>
        /// <param name="def">
        /// float value specifying the default value to be returned if \a attrName does not exist in the attribute block.
        /// For some parsers, defaults can be gotten from schemas and such like, though for others this may not be desired
        /// or possible, so this parameter is used to ensure a default is available in the abscence of other mechanisms.
        /// </param>
        /// <returns>
        /// float value equal to the value of attribute \a attrName if present, or \a def if not.
        /// </returns>
        /// \exception IllegalRequestException  thrown if the attribute value string coul dnot be converted to the requested type.
        public float GetValueAsFloat(string attrName, float def = 0.0f)
        {
            return !Exists(attrName)
                       ? def
                       : Convert.ToSingle(GetValue(attrName), global::System.Globalization.CultureInfo.InvariantCulture);
        }

        #region Fields

        private readonly Dictionary<string, string> d_attrs = new Dictionary<string, string>();

        #endregion
    }
}