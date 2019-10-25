using System;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Provides a strategy for navigating the GUI based on a specified mapping.
    /// 
    /// For a brief tutorial on how to use the GUI navigation please refer
    /// to the @ref gui_navigation_tutorial
    /// </summary>
    public abstract class NavigationStrategy
    {
        // TODO: virtual ~NavigationStrategy() {}

        /// <summary>
        /// Returns a window based on its neighbour and a certain payload
        /// </summary>
        /// <param name="neighbour">
        /// The neighbour window relative to which the new window is requested
        /// </param>
        /// <param name="payload">
        /// A string payload value to help decide what window to return
        /// </param>
        /// <returns></returns>
        public abstract Window GetWindow(Window neighbour, string payload);
    }

    /// <summary>
    /// Provides a way of navigating the GUI by means of focusing windows
    /// 
    /// For a brief tutorial on how to use the GUI navigation please refer
    /// to the @ref gui_navigation_tutorial
    /// </summary>
    public class WindowNavigator
    {
        //typedef std::map<int, String> SemanticMappingsMap;

        // TODO: ~WindowNavigator() {}

        /// <summary>
        /// Creates a new navigator with the specified event <-> payload mapping and
        /// the specified strategy
        /// </summary>
        /// <param name="mappings">
        /// A mapping from semantic input events to certain strategy-specific payloads
        /// </param>
        /// <param name="strategy">
        /// The navigation strategy to be used
        /// </param>
        public WindowNavigator(Dictionary<int, string> mappings, NavigationStrategy strategy)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        ///  Handles the specified semantic input event and generate a navigation if
        /// that is the case (a mapping matches)
        /// </summary>
        /// <param name="event">
        /// The semantic input event
        /// </param>
        public void HandleSemanticEvent(SemanticInputEvent @event)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a new current focused window
        /// </summary>
        /// <param name="window">
        /// The window to be the new focused one
        /// </param>
        public void SetCurrentFocusedWindow(Window window)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current focused window
        /// </summary>
        /// <returns>
        /// An instance of Window
        /// </returns>
        public Window GetCurrentFocusedWindow()
        {
            throw new NotImplementedException();
        }

        private Dictionary<int, string> d_mappings;
        private NavigationStrategy d_strategy;

        private Window d_currentFocusedWindow;
    }
}