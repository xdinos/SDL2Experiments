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
    /// ItemEntry class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled           - basic rendering for enabled state.
    ///     - Disabled          - basic rendering for disabled state.
    /// 
    /// Optional states:
    ///     - SelectedEnabled   - basic rendering for enabled and selected state.
    ///     - SelectedDisabled  - basic rendering for disabled and selected state.
    /// 
    /// You only need to provide the 'Selected' states if the item will be selectable.
    /// If if the item is selected (which implies that it is selectable) only the SelectedEnabled
    /// state will be rendered.
    /// 
    /// Named areas:
    ///     - ContentSize
    /// </summary>
    public class FalagardItemEntry : ItemEntryWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/ItemEntry";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardItemEntry(string type) : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            var item = (ItemEntry) Window;
            
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            StateImagery imagery;
            // render basic imagery
            if (item.IsSelectable() && item.IsSelected())
            {
                imagery = wlf.GetStateImagery(item.IsEffectiveDisabled() ? "SelectedDisabled" : "SelectedEnabled");
            }
            else
            {
                imagery = wlf.GetStateImagery(item.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            }

            imagery.Render(Window);
        }

        public override Sizef GetItemPixelSize()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            return wlf.GetNamedArea("ContentSize").GetArea().GetPixelRect(Window).Size;
        }
    }
}