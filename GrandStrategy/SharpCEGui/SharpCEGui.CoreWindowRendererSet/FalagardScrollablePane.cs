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
    /// ScrollablePane class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Named Areas:
    ///     - ViewableArea         - area where content will be drawn when no scrollbars are visible.
    ///     - ViewableAreaHScroll  - area where content will be drawn when the horizontal scrollbar is visible.
    ///     - ViewableAreaVScroll  - area where content will be drawn when the vertical scrollbar is visible.
    ///     - ViewableAreaHVScroll - area where content will be drawn when both scrollbars are visible.
    /// 
    /// Child Widgets:
    ///     Scrollbar based widget with name suffix "__auto_vscrollbar__"
    ///     Scrollbar based widget with name suffix "__auto_hscrollbar__"
    /// </summary>
    public class FalagardScrollablePane : ScrollablePaneWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/ScrollablePane";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardScrollablePane(string type)
            : base(type)
        {
            _widgetLookAssigned = false;
        }

        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            // try and get imagery for our current state
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            // peform the rendering operation.
            imagery.Render(Window);
        }

        public override Rectf GetViewableArea()
        {
            var w = (ScrollablePane) Window;

            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            var vVisible = w.GetVertScrollbar().IsVisible();
            var hVisible = w.GetHorzScrollbar().IsVisible();

            // if either of the scrollbars are visible, we might want to use another text rendering area
            if (vVisible || hVisible)
            {
                var areaName = "ViewableArea";

                if (hVisible)
                {
                    areaName += "H";
                }
                if (vVisible)
                {
                    areaName += "V";
                }
                areaName += "Scroll";

                if (wlf.IsNamedAreaPresent(areaName))
                {
                    return wlf.GetNamedArea(areaName).GetArea().GetPixelRect(w);
                }
            }

            // default to plain ViewableArea
            return wlf.GetNamedArea("ViewableArea").GetArea().GetPixelRect(w);
        }

        public override Rectf GetUnclippedInnerRect()
        {
            if (!_widgetLookAssigned)
                return Window.GetUnclippedOuterRect().Get();

            var lr = GetViewableArea();
            lr.Offset(Window.GetUnclippedOuterRect().Get().d_min);
            return lr;
        }

        protected override void OnLookNFeelAssigned()
        {
            _widgetLookAssigned = true;
        }

        protected override void OnLookNFeelUnassigned()
        {
            _widgetLookAssigned = false;
        }

        /// <summary>
        /// flag whether target window has looknfeel assigned yet.
        /// </summary>
        private bool _widgetLookAssigned;
    }
}