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
using System.Linq;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Base-class for the assignable WindowRenderer object
    /// </summary>
    public abstract class WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// Factory type name
        /// </param>
        /// <param name="className">
        /// The name of a widget class that is to be the minimum requirement for
        /// this window renderer.
        /// </param>
        protected WindowRenderer(string name, string className = "Window")
        {
            Window = null;
            d_name = name;
            d_class = className;
        }
        
        /// <summary>
        /// Populate render cache.
        /// 
        /// This method must be implemented by all window renderers and should
        /// perform the rendering operations needed for this widget.
        /// Normally using the Falagard API...
        /// </summary>
        public abstract void CreateRenderGeometry();

        /// <summary>
        /// Returns the factory type name of this window renderer.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return d_name;
        }

        /// <summary>
        /// Get the window this windowrenderer is attached to.
        /// </summary>
        /// <returns></returns>
        public Window GetWindow()
        {
            return Window;
        }

        /// <summary>
        /// Get the "minimum" Window class this renderer requires
        /// </summary>
        /// <returns></returns>
        public string GetClass()
        {
            return d_class;
        }

        /// <summary>
        /// Get the Look'N'Feel assigned to our window
        /// </summary>
        /// <returns></returns>
        public WidgetLookFeel GetLookNFeel()
        {
            return WidgetLookManager.GetSingleton().GetWidgetLook(Window.GetLookNFeel());
        }

        /// <summary>
        /// Get unclipped inner rectangle that our window should return from its
        /// member function with the same name.
        /// </summary>
        /// <returns></returns>
        public virtual Rectf GetUnclippedInnerRect()
        {
            var wlf = GetLookNFeel();

            if (wlf.IsNamedAreaPresent("inner_rect"))
                return wlf.GetNamedArea("inner_rect").GetArea().GetPixelRect(Window, Window.GetUnclippedOuterRect().Get());
            else
                return Window.GetUnclippedOuterRect().Get();
        }

        /// <summary>
        /// Method called to perform extended laying out of the window's attached
        /// child windows.
        /// </summary>
        public virtual void PerformChildWindowLayout()
        {
            // Empty
        }

        /// <summary>
        /// update the RenderingContext as needed for our window.  This is normally
        /// invoked via our window's member function with the same name.
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void GetRenderingContext(out RenderingContext ctx)
        {
            // default just calls back to the window implementation version.
            Window.GetRenderingContextImpl(out ctx);
        }

        /// <summary>
        /// perform any time based updates for this WindowRenderer.
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Update(float elapsed)
        {
            // Empty
        }

        /// <summary>
        /// Perform any updates needed because the given font's render size has
        /// changed.
        /// </summary>
        /// <param name="font">
        /// Pointer to the Font whose render size has changed.
        /// </param>
        /// <returns>
        /// - true if some action was taken.
        /// - false if no action was taken (i.e font is not used here).
        /// </returns>
        /// <remarks>
        /// This base implementation deals with updates needed for various
        /// definitions in the assigned widget look.  If you override, you should
        /// generally always call this base class implementation.
        /// </remarks>
        public virtual bool HandleFontRenderSizeChange(Font font)
        {
            var lf = GetLookNFeel();
            return lf.HandleFontRenderSizeChange(Window, font);
        }

        /// <summary>
        /// Register a property class that will be properly managed by this window
        /// renderer.
        /// </summary>
        /// <param name="property">
        /// Pointer to a static Property object that will be added to the target
        /// window.</param>
        /// <param name="banFromXml">
        /// - true if this property should be added to the 'ban' list so that it is
        ///   not written in XML output.
        /// - false if this property is not banned and should appear in XML output.
        /// </param>
        protected void RegisterProperty(Property property, bool banFromXml)
        {
            d_properties.Add(new Tuple<Property, bool>(property, banFromXml));
        }

        /// <summary>
        /// Register a property class that will be properly managed by this window
        /// renderer.
        /// </summary>
        /// <param name="property">
        /// Pointer to a static Property object that will be added to the target 
        /// window.
        /// </param>
        protected void RegisterProperty(Property property)
        {
            RegisterProperty(property, false);
        }

        /// <summary>
        /// Handler called when this windowrenderer is attached to a window
        /// </summary>
        protected internal virtual void OnAttach()
        {
            foreach (var i in d_properties)
            {
                Window.AddProperty(i.Item1);

                // ban from xml if neccessary
                if (i.Item2)
                    Window.BanPropertyFromXML(i.Item1);
            }
        }

        /// <summary>
        /// Handler called when this windowrenderer is detached from its window
        /// </summary>
        protected internal virtual void OnDetach()
        {
            foreach (var i in d_properties.Reverse<Tuple<Property,bool>>())
            {
                // unban from xml if neccessary
                if (i.Item2)
                    Window.UnbanPropertyFromXML(i.Item1);

                Window.RemoveProperty(i.Item1.GetName());
            }
        }

        /// <summary>
        /// Handler called when a Look'N'Feel is assigned to our window.
        /// </summary>
        protected internal virtual void OnLookNFeelAssigned()
        {
            // Empty implementation
        }

        /// <summary>
        /// Handler called when a Look'N'Feel is removed/unassigned from our window.
        /// </summary>
        protected internal virtual void OnLookNFeelUnassigned()
        {
            // Empty implementation
        }

        protected internal Window Window; //!< Pointer to the window this windowrenderer is assigned to.
        protected string d_name; //!< Name of the factory type used to create this window renderer.
        protected string d_class; //!< Name of the widget class that is the "minimum" requirement.

        ////! type used for entries in the PropertyList.
        //typedef std::pair<Property*, bool> PropertyEntry;

        ////! type to use for the property list.
        //typedef std::vector<PropertyEntry,CEGUI_VECTOR_ALLOC(PropertyEntry)> PropertyList;

        /// <summary>
        /// The list of properties that this windowrenderer will be handling.
        /// </summary>
        protected List<Tuple<Property, bool>> d_properties = new List<Tuple<Property, bool>>();
        

        // Window is friend so it can manipulate our 'd_window' member directly.
        // We don't want users fiddling with this so no public interface.
        // friend class Window;
        //private WindowRenderer& operator=(const WindowRenderer&) { return *this; }
    }
}