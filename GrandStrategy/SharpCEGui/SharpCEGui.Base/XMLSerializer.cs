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
using System.IO;
using System.Text;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class used to create XML Document. 
    /// 
    /// This class hides the complexity of formatting valid XML files. The
    /// class provides automatic substitution of entities, XML indenting
    /// in respect of the spaces. It does not contains any codes specific
    /// to CEGUI taking appart the CEGUI::String class. The following
    /// example show the code needed to exports parts of an XML document
    /// similar to what can be found in a layout.
    /// 
    /// <code>
    /// // Create an encoder that outputs its result on standard output 
    /// XMLSerializer xml(std::cout, 4);
    /// xml.openTag("Window")
    ///     .attribute("Type", "TaharezLook/StaticText")
    ///     .attribute("Name", "Test")
    ///         .openTag("Property")
    ///             .attribute("Name", "Text")
    ///             .text("This is the static text to be displayed")
    ///         .closeTag()
    ///         .openTag("Window")
    ///             .attribute("Name", "Button")
    ///             .attribute("Type", "Vanilla/Button")
    ///                 .openTag("Property")
    ///                     .attribute("Name", "Text")
    ///                     .attribute("Value", "Push me")
    ///                 .closeTag()
    ///         .closeTag()
    ///     .closeTag();
    /// 
    /// if (xml)
    /// {
    ///     Console.WriteLine("XML Exported successfully");
    /// }
    /// 
    /// return null;
    /// </code>
    /// </summary>
    public class XMLSerializer
    {
        /// <summary>
        /// XMLSerializer constructor.
        /// </summary>
        /// <param name="out">
        /// The stream to use to export the result.
        /// </param>
        /// <param name="indentSpace">
        /// The indentation level (0 to disable indentation).
        /// </param>
        public XMLSerializer(StreamWriter @out, int indentSpace = 4)
        {
            d_error=false;
            d_tagCount = 0;
            d_depth = 0;
            d_indentSpace = indentSpace;
            d_needClose = false;
            d_lastIsText = false;
            d_stream = @out;

            d_stream.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            d_error = d_stream == null;
        }

        // TODO: XMLSerializer destructor
        //virtual ~XMLSerializer(void)
        //{
        //    if (!d_error || !d_tagStack.empty())
        //    {
        //        d_stream << std::endl;
        //    }
        //}

        /// <summary>
        /// Start a new tag in the xml document.
        /// </summary>
        /// <param name="name">
        /// The name of the tag.
        /// </param>
        /// <returns>
        /// A reference to the current object for chaining operation.
        /// </returns>
        public XMLSerializer OpenTag(string name)
        {
            if (! d_error)
            {
                ++d_tagCount;
                if (d_needClose)
                {
                    d_stream.Write(">");
                }
                if (!d_lastIsText)
                {
                    d_stream.WriteLine();
                    IndentLine();
                }
                d_stream.Write(String.Format("<{0} ", name));
                d_tagStack.Push(name);
                ++d_depth;
                d_needClose = true;
                d_lastIsText = false;
                d_error = d_stream == null;
            }

            return this;
        }

        /// <summary>
        /// Close the current tag. 
        /// </summary>
        /// <returns>
        /// A reference to the current object for chaining operation
        /// </returns>
        public XMLSerializer CloseTag()
        {
            var back = d_tagStack.Pop();
            if (!d_error)
            {
                --d_depth;
                if (d_needClose)
                {
                    d_stream.Write("/>");
                }
                else if (!d_lastIsText)
                {
                    d_stream.WriteLine();
                    IndentLine();
                    d_stream.Write(String.Format("</{0}>", back));
                }
                else
                {
                    d_stream.Write(String.Format("</{0}>", back));
                }
                d_lastIsText = false;
                d_needClose = false;
                //d_tagStack.pop_back();
                d_error = d_stream == null;
            }
            return this;
        }

        /// <summary>
        /// After an opening tag you can populate attribute list with this function.
        /// </summary>
        /// <param name="name">
        /// The name of the attribute.
        /// </param>
        /// <param name="value">
        /// The value of the attribute.
        /// </param>
        /// <returns>
        /// A reference to the current object for chaining operation
        /// </returns>
        public XMLSerializer Attribute(string name, string value)
        {
            if (!d_needClose)
            {
                d_error = true;
            }
            if (!d_error)
            {
                d_stream.Write(String.Format("{0}=\"{1}\" ", name, ConvertEntityInAttribute(value)));
                d_lastIsText = false;
                d_error = d_stream == null;
            }
            return this;
        }

        /// <summary>
        /// Create a text node.
        /// </summary>
        /// <param name="text">
        /// text the content of the node.
        /// </param>
        /// <returns>
        /// A reference to the current object for chaining operation.
        /// </returns>
        public XMLSerializer Text(string text)
        {
            if (!d_error)
            {
                if (d_needClose)
                {
                    d_stream.Write(">");
                    d_needClose = false;
                }
                d_stream.Write(ConvertEntityInText(text));
                d_lastIsText = true;
                d_error = d_stream == null;
            }
            return this;
        }

        /// <summary>
        /// report the nimber of tags created in the document 
        /// </summary>
        /// <returns>
        /// return the number of tag created in the document 
        /// </returns>
        public uint GetTagCount()
        {
            return d_tagCount;
        }
        
        // TODO:...
        ///*!
        //\brief Check wether the XML Serializer status is valid 
        
        //\return 
        //    True if all previous operations where successfull 
        //*/
        //operator bool () const
        //{
        //    return false == d_error;
        //}
        ///*!
        //\brief Check wether the XML Serializer status is invalid 
        
        //\return 
        //    True if one operations failed
        //*/ 
        //bool operator!() const
        //{
        //    return false != d_error;
        //}
        
        /// <summary>
        /// put padding in the stream before line data 
        /// </summary>
        private void IndentLine()
        {
            var spaceCount = d_depth * d_indentSpace;
            // There's for sure a best way to do this but it works 
            for (var i = 0; i < spaceCount; ++i)
            {
                d_stream.Write(" ");
            }  
        }

        /// <summary>
        /// convert special char to there corresponding entity in text data. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ConvertEntityInText(string text)
        {
            var res = new StringBuilder(text.Length*2);
            //res.reserve(text.size()*2);
            //const String::const_iterator iterEnd = text.end();
            //for (String::const_iterator iter = text.begin(); iter != iterEnd ; ++iter)
            foreach (var iter in text)
            {
                switch (iter)
                {
                    case '<':
                        res.Append("&lt;");
                        break;

                    case '>':
                        res.Append("&gt;");
                        break;

                    case '&':
                        res.Append("&amp;");
                        break;

                    case '\'':
                        res .Append("&apos;");
                        break;

                    case '"':
                        res .Append("&quot;");
                        break;

                    default:
                        res .Append(iter);
                        break;
                }
            }
            return res.ToString();
        }

        /// <summary>
        /// convert special char into entities including line ending for use in attributes.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        private static string ConvertEntityInAttribute(string attributeValue)
        {
            //throw new NotImplementedException();
            // Reserve a lot of space 
            var res = new StringBuilder(attributeValue.Length*2);
            //res.reserve(attributeValue.size() * 2);
            //const String::const_iterator iterEnd = attributeValue.end();
            foreach (var iter in attributeValue)
            //for (String::const_iterator iter = attributeValue.begin(); iter != iterEnd; ++iter)
            {
                switch (iter)
                {
                    case '<':
                        res.Append("&lt;");
                        break;

                    case '>':
                        res.Append("&gt;");
                        break;

                    case '&':
                        res.Append("&amp;");
                        break;

                    case '\'':
                        res.Append("&apos;");
                        break;

                    case '"':
                        res.Append("&quot;");
                        break;

                    case '\n':
                        res.Append("\\n");
                        break;

                    default:
                        res.Append(iter);
                        break;
                }
            }

            return res.ToString();
        }
        
        // TODO: ...
        //// Disabled operation 
        //XMLSerializer(const XMLSerializer& obj);
        //// Disabled operation 
        //XMLSerializer& operator=(const XMLSerializer& obj);  

        #region Fields

        /// <summary>
        /// Store the status of the serializer.
        /// </summary>
        private bool d_error;

        /// <summary>
        /// Return the number of tag in the document.
        /// </summary>
        private uint d_tagCount;
        
        /// <summary>
        /// Store the current depth for indentation purpose.
        /// </summary>
        private int d_depth;
        
        /// <summary>
        /// Store the number of space use for indenting.
        /// </summary>
        private int d_indentSpace;

        /// <summary>
        /// Store whether the next operation need to close the tag or not.
        /// </summary>
        private bool d_needClose;
        
        /// <summary>
        /// Store whether the last operation was a text node or not.
        /// </summary>
        private bool d_lastIsText;
        
        /// <summary>
        /// A reference to the stream object use.
        /// </summary>
        private StreamWriter d_stream;
        //OutStream& d_stream; //!< A reference to the stream object use

        //std::vector<String
        //    CEGUI_VECTOR_ALLOC(String)> d_tagStack; //!< Store the tag stack for correct closing of the tags. 

        /// <summary>
        /// Store the tag stack for correct closing of the tags.
        /// </summary>
        private Stack<string> d_tagStack=new Stack<string>();

        #endregion
    }
}