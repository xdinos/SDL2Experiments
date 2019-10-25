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

namespace SharpCEGui.Base
{
    /// <summary>
    /// The WindowManager class describes an object that manages creation and lifetime of Window objects.
    /// 
    /// The WindowManager is the means by which Window objects are created and destroyed.  For each sub-class
    /// of Window that is to be created, there must exist a WindowFactory object which is registered with the
    /// WindowFactoryManager.  Additionally, the WindowManager tracks every Window object created, and can be
    /// used to access those Window objects by name.
    /// </summary>
    public class WindowManager
    {
        #region Implementation of Singleton
        private static readonly Lazy<WindowManager> Instance = new Lazy<WindowManager>(()=>new WindowManager());

        public IEnumerable<Window> Windows
        {
            get { return d_windowRegistry; }
        }

        public static WindowManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        /// <summary>
        /// Base name to use for generated window names.
        /// </summary>
        public const string GeneratedWindowNameBase = "__cewin_uid_";

        /// <summary>
        /// Filename of the XML schema used for validating GUILayout files.
        /// </summary>
        public const string GuiLayoutSchemaName = "GUILayout.xsd";

        #region Events

        /// <summary>
        /// Event fired when a new <see cref="Window"/> object is created.
        /// Handlers are passed a <see cref="WindowEventArgs"/> reference with
        /// <see cref="Window"/> set to the <see cref="Window"/>
        /// that has just been created.
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowCreated;

        /// <summary>
        /// Event fired when a <see cref="Window"/> object is destroyed.
        /// Handlers are passed a <see cref="WindowEventArgs"/> reference with
        /// <see cref="Window"/> set to the <see cref="Window"/>
        /// that has been destroyed.
        /// </summary>
        public event EventHandler<WindowEventArgs> WindowDestroyed;
        
        #endregion

        /// <summary>
        /// Function type that is used as a callback when loading layouts from XML; the function is called
        /// for each Property element encountered.
        /// </summary>
        /// <param name="window">Window object that the property is to be applied to.</param>
        /// <param name="propname">String holding the name of the property that is being set.</param>
        /// <param name="propvalue">
        /// String holding the new value that will be applied to the property specified by /a propname.
        /// </param>
        /// <param name="userdata">Some client code supplied data.</param>
        /// <returns>
        /// - true if the property should be set.
        /// - false if the property should not be set,
        /// </returns>
        public delegate bool PropertyCallback(Window window, string propname, string propvalue, object userdata);
       
        /// <summary>
        /// Constructs a new WindowManager object.
        /// 
        /// NB: Client code should not create WindowManager objects - they are of limited use to you!  The
        /// intended pattern of access is to get a pointer to the GUI system's WindowManager via the System
        /// object, and use that.
        /// </summary>
        public WindowManager()
        {
            d_uid_counter = 0;
            d_lockCount = 0;

            System.GetSingleton().Logger
                .LogEvent("CEGUI::WindowManager singleton created " + GetHashCode().ToString("X8"));
        }

        /*!
        \brief
            Destructor for WindowManager objects

            This will properly destry all remaining Window objects.  Note that WindowFactory objects will not
            be destroyed (since they are owned by whoever created them).
        */
        // TODO: ~WindowManager(void);


