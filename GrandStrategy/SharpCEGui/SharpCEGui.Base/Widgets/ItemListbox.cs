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

using System.Linq;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// ItemListbox window class
    /// </summary>
    public class ItemListbox : ScrolledItemListBase
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ItemListbox";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ItemListbox";

        public const string EventSelectionChanged = "SelectionChanged";
        public const string EventMultiSelectModeChanged = "MultiSelectModeChanged";
        
        /// <summary>
        /// Event fired when the list selection changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemListbox whose current selection
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SelectionChanged
        {
            add { SubscribeEvent(EventSelectionChanged, value); }
            remove { UnsubscribeEvent(EventSelectionChanged, value); }
        }

        /// <summary>
        /// Event fired when the multiselect mode of the list box is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemListbox whose multiselect mode
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> MultiSelectModeChanged
        {
            add { SubscribeEvent(EventMultiSelectModeChanged, value); }
            remove { UnsubscribeEvent(EventMultiSelectModeChanged, value); }
        }

        #endregion

        /// <summary>
        /// Returns the number of selected items in this ItemListbox.
        /// </summary>
        /// <returns></returns>
        public int GetSelectedCount()
        {
            if (!_multiSelect)
                return _lastSelected != null ? 1 : 0;

            var count = 0;
            var max = ListItems.Count;
            for (var i = 0; i < max; ++i)
            {
                if (ListItems[i].IsSelected())
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns a pointer to the last selected item.
        /// </summary>
        /// <returns>
        /// A pointer to the last selected item, 0 is none.
        /// </returns>
        public ItemEntry GetLastSelectedItem()
        {
            return _lastSelected;
        }

        /// <summary>
        /// Returns a pointer to the first selected item
        /// </summary>
        /// <param name="startIndex">
        /// The index where the search should begin. If omitted the search will begin with the first item.
        /// </param>
        /// <returns>
        /// A pointer to the first selected item in the listbox.
        /// If no item is selected the return value is 0.
        /// If \a start_index is out of bounds the return value is 0.
        /// </returns>
        /// <remarks>
        /// If multiselect is disabled then this does the equivalent of calling GetLastSelectedItem.
        /// If multiselect is enabled it will search the array starting at \a start_index
        /// </remarks>
        public ItemEntry GetFirstSelectedItem(int startIndex = 0)
        {
            if (!_multiSelect)
                return _lastSelected;

            return FindSelectedItem(startIndex);
        }

        /// <summary>
        /// Returns a pointer to the next seleced item relative to a previous call to
        /// GetFirstSelectedItem or GetNextSelectedItem.
        /// </summary>
        /// <returns>
        /// A pointer to the next seleced item. If there are no further selected items
        /// the return value is 0.
        /// If multiselect is disabled the return value is 0.
        /// </returns>
        /// <remarks>
        /// This member function will take on from where the last call to
        /// GetFirstSelectedItem or GetNextSelectedItem returned. So be sure to start with a
        /// call to GetFirstSelectedItem.
        /// <para>
        /// This member function should be preferred over GetNextSelectedItemAfter as it will
        /// perform better, especially on large lists.
        /// </para>
        /// </remarks>
        public ItemEntry GetNextSelectedItem()
        {
            if (!_multiSelect)
                return null;
            
            return FindSelectedItem(_nextSelectionIndex);
        }

        /// <summary>
        /// Returns a pointer to the next selected item after the item 'start_item' given.
        /// </summary>
        /// <param name="startItem"></param>
        /// <returns></returns>
        /// <remarks>
        /// This member function will search the array from the beginning and will be slow
        /// for large lists, it will not advance the internal counter used by
        /// GetFirstSelectedItem and GetNextSelectedItem either.
        /// </remarks>
        public ItemEntry GetNextSelectedItemAfter(ItemEntry startItem)
        {
            if (startItem == null || !_multiSelect)
                return null;

            var max = ListItems.Count;
            var i = GetItemIndex(startItem);

            while (i < max)
            {
                var li = ListItems[i];
                if (li.IsSelected())
                    return li;
                ++i;
            }

            return null;
        }

        /// <summary>
        /// Returns 'true' if multiple selections are allowed. 'false' if not.
        /// </summary>
        /// <returns></returns>
        public bool IsMultiSelectEnabled()
        {
            return _multiSelect;
        }

        /// <summary>
        /// Returns 'true' if the item at the given index is selectable and currently selected.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsItemSelected(int index)
        {
            if (index >= ListItems.Count)
                throw new InvalidRequestException("The index given is out of range for this ItemListbox");

            var li = ListItems[index];
            return li.IsSelected();
        }

        /// <summary>
        /// Set whether or not multiple selections should be allowed.
        /// </summary>
        /// <param name="state"></param>
        public void SetMultiSelectEnabled(bool state)
        {
            if (state != _multiSelect)
            {
                _multiSelect = state;
                OnMultiSelectModeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        public void ClearAllSelections()
        {
            var max = ListItems.Count;
            for (var i=0; i<max; ++i)
                ListItems[i].SetSelectedImpl(false,false);
            
            _lastSelected = null;

            OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Select a range of items.
        /// </summary>
        /// <param name="a">Start item. (inclusive)</param>
        /// <param name="z">End item. (inclusive)</param>
        public void SelectRange(int a, int z)
        {
            // do nothing if the list is empty
            if (ListItems.Count==0)
                return;

            var max = ListItems.Count;
            if (a >= max)
                a = 0;

            if (z >= max)
                z = max-1;
            
            if (a>z)
            {
                var tmp = a;
                a = z;
                z = tmp;
            }

            for (var i=a; i<=z; ++i)
                ListItems[i].SetSelectedImpl(true,false);

            _lastSelected = ListItems[z];
    
            OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Select all items.
        /// Does nothing if multiselect is disabled.
        /// </summary>
        public void SelectAllItems()
        {
            if (!_multiSelect)
                return;

            var max = ListItems.Count;
            for (var i=0; i<max; ++i)
            {
                _lastSelected = ListItems[i];
                _lastSelected.SetSelectedImpl(true,false);
            }

            OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Constructor for the ItemListbox base class constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ItemListbox(string type, string name) : base(type, name)
        {
            _multiSelect = false;
            _lastSelected = null;
            _nextSelectionIndex = 0;

            AddItemListboxProperties();
        }

        /// <summary>
        /// Setup size and position for the item widgets attached to this ItemListbox
        /// </summary>
        protected override void LayoutItemWidgets()
        {
            float y = 0f;
            float widest = 0f;

            foreach (var entry in ListItems)
            {
                var pxs = entry.GetItemPixelSize();
                if (pxs.Width > widest)
                    widest = pxs.Width;

                entry.SetArea(new URect(new UVector2(UDim.Absolute(0), UDim.Absolute(y)),
                                        new UVector2(UDim.Relative(1), UDim.Absolute(y + pxs.Height))));

                y += pxs.Height;
            }

            // reconfigure scrollbars
            ConfigureScrollbars(new Sizef(widest, y));
        }

        /// <summary>
        /// Returns the Size in unclipped pixels of the content attached to this ItemListbox.
        /// </summary>
        /// <returns></returns>
        protected override Sizef GetContentSize()
        {
            var h = ListItems.Sum(i => i.GetItemPixelSize().Height);

            return new Sizef(GetItemRenderArea().Width, h);
        }

        /// <summary>
        /// Notify this ItemListbox that the given ListItem was just clicked.
        /// Internal function - not to be used from client code.
        /// </summary>
        /// <param name="li"></param>
        /// <param name="cumulativeSelection"></param>
        /// <param name="rangeSelection"></param>
        public override void NotifyItemActivated(ItemEntry li, bool cumulativeSelection, bool rangeSelection)
        {
            var selState = !(li.IsSelected() && _multiSelect);
            var skip = false;

            // multiselect enabled
            if (_multiSelect)
            {
                var last = _lastSelected;

                // no Control? clear others
                if (cumulativeSelection)
                {
                    ClearAllSelections();
                    if (!selState)
                        selState = true;
                }

                // select range if Shift if held, and we have a 'last selection'
                if (last != null && rangeSelection)
                {
                    SelectRange(GetItemIndex(last), GetItemIndex(li));
                    skip = true;
                }
            }
            else
            {
                ClearAllSelections();
            }

            if (!skip)
            {
                li.SetSelectedImpl(selState, false);
                if (selState)
                {
                    _lastSelected = li;
                }
                else if (_lastSelected == li)
                {
                    _lastSelected = null;
                }
            }

            OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Notify this ItemListbox that the given ListItem just changed selection state.
        /// Internal function - not to be used from client code.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        public override void NotifyItemSelectState(ItemEntry item, bool state)
        {
            // deselect
            if (!state)
            {
                // clear last selection if this one was it
                if (_lastSelected == item)
                {
                    _lastSelected = null;
                }
            }
            // if we dont support multiselect, we must clear all the other selections
            else if (!_multiSelect)
            {
                ClearAllSelections();
                item.SetSelectedImpl(true,false);
                _lastSelected = item;
            }

            OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Returns a pointer to the first selected item starting the search from \a start_index
        /// </summary>
        /// <param name="startIndex">The index where the search should begin (inclusive)</param>
        /// <returns>
        /// A pointer to the first selected item in the listbox found
        /// If no item is selected the return value is 0
        /// If \a start_index is out of bounds the return value is 0
        /// </returns>
        /// <remarks>
        /// This function advances the internal counter and is made for
        /// GetFirstSelectedItem and GetNextSelectedItem
        /// </remarks>
        protected ItemEntry FindSelectedItem(int startIndex)
        {
            var max = ListItems.Count;

            for (var i = startIndex; i < max; ++i)
            {
                var li = ListItems[i];
                if (li.IsSelected())
                {
                    _nextSelectionIndex = i + 1;
                    return li;
                }
            }

            return null;
        }

        protected override bool HandlePaneChildRemoved(EventArgs e)
        {
            base.HandlePaneChildRemoved(e);

            // get the window that's being removed
            var w = (Window) ((ElementEventArgs) e).element;
            // Clear last selected pointer if that item was just removed.
            if (w == _lastSelected)
                _lastSelected = null;

            return true;
        }

        protected virtual void OnSelectionChanged(WindowEventArgs e)
        {
            FireEvent(EventSelectionChanged, e, EventNamespace);
        }

        protected virtual void OnMultiSelectModeChanged(WindowEventArgs e)
        {
            FireEvent(EventMultiSelectModeChanged, e, EventNamespace);
        }
        
        private void AddItemListboxProperties()
        {
            const string propertyOrigin = "ItemListbox";

            AddProperty(new TplWindowProperty<ItemListbox, bool>(
                            "MultiSelect",
                            "Property to get/set the state of the multiselect enabled setting for the ItemListbox.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetMultiSelectEnabled(v), x => x.IsMultiSelectEnabled(), propertyOrigin));
        }

        #region Fields

        /// <summary>
        /// Controls whether multiple items can be selected simultaneously
        /// </summary>
        private bool _multiSelect;

        /// <summary>
        /// The index of the last item that was returned with the GetFirst/NextSelection members
        /// </summary>
        private int /*mutable*/ _nextSelectionIndex;

        /// <summary>
        /// The last item that was selected
        /// </summary>
        private ItemEntry _lastSelected;

        #endregion
    }
}