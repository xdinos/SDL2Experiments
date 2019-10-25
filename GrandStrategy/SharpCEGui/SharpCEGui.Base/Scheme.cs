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
using System.Linq;
using System.Reflection;

namespace SharpCEGui.Base
{
    /// <summary>
    /// A class that groups a set of GUI elements and initialises the system to access those elements.
    /// 
    /// A GUI Scheme is a high-level construct that loads and initialises various lower-level objects
    /// and registers them within the system for usage.  So, for example, a Scheme might create some
    /// Imageset objects, some Font objects, and register a collection of WindowFactory objects within
    /// the system which would then be in a state to serve those elements to client code.
    /// </summary>
    public class Scheme: IDisposable
    {
        // TODO: friend class Scheme_xmlHandler;

        /// <summary>
        /// Constructs an empty scheme object with the specified name.
        /// </summary>
        /// <param name="name">
        /// String object holding the name of the Scheme object.
        /// </param>
        internal Scheme(string name)
        {
            d_name = name;
        }

        /*!
        \brief
            Destroys a Scheme object

        \internal
            has to be public for luabind compatibility
        */
        //TODO: ~Scheme(void)
        //{
        //    unloadResources();

        //    char addr_buff[32];
        //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
        //    Logger::getSingleton().logEvent("GUI scheme '" + d_name + "' has been "
        //        "unloaded (object destructor). " + addr_buff, Informative);
        //}

        #region IDisposable
        public void Dispose()
        {
            UnloadResources();
            //    char addr_buff[32];
            //    sprintf(addr_buff, "(%p)", static_cast<void*>(this));
            //    Logger::getSingleton().logEvent("GUI scheme '" + d_name + "' has been "
            //        "unloaded (object destructor). " + addr_buff, Informative);
        }
        #endregion

        /// <summary>
        /// Loads all resources for this scheme.
        /// </summary>
        public void LoadResources()
        {
            System.GetSingleton().Logger
                .LogEvent("---- Begining resource loading for GUI scheme '" + d_name + "' ----", LoggingLevel.Informative);

            // load all resources specified for this scheme.
            LoadXMLImagesets();
            LoadImageFileImagesets();
            LoadFonts();
            LoadLookNFeels();
            LoadWindowRendererFactories();
            LoadWindowFactories();
            LoadFactoryAliases();
            LoadFalagardMappings();

            System.GetSingleton().Logger
                .LogEvent("---- Resource loading for GUI scheme '" + d_name + "' completed ----", LoggingLevel.Informative);
        }
        
        /// <summary>
        /// Unloads all resources for this scheme.  This should be used very carefully.
        /// </summary>
        public void UnloadResources()
        {
            System.GetSingleton().Logger
                .LogEvent("---- Begining resource cleanup for GUI scheme '" + d_name + "' ----", LoggingLevel.Informative);

            // unload all resources specified for this scheme.
            UnloadFonts();
            // FIXME: UnloadXMLImagesets();
            UnloadImageFileImagesets();
            UnloadWindowFactories();
            UnloadWindowRendererFactories();
            UnloadFactoryAliases();
            UnloadFalagardMappings();
            UnloadLookNFeels();

            System.GetSingleton().Logger
                .LogEvent("---- Resource cleanup for GUI scheme '" + d_name + "' completed ----", LoggingLevel.Informative);
        }

