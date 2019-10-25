using SharpCEGui.Base;
using System;
using SharpCEGui.Base.Views;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// ListView class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.
    /// The LookNFeel should provide the following:
    /// 
    /// States:
    /// - Enabled
    /// - Disabled
    /// 
    /// Named Areas:
    /// - ItemRenderingArea
    /// - ItemRenderingAreaHScroll
    /// - ItemRenderingAreaVScroll
    /// - ItemRenderingAreaHVScroll
    /// 
    /// OR
    /// 
    /// - ItemRenderArea
    /// - ItemRenderAreaHScroll
    /// - ItemRenderAreaVScroll
    /// - ItemRenderAreaHVScroll
    /// 
    /// Child Widgets:
    /// Scrollbar based widget with name suffix "__auto_vscrollbar__"
    /// Scrollbar based widget with name suffix "__auto_hscrollbar__"
    /// </summary>
    public class FalagardListView : ItemViewWindowRenderer//, ItemViewRenderer
    {
        /// <summary>
        /// The name of this renderer's factory.
        /// </summary>
        public const string TypeName = "Core/ListView";

        /// <summary>
        /// Constructor for the ListView Falagard class.
        /// </summary>
        /// <param name="type">
        /// The name of this renderer's factory.
        /// </param>
        public FalagardListView(string type)
                : base(type)
        {
            
        }

        public override void CreateRenderGeometry()
        {
            var wlf = GetLookNFeel();
            var listView = (ListView) Window;

            listView.PrepareForRender();

            var hasFocusedState = listView.IsFocused() && wlf.IsStateImageryPresent("EnabledFocused");
            var imagery = wlf.GetStateImagery(listView.IsEffectiveDisabled()
                                                               ? "Disabled"
                                                               : (hasFocusedState ? "EnabledFocused" : "Enabled"));
            imagery.Render(listView);

            CreateRenderGeometry(listView);
        }

        public override Rectf GetViewRenderArea()
        {
            return _itemViewRenderer.GetViewRenderArea(GetView());
        }

        public override void ResizeViewToContent(bool fitWidth, bool fitHeight)
        {
            _itemViewRenderer.ResizeViewToContent(GetView(), fitWidth, fitHeight);
        }

        public override void AutoPositionSize(Lunatics.Mathematics.Vector2 position, Sizef ownerSize)
        {
            var view = GetView();
            var totalArea = view.GetUnclippedOuterRect().Get();

            var contentArea = _itemViewRenderer.GetViewRenderArea(view, false, false);
            var withScrollContentArea = _itemViewRenderer.GetViewRenderArea(view, true, true);

            var frameSize = totalArea.Size - contentArea.Size;
            var withScrollFrameSize = totalArea.Size - withScrollContentArea.Size;
            var contentSize = new Sizef(view.GetRenderedMaxWidth(), view.GetRenderedTotalHeight());

            var parentSize = view.GetParentPixelSize();
            var maxSize =
                    new Sizef(ownerSize.Width/*parentSize.Width - CoordConverter.AsAbsolute(view.GetXPosition(), parentSize.Width)*/,
                              0.8f*parentSize.Height/*CoordConverter.AsAbsolute(view.GetYPosition(), parentSize.Height)*/);

            var requiredSize = frameSize + contentSize + new Sizef(1, 1);

            if (requiredSize.Height > maxSize.Height)
            {
                requiredSize.Height = maxSize.Height;
                requiredSize.Width = Math.Min(maxSize.Width,
                                              requiredSize.Width - frameSize.Width + withScrollFrameSize.Width);
                view.SetVertScrollbarDisplayMode(ScrollbarDisplayMode.Shown);
            }
            else
            {
                view.SetVertScrollbarDisplayMode(ScrollbarDisplayMode.Hidden);
            }

            requiredSize.Width = ownerSize.Width;
            if (requiredSize.Width > maxSize.Width)
            {
                requiredSize.Width = maxSize.Width;
                requiredSize.Height = Math.Min(maxSize.Height,
                                               requiredSize.Height - frameSize.Height + withScrollFrameSize.Height);
                view.SetHorzScrollbarDisplayMode(ScrollbarDisplayMode.Shown);
            }
            else
            {
                view.SetHorzScrollbarDisplayMode(ScrollbarDisplayMode.Hidden);
            }

            var posY = position.Y + (ownerSize.Height - requiredSize.Height)/2f;
            posY = posY < 0f ? 0f : posY;
            posY -= posY + requiredSize.Height > parentSize.Height
                                  //? parentSize.Height - (posY + requiredSize.Height + 1f)
                                  ? (posY + requiredSize.Height) - parentSize.Height
                                  : 0f;

            view.SetPosition(new UVector2(UDim.Absolute(position.X), UDim.Absolute(posY)));
            view.SetHeight(new UDim(0, requiredSize.Height));
            view.SetWidth(new UDim(0, requiredSize.Width));
            
        }

        private void CreateRenderGeometry(ListView listView)
        {
            var itemsArea = GetViewRenderArea();
            var itemPos = _itemViewRenderer.GetItemRenderStartPosition(listView, itemsArea);

            for (var i = 0; i < listView.GetItems().Count; ++i)
            {
                var item = listView.GetItems()[i];
                var renderedString = item.d_string;
                var size = item.d_size;

                size.Width = Math.Max(itemsArea.Width, size.Width);

                var itemRect = new Rectf(itemPos, size);

                if (!string.IsNullOrEmpty(item.d_icon))
                {
                    var img = ImageManager.GetSingleton().Get(item.d_icon);

                    var iconRect = itemRect;
                    iconRect.Width = size.Height;
                    iconRect.Height = size.Height;

                    var iconClipper = iconRect.GetIntersection(itemsArea);

                    var renderSettings = new ImageRenderSettings(iconRect, iconClipper, true, ICON_COLOUR_RECT, 1.0f);
                    var imgGeomBuffers = img.CreateRenderGeometry(renderSettings);
                    
                    if (item.d_isSelected)
                    {
                        renderSettings = new ImageRenderSettings(iconRect, iconClipper, true, listView.GetSelectionColourRect());
                        var brushGeomBuffers = listView.GetSelectionBrushImage().CreateRenderGeometry(renderSettings);
                        listView.AppendGeometryBuffers(brushGeomBuffers);
                    }

                    listView.AppendGeometryBuffers(imgGeomBuffers);

                    itemRect.Left = itemRect.Left + iconRect.Width;
                }

                var itemClipper = itemRect.GetIntersection(itemsArea);

                _itemViewRenderer.CreateRenderGeometryAndAddToItemView(listView, renderedString, itemRect,
                                                                       listView.GetFont(), itemClipper,
                                                                       item.d_isSelected);

                itemPos.Y += size.Height;
            }
        }

        private readonly ItemViewRenderer _itemViewRenderer = new ItemViewRenderer();
        private static readonly ColourRect ICON_COLOUR_RECT = new ColourRect(new Colour(1f, 1f, 1f, 1f));
        
    }
}