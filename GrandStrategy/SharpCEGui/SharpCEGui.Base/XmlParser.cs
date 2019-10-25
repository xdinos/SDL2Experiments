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

using System.IO;

namespace SharpCEGui.Base
{
    /// <summary>
    /// This is an abstract class that is used by CEGUI to interface with XML parser libraries.
    /// </summary>
    public abstract class XMLParser
    {
        /// <summary>
        /// XMLParser base class constructor.
        /// </summary>
        protected XMLParser()
        {
            d_identifierString = "Unknown XML parser (vendor did not set the ID string!)";
            d_initialised = false;
        }

        //XMLParser base class destructor.
        // TODO: virtual ~XMLParser() { }

        /// <summary>
        /// Initialises the XMLParser module ready for use.
        /// 
        /// Note that this calls the protected abstract method 'InitialiseImpl', which should
        /// be provided in your implementation to perform any required initialisation.
        /// </summary>
        /// <returns>
        /// - true if the module initialised successfully.
        /// - false if the module initialisation failed.
        /// </returns>
        public bool Initialise()
        {
            // do this to ensure only one initialise call is made
            if (!d_initialised)
            {
                d_initialised = InitialiseImpl();
            }

            return d_initialised;
        }

        /// <summary>
        /// Cleans up the XMLParser module after use.
        /// 
        /// Note that this calls the protected abstract method 'CleanupImpl', which should
        /// be provided in your implementation to perform any required cleanup.
        /// </summary>
        public void Cleanup()
        {
            if (d_initialised)
            {
                CleanupImpl();
                d_initialised = false;
            }
        }

        /// <summary>
        /// abstract method which initiates parsing of an XML.
        /// </summary>
        /// <param name="handler">
        /// XMLHandler based object which will process the XML elements.
        /// </param>
        /// <param name="source">
        /// RawDataContainer containing the data to parse
        /// </param>
        /// <param name="schemaName">
        /// String object holding the name of the XML schema file to use for validating the XML.
        /// Note that whether this is used or not is dependant upon the XMLParser in use.
        /// </param>
        /// <param name="allowXmlValidation">
        ///  A boolean object used for disallowing xml validation for a single call, 
        /// defaulting to "true" to allow validation.
        /// Only needed if xml validation should be disallowed once.
        /// </param>
        public abstract void ParseXml(XmlHandler handler, RawDataContainer source, string schemaName, bool allowXmlValidation = true);

        /// <summary>
        /// convenience method which initiates parsing of an XML file.
        /// </summary>
        /// <param name="handler">
        /// XMLHandler based object which will process the XML elements.
        /// </param>
        /// <param name="filename">
        /// String object holding the filename of the XML file to be parsed.
        /// </param>
        /// <param name="schemaName">
        /// String object holding the name of the XML schema file to use for validating the XML.
        /// Note that whether this is used or not is dependant upon the XMLParser in use.
        /// </param>
        /// <param name="resourceGroup">
        /// String object holding the resource group identifier which will be passed to the
        /// ResourceProvider when loading the XML and schema files.
        /// </param>
        /// <param name="allowXmlValidation">
        /// A boolean object used for disallowing xml validation for a single call, 
        /// defaulting to "true" to allow validation.
        /// Only needed if xml validation should be disallowed once.
        /// </param>
        public virtual void ParseXmlFile(XmlHandler handler, string filename, string schemaName, string resourceGroup, bool allowXmlValidation = true)
        {
            // Acquire resource using CEGUI ResourceProvider
            var rawXmlData = new RawDataContainer();
            System.GetSingleton().GetResourceProvider()
                .LoadRawDataContainer(filename, rawXmlData, resourceGroup);

            try
            {
                // The actual parsing action (this is overridden and depends on the specific parser)
                ParseXml(handler, rawXmlData, schemaName);
            }
            catch (Exception)
            {
                // hint the related file name in the log
                System.GetSingleton().Logger
                      .LogEvent("The last thrown exception was related to XML file '" +
                                filename + "' from resource group '" + resourceGroup + "'.", LoggingLevel.Errors);

                throw;
            }
            finally
            {
                // Release resource
                System.GetSingleton().GetResourceProvider()
                      .UnloadRawDataContainer(rawXmlData);
            }
        }

        /// <summary>
        /// convenience method which initiates parsing of an XML source from string.
        /// </summary>
        /// <param name="handler">
        /// XMLHandler based object which will process the XML elements.
        /// </param>
        /// <param name="source">
        /// The XML source passed as a String
        /// </param>
        /// <param name="schemaName">
        /// String object holding the name of the XML schema file to use for validating the XML.
        /// Note that whether this is used or not is dependant upon the XMLParser in use.
        /// </param>
        public virtual void ParseXmlString(XmlHandler handler, string source, string schemaName)
        {
            // Put the source string into a RawDataContainer
            using (var rawXMLData = new RawDataContainer())
            {
                rawXMLData.SetData(new MemoryStream(global::System.Text.Encoding.ASCII.GetBytes(source)));
                
                try
                {
                    // The actual parsing action (this is overridden and depends on the specific parser)
                    ParseXml(handler, rawXMLData, schemaName);
                }
                catch
                {
                    //// make sure we don't allow rawXMLData to release String owned data no matter what!
                    //rawXMLData.SetData((byte[]) null);
                    //rawXMLData.SetSize(0);
                    //throw;
                }

                //// !!! We must not allow DataContainer to delete String owned data,
                ////     therefore, we set it's data to 0 to avoid double-deletion
                //rawXMLData.SetData((byte[]) null);
                //rawXMLData.SetSize(0);
            }
        }

        /// <summary>
        /// Return identification string for the XML parser module.  If the internal id string has not been
        /// set by the XML parser module creator, a generic string of "Unknown XML parser" will be returned.
        /// </summary>
        /// <returns>
        /// String object holding a string that identifies the XML parser in use.
        /// </returns>
        public string GetIdentifierString()
        {
            return d_identifierString;
        }

        /// <summary>
        /// abstract method which initialises the XMLParser ready for use.
        /// </summary>
        /// <returns>
        /// - true if the module initialised successfully.
        /// - false if the module initialisation failed.
        /// </returns>
        protected abstract bool InitialiseImpl();

        /// <summary>
        /// abstract method which cleans up the XMLParser after use.
        /// </summary>
        protected abstract void CleanupImpl();

        #region Fields

        /// <summary>
        /// String that holds some id information about the module.
        /// </summary>
        protected string d_identifierString;

        /// <summary>
        /// true if the parser module has been initialised,
        /// </summary>
        private bool d_initialised;

        #endregion
    }
}