        /*************************************************************************
            Window Related Methods
        *************************************************************************/
        /*!
        \brief
            

        \param type
        \param name
            

        \return
        \exception  InvalidRequestException WindowManager is locked and no Windows may be created.
        \exception	UnknownObjectException		
        \exception	GenericException			
        */
        /// <summary>
        /// Creates a new Window object of the specified type, and gives it the specified unique name.
        /// </summary>
        /// <param name="type">
        /// String that describes the type of Window to be created.
        /// A valid WindowFactory for the specified type must be registered.
        /// </param>
        /// <param name="name">
        /// String that holds the name that is to be given to the new window.  
        /// If \a name is empty, a name will be generated for the window.
        /// </param>
        /// <returns>
        /// Pointer to the newly created Window object.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// WindowManager is locked and no Windows may be created.
        /// </exception>
        /// <exception cref="UnknownObjectException">
        /// No WindowFactory is registered for \a type Window objects.
        /// </exception>
        /// <exception cref="Exception">
        /// Some other error occurred (Exception message has details).
        /// </exception>
        public Window CreateWindow(string type, string name = "")
        {
            // only allow creation of Window objects if we are in unlocked state
            if (IsLocked())
                throw new InvalidRequestException("WindowManager is in the locked state.");

            var finalName = String.IsNullOrEmpty(name) ? GenerateUniqueWindowName() : name;

            var wfMgr = WindowFactoryManager.GetSingleton();
            var factory = wfMgr.GetFactory(type);

            var  newWindow = factory.CreateWindow(finalName);

            System.GetSingleton().Logger
                  .LogEvent(
                      "Window '" + finalName + "' of type '" + type + "' has been created. " +
                      newWindow.GetHashCode().ToString("X8"), LoggingLevel.Informative);

            // see if we need to assign a look to this window
            if (wfMgr.IsFalagardMappedType(type))
            {
                var fwm = wfMgr.GetFalagardMappingForType(type);
                // this was a mapped type, so assign a look to the window so it can finalise
                // its initialisation
                newWindow.d_falagardType = type;
                newWindow.SetWindowRenderer(fwm.d_rendererType);
                newWindow.SetLookNFeel(fwm.d_lookName);

                InitialiseRenderEffect(newWindow, fwm.d_effectName);
            }

	        d_windowRegistry.Add(newWindow);

            // fire event to notify interested parites about the new window.
            var args =new WindowEventArgs(newWindow);
            var handler = WindowCreated;
            if (handler != null)
                handler(this, args);
    
	        return newWindow;
        }
        
        /// <summary>
        /// Destroy the specified Window object.
        /// </summary>
        /// <param name="window">
        /// Pointer to the Window object to be destroyed.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// Can be thrown if the WindowFactory for \a window's object type was removed.
        /// </exception>
        public void DestroyWindow(Window window)
        {
            var addrBuff = window.GetHashCode().ToString("X8");

            if (!d_windowRegistry.Contains(window))
            {
                System.GetSingleton()
                      .Logger.LogEvent(
                              "[WindowManager] Attempt to delete Window that does not exist!  Address was: " +
                              addrBuff +
                              ". WARNING: This could indicate a double-deletion issue!!",
                              LoggingLevel.Errors);
                return;
            }

            d_windowRegistry.Remove(window);

            System.GetSingleton().Logger
                .LogEvent("Window at '" + window.GetNamePath() + "' will be added to dead pool. " + addrBuff, LoggingLevel.Informative);

            // do 'safe' part of cleanup
            window.Destroy();

            // add window to dead pool
            d_deathrow.Add(window);

            // fire event to notify interested parites about window destruction.
            // TODO: Perhaps this should fire first, so window is still usable?
            var handler = WindowDestroyed;
            if (handler != null)
                handler(this, new WindowEventArgs(window));
        }

        /// <summary>
        /// Destroys all Window objects within the system.
        /// </summary>
        /// <exception cref="InvalidRequestException">
        /// Thrown if the WindowFactory for any Window object type has been removed.
        /// </exception>
        public void DestroyAllWindows()
        {
            while (d_windowRegistry.Count!=0)
                DestroyWindow(d_windowRegistry[0]);
        }

        /// <summary>
        /// return whether Window is alive.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool IsAlive(Window window)
        {
            return d_windowRegistry.Contains(window);
        }

        /*!
        \brief
            Creates a set of windows (a GUI layout) from the information in the specified XML.

        \param source
            RawDataContainer holding the XML source

        \param callback
            PropertyCallback function to be called for each Property element loaded from the layout.  This is
            called prior to the property value being applied to the window enabling client code manipulation of
            properties.

        \param userdata
            Client code data pointer passed to the PropertyCallback function.

        \return
            Pointer to the root Window object defined in the layout.
        */

