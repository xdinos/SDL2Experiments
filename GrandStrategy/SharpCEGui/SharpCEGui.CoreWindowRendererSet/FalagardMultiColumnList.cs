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
    /// MultiColumnList class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Named Areas:
    ///     - ItemRenderingArea         - area where items will be drawn when no scrollbars are visible.
    ///     - ItemRenderingAreaHScroll  - area where items will be drawn when the horizontal scrollbar is visible.
    ///     - ItemRenderingAreaVScroll  - area where items will be drawn when the vertical scrollbar is visible.
    ///     - ItemRenderingAreaHVScroll - area where items will be drawn when both scrollbars are visible.
    /// 
    /// Child Widgets:
    ///     Scrollbar based widget with name suffix "__auto_vscrollbar__"   
    ///     Scrollbar based widget with name suffix "__auto_hscrollbar__"
    ///     ListHeader based widget with name suffix "__auto_listheader__"
    /// </summary>
    public class FalagardMultiColumnList : MultiColumnListWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/MultiColumnList";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardMultiColumnList(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Perform rendering of the widget control frame and other 'static'
        /// areas.  This method should not render the actual items.  Note that
        /// the items are typically rendered to layer 3, other layers can be
        /// used for rendering imagery behind and infront of the items.
        /// </summary>
        public void CacheListboxBaseImagery()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            // try and get imagery for our current state
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            // peform the rendering operation.
            imagery.Render(Window);
        }

        // overridden from MultiColumnList base class.
        public override Rectf GetListRenderArea()
        {
            var w = (MultiColumnList) Window;
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();
            var vertvisible = w.GetVertScrollbar().IsVisible();
            var horzVisible = w.GetHorzScrollbar().IsVisible();

            // if either of the scrollbars are visible, we might want to use another item rendering area
            if (vertvisible || horzVisible)
            {
                var areaName = "ItemRenderingArea";

                if (horzVisible)
                {
                    areaName += "H";
                }
                if (vertvisible)
                {
                    areaName += "V";
                }
                areaName += "Scroll";

                if (wlf.IsNamedAreaPresent(areaName))
                {
                    return wlf.GetNamedArea(areaName).GetArea().GetPixelRect(w);
                }
            }

            // default to plain ItemRenderingArea
            return wlf.GetNamedArea("ItemRenderingArea").GetArea().GetPixelRect(w);
        }

        // overridden from base class.
        public override void CreateRenderGeometry()
        {
            var w = (MultiColumnList) Window;
            var header = w.GetListHeader();
            var vertScrollbar = w.GetVertScrollbar();
            var horzScrollbar = w.GetHorzScrollbar();

            // render general stuff before we handle the items
            CacheListboxBaseImagery();

            //
            // Render list items
            //
            Lunatics.Mathematics.Vector3 itemPos;
            var itemRect = new Rectf();

            // calculate position of area we have to render into
            var itemsArea = GetListRenderArea();

            // set up initial positional details for items
            itemPos.Y = itemsArea.Top - vertScrollbar.GetScrollPosition();
            itemPos.Z = 0.0f;

            var alpha = w.GetEffectiveAlpha();

            // loop through the items
            for (var i = 0; i < w.GetRowCount(); ++i)
            {
                // set initial x position for this row.
                itemPos.X = itemsArea.Left - horzScrollbar.GetScrollPosition();

                // calculate height for this row.
                Sizef itemSize;
                itemSize.Height = w.GetHighestRowItemHeight(i);

                // loop through the columns in this row
                for (var j = 0; j < w.GetColumnCount(); ++j)
                {
                    // allow item to use full width of the column
                    itemSize.Width = CoordConverter.AsAbsolute(header.GetColumnWidth(j), header.GetPixelSize().Width);

                    var item = w.GetItemAtGridReference(new MCLGridRef(i, j));

                    // is the item for this column set?
                    if (item != null)
                    {
                        // calculate destination area for this item.
                        itemRect.Left = itemPos.X;
                        itemRect.Top = itemPos.Y;
                        itemRect.Size = itemSize;
                        var itemClipper = itemRect.GetIntersection(itemsArea);

                        // skip this item if totally clipped
                        if (itemClipper.Width == 0f)
                        {
                            itemPos.X += itemSize.Width;
                            continue;
                        }

                        // draw this item
                        item.CreateRenderGeometry(itemRect, alpha, itemClipper);
                    }

                    // update position for next column.
                    itemPos.X += itemSize.Width;
                }

                // update position ready for next row
                itemPos.Y += itemSize.Height;
            }
        }
    }
}