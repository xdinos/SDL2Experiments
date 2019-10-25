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
    /// Tree class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Named Areas:
    ///     - ItemRenderingArea
    ///     - ItemRenderingAreaHScroll
    ///     - ItemRenderingAreaVScroll
    ///     - ItemRenderingAreaHVScroll
    /// 
    /// Child Widgets:
    ///     Scrollbar based widget with name suffix "__auto_vscrollbar__"
    ///     Scrollbar based widget with name suffix "__auto_hscrollbar__"
    /// </summary>
    public class FalagardTree : WindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Tree";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardTree(string type)
            : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            var tree = (Tree) Window;
            //Set the render area for this.
            var rect = GetTreeRenderArea();
            tree.SetItemRenderArea(rect);
            var wlf = GetLookNFeel();

            //Get the Falagard imagery to render
            var imagery = wlf.GetStateImagery(tree.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            //Render the window
            imagery.Render(tree);
            //Fix Scrollbars
            tree.DoScrollbars();
            //Render the tree.
            tree.DoTreeRender();
        }

        private Rectf GetTreeRenderArea()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            var tree = (Tree) Window;

            var vertVisible = tree.GetVertScrollbar().IsVisible();
            var horzVisible = tree.GetHorzScrollbar().IsVisible();

            // if either of the scrollbars are visible, we might want to use another text rendering area
            if (vertVisible || horzVisible)
            {
                var areaName = "ItemRenderingArea";

                if (horzVisible)
                {
                    areaName += "H";
                }
                if (vertVisible)
                {
                    areaName += "V";
                }
                areaName += "Scroll";

                if (wlf.IsNamedAreaPresent(areaName))
                {
                    return wlf.GetNamedArea(areaName).GetArea().GetPixelRect(tree);
                }
            }

            // default to plain ItemRenderingArea
            return wlf.GetNamedArea("ItemRenderingArea").GetArea().GetPixelRect(tree);
        }
    }
}