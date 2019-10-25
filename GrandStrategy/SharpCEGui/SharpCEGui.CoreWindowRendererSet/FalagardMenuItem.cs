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

using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// MenuItem class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States (missing states will default to '***Normal'):
    ///     - EnabledNormal
    ///     - EnabledHover
    ///     - EnabledPushed
    ///     - EnabledPushedOff
    ///     - EnabledPopupOpen
    ///     - DisabledNormal
    ///     - DisabledHover
    ///     - DisabledPushed
    ///     - DisabledPushedOff
    ///     - DisabledPopupOpen
    ///     - PopupClosedIcon   - Additional state drawn when item has a pop-up attached (in closed state)
    ///     - PopupOpenIcon     - Additional state drawn when item has a pop-up attached (in open state)
    /// 
    /// Named Areas:
    ///     ContentSize
    ///     HasPopupContentSize
    /// </summary>
    public class FalagardMenuItem : ItemEntryWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/MenuItem";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardMenuItem(string type)
            : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            var w = (MenuItem) Window;
            // build name of state we're in
            var stateName = w.IsEffectiveDisabled() ? "Disabled" : "Enabled";

            string suffix;

            // only show opened imagery if the menu items popup window is not closing
            // (otherwise it might look odd)
            if (w.IsOpened() && !(w.HasAutoPopup() && w.IsPopupClosing()))
                suffix = "PopupOpen";
            else if (w.IsPushed())
                suffix = w.IsHovering() ? "Pushed" : "PushedOff";
            else if (w.IsHovering())
                suffix = "Hover";
            else
                suffix = "Normal";

            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // try and get imagery for our current state
            var imagery = wlf.IsStateImageryPresent(stateName + suffix)
                              ? wlf.GetStateImagery(stateName + suffix)
                              : wlf.GetStateImagery(stateName + "Normal");

            // peform the rendering operation.
            imagery.Render(w);

            // only draw popup-open/closed-icon if we have a popup menu, and parent is not a menubar
            var parentWindow = w.GetParent();
            var notMenubar = (parentWindow == null) || (parentWindow as Menubar) == null;

            if (w.GetPopupMenu() != null && notMenubar)
            {
                // get imagery for popup open/closed state
                imagery = wlf.GetStateImagery(w.IsOpened() ? "PopupOpenIcon" : "PopupClosedIcon");
                // peform the rendering operation.
                imagery.Render(w);
            }
        }

        public override Sizef GetItemPixelSize()
        {
            return GetContentNamedArea().GetArea().GetPixelRect(Window).Size;
        }

        public override bool HandleFontRenderSizeChange(Font font)
        {
            if (GetContentNamedArea().HandleFontRenderSizeChange(Window, font))
            {
                var parent = Window.GetParent();
                if (parent != null)
                    parent.PerformChildWindowLayout();

                return true;
            }

            return false;
        }

        protected NamedArea GetContentNamedArea()
        {
            var wlf = GetLookNFeel();

            if (((MenuItem) Window).GetPopupMenu() != null && !ParentIsMenubar() &&
                wlf.IsNamedAreaPresent("HasPopupContentSize"))
            {
                return wlf.GetNamedArea("HasPopupContentSize");
            }

            return wlf.GetNamedArea("ContentSize");
        }

        protected bool ParentIsMenubar()
        {
            var parent = Window.GetParent();
            return parent != null && (parent as Menubar) != null;
        }
    }
}