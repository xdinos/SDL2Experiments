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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Abstract base class for menus.
    /// </summary>
    public abstract class MenuBase : ItemListBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "MenuBase";

        /// <summary>
        /// Event fired when a MenuItem attached to this menu opened a PopupMenu.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the PopupMenu that was opened.
        /// </summary>
        public event EventHandler<WindowEventArgs> PopupOpened;

        /// <summary>
        /// Event fired when a MenuItem attached to this menu closed a PopupMenu.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the PopupMenu that was closed.
        /// </summary>
        public event EventHandler<WindowEventArgs> PopupClosed;

        /// <summary>
        /// Get the item spacing for this menu.
        /// </summary>
        /// <returns>
        /// A float value with the current item spacing for this menu
        /// </returns>
        public float GetItemSpacing()
        {
            return ItemSpacing;
        }

        /// <summary>
        /// Return whether this menu allows multiple popup menus to open at the same time.
        /// </summary>
        /// <returns>
        /// true if this menu allows multiple popup menus to be opened simultaneously. false if not
        /// </returns>
        public bool IsMultiplePopupsAllowed()
        {
            return _allowMultiplePopups;
        }

        /// <summary>
        /// Return whether this menu should close all its open child popups, when it gets hidden
        /// </summary>
        /// <returns>
        /// true if the menu should close all its open child popups, when it gets hidden
        /// </returns>
        public bool GetAutoCloseNestedPopups()
        {
            return _autoCloseNestedPopups;
        }

        /// <summary>
        /// Get currently opened MenuItem in this menu. Returns NULL if no menu item is open.
        /// </summary>
        /// <returns>
        /// Pointer to the MenuItem currently open.
        /// </returns>
        public MenuItem GetPopupMenuItem()
        {
            return _popupItem;
        }

        /// <summary>
        /// Set the item spacing for this menu.
        /// </summary>
        /// <param name="spacing"></param>
        public void SetItemSpacing(float spacing)
        {
            ItemSpacing = spacing;
            HandleUpdatedItemData();
        }

        /// <summary>
        /// Change the currently open MenuItem in this menu.
        /// </summary>
        /// <param name="item">
        /// Pointer to a MenuItem to open or NULL to close any opened.
        /// </param>
        public void ChangePopupMenuItem(MenuItem item)
        {
            if (!_allowMultiplePopups && _popupItem == item)
                return;

            if (!_allowMultiplePopups && _popupItem != null)
            {
                var we = new WindowEventArgs(_popupItem.GetPopupMenu());
                _popupItem.ClosePopupMenu(false);
                _popupItem = null;
                OnPopupClosed(we);
            }

            if (item != null)
            {
                _popupItem = item;
                _popupItem.OpenPopupMenu(false);
                var we = new WindowEventArgs(_popupItem.GetPopupMenu());
                OnPopupOpened(we);
            }
        }

        /// <summary>
        /// Set whether this menu allows multiple popup menus to be opened simultaneously.
        /// </summary>
        /// <param name="setting"></param>
        public void SetAllowMultiplePopups(bool setting)
        {
            if (_allowMultiplePopups != setting)
            {
                // TODO :
                // close all popups except perhaps the last one opened!
                _allowMultiplePopups = setting;
            }
        }

        /// <summary>
        /// Set whether the menu should close all its open child popups, when it gets hidden
        /// </summary>
        /// <param name="setting"></param>
        public void SetAutoCloseNestedPopups(bool setting)
        {
            _autoCloseNestedPopups = setting;
        }

        /// <summary>
        /// tells the current popup that it should start its closing timer.
        /// </summary>
        public void SetPopupMenuItemClosing()
        {
            if (_popupItem != null)
            {
                _popupItem.StartPopupClosing();
            }
        }

        protected MenuBase(string type, string name)
            : base(type, name)
        {
            ItemSpacing = 0.0f;
            _popupItem = null;
            _allowMultiplePopups = false;
            _autoCloseNestedPopups = false;

            // add properties for MenuBase class
            AddMenuBaseProperties();
        }

        /// <summary>
        /// handler invoked internally when the a MenuItem attached to this menu opens its popup.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPopupOpened(WindowEventArgs e)
        {
            FireEvent(PopupOpened, e);
        }

        /// <summary>
        /// handler invoked internally when the a MenuItem attached to this menu closes its popup.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPopupClosed(WindowEventArgs e)
        {
            FireEvent(PopupClosed, e);
        }

        protected override void OnChildRemoved(ElementEventArgs e)
        {
            // if the removed window was our tracked popup item, zero ptr to it.
            if (e.element == _popupItem)
                _popupItem = null;

            // base class version
            base.OnChildRemoved(e);
        }

        protected override void OnHidden(WindowEventArgs e)
        {
            if (!GetAutoCloseNestedPopups())
                return;

            ChangePopupMenuItem(null);

            if (_allowMultiplePopups)
            {
                foreach (var item in ListItems)
                {
                    if (item == null)
                        continue;

                    var menuItem = item as MenuItem;
                    if (menuItem == null)
                        continue;

                    if (menuItem.GetPopupMenu() == null)
                        continue;

                    var we = new WindowEventArgs(menuItem.GetPopupMenu());
                    menuItem.ClosePopupMenu(false);
                    OnPopupClosed(we);
                }
            }
        }

        private void AddMenuBaseProperties()
        {
            const string propertyOrigin = "CEGUI/MenuBase";

            AddProperty(new TplWindowProperty<MenuBase, float>(
                            "ItemSpacing",
                            "Property to get/set the item spacing of the menu.  Value is a float.",
                            (x, v) => x.SetItemSpacing(v), x => x.GetItemSpacing(), propertyOrigin, 10.0f));

            // TODO: Inconsistency and awful English
            AddProperty(new TplWindowProperty<MenuBase, bool>(
                            "AllowMultiplePopups",
                            "Property to get/set the state of the allow multiple popups setting for the menu.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetAllowMultiplePopups(v), x => x.IsMultiplePopupsAllowed(), propertyOrigin));

            AddProperty(new TplWindowProperty<MenuBase, bool>(
                            "AutoCloseNestedPopups",
                            "Property to set if the menu should close all its open child popups, when it gets hidden. Value is either \"True\" or \"False\".",
                            (x, v) => x.SetAutoCloseNestedPopups(v), x => x.GetAutoCloseNestedPopups(), propertyOrigin));
        }

        #region Fields

        /// <summary>
        /// The spacing in pixels between items.
        /// </summary>
        protected float ItemSpacing;

        /// <summary>
        /// The currently open MenuItem. NULL if no item is open. If multiple popups are allowed, this means nothing.
        /// </summary>
        private MenuItem _popupItem;

        /// <summary>
        /// true if multiple popup menus are allowed simultaneously.  false if not.
        /// </summary>
        private bool _allowMultiplePopups;

        /// <summary>
        /// true if the menu should close all its open child popups, when it gets hidden
        /// </summary>
        private bool _autoCloseNestedPopups;

        #endregion
    }
}