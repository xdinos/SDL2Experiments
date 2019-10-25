using System;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class GuiLayoutXmlHandler : XmlHandler
    {
        // note: The assets' versions aren't usually the same as CEGUI version, they are versioned from version 1 onwards!
        public const string NativeVersion = "4"; //!< The only version that we will allow to load

        /// <summary>
        /// Constructor for GUILayout_xmlHandler objects
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userdata"></param>
        public GuiLayoutXmlHandler(WindowManager.PropertyCallback callback = null, object userdata = null)
        {
            d_root = null;
            d_propertyCallback = callback;
            d_userData = userdata;
        }

        // TODO: Destructor for GUILayout_xmlHandler objects
        // TODO: virtual ~GUILayout_xmlHandler(void) {}

        public override string GetSchemaName()
        {
            return WindowManager.GuiLayoutSchemaName;
        }

        public override string GetDefaultResourceGroup()
        {
            return WindowManager.GetDefaultResourceGroup();
        }

        /// <summary>
        /// document processing (only care about elements, schema validates format)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributes"></param>
        public override void ElementStart(string element, XMLAttributes attributes)
        {
            // handle root GUILayoutElement element
            if (element == GuiLayoutElement)
            {
                ElementGUILayoutStart(attributes);
            }
                    // handle Window element (create window and make an entry on our "window stack")
            else if (element == Window.WindowXMLElementName)
            {
                ElementWindowStart(attributes);
            }
                    // handle AutoWindow element (get an auto child from the window on top of the "window stack")
            else if (element == Window.AutoWindowXMLElementName)
            {
                ElementAutoWindowStart(attributes);
            }
                    // handle UserString element (set user string for window at top of stack)
            else if (element == Window.UserStringXMLElementName)
            {
                ElementUserStringStart(attributes);
            }
                    // handle Property element (set property for window at top of stack)
            else if (element == Property.XMLElementName)
            {
                ElementPropertyStart(attributes);
            }
                    // handle layout import element (attach a layout to the window at the top of the stack)
            else if (element == LayoutImportElement)
            {
                elementLayoutImportStart(attributes);
            }
                    // handle event subscription element
            else if (element == EventElement)
            {
                elementEventStart(attributes);
            }
                    // anything else is an error which *should* have already been caught by XML validation
            else
            {
                System.GetSingleton().Logger
                      .LogEvent(
                              "GuiLayoutXmlHandler::startElement - Unexpected data was found while parsing the gui-layout file: '" +
                              element + "' is unknown.", LoggingLevel.Errors);
            }
        }

        public override void ElementEnd(string element)
        {
            //if (element == GUILayoutElement)
            //{
            //	NOOP
            //}

            // handle Window element
            if (element == Window.WindowXMLElementName)
            {
                ElementWindowEnd();
            }
                    // handle Window element
            else if (element == Window.AutoWindowXMLElementName)
            {
                ElementAutoWindowEnd();
            }
                    // handle UserString element
            else if (element == Window.UserStringXMLElementName)
            {
                ElementUserStringEnd();
            }
                    // handle Property element
            else if (element == Property.XMLElementName)
            {
                ElementPropertyEnd();
            }
        }

        public override void Text(string text)
        {
            d_stringItemValue += text;
        }

        /// <summary>
        /// Destroy all windows created so far.
        /// </summary>
        public void CleanupLoadedWindows()
        {
            // Notes: We could just destroy the root window of the layout, which normally would also destroy
            // all attached windows.  Since the client may have specified that certain windows are not auto-destroyed
            // we can't rely on this, so we work backwards detaching and deleting windows instead.
            while (d_stack.Count!=0)
            {
                // only destroy if not an auto window
                var back = d_stack[d_stack.Count - 1];
                if (back.Item2)
                {
                    var wnd = back.Item1;

                    // detach from parent
                    if (wnd.GetParent()!=null)
                    {
                        wnd.GetParent().RemoveChild(wnd);
                    }

                    // destroy the window
                    WindowManager.GetSingleton().DestroyWindow(wnd);
                }

                // pop window from stack
                d_stack.RemoveAt(d_stack.Count-1);
            }

            d_root = null;
        }
        
        /// <summary>
        /// Return a pointer to the 'root' window created.
        /// </summary>
        /// <returns></returns>
        public Window GetLayoutRootWindow()
        {
            return d_root;
        }

        /// <summary>
        /// Tag name for GUILayout elements.
        /// </summary>
        public const string GuiLayoutElement = "GUILayout";

        /// <summary>
        /// Tag name for LayoutImport elements.
        /// </summary>
        public const string LayoutImportElement = "LayoutImport";

        /// <summary>
        /// Tag name for Event elements.
        /// </summary>
        public const string EventElement = "Event";

        /// <summary>
        /// Attribute name that stores the file name of the layout to import.
        /// </summary>
        public const string LayoutImportFilenameAttribute = "filename";

        /// <summary>
        /// Attribute name that stores the resource group identifier used when loading imported file.
        /// </summary>
        public const string LayoutImportResourceGroupAttribute = "resourceGroup";
                            

        /// <summary>
        /// Attribute name that stores the event name to be subscribed.
        /// </summary>
        public const string EventNameAttribute = "name";
                            

        /// <summary>
        /// Attribute name that stores the name of the scripted function to be bound.
        /// </summary>
        public const string EventFunctionAttribute = "function";
                            

        public const string GuiLayoutVersionAttribute = "version"; //!< Attribute name that stores the xml file version.

        /// <summary>
        /// Method that handles the opening GUILayout XML element.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This method just checks the version attribute and there is no equivalent
        /// elementGUILayoutEnd method, because it would be just NOOP anyways
        /// </remarks>
        private void ElementGUILayoutStart(XMLAttributes attributes)
        {
            var version = attributes.GetValueAsString(GuiLayoutVersionAttribute, "unknown");

            if (version != NativeVersion)
            {
                throw new InvalidRequestException(
                        "You are attempting to load a layout of version '" + version +
                        "' but this CEGUI version is only meant to load layouts of " +
                        "version '" + NativeVersion + "'. Consider using the " +
                        "migrate.py script bundled with CEGUI Unified Editor to " +
                        "migrate your data.");
            }
        }

        /// <summary>
        /// Method that handles the opening Window XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementWindowStart(XMLAttributes attributes)
        {
            // get type of window to create
            var windowType = attributes.GetValueAsString(Window.WindowTypeXMLAttributeName);

            // get name for new window
            var windowName = attributes.GetValueAsString(Window.WindowNameXMLAttributeName);

            // attempt to create window
            try
            {
                var wnd = WindowManager.GetSingleton().CreateWindow(windowType, windowName);

                // add this window to the current parent (if any)
                if (d_stack.Count != 0)
                    d_stack[d_stack.Count - 1].Item1.AddChild(wnd);
                else
                    d_root = wnd;

                // make this window the top of the stack
                d_stack.Add(new Tuple<Window, bool>(wnd, true));

                // tell it that it is being initialised
                wnd.BeginInitialisation();
            }
            catch (AlreadyExistsException)
            {
                // delete all windows created
                CleanupLoadedWindows();

                // signal error - with more info about what we have done.
                throw new InvalidRequestException("layout loading has been aborted since Window named '" + windowName +
                                                  "' already exists.");
            }
            catch (UnknownObjectException)
            {
                // delete all windows created
                CleanupLoadedWindows();

                // signal error - with more info about what we have done.
                throw new InvalidRequestException(
                        "layout loading has been aborted since no WindowFactory is available for '" + windowType +
                        "' objects.");
            }
        }

        /// <summary>
        /// Method that handles the opening AutoWindow XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementAutoWindowStart(XMLAttributes attributes)
        {
            // get window name
            var name_path = attributes.GetValueAsString(Window.AutoWindowNamePathXMLAttributeName);

            try
            {
                // we need a window to fetch children
                if (d_stack.Count != 0)
                {
                    var wnd = d_stack[d_stack.Count - 1].Item1.GetChild(name_path);
                    // make this window the top of the stack
                    d_stack.Add(new Tuple<Window, bool>(wnd, false));
                }
            }
            catch (UnknownObjectException)
            {
                // delete all windows created
                CleanupLoadedWindows();

                // signal error - with more info about what we have done.
                throw new InvalidRequestException("layout loading has been aborted since auto window '" + name_path +
                                                  "' could not be referenced.");
            }
        }

        /// <summary>
        /// Method that handles the UserString XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementUserStringStart(XMLAttributes attributes)
        {
            // Get user string name
            var userStringName = attributes.GetValueAsString(Window.UserStringNameXMLAttributeName);

            // Get user string value
            var userStringValue = String.Empty;
            if (attributes.Exists(Window.UserStringValueXMLAttributeName))
                userStringValue = attributes.GetValueAsString(Window.UserStringValueXMLAttributeName);

            // Short user string
            if (!String.IsNullOrEmpty(userStringValue))
            {
                d_stringItemName = null;
                try
                {
                    // need a window to be able to set properties!
                    if (d_stack.Count != 0)
                    {
                        // get current window being defined.
                        var curwindow = d_stack[d_stack.Count - 1].Item1;

                        curwindow.SetUserString(userStringName, userStringValue);
                    }
                }
                catch (Exception)
                {
                    // Don't do anything here, but the error will have been logged.
                }
            }
                    // Long user string
            else
            {
                // Store name for later use
                d_stringItemName = userStringName;
                // reset the property (user string) value buffer
                d_stringItemValue = null;
            }
        }

        /// <summary>
        /// Method that handles the Property XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementPropertyStart(XMLAttributes attributes)
        {
            // get property name
            var propertyName = attributes.GetValueAsString(Property.NameXMLAttributeName);

            // get property value string
            var propertyValue = String.Empty;
            if (attributes.Exists(Property.ValueXMLAttributeName))
                propertyValue = attributes.GetValueAsString(Property.ValueXMLAttributeName);

            // Short property 
            if (!String.IsNullOrEmpty(propertyValue))
            {
                d_stringItemName = String.Empty;
                try
                {
                    // need a window to be able to set properties!
                    if (d_stack.Count != 0)
                    {
                        // get current window being defined.
                        var curwindow = d_stack[d_stack.Count - 1].Item1;

                        var useit = true;

                        // if client defined a callback, call it and discover if we should
                        // set the property.
                        if (d_propertyCallback != null)
                        {
                            useit = d_propertyCallback(curwindow,
                                                       propertyName,
                                                       propertyValue,
                                                       d_userData);
                        }
                        // set the property as needed
                        if (useit)
                            curwindow.SetProperty(propertyName, propertyValue);
                    }
                }
                catch (Exception)
                {
                    // Don't do anything here, but the error will have been logged.
                }
            }
                    // Long property 
            else
            {
                // Store name for later use 
                d_stringItemName = propertyName;
                // reset the property value buffer
                d_stringItemValue = String.Empty;
            }
        }

        /// <summary>
        /// Method that handles the LayoutImport XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void elementLayoutImportStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that handles the Event XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void elementEventStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that handles the closing Window XML element.
        /// </summary>
        private void ElementWindowEnd()
        {
            // pop a window from the window stack
            if (d_stack.Count != 0)
            {
                d_stack[d_stack.Count - 1].Item1.EndInitialisation();
                d_stack.RemoveAt(d_stack.Count - 1);
            }
        }

        /// <summary>
        /// Method that handles the closing AutoWindow XML element.
        /// </summary>
        private void ElementAutoWindowEnd()
        {
            // pop a window from the window stack
            if (d_stack.Count != 0)
            {
                d_stack.RemoveAt(d_stack.Count - 1);
            }
        }

        /// <summary>
        /// Method that handles the closing of a UserString XML element.
        /// </summary>
        private void ElementUserStringEnd()
        {
            // only do something if this is a "long" user string
            if (String.IsNullOrEmpty(d_stringItemName))
                return;

            try
            {
                // need a window to be able to set user strings!
                if (d_stack.Count != 0)
                {
                    // get current window being defined.
                    var curwindow = d_stack[d_stack.Count - 1].Item1;

                    curwindow.SetUserString(d_stringItemName, d_stringItemValue);
                }
            }
            catch (Exception)
            {
                // Don't do anything here, but the error will have been logged.
            }
        }

        /// <summary>
        /// Method that handles the closing of a property XML element.
        /// </summary>
        private void ElementPropertyEnd()
        {
            // only do something if this is a "long" property
            if (String.IsNullOrEmpty(d_stringItemName))
                return;

            try
            {
                // need a window to be able to set properties!
                if (d_stack.Count != 0)
                {
                    // get current window being defined.
                    var curwindow = d_stack[d_stack.Count - 1].Item1;

                    var useit = true;

                    // if client defined a callback, call it and discover if we should
                    // set the property.
                    if (d_propertyCallback != null)
                    {
                        useit = d_propertyCallback(curwindow,
                                                   d_stringItemName,
                                                   d_stringItemValue,
                                                   d_userData);
                    }
                    // set the property as needed
                    if (useit)
                        curwindow.SetProperty(d_stringItemName, d_stringItemValue);
                }
            }
            catch (Exception)
            {
                // Don't do anything here, but the error will have been logged.
            }
        }

        // TODO: void operator=(const GUILayout_xmlHandler&) {}

        //typedef std::pair<Window*, bool> WindowStackEntry; //!< Pair used as datatype for the window stack. second is false if the window is an autowindow.
        //typedef std::vector<WindowStackEntry CEGUI_VECTOR_ALLOC(WindowStackEntry)> WindowStack;

        private Window d_root; //!< Will point to first window created.
        
        private string d_stringItemName; //!< Use for long property or user string value

        private string d_stringItemValue; //!< Use for long property or user string value

        /// <summary>
        /// Callback for every property loaded
        /// </summary>
        private readonly WindowManager.PropertyCallback d_propertyCallback; 

        /// <summary>
        /// User data for the property callback
        /// </summary>
        private readonly object d_userData;

        /// <summary>
        /// Stack used to keep track of what we're doing to which window. 
        /// </summary>
        private readonly List<Tuple<Window, bool>> d_stack = new List<Tuple<Window, bool>>();
    }
}