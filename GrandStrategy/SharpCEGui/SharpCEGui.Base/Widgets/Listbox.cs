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
using System.Linq;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for Listbox window renderer.
    /// </summary>
    public abstract class ListboxWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ListboxWindowRenderer(string name)
            : base(name, Listbox.EventNamespace)
        {
            
        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window
        /// relative area that is to be used for rendering list items.
        /// </summary>
        /// <returns></returns>
        public abstract Rectf GetListRenderArea();
        
        /// <summary>
        /// Resize the Listbox the renderer is attached to such that it's
        /// content can be displayed without needing scrollbars if there is
        /// enough space, otherwise make the Listbox as large as possible
        /// (without moving it).
        /// </summary>
        /// <param name="fitWidth"></param>
        /// <param name="fitHeight"></param>
        public abstract void ResizeListToContent(bool fitWidth, bool fitHeight);
    }

    /// <summary>
    /// Base class for standard Listbox widget.
    /// </summary>
    public class Listbox : Window
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Listbox";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Listbox";

        public const string EventListContentsChanged= "ListContentsChanged" ;
        public const string EventSelectionChanged= "SelectionChanged" ;
        public const string EventSortModeChanged= "SortModeChanged" ;
        public const string EventMultiselectModeChanged= "MultiselectModeChanged" ;
        public const string EventVertScrollbarModeChanged= "VertScrollbarModeChanged" ;
        public const string EventHorzScrollbarModeChanged= "HorzScrollbarModeChanged" ;

        /// <summary>
        /// Event fired when the contents of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Listbox whose content is changed.
        /// </summary>
	    public event GuiEventHandler<EventArgs> ListContentsChanged
        {
            add { SubscribeEvent(EventListContentsChanged, value); }
            remove { UnsubscribeEvent(EventListContentsChanged, value); }
        }
        
        /// <summary>
        /// Event fired when there is a change to the currently selected item(s)
        /// within the list.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Litbox that has had a change in the
        /// selected items.
        /// </summary>
        public event GuiEventHandler<EventArgs> SelectionChanged
        {
            add { SubscribeEvent(EventSelectionChanged, value); }
            remove { UnsubscribeEvent(EventSelectionChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the sort mode setting changes for the Listbox.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Listbox whose sort mode has been
        /// changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SortModeChanged
        {
            add { SubscribeEvent(EventSortModeChanged, value); }
            remove { UnsubscribeEvent(EventSortModeChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the multi-select mode setting changes for the Listbox.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Listbox whose multi-select mode has
        /// been changed.
        /// </summary>
	    public event GuiEventHandler<EventArgs> MultiselectModeChanged
        {
            add { SubscribeEvent(EventMultiselectModeChanged, value); }
            remove { UnsubscribeEvent(EventMultiselectModeChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the mode setting that forces the display of the
        /// vertical scroll bar for the Listbox is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Listbox whose vertical
        /// scrollbar mode has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> VertScrollbarModeChanged
        {
            add { SubscribeEvent(EventVertScrollbarModeChanged, value); }
            remove { UnsubscribeEvent(EventVertScrollbarModeChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the mode setting that forces the display of the
        /// horizontal scroll bar for the Listbox is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Listbox whose horizontal
        /// scrollbar mode has been changed.
        /// </summary>
	    public event GuiEventHandler<EventArgs> HorzScrollbarModeChanged
        {
            add { SubscribeEvent(EventHorzScrollbarModeChanged, value); }
            remove { UnsubscribeEvent(EventHorzScrollbarModeChanged, value); }
        }

        #endregion

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        /// <summary>
        /// Return number of items attached to the list box
        /// </summary>
        /// <returns>
        /// the number of items currently attached to this list box.
        /// </returns>
        public int GetItemCount()
        {
            return _listItems.Count;
        }

        /// <summary>
        /// Return the number of selected items in the list box.
        /// </summary>
        /// <returns>
        /// Total number of attached items that are in the selected state.
        /// </returns>
        public int GetSelectedCount()
        {
            return _listItems.Count(t => t.IsSelected());
        }

        /// <summary>
        /// Return a pointer to the first selected item.
        /// </summary>
        /// <returns>
        /// Pointer to a ListboxItem based object that is the first selected item in the list.
        /// will return NULL if no item is selected.
        /// </returns>
	    public ListboxItem GetFirstSelectedItem()
	    {
            return GetNextSelected(null);
	    }

        /// <summary>
        /// Return a pointer to the next selected item after item \a start_item
        /// </summary>
        /// <param name="startItem">
        /// Pointer to the ListboxItem where the search for the next selected item is to begin.  
        /// If this parameter is NULL, the search will begin with the first item in the list box.
        /// </param>
        /// <returns>
        /// Pointer to a ListboxItem based object that is the next selected item in the list after
        /// the item specified by \a start_item.  Will return NULL if no further items were selected.</returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a start_item is not attached to this list box.
        /// </exception>
	    public ListboxItem GetNextSelected(ListboxItem startItem)
	    {
            // if start_item is NULL begin search at begining, else start at item after start_item
            var index = (startItem == null) ? 0 : (GetItemIndex(startItem) + 1);

            while (index < _listItems.Count)
            {
                // return pointer to this item if it's selected.
                if (_listItems[index].IsSelected())
                {
                    return _listItems[index];
                }
             
                // not selected, advance to next
                index++;
            }

            // no more selected items.
            return null;
	    }

        /// <summary>
        /// Return the item at index position \a index.
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be returned.
        /// </param>
        /// <returns>
        /// Pointer to the ListboxItem at index position \a index in the list box.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public ListboxItem GetListboxItemFromIndex(int index)
        {
            if (index < _listItems.Count)
                return _listItems[index];

            throw new InvalidRequestException("the specified index is out of range for this Listbox.");
        }

        /// <summary>
        /// Return the index of ListboxItem \a item
        /// </summary>
        /// <param name="item">
        /// Pointer to a ListboxItem whos zero based index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based index indicating the position of ListboxItem \a item in the list box.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
	    public int GetItemIndex(ListboxItem item)
	    {
            var idx = _listItems.IndexOf(item);
            if (idx == -1)
                throw new InvalidRequestException("the specified ListboxItem is not attached to this Listbox.");

            return idx;
	    }

        /// <summary>
        /// return whether list sorting is enabled
        /// </summary>
        /// <returns>
        /// true if the list is sorted, 
        /// false if the list is not sorted
        /// </returns>
	    public bool IsSortEnabled()
	    {
	        return _sorted;
	    }

        /// <summary>
        /// return whether multi-select is enabled
        /// </summary>
        /// <returns>
        /// true if multi-select is enabled, false if multi-select is not enabled.
        /// </returns>
        public bool IsMultiselectEnabled()
        {
            return _multiselect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    public bool IsItemTooltipsEnabled()
	    {
	        return _itemTooltips;
	    }

        /// <summary>
        /// return whether the string at index position \a index is selected
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be examined.
        /// </param>
        /// <returns>
        /// true if the item at \a index is selected, false if the item at \a index is not selected.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
	    public bool IsItemSelected(int index)
	    {
            if (index < _listItems.Count)
                return _listItems[index].IsSelected();

            throw new InvalidRequestException("the specified index is out of range for this Listbox.");
	    }

        /// <summary>
        /// Search the list for an item with the specified text
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="startItem">
        /// ListboxItem where the search is to begin, the search will not include \a item.  If \a item is
        /// NULL, the search will begin from the first item in the list.</param>
        /// <returns>
        /// Pointer to the first ListboxItem in the list after \a item that has text matching \a text.  If
        /// no item matches the criteria NULL is returned.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
	    public ListboxItem FindItemWithText(string text, ListboxItem startItem)
	    {
            // if start_item is NULL begin search at begining, else start at item after start_item
            var index = (startItem==null) ? 0 : (GetItemIndex(startItem) + 1);

            while (index < _listItems.Count)
            {
                // return pointer to this item if it's text matches
                if (_listItems[index].GetText() == text)
                {
                    return _listItems[index];
                }
             
                // no matching text, advance to next item
                index++;
            }
            // no items matched.
            return null;
	    }

        /// <summary>
        /// Return whether the specified ListboxItem is in the List
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// true if ListboxItem \a item is in the list, false if ListboxItem \a item is not in the list.
        /// </returns>
	    public bool IsListboxItemInList(ListboxItem item)
        {
            return _listItems.Contains(item);
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
            return _forceVertScroll;
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
            return _forceHorzScroll;
        }

        /// <summary>
        /// Initialise the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.
        /// Normally this is handled automatically by the WindowFactory for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            // get the component sub-widgets
	        var vertScrollbar = GetVertScrollbar();
	        var horzScrollbar = GetHorzScrollbar();

            vertScrollbar.ScrollPositionChanged += HandleScrollChange;
            horzScrollbar.ScrollPositionChanged += HandleScrollChange;

	        ConfigureScrollbars();
	        PerformChildWindowLayout();
        }

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </para>
        /// </summary>
        public void ResetList()
        {
            if (ResetListImpl())
                OnListContentsChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Add the given ListboxItem to the list.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be added to the list.  
        /// Note that it is the passed object that is added to the
        /// list, a copy is not made.  
        /// If this parameter is NULL, nothing happens.
        /// </param>
	    public void AddItem(ListboxItem item)
	    {
	        if (item!=null)
	        {
		        // establish ownership
		        item.SetOwnerWindow(this);

		        // if sorting is enabled, re-sort the list
		        if (IsSortEnabled())
		        {
			        // TODO: d_listItems.insert(std::upper_bound(d_listItems.begin(), d_listItems.end(), item, &lbi_less), item);
                    _listItems.Add(item);
		        }
		        // not sorted, just stick it on the end.
		        else
		        {
			        _listItems.Add(item);
		        }

		        OnListContentsChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Insert an item into the list box before a specified item already in the list.
        /// <para>
        /// Note that if the list is sorted, the item may not end up in the
        /// requested position.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be inserted.  Note that it is the passed
        /// object that is added to the list, a copy is not made.  If this parameter
        /// is NULL, nothing happens.
        /// </param>
        /// <param name="position">
        /// Pointer to a ListboxItem that \a item is to be inserted before.  If this
        /// parameter is NULL, the item is inserted at the start of the list.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if no ListboxItem \a position is attached to this list box.
        /// </exception>
	    public void InsertItem(ListboxItem item, ListboxItem position)
	    {
	        // if the list is sorted, it's the same as a normal add operation
	        if (IsSortEnabled())
	        {
		        AddItem(item);
	        }
	        else if (item!=null)
	        {
		        // establish ownership
		        item.SetOwnerWindow(this);

		        // if position is NULL begin insert at begining, else insert after item 'position'
		        var insPos = position == null ? 0 : _listItems.IndexOf(position);

		        // throw if item 'position' is not in the list
			    if (insPos == -1)
			    {
			        throw new InvalidRequestException(
			            "the specified ListboxItem for parameter 'position' is not attached to this Listbox.");
			    }

		        _listItems.Insert(insPos, item);

		        OnListContentsChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Removes the given item from the list box.  If the item is has the auto delete state set, the item will be deleted.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem that is to be removed.  If \a item is not attached to this list box then nothing
        /// will happen.
        /// </param>
	    public void RemoveItem(ListboxItem item)
	    {
	        if (item!=null)
	        {
		        // if item is in the list
		        if (_listItems.Contains(item))
		        {
			        // disown item
			        item.SetOwnerWindow(null);

			        // remove item
			        _listItems.Remove(item);

			        // if item was the last selected item, reset that to NULL
			        if (item == _lastSelected)
			        {
				        _lastSelected = null;
			        }

			        // if item is supposed to be deleted by us
			        if (item.IsAutoDeleted())
			        {
				        // clean up this item.
				        // TODO: CEGUI_DELETE_AO item;
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
		        OnSelectionChanged(new WindowEventArgs(this));
	    }

        /// <summary>
        /// Set whether the list should be sorted.
        /// </summary>
        /// <param name="setting">
        /// true if the list should be sorted, 
        /// false if the list should not be sorted.
        /// </param>
	    public void SetSortingEnabled(bool setting)
	    {
	        // only react if the setting will change
	        if (_sorted != setting)
	        {
		        _sorted = setting;

		        // if we are enabling sorting, we need to sort the list
		        if (_sorted)
	                ResortList();

		        OnSortModeChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set whether the list should allow multiple selections or just a single selection
        /// </summary>
        /// <param name="setting">
        /// true if the widget should allow multiple items to be selected, false if the widget should only allow
        /// a single selection.
        /// </param>
	    public void SetMultiselectEnabled(bool setting)
	    {
	        // only react if the setting is changed
	        if (_multiselect != setting)
	        {
		        _multiselect = setting;

		        // if we change to single-select, deselect all except the first selected item.
                var args=new WindowEventArgs(this);
		        if ((!_multiselect) && (GetSelectedCount() > 1))
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

        /// <summary>
        /// Set whether the vertical scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// true if the vertical scroll bar should be shown even when it is not required.  
        /// false if the verticalscroll bar should only be shown when it is required.
        /// </param>
	    public void SetShowVertScrollbar(bool setting)
	    {
	        if (_forceVertScroll != setting)
	        {
		        _forceVertScroll = setting;

		        ConfigureScrollbars();
		        OnVertScrollbarModeChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Set whether the horizontal scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// true if the horizontal scroll bar should be shown even when it is not required.
        /// false if the horizontal scroll bar should only be shown when it is required.
        /// </param>
        public void SetShowHorzScrollbar(bool setting)
        {
            if (_forceHorzScroll != setting)
	        {
		        _forceHorzScroll = setting;

		        ConfigureScrollbars();
		        OnHorzScrollbarModeChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public void SetItemTooltipsEnabled(bool setting)
        {
            _itemTooltips = setting;
        }

        /// <summary>
        /// Set the select state of an attached ListboxItem.
        /// <para>
        /// This is the recommended way of selecting and deselecting items attached to a list box as it respects the
        /// multi-select mode setting.  It is possible to modify the setting on ListboxItems directly, but that approach
        /// does not respect the settings of the list box.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// The ListboxItem to be affected. This item must be attached to the list box.
        /// </param>
        /// <param name="state">
        /// true to select the item, false to de-select the item.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
	    public void SetItemSelectState(ListboxItem item, bool state)
        {
            var idx = _listItems.IndexOf(item);
            if (idx != -1)
            {
                SetItemSelectState(idx, state);
            }
            else
            {
                throw new InvalidRequestException("the specified ListboxItem is not attached to this Listbox.");
            }
        }

        /// <summary>
        /// Set the select state of an attached ListboxItem.
        /// <para>
        /// This is the recommended way of selecting and deselecting items attached to a list box as it respects the
        /// multi-select mode setting.  It is possible to modify the setting on ListboxItems directly, but that approach
        /// does not respect the settings of the list box.
        /// </para>
        /// </summary>
        /// <param name="itemIndex">
        /// The zero based index of the ListboxItem to be affected.  
        /// This must be a valid index (0 lt;= index lt; getItemCount())
        /// </param>
        /// <param name="state">
        /// true to select the item, false to de-select the item.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item_index is out of range for the list box
        /// </exception>
	    public void SetItemSelectState(int itemIndex, bool state)
	    {
	        if (itemIndex < GetItemCount())
	        {
		        // only do this if the setting is changing
		        if (_listItems[itemIndex].IsSelected() != state)
		        {
			        // conditions apply for single-select mode
			        if (state && !_multiselect)
			        {
				        ClearAllSelectionsImpl();
			        }

			        _listItems[itemIndex].SetSelected(state);
			        OnSelectionChanged(new WindowEventArgs(this));
		        }

	        }
	        else
	        {
	            throw new InvalidRequestException(
	                "the value passed in the 'item_index' parameter is out of range for this Listbox.");
	        }
	    }

        /// <summary>
        /// Causes the list box to update it's internal state after changes have been made to one or more
        /// attached ListboxItem objects.
        /// <para>
        /// Client code must call this whenever it has made any changes to ListboxItem objects already attached to the
        /// list box.  If you are just adding items, or removed items to update them prior to re-adding them, there is
        /// no need to call this method.
        /// </para>
        /// </summary>
	    public void HandleUpdatedItemData()
	    {
            if (_sorted)
                ResortList();

            ConfigureScrollbars();
            Invalidate(false);
	    }

        /// <summary>
        /// Ensure the item at the specified index is visible within the list box.
        /// </summary>
        /// <param name="itemIndex">
        /// Zero based index of the item to be made visible in the list box.  
        /// If this value is out of range, the list is always scrolled to the bottom.
        /// </param>
	    public void EnsureItemIsVisible(int itemIndex)
	    {
            var vertScrollbar = GetVertScrollbar();

            // handle simple "scroll to the bottom" case
            if (itemIndex >= GetItemCount())
            {
                vertScrollbar.SetScrollPosition(vertScrollbar.GetDocumentSize() - vertScrollbar.GetPageSize());
            }
            else
            {
                var listHeight = GetListRenderArea().Height;
                var top = 0f;

                // get height to top of item
                int i;
                for (i = 0; i < itemIndex; ++i)
                {
                    top += _listItems[i].GetPixelSize().Height;
                }

                // calculate height to bottom of item
                var bottom = top + _listItems[i].GetPixelSize().Height;

                // account for current scrollbar value
                float currPos = vertScrollbar.GetScrollPosition();
                top -= currPos;
                bottom -= currPos;

                // if top is above the view area, or if item is too big to fit
                if ((top < 0.0f) || ((bottom - top) > listHeight))
                {
                    // scroll top of item to top of box.
                    vertScrollbar.SetScrollPosition(currPos + top);
                }
                // if bottom is below the view area
                else if (bottom >= listHeight)
                {
                    // position bottom of item at the bottom of the list
                    vertScrollbar.SetScrollPosition(currPos + bottom - listHeight);
                }

                // Item is already fully visible - nothing more to do.
            }
	    }

        /// <summary>
        /// Ensure the item at the specified index is visible within the list box.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ListboxItem to be made visible in the list box.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
	    public void EnsureItemIsVisible(ListboxItem item)
	    {
            EnsureItemIsVisible(GetItemIndex(item));
	    }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering list items.
        /// </summary>
        /// <returns>
        /// Rect object describing the area of the Window to be used for rendering
        /// list box items.
        /// </returns>
        public virtual Rectf GetListRenderArea()
        {
            if (d_windowRenderer != null)
            {
                var wr = (ListboxWindowRenderer)d_windowRenderer;
                return wr.GetListRenderArea();
            }

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// Return a pointer to the vertical scrollbar component widget for this Listbox.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the vertical Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetVertScrollbar()
        {
            return (Scrollbar) GetChild(VertScrollbarName);
        }

        /// <summary>
        /// Return a pointer to the horizontal scrollbar component widget for this
        /// Listbox.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the horizontal Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar) GetChild(HorzScrollbarName);
        }

        /// <summary>
        /// Return the sum of all item heights
        /// </summary>
        /// <returns></returns>
        public float GetTotalItemsHeight()
        {
            float height = 0;

            for (var i = 0; i < GetItemCount(); ++i)
            {
                height += _listItems[i].GetPixelSize().Height;
            }

            return height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Return the width of the widest item
        /// </returns>
        public float GetWidestItemWidth()
        {
            float widest = 0;

            for (var i = 0; i < GetItemCount(); ++i)
            {
                var thisWidth = _listItems[i].GetPixelSize().Width;

                if (thisWidth > widest)
                {
                    widest = thisWidth;
                }

            }

            return widest;
        }

        /// <summary>
        /// Return a pointer to the ListboxItem attached to this Listbox at the
        /// given screen pixel co-ordinate.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns>
        /// Pointer to the ListboxItem attached to this Listbox that is at screen
        /// position \a pt, or 0 if no ListboxItem attached to this Listbox is at
        /// that position.</returns>
        public ListboxItem GetItemAtPoint(Lunatics.Mathematics.Vector2 pt)
        {
            var localPos = CoordConverter.ScreenToWindow(this, pt);
            var renderArea = GetListRenderArea();

	        // point must be within the rendering area of the Listbox.
	        if (renderArea.IsPointInRect(localPos))
	        {
		        var y = renderArea.d_min.X - GetVertScrollbar().GetScrollPosition();

		        // test if point is above first item
		        if (localPos.Y >= y)
		        {
			        for (var i = 0; i < GetItemCount(); ++i)
			        {
				        y += _listItems[i].GetPixelSize().Height;

				        if (localPos.Y < y)
				        {
					        return _listItems[i];
				        }

			        }
		        }
	        }

	        return null;
        }

        /// <summary>
        /// Constructor for Listbox base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Listbox(string type, string name):base(type,name)
        {
            _sorted = false;
            _multiselect = false;
            _forceVertScroll = false;
            _forceHorzScroll = false;
            _itemTooltips = false;
            _lastSelected = null;

            AddListboxProperties();
        }

        /// <summary>
        /// display required integrated scroll bars according to current state of the list box and update their values.
        /// </summary>
        protected void ConfigureScrollbars()
        {
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

	        var totalHeight	= GetTotalItemsHeight();
	        var widestItem	= GetWidestItemWidth();

	        //
	        // First show or hide the scroll bars as needed (or requested)
	        //
	        // show or hide vertical scroll bar as required (or as specified by option)
	        if ((totalHeight > GetListRenderArea().Height) || _forceVertScroll)
	        {
		        vertScrollbar.Show();

		        // show or hide horizontal scroll bar as required (or as specified by option)
		        if ((widestItem > GetListRenderArea().Width) || _forceHorzScroll)
		        {
			        horzScrollbar.Show();
		        }
		        else
		        {
			        horzScrollbar.Hide();
		        }

	        }
	        else
	        {
		        // show or hide horizontal scroll bar as required (or as specified by option)
		        if ((widestItem > GetListRenderArea().Width) || _forceHorzScroll)
		        {
			        horzScrollbar.Show();

			        // show or hide vertical scroll bar as required (or as specified by option)
			        if ((totalHeight > GetListRenderArea().Height) || _forceVertScroll)
			        {
				        vertScrollbar.Show();
			        }
			        else
			        {
				        vertScrollbar.Hide();
			        }

		        }
		        else
		        {
			        vertScrollbar.Hide();
			        horzScrollbar.Hide();
		        }

	        }

	        //
	        // Set up scroll bar values
	        //
            var renderArea = GetListRenderArea();

	        vertScrollbar.SetDocumentSize(totalHeight);
	        vertScrollbar.SetPageSize(renderArea.Height);
	        vertScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Height / 10.0f));
	        vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition());

	        horzScrollbar.SetDocumentSize(widestItem);
	        horzScrollbar.SetPageSize(renderArea.Width);
	        horzScrollbar.SetStepSize(Math.Max(1.0f, renderArea.Width / 10.0f));
	        horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition());
        }

        /// <summary>
        /// select all strings between positions \a start and \a end.  (inclusive) including \a end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected void SelectRange(int start, int end)
        {
            // only continue if list has some items
            if (_listItems.Count!=0)
            {
                // if start is out of range, start at begining.
                if (start > _listItems.Count)
                    start = 0;
                
                // if end is out of range end at the last item.
                if (end >= _listItems.Count)
                    end = _listItems.Count - 1;
                
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
                    _listItems[start].SetSelected(true);
                }

            }
        }

        /// <summary>
        /// Clear the selected state for all items (implementation)
        /// </summary>
        /// <returns>
        /// true if some selections were cleared, false nothing was changed.
        /// </returns>
        protected bool ClearAllSelectionsImpl()
        {
            // flag used so we can track if we did anything.
            var modified = false;

            foreach (var item in _listItems)
            {
                if (item.IsSelected())
                {
                    item.SetSelected(false);
                    modified = true;
                }
            }

            return modified;
        }

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause 'AutoDelete' items to be deleted.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the list contents were changed.
        /// - false if the list contents were not changed (list already empty).
        /// </returns>
        protected bool ResetListImpl()
        {
            // just return false if the list is already empty
	        if (GetItemCount() == 0)
	        {
		        return false;
	        }

            // we have items to be removed and possible deleted
		    
            // delete any items we are supposed to
		    for (var i = 0; i < GetItemCount(); ++i)
		    {
			    // if item is supposed to be deleted by us
			    if (_listItems[i].IsAutoDeleted())
			    {
				    // clean up this item.
				    // TODO: CEGUI_DELETE_AO d_listItems[i];
			    }

		    }

		    // clear out the list.
		    _listItems.Clear();

		    _lastSelected = null;

		    return true;
        }

        /// <summary>
        /// Internal handler that is triggered when the user interacts with the scrollbars.
        /// </summary>
        /// <param name="args"></param>
        protected bool HandleScrollChange(EventArgs args)
        {
            // simply trigger a redraw of the Listbox.
            Invalidate(false);
            return true;
        }

        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ListboxWindowRenderer) != null;
        }

        /// <summary>
        /// Causes the internal list to be (re)sorted.
        /// </summary>
        protected void ResortList()
        {
            // TODO: add comparer
            _listItems.Sort((a, b) => a.GetText().CompareTo(b.GetText()));
        }

        /// <summary>
        /// Handler called internally when the list contents are changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListContentsChanged(WindowEventArgs e)
        {
            ConfigureScrollbars();
            Invalidate(false);
            FireEvent(EventListContentsChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the currently selected item or items changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventSelectionChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the sort mode setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventSortModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the multi-select mode setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMultiselectModeChanged(WindowEventArgs e)
        {
            FireEvent(EventMultiselectModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the forced display of the vertical scroll bar setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventVertScrollbarModeChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler called internally when the forced display of the horizontal scroll bar setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventHorzScrollbarModeChanged, e, EventNamespace);
        }

        protected override void OnSized(ElementEventArgs e)
        {
            // base class handling
	        base.OnSized(e);

	        ConfigureScrollbars();

	        ++e.handled;
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorActivate(e);

	        if (e.Source == CursorInputSource.Left)
	        {
		        var modified = false;

                // TODO: ...
		        // clear old selections if no control key is pressed or if multi-select is off
                //if ((e.sysKeys & (uint)SystemKey.Control)!=(uint)SystemKey.Control || !_multiselect)
                //{
                //    modified = ClearAllSelectionsImpl();
                //}

		        var item = GetItemAtPoint(e.Position);

	            if (item != null)
	            {
	                modified = true;

                    // TODO: ...
	                // select range or item, depending upon keys and last selected item
                    //if (((e.sysKeys & (uint) SystemKey.Shift) == (uint) SystemKey.Shift && (_lastSelected != null)) &&
                    //    _multiselect)
                    //{
                    //    SelectRange(GetItemIndex(item), GetItemIndex(_lastSelected));
                    //}
                    //else
	                {
	                    item.SetSelected(item.IsSelected() ^ true);
	                }

	                // update last selected item
	                _lastSelected = item.IsSelected() ? item : null;
	            }

	            // fire event if needed
		        if (modified)
		        {
			        OnSelectionChanged(new WindowEventArgs(this));
		        }

		        ++e.handled;
	        }
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing.
	        base.OnScroll(e);

            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

	        if (vertScrollbar.IsEffectiveVisible() && (vertScrollbar.GetDocumentSize() > vertScrollbar.GetPageSize()))
	        {
		        vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() + vertScrollbar.GetStepSize() * -e.scroll);
	        }
	        else if (horzScrollbar.IsEffectiveVisible() && (horzScrollbar.GetDocumentSize() > horzScrollbar.GetPageSize()))
	        {
		        horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() + horzScrollbar.GetStepSize() * -e.scroll);
	        }

	        ++e.handled;
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            if (_itemTooltips)
            {
                var item = GetItemAtPoint(e.Position);
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

	    private void AddListboxProperties()
        {
            // TODO: Inconsistency
	        DefineProperty(
	            "Sort", "Property to get/set the sort setting of the list box.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetSortingEnabled(v), x => x.IsSortEnabled(), false);

            // TODO: Inconsistency
	        DefineProperty(
	            "MultiSelect",
	            "Property to get/set the multi-select setting of the list box.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetMultiselectEnabled(v), x => x.IsMultiselectEnabled(), false);

            // TODO: Inconsistency
	        DefineProperty(
	            "ForceVertScrollbar",
	            "Property to get/set the 'always show' setting for the vertical scroll bar of the list box.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), false);

            // TODO: Inconsistency
	        DefineProperty(
	            "ForceHorzScrollbar",
	            "Property to get/set the 'always show' setting for the horizontal scroll bar of the list box.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), false);

            // TODO: Inconsistency
	        DefineProperty(
	            "ItemTooltips",
	            "Property to access the show item tooltips setting of the list box.  Value is either \"True\" or \"False\".",
	            (x, v) => x.SetItemTooltipsEnabled(v), x => x.IsItemTooltipsEnabled(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<Listbox, T> setter, Func<Listbox, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<Listbox, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// true if list is sorted
        /// </summary>
        private bool _sorted;

        /// <summary>
        /// true if multi-select is enabled
        /// </summary>
        private bool _multiselect;

        /// <summary>
        /// true if vertical scrollbar should always be displayed
        /// </summary>
        private bool _forceVertScroll;

        /// <summary>
        /// true if horizontal scrollbar should always be displayed
        /// </summary>
        private bool _forceHorzScroll;

        /// <summary>
        /// true if each item should have an individual tooltip
        /// </summary>
        private bool _itemTooltips;

        /// <summary>
        /// holds pointer to the last selected item (used in range selections)
        /// </summary>
        private ListboxItem _lastSelected;

        /// <summary>
        /// list of items in the list box.
        /// </summary>
        private readonly List<ListboxItem> _listItems = new List<ListboxItem>();

        private static ListboxItem _lastItem;

        #endregion
    }
}