        /// <summary>
        /// Return whether the resources for this Scheme are all loaded.
        /// </summary>
        /// <returns>
        /// true if all resources for the Scheme are loaded and available, 
        /// or false of one or more resource is not currently loaded.
        /// </returns>
        public bool ResourcesLoaded()
        {
            // test state of all loadable resources for this scheme.
            if (//AreXMLImagesetsLoaded() && FIXME: ????
                AreImageFileImagesetsLoaded() &&
                AreFontsLoaded() &&
                AreWindowRendererFactoriesLoaded() &&
                AreWindowFactoriesLoaded() &&
                AreFactoryAliasesLoaded() &&
                AreFalagardMappingsLoaded())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the name of this Scheme.
        /// </summary>
        /// <returns>
        /// String object containing the name of this Scheme.
        /// </returns>
        public string GetName()
        {
            return d_name;
        }

        /// <summary>
        /// Returns the default resource group currently set for Schemes.
        /// </summary>
        /// <returns>
        /// String describing the default resource group identifier that will be
        /// used when loading Scheme xml file data.
        /// </returns>
        public static string GetDefaultResourceGroup()
        {
            return d_defaultResourceGroup;
        }

        /// <summary>
        /// Sets the default resource group to be used when loading scheme xml data
        /// </summary>
        /// <param name="resourceGroup">
        /// String describing the default resource group identifier to be used.
        /// </param>
        public static void SetDefaultResourceGroup(string resourceGroup)
        {
            d_defaultResourceGroup = resourceGroup;
        }

        /// <summary>
        /// Load all XML based imagesets required by the scheme.
        /// </summary>
        public void LoadXMLImagesets()
        {
            // check all imagesets
            foreach (var element in d_imagesets)
            {
                ImageManager.GetSingleton().LoadImageset(element.filename, element.resourceGroup);
            }
        }

        /// <summary>
        /// Load all image file based imagesets required by the scheme.
        /// </summary>
        public void LoadImageFileImagesets()
        {
            var imgr = ImageManager.GetSingleton();
            
            // check images that are created directly from image files
            foreach (var element in d_imagesetsFromImages)
            {
                // if name is empty use the name of the image file.
                if (String.IsNullOrEmpty(element.name))
                    element.name = element.filename;

                // see if image is present, and create it if not.
                if (!imgr.IsDefined(element.name))
                    imgr.AddBitmapImageFromFile(element.name, element.filename, element.resourceGroup);
            }
        }

        /// <summary>
        /// Load all xml based fonts required by the scheme.
        /// </summary>
        public void LoadFonts()
        {
            var fntmgr = FontManager.GetSingleton();

            // check fonts
            foreach (var element in d_fonts)
            {
                // skip if a font with this name is already loaded
                if (!String.IsNullOrEmpty(element.name) && fntmgr.IsDefined(element.name))
                    continue;

                // create font using specified xml file.
                var font = fntmgr.CreateFromFile(element.filename, element.resourceGroup);
                var realname = font.GetName();

                // if name was not in scheme, set it now and proceed to next font
                if (String.IsNullOrEmpty(element.name))
                {
                    element.name = realname;
                    continue;
                }

                // confirm the font loaded has same name specified in scheme
                if (realname != element.name)
                {
                    fntmgr.Destroy(font);
                    throw new InvalidRequestException(
                        "The Font created by file '" + element.filename +
                        "' is named '" + realname + "', not '" + element.name +
                        "' as required by Scheme '" + d_name + "'.");
                }
            }
        }

        /// <summary>
        /// Load all xml looknfeel files required by the scheme.
        /// </summary>
        public void LoadLookNFeels()
        {
            var wlfMgr = WidgetLookManager.GetSingleton();
            
            // load look'n'feels
            // (we can't actually check these, at the moment, so we just re-parse data;
            // it does no harm except maybe waste a bit of time)
            foreach (var element in d_looknfeels)
            {
                wlfMgr.ParseLookNFeelSpecificationFromFile(element.filename, element.resourceGroup);
            }
        }

        
        /// <summary>
        /// Register all window factories required by the scheme.
        /// </summary>
        public void LoadWindowFactories()
        {
            // check factories
            foreach (var cmod in d_widgetModules)
            {
                if (cmod.factoryModule==null)
                {
        //#if !defined(CEGUI_STATIC)
                    // load dynamic module as required
                    if (cmod.dynamicModule == null)
                    {
                        var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cmod.name + ".dll");
                        cmod.dynamicModule = Assembly.LoadFile(filename);
                    }

                    cmod.factoryModule =
                        (FactoryModule) Activator.CreateInstance(
                            cmod.dynamicModule.GetTypes().Single(x => typeof(FactoryModule).IsAssignableFrom(x)));


                    //            FactoryModule& (*getWindowFactoryModuleFunc)() =
                    //                reinterpret_cast<FactoryModule&(*)()>(
                    //                    (*cmod).dynamicModule->
                    //                        getSymbolAddress("getWindowFactoryModule"));

                    //            if (!getWindowFactoryModuleFunc)
                    //                CEGUI_THROW(InvalidRequestException(
                    //                    "Required function export "
                    //                    "'FactoryModule& ""getWindowFactoryModule()' "
                    //                    "was not found in module '" + (*cmod).name + "'."));

                    //            // get the WindowRendererModule object for this module.
                    //            (*cmod).factoryModule = &getWindowFactoryModuleFunc();
                    //#else
                    //            (*cmod).factoryModule = &getWindowFactoryModule();
                    //#endif
                }

                // see if we should just register all factories available in the module
                // (i.e. No factories explicitly specified)
                if (cmod.types.Count == 0)
                {
                    System.GetSingleton().Logger.LogEvent("No Window factories specified for module '" + cmod.name + "' - adding all available factories...");
                    cmod.factoryModule.RegisterAllFactories();
                }
                // some names were explicitly given, so only register those.
                else
                {
                    foreach (var elem in cmod.types)
                    {
                        cmod.factoryModule.RegisterFactory(elem);
                    }
                }
            }
        }
        
