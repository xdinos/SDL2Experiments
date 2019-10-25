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
    public interface IXmlHandler<T>
    {
        string GetObjectName();
        T GetObject();
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class XmlHandler
    {
        /*!
        \brief
            XMLHandler base class destructor.
         */
        // TODO: virtual ~XMLHandler(void);

        //public abstract string GetObjectName();

        //public abstract T GetObject();

        /// <summary>
        /// Retrieves the schema file name to use with resources handled by this handler
        /// </summary>
        /// <returns></returns>
        public virtual string GetSchemaName()
        {
            // by default, don't use XML schemas
            return String.Empty;
        }

        /// <summary>
        /// Retrieves the default resource group to be used when handling files
        /// </summary>
        /// <returns></returns>
        public abstract string GetDefaultResourceGroup();

        /// <summary>
        /// Takes given RawDataContainer containing XML and handles it
        /// 
        /// This is basically a convenience function used by NamedXMLResourceManager
        /// <internal>No need for this to be virtual</internal>
        /// </summary>
        /// <param name="source"></param>
        public void HandleContainer(RawDataContainer source)
        {
            System.GetSingleton().GetXMLParser()
                .ParseXml(this, source, GetSchemaName());
        }

        /// <summary>
        /// Takes given file containing XML and handles it
        /// 
        /// This is basically a convenience function used by NamedXMLResourceManager
        /// <internal>No need for this to be virtual</internal>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="resourceGroup"></param>
        public void HandleFile(string fileName, string resourceGroup)
        {
            System.GetSingleton().GetXMLParser()
                  .ParseXmlFile(this, fileName, GetSchemaName(), String.IsNullOrEmpty(resourceGroup)
                                                                     ? GetDefaultResourceGroup()
                                                                     : resourceGroup);
        }

        /// <summary>
        /// Takes given string containing XML source and handles it
        /// 
        /// This is basically a convenience function used by NamedXMLResourceManager
        /// <internal>No need for this to be virtual</internal>
        /// </summary>
        /// <param name="source"></param>
        public void HandleString(string source)
        {
            System.GetSingleton().GetXMLParser()
                  .ParseXmlString(this, source, GetSchemaName());
        }

        /// <summary>
        /// Method called to notify the handler at the start of each XML element encountered.
        /// </summary>
        /// <param name="element">
        /// String object holding the name of the element that is starting.
        /// </param>
        /// <param name="attributes">
        /// An XMLAttributes object holding the collection of attributes specified for the element.
        /// </param>
        public virtual void ElementStart(string element, XMLAttributes attributes)
        {
            
        }

        /// <summary>
        /// Method called to notify the handler at the end of each XML element encountered.
        /// </summary>
        /// <param name="element">
        /// String object holding the name of the element that is ending.
        /// </param>
        public virtual void ElementEnd(string element)
        {
            
        }

        /// <summary>
        /// Method called to notify text node, several successiv text node are agregated. 
        /// </summary>
        /// <param name="text">
        /// String object holding the content of the text node.
        /// </param>
        public virtual void Text(string text)
        {
            
        }
    }
}