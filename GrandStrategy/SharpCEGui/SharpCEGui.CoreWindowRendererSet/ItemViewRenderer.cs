using System;
using SharpCEGui.Base;
using SharpCEGui.Base.Views;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Base class that contains common routines for view renderers
    /// </summary>
    public class ItemViewRenderer
    {
        // TOOD: public virtual ~ItemViewRenderer();

        public virtual Rectf GetViewRenderArea(ItemView itemView)
        {
            return GetViewRenderArea(itemView, itemView.GetHorzScrollbar().IsVisible(), itemView.GetVertScrollbar().IsVisible());
        }

        public Rectf GetViewRenderArea(ItemView itemView, bool hscroll, bool vscroll)
        {
            var wlf = WidgetLookManager.GetSingleton().GetWidgetLook(itemView.GetLookNFeel());
            var scrollSuffix = string.Empty;

            if (vscroll)
                scrollSuffix += "V";

            if (hscroll)
                scrollSuffix += "H";

            if (!String.IsNullOrEmpty(scrollSuffix))
                scrollSuffix += "Scroll";

            var areaNames = new[] {"ItemRenderingArea", "ItemRenderArea"};
            var suffixes = new[] {scrollSuffix, ""};

            for (var suffixId = 0; suffixId < 2; suffixId++)
            {
                var suffix = suffixes[suffixId];

                for (var areaId = 0; areaId < 2; ++areaId)
                {
                    var fullAreaName = areaNames[areaId] + suffix;

                    if (wlf.IsNamedAreaPresent(fullAreaName))
                        return wlf.GetNamedArea(fullAreaName).GetArea().GetPixelRect(itemView);
                }
            }

            throw new UnknownObjectException("There is no item rendering area defined!");
        }

        public Lunatics.Mathematics.Vector2 GetItemRenderStartPosition(ItemView view, Rectf itemsArea)
        {
            return new Lunatics.Mathematics.Vector2(itemsArea.Left - view.GetHorzScrollbar().GetScrollPosition(),
                                                   itemsArea.Top - view.GetVertScrollbar().GetScrollPosition());
        }
        public void CreateRenderGeometryAndAddToItemView(ItemView view, RenderedString renderedString, Rectf drawRect, Font font, Rectf? itemClipper, bool isSelected)
        {
            if (view.GetSelectionBrushImage() != null && isSelected)
            {
                var renderSettings = new ImageRenderSettings(drawRect, itemClipper, true, view.GetSelectionColourRect());
                var brushGeomBuffers = view.GetSelectionBrushImage().CreateRenderGeometry(renderSettings);
                view.AppendGeometryBuffers(brushGeomBuffers);
            }

            var drawPos = drawRect.Position;
            for (var i = 0; i < renderedString.GetLineCount(); ++i)
            {
                drawPos.Y += CoordConverter.AlignToPixels((font.GetLineSpacing() - font.GetFontHeight()) * 0.5f);

                var stringGeomBuffers = renderedString.CreateRenderGeometry(view, i, drawPos, null, itemClipper, 0.0f);
                view.AppendGeometryBuffers(stringGeomBuffers);
                drawPos.Y += renderedString.GetPixelSize(view, i).Height;
            }
        }

        public void ResizeViewToContent(ItemView view, bool fitWidth, bool fitHeight)
        {
            var totalArea = view.GetUnclippedOuterRect().Get();
            var contentArea = GetViewRenderArea(view,
                                                !fitWidth && view.GetHorzScrollbar().IsVisible(),
                                                !fitHeight && view.GetVertScrollbar().IsVisible());
            var withScrollContentArea = GetViewRenderArea(view, true, true);

            var frameSize = totalArea.Size - contentArea.Size;
            var withScrollFrameSize = totalArea.Size - withScrollContentArea.Size;
            var contentSize = new Sizef(view.GetRenderedMaxWidth(), view.GetRenderedTotalHeight());

            var parentSize = view.GetParentPixelSize();
            var maxSize =
                    new Sizef(parentSize.Width - CoordConverter.AsAbsolute(view.GetXPosition(), parentSize.Width),
                              parentSize.Height - CoordConverter.AsAbsolute(view.GetYPosition(), parentSize.Height));

            var requiredSize = frameSize + contentSize + new Sizef(1, 1);

            if (fitHeight && requiredSize.Height > maxSize.Height)
            {
                requiredSize.Height = maxSize.Height;
                requiredSize.Width = Math.Min(maxSize.Width,
                                                requiredSize.Width - frameSize.Width + withScrollFrameSize.Width);
            }

            if (fitWidth && requiredSize.Width > maxSize.Width)
            {
                requiredSize.Width = maxSize.Width;
                requiredSize.Height = Math.Min(maxSize.Height,
                                                 requiredSize.Height - frameSize.Height + withScrollFrameSize.Height);
            }

            if (fitHeight)
                view.SetHeight(new UDim(0, requiredSize.Height));

            if (fitWidth)
                view.SetWidth(new UDim(0, requiredSize.Width));
        }
    }
}