        /// <summary>
        /// Register all window renderer factories required by the scheme.
        /// </summary>
        public void LoadWindowRendererFactories()
        {
            // check factories
            foreach (var cmod in d_windowRendererModules)
            {
                if (cmod.factoryModule==null)
                {
        //#if !defined(CEGUI_STATIC)
                    // load dynamic module as required
                    if (cmod.dynamicModule == null)
                    {
                        var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    (cmod.name == "CEGUICoreWindowRendererSet"
                                                         ? "SharpCEGui.CoreWindowRendererSet"
                                                         : cmod.name) + ".dll");
                        
                        cmod.dynamicModule = Assembly.LoadFile(filename);
                    }

                    cmod.factoryModule =
                        (FactoryModule)Activator.CreateInstance(
                            cmod.dynamicModule.GetTypes().Single(x => typeof(FactoryModule).IsAssignableFrom(x)));

        //            FactoryModule& (*getWRFactoryModuleFunc)() =
        //                reinterpret_cast<FactoryModule&(*)()>((*cmod).dynamicModule->
        //                    getSymbolAddress("getWindowRendererFactoryModule"));

        //            if (!getWRFactoryModuleFunc)
        //                CEGUI_THROW(InvalidRequestException(
        //                    "Required function export "
        //                    "'FactoryModule& getWindowRendererFactoryModule()' "
        //                    "was not found in module '" + (*cmod).name + "'."));

        //            // get the WindowRendererModule object for this module.
        //            (*cmod).factoryModule = &getWRFactoryModuleFunc();
        //#else
                    //cmod.factoryModule = GetWindowRendererFactoryModule();
        //#endif
                }

                // see if we should just register all factories available in the module
                // (i.e. No factories explicitly specified)
                if (cmod.types.Count == 0)
                {
                    System.GetSingleton().Logger.LogEvent("No window renderer factories specified for module '" + cmod.name + "' - adding all available factories...");
                    cmod.factoryModule.RegisterAllFactories();
                }
                // some names were explicitly given, so only register those.
                else
                {
                    foreach (var elem in cmod.types)
                    {
                        cmod.factoryModule.RegisterFactory(elem);
                    }
                }
            }
        }

        /// <summary>
        /// Register all factory aliases required by the scheme.
        /// </summary>
        public void LoadFactoryAliases()
        {
            var wfmgr = WindowFactoryManager.GetSingleton();

            // check aliases
            foreach (var alias in d_aliasMappings)
            {
                // get iterator
                var iter = wfmgr.getAliasIterator();
                

                //// look for this alias
                //while (!iter.isAtEnd() && (iter.getCurrentKey() != (*alias).aliasName))
                //    ++iter;

                //// if the alias exists
                //if (!iter.isAtEnd())
                //{
                //    // if the current target type matches
                //    if (iter.getCurrentValue().getActiveTarget() == alias.targetName)
                //    {
                //        // assume this mapping is ours and skip to next alias
                //        continue;
                //    }
                //}

                // create a new alias entry
                wfmgr.AddWindowTypeAlias(alias.aliasName, alias.targetName);
            }
        }

        /// <summary>
        /// Create all falagard mappings required by the scheme.
        /// </summary>
        public void LoadFalagardMappings()
        {
            var wfmgr = WindowFactoryManager.GetSingleton();

            // check falagard window mappings.
            foreach (var falagard in d_falagardMappings)
            {
                // get iterator
                var iter = wfmgr.getFalagardMappingIterator();

                // look for this mapping
                var kvp = iter.SingleOrDefault(x => x.Key == falagard.windowName);
                if (!kvp.Equals(default(KeyValuePair<string, WindowFactoryManager.FalagardWindowMapping>)))
                {
                    // check if the current target and looks and window renderer match
                    if ((kvp.Value.d_baseType == falagard.targetName) &&
                        (kvp.Value.d_rendererType == falagard.rendererName) &&
                        (kvp.Value.d_lookName == falagard.lookName))
                    {
                        // assume this mapping is ours and skip to next
                        continue;
                    }
                }
                //// look for this mapping
                //while (!iter.isAtEnd() && (iter.getCurrentKey() != falagard.windowName))
                //    ++iter;

                //// if the mapping exists
                //if (!iter.isAtEnd())
                //{
                //    // check if the current target and looks and window renderer match
                //    if ((iter.getCurrentValue().d_baseType == falagard.targetName) &&
                //        (iter.getCurrentValue().d_rendererType == falagard.rendererName) &&
                //        (iter.getCurrentValue().d_lookName == falagard.lookName))
                //    {
                //        // assume this mapping is ours and skip to next
                //        continue;
                //    }
                //}

                // create a new mapping entry
                wfmgr.AddFalagardWindowMapping(falagard.windowName,
                                               falagard.targetName,
                                               falagard.lookName,
                                               falagard.rendererName,
                                               falagard.effectName);
            }
        }

        /*!
        \brief
            Unload all XML based imagesets created by the scheme.
        */