        public Window LoadLayoutFromContainer(RawDataContainer source, PropertyCallback callback = null,
                                              object userdata = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a set of windows (a GUI layout) from the information in the specified XML file.
        /// </summary>
        /// <param name="filename">
        /// String object holding the filename of the XML file to be processed.
        /// </param>
        /// <param name="resourceGroup">
        /// Resource group identifier to be passed to the resource provider when loading the layout file.
        /// </param>
        /// <param name="callback">
        /// PropertyCallback function to be called for each Property element loaded from the layout.  This is
        /// called prior to the property value being applied to the window enabling client code manipulation of
        /// properties.
        /// </param>
        /// <param name="userdata">
        /// Client code data pointer passed to the PropertyCallback function.
        /// </param>
        /// <returns>
        /// Pointer to the root Window object defined in the layout.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if a filename appears to be invalid.
        /// </exception>
        public Window LoadLayoutFromFile(string filename,
                                         string resourceGroup = "",
                                         PropertyCallback callback = null,
                                         object userdata = null)
        {
            if (String.IsNullOrEmpty(filename))
                throw new InvalidRequestException("Filename supplied for gui-layout loading must be valid.");

            // log the fact we are about to load a layout
            System.GetSingleton()
                  .Logger.LogEvent("---- Beginning loading of GUI layout from '" + filename + "' ----",
                                   LoggingLevel.Informative);

            // create handler object
            var handler = new GuiLayoutXmlHandler(callback, userdata);

            // do parse (which uses handler to create actual data)
            try
            {
                System.GetSingleton()
                      .GetXMLParser()
                      .ParseXmlFile(handler, filename, GuiLayoutSchemaName,
                                    String.IsNullOrEmpty(resourceGroup) ? d_defaultResourceGroup : resourceGroup);
            }
            catch
            {
                System.GetSingleton()
                      .Logger.LogEvent(
                              "WindowManager::loadLayoutFromFile - loading of layout from file '" + filename +
                              "' failed.",
                              LoggingLevel.Errors);
                throw;
            }

            // log the completion of loading
            System.GetSingleton()
                  .Logger.LogEvent("---- Successfully completed loading of GUI layout from '" + filename + "' ----");

            return handler.GetLayoutRootWindow();
        }

        /// <summary>
        /// Creates a set of windows (a GUI layout) from the information in the specified XML.
        /// </summary>
        /// <param name="source">
        /// String holding the XML source
        /// </param>
        /// <param name="callback">
        /// PropertyCallback function to be called for each Property element loaded from the layout.  This is
        /// called prior to the property value being applied to the window enabling client code manipulation of
        /// properties.
        /// </param>
        /// <param name="userdata">
        /// Client code data pointer passed to the PropertyCallback function.
        /// </param>
        /// <returns>
        /// Pointer to the root Window object defined in the layout.
        /// </returns>
        public Window LoadLayoutFromString(string source, PropertyCallback callback = null, object userdata = null)
        {
            // log the fact we are about to load a layout
            System.GetSingleton()
                  .Logger.LogEvent("---- Beginning loading of GUI layout from string ----", LoggingLevel.Informative);

            // create handler object
            var handler = new GuiLayoutXmlHandler(callback, userdata);

            // do parse (which uses handler to create actual data)
            try
            {
                System.GetSingleton()
                      .GetXMLParser()
                      .ParseXmlString(handler, source, GuiLayoutSchemaName);
            }
            catch
            {
                System.GetSingleton()
                      .Logger.LogEvent("WindowManager::loadLayoutFromString - loading of layout from string failed.",
                                       LoggingLevel.Errors);
                throw;
            }

            // log the completion of loading
            System.GetSingleton()
                  .Logger.LogEvent("---- Successfully completed loading of GUI layout from string ----");

            return handler.GetLayoutRootWindow();
        }

        /*!
        \brief
            Return whether the window dead pool is empty.

        \return
            - true if there are no windows in the dead pool.
            - false if the dead pool contains >=1 window awaiting destruction.
        */

        public bool IsDeadPoolEmpty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Permanently destroys any windows placed in the dead pool.
        /// </summary>
        /// <remarks>
        /// It is probably not a good idea to call this from a Window based event handler
        /// if the specific window has been or is being destroyed.
        /// </remarks>
        public void CleanDeadPool()
        {
            var wfm = WindowFactoryManager.GetSingleton();
            foreach (var curr in Enumerable.Reverse(d_deathrow))
            {
#if DEBUG
                // in debug mode, log what gets cleaned from the dead pool (insane level)
                Logger.LogInsane("Window '" + curr.GetName() + "' about to be finally destroyed from dead pool.");
#endif
                wfm.GetFactory(curr.GetWidgetType())
                   .DestroyWindow(curr);
            }

            // all done here, so clear all pointers from dead pool
            d_deathrow.Clear();
        }

        /*!
        \brief
            Writes a full XML window layout, starting at the given Window to the given OutStream.

        \param window
            Window object to become the root of the layout.

        \param out_stream
            OutStream (std::ostream based) object where data is to be sent.

        \return
            Nothing.
        */

        public void WriteLayoutToStream(Window window, StreamWriter outStream)
        {
            var xml = new XMLSerializer(outStream);
            // output GUILayout start element
            xml.OpenTag(GuiLayoutXmlHandler.GuiLayoutElement);
            xml.Attribute(GuiLayoutXmlHandler.GuiLayoutVersionAttribute,
                          GuiLayoutXmlHandler.NativeVersion);

            // write windows
            window.WriteXMLToStream(xml);
            // write closing GUILayout element
            xml.CloseTag();
        }

        /// <summary>
        /// Writes a full XML window layout, starting at the given Window and returns the result as string
        /// </summary>
        /// <param name="window">
        /// Window object to become the root of the layout.
        /// </param>
        /// <returns>
        /// String containing XML of the resulting layout
        /// </returns>
        /// <remarks>
        /// This is a convenience function and isn't designed to be fast at all! Use the other alternatives
        /// if you want performance.
        /// </remarks>
        public string GetLayoutAsString(Window window)
        {
            using (var memoryStream = new MemoryStream(UInt16.MaxValue))
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    WriteLayoutToStream(window, streamWriter);
                }
                return global::System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /*!
        \brief
            Save a full XML window layout, starting at the given Window, to a file
            with the given file name.

        \param window
            Window object to become the root of the layout.

        \param filename
            The name of the file to which the XML will be written.  Note that this
            does not use any part of the ResourceProvider system, but rather will
            write directly to disk.  If this is not desirable, you should prefer the
            OutStream based writeWindowLayoutToStream functions.
        */

        public void SaveLayoutToFile(Window window, string filename)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Returns the default resource group currently set for layouts.

        \return
            String describing the default resource group identifier that will be
            used when loading layouts.
        */

        public static string GetDefaultResourceGroup()
        {
            return d_defaultResourceGroup;
        }

        /*!
        \brief
            Sets the default resource group to be used when loading layouts

        \param resourceGroup
            String describing the default resource group identifier to be used.

        \return
            Nothing.
        */

        public static void SetDefaultResourceGroup(string resourceGroup)
        {
            d_defaultResourceGroup = resourceGroup;
        }

        /// <summary>
        /// Put WindowManager into the locked state.
        /// <para>
        /// While WindowManager is in the locked state all attempts to create a
        /// Window of any type will fail with an InvalidRequestException being
        /// thrown.  Calls to lock/unlock are recursive; if multiple calls to lock
        /// are made, WindowManager is only unlocked after a matching number of
        /// calls to unlock.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is primarily intended for internal use within the system.
        /// </remarks>
        internal void Lock()
        {
            ++d_lockCount;
        }

        /// <summary>
        /// Put WindowManager into the unlocked state.
        /// <para>
        /// While WindowManager is in the locked state all attempts to create a
        /// Window of any type will fail with an InvalidRequestException being
        /// thrown.  Calls to lock/unlock are recursive; if multiple calls to lock
        /// are made, WindowManager is only unlocked after a matching number of
        /// calls to unlock.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is primarily intended for internal use within the system.
        /// </remarks>
        internal void Unlock()
        {
            if (d_lockCount > 0)
                --d_lockCount;
        }

        /// <summary>
        /// Returns whether WindowManager is currently in the locked state.
        /// 
        /// While WindowManager is in the locked state all attempts to create a
        /// Window of any type will fail with an InvalidRequestException being
        /// thrown.  Calls to lock/unlock are recursive; if multiple calls to lock
        /// are made, WindowManager is only unlocked after a matching number of
        /// calls to unlock.
        /// </summary>
        /// <returns>
        /// - true to indicate WindowManager is locked and that any attempt to
        ///   create Window objects will fail.
        /// - false to indicate WindowManager is unlocked and that Window objects
        ///   may be created as normal.
        /// </returns>
        internal bool IsLocked()
        {
            return d_lockCount != 0;
        }


        /// <summary>
        /// Implementation method to generate a unique name to use for a window.
        /// </summary>
        /// <returns></returns>
        private string GenerateUniqueWindowName()
        {
            var ret = GeneratedWindowNameBase + PropertyHelper.ToString(d_uid_counter);

            // update counter for next time
            var old_uid = d_uid_counter;
            ++d_uid_counter;

            // log if we ever wrap-around (which should be pretty unlikely)
            if (d_uid_counter < old_uid)
            {
                System.GetSingleton().Logger.
                       LogEvent(
                           "UID counter for generated Window names has wrapped around - the fun shall now commence!");
            }

            return ret;
        }

        /// <summary>
        /// function to set up RenderEffect on a window
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="effect"></param>
        private void InitialiseRenderEffect(Window wnd, string effect)
        {
            var logger = System.GetSingleton().Logger;

            // nothing to do if effect is empty string
            if (String.IsNullOrEmpty(effect))
                return;

            // if requested RenderEffect is not available, log that and continue
            if (!RenderEffectManager.GetSingleton().IsEffectAvailable(effect))
            {
                logger.LogEvent(
                    "Missing RenderEffect '" + effect + "' requested for window '" + wnd.GetName() +
                    "' - continuing without effect...", LoggingLevel.Errors);

                return;
            }

            // If we do not have a RenderingSurface, enable AutoRenderingSurface to
            // try and create one
            if (wnd.GetRenderingSurface() == null)
            {
                logger.LogEvent("Enabling AutoRenderingSurface on '" + wnd.GetName() + "' for RenderEffect support.");

                wnd.SetUsingAutoRenderingSurface(true);
            }

            // If we have a RenderingSurface and it's a RenderingWindow
            if (wnd.GetRenderingSurface()!=null && wnd.GetRenderingSurface().IsRenderingWindow())
            {
                // Set an instance of the requested RenderEffect
                ((RenderingWindow) wnd.GetRenderingSurface()).SetRenderEffect(
                    RenderEffectManager.GetSingleton().Create(effect, wnd));
            }
            // log fact that we could not get a usable RenderingSurface
            else
            {
                logger.LogEvent(
                    "Unable to set effect for window '" + wnd.GetName() +
                    "' since RenderingSurface is either missing or of wrong type (i.e. not a RenderingWindow).",
                    LoggingLevel.Errors);
            }
        }

        #region Fields

        /// <summary>
        /// collection of created windows.
        /// </summary>
        private readonly List<Window> d_windowRegistry = new List<Window>();

        /// <summary>
        /// Collection of 'destroyed' windows.
        /// </summary>
        private readonly List<Window> d_deathrow= new List<Window>();

        /// <summary>
        /// Counter used to generate unique window names.
        /// </summary>
        private ulong d_uid_counter;

        /// <summary>
        /// count of times WM is locked against new window creation.
        /// </summary>
        private uint d_lockCount;

        /// <summary>
        /// holds default resource group
        /// </summary>
        private static string d_defaultResourceGroup;

        #endregion

        //public:
        //    /*************************************************************************
        //        Iterator stuff
        //    *************************************************************************/
        //    typedef	ConstVectorIterator<WindowVector>	WindowIterator;

        //    /*!
        //    \brief
        //        Return a WindowManager::WindowIterator object to iterate over the currently defined Windows.
        //    */
        //    WindowIterator	getIterator(void) const;
        
        /// <summary>
        /// Outputs the names of ALL existing windows to log (DEBUG function).
        /// </summary>
        /// <param name="zone">
        /// Helper string that can specify where the name dump was made (e.g. "BEFORE FRAME DELETION").
        /// </param>
        public void DEBUG_dumpWindowNames(string zone)
        {
            var logger = System.GetSingleton().Logger;

            logger.LogEvent("WINDOW NAMES DUMP (" + zone + ")");
            logger.LogEvent("-----------------");

            foreach (var i in d_windowRegistry)
                logger.LogEvent("Window : " + i.GetNamePath());

            logger.LogEvent("-----------------");
        }
    }
}