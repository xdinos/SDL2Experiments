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
    /// Base class for ItemListBase window renderer.
    /// </summary>
    public abstract class ItemListBaseWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ItemListBaseWindowRenderer(string name)
            : base(name, ItemListBase.EventNamespace)
        {

        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering list items.
        /// </summary>
        /// <returns>
        /// Rect object describing the window relative area of the that is to be used for rendering
        /// the items.
        /// </returns>
        public abstract Rectf GetItemRenderArea();
    }

    /// <summary>
    /// Base class for item list widgets.
    /// </summary>
    public abstract class ItemListBase : Window
    {
        /// <summary>
        /// Sort modes for ItemListBase
        /// </summary>
        public enum SortMode
        {
            /// <summary>
            /// 
            /// </summary>
            Ascending,

            /// <summary>
            /// 
            /// </summary>
            Descending,

            /// <summary>
            /// 
            /// </summary>
            UserSort
        }

        /// <summary>
        /// Sorting callback type
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public delegate bool SortCallback(ItemEntry a, ItemEntry b);

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ItemListBase";

        public const string EventListContentsChanged = "ListContentsChanged";
        public const string EventSortEnabledChanged = "SortEnabledChanged";
        public const string EventSortModeChanged = "SortModeChanged";

        /// <summary>
        /// Event fired when the contents of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemListBase whose contents
        /// has changed.
        /// </summary>
	    public event GuiEventHandler<EventArgs> ListContentsChanged
        {
            add { SubscribeEvent(EventListContentsChanged, value); }
            remove { UnsubscribeEvent(EventListContentsChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the sort enabled state of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemListBase whose sort enabled mode
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SortEnabledChanged
        {
            add { SubscribeEvent(EventSortEnabledChanged, value); }
            remove { UnsubscribeEvent(EventSortEnabledChanged, value); }
        }
        
        /// <summary>
        /// Event fired when the sort mode of the list is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemListBase whose sorting mode
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SortModeChanged
        {
            add { SubscribeEvent(EventSortModeChanged, value); }
            remove { UnsubscribeEvent(EventSortModeChanged, value); }
        }

        #endregion

        /// <summary>
        /// Return number of items attached to the list
        /// </summary>
        /// <returns>
        /// the number of items currently attached to this list.
        /// </returns>
        public int GetItemCount()
        {
            return ListItems.Count;
        }

        /// <summary>
        /// Return the item at index position \a index.
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be returned.
        /// </param>
        /// <returns>
        /// Pointer to the ItemEntry at index position \a index in the list.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
	    public ItemEntry GetItemFromIndex(int index)
	    {
            if (index < ListItems.Count)
                return ListItems[index];

            throw new InvalidRequestException("the specified index is out of range for this ItemListBase.");
	    }

        /// <summary>
        /// Return the index of ItemEntry \a item
        /// </summary>
        /// <param name="item">
        /// Pointer to a ItemEntry whos zero based index is to be returned.
        /// </param>
        /// <returns>
        /// Zero based index indicating the position of ItemEntry \a item in the list.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list.
        /// </exception>
        public int GetItemIndex(ItemEntry item)
        {
            if (ListItems.Contains(item))
            {
                return ListItems.IndexOf(item);
	        }

            throw new InvalidRequestException("the specified ItemEntry is not attached to this ItemListBase.");
        }

        /// <summary>
        /// Search the list for an item with the specified text
        /// </summary>
        /// <param name="text">
        /// String object containing the text to be searched for.
        /// </param>
        /// <param name="startItem">
        /// ItemEntry where the search is to begin, the search will not include \a item.  If \a item is
        /// NULL, the search will begin from the first item in the list.
        /// </param>
        /// <returns>
        /// Pointer to the first ItemEntry in the list after \a item that has text matching \a text.  If
        /// no item matches the criteria NULL is returned.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a item is not attached to this list box.
        /// </exception>
	    public ItemEntry FindItemWithText(string text, ItemEntry startItem)
	    {
            // if start_item is NULL begin search at begining, else start at item after start_item
            var index = (startItem == null) ? 0 : (GetItemIndex(startItem) + 1);

            while (index < ListItems.Count)
            {
                // return pointer to this item if it's text matches
                if (ListItems[index].GetText() == text)
                {
                    return ListItems[index];
                }

                // no matching text, advance to next item
                index++;
            }

            // no items matched.
            return null;
	    }

        /// <summary>
        /// Return whether the specified ItemEntry is in the List
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// true if ItemEntry \a item is in the list, false if ItemEntry \a item is not in the list.
        /// </returns>
	    public bool IsItemInList(ItemEntry item)
	    {
            return (item.OwnerList == this);
	    }

        /// <summary>
        /// Return whether this window is automatically resized to fit its content.
        /// </summary>
        /// <returns>
        /// true if automatic resizing is enabled, false if it is disabled.
        /// </returns>
	    public bool IsAutoResizeEnabled()
	    {
	        return AutoResize;
	    }

        /// <summary>
        /// Returns 'true' if the list is sorted
        /// </summary>
        /// <returns></returns>
        public bool IsSortEnabled()
        {
            return _sortEnabled;
        }

        /// <summary>
        /// Get sort mode.
        /// </summary>
        /// <returns></returns>
        public SortMode GetSortMode()
        {
            return _sortMode;
        }

        /// <summary>
        /// Get user sorting callback.
        /// </summary>
        /// <returns></returns>
        public SortCallback GetSortCallback()
        {
            return _sortCallback;
        }

        protected override void InitialiseComponents()
        {
            // this pane may be ourselves, and in fact is by default...
            Pane.ChildRemoved += HandlePaneChildRemoved;
        }

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause items, which does not have the 'DestroyedByParent' property set to 'false', to be deleted.
        /// </para>
        /// </summary>
	    public void ResetList()
	    {
            if (ResetListImpl())
            {
                HandleUpdatedItemData();
            }
	    }

        /// <summary>
        /// Add the given ItemEntry to the list.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ItemEntry to be added to the list.  Note that it is the passed object that is added to the
        /// list, a copy is not made.  If this parameter is NULL, nothing happens.
        /// </param>
	    public void AddItem(ItemEntry item)
	    {
	        // make sure the item is valid and that we dont already have it in our list
	        if (item!=null && item.OwnerList != this)
	        {
	            // if sorting is enabled, re-sort the list
                if (_sortEnabled)
                {
                    ListItems.Add(item);
                    // TODO: d_listItems.insert(std::upper_bound(d_listItems.begin(), d_listItems.end(), item, getRealSortCallback()), item);
                }
                // just stick it on the end.
                else
                {
                    ListItems.Add(item);
                }

                // make sure it gets added properly
		        item.OwnerList = this;
		        AddChild(item);
		        HandleUpdatedItemData();
	        }
	    }

        /// <summary>
        /// Insert an item into the list before a specified item already in the list.
        /// <para>
        /// Note that if the list is sorted, the item may not end up in the
        /// requested position.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// Pointer to the ItemEntry to be inserted.  Note that it is the passed
        /// object that is added to the list, a copy is not made.  If this parameter
        /// is NULL, nothing happens.
        /// </param>
        /// <param name="position">
        /// Pointer to a ItemEntry that \a item is to be inserted before.  If this
        /// parameter is NULL, the item is inserted at the start of the list.
        /// </param>
	    public void InsertItem(ItemEntry item, ItemEntry position)
	    {
	        if (_sortEnabled)
            {
                AddItem(item);
            }
	        else if (item!=null && item.OwnerList != this)
	        {
	            var insPos = position != null ? ListItems.IndexOf(position) : 0;
			    if (insPos == -1)
			    {
				    throw new InvalidRequestException("the specified ItemEntry for parameter 'position' is not attached to this ItemListBase.");
			    }

		        ListItems.Insert(insPos, item);
		        item.OwnerList = this;
		        AddChild(item);

		        HandleUpdatedItemData();
	        }
	    }

        /// <summary>
        /// Removes the given item from the list.  
        /// If the item is has the 'DestroyedByParent' property set to 'true', 
        /// the item will be deleted.
        /// </summary>
        /// <param name="item">
        /// Pointer to the ItemEntry that is to be removed.  
        /// If \a item is not attached to this list then nothing will happen.
        /// </param>
	    public void RemoveItem(ItemEntry item)
	    {
	        if (item!=null && item.OwnerList == this)
	        {
	            Pane.RemoveChild(item);
	            if (item.IsDestroyedByParent())
	                WindowManager.GetSingleton().DestroyWindow(item);
	        }
	    }

        /// <summary>
        /// Causes the list to update it's internal state after changes have been made to one or more
        /// attached ItemEntry objects.
        /// <para>
        /// It should not be necessary to call this from client code, 
        /// as the ItemEntries themselves call it if their parent is an ItemListBase.
        /// </para>
        /// </summary>
        /// <param name="resort">
        /// 'true' to redo the list sorting as well.
        /// 'false' to only do layout and perhaps auto resize.
        /// (defaults to 'false')
        /// </param>
        public void HandleUpdatedItemData(bool resort = false)
        {
            if (!d_destructionStarted)
            {
                _resort |= resort;
                OnListContentsChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set whether or not this ItemListBase widget should automatically resize to fit its content.
        /// </summary>
        /// <param name="setting">
        /// Boolean value that if true enables automatic resizing, if false disables automatic resizing.
        /// </param>
	    public void SetAutoResizeEnabled(bool setting)
	    {
            var old = AutoResize;
            AutoResize = setting;

            // if not already enabled, trigger a resize - only if not currently initialising
            if (AutoResize && !old && !d_initialising)
            {
                SizeToContent();
            }
	    }

        /// <summary>
        /// Resize the ItemListBase to exactly fit the content that is attached to it.
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering items.
        /// </summary>
	    public virtual void SizeToContent()
	    {
	        SizeToContentImpl();
	    }

        /// <summary>
        /// Triggers a ListContentsChanged event.
        /// These are not fired during initialisation for optimization purposes.
        /// </summary>
        public override void EndInitialisation()
        {
            base.EndInitialisation();
            HandleUpdatedItemData(true);
        }


        public override void PerformChildWindowLayout(bool nonClientSizedHint = false,
                                                      bool clientSizedHint = false)
        {
            base.PerformChildWindowLayout(nonClientSizedHint, clientSizedHint);

	        // if we are not currently initialising
	        if (!d_initialising)
	        {
	            // Redo the item layout.
	            // We don't just call handleUpdateItemData, as that could trigger a resize,
	            // which is not what is being requested.
	            // It would also cause infinite recursion... so lets just avoid that :)
	            LayoutItemWidgets();
	        }
        }

        /// <summary>
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering list items.
        /// </summary>
        /// <returns>
        /// Rect object describing the window relative area of the that is to be used for rendering
        /// the items.
        /// </returns>
        public Rectf GetItemRenderArea()
        {
            if (d_windowRenderer != null)
                return ((ItemListBaseWindowRenderer) d_windowRenderer).GetItemRenderArea();
            
            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// Returns a pointer to the window that all items are directed too.
        /// </summary>
        /// <returns>
        /// A pointer to the content pane window, or 'this' if children are added
        /// directly to this window.
        /// </returns>
        public Window GetContentPane()
        {
            return Pane;
        }

        /// <summary>
        ///  Notify this ItemListBase that the given item was just activated.
        /// Internal function - NOT to be used from client code.
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <param name="cumulativeSelection">
        /// True if this entry should cumulate to the previous selection
        /// </param>
        /// <param name="rangeSelection">
        /// True if this entry should do a range selection
        /// </param>
        public virtual void NotifyItemActivated(ItemEntry item, bool cumulativeSelection, bool rangeSelection)
        {
            
        }

        /// <summary>
        /// Notify this ItemListBase that the given item just changed selection state.
        /// Internal function - NOT to be used from client code.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        public virtual void NotifyItemSelectState(ItemEntry item, bool state)
        {
            
        }

        /// <summary>
        /// Set whether the list should be sorted (by text).
        /// </summary>
        /// <param name="setting"></param>
        public void SetSortEnabled(bool setting)
        {
            if (_sortEnabled != setting)
            {
                _sortEnabled = setting;

                if (_sortEnabled && !d_initialising)
                    SortList();
                
                OnSortEnabledChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set mode to be used when sorting the list.
        /// </summary>
        /// <param name="mode">
        /// SortMode enum.
        /// </param>
        public void SetSortMode(SortMode mode)
        {
            if (_sortMode != mode)
            {
                _sortMode = mode;
                if (_sortEnabled && !d_initialising)
                    SortList();

                OnSortModeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set a user callback as sorting function
        /// </summary>
        /// <param name="cb">SortCallback</param>
        public void SetSortCallback(SortCallback cb)
        {
            if (_sortCallback != cb)
            {
                _sortCallback = cb;
                if (_sortEnabled && !d_initialising)
                    SortList();
                HandleUpdatedItemData(true);
            }
        }

        /// <summary>
        /// Sort the list.
        /// </summary>
        /// <param name="relayout">
        /// True if the item layout should be redone after the sorting.
        /// False to only sort the internal list. Nothing more.
        /// 
        /// This parameter defaults to true and should generally not be
        /// used in client code.
        /// </param>
        public void SortList(bool relayout = true)
        {
            ListItems.Sort();
            // TODO: d_listItems.Sort(GetRealSortCallback());
            if (relayout)
            {
                LayoutItemWidgets();
            }
        }

        /// <summary>
        /// Constructor for ItemListBase base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        protected ItemListBase(string type, string name)
            : base(type, name)
        {
            AutoResize = false;
            _sortEnabled = false;
            _sortMode = SortMode.Ascending;
            _sortCallback = null;
            _resort = false;

            // by default we dont have a content pane, but to make sure things still work
            // we "emulate" it by setting it to this
            Pane = this;

            // add properties for ItemListBase class
            AddItemListBaseProperties();
        }

        /// <summary>
        /// Resize the ItemListBase to exactly fit the content that is attached to it.
        /// Return a Rect object describing, in un-clipped pixels, the window relative area
        /// that is to be used for rendering items.
        /// </summary>
	    protected virtual void SizeToContentImpl()
        {
            var renderArea = GetItemRenderArea();
            var wndArea = CoordConverter.AsAbsolute(GetArea(), GetParentPixelSize());

            // get size of content
            var sz = GetContentSize();

            // calculate the full size with the frame accounted for and resize the window to this
            sz.Width  += wndArea.Width - renderArea.Width;
            sz.Height += wndArea.Height - renderArea.Height;
            SetSize(new USize(UDim.Absolute(sz.Width), UDim.Absolute(sz.Height)));
	    }

        /// <summary>
        /// Returns the Size in unclipped pixels of the content attached to this ItemListBase that is attached to it.
        /// </summary>
        /// <returns>
        /// Size object describing in unclipped pixels the size of the content ItemEntries attached to this menu.
        /// </returns>
        protected abstract Sizef GetContentSize();

        /// <summary>
        /// Setup size and position for the item widgets attached to this ItemListBase
        /// </summary>
        protected abstract void LayoutItemWidgets();

        /// <summary>
        /// Remove all items from the list.
        /// <para>
        /// Note that this will cause items with the 'DestroyedByParent' property set to 'true', to be deleted.
        /// </para>
        /// </summary>
        /// <returns>
        /// - true if the list contents were changed.
        /// - false if the list contents were not changed (list already empty).
        /// </returns>
	    protected bool ResetListImpl()
	    {
	        // just return false if the list is already empty
	        if (ListItems.Count==0)
	            return false;
	        
            // we have items to be removed and possible deleted
	        // delete any items we are supposed to
		    while (ListItems.Count!=0)
		    {
		        var item = ListItems[0];
			    Pane.RemoveChild(item);
			    if (item.IsDestroyedByParent())
			    {
			        WindowManager.GetSingleton().DestroyWindow(item);
			    }
		    }

		    // list is cleared by the removeChild calls
		    return true;
	    }

        // validate window renderer
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ItemListBaseWindowRenderer) != null;
        }

        /// <summary>
        /// Returns the SortCallback that's really going to be used for the sorting operation.
        /// </summary>
        /// <returns></returns>
        protected SortCallback GetRealSortCallback()
        {
            throw new NotImplementedException();
            //switch (d_sortMode)
            //{
            //    case SortMode.Ascending:
            //        return &ItemEntry_less;

            //    case SortMode.Descending:
            //        return &ItemEntry_greater;

            //    case SortMode.UserSort:
            //        return (d_sortCallback != 0) ? d_sortCallback : &ItemEntry_less;

            //    // we default to ascending sorting
            //    default:
            //        return &ItemEntry_less;
            //}
        }

        /// <summary>
        /// Handler called internally when the list contents are changed
        /// </summary>
        /// <param name="e"></param>
	    protected virtual void OnListContentsChanged(WindowEventArgs e)
	    {
            // if we are not currently initialising we might have things todo
            if (!d_initialising)
            {
                Invalidate(false);

                // if auto resize is enabled - do it
                if (AutoResize)
                    SizeToContent();

                // resort list if requested and enabled
                if (_resort && _sortEnabled)
                    SortList(false);
                _resort = false;

                // redo the item layout and fire our event
                LayoutItemWidgets();
                FireEvent(EventListContentsChanged, e);
            }
	    }

        /// <summary>
        /// Handler called internally when sorting gets enabled.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortEnabledChanged(WindowEventArgs e)
        {
            FireEvent(EventSortEnabledChanged, e);
        }

        /// <summary>
        /// Handler called internally when the sorting mode is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortModeChanged(WindowEventArgs e)
	    {
            FireEvent(EventSortModeChanged, e);
	    }

        protected internal override void OnParentSized(ElementEventArgs e)
        {
            base.OnParentSized(e);

            if (AutoResize)
                SizeToContent();
        }

	    /// <summary>
        /// Handler to manage items being removed from the content pane. 
        /// If there is one!
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// If you override this, you should call this base class version to
        /// ensure correct behaviour is maintained.
        /// </remarks>
        protected virtual bool HandlePaneChildRemoved(EventArgs e)
        {
            // make sure it is removed from the itemlist if we have an ItemEntry
	        var item = ((ElementEventArgs)e).element as ItemEntry;
            if (item!=null)
            {
                // if item is in the list
                if (ListItems.Contains(item))
                {
                    // make sure the item is no longer related to us
                    item.OwnerList = null;

                    // remove item
                    ListItems.Remove(item);
                    
                    // trigger list update
                    HandleUpdatedItemData();
                }
            }

            return false;
        }

        protected sealed override void AddChildImpl(Element element)
        {
            var item = element as ItemEntry;
    
            // if this is an ItemEntry we add it like one, but only if it is not already in the list!
            if (item!=null)
            {
                // add to the pane if we have one
                if (Pane != this)
                {
                    Pane.AddChild(item);
                }
                // add item directly to us
                else
                {
                    base.AddChildImpl(item);
                }

	            if (item.OwnerList != this)
	            {
	                // perform normal addItem
	                // if sorting is enabled, re-sort the list
                    if (_sortEnabled)
                    {
                        ListItems.Add(item);
                        // TODO: d_listItems.insert(std::upper_bound(d_listItems.begin(), d_listItems.end(), item, getRealSortCallback()), item);
                    }
                    // just stick it on the end.
                    else
                    {
                        ListItems.Add(item);
                    }
	                item.OwnerList = this;
		            HandleUpdatedItemData();
	            }
	        }
	        // otherwise it's base class processing
            else
            {
                base.AddChildImpl(element);
            }
        }

	    private void AddItemListBaseProperties()
	    {
	        AddProperty(new TplWindowProperty<ItemListBase, bool>(
	                        "AutoResizeEnabled",
	                        "Property to get/set the state of the auto resizing enabled setting for the ItemListBase.  Value is either \"True\" or \"False\".",
	                        (x, v) => x.SetAutoResizeEnabled(v), x => x.IsAutoResizeEnabled(), "ItemListBase"));

	        AddProperty(new TplWindowProperty<ItemListBase, bool>(
	                        "SortEnabled",
	                        "Property to get/set the state of the sorting enabled setting for the ItemListBase.  Value is either \"True\" or \"False\".",
	                        (x, v) => x.SetSortEnabled(v), x => x.IsSortEnabled(), "ItemListBase"));

	        AddProperty(new TplWindowProperty<ItemListBase, SortMode>(
	                        "SortMode",
	                        "Property to get/set the sorting mode for the ItemListBase.  Value is either \"Ascending\", \"Descending\" or \"UserSort\".",
	                        (x, v) => x.SetSortMode(v), x => x.GetSortMode(), "ItemListBase"));
	    }

        

        #region Fields

        /// <summary>
        /// True if this ItemListBase widget should automatically resize to fit its content. False if not.
        /// </summary>
        protected bool AutoResize;

        /// <summary>
        /// Pointer to the content pane (for items), 0 if we're not using one
        /// </summary>
        protected Window Pane;

        /// <summary>
        /// list of items in the list.
        /// </summary>
        protected readonly List<ItemEntry> ListItems = new List<ItemEntry>();
        
        /// <summary>
        /// True if this ItemListBase is sorted. False if not.
        /// </summary>
        private bool _sortEnabled;

        /// <summary>
        /// True if the list needs to be resorted.
        /// </summary>
        private bool _resort;

        /// <summary>
        /// The current sorting mode applied if sorting is enabled.
        /// </summary>
        private SortMode _sortMode;

        /// <summary>
        /// The user sort callback or null if none.
        /// </summary>
        private SortCallback _sortCallback;

        #endregion
    }
}