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
    /// Handler class used to parse the Scheme XML files using SAX2
    /// </summary>
    public class Scheme_xmlHandler : XmlHandler, IXmlHandler<Scheme>
    {
        //! Constructor.
        public Scheme_xmlHandler()
        {
            d_scheme = null;
            d_objectRead = false;
        }

        //! Destructor.
        // TODO: ~Scheme_xmlHandler();
        //{
        //    if (!d_objectRead)
        //        CEGUI_DELETE_AO d_scheme;
        //}

        /// <summary>
        /// Return string holding the name of the created Imageset.
        /// </summary>
        /// <returns></returns>
        public string GetObjectName()
        {
            if (d_scheme == null)
                throw new InvalidRequestException("Attempt to access null object.");

            return d_scheme.GetName();
        }

        /// <summary>
        /// Return reference to the created Scheme object.
        /// </summary>
        /// <returns></returns>
        public Scheme GetObject()
        {
            if (d_scheme == null)
                throw new InvalidRequestException("Attempt to access null object.");

            d_objectRead = true;
            return d_scheme;
        }

        // XMLHandler overrides
        public override string GetSchemaName()
        {
            return GUISchemeSchemaName;
        }

        public override string GetDefaultResourceGroup()
        {
            return Scheme.GetDefaultResourceGroup();
        }

        public override void ElementStart(string element, XMLAttributes attributes)
        {
            if (element == WindowAliasElement)
                elementWindowAliasStart(attributes);
            else if (element == ImagesetElement)
                elementImagesetStart(attributes);
            else if (element == ImagesetFromImageElement)
                elementImagesetFromImageStart(attributes);
            else if (element == FontElement)
                elementFontStart(attributes);
            else if (element == WindowSetElement)
                elementWindowSetStart(attributes);
            else if (element == WindowFactoryElement)
                elementWindowFactoryStart(attributes);
            else if (element == WindowRendererSetElement)
                elementWindowRendererSetStart(attributes);
            else if (element == WindowRendererFactoryElement)
                elementWindowRendererFactoryStart(attributes);
            else if (element == GUISchemeElement)
                elementGUISchemeStart(attributes);
            else if (element == FalagardMappingElement)
                elementFalagardMappingStart(attributes);
            else if (element == LookNFeelElement)
                elementLookNFeelStart(attributes);
                // anything else is a non-fatal error.
            else
                System.GetSingleton().Logger
                      .LogEvent("Scheme_xmlHandler::elementStart: Unknown element encountered: <" + element + ">",
                                LoggingLevel.Errors);
        }

        public override void ElementEnd(string element)
        {
            if (element == GUISchemeElement)
                elementGUISchemeEnd();
        }

        //! Function that handles the opening GUIScheme XML element.
        private void elementGUISchemeStart(XMLAttributes attributes)
        {
            var name = attributes.GetValueAsString(NameAttribute);
            var logger = System.GetSingleton().Logger;
            logger.LogEvent("Started creation of Scheme from XML specification:");
            logger.LogEvent("---- CEGUI GUIScheme name: " + name);

            validateSchemeFileVersion(attributes);

            // create empty scheme with desired name
            d_scheme = new Scheme(name);
        }

        //! Function that handles the Imageset XML element.
        private void elementImagesetStart(XMLAttributes attributes)
        {
            var imageset =new Scheme.LoadableUIElement();

            imageset.name = attributes.GetValueAsString(NameAttribute);
            imageset.filename = attributes.GetValueAsString(FilenameAttribute);
            imageset.resourceGroup = attributes.GetValueAsString(ResourceGroupAttribute);

            d_scheme.d_imagesets.Add(imageset);
        }

        //! Function that handles the ImagesetFromImage XML element.
        private void elementImagesetFromImageStart(XMLAttributes attributes)
        {
            var imageset = new Scheme.LoadableUIElement();

            imageset.filename = attributes.GetValueAsString(FilenameAttribute);
            imageset.name = attributes.GetValueAsString(NameAttribute, imageset.filename);
            imageset.resourceGroup = attributes.GetValueAsString(ResourceGroupAttribute);

            d_scheme.d_imagesetsFromImages.Add(imageset);
        }

        //! Function that handles the Font XML element.
        private void elementFontStart(XMLAttributes attributes)
        {
            var font = new Scheme.LoadableUIElement();

            font.name = attributes.GetValueAsString(NameAttribute);
            font.filename = attributes.GetValueAsString(FilenameAttribute);
            font.resourceGroup = attributes.GetValueAsString(ResourceGroupAttribute);

            d_scheme.d_fonts.Add(font);
        }

        //! Function that handles the WindowSet XML element.
        private void elementWindowSetStart(XMLAttributes attributes)
        {
            var module = new Scheme.UIModule
                         {
                                 name = attributes.GetValueAsString(FilenameAttribute),
                                 dynamicModule = null,
                                 factoryModule = null
                         };

            d_scheme.d_widgetModules.Add(module);
        }

        //! Function that handles the WindowFactory XML element.
        private void elementWindowFactoryStart(XMLAttributes attributes)
        {
            d_scheme.d_widgetModules[d_scheme.d_widgetModules.Count - 1].types.Add(attributes.GetValueAsString(NameAttribute));
        }

        //! Function that handles the WindowRendererSet XML element.
        private void elementWindowRendererSetStart(XMLAttributes attributes)
        {
            var module = new Scheme.UIModule
                             {
                                 name = attributes.GetValueAsString(FilenameAttribute),
                                 dynamicModule = null,
                                 factoryModule = null
                             };

            d_scheme.d_windowRendererModules.Add(module);
        }

        //! Function that handles the WindowRendererFactory XML element.
        private void elementWindowRendererFactoryStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        //! Function that handles the WindowAlias XML element.
        private void elementWindowAliasStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        //! Function that handles the FalagardMapping XML element.
        private void elementFalagardMappingStart(XMLAttributes attributes)
        {
            var fmap = new Scheme.FalagardMapping();
            fmap.windowName = attributes.GetValueAsString(WindowTypeAttribute);
            fmap.targetName = attributes.GetValueAsString(TargetTypeAttribute);
            fmap.lookName = attributes.GetValueAsString(LookNFeelAttribute);
            fmap.rendererName = attributes.GetValueAsString(WindowRendererAttribute);
            fmap.effectName = attributes.GetValueAsString(RenderEffectAttribute);

            d_scheme.d_falagardMappings.Add(fmap);
        }

        //! Function that handles the LookNFeel XML element.
        private void elementLookNFeelStart(XMLAttributes attributes)
        {
            var lnf = new Scheme.LoadableUIElement();
            lnf.filename = attributes.GetValueAsString(FilenameAttribute);
            lnf.resourceGroup = attributes.GetValueAsString(ResourceGroupAttribute);

            d_scheme.d_looknfeels.Add(lnf);
        }

        //! Function that handles the closing GUIScheme XML element.
        private void elementGUISchemeEnd()
        {
            if (d_scheme==null)
                throw new InvalidRequestException("Attempt to access null object.");

            //char addr_buff[32];
            //sprintf(addr_buff, "(%p)", static_cast<void*>(d_scheme));

            System.GetSingleton().Logger
                  .LogEvent(String.Format("Finished creation of GUIScheme '{0}' via XML file. (0x{1:X8})",
                                          d_scheme.GetName(), d_scheme.GetHashCode()),
                            LoggingLevel.Informative);
        }

        //! throw exception if file version is not supported.
        private void validateSchemeFileVersion(XMLAttributes attrs)
        {
            var version = attrs.GetValueAsString(SchemeVersionAttribute, "unknown");

            if (version == NativeVersion)
                return;

            throw new InvalidRequestException("You are attempting to load a GUI scheme of version '" + version +
                                              "' but this CEGUI version is only meant to load GUI schemes of version '" +
                                              NativeVersion + "'. Consider using the migrate.py script bundled with " +
                                              "CEGUI Unified Editor to migrate your data.");
        }

        #region Const Fields

        //! Filename of the XML schema used for validating GUIScheme files.
        private const string GUISchemeSchemaName = "GUIScheme.xsd";

        //! Root GUIScheme element.
        private const string GUISchemeElement = "GUIScheme";

        //! Element specifying an Imageset.
        private const string ImagesetElement = "Imageset";

        //! Element specifying an Imageset to be created directly via an image file.
        private const string ImagesetFromImageElement = "ImagesetFromImage";

        //! Element specifying a Font.
        private const string FontElement = "Font";

        //! Element specifying a module and set of WindowFactory elements.
        private const string WindowSetElement = "WindowSet";

        //! Element specifying a WindowFactory type.
        private const string WindowFactoryElement = "WindowFactory";

        //! Element specifying a WindowFactory type alias.
        private const string WindowAliasElement = "WindowAlias";

        //! Element specifying a Falagard window mapping.
        private const string FalagardMappingElement = "FalagardMapping";

        //! Element specifying a LookNFeel.
        private const string LookNFeelElement = "LookNFeel";

        //! Element specifying a module and set of WindowRendererFactory elements.
        private const string WindowRendererSetElement = "WindowRendererSet";

        //! Element specifying a WindowRendererFactory type.
        private const string WindowRendererFactoryElement = "WindowRendererFactory";

        //! Attribute specifying the name of some object.
        private const string NameAttribute = "name";

        //! Attribute specifying the name of some file.
        private const string FilenameAttribute = "filename";

        //! Attribute specifying an alias name.
        private const string AliasAttribute = "alias";

        //! Attribute specifying target for an alias.
        private const string TargetAttribute = "target";

        //! Attribute specifying resource group for some loadable resource.
        private const string ResourceGroupAttribute = "resourceGroup";

        //! Attribute specifying the type of a window being created via a mapping.
        private const string WindowTypeAttribute = "windowType";

        //! Attribute specifying the base type of a falagard mapped window type.
        private const string TargetTypeAttribute = "targetType";

        //! Attribute specifying the name of a LookNFeel for a falagard mapping.
        private const string LookNFeelAttribute = "lookNFeel";

        //! Attribute specifying the type name of a window renderer.
        private const string WindowRendererAttribute = "renderer";

        //! Attribute specifying the name of a registered RenderEffect.
        private const string RenderEffectAttribute = "renderEffect";

        //! Attribute specifying the datafile version.
        private const string SchemeVersionAttribute = "version";

        //----------------------------------------------------------------------------//
        // note: The assets' versions aren't usually the same as CEGUI version, they
        // are versioned from version 1 onwards!
        //
        // previous versions (though not specified in files until 5)
        // 1 - CEGUI up to and including 0.3.x
        // 2 - CEGUI version 0.4.x (added initial falagard support)
        // 3 - CEGUI version 0.5.x and 0.6.x (added window renderer support)
        // 4 - CEGUI version 0.7.x (RenderEffect support, relax need to specify font/imageset names)
        // 5 - CEGUI version 1.x.x (changed case of attr names, added version support)
        private const string NativeVersion = "5";

        #endregion

        #region Fields

        /// <summary>
        /// Scheme object that we are constructing
        /// </summary>
        private Scheme d_scheme;

        /// <summary>
        /// inidcates whether client read the created object
        /// </summary>
        private bool d_objectRead;

        #endregion
    }
}