        public void UnloadXMLImagesets()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unload all image file based imagesets created by the scheme.
        */

        public void UnloadImageFileImagesets()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unload all xml based fonts created by the scheme.
        */

        public void UnloadFonts()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unload all xml looknfeel files loaded by the scheme.
        */

        public void UnloadLookNFeels()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unregister all window factories registered by the scheme.
        */

        public void UnloadWindowFactories()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unregister all window renderer factories registered by the scheme.
        */

        public void UnloadWindowRendererFactories()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unregister all factory aliases created by the scheme.
        */

        public void UnloadFactoryAliases()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Unregister all falagard mappings created by the scheme.
        */

        public void UnloadFalagardMappings()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all XML based imagesets created by the scheme.
        */

        public bool AreXMLImagesetsLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all image file based imagesets created by the scheme.
        */

        public bool AreImageFileImagesetsLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all xml based fonts created by the scheme.
        */

        public bool AreFontsLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all looknfeel files loaded by the scheme.
        */

        public bool AreLookNFeelsLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all window factories registered by the scheme.
        */

        public bool AreWindowFactoriesLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all window renderer factories registered by the scheme.
        */

        public bool AreWindowRendererFactoriesLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all factory aliases created by the scheme.
        */

        public bool AreFactoryAliasesLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Check state of all falagard mappings created by the scheme.
        */

        public bool AreFalagardMappingsLoaded()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            A single file reference to a font, imageset or so to be loaded as part of this Scheme
        
        \note
            This is public because you might want to iterate over these and load them yourself!
        */

        public class LoadableUIElement
        {
            public string name;
            public string filename;
            public string resourceGroup;
        };

        //private:
        //    //! \internal This is implementation specific so we keep it private!
        //    typedef std::vector<LoadableUIElement
        //        CEGUI_VECTOR_ALLOC(LoadableUIElement)>      LoadableUIElementList;

        // TODO: typedef ConstVectorIterator<LoadableUIElementList> LoadableUIElementIterator;

        /*!
        \brief
            Retrieves iterator for all references to XML imagesets that are to be loaded with this Scheme
        */

        public IEnumerable<LoadableUIElement> GetXMLImagesets()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Retrieves iterator for all references to image file imagesets that are to be loaded with this Scheme
        */

        public IEnumerable<LoadableUIElement> GetImageFileImagesets()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Retrieves iterator for all references to font files that are to be loaded with this Scheme
        */

        public IEnumerable<LoadableUIElement> GetFonts()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Retrieves iterator for all references to LookNFeels files that are to be loaded with this Scheme
        */

        public IEnumerable<LoadableUIElement> GetLookNFeels()
        {
            throw new NotImplementedException();
        }

        //TODO: private:
        /*************************************************************************
            Structs used to hold scheme information
        *************************************************************************/

        public class UIModule
        {
            public string name;
            //TODO: DynamicModule* dynamicModule;
            public Assembly dynamicModule;
            public FactoryModule factoryModule;

            //typedef std::vector<String
            //    CEGUI_VECTOR_ALLOC(String)> TypeList;

            public List<string> types = new List<string>();
        };

        private class AliasMapping
        {
            public string aliasName;
            public string targetName;
        };

        public class FalagardMapping
        {
            public string windowName;
            public string targetName;
            public string rendererName;
            public string lookName;
            public string effectName;
        };

        /*************************************************************************
            Implementation Data
        *************************************************************************/
        private string d_name; //!< the name of this scheme.

        // internal because of Scheme_xmlHandler
        internal List<LoadableUIElement> d_imagesets = new List<LoadableUIElement>();
        internal List<LoadableUIElement> d_imagesetsFromImages = new List<LoadableUIElement>();
        internal List<LoadableUIElement> d_fonts =new List<LoadableUIElement>();

        //typedef std::vector<UIModule
        //    CEGUI_VECTOR_ALLOC(UIModule)>               UIModuleList;
        internal List<UIModule> d_widgetModules = new List<UIModule>();

        //typedef std::vector<UIModule
        //    CEGUI_VECTOR_ALLOC(UIModule)>               WRModuleList;
        internal List<UIModule> d_windowRendererModules = new List<UIModule>();

        //typedef std::vector<AliasMapping
        //    CEGUI_VECTOR_ALLOC(AliasMapping)>			AliasMappingList;

        private List<AliasMapping> d_aliasMappings =new List<AliasMapping>();

        internal List<LoadableUIElement> d_looknfeels =new List<LoadableUIElement>();

        //typedef std::vector<FalagardMapping
        //    CEGUI_VECTOR_ALLOC(FalagardMapping)>        FalagardMappingList;
        internal List<FalagardMapping> d_falagardMappings = new List<FalagardMapping>();

        /// <summary>
        /// holds default resource group
        /// </summary>
        private static string d_defaultResourceGroup;
    }
}