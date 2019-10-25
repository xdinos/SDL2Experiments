using System;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// This is an example model that inherits the default GenericItemModel
    /// functionality over a specified model type (StandardItem).
    /// 
    /// Besides that we added a new function that updates a given item's text.
    /// </summary>
    public class StandardItemModel : GenericItemModel<StandardItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public StandardItemModel()
            :base(new StandardItem())
        {
        }

        /// <summary>
        /// Updates the specified \a item's text with the new one.
        /// 
        /// This function will notify the listeners that the specified \a item's
        /// text has been changed, via the EventChildrenDataWillChange and
        /// EventChildrenDataChanged events.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newText"></param>
        public void UpdateItemText(StandardItem item, String newText)
        {
            var parentIndex = GetParentIndex(GetIndexForItem(item));

            NotifyChildrenDataWillChange(parentIndex, 0, 1);

            item.SetText(newText);

            NotifyChildrenDataChanged(parentIndex, 0, 1);
        }
    }
}