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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for ItemEntry window renderer objects.
    /// </summary>
    public abstract class ItemEntryWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ItemEntryWindowRenderer(string name)
            : base(name, "ItemEntry")
        {
        }

        /// <summary>
        /// Return the "optimal" size for the item
        /// </summary>
        /// <returns>
        /// Size describing the size in pixel that this ItemEntry's content requires
        /// for non-clipped rendering
        /// </returns>
        public abstract Sizef GetItemPixelSize();
    }

    // TODO: Fire events on selection / deselection.
    // TODO: (Maybe selectable mode changed as well?)

    /// <summary>
    /// Base class for item type widgets.
    /// </summary>
    public class ItemEntry : Window
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ItemEntry";

        #region Events

        public const string EventSelectionChanged = "SelectionChanged";

        /// <summary>
        /// Event fired when the item's selection state changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ItemEntry whose selection state has
        /// changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> SelectionChanged
        {
            add { SubscribeEvent(EventSelectionChanged, value); }
            remove { UnsubscribeEvent(EventSelectionChanged, value); }
        }

        #endregion

        /// <summary>
        /// Return the "optimal" size for the item
        /// </summary>
        /// <returns>
        /// Size describing the size in pixel that this ItemEntry's content requires
        /// for non-clipped rendering
        /// </returns>
        public Sizef GetItemPixelSize()
        {
            if (d_windowRenderer != null)
                return ((ItemEntryWindowRenderer) d_windowRenderer).GetItemPixelSize();

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// Returns a pointer to the owner ItemListBase. null if there is none.
        /// </summary>
        /// <returns></returns>
        public ItemListBase GetOwnerList()
        {
            return OwnerList;
        }

        /// <summary>
        /// Returns whether this item is selected or not.
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return _selected;
        }

        /// <summary>
        /// Returns whether this item is selectable or not.
        /// </summary>
        /// <returns></returns>
        public bool IsSelectable()
        {
            return _selectable;
        }

        /// <summary>
        /// Sets the selection state of this item (on/off).
        /// If this item is not selectable this function does nothing.
        /// </summary>
        /// <param name="setting">
        /// 'true' to select the item.
        /// 'false' to deselect the item.
        /// </param>
        public void SetSelected(bool setting)
        {
            SetSelectedImpl(setting, true);
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        public void Select()
        {
            SetSelectedImpl(true, true);
        }

        /// <summary>
        /// Deselects the item.
        /// </summary>
        public void Deselect()
        {
            SetSelectedImpl(false, true);
        }

        /// <summary>
        /// Set the selection state for this ListItem.
        /// Internal version. Should NOT be used by client code.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="notify"></param>
        public void SetSelectedImpl(bool setting, bool notify)
        {
            if (_selectable && setting != _selected)
            {
                _selected = setting;

                // notify the ItemListbox if there is one that we just got selected
                // to ensure selection scheme is not broken when setting selection from code
                if (OwnerList != null && notify)
                {
                    OwnerList.NotifyItemSelectState(this, setting);
                }

                OnSelectionChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Sets whether this item will be selectable.
        /// </summary>
        /// <param name="setting">
        /// 'true' to allow this item to be selected.
        /// 'false' to disallow this item from ever being selected.
        /// </param>
        /// <remarks>
        /// If the item is currently selectable and selected, calling this
        /// function with \a setting as 'false' will first deselect the item
        /// and then disable selectability.
        /// </remarks>
        public void SetSelectable(bool setting)
        {
            if (_selectable != setting)
            {
                SetSelected(false);
                _selectable = setting;
            }
        }

        /// <summary>
        /// Constructor for ItemEntry objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ItemEntry(string type, string name)
            : base(type, name)
        {
            OwnerList = null;
            _selected = false;
            _selectable = false;

            // add the new properties
            AddItemEntryProperties();
        }

        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ItemEntryWindowRenderer) != null;
        }

        /// <summary>
        /// Handles selection state changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(EventSelectionChanged, e, EventNamespace);
        }

        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            var rangeSelection = e.d_semanticValue == SemanticValue.SV_SelectRange;
            var cumulativeSelection = e.d_semanticValue == SemanticValue.SV_SelectCumulative;

            if (_selectable &&
                (e.d_semanticValue == SemanticValue.SV_CursorActivate || rangeSelection || cumulativeSelection) &&
                e.d_payload.source == CursorInputSource.Left)
            {
                if (OwnerList != null)
                    OwnerList.NotifyItemActivated(this, cumulativeSelection, rangeSelection);
                else
                    SetSelected(!IsSelected());
                ++e.handled;
            }
        }

        private void AddItemEntryProperties()
        {
            AddProperty(new TplWindowProperty<ItemEntry, bool>(
                            "Selectable",
                            "Property to get/set the state of the selectable setting for the ItemEntry.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetSelectable(v), x => x.IsSelectable(), WidgetTypeName));

            AddProperty(new TplWindowProperty<ItemEntry, bool>(
                            "Selected",
                            "Property to get/set the state of the selected setting for the ItemEntry.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetSelected(v), x => x.IsSelected(), WidgetTypeName));
        }

        #region Fields

        /// <summary>
        /// pointer to the owner ItemListBase. 0 if there is none.
        /// </summary>
        protected internal ItemListBase OwnerList;

        /// <summary>
        /// 'true' when the item is in the selected state, 'false' if not.
        /// </summary>
        private bool _selected;

        /// <summary>
        /// 'true' when the item is selectable.
        /// </summary>
        private bool _selectable;

        #endregion
    }
}