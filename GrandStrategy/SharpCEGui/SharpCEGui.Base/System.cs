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
using System.Linq;
using System.Reflection;
using SharpCEGui.Base.Views;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.Base
{
    /// <summary>
    /// The System class is the CEGUI class that provides access to all other elements in this system.
    /// 
    /// This object must be created by the client application.  The System object requires that you pass it
    /// an initialised Renderer object which it can use to interface to whatever rendering system will be
    /// used to display the GUI imagery.
    /// </summary>
    public class System : IDisposable
    {
        #region Version Info

        public const int CEGUI_VERSION_MAJOR = 9999;
        public const int CEGUI_VERSION_MINOR = 0;
        public const int CEGUI_VERSION_PATCH = 0;

        // this is used to check consistency between runtime binary and headers
        // used for compiling.  You should not generally use this in client code
        // or rely on its value meaning anything in particular.
        public const int CEGUI_VERSION_ABI = 99990;

        #endregion

        #region Implementation of Singleton Pattern

        private static System _instance;

        #endregion

        /// <summary>
        /// Event fired for display size changes (as notified by client code).
        /// </summary>
        public event EventHandler<DisplayEventArgs> DisplaySizeChanged;

        /// <summary>
        /// Event fired when global custom RenderedStringParser is set.
        /// </summary>
        public event EventHandler<EventArgs> RenderedStringParserChanged;

        /*!
        \brief
            Create the System object and return a reference to it.

        \param renderer
            Reference to a valid Renderer object that will be used to render GUI
            imagery.

        \param resourceProvider
            Pointer to a ResourceProvider object, or NULL to use whichever default
            the Renderer provides.

        \param xmlParser
            Pointer to a valid XMLParser object to be used when parsing XML files,
            or NULL to use a default parser.

        \param imageCodec
            Pointer to a valid ImageCodec object to be used when loading image
            files, or NULL to use a default image codec.

        \param scriptModule
            Pointer to a ScriptModule object.  may be NULL for none.

        \param configFile
            String object containing the name of a configuration file to use.

        \param logFile
            String object containing the name to use for the log file.

        \param abi
            This must be set to CEGUI_VERSION_ABI
        */

        //public static System Create(Renderer renderer,
        //                      ResourceProvider resourceProvider = 0,
        //                      XMLParser xmlParser = 0,
        //                      ImageCodec imageCodec = 0,
        //                      ScriptModule scriptModule = 0,
        //                      string configFile = "",
        //                      string logFile = "CEGUI.log",
        //                      int abi = CEGUI_VERSION_ABI);

        public static System Create(Renderer renderer,
                                    ResourceProvider resourceProvider = null,
                                    XMLParser xmlParser = null,
                                    ImageCodec imageCodec = null,
                                    string configFile = "",
                                    string logFile = "CEGUISharp.log")
        {
            if (_instance == null)
            {
                // TODO: PerformVersionTest(CEGUI_VERSION_ABI, abi, CEGUI_FUNCTION_NAME);

                _instance = new System(renderer, resourceProvider, xmlParser, imageCodec, configFile, logFile);
                _instance.Initialize(configFile, logFile);
            }

            return _instance;
        }

        /// <summary>
        /// Destroy the System object.
        /// </summary>
        public static void Destroy()
        {
            _instance.Dispose();
        }

        /// <summary>
        /// Retrieves CEGUI's major version as an integer
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// API/headers major version is a macro called CEGUI_MAJOR_VERSION,
        /// this returns the version your application is linking to
        /// </remarks>
        public static int GetMajorVersion()
        {
            return CEGUI_VERSION_MAJOR;
        }

        /// <summary>
        /// Retrieves CEGUI's minor version as an integer
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// API/headers minor version is a macro called CEGUI_MINOR_VERSION,
        /// this returns the version your application is linking to
        /// </remarks>
        public static int GetMinorVersion()
        {
            return CEGUI_VERSION_MINOR;
        }

        /// <summary>
        /// Retrieves CEGUI's patch version as an integer
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// API/headers patch version is a macro called CEGUI_PATCH_VERSION,
        /// this returns the version your application is linking to
        /// </remarks>
        public static int GetPatchVersion()
        {
            return CEGUI_VERSION_PATCH;
        }

        /// <summary>
        /// Retrieves CEGUI's "short" version ("1.2.3" for example)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// API/headers version can be constructed using CEGUI_*_VERSION macros,
        /// this returns the version your application is linking to
        /// </remarks>
        public static string GetVersion()
        {
            return String.Format("{0}.{1}.{2}", GetMajorVersion(), GetMinorVersion(), GetPatchVersion());
        }

        /*!
        \brief
            Retrieves CEGUI's "verbose" version, includes info about compiler, platform, etc...

        \note
            API/headers verbose version can be constructed using various compiler specific macros,
            this returns the version your application is linking to
        */

        private static string _verboseVersion;
        public static string GetVerboseVersion()
        {
            if (String.IsNullOrEmpty(_verboseVersion))
            {
                _verboseVersion = GetVersion();

                //_verboseVersion += " (Build: " __DATE__;

                //#if defined(CEGUI_STATIC)
                //        ret += " Static";
                //#endif

                //#if defined(DEBUG) || defined(_DEBUG)
                //        ret += " Debug";
                //#endif

                //#if defined(__linux__)
                //        ret += " GNU/Linux";
                //#elif defined (__FreeBSD__)
                //        ret += " FreeBSD";
                //#elif defined (__APPLE__)
                //        ret += " Apple Mac";
                //#elif defined (_WIN32) || defined (__WIN32__)
                //        ret += " Microsoft Windows";
                //#endif

                //#ifdef __GNUG__
                //        ret += " g++ " __VERSION__;

                //#ifdef _LP64
                //        ret += " 64 bit";
                //#else
                //        ret += " 32 bit";
                //#endif

                //#elif defined(_MSC_VER)
                //        ret += " MSVC++ ";
                //#if _MSC_VER <= 1200
                //        ret += "Dinosaur Edition!";
                //#elif _MSC_VER == 1300
                //        ret += "7.0";
                //#elif _MSC_VER == 1310
                //        ret += "7.1";
                //#elif _MSC_VER == 1400
                //        ret += "8.0";
                //#elif _MSC_VER == 1500
                //        ret += "9.0";
                //#elif _MSC_VER == 1600
                //        ret += "10.0";
                //#elif _MSC_VER == 1700
                //        ret += "11.0";
                //#elif _MSC_VER > 1700
                //        ret += "Great Scott!";
                //#endif

                //#ifdef _WIN64
                //        ret += " 64 bit";
                //#else
                //        ret += " 32 bit";
                //#endif

                //#endif

                //ret += ")";
            }

            return _verboseVersion;
        }

        /// <summary>
        /// Return a pointer to the Renderer object being used by the system
        /// </summary>
        /// <returns>
        /// Pointer to the Renderer object used by the system.
        /// </returns>
        public Renderer GetRenderer()
        {
            return d_renderer;
        }

        /// <summary>
        /// Return singleton System object
        /// </summary>
        /// <returns>
        /// Singleton System object
        /// </returns>
        public static System GetSingleton()
        {
            return _instance;
        }


        /// <summary>
        /// Retrieves internal CEGUI clipboard, optionally synced with system wide clipboard
        /// </summary>
        public Clipboard GetClipboard()
        {
            return d_clipboard;
        }

        public GUIContext GetDefaultGUIContext()
        {
            if (d_guiContexts.Count == 0)
                throw new InvalidRequestException(
                        "Requesting the DefaultGUIContext, but no DefaultGUIContext is available. The list of GUIContexts is empty.");

            return d_guiContexts.FirstOrDefault();
        }

        /// <summary>
        /// Depending upon the internal state, for each GUIContext this may either
        /// re-use cached rendering from last time or trigger a full re-draw of all
        /// elements.
        /// </summary>
        public void RenderAllGUIContexts()
        {
            d_renderer.BeginRendering();

            foreach (var i in d_guiContexts)
                i.Draw();

            d_renderer.EndRendering();

            // do final destruction on dead-pool windows
            WindowManager.GetSingleton().CleanDeadPool();
        }

        /// <summary>
        /// Return a pointer to the ScriptModule being used for scripting within the GUI system.
        /// </summary>
        /// <returns>
        /// Pointer to a ScriptModule based object.
        /// </returns>
        // TODO: public ScriptModule GetScriptingModule();

        /// <summary>
        /// Set the ScriptModule to be used for scripting within the GUI system.
        /// </summary>
        /// <param name="scriptModule">
        /// Pointer to a ScriptModule based object, or null for none (be careful!)
        /// </param>
        // TODO: public void SetScriptingModule(ScriptModule scriptModule);

        /// <summary>
        /// Return a pointer to the ResourceProvider being used within the GUI system.
        /// </summary>
        /// <returns>
        /// Pointer to a ResourceProvider based object.
        /// </returns>
        public ResourceProvider GetResourceProvider()
        {
            return d_resourceProvider;
        }

        #region Singletons

        public Logger Logger
        {
            get { return _logger; }
        }

        #endregion

        /// <summary>
        /// Execute a script file if possible.
        /// </summary>
        /// <param name="filename">
        /// String object holding the filename of the script file that is to be executed
        /// </param>
        /// <param name="resourceGroup">
        /// Resource group identifier to be passed to the ResourceProvider when loading the script file.
        /// </param>
        public void ExecuteScriptFile(string filename, string resourceGroup = "")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute a scripted global function if possible. 
        /// The function should not take any parameters and should return an integer.
        /// </summary>
        /// <param name="function_name">
        /// String object holding the name of the function, in the global script environment, that
        /// is to be executed.
        /// </param>
        /// <returns>
        /// The integer value returned from the script function.
        /// </returns>
        public int ExecuteScriptGlobal(string function_name)
        {
            throw new NotImplementedException();
        }


        /*!
    \brief
        If possible, execute script code contained in the given CEGUI::String object.

    \param str
        String object holding the valid script code that should be executed.

    \return
        Nothing.
    */

        public void ExecuteScriptString(string str)
        {
            throw new NotImplementedException();
        }


        ///*!
        //\brief
        //    Set a new XML parser module to be used.

        //    The current XMLParser will be cleaned up and, if owned by the system,
        //    also deleted, as will any dynamically loaded module associated with the
        //    XMLParser object.  The newly created XMLParser object, and the
        //    associated module will be owned by the system.

        //\param parserName
        //    String object describing the name of the XML parser module to be used.
        //*/
        //void setXMLParser(const String& parserName);

        ///*!
        //\brief
        //    Sets the XMLParser object to be used by the system.

        //    The current XMLParser will be cleaned up and, if owned by the system,
        //    also deleted, as will any dynamically loaded module associated with the
        //    XMLParser object.

        //    If the argument passed in the \a parser parameter is 0, the system will
        //    cleanup any existing parser as described above, and revert to using
        //    the parser provided by the default module (see getDefaultXMLParserName
        //    and setDefaultXMLParserName).

        //\param parser
        //    Pointer to the XMLParser object to be used by the system, or 0 to cause
        //    the system to initialise a default parser.
        //*/
        public void setXMLParser(XMLParser parser)
        {
            CleanupXmlParser();
            d_xmlParser = parser;
            d_ourXmlParser = false;
            SetupXmlParser();
        }

        /// <summary>
        /// Return the XMLParser object.
        /// </summary>
        /// <returns></returns>
        public XMLParser GetXMLParser()
        {
            return d_xmlParser;
        }


        ///*!
        //\brief
        //    Static member to set the name of the default XML parser module that
        //    should be used.

        //    If you want to modify the default parser from the one compiled in, you
        //    need to call this static member prior to instantiating the main
        //    CEGUI::System object.

        //    Note that calling this member to change the name of the default module
        //    after CEGUI::System, and therefore the default xml parser, has been
        //    created will have no real effect - the default parser name will be
        //    updated, though no actual changes to the xml parser module will occur.

        //    The built-in options for this are:
        //     - XercesParser
        //     - ExpatParser
        //     - LibxmlParser
        //     - TinyXMLParser

        //    Whether these are actually available, depends upon how you built the
        //    system.  If you have some custom parser, you can provide the name of
        //    that here to have it used as the default, though note that the
        //    final filename of the parser module should be of the form:

        //    [prefix]CEGUI[parserName][suffix]

        //    where:
        //    - [prefix] is some optional prefix; like 'lib' on linux.
        //    - CEGUI is a required prefix.
        //    - [parserName] is the name of the parser, as supplied to this function.
        //    - [suffix] is the filename suffix, like .dll or .so

        //    Final module filenames are, thus, of the form:
        //    - CEGUIXercesParser.dll
        //    - libCEGUIXercesParser.so

        //\param parserName
        //    String describing the name of the xml parser module to be used as the
        //    default.

        //\return
        //    Nothing.
        //*/
        public static void SetDefaultXMLParserName(string parserName)
        {
            d_defaultXMLParserName = parserName;
        }

        ///*!
        //\brief
        //    Return the name of the currently set default xml parser module.

        //\return
        //    String holding the currently set default xml parser name.  Note that if
        //    this name has been changed after instantiating the system, the name
        //    returned may not actually correspond to the module in use.
        //*/
        public static string GetDefaultXMLParserName()
        {
            throw new NotImplementedException();
        }

        ///*!
        //\brief
        //    Retrieve the image codec to be used by the system.
        //*/
        public ImageCodec GetImageCodec()
        {
            return d_imageCodec;
        }

        ///*!
        //\brief
        //    Set the image codec to be used by the system.
        //*/
        public void setImageCodec(string codecName)
        {
            throw new NotImplementedException();
        }

        ///*!
        //\brief
        //    Set the image codec to use from an existing image codec.

        //    In this case the renderer does not take the ownership of the image codec
        //    object.

        //\param codec
        //    The ImageCodec object to be used.
        //*/
        public void SetImageCodec(ImageCodec codec)
        {
            throw new NotImplementedException();
        }

        ///*!
        //\brief
        //    Set the name of the default image codec to be used.
        //*/
        public static void SetDefaultImageCodecName(string codecName)
        {
            throw new NotImplementedException();
        }

        ///*!
        //\brief
        //    Get the name of the default image codec.
        //*/
        public static string getDefaultImageCodecName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notification function to be called when the main display changes size.
        /// Client code should call this function when the host window changes size,
        /// or if the display resolution is changed in full-screen mode.
        /// 
        /// Calling this function ensures that any other parts of the system that
        /// need to know about display size changes are notified.  This affects
        /// things such as the Cursor default constraint area, and also the
        /// auto-scale functioning of Imagesets and Fonts.
        /// </summary>
        /// <param name="new_size">
        /// Size object describing the new display size in pixels.
        /// </param>
        /// <remarks>
        /// This function will also fire the System::EventDisplaySizeChanged event.
        /// </remarks>
        public void NotifyDisplaySizeChanged(Sizef new_size)
        {
            // notify other components of the display size change
            ImageManager.GetSingleton().NotifyDisplaySizeChanged(new_size);
            FontManager.GetSingleton().NotifyDisplaySizeChanged(new_size);
            d_renderer.SetDisplaySize(new_size);

            InvalidateAllWindows();

            // Fire event
            var handler = DisplaySizeChanged;
            if (handler!=null)
                handler(this, new DisplayEventArgs(new_size));

            Logger.LogEvent("Display resize:" +
                            " w=" + PropertyHelper.ToString(new_size.Width) +
                            " h=" + PropertyHelper.ToString(new_size.Height));
        }

        /// <summary>
        /// Return pointer to the currently set global default custom
        /// RenderedStringParser object.
        /// 
        /// The returned RenderedStringParser is used for all windows that have
        /// parsing enabled and no custom RenderedStringParser set on the window
        /// itself.
        /// 
        /// If this global custom RenderedStringParser is set to 0, then all windows
        /// with parsing enabled and no custom RenderedStringParser set on the
        /// window itself will use the systems BasicRenderedStringParser. 
        /// </summary>
        /// <returns></returns>
        public RenderedStringParser GetDefaultCustomRenderedStringParser()
        {
            return d_customRenderedStringParser;
        }

        /*!
    \brief
        Set the global default custom RenderedStringParser object.  This change
        is reflected the next time an affected window reparses it's text.  This
        may be set to 0 for no system wide custom parser (which is the default).

        The set RenderedStringParser is used for all windows that have
        parsing enabled and no custom RenderedStringParser set on the window
        itself.

        If this global custom RenderedStringParser is set to 0, then all windows
        with parsing enabled and no custom RenderedStringParser set on the
        window itself will use the systems BasicRenderedStringParser. 
    */

        public void SetDefaultCustomRenderedStringParser(RenderedStringParser parser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalidate all imagery and geometry caches for CEGUI managed elements.
        /// </summary>
        /// <remarks>
        /// This function will invalidate the caches used for both imagery and
        /// geometry for all content that is managed by the core CEGUI manager
        /// objects, causing a full and total redraw of that content.  This
        /// includes Window object's cached geometry, rendering surfaces and
        /// rendering windows and the mouse pointer geometry.
        /// </remarks>
        public void InvalidateAllCachedRendering()
        {
            InvalidateAllWindows();
        }
        
        /// <summary>
        /// Create a RegexMatcher instance if support is available.
        /// </summary>
        /// <returns>
        /// Pointer to an object that implements the RegexMatcher interface, or 0
        /// if the system has no built in support for RegexMatcher creation.
        /// </returns>
        /// <remarks>
        /// The created RegexMatcher is not tracked in any way, and it is the
        /// resposibility of the caller to destroy the RegexMatcher when it is no
        /// longer needed by calling System::destroyRegexMatcher.
        /// </remarks>
        public RegexMatcher CreateRegexMatcher()
        {
            return new DefaultRegexMatcher();
        }

        /// <summary>
        /// destroy a RegexMatcher instance returned by System::createRegexMatcher.
        /// </summary>
        /// <param name="rm"></param>
        public void DestroyRegexMatcher(RegexMatcher rm)
        {
            // TODO: CEGUI_DELETE_AO rm;
        }

        /// <summary>
        /// call this to ensure system-level time based updates occur.
        /// </summary>
        /// <param name="timeElapsed"></param>
        /// <returns></returns>
        public bool InjectTimePulse(float timeElapsed)
        {
            AnimationManager.GetSingleton().AutoStepInstances(timeElapsed);
            return true;
        }

        public GUIContext CreateGUIContext(IRenderTarget rt)
        {
            var c = new GUIContext(rt);
            d_guiContexts.Add(c);
            return c;
        }

        public void DestroyGUIContext(GUIContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// adds factories for all the basic window types
        /// 
        /// You do not need to call this manually! Standard Window factories will be
        /// added automatically. One occasion when you will need this is if you
        /// remove all window factories from WindowFactoryManager and want to add the
        /// standard ones back
        /// </summary>
        public void AddStandardWindowFactories()
        {
            WindowFactoryManager.AddWindowType<DefaultWindow>();
            WindowFactoryManager.AddWindowType<DragContainer>();
            WindowFactoryManager.AddWindowType<ScrolledContainer>();
            WindowFactoryManager.AddWindowType<ClippedContainer>(); // TODO: to be removed
            WindowFactoryManager.AddWindowType<PushButton>();
            WindowFactoryManager.AddWindowType<RadioButton>();
            WindowFactoryManager.AddWindowType<Combobox>();
            WindowFactoryManager.AddWindowType<ComboDropList>();
            //WindowFactoryManager.AddWindowType<ComboboxV2>(); // TODO: to be removed
            //WindowFactoryManager.AddWindowType<ComboDropListV2>(); // TODO: to be removed
            WindowFactoryManager.AddWindowType<Editbox>();
            WindowFactoryManager.AddWindowType<FrameWindow>();
            WindowFactoryManager.AddWindowType<ItemEntry>();
            WindowFactoryManager.AddWindowType<Listbox>(); // TODO: to be removed
            WindowFactoryManager.AddWindowType<ListHeader>();
            WindowFactoryManager.AddWindowType<ListHeaderSegment>();
            WindowFactoryManager.AddWindowType<ListWidget>();
            WindowFactoryManager.AddWindowType<Menubar>();
            WindowFactoryManager.AddWindowType<PopupMenu>();
            WindowFactoryManager.AddWindowType<MenuItem>();
            WindowFactoryManager.AddWindowType<MultiColumnList>();
            WindowFactoryManager.AddWindowType<MultiLineEditbox>();
            WindowFactoryManager.AddWindowType<ProgressBar>();
            WindowFactoryManager.AddWindowType<ScrollablePane>();
            WindowFactoryManager.AddWindowType<Scrollbar>();
            WindowFactoryManager.AddWindowType<Slider>();
            WindowFactoryManager.AddWindowType<Spinner>();
            WindowFactoryManager.AddWindowType<TabButton>();
            WindowFactoryManager.AddWindowType<TabControl>();
            WindowFactoryManager.AddWindowType<Thumb>();
            WindowFactoryManager.AddWindowType<Titlebar>();
            WindowFactoryManager.AddWindowType<ToggleButton>();
            WindowFactoryManager.AddWindowType<Tooltip>();
            WindowFactoryManager.AddWindowType<ItemListbox>();
            // TODO: WindowFactoryManager.AddWindowType<GroupBox>();
            WindowFactoryManager.AddWindowType<Tree>(); // TODO: to be removed
            // TODO: WindowFactoryManager.AddWindowType<TreeWidget>();
            WindowFactoryManager.AddWindowType<HorizontalLayoutContainer>();
            WindowFactoryManager.AddWindowType<VerticalLayoutContainer>();
            WindowFactoryManager.AddWindowType<GridLayoutContainer>();

            WindowFactoryManager.AddWindowType<ListView>();
            WindowFactoryManager.AddWindowType<TreeView>();
        }

        ////! Return the system StringTranscoder object
        //static const StringTranscoder& getStringTranscoder();

        //! Internal CEGUI version validation function.
        //static void performVersionTest(const int expected, const int received, const String& func);

        /*************************************************************************
		Implementation Functions
	*************************************************************************/
        /*!
    \brief
        Construct a new System object

    \param renderer
        Reference to a valid Renderer object that will be used to render GUI
        imagery.

    \param resourceProvider
        Pointer to a ResourceProvider object, or NULL to use whichever default
        the Renderer provides.

    \param xmlParser
        Pointer to a valid XMLParser object to be used when parsing XML files,
        or NULL to use a default parser.

    \param imageCodec
        Pointer to a valid ImageCodec object to be used when loading image
        files, or NULL to use a default image codec.

    \param scriptModule
        Pointer to a ScriptModule object.  may be NULL for none.

    \param configFile
        String object containing the name of a configuration file to use.

    \param logFile
        String object containing the name to use for the log file.
    */
        //protected System(Renderer renderer, ResourceProvider* resourceProvider,
        //       XMLParser* xmlParser, ImageCodec* imageCodec,
        //       ScriptModule* scriptModule, const String& configFile,
        //       const String& logFile);

        protected System(Renderer renderer,
                         ResourceProvider resourceProvider,
                         XMLParser xmlParser,
                         ImageCodec imageCodec,
                         string configFile, string logFile)
        {
            d_renderer = renderer;
            d_resourceProvider = resourceProvider;
            d_ourResourceProvider = false;
            d_clipboard = new Clipboard();
            //TODO: d_scriptModule(scriptModule),
            d_xmlParser = xmlParser;
            d_ourXmlParser = false;
            //TODO: d_parserModule(0),
            d_imageCodec = imageCodec;
            d_ourImageCodec = false;
            //TODO: d_imageCodecModule(0),
            d_ourLogger = Logger.Instance == null;
            d_customRenderedStringParser = null;
        }

        protected void Initialize(string configFile, string logFile)
        {
            //// Instantiate logger first (we have no file at this point, but entries will
            //// be cached until we do)
            ////
            //// NOTE: If the user already created a logger prior to calling this
            //// constructor, we mark it as so and leave the logger untouched. This allows
            //// the user to fully customize the logger as he sees fit without fear of
            //// seeing its configuration overwritten by this.
            //#ifdef CEGUI_HAS_DEFAULT_LOGGER
            if (d_ourLogger)
                new DefaultLogger();
            //#endif

            _logger = Logger.Instance;
#if DEBUG
            _logger.SetLoggingLevel(LoggingLevel.Insane);
#endif
            _logger.SetLogFilename(logFile);

            var logger = _logger;

            if (d_resourceProvider==null)
            {
                d_resourceProvider = new DefaultResourceProvider();
                d_ourResourceProvider = true;
            }

            // handle initialisation and setup of the XML parser
            SetupXmlParser();

            //// now XML is available, read the configuration file (if any)
            //Config_xmlHandler config;
            //if (!configFile.empty())
            //{
            //    CEGUI_TRY
            //    {
            //        d_xmlParser->parseXMLFile(config, configFile,
            //                                  config.CEGUIConfigSchemaName,
            //                                  "");
            //    }
            //    CEGUI_CATCH(...)
            //    {
            //        // cleanup XML stuff
            //        d_xmlParser->cleanup();
            //        CEGUI_DELETE_AO d_xmlParser;
            //        CEGUI_RETHROW;
            //    }
            //}

            //// Initialise logger if the user didn't create a logger beforehand
            if (d_ourLogger)
            {
                // TODO: config.initialiseLogger(logFile);
            }

            //// if we created the resource provider we know it's DefaultResourceProvider
            //// so can auto-initialise the resource group directories via the config
            //if (d_ourResourceProvider)
            //    config.initialiseResourceGroupDirectories();

            //// get config to update XML parser if it needs to.
            //config.initialiseXMLParser();

            //// set up ImageCodec
            //config.initialiseImageCodec();
            if (d_imageCodec==null)
                SetupImageCodec("");

            //// initialise any default resource groups specified in the config.
            //config.initialiseDefaultResourceGroups();

            OutputLogHeader();

            // beginning main init
            logger.LogEvent("---- Begining CEGUI System initialisation ----");

            // create the core system singleton objects
            CreateSingletons();

            // create the first GUIContext using the renderers default target,
            // this will become the default GUIContext.
            CreateGUIContext(d_renderer.GetDefaultRenderTarget());

            // add the window factories for the core window types
            AddStandardWindowFactories();

            //char addr_buff[32];
            //sprintf(addr_buff, "(%p)", static_cast<void*>(this));
            logger.LogEvent("CEGUI::System singleton created. " + GetHashCode().ToString("X8"));
            logger.LogEvent("---- CEGUI System initialisation completed ----");
            logger.LogEvent("");

            //// autoload resources specified in config
            //config.loadAutoResources();

            //// set up defaults
            //config.initialiseDefaultFont();
            //config.initialiseDefaultMouseCursor();
            //config.initialiseDefaulTooltip();

            //// scripting available?
            //if (d_scriptModule)
            //{
            //    d_scriptModule->createBindings();
            //    config.executeInitScript();
            //    d_termScriptName = config.getTerminateScriptName();
            //}
        }

        /// <summary>
        /// Destructor for System objects.
        /// </summary>
        // TODO: ~System();

        #region Implementation of IDisposable

        public void Dispose()
        {
            Logger.LogEvent("---- Begining CEGUI System destruction ----");

            //// execute shut-down script
            //if (!d_termScriptName.empty())
            //{
            //    CEGUI_TRY
            //    {
            //        executeScriptFile(d_termScriptName);
            //    }
            //    CEGUI_CATCH (...) {}  // catch all exceptions and continue system shutdown

            //}

            CleanupImageCodec();

            // cleanup XML stuff
            CleanupXmlParser();

            //
	        // perform cleanup in correct sequence
	        //
            // ensure no windows get created during destruction.  NB: I'm allowing the
            // potential exception to escape here so as to make it obvious that client
            // code should really be adjusted to not create windows during cleanup.
            WindowManager.GetSingleton().Lock();
	        // destroy windows so it's safe to destroy factories
            WindowManager.GetSingleton().DestroyAllWindows();
            WindowManager.GetSingleton().CleanDeadPool();

            // remove factories so it's safe to unload GUI modules
	        WindowFactoryManager.GetSingleton().RemoveAllFactories();

            //// Cleanup script module bindings
            //if (d_scriptModule)
            //    d_scriptModule->destroyBindings();

	        // cleanup singletons
            DestroySingletons();

            // delete all the GUIContexts
            foreach (var i in d_guiContexts)
                i.Dispose();
            
            // TODO: cleanup resource provider if we own it
            //if (d_ourResourceProvider)
            //    CEGUI_DELETE_AO d_resourceProvider;

            Logger.LogEvent("CEGUI::System singleton destroyed. " + this.GetHashCode().ToString("X8"));
	        
            Logger.LogEvent("---- CEGUI System destruction completed ----");

        //#if CEGUI_HAS_DEFAULT_LOGGER
            // delete the Logger object only if we created it.
            if (d_ourLogger)
                Logger.Instance.Dispose();
        //#endif
    
            //CEGUI_DELETE_AO d_clipboard;
            d_clipboard = null;
        }

        #endregion

        /// <summary>
        /// output the standard log header
        /// </summary>
        protected void OutputLogHeader()
        {
            var l = _logger; // TODO: Logger& l(Logger::getSingleton());
            l.LogEvent("");
            l.LogEvent("********************************************************************************");
            l.LogEvent("* Important:                                                                   *");
            l.LogEvent("*     To get support at the CEGUI forums, you must post _at least_ the section *");
            l.LogEvent("*     of this log file indicated below.  Failure to do this will result in no  *");
            l.LogEvent("*     support being given; please do not waste our time.                       *");
            l.LogEvent("********************************************************************************");
            l.LogEvent("********************************************************************************");
            l.LogEvent("* -------- START OF ESSENTIAL SECTION TO BE POSTED ON THE FORUM       -------- *");
            l.LogEvent("********************************************************************************");
            l.LogEvent("---- Version: " + GetVerboseVersion() + " ----");
            l.LogEvent("---- Renderer module is: " + d_renderer.GetIdentifierString() + " ----");
            l.LogEvent("---- XML Parser module is: " + d_xmlParser.GetIdentifierString() + " ----");
            // TODO: l.LogEvent("---- Image Codec module is: " + d_imageCodec.GetIdentifierString() + " ----");
            // TODO: l.LogEvent(d_scriptModule ? "---- Scripting module is: " + d_scriptModule->getIdentifierString() + " ----" : "---- Scripting module is: None ----");
            l.LogEvent("********************************************************************************");
            l.LogEvent("* -------- END OF ESSENTIAL SECTION TO BE POSTED ON THE FORUM         -------- *");
            l.LogEvent("********************************************************************************");
            l.LogEvent("");
        }
        
        /// <summary>
        /// create the other core system singleton objects (except the logger)
        /// </summary>
        protected void CreateSingletons()
        {
            // cause creation of other singleton objects

            //CEGUI_NEW_AO ImageManager();
            //CEGUI_NEW_AO FontManager();
            //CEGUI_NEW_AO WindowFactoryManager();
            //CEGUI_NEW_AO WindowManager();
            //CEGUI_NEW_AO SchemeManager();
            //CEGUI_NEW_AO GlobalEventSet();
            //CEGUI_NEW_AO AnimationManager();
            //CEGUI_NEW_AO WidgetLookManager();
            //CEGUI_NEW_AO WindowRendererManager();
            //CEGUI_NEW_AO RenderEffectManager();
        }

        /// <summary>
        /// cleanup the core system singleton objects
        /// </summary>
        protected void DestroySingletons()
        {
            // TODO: DestroySingletons()
            //CEGUI_DELETE_AO SchemeManager::getSingletonPtr();
            //CEGUI_DELETE_AO WindowManager::getSingletonPtr();
            //CEGUI_DELETE_AO WindowFactoryManager::getSingletonPtr();
            //CEGUI_DELETE_AO WidgetLookManager::getSingletonPtr();
            //CEGUI_DELETE_AO WindowRendererManager::getSingletonPtr();
            //CEGUI_DELETE_AO AnimationManager::getSingletonPtr();
            //CEGUI_DELETE_AO RenderEffectManager::getSingletonPtr();
            FontManager.GetSingleton().Dispose();
            ImageManager.GetSingleton().Dispose();
            //CEGUI_DELETE_AO GlobalEventSet::getSingletonPtr();
        }

        /// <summary>
        /// handle creation and initialisation of the XML parser.
        /// </summary>
        protected void SetupXmlParser()
        {
            // handle creation / initialisation of XMLParser
            if (d_xmlParser == null)
            {
                //#ifndef CEGUI_STATIC
                //        setXMLParser(d_defaultXMLParserName);
                //#else
                //        //Static Linking Call
                //        d_xmlParser = createParser();
                //        // make sure we know to cleanup afterwards.
                //        d_ourXmlParser = true;
                //        d_xmlParser->initialise();
                //#endif
            }
            else
            {
                // parser object already set, just initialise it.
                d_xmlParser.Initialise();
            }
        }

        /// <summary>
        /// handle cleanup of the XML parser
        /// </summary>
        protected void CleanupXmlParser()
        {
            // bail out if no parser
            if (d_xmlParser == null)
                return;

            // get parser object to do whatever cleanup it needs to
            d_xmlParser.Cleanup();

            // exit if we did not create this parser object
            if (!d_ourXmlParser)
                return;

            // TODO: ...
            // if parser module loaded, destroy the parser object & cleanup module
            //    if (d_parserModule)
            //    {
            //        // get pointer to parser deletion function
            //        void(*deleteFunc)(XMLParser*) = (void(*)(XMLParser*))d_parserModule->
            //            getSymbolAddress("destroyParser");
            //        // cleanup the xml parser object
            //        deleteFunc(d_xmlParser);

            //        // delete the dynamic module for the xml parser
            //        CEGUI_DELETE_AO d_parserModule;
            //        d_parserModule = 0;
            //    }
            //#ifdef CEGUI_STATIC
            //    else
            //        //Static Linking Call
            //        destroyParser(d_xmlParser);
            //#endif

            d_xmlParser = null;
        }

        /// <summary>
        /// setup image codec 
        /// </summary>
        /// <param name="codecName"></param>
        protected void SetupImageCodec(string codecName)
        {
            // Cleanup the old image codec
            CleanupImageCodec();

            // TODO: ...
            //#if defined(CEGUI_STATIC)
            //    // for static build use static createImageCodec to create codec object
            //    d_imageCodec = createImageCodec();
            //#else
            //    // load the appropriate image codec module
            //    d_imageCodecModule = codecName.empty() ?
            //        new DynamicModule(String("CEGUI") + d_defaultImageCodecName) :
            //        new DynamicModule(String("CEGUI") + codecName);

            //    // use function from module to create the codec object.
            //    d_imageCodec = ((ImageCodec*(*)(void))d_imageCodecModule->
            //        getSymbolAddress("createImageCodec"))();
            //#endif

            // make sure we mark this as our own object so we can clean it up later.
            d_ourImageCodec = true;
        }

        /// <summary>
        /// cleanup image codec
        /// </summary>
        protected void CleanupImageCodec()
        {
            // bail out if no codec, or if we did not create it.
            if (d_imageCodec==null || !d_ourImageCodec)
                return;

            //if (d_imageCodecModule)
            //{
            //    ((void(*)(ImageCodec*))d_imageCodecModule->
            //        getSymbolAddress("destroyImageCodec"))(d_imageCodec);

            //    CEGUI_DELETE_AO d_imageCodecModule;
            //    d_imageCodecModule = 0;
            //}
            //#if defined(CEGUI_STATIC)
            //else
            //    destroyImageCodec(d_imageCodec);
            //#endif

            d_imageCodec = null;
        }

        /// <summary>
        /// invalidate all windows and any rendering surfaces they may be using.
        /// </summary>
        protected void InvalidateAllWindows()
        {
            foreach (var wi in WindowManager.GetSingleton().Windows)
            {
                // invalidate window itself
                wi.Invalidate(false);
                // if window has rendering window surface, invalidate it's geometry
                var rs = wi.GetRenderingSurface();
                if (rs != null && rs.IsRenderingWindow())
                    ((RenderingWindow) rs).InvalidateGeometry();
            }
        }
        
        #region Fields

        /// <summary>
        /// Holds the pointer to the Renderer object given to us in the constructor
        /// </summary>
        protected Renderer d_renderer;

        /// <summary>
        /// Holds the pointer to the ResourceProvider object given to us by the renderer or the System constructor.
        /// </summary>
        protected ResourceProvider d_resourceProvider;

        protected bool d_ourResourceProvider;

        /// <summary>
        /// Internal clipboard with optional sync with native clipboard
        /// </summary>
        protected Clipboard d_clipboard;

        // scripting
        //!< Points to the scripting support module.
        // TODO: protected ScriptModule d_scriptModule;

        /// <summary>
        /// Name of the script to run upon system shutdown.
        /// </summary>
        protected string d_termScriptName;

        /// <summary>
        /// XMLParser object we use to process xml files.
        /// </summary>
        protected XMLParser d_xmlParser;

        /// <summary>
        /// true when we created the xml parser.
        /// </summary>
        protected bool d_ourXmlParser;

        /// <summary>
        /// pointer to parser module.
        /// </summary>
        // TODO: protected DynamicModule d_parserModule;

        protected static string d_defaultXMLParserName; //!< Holds name of default XMLParser

        /// <summary>
        /// Holds a pointer to the image codec to use.
        /// </summary>
        protected ImageCodec d_imageCodec;

        /// <summary>
        /// true when we created the image codec.
        /// </summary>
        protected bool d_ourImageCodec;

        /// <summary>
        /// Holds a pointer to the image codec module. If d_imageCodecModule is null we
        /// are not owner of the image codec object
        /// </summary>
        protected Assembly d_imageCodecModule;

        //! Holds the name of the default codec to use.
        protected static string d_defaultImageCodecName;

        //! true when we created the CEGUI::Logger based object.
        protected bool d_ourLogger;
        protected Logger _logger;

        /// <summary>
        /// currently set global RenderedStringParser.
        /// </summary>
        protected RenderedStringParser d_customRenderedStringParser;

        protected List<GUIContext> d_guiContexts =new List<GUIContext>();


        #endregion
    }
}