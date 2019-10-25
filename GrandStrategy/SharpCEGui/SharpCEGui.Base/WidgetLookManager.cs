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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Manager class that gives top-level access to widget data based 
    /// "look and feel" specifications loaded into the system.
    /// </summary>
    public class WidgetLookManager
    {
        #region Implementation of Singleton
        private readonly static Lazy<WidgetLookManager> Instance = new Lazy<WidgetLookManager>(()=>new WidgetLookManager());
        public static WidgetLookManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        /*!
       \brief
           Constructor.
       */
        private WidgetLookManager()
        {
            System.GetSingleton().Logger.
                LogEvent(String.Format("CEGUI::WidgetLookManager singleton created. ({0:X8})", GetHashCode()));
        }

        /*!
        \brief
            Destructor
        */
        // TODO: ~WidgetLookManager();

        /*!
        \brief
            Parses a file containing window look & feel specifications (in the form of XML).

        \note
            If the new file contains specifications for widget types that are already specified, it is not an error;
            the previous definitions are overwritten by the new data.  An entry will appear in the log each time any
            look & feel component is overwritten.

        \param source
            RawDataContainer containing the source code that will be parsed

        \param resourceGroup
            Resource group identifier to pass to the resource provider when loading the file.

        \return
            Nothing.

        \exception FileIOException             thrown if there was some problem accessing or parsing the file \a filename
        \exception InvalidRequestException     thrown if an invalid filename was provided.
        */
        public void ParseLookNFeelSpecificationFromContainer(RawDataContainer source)
        {
            throw new NotImplementedException();
        }
        
        /*!
        \see WidgetLookManager::parseLookNFeelSpecificationFromContainer
        */

        public void ParseLookNFeelSpecificationFromFile(string filename, string resourceGroup = "")
        {
            // valid filenames are required!
            if (String.IsNullOrEmpty(filename))
                throw new InvalidRequestException("Filename supplied for look & feel file must be valid");

            // create handler object
            var handler = new Falagard_xmlHandler(this);

            // perform parse of XML data
            try
            {
                System.GetSingleton().GetXMLParser()
                      .ParseXmlFile(handler, filename, FalagardSchemaName, String.IsNullOrEmpty(resourceGroup)
                                                                               ? d_defaultResourceGroup
                                                                               : resourceGroup);
            }
            catch
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          String.Format(
                              "WidgetLookManager::parseLookNFeelSpecification - loading of look and feel data from file '{0}' has failed.",
                              filename),
                          LoggingLevel.Errors);
                throw;
            }
        }

        /*!
        \see WidgetLookManager::parseLookNFeelSpecificationFromContainer
        */
        public void ParseLookNFeelSpecificationFromString(string source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether a WidgetLookFeel has been created with the specified name.
        /// </summary>
        /// <param name="widget">
        /// String object holding the name of a widget look to test for.
        /// </param>
        /// <returns>
        /// - true if a WidgetLookFeel named \a widget is available.
        /// - false if so such WidgetLookFeel is currently available.
        /// </returns>
        public bool IsWidgetLookAvailable(string widget)
        {
            return d_widgetLooks.ContainsKey(widget);
        }

        /// <summary>
        /// Return a const reference to a WidgetLookFeel object which has the specified name.
        /// </summary>
        /// <param name="widget">
        /// String object holding the name of a widget look that is to be returned.
        /// </param>
        /// <returns>
        /// const reference to the requested WidgetLookFeel object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// thrown if no WidgetLookFeel is available with the requested name.
        /// </exception>
        public WidgetLookFeel GetWidgetLook(string widget)
        {
            if (d_widgetLooks.ContainsKey(widget))
            {
                return d_widgetLooks[widget];
            }

            throw new UnknownObjectException("WidgetLook '" + widget + "' does not exist.");
        }


        /*!
        \brief
            Erase the WidgetLookFeel that has the specified name.

        \param widget
            String object holding the name of a widget look to be erased.  If no such WidgetLookFeel exists, nothing
            happens.

        \return
            Nothing.
        */
        public void EraseWidgetLook(string widget)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// erase all defined WidgetLookFeel.
        /// </summary>
        public void EraseAllWidgetLooks()
        {
            d_widgetLooks.Clear();
        }

        /*!
        \brief
            Add the given WidgetLookFeel.

        \note
            If the WidgetLookFeel specification uses a name that already exists within the system, it is not an error;
            the previous definition is overwritten by the new data.  An entry will appear in the log each time any
            look & feel component is overwritten.

        \param look
            

        \return
            Nothing.
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="look">
        /// WidgetLookFeel object to be added to the system.  NB: The WidgetLookFeel is copied, no change of ownership of the
        /// input object occurrs.
        /// </param>
        /// <remarks>
        /// If the WidgetLookFeel specification uses a name that already exists within the system, it is not an error;
        /// the previous definition is overwritten by the new data.  An entry will appear in the log each time any
        /// look & feel component is overwritten.
        /// </remarks>
        public void AddWidgetLook(WidgetLookFeel look)
        {
            if (IsWidgetLookAvailable(look.GetName()))
            {
                System.GetSingleton().Logger
                      .LogEvent("WidgetLookManager.AddWidgetLook - Widget look and feel '" + look.GetName() +
                                "' already exists.  Replacing previous definition.");
            }
            
            d_widgetLooks[look.GetName()] = look;
        }


        /*!
        \brief
            Writes a complete Widge Look to a stream.  Note that xml file header and
            falagard opening/closing tags will also be written.

        \param name
            String holding the name of the widget look to be output to the stream.

        \param out_stream
            OutStream where XML data should be sent.
        */
        public void WriteWidgetLookToStream(String name, StreamWriter out_stream)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Writes a series of complete Widge Look objects to a stream.  Note that xml file header and
            falagard opening/closing tags will also be written.

            The \a prefix specifies a name prefix common to all widget looks to be written, you could
            specify this as "TaharezLook/" and then any defined widget look starting with that prefix, such
            as "TaharezLook/Button" and "TaharezLook/Listbox" will be written to the stream.

        \param prefix
            String holding the widget look name prefix, which will be used when searching for the widget looks
            to be output to the stream.

        \param out_stream
            OutStream where XML data should be sent.
        */
        public void WriteWidgetLookSeriesToStream(string prefix, StreamWriter out_stream)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Returns the default resource group currently set for LookNFeels.

        \return
            String describing the default resource group identifier that will be
            used when loading LookNFeel data.
        */
        public static string GetDefaultResourceGroup()
            { return d_defaultResourceGroup; }

        /*!
        \brief
            Sets the default resource group to be used when loading LookNFeel data

        \param resourceGroup
            String describing the default resource group identifier to be used.

        \return
            Nothing.
        */
        public static void SetDefaultResourceGroup(String resourceGroup)
            { d_defaultResourceGroup = resourceGroup; }


        private const string FalagardSchemaName = @"falagard.xsd";     //!< Name of schema file used for XML validation.

        //typedef std::map<String, WidgetLookFeel, StringFastLessCompare> WidgetLookList;
        private /*WidgetLookList*/ Dictionary<string, WidgetLookFeel>  d_widgetLooks = new Dictionary<string, WidgetLookFeel>();

        private static string d_defaultResourceGroup;   //!< holds default resource group
    
        // TODO: ... public:
        //typedef ConstMapIterator<WidgetLookList> WidgetLookIterator;
        //WidgetLookIterator getWidgetLookIterator() const;
    }
}