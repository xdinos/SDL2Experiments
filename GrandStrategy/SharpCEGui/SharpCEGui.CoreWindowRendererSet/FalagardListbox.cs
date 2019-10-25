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
using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Listbox class for the FalagardBase module.
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
    ///     - - OR -
    ///     - ItemRenderArea
    ///     - ItemRenderAreaHScroll
    ///     - ItemRenderAreaVScroll
    ///     - ItemRenderAreaHVScroll
    ///     
    /// Child Widgets:
    ///     Scrollbar based widget with name suffix "__auto_vscrollbar__"
    ///     Scrollbar based widget with name suffix "__auto_hscrollbar__"
    /// </summary>
    public class FalagardListbox : ListboxWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Listbox";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardListbox(string type)
            : base(type)
        {

        }

        /// <summary>
        /// Perform caching of the widget control frame and other 'static' areas.  This
        /// method should not render the actual items.  Note that the items are typically
        /// rendered to layer 3, other layers can be used for rendering imagery behind and
        /// infront of the items.
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

        // overriden from ListboxWindowRenderer
        public override void CreateRenderGeometry()
        {
            var lb = (Listbox) Window;

            // render frame and stuff before we handle the items
            CacheListboxBaseImagery();

            //
            // Render list items
            //
            Lunatics.Mathematics.Vector3 itemPos;
            var itemRect = new Rectf();
            var widest = lb.GetWidestItemWidth();

            // calculate position of area we have to render into
            var itemsArea = GetListRenderArea();

            // set up some initial positional details for items
            itemPos.X = itemsArea.Left - lb.GetHorzScrollbar().GetScrollPosition();
            itemPos.Y = itemsArea.Top - lb.GetVertScrollbar().GetScrollPosition();
            itemPos.Z = 0.0f;

            var alpha = lb.GetEffectiveAlpha();

            // loop through the items
            var itemCount = lb.GetItemCount();

            for (var i = 0; i < itemCount; ++i)
            {
                var listItem = lb.GetListboxItemFromIndex(i);
                Sizef itemSize;
                itemSize.Height = listItem.GetPixelSize().Height;

                // allow item to have full width of box if this is wider than items
                itemSize.Width = Math.Max(itemsArea.Width, widest);

                // calculate destination area for this item.
                itemRect.Left = itemPos.X;
                itemRect.Top = itemPos.Y;
                itemRect.Size = itemSize;
                var itemClipper = itemRect.GetIntersection(itemsArea);

                // skip this item if totally clipped
                if (Math.Abs(itemClipper.Width - 0) < float.Epsilon)
                {
                    itemPos.Y += itemSize.Height;
                    continue;
                }

                // draw this item
                Window.AppendGeometryBuffers(listItem.CreateRenderGeometry(itemRect, alpha, itemClipper));

                // update position ready for next item
                itemPos.Y += itemSize.Height;
            }
        }

        public override Rectf GetListRenderArea()
        {
            var lb = (Listbox) Window;

            return GetItemRenderingArea(lb.GetHorzScrollbar().IsVisible(),
                                        lb.GetVertScrollbar().IsVisible());
        }

        public override void ResizeListToContent(bool fitWidth, bool fitHeight)
        {
            var lb = (Listbox) Window;

            var totalArea = lb.GetUnclippedOuterRect().Get();
            var contentArea = GetItemRenderingArea(!fitWidth && lb.GetHorzScrollbar().IsVisible(),
                                                   !fitHeight && lb.GetVertScrollbar().IsVisible());

            var withScrollContentArea = GetItemRenderingArea(true, true);

            var frameSize = totalArea.Size - contentArea.Size;
            var withScrollFrameSize = totalArea.Size - withScrollContentArea.Size;
            var contentSize = new Sizef(lb.GetWidestItemWidth(),
                                        lb.GetTotalItemsHeight());

            var parentSize = lb.GetParentPixelSize();
            var maxSize =
                new Sizef(parentSize.Width - CoordConverter.AsAbsolute(lb.GetXPosition(), parentSize.Width),
                          parentSize.Height - CoordConverter.AsAbsolute(lb.GetYPosition(), parentSize.Height));

            var requiredSize = frameSize + contentSize + new Sizef(1, 1);

            if (fitHeight)
            {
                if (requiredSize.Height > maxSize.Height)
                {
                    requiredSize.Height = maxSize.Height;
                    requiredSize.Width = Math.Min(maxSize.Width,
                                                    requiredSize.Width - frameSize.Width +
                                                    withScrollFrameSize.Width);
                }
            }

            if (fitWidth)
            {
                if (requiredSize.Width > maxSize.Width)
                {
                    requiredSize.Width = maxSize.Width;
                    requiredSize.Height = Math.Min(maxSize.Height,
                                                     requiredSize.Height - frameSize.Height +
                                                     withScrollFrameSize.Height);
                }
            }

            if (fitHeight)
                lb.SetHeight(new UDim(0, requiredSize.Height));

            if (fitWidth)
                lb.SetWidth(new UDim(0, requiredSize.Width));
        }

        public override bool HandleFontRenderSizeChange(Font font)
        {
            var res = base.HandleFontRenderSizeChange(font);

            if (!res)
            {
                var listbox = (Listbox) Window;

                for (var i = 0; i < listbox.GetItemCount(); ++i)
                    res |= listbox.GetListboxItemFromIndex(i).HandleFontRenderSizeChange(font);

                if (res)
                    listbox.Invalidate(false);
            }

            return res;
        }

        protected Rectf GetItemRenderingArea(bool hscroll, bool vscroll)
        {
            var lb = (Listbox) Window;
            var wlf = GetLookNFeel();
            const string areaName = "ItemRenderingArea";
            const string alternateName = "ItemRenderArea";
            var scrollSuffix = vscroll ? hscroll ? "HVScroll" : "VScroll" : hscroll ? "HScroll" : "";

            if (wlf.IsNamedAreaPresent(areaName + scrollSuffix))
                return wlf.GetNamedArea(areaName + scrollSuffix).GetArea().GetPixelRect(lb);

            if (wlf.IsNamedAreaPresent(alternateName + scrollSuffix))
                return wlf.GetNamedArea(alternateName + scrollSuffix).GetArea().GetPixelRect(lb);

            // default to plain ItemRenderingArea
            return wlf.IsNamedAreaPresent(areaName)
                       ? wlf.GetNamedArea(areaName).GetArea().GetPixelRect(lb)
                       : wlf.GetNamedArea(alternateName).GetArea().GetPixelRect(lb);
        }
    }
}