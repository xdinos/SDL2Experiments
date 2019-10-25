using System;
using SharpCEGui.Base.Views;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// This is a convenience widget as an alternative to the new list view, for
    /// simple scenarios that don't require a custom ItemModel implementation.
    /// 
    /// Basically, what this class does, is to use a StandardItemModel as the source
    /// ItemModel, and provide some quick-access methods directly to the items,
    /// instead of going through the ModelIndex. Most of the ListView functions
    /// have the item-typed variants/overloads here. It also provides size_t-based
    /// index overloads, since this is a list with only one level of nestedness.
    /// 
    /// This a direct alternative for the old ListBox or ItemListbox widgets.
    /// </summary>
    public class ListWidget : ListView
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/ListWidget";

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ListWidget";

        public ListWidget(string type, string name)
                : base(type, name)
        {
        }

        // TODO: virtual ~ListWidget();

        public void SetIndexSelectionState(int item_index, bool state)
        {
            if (item_index > GetItemCount())
                throw new InvalidRequestException("the value passed in the 'item_index' parameter is out of range for this ListWidget.");

            base.SetIndexSelectionState(d_itemModel.MakeIndex(item_index, d_itemModel.GetRootIndex()), state);
        }

        public void SetIndexSelectionState(StandardItem item, bool state)
        {
            if (item == null)
                throw new InvalidRequestException("the item passed was null.");

            //SetIndexSelectionState(d_itemModel.GetIndexForItem(item), state);
            SetSelectedIndex(d_itemModel.GetIndexForItem(item));
        }

        /// <summary>
        /// Gets the ordinal first item. If the ListWidget has multi selection enabled
        /// and multiple items are selected, the first one will be returned.
        /// </summary>
        /// <returns></returns>
        public StandardItem GetFirstSelectedItem()
        {
            return GetNextSelectedItem(null);
        }

        /// <summary>
        /// Returns the next selected item after the specified \a start_item
        /// </summary>
        /// <param name="startItem">
        /// The item to start the search after. If this is NULL the search will
        /// begin at the beginning of the list.
        /// </param>
        /// <returns></returns>
        public StandardItem GetNextSelectedItem(StandardItem startItem)
        {
            if (d_indexSelectionStates.Count==0)
                return null;

            var childId = d_itemModel.GetChildId(startItem);
            if (startItem != null && childId == -1)
                return null;

            var index = startItem == null ? 0 : (childId + 1);
            var listSize = GetItemCount();

            for (; index < listSize; ++index)
            {
                if (IsIndexSelected(index))
                    return GetItemAtIndex(index);
            }

            return null;
        }

        public StandardItem GetItemAtIndex(int index)
        {
            return d_itemModel.GetItemForIndex(d_itemModel.MakeIndex(index, d_itemModel.GetRootIndex()));
        }

        /// <summary>
        /// Returns a StandardItem that has the specified \a text, the searching
        /// procedure starting from \a start_item
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startItem">
        /// If \a start_item is NULL, the search will start from the first item
        /// of the list.
        /// </param>
        /// <returns></returns>
        public StandardItem FindItemWithText(string text, StandardItem startItem)
        {
            var childId = d_itemModel.GetChildId(startItem);
            if (startItem != null && childId == -1)
                return null;

            // if start_item is NULL begin search at beginning, else start at item after start_item
            var index = startItem == null ? 0 : (childId + 1);
            var listSize = GetItemCount();

            while (index < listSize)
            {
                if (GetItemAtIndex(index).GetText() == text)
                {
                    return GetItemAtIndex(index);
                }

                index++;
            }

            return null;
        }

        public bool IsItemInList(StandardItem item)
        {
            //TODO: move this logic to the model?
            var childCount = d_itemModel.GetChildCount(d_itemModel.GetRootIndex());
            for (var i = 0; i < childCount; ++i)
            {
                if (item == d_itemModel.GetItemForIndex(d_itemModel.MakeIndex(i, d_itemModel.GetRootIndex())))
                    return true;
            }

            return false;
        }

        public bool IsItemSelected(StandardItem item)
        {
            var states = GetIndexSelectionStates();
            foreach (var itor in states)
            {
                if (item == d_itemModel.GetItemForIndex(itor.d_selectedIndex))
                    return true;
            }

            return false;
        }

        public bool IsIndexSelected(int index)
        {
            //TODO: make a macro/inline function for lists that makes an index - since Root is always the parent
            return base.IsIndexSelected(d_itemModel.MakeIndex(index, d_itemModel.GetRootIndex()));
        }

        public int GetItemCount()
        {
            return d_itemModel.GetChildCount(d_itemModel.GetRootIndex());
        }

        public int GetSelectedItemsCount()
        {
            return d_indexSelectionStates.Count;
        }

        public new virtual StandardItemModel GetModel()
        {
            return (StandardItemModel)d_itemModel;
        }

        public void AddItem(string text)
        {
            d_itemModel.AddItem(text);
        }

        public void AddItem(StandardItem item)
        {
            d_itemModel.AddItem(item);
        }

        public void InsertItem(StandardItem item, StandardItem position)
        {
            d_itemModel.InsertItem(item, position);
        }

        public void RemoveItem(StandardItem item)
        {
            d_itemModel.RemoveItem(item);
        }

        //! Clears the items in this list and deletes all associated items.
        public void ClearList()
        {
            d_itemModel.Clear();
            OnViewContentsChanged(new WindowEventArgs(this));
        }

        public virtual void EnsureIndexIsVisible(StandardItem item)
        {
            base.EnsureIndexIsVisible(d_itemModel.GetIndexForItem(item));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void InitialiseComponents()
        {
            base.InitialiseComponents();
            SetModel(d_itemModel);
        }

        protected new StandardItemModel d_itemModel = new StandardItemModel();
    }
}