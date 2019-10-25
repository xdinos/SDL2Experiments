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
    /// Handler class used to parse the Font XML files to create Font objects.
    /// </summary>
    public class Font_xmlHandler : XmlHandler, IXmlHandler<Font>
    {
        #region Const Strings

        /// <summary>
        /// Filename of the XML schema used for validating Font files.
        /// </summary>
        public const string FontSchemaName = "Font.xsd";

        /// <summary>
        /// Tag name for Font elements.
        /// </summary>
        public const string FontElement = "Font";

        //! Tag name for Mapping elements.
        public const string MappingElement = "Mapping";
        //! Attribute name that stores the specific font type.
        public const string FontTypeAttribute = "type";
        //! Attribute name that stores the font name.
        public const string FontNameAttribute = "name";
        //! Attribute name that stores the filename of the font source (font / imageset)
        public const string FontFilenameAttribute = "filename";
        //! Attribute name that stores the resource group of the font source.
        public const string FontResourceGroupAttribute = "resourceGroup";
        //! Attribute name that stores the auto-scaled setting.
        public const string FontAutoScaledAttribute = "autoScaled";
        //! Attribute name that stores the horizontal native resolution.
        public const string FontNativeHorzResAttribute = "nativeHorzRes";
        //! Attribute name that stores the vertical native resolution.
        public const string FontNativeVertResAttribute = "nativeVertRes";
        //! Attribute name that stores the line height that we'll report for this font.
        public const string FontLineSpacingAttribute = "lineSpacing";
        //! Attribute name that stores the font point size.
        public const string FontSizeAttribute = "size";
        //! Attribute name that stores the font anti-aliasing setting.
        public const string FontAntiAliasedAttribute = "antiAlias";
        //! Attribute name that stores the codepoint value for a mapping
        public const string MappingCodepointAttribute = "codepoint";
        //! Attribute name that stores the image name for a mapping
        public const string MappingImageAttribute = "image";
        //! Attribute name that stores the horizontal advance value for a mapping.
        public const string MappingHorzAdvanceAttribute = "horzAdvance";
        //! Attribute specifying the datafile version.
        public const string FontVersionAttribute = "version";
        //! Type name of FreeType fonts.
        public const string FontTypeFreeType = "FreeType";
        
        //! Type name of Pixmap fonts.
        public const string FontTypePixmap = "Pixmap";

        //! Type name of BMFont fonts.
        public const string FontTypeFnt = "Fnt";

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public Font_xmlHandler()
        {
            _font = null;
            _objectRead = false;
        }

        //! TODO: Destructor.
        //~Font_xmlHandler()
        //{
        //     if (!d_objectRead)
        //         CEGUI_DELETE_AO d_font;
        //}

        #region Implementation of IXmlHandler<Font>

        /// <summary>
        /// Return string holding the name of the created Font.
        /// </summary>
        /// <returns></returns>
        public string GetObjectName()
        {
            if (_font==null)
                throw  new InvalidRequestException("Attempt to access null object.");

            return _font.GetName();
        }

        /// <summary>
        /// Return reference to the created Font object.
        /// </summary>
        /// <returns></returns>
        public Font GetObject()
        {
            if (_font == null)
                throw new InvalidRequestException("Attempt to access null object.");

            _objectRead = true;
            return _font;
        }

        #endregion

        #region Overrides of XMLHandler<Font>

        public override string GetSchemaName()
        {
            return FontSchemaName;
        }

        public override string GetDefaultResourceGroup()
        {
            return Font.GetDefaultResourceGroup();
        }

        public override void ElementStart(string element, XMLAttributes attributes)
        {
            // handle root Font element
            if (element == FontElement)
                ElementFontStart(attributes);
            // handle a Mapping element
            else if (element == MappingElement)
                ElementMappingStart(attributes);
            // anything else is a non-fatal error.
            else
                System.GetSingleton().Logger.LogEvent("Font_xmlHandler.ElementStart: " +
                                                      "Unknown element encountered: <" + element + ">",
                                                      LoggingLevel.Errors);
        }

        public override void ElementEnd(string element)
        {
            if (element == FontElement)
                ElementFontEnd();
        }

        #endregion

        /// <summary>
        /// handles the opening Font XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementFontStart(XMLAttributes attributes)
        {
            ValidateFontFileVersion(attributes);

            // get type of font being created
            var fontType = attributes.GetValueAsString(FontTypeAttribute);

            // log the start of font creation.
            Logger.LogInsane("Started creation of Font from XML specification:");

            if (fontType == FontTypeFreeType)
                CreateFreeTypeFont(attributes);
            else if (fontType == FontTypePixmap)
                CreatePixmapFont(attributes);
            else if (fontType == FontTypeFnt)
                CreateFntFont(attributes);
            else
                throw new InvalidRequestException("Encountered unknown font type of '" + fontType + "'");
        }

        /// <summary>
        /// handles the closing Font XML element.
        /// </summary>
        private void ElementFontEnd()
        {
            System.GetSingleton().Logger
                  .LogEvent(String.Format("Finished creation of Font '{0}' via XML file. (0x{1:X8})",
                                          _font.GetName(), _font.GetHashCode()),
                            LoggingLevel.Informative);
        }

        /// <summary>
        /// handles the opening Mapping XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementMappingStart(XMLAttributes attributes)
        {
            if (_font == null)
                throw new InvalidRequestException("Attempt to access null object.");

            // double-check font type just in case - report issues as 'soft' errors
            if (_font.GetTypeName() != FontTypePixmap)
                System.GetSingleton().Logger.LogEvent(
                    "Imageset_xmlHandler::elementMappingStart: <Mapping> element is " +
                    "only valid for Pixmap type fonts.", LoggingLevel.Errors);
            else
                ((PixmapFont) _font).DefineMapping(
                    (char)attributes.GetValueAsInteger(MappingCodepointAttribute),
                    attributes.GetValueAsString(MappingImageAttribute),
                    attributes.GetValueAsFloat(MappingHorzAdvanceAttribute, -1.0f));
        }

        /// <summary>
        /// creates a FreeTypeFont
        /// </summary>
        /// <param name="attributes"></param>
        private void CreateFreeTypeFont(XMLAttributes attributes)
        {
#if CEGUI_HAS_FREETYPE
			var name = attributes.GetValueAsString(FontNameAttribute);
	        var filename = attributes.GetValueAsString(FontFilenameAttribute);
	        var resource_group = attributes.GetValueAsString(FontResourceGroupAttribute);

	        Logger.LogInsane("---- CEGUI font name: " + name);
	        Logger.LogInsane("----       Font type: FreeType");
	        Logger.LogInsane("----     Source file: " + filename + " in resource group: " +
	                         (String.IsNullOrEmpty(resource_group)
		                          ? "(Default)"
		                          : resource_group));
	        Logger.LogInsane("---- Real point size: " + attributes.GetValueAsString(FontSizeAttribute, "12"));
			
	        _font = new FreeTypeFont(name,
	                                 attributes.GetValueAsFloat(FontSizeAttribute, 12.0f),
	                                 attributes.GetValueAsBool(FontAntiAliasedAttribute, true),
	                                 filename, resource_group,
	                                 PropertyHelper.FromString<AutoScaledMode>(attributes.GetValueAsString(FontAutoScaledAttribute)),
	                                 new Sizef(attributes.GetValueAsFloat(FontNativeHorzResAttribute, 640.0f),
	                                           attributes.GetValueAsFloat(FontNativeVertResAttribute, 480.0f)),
	                                 attributes.GetValueAsFloat(FontLineSpacingAttribute, 0.0f));
#else
	        throw new InvalidRequestException("CEGUI was compiled without freetype support.");
#endif
		}

		/// <summary>
		/// creates a PixmapFont
		/// </summary>
		/// <param name="attributes"></param>
		private void CreatePixmapFont(XMLAttributes attributes)
        {
            var name = attributes.GetValueAsString(FontNameAttribute);
            var filename = attributes.GetValueAsString(FontFilenameAttribute);
            var resourceGroup = attributes.GetValueAsString(FontResourceGroupAttribute);

            Logger.LogInsane("---- CEGUI font name: " + name);
            Logger.LogInsane("----       Font type: Pixmap");
            Logger.LogInsane("----     Source file: " + filename + " in resource group: " + (String.IsNullOrEmpty(resourceGroup) ? "(Default)" : resourceGroup));

            _font = new PixmapFont(
                name, filename, resourceGroup,
                PropertyHelper.FromString<AutoScaledMode>(attributes.GetValueAsString(FontAutoScaledAttribute)),
                new Sizef(attributes.GetValueAsFloat(FontNativeHorzResAttribute, 640.0f),
                          attributes.GetValueAsFloat(FontNativeVertResAttribute, 480.0f)));
        }

        private void CreateFntFont(XMLAttributes attributes)
        {
            var name = attributes.GetValueAsString(FontNameAttribute);
            var filename = attributes.GetValueAsString(FontFilenameAttribute);
            var resourceGroup = attributes.GetValueAsString(FontResourceGroupAttribute);

            Logger.LogInsane("---- CEGUI font name: " + name);
            Logger.LogInsane("----       Font type: Fnt");
            Logger.LogInsane("----     Source file: " + filename + " in resource group: " + (String.IsNullOrEmpty(resourceGroup) ? "(Default)" : resourceGroup));

            _font = new FntFont(
                    name, filename, resourceGroup,
                    PropertyHelper.FromString<AutoScaledMode>(attributes.GetValueAsString(FontAutoScaledAttribute)),
                    new Sizef(attributes.GetValueAsFloat(FontNativeHorzResAttribute, 640.0f),
                              attributes.GetValueAsFloat(FontNativeVertResAttribute, 480.0f)));
        }

        /// <summary>
        /// throw exception if file version is not supported.
        /// </summary>
        /// <param name="attrs"></param>
        private void ValidateFontFileVersion(XMLAttributes attrs)
        {
            string version = (attrs.GetValueAsString(FontVersionAttribute, "unknown"));

            if (version == NativeVersion)
                return;

            throw new InvalidRequestException(
                "You are attempting to load a font of version '" + version + "' but " +
                "this CEGUI version is only meant to load fonts of version '" +
                NativeVersion + "'. Consider using the migrate.py script bundled with " +
                "CEGUI Unified Editor to migrate your data.");
        }

#region Fields

        /// <summary>
        /// Font object that we are constructing.
        /// </summary>
        private Font _font;

        /// <summary>
        /// inidcates whether client read the created object
        /// </summary>
        private bool _objectRead;

        //----------------------------------------------------------------------------//
        // note: The assets' versions aren't usually the same as CEGUI version, they
        // are versioned from version 1 onwards!
        //
        // previous versions (though not specified in files until 3)
        // 1 - CEGUI up to and including 0.4.x
        // 2 - CEGUI versions 0.5.x through 0.7.x (Static/Dynamic types renamed to Pixmap/TrueType
        //                                         Removed facility to pre-declare glyphs and glyph ranges)
        // 3 - CEGUI version 1.x.x (changed case of attr names, added version support)
        private const string NativeVersion = "3";

#endregion
    }
}