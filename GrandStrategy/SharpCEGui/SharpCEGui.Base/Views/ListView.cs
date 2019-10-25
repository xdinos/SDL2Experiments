using System;
using System.Collections.Generic;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// This internal struct represents the rendering state of an item that is part
    /// of the ListView. This is recomputed each time the ListView is invalidated.
    /// 
    /// This struct is meant to be used as a caching mechanism so that the ItemModel
    /// is not queries each time rendering is done. That means, the users of ListView
    /// shouldn't use this struct for interacting with the list, but rather use the
    /// attached ItemModel.
    /// </summary>
    public /*struct*/class ListViewItemRenderingState
    {
        public RenderedString d_string;
        
        /// <summary>
        /// The name of the image that represents the icon
        /// </summary>
        public string d_icon;
        
        public Sizef d_size;
        
        public bool d_isSelected;
        public ModelIndex d_index;
        public string d_text;
        public ListView d_attachedListView;

        public ListViewItemRenderingState(ListView list_view)
        {
            d_isSelected = false;
            d_attachedListView = list_view;
        }
        
        public static bool operator <(ListViewItemRenderingState lhs, ListViewItemRenderingState rhs)
        {
            return lhs.d_attachedListView.GetModel().CompareIndices(lhs.d_index, rhs.d_index) < 0;
        }
        public static bool operator >(ListViewItemRenderingState lhs, ListViewItemRenderingState rhs)
        {
            return lhs.d_attachedListView.GetModel().CompareIndices(lhs.d_index, rhs.d_index) > 0;
        }
    }

    /// <summary>
    /// View that displays items in a listed fashion.
    /// 
    /// ItemModel%s that use a tree structure can be still rendered by this view,
    /// but only the children that are direct children of the ItemModel::getRootIndex()
    /// are taken into considerations. That is, you cannot use a list to render
    /// arbitrary list of children, unless a specific ItemModel implementation that
    /// provides that is specified.
    /// </summary>
    public class ListView : ItemView
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ListView";
        
        //! Namespace for global events
        public new const string EventNamespace = "ListView";

        public ListView(string type, string name)
                : base(type, name)
        { }

        // TODO: virtual ~ListView() { }

        public IList<ListViewItemRenderingState> GetItems()
        {
            return d_sortedItems;
        }

        public override void PrepareForRender()
        {
            base.PrepareForRender();
            if (d_itemModel == null || !IsDirty())
                return;

            if (d_needsFullRender)
            {
                d_renderedMaxWidth = d_renderedTotalHeight = 0;
                d_items.Clear();
            }

            var rootIndex = d_itemModel.GetRootIndex();
            var childCount = d_itemModel.GetChildCount(rootIndex);

            for (var child = 0; child < childCount; ++child)
            {
                var index = d_itemModel.MakeIndex(child, rootIndex);

                if (d_needsFullRender)
                {
                    var state = new ListViewItemRenderingState(this);
                    UpdateItem(state, index, ref d_renderedMaxWidth, ref d_renderedTotalHeight);
                    d_items.Add(state);
                }
                else
                {
                    var item = d_items[child];
                    d_renderedTotalHeight -= item.d_size.Height;

                    UpdateItem(item, index, ref d_renderedMaxWidth, ref d_renderedTotalHeight);
                }
            }

            UpdateScrollbars();
            SetIsDirty(false);
            ResortListView();
            d_needsFullRender = false;
        }

        public override ModelIndex IndexAt(Lunatics.Mathematics.Vector2 position)
        {
            if (d_itemModel == null)
                return new ModelIndex();

            //TODO: add prepareForLayout() as a cheaper operation alternative?
            PrepareForRender();

            var windowPosition = CoordConverter.ScreenToWindow(this, position);
            var renderArea = GetViewRenderer().GetViewRenderArea();

            if (!renderArea.IsPointInRect(windowPosition))
                return new ModelIndex();

            var curHeight = renderArea.d_min.Y - GetVertScrollbar().GetScrollPosition();
            //TODO: start only on the visible area
            for (var index = 0; index < d_sortedItems.Count; ++index)
            {
                var item = d_sortedItems[index];
                var size = item.d_size;
                var nextHeight = curHeight + size.Height;

                if (windowPosition.Y >= curHeight &&
                    windowPosition.Y <= nextHeight)
                {
                    return item.d_index;
                }

                curHeight = nextHeight;
            }

            return new ModelIndex();
        }


        protected override bool OnChildrenAdded(EventArgs args)
        {
            base.OnChildrenAdded(args);
            var margs = (ModelEventArgs) args;

            if (!d_itemModel.AreIndicesEqual(margs.d_parentIndex, d_itemModel.GetRootIndex()))
                return true;

            var items = new List<ListViewItemRenderingState>();
            for (var i = 0; i < margs.d_count; ++i)
            {
                var item = new ListViewItemRenderingState(this);

                UpdateItem(item, d_itemModel.MakeIndex(margs.d_startId + i, margs.d_parentIndex), ref d_renderedMaxWidth, ref d_renderedTotalHeight);

                items.Add(item);
            }

            d_items.InsertRange(margs.d_startId, items);

            //TODO: insert in the right place directly!
            ResortListView();
            InvalidateView(false);
            return true;
        }

        protected override bool OnChildrenRemoved(EventArgs args)
        {
            base.OnChildrenRemoved(args);
            var margs = (ModelEventArgs) args;

            if (!d_itemModel.AreIndicesEqual(margs.d_parentIndex, d_itemModel.GetRootIndex()))
                return true;
            
            var begin = margs.d_startId;
            for (var itor = begin; itor < margs.d_count; itor++)
            {
                d_renderedTotalHeight -= d_items[itor].d_size.Height;
            }

            d_items.RemoveRange(begin, margs.d_count);

            ResortListView();
            InvalidateView(false);
            return true;
        }

        private List<ListViewItemRenderingState> d_items = new List<ListViewItemRenderingState>();
        private List<ListViewItemRenderingState> d_sortedItems = new List<ListViewItemRenderingState>();

        private void ResortListView()
        {
            d_sortedItems.Clear();

            foreach (var itor in d_items)
                d_sortedItems.Add(itor);
            
            if (d_sortMode == ViewSortMode.None)
                return;

            // TODO: sort(d_sortedItems.begin(), d_sortedItems.end(), d_sortMode == VSM_Ascending ? &listViewItemPointerLess : &listViewItemPointerGreater);
        }

        protected override void ResortView()
        {
            ResortListView();
            InvalidateView(false);
        }

        /// <summary>
        /// Updates the rendering state for the specified \a item using the specified
        /// \a index as the data source.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="maxWidth"></param>
        /// <param name="totalHeight"></param>
        private void UpdateItem(ListViewItemRenderingState item, ModelIndex index, ref float maxWidth, ref float totalHeight)
        {
            var text = d_itemModel.GetData(index);
            var isSelected = IsIndexSelected(index);
            var initialColours = isSelected ? _selectionTextColourRect : d_textColourRect;
            var renderedString = GetRenderedStringParser().Parse(text, GetFont(), initialColours);
            item.d_string = renderedString;
            item.d_index = index;
            item.d_text = text;
            item.d_icon = d_itemModel.GetData(index, ItemDataRole.IDR_Icon);

            var iconSize = Sizef.Zero;
            if (!String.IsNullOrEmpty(item.d_icon))
                iconSize = ImageManager.GetSingleton().Get(item.d_icon).GetRenderedSize();

            var textSize = new Sizef(renderedString.GetHorizontalExtent(this), renderedString.GetVerticalExtent(this));

            item.d_size = new Sizef(iconSize.Width + textSize.Width, Math.Max(iconSize.Height, textSize.Height));

            maxWidth = Math.Max(item.d_size.Width, maxWidth);

            totalHeight += item.d_size.Height;

            item.d_isSelected = isSelected;//IsIndexSelected(index);
        }

        protected override Rectf GetIndexRect(ModelIndex index)
        {
            var childId = d_itemModel.GetChildId(index);
            if (childId == -1)
                return Rectf.Zero;

            var pos = Lunatics.Mathematics.Vector2.Zero;

            for (var i = 0; i < childId; ++i)
                pos.Y += d_items[i].d_size.Height;

            return new Rectf(pos, d_items[childId].d_size);
        }
    }
}