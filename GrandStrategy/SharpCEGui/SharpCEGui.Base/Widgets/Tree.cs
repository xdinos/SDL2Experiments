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
using System.Collections.Generic;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// EventArgs based class that is used for objects passed to input event
    /// handlers concerning Tree events.
    /// </summary>
    public class TreeEventArgs : WindowEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wnd"></param>
        public TreeEventArgs(Window wnd) : base(wnd)
        {
            treeItem = null;
        }

        public TreeItem treeItem;
    }

    /// <summary>
    /// Base class for standard Tree widget.
    /// <remarks>
    /// The CEGUI::Tree, CEGUI::TreeItem and any other associated classes are
    /// deprecated and thier use should be minimised - preferably eliminated -
    /// where possible.  It is extremely unfortunate that this widget was ever added
    /// to CEGUI since its design and implementation are poor and do not meet 
    /// established standards for the CEGUI project.
    /// <para>
    /// While no alternative currently exists, a superior, replacement tree widget
    /// will be provided prior to the final removal of the current implementation.
    /// </para>
    /// </remarks>
    /// </summary>
    public class Tree: Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Tree";

        /// <summary>
        /// 
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Tree";

        #region Events

        /** Event fired when the content of the tree is changed.
         * Handlers are passed a const WindowEventArgs reference with
         * WindowEventArgs::window set to the Tree whose content has changed.
         */
        public event EventHandler<WindowEventArgs> ListContentsChanged;

        /** Event fired when there is a change to the currently selected item(s).
         * Handlers are passed a const TreeEventArgs reference with
         * WindowEventArgs::window set to the Tree whose item selection has changed,
         * and TreeEventArgs::treeItem is set to the (last) item to be selected, or
         * 0 if none.
         */
        public event EventHandler<WindowEventArgs> SelectionChanged;

        /** Event fired when the sort mode setting for the Tree is changed.
         * Handlers are passed a const WindowEventArgs reference with
         * WindowEventArgs::window set to the Tree whose sort mode has been
         * changed.
         */
        public event EventHandler<WindowEventArgs> SortModeChanged;
        
        /** Event fired when the multi-select mode setting for the Tree changes.
         * Handlers are passed a const TreeEventArgs reference with
         * WindowEventArgs::window set to the Tree whose setting has changed.
         * TreeEventArgs::treeItem is always set to 0.
         */
        public event EventHandler<WindowEventArgs> MultiselectModeChanged;
        
        /** Event fired when the mode setting that forces the display of the
         * vertical scroll bar for the tree is changed.
         * Handlers are passed a const WindowEventArgs reference with
         * WindowEventArgs::window set to the Tree whose vertical scrollbar mode has
         * been changed.
         */
        public event EventHandler<WindowEventArgs> VertScrollbarModeChanged;
        
        /** Event fired when the mode setting that forces the display of the
         * horizontal scroll bar for the tree is changed.
         * Handlers are passed a const WindowEventArgs reference with
         * WindowEventArgs::window set to the Tree whose horizontal scrollbar mode
         * has been changed.
         */
        public event EventHandler<WindowEventArgs> HorzScrollbarModeChanged;
        
        /** Event fired when a branch of the tree is opened by the user.
         * Handlers are passed a const TreeEventArgs reference with
         * WindowEventArgs::window set to the Tree containing the branch that has
         * been opened and TreeEventArgs::treeItem set to the TreeItem at the head
         * of the opened branch.
         */
        public event EventHandler<WindowEventArgs> BranchOpened;
        
        /** Event fired when a branch of the tree is closed by the user.
         * Handlers are passed a const TreeEventArgs reference with
         * WindowEventArgs::window set to the Tree containing the branch that has
         * been closed and TreeEventArgs::treeItem set to the TreeItem at the head
         * of the closed branch.
         */
        public event EventHandler<WindowEventArgs> BranchClosed;

        #endregion

        /// <summary>
        /// Render the actual tree
        /// </summary>
        public void DoTreeRender()
        {
            PopulateGeometryBuffer();
        }
        
        /// <summary>
        /// UpdateScrollbars
        /// </summary>
        public void DoScrollbars()
        {
            ConfigureScrollbars();
        }

        /// <summary>
        /// Return number of items attached to the tree
        /// </summary>
        /// <returns>
        /// the number of items currently attached to this tree.
        /// </returns>
        public int GetItemCount()
        {
            return d_listItems.Count;
        }

        /// <summary>
        /// Return the number of selected items in the tree.
        /// </summary>
        /// <returns>
        /// Total number of attached items that are in the selected state.
        /// </returns>
        public int GetSelectedCount()
        {
            var count = 0;

            for (var index = 0; index < d_listItems.Count; ++index)
            {
                if (d_listItems[index].IsSelected())
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Return a pointer to the first selected item.
        /// </summary>
        /// <returns>
        /// Pointer to a TreeItem based object that is the first selected item in
        /// the tree.  will return 0 if no item is selected.
        /// </returns>
        public TreeItem GetFirstSelectedItem()
        {
            return GetNextSelectedItemFromList(d_listItems, null, true);
        }
        
        /// <summary>
        /// Return a pointer to the first selected item.
        /// </summary>
        /// <returns>
        /// Pointer to a TreeItem based object that is the last item selected by the
        /// user, not necessarily the last selected in the tree.  Will return 0 if
        /// no item is selected.
        /// </returns>
        public TreeItem GetLastSelectedItem()
        {
            return d_lastSelected;
        }

        /// <summary>
        /// Return a pointer to the next selected item after item \a start_item
        /// </summary>
        /// <param name="startItem">
        /// Pointer to the TreeItem where the search for the next selected item is
        /// to begin.  If this parameter is 0, the search will begin with the first
        /// item in the tree.
        /// </param>
        /// <returns>
        /// Pointer to a TreeItem based object that is the next selected item in
        /// the tree after the item specified by \a start_item.  Will return 0 if
        /// no further items were selected.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to this tree.
        /// </exception>
        public TreeItem GetNextSelected(TreeItem startItem)
        {
            return GetNextSelectedItemFromList(d_listItems, startItem, (startItem == null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="startItem"></param>
        /// <param name="foundStartItem"></param>
        /// <returns></returns>
        public TreeItem GetNextSelectedItemFromList(List<TreeItem> itemList, TreeItem startItem, bool foundStartItem)
        {
            var itemCount = itemList.Count;

            for (var index = 0; index < itemCount; ++index)
            {
                if (foundStartItem)
                {
                    // Already found the startItem, now looking for next selected item.
                    if (itemList[index].IsSelected())
                        return itemList[index];
                }
                else
                {
                    // Still looking for startItem.  Is this it?
                    if (itemList[index] == startItem)
                        foundStartItem = true;
                }

                if (itemList[index].GetItemCount() > 0)
                {
                    if (itemList[index].GetIsOpen())
                    {
                        var foundSelectedTree = GetNextSelectedItemFromList(itemList[index].GetItemList(), startItem, foundStartItem);
                        if (foundSelectedTree != null)
                            return foundSelectedTree;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// return whether tree sorting is enabled
        /// </summary>
        /// <returns>
        /// - true if the tree is sorted
        /// - false if the tree is not sorted
        /// </returns>
        public bool IsSortEnabled()
        {
            return d_sorted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public void SetItemRenderArea(Rectf r)
        {
            d_itemArea = r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Scrollbar GetVertScrollbar()
        {
            return d_vertScrollbar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Scrollbar GetHorzScrollbar()
        { 
            return d_horzScrollbar; 
        }

        /// <summary>
        /// return whether multi-select is enabled
        /// </summary>
        /// <returns>
        /// true if multi-select is enabled, false if multi-select is not enabled.
        /// </returns>
        public bool IsMultiselectEnabled()
        {
            return d_multiselect;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsItemTooltipsEnabled()
        {
            return d_itemTooltips;
        }

        /// <summary>
        /// Search the tree for an item with the specified text
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <returns></returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this tree.
        /// </exception>
        /// /// <returns>
        /// Pointer to the first TreeItem in the tree after \a item that has text
        /// matching \a text.  If no item matches the criteria, 0 is returned.
        /// </returns>
        public TreeItem FindFirstItemWithText(string text)
        {
            return FindItemWithTextFromList(d_listItems, text, null, true);
        }

        /// <summary>
        /// Search the tree for an item with the specified text
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="startItem">
        /// TreeItem where the search is to begin, the search will not include \a
        /// item.  If \a item is 0, the search will begin from the first item in
        /// the tree.</param>
        /// <returns>
        /// Pointer to the first TreeItem in the tree after \a item that has text
        /// matching \a text.  If no item matches the criteria, 0 is returned.
        /// </returns>
        /// /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this tree.
        /// </exception>
        public TreeItem FindNextItemWithText(string text, TreeItem startItem)
        {
            return startItem == null
                       ? FindItemWithTextFromList(d_listItems, text, null, true)
                       : FindItemWithTextFromList(d_listItems, text, startItem, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="text"></param>
        /// <param name="startItem"></param>
        /// <param name="foundStartItem"></param>
        /// <returns></returns>
        public TreeItem FindItemWithTextFromList(List<TreeItem> itemList, string text, TreeItem startItem, bool foundStartItem)
        {
            var itemCount = itemList.Count;

            for (var index = 0; index < itemCount; ++index)
            {
                if (foundStartItem)
                {
                    // Already found the startItem, now looking for the actual text.
                    if (itemList[index].GetText() == text)
                        return itemList[index];
                }
                else
                {
                    // Still looking for startItem.  Is this it?
                    if (itemList[index] == startItem)
                        foundStartItem = true;
                }

                if (itemList[index].GetItemCount() > 0)
                {
                    // Search the current item's itemList regardless if it's open or not.
                    var foundSelectedTree = FindItemWithTextFromList(itemList[index].GetItemList(), text, startItem, foundStartItem);
                    if (foundSelectedTree != null)
                        return foundSelectedTree;
                }
            }

            return null;
        }

        /// <summary>
        /// Search the tree for an item with the specified id.
        /// </summary>
        /// <param name="searchId"></param>
        /// <returns>
        /// Pointer to the first TreeItem in the tree after \a item that has id
        /// matching \a searchId.  If no item matches the criteria 0 is returned.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this tree.
        /// </exception>
        public TreeItem FindFirstItemWithId(int searchId)
        {
            return FindItemWithIdFromList(d_listItems, searchId, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchId"></param>
        /// <param name="startItem"></param>
        /// <returns></returns>
        public TreeItem FindNextItemWithId(int searchId, TreeItem startItem)
        {
            return startItem == null
                       ? FindItemWithIdFromList(d_listItems, searchId, null, true)
                       : FindItemWithIdFromList(d_listItems, searchId, startItem, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="searchId"></param>
        /// <param name="startItem"></param>
        /// <param name="foundStartItem"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TreeItem FindItemWithIdFromList(List<TreeItem> itemList, int searchId, TreeItem startItem, bool foundStartItem)
        {
            var itemCount = itemList.Count;

            for (var index = 0; index < itemCount; ++index)
            {
                if (foundStartItem)
                {
                    // Already found the startItem, now looking for the actual text.
                    if (itemList[index].GetId() == searchId)
                        return itemList[index];
                }
                else
                {
                    // Still looking for startItem.  Is this it?
                    if (itemList[index] == startItem)
                        foundStartItem = true;
                }

                if (itemList[index].GetItemCount() > 0)
                {
                    // Search the current item's itemList regardless if it's open or not.
                    var foundSelectedTree = FindItemWithIdFromList(itemList[index].GetItemList(), searchId, startItem, foundStartItem);
                    if (foundSelectedTree != null)
                        return foundSelectedTree;
                }
            }

            return null;
        }

        /// <summary>
        /// Return whether the specified TreeItem is in the tree
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// - true if TreeItem \a item is in the tree.
        /// - false if TreeItem \a item is not in the tree.
        /// </returns>
        public bool IsTreeItemInList(TreeItem item)
        {
            return d_listItems.Contains(item);
        }

        /// <summary>
        /// Return whether the vertical scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will always be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsVertScrollbarAlwaysShown()
        {
            return d_forceVertScroll;
        }

        /// <summary>
        /// Return whether the horizontal scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will always be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsHorzScrollbarAlwaysShown()
        {
            return d_forceHorzScroll;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialise()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = WidgetLookManager.GetSingleton().GetWidgetLook(d_lookName);
            var tempOpenImagery = wlf.GetImagerySection("OpenTreeButton");
            var tempCloseImagery = wlf.GetImagerySection("CloseTreeButton");
            d_openButtonImagery = tempOpenImagery;
            d_closeButtonImagery = tempCloseImagery;
    
            // create the component sub-widgets
            d_vertScrollbar = CreateVertScrollbar("__auto_vscrollbar__");
            d_horzScrollbar = CreateHorzScrollbar("__auto_hscrollbar__");
    
            AddChild(d_vertScrollbar);
            AddChild(d_horzScrollbar);

            d_vertScrollbar.ScrollPositionChanged += HandleScrollChange;
            d_horzScrollbar.ScrollPositionChanged += HandleScrollChange;
    
            ConfigureScrollbars();
            PerformChildWindowLayout();
        }

        /// <summary>
        /// Remove all items from the tree.
        /// <para>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </para>
        /// </summary>
        public void ResetList()
        {
            if (ResetListImpl())
            {
                OnListContentsChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Add the given TreeItem to the tree.
        /// </summary>
        /// <param name="item">
        /// Pointer to the TreeItem to be added to the tree.  Note that it is the
        /// passed object that is added to the tree, a copy is not made.  If this
        /// parameter is NULL, nothing happens.
        /// </param>
        public void AddItem(TreeItem item)
        {
            if (item != null)
            {
                // establish ownership
                item.SetOwnerWindow(this);
        
                // if sorting is enabled, re-sort the list
                if (IsSortEnabled())
                {
                    d_listItems.Add(item);
                    //d_listItems.insert(std::upper_bound(d_listItems.begin(), d_listItems.end(), item, &lbi_less), item);
                }
                // not sorted, just stick it on the end.
                else
                {
                    d_listItems.Add(item);
                }
                
                OnListContentsChanged(new WindowEventArgs(this));
            }
        }

        /*!
         \brief
            Insert an item into the tree after a specified item already in the
            tree.
     
            Note that if the tree is sorted, the item may not end up in the
            requested position.
     
         \param item
             Pointer to the TreeItem to be inserted.  Note that it is the passed
             object that is added to the tree, a copy is not made.  If this
             parameter is 0, nothing happens.
     
         \param position
             Pointer to a TreeItem that \a item is to be inserted after.  If this
             parameter is 0, the item is inserted at the start of the tree.
     
         \return
            Nothing.
     
         \exception InvalidRequestException	thrown if no TreeItem \a position is
            attached to this tree.
         */
        public void InsertItem(TreeItem item, TreeItem position)
        {
            // if the list is sorted, it's the same as a normal add operation
            if (IsSortEnabled())
            {
                AddItem(item);
            }
            else if (item != null)
            {
                // establish ownership
                item.SetOwnerWindow(this);
        
                // if position is NULL begin insert at begining, else insert after item 'position'
                int insPos = position == null ? 0 : d_listItems.IndexOf(position);
        
                // throw if item 'position' is not in the list
                if (insPos == -1)
                {
                    throw new InvalidRequestException(
                        "the specified TreeItem for parameter 'position' is not attached to this Tree.");
                }
        
                d_listItems.Insert(insPos, item);
        
                OnListContentsChanged(new WindowEventArgs(this));
            }
        }

        /*!
         \brief
            Removes the given item from the tree.  If the item is has the auto
            delete state set, the item will be deleted.
     
         \param item
            Pointer to the TreeItem that is to be removed.  If \a item is not
            attached to this tree then nothing will happen.
     
         \return
            Nothing.
         */
        public void RemoveItem(TreeItem item)
        {
            if (item != null)
            {
                // if item is in the list
                if (d_listItems.Contains(item))
                {
                    // disown item
                    item.SetOwnerWindow(null);
            
                    // remove item
                    d_listItems.Remove(item);
            
                    // if item was the last selected item, reset that to NULL
                    if (item == d_lastSelected)
                    {
                        d_lastSelected = null;
                    }
            
                    // if item is supposed to be deleted by us
                    if (item.IsAutoDeleted())
                    {
                        // clean up this item.
                        //TODO: CEGUI_DELETE_AO item;
                    }
            
                    OnListContentsChanged(new WindowEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Clear the selected state for all items.
        /// </summary>
        public void ClearAllSelections()
        {
            // only fire events and update if we actually made any changes
            if (ClearAllSelectionsImpl())
            {
                OnSelectionChanged(new TreeEventArgs(this));
            }
        }

        public bool ClearAllSelectionsFromList(List<TreeItem>itemList)
        {
            // flag used so we can track if we did anything.
            var modified = false;

            for (var index = 0; index < itemList.Count; ++index)
            {
                if (itemList[index].IsSelected())
                {
                    itemList[index].SetSelected(false);
                    modified = true;
                }

                if (itemList[index].GetItemCount() > 0)
                {
                    var modifiedSubList = ClearAllSelectionsFromList(itemList[index].GetItemList());
                    if (modifiedSubList)
                        modified = true;
                }
            }

            return modified;
        }

        /// <summary>
        /// Set whether the tree should be sorted.
        /// </summary>
        /// <param name="setting">
        /// - true if the tree should be sorted
        /// - false if the tree should not be sorted.
        /// </param>
        public void SetSortingEnabled(bool setting)
        {
            // only react if the setting will change
            if (d_sorted != setting)
            {
                d_sorted = setting;
        
                // if we are enabling sorting, we need to sort the list
                if (d_sorted)
                {
                    d_listItems.Sort();
                }
        
                OnSortModeChanged(new WindowEventArgs(this));
            }
        }

        /*!
         \brief
            Set whether the tree should allow multiple selections or just a single
            selection.
     
         \param  setting
             - true if the widget should allow multiple items to be selected
             - false if the widget should only allow a single selection.

         \return
            Nothing.
         */
        public void SetMultiselectEnabled(bool setting)
        {
            // only react if the setting is changed
            if (d_multiselect != setting)
            {
                d_multiselect = setting;
        
                // if we change to single-select, deselect all except the first selected item.
                var args=new TreeEventArgs(this);
                if ((!d_multiselect) && (GetSelectedCount() > 1))
                {
                    var itm = GetFirstSelectedItem();
            
                    while (itm!=null)
                    {
                        itm.SetSelected(false);
                        itm = GetNextSelected(itm);
                    }
            
                    OnSelectionChanged(args);
                }
        
                OnMultiselectModeChanged(args);
            }
        }

        /*!
         \brief
            Set whether the vertical scroll bar should always be shown.
     
         \param setting
             - true if the vertical scroll bar should be shown even when it is not
               required.
             - false if the vertical scroll bar should only be shown when it is
               required.

         \return
            Nothing.
         */
        public void SetShowVertScrollbar(bool setting)
        {
            if (d_forceVertScroll != setting)
            {
                d_forceVertScroll = setting;
        
                ConfigureScrollbars();
                OnVertScrollbarModeChanged(new WindowEventArgs(this));
            }
        }

        /*!
         \brief
            Set whether the horizontal scroll bar should always be shown.
     
         \param setting
             - true if the horizontal scroll bar should be shown even when it is not
               required.
             - false if the horizontal scroll bar should only be shown when it is
               required.

         \return
            Nothing.
         */
        public void SetShowHorzScrollbar(bool setting)
        {
            if (d_forceHorzScroll != setting)
            {
                d_forceHorzScroll = setting;
        
                ConfigureScrollbars();
                OnHorzScrollbarModeChanged(new WindowEventArgs(this));
            }
        }
    
        public void SetItemTooltipsEnabled(bool setting)
        {
            d_itemTooltips = setting;
        }

        /*!
         \brief
            Set the select state of an attached TreeItem.
     
            This is the recommended way of selecting and deselecting items attached
            to a tree as it respects the multi-select mode setting.  It is
            possible to modify the setting on TreeItems directly, but that approach
            does not respect the settings of the tree.
     
         \param item
            The TreeItem to be affected.
            This item must be attached to the tree.
     
         \param state
            - true to select the item.
            - false to de-select the item.
     
         \return
            Nothing.
     
         \exception	InvalidRequestException	thrown if \a item is not attached to
            this tree.
         */
        public void SetItemSelectState(TreeItem item, bool state)
        {
            if (ContainsOpenItemRecursive(d_listItems, item))
            {
                var args=new TreeEventArgs(this);
                args.treeItem = item;

                if (state && !d_multiselect)
                    ClearAllSelectionsImpl();

                item.SetSelected(state);
                d_lastSelected = item.IsSelected() ? item : null;
                OnSelectionChanged(args);
            }
            else
            {
                throw new InvalidRequestException("the specified TreeItem is not attached to this Tree or not visible.");
            }
        }

        /*!
         \brief
            Set the select state of an attached TreeItem.
     
            This is the recommended way of selecting and deselecting items attached
            to a tree as it respects the multi-select mode setting.  It is
            possible to modify the setting on TreeItems directly, but that approach
            does not respect the settings of the tree.
     
         \param item_index
            The zero based index of the TreeItem to be affected.
            This must be a valid index (0 <= index < getItemCount())

         \param state
            - true to select the item.
            - false to de-select the item.
     
         \return
            Nothing.
     
         \exception	InvalidRequestException	thrown if \a item_index is out of range
            for the tree
         */
        public void SetItemSelectState(int item_index, bool state)
        {
            if (item_index < GetItemCount())
            {
                // only do this if the setting is changing
                if (d_listItems[item_index].IsSelected() != state)
                {
                    // conditions apply for single-select mode
                    if (state && !d_multiselect)
                    {
                        ClearAllSelectionsImpl();
                    }
            
                    d_listItems[item_index].SetSelected(state);
                    var args=new TreeEventArgs(this);
                    args.treeItem = d_listItems[item_index];
                    OnSelectionChanged(args);
                }
            }
            else
            {
                throw new InvalidRequestException(
                    "the value passed in the 'item_index' parameter is out of range for this Tree.");
            }
        }
    
        /*!
         \brief
            Set the LookNFeel that shoule be used for this window.
     
         \param look
            String object holding the name of the look to be assigned to the window.
     
         \return
            Nothing.
     
         \exception UnknownObjectException
            thrown if the look'n'feel specified by \a look does not exist.
     
         \note
            Once a look'n'feel has been assigned it is locked - as in cannot be
            changed.
         */

        public override void SetLookNFeel(string look)
        {
            base.SetLookNFeel(look);
            Initialise();
        }

        /*!
         \brief
            Causes the tree to update it's internal state after changes have
            been made to one or more attached TreeItem objects.
     
            Client code must call this whenever it has made any changes to TreeItem
            objects already attached to the tree.  If you are just adding items,
            or removed items to update them prior to re-adding them, there is no
            need to call this method.
     
         \return
            Nothing.
         */
        public void HandleUpdatedItemData()
        {
            ConfigureScrollbars();
            Invalidate(false);
        }

        /// <summary>
        /// Ensure the item at the specified index is visible within the tree.
        /// </summary>
        /// <param name="treeItem">
        /// Pointer to the TreeItem to be made visible in the tree.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this tree.
        /// </exception>
        public void EnsureItemIsVisible(TreeItem treeItem)
        {
            if (treeItem==null)
                return;

            float top = 0;
            if (!GetHeightToItemInList(d_listItems, treeItem, 0, ref top))
                return;  // treeItem wasn't found by getHeightToItemInList

            // calculate height to bottom of item
            float bottom = top + treeItem.GetPixelSize().Height;

            // account for current scrollbar value
            var currPos = d_vertScrollbar.GetScrollPosition();
            top -= currPos;
            bottom -= currPos;

            var listHeight = GetTreeRenderArea().Height;

            // if top is above the view area, or if item is too big to fit
            if ((top < 0.0f) || ((bottom - top) > listHeight))
            {
                // scroll top of item to top of box.
                d_vertScrollbar.SetScrollPosition(currPos + top);
            }
            // if bottom is below the view area
            else if (bottom >= listHeight)
            {
                // position bottom of item at the bottom of the list
                d_vertScrollbar.SetScrollPosition(currPos + bottom - listHeight);
            }
        }
    
        /// <summary>
        /// Constructor for Tree base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Tree(string type, string name)
            : base(type, name)
        {
            d_sorted = false;
            d_multiselect = false;
            d_forceVertScroll = false;
            d_forceHorzScroll = false;
            d_itemTooltips = false;
            d_vertScrollbar = null;
            d_horzScrollbar = null;
            d_lastSelected = null;
            d_openButtonImagery = null;
            d_closeButtonImagery = null;
            d_itemArea = Rectf.Zero;

            // add new events specific to tree.
            AddTreeEvents();
            AddTreeProperties();
        }

        // TODO: virtual ~Tree() { resetList_impl(); } 

        /*!
         \brief
            Return a Rect object describing, in un-clipped pixels, the window
            relative area that is to be used for rendering tree items.
     
         \return
            Rect object describing the area of the Window to be used for rendering
            tree items.
         */
        protected virtual Rectf GetTreeRenderArea()
        {
            return d_itemArea;
        }
    
        /*!
         \brief
            create and return a pointer to a Scrollbar widget for use as vertical
            scroll bar.
     
         \param name
            String holding the name to be given to the created widget component.
     
         \return
            Pointer to a Scrollbar to be used for scrolling the tree vertically.
         */
        protected virtual Scrollbar CreateVertScrollbar(string name)
        {
            return (Scrollbar)GetChild(name);
        }

        /*!
         \brief
            create and return a pointer to a Scrollbar widget for use as horizontal
            scroll bar.
     
         \param name
            String holding the name to be given to the created widget component.
     
         \return
            Pointer to a Scrollbar to be used for scrolling the tree horizontally.
         */
        protected virtual Scrollbar CreateHorzScrollbar(string name)
        {
            return (Scrollbar)GetChild(name);
        }

        /// <summary>
        /// Perform caching of the widget control frame and other 'static' areas.
        /// This method should not render the actual items.  Note that the items
        /// are typically rendered to layer 3, other layers can be used for
        /// rendering imagery behind and infront of the items.
        /// </summary>
        protected virtual void CacheTreeBaseImagery()
        {
        }

        /// <summary>
        /// Checks if a tree item is visible (searches sub-items)
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool ContainsOpenItemRecursive(List<TreeItem> itemList, TreeItem item)
        {
            var itemCount = itemList.Count;
            for (var index = 0; index < itemCount; ++index)
            {
                if (itemList[index] == item)
                    return true;

                if (itemList[index].GetItemCount() > 0)
                {
                    if (itemList[index].GetIsOpen())
                    {
                        if (ContainsOpenItemRecursive(itemList[index].GetItemList(), item))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Add tree specific events
        /// </summary>
        protected void AddTreeEvents()
        {
            // TODO: 
            //addEvent(EventListContentsChanged);
            //addEvent(EventSelectionChanged);
            //addEvent(EventSortModeChanged);
            //addEvent(EventMultiselectModeChanged);
            //addEvent(EventVertScrollbarModeChanged);
            //addEvent(EventHorzScrollbarModeChanged);
            //addEvent(EventBranchOpened);
            //addEvent(EventBranchClosed);
        }
    
        /// <summary>
        /// display required integrated scroll bars according to current state of the tree and update their values.
        /// </summary>
        protected void ConfigureScrollbars()
        {
            var renderArea = GetTreeRenderArea();

            if (d_vertScrollbar == null)
                d_vertScrollbar = CreateVertScrollbar("__auto_vscrollbar__");
            if (d_horzScrollbar == null)
                d_horzScrollbar = CreateHorzScrollbar("__auto_hscrollbar__");
    
            var totalHeight = GetTotalItemsHeight();
            var widestItem  = GetWidestItemWidth() + 20;
    
            //
            // First show or hide the scroll bars as needed (or requested)
            //
            // show or hide vertical scroll bar as required (or as specified by option)
            if ((totalHeight > renderArea.Height) || d_forceVertScroll)
            {
                d_vertScrollbar.Show();
                renderArea.d_max.X -= d_vertScrollbar.GetWidth().d_offset + d_vertScrollbar.GetXPosition().d_offset;
                // show or hide horizontal scroll bar as required (or as specified by option)
                if ((widestItem > renderArea.Width) || d_forceHorzScroll)
                {
                    d_horzScrollbar.Show();
                    renderArea.d_max.Y -= d_horzScrollbar.GetHeight().d_offset;
                }
                else
                {
                    d_horzScrollbar.Hide();
                    d_horzScrollbar.SetScrollPosition(0);
                }
            }
            else
            {
                // show or hide horizontal scroll bar as required (or as specified by option)
                if ((widestItem > renderArea.Width) || d_forceHorzScroll)
                {
                    d_horzScrollbar.Show();
                    renderArea.d_max.Y -= d_vertScrollbar.GetHeight().d_offset;
            
                    // show or hide vertical scroll bar as required (or as specified by option)
                    if ((totalHeight > renderArea.Height) || d_forceVertScroll)
                    {
                        d_vertScrollbar.Show();
                        //            renderArea.d_right -= d_vertScrollbar->getAbsoluteWidth();
                        renderArea.d_max.X -= d_vertScrollbar.GetWidth().d_offset;
                    }
                    else
                    {
                        d_vertScrollbar.Hide();
                        d_vertScrollbar.SetScrollPosition(0);
                    }
                }
                else
                {
                    d_vertScrollbar.Hide();
                    d_vertScrollbar.SetScrollPosition(0);
                    d_horzScrollbar.Hide();
                    d_horzScrollbar.SetScrollPosition(0);
                }
            }
    
            //
            // Set up scroll bar values
            //
    
            float itemHeight;
            if (d_listItems.Count!=0)
                itemHeight = d_listItems[0].GetPixelSize().Height;
            else
                itemHeight = 10;
    
            d_vertScrollbar.SetDocumentSize(totalHeight);
            d_vertScrollbar.SetPageSize(renderArea.Height);
            d_vertScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Height / itemHeight));
            d_vertScrollbar.SetScrollPosition(d_vertScrollbar.GetScrollPosition());
    
            d_horzScrollbar.SetDocumentSize(widestItem + d_vertScrollbar.GetWidth().d_offset);
            d_horzScrollbar.SetPageSize(renderArea.Width);
            d_horzScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Width / HORIZONTAL_STEP_SIZE_DIVISOR));
            d_horzScrollbar.SetScrollPosition(d_horzScrollbar.GetScrollPosition());
        }
    
        /*!
         \brief
            select all strings between positions \a start and \a end.  (inclusive)
            including \a end.
         */
        protected void SelectRange(int start, int end)
        {
            // only continue if list has some items
            if (d_listItems.Count!=0)
            {
                // if start is out of range, start at begining.
                if (start > d_listItems.Count)
                    start = 0;
                
                // if end is out of range end at the last item.
                if (end >= d_listItems.Count)
                    end = d_listItems.Count - 1;
                
                // ensure start becomes before the end.
                if (start > end)
                {
                    var tmp = start;
                    start = end;
                    end = tmp;
                }

                // perform selections
                for (; start <= end; ++start)
                {
                    d_listItems[start].SetSelected(true);
                }
            }
        }
    
        /*!
         \brief
            Return the sum of all item heights
         */
        protected float GetTotalItemsHeight()
        {
            float heightSum = 0;

            GetTotalItemsInListHeight(d_listItems, ref heightSum);
            return heightSum;
        }

        protected void GetTotalItemsInListHeight(List<TreeItem> itemList, ref float heightSum)
        {
            var itemCount = itemList.Count;
            for (var index = 0; index < itemCount; ++index)
            {
                heightSum += itemList[index].GetPixelSize().Height;
                if (itemList[index].GetIsOpen() && (itemList[index].GetItemCount() > 0))
                    GetTotalItemsInListHeight(itemList[index].GetItemList(), ref heightSum);
            }
        }

        /*!
         \brief
            Return the width of the widest item
         */
        protected float GetWidestItemWidth()
        {
            float widest = 0;

            GetWidestItemWidthInList(d_listItems, 0, ref widest);

            return widest;
        }
        protected void GetWidestItemWidthInList(List<TreeItem> itemList, int itemDepth, ref float widest)
        {
            var itemCount = itemList.Count;
            for (var index = 0; index < itemCount; ++index)
            {
                var buttonLocation = itemList[index].GetButtonLocation();
                float thisWidth = itemList[index].GetPixelSize().Width +
                buttonLocation.Width +
                (d_horzScrollbar.GetScrollPosition() / HORIZONTAL_STEP_SIZE_DIVISOR) +
                (itemDepth * 20);

                if (thisWidth > widest)
                    widest = thisWidth;

                if (itemList[index].GetIsOpen() && (itemList[index].GetItemCount() > 0))
                    GetWidestItemWidthInList(itemList[index].GetItemList(), itemDepth + 1, ref widest);
            }
        }
    
        /*!
         \brief
            Clear the selected state for all items (implementation)
     
         \return
            - true if treeItem was found in the search.
            - false if it was not.
         */
        protected bool GetHeightToItemInList(List<TreeItem> itemList, TreeItem treeItem, int itemDepth, ref float height)
        {
            var itemCount = itemList.Count;
            for (var index = 0; index < itemCount; ++index)
            {
                if (treeItem == itemList[index])
                    return true;

                height += itemList[index].GetPixelSize().Height;

                if (itemList[index].GetIsOpen() && (itemList[index].GetItemCount() > 0))
                {
                    if (GetHeightToItemInList(itemList[index].GetItemList(), treeItem, itemDepth + 1, ref height))
                        return true;
                }
            }

            return false;
        }

        /*!
         \brief
            Clear the selected state for all items (implementation)
     
         \return
            - true if some selections were cleared
            - false nothing was changed.
         */
        protected bool ClearAllSelectionsImpl()
        {
            return ClearAllSelectionsFromList(d_listItems);
        }

        /*!
         \brief
            Return the TreeItem under the given window local pixel co-ordinate.
     
         \return
             TreeItem that is under window pixel co-ordinate \a pt, or 0 if no
             item is under that position.
         */
        protected TreeItem GetItemAtPoint(Lunatics.Mathematics.Vector2 pt)
        {
            var renderArea = GetTreeRenderArea();
    
            // point must be within the rendering area of the Tree.
            if (renderArea.IsPointInRect(pt))
            {
                float y = renderArea.Top - d_vertScrollbar.GetScrollPosition();
        
                // test if point is above first item
                if (pt.Y >= y)
                    return GetItemFromListAtPoint(d_listItems, ref y, pt);
            }
    
            return null;
        }

        protected TreeItem GetItemFromListAtPoint(List<TreeItem> itemList, ref float bottomY, Lunatics.Mathematics.Vector2 pt)
        {
            var itemCount = itemList.Count;

            for (var i = 0; i < itemCount; ++i)
            {
                bottomY += itemList[i].GetPixelSize().Height;
                if (pt.Y < bottomY)
                    return itemList[i];

                if (itemList[i].GetItemCount() > 0)
                {
                    if (itemList[i].GetIsOpen())
                    {
                        var foundPointedAtTree = GetItemFromListAtPoint(itemList[i].GetItemList(), ref bottomY, pt);
                        if (foundPointedAtTree != null)
                            return foundPointedAtTree;
                    }
                }
            }

            return null;
        }

        /*!
         \brief
            Remove all items from the tree.
     
         \note
            Note that this will cause 'AutoDelete' items to be deleted.
     
         \return
             - true if the tree contents were changed.
             - false if the tree contents were not changed (tree already empty).
         */
        protected bool ResetListImpl()
        {
            // just return false if the list is already empty
            if (GetItemCount() == 0)
            {
                return false;
            }
            // we have items to be removed and possible deleted
            else
            {
                // delete any items we are supposed to
                for (var i = 0; i < GetItemCount(); ++i)
                {
                    // if item is supposed to be deleted by us
                    if (d_listItems[i].IsAutoDeleted())
                    {
                        // clean up this item.
                        // TODO: CEGUI_DELETE_AO d_listItems[i];
                    }
                }
        
                // clear out the list.
                d_listItems.Clear();
                d_lastSelected = null;
                return true;
            }
        }
    
        /// <summary>
        /// Internal handler that is triggered when the user interacts with the scrollbars.
        /// </summary>
        /// <param name="args"></param>
        protected bool HandleScrollChange(EventArgs args)
        {
            // simply trigger a redraw of the Tree.
            Invalidate(false);
            return true;
        }

        protected override void PopulateGeometryBuffer()
        {
            // get the derived class to render general stuff before we handle the items
            CacheTreeBaseImagery();

            // Render list items
            Lunatics.Mathematics.Vector2 itemPos;
            float widest = GetWidestItemWidth();

            // calculate position of area we have to render into
            //Rect itemsArea(getTreeRenderArea());
            //Rect itemsArea(0,0,500,500);

            // set up some initial positional details for items
            itemPos.X = d_itemArea.Left - d_horzScrollbar.GetScrollPosition();
            itemPos.Y = d_itemArea.Top - d_vertScrollbar.GetScrollPosition();

            DrawItemList(d_listItems, d_itemArea, widest, ref itemPos, GetGeometryBuffers(), GetEffectiveAlpha());
        }

        protected override void HandleFontRenderSizeChange(object sender, FontEventArgs args)
        {
            base.HandleFontRenderSizeChange(sender, args);

            if (args.handled==0)
            {
                var font = args.font;
                var res = false;
                for (var i = 0; i < GetItemCount(); ++i)
                    res |= d_listItems[i].HandleFontRenderSizeChange(font);

                if (res)
                    Invalidate(false);

                args.handled += res ? 1 : 0;
            }
            
            // TODO: return res;
        }

        protected void DrawItemList(List<TreeItem> itemList, Rectf itemsArea, float widest, ref Lunatics.Mathematics.Vector2 itemPos, List<GeometryBuffer> geometryBuffers, float alpha)
        {
            if (itemList.Count==0)
                return;

            // loop through the items
            var itemCount = itemList.Count;
            for (var i = 0; i < itemCount; ++i)
            {
                var itemSize = new Sizef
                                   {
                                       Height = itemList[i].GetPixelSize().Height,
                                       // allow item to have full width of box if this is wider than items
                                       Width = Math.Max(itemsArea.Width, widest)
                                   };

                

                // calculate destination area for this item.
                var itemRect=new Rectf {Left = itemPos.X, Top = itemPos.Y, Size = itemSize};
                var itemClipper = itemRect.GetIntersection(itemsArea);
                itemRect.d_min.X += 20f; // start text past open/close buttons

                bool itemIsVisible;
                if (itemClipper.Height > 0)
                {
                    itemIsVisible = true;
                    AppendGeometryBuffers(itemList[i].Draw(itemRect, alpha, itemClipper));
                }
                else
                {
                    itemIsVisible = false;
                }

                // Process this item's list if it has items in it.
                if (itemList[i].GetItemCount() > 0)
                {
                    var buttonRenderRect = new Rectf();
                    buttonRenderRect.Left = itemPos.X;
                    buttonRenderRect.Right = buttonRenderRect.Left + 10;
                    buttonRenderRect.Top = itemPos.Y;
                    buttonRenderRect.Bottom = buttonRenderRect.Top + 10;
                    itemList[i].SetButtonLocation(buttonRenderRect);

                    if (itemList[i].GetIsOpen())
                    {
                        // Draw the Close button
                        if (itemIsVisible)
                            d_closeButtonImagery.Render(this, buttonRenderRect, null, itemClipper);

                        // update position ready for next item
                        itemPos.Y += itemSize.Height;

                        itemPos.X += 20;
                        DrawItemList(itemList[i].GetItemList(), itemsArea, widest, ref itemPos, geometryBuffers, alpha);
                        itemPos.X -= 20;
                    }
                    else
                    {
                        // Draw the Open button
                        if (itemIsVisible)
                            d_openButtonImagery.Render(this, buttonRenderRect, null, itemClipper);

                        // update position ready for next item
                        itemPos.Y += itemSize.Height;
                    }
                }
                else
                {
                    // update position ready for next item
                    itemPos.Y += itemSize.Height;
                }
            }

            // Successfully drew all items, so vertical scrollbar not needed.
            //   setShowVertScrollbar(false);
        }

        /// <summary>
        /// Handler called internally when the tree contents are changed
        /// </summary>
        /// <param name="e"></param>
        internal virtual void OnListContentsChanged(WindowEventArgs e)
        {
            ConfigureScrollbars();
            Invalidate(false);
            FireEvent(ListContentsChanged, e);
        }

        /// <summary>
        /// Handler called internally when the currently selected item or items changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(TreeEventArgs e)
        {
            Invalidate(false);
            FireEvent(SelectionChanged, e);
        }

        /// <summary>
        /// Handler called internally when the sort mode setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SortModeChanged, e);
        }

        /// <summary>
        /// Handler called internally when the multi-select mode setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual	void OnMultiselectModeChanged(WindowEventArgs e)
        {
            FireEvent(MultiselectModeChanged, e);
        }

        /// <summary>
        /// Handler called internally when the forced display of the vertical scroll bar setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual	void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(VertScrollbarModeChanged, e);
        }

        /// <summary>
        /// Handler called internally when the forced display of the horizontal scroll bar setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(HorzScrollbarModeChanged, e);
        }
    
        /// <summary>
        /// Handler called internally when the user opens a branch of the tree.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBranchOpened(TreeEventArgs e)
        {
            Invalidate(false);
            FireEvent(BranchOpened, e);
        }
        
        /// <summary>
        /// Handler called internally when the user closes a branch of the tree.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBranchClosed(TreeEventArgs e)
        {
            Invalidate(false);
            FireEvent(BranchClosed, e);
        }

        protected override void OnSized(ElementEventArgs e)
        {
            // base class handling
            base.OnSized(e);
    
            ConfigureScrollbars();
    
            ++e.handled;
        }


        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class processing
            // populateGeometryBuffer();
            base.OnCursorPressHold(e);
    
            if (e.Source == CursorInputSource.Left)
            {
                //bool modified = false;
        
                var localPos = (CoordConverter.ScreenToWindow(this, e.Position));
        
                var item = GetItemAtPoint(localPos);
        
                if (item != null)
                {
                    //modified = true;
                    var args = new TreeEventArgs(this) {treeItem = item};
                    PopulateGeometryBuffer();
                    var buttonLocation = item.GetButtonLocation();
                    if ((localPos.X >= buttonLocation.Left) && (localPos.X <= buttonLocation.Right) &&
                        (localPos.Y >= buttonLocation.Top) && (localPos.Y <= buttonLocation.Bottom))
                    {
                        item.ToggleIsOpen();
                        if (item.GetIsOpen())
                        {
                            var lastItemInList = item.GetTreeItemFromIndex(item.GetItemCount() - 1);
                            EnsureItemIsVisible(lastItemInList);
                            EnsureItemIsVisible(item);
                            OnBranchOpened(args);
                        }
                        else
                        {
                            OnBranchClosed(args);
                        }
                
                        // Update the item screen locations, needed to update the scrollbars.
                        //	populateGeometryBuffer();
                
                        // Opened or closed a tree branch, so must update scrollbars.
                        ConfigureScrollbars();
                    }
                    else
                    {
                        // TODO: ...
                        //// clear old selections if no control key is pressed or if multi-select is off
                        //if ((e.sysKeys & (uint)SystemKey.Control) != (uint)SystemKey.Control || !d_multiselect)
                        //    ClearAllSelectionsImpl();
                
                        //// select range or item, depending upon keys and last selected item
                        //// TODO: fix this
                        ////if (((e.sysKeys & Shift) && (d_lastSelected != 0)) && d_multiselect)
                        ////    selectRange(getItemIndex(item), getItemIndex(d_lastSelected));
                        ////else
                        //    item.SetSelected(item.IsSelected() ^ true);
                
                        //// update last selected item
                        //d_lastSelected = item.IsSelected() ? item : null;
                        //OnSelectionChanged(args);
                    }
                }
                else
                {
                    // TODO: ...
                    //// clear old selections if no control key is pressed or if multi-select is off
                    //if ((e.sysKeys & (uint)SystemKey.Control) != (uint)SystemKey.Control  || !d_multiselect)
                    //{
                    //    if (ClearAllSelectionsImpl())
                    //    {
                    //        // Changes to the selections were actually made
                    //        var args=new TreeEventArgs(this);
                    //        args.treeItem = item;
                    //        OnSelectionChanged(args);
                    //    }
                    //}
                }
        
        
                ++e.handled;
            }
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing.
            base.OnScroll(e);
    
            if (d_vertScrollbar.IsEffectiveVisible() && (d_vertScrollbar.GetDocumentSize() > d_vertScrollbar.GetPageSize()))
                d_vertScrollbar.SetScrollPosition(d_vertScrollbar.GetScrollPosition() + d_vertScrollbar.GetStepSize() * -e.scroll);
            else if (d_horzScrollbar.IsEffectiveVisible() && (d_horzScrollbar.GetDocumentSize() > d_horzScrollbar.GetPageSize()))
                d_horzScrollbar.SetScrollPosition(d_horzScrollbar.GetScrollPosition() + d_horzScrollbar.GetStepSize() * -e.scroll);
    
            ++e.handled;
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            if (d_itemTooltips)
            {
                var posi = CoordConverter.ScreenToWindow(this, e.Position);
                var item = GetItemAtPoint(posi);
                if (item != _lastItem)
                {
                    SetTooltipText(item != null ? item.GetTooltipText() : "");
                    _lastItem = item;
                }

                // must check the result from getTooltip(), as the tooltip object could
                // be 0 at any time for various reasons.
                var tooltip = GetTooltip();
        
                if (tooltip!=null)
                {
                    if (tooltip.GetTargetWindow() != this)
                        tooltip.SetTargetWindow(this);
                    else
                        tooltip.PositionSelf();
                }
            }
    
            base.OnCursorMove(e);
        }

        private void AddTreeProperties()
        {
            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Tree, bool>(
                            "Sort",
                            "Property to get/set the sort setting of the tree.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetSortingEnabled(v), x => x.IsSortEnabled(), WidgetTypeName));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Tree, bool>(
                            "MultiSelect",
                            "Property to get/set the multi-select setting of the tree.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetMultiselectEnabled(v), x => x.IsMultiselectEnabled(), WidgetTypeName));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Tree, bool>(
                            "ForceVertScrollbar",
                            "Property to get/set the 'always show' setting for the vertical scroll bar of the tree.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), WidgetTypeName));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Tree, bool>(
                            "ForceHorzScrollbar",
                            "Property to get/set the 'always show' setting for the horizontal scroll bar of the tree.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), WidgetTypeName));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Tree, bool>(
                            "ItemTooltips",
                            "Property to access the show item tooltips setting of the tree.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetItemTooltipsEnabled(v), x => x.IsItemTooltipsEnabled(), WidgetTypeName));
        }

        private Rectf d_itemArea;

        #region Fields

        //! true if tree is sorted
        bool d_sorted;
        //! true if multi-select is enabled
        bool d_multiselect;
        //! true if vertical scrollbar should always be displayed
        bool d_forceVertScroll;
        //! true if horizontal scrollbar should always be displayed
        bool d_forceHorzScroll;
        //! true if each item should have an individual tooltip
        bool d_itemTooltips;
        //! vertical scroll-bar widget
        Scrollbar d_vertScrollbar;
        //! horizontal scroll-bar widget
        Scrollbar d_horzScrollbar;
        //! list of items in the tree.
        List<TreeItem> d_listItems=new List<TreeItem>();
        //! holds pointer to the last selected item (used in range selections)
        TreeItem d_lastSelected;
        ImagerySection d_openButtonImagery;
        ImagerySection d_closeButtonImagery;

        private TreeItem _lastItem;

        private const float HORIZONTAL_STEP_SIZE_DIVISOR = 20.0f;

        #endregion
    }
}