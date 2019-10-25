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
    /// Base class for menu items.
    /// </summary>
    public class MenuItem : ItemEntry
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "MenuItem";

        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/MenuItem";

        #region Events

        public const string EventClicked = "Clicked";

        /// <summary>
        /// Event fired when the menu item is clicked.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the MenuItem that was clicked.
        /// </summary>
        public event GuiEventHandler<EventArgs> Clicked
        {
            add { SubscribeEvent(EventClicked, value); }
            remove { throw new NotImplementedException(); }
        }

        #endregion

        /// <summary>
        /// return true if user is hovering over this widget (or it's pushed and user is not over it for highlight)
        /// </summary>
        /// <returns>
        /// true if the user is hovering or if the button is pushed and the mouse is not over the button.  Otherwise return false.
        /// </returns>
        public bool IsHovering()
        {
            return _hovering;
        }

        /// <summary>
        /// Return true if the button widget is in the pushed state.
        /// </summary>
        /// <returns>
        /// true if the button-type widget is pushed, false if the widget is not pushed.
        /// </returns>
        public bool IsPushed()
        {
            return _pushed;
        }

        /// <summary>
        /// Returns true if the popup menu attached to the menu item is open.
        /// </summary>
        /// <returns></returns>
        public bool IsOpened()
        {
            return _opened;
        }

        /// <summary>
        /// Returns true if the menu item popup is closing or not.
        /// </summary>
        /// <returns></returns>
        public bool IsPopupClosing()
        {
            return _popupClosing;
        }

        /// <summary>
        /// Returns true if the menu item popup is closed or opened automatically if hovering with the mouse.
        /// </summary>
        /// <returns></returns>
        public bool HasAutoPopup()
        {
            return _autoPopupTimeout > 0.0f;
        }

        /// <summary>
        /// Returns the time, which has to elapse before the popup window is opened/closed if the hovering state changes.
        /// </summary>
        /// <returns></returns>
        public float GetAutoPopupTimeout()
        {
            return _autoPopupTimeout;
        }

        /// <summary>
        /// Sets the time, which has to elapse before the popup window is opened/closed if the hovering state changes.
        /// </summary>
        /// <param name="time"></param>
        public void SetAutoPopupTimeout(float time)
        {
            _autoPopupTimeout = time;
        }

        /// <summary>
        /// Get the PopupMenu that is currently attached to this MenuItem.
        /// </summary>
        /// <returns>
        /// A pointer to the currently attached PopupMenu.  Null is there is no PopupMenu attached.
        /// </returns>
        public PopupMenu GetPopupMenu()
        {
            return _popup;
        }

        /// <summary>
        /// Returns the current offset for popup placement.
        /// </summary>
        /// <returns></returns>
        public UVector2 GetPopupOffset()
        {
            return _popupOffset;
        }

        /// <summary>
        /// sets the current offset for popup placement.
        /// </summary>
        /// <param name="popupOffset"></param>
        public void SetPopupOffset(UVector2 popupOffset)
        {
            _popupOffset = popupOffset;
        }

        /// <summary>
        /// Set the popup menu for this item.
        /// </summary>
        /// <param name="popup">
        /// popupmenu window to attach to this item
        /// </param>
        public void SetPopupMenu(PopupMenu popup)
        {
            SetPopupMenuImpl(popup);
        }

        /// <summary>
        /// Opens the PopupMenu.
        /// </summary>
        /// <param name="notify">
        /// true if the parent menu bar or menu popup (if any) is to handle the open.
        /// </param>
        public void OpenPopupMenu(bool notify = true)
        {
            // no popup? or already open...
            if (_popup == null || _opened)
                return;

            _popupOpening = false;
            _popupClosing = false;

            // should we notify ?
            // if so, and we are attached to a menu bar or popup menu, we let it handle the "activation"
            var p = OwnerList;

            if (notify && p!=null)
            {
                var menubar = p as Menubar;
                if (menubar != null)
                {
                    // align the popup to the bottom-left of the menuitem
                    var pos = new UVector2(UDim.Absolute(0), UDim.Absolute(d_pixelSize.Height));
                    _popup.SetPosition(pos + _popupOffset);

                    menubar.ChangePopupMenuItem(this);
                    return; // the rest is handled when the menu bar eventually calls us itself
                }

                // or maybe a popup menu?
                var popupMenu = p as PopupMenu;
                if (popupMenu != null)
                {
                    // align the popup to the top-right of the menuitem
                    var pos=new UVector2(UDim.Absolute(d_pixelSize.Width), UDim.Absolute(0));
                    _popup.SetPosition(pos + _popupOffset);

                    popupMenu.ChangePopupMenuItem(this);
                    return; // the rest is handled when the popup menu eventually calls us itself
                }
            }

            // by now we must handle it ourselves
            // match up with Menubar::changePopupMenu
            _popup.OpenPopupMenu(false);

            _opened = true;
            Invalidate(false);
        }

        /// <summary>
        /// Closes the PopupMenu.
        /// </summary>
        /// <param name="notify">
        /// true if the parent menubar (if any) is to handle the close.
        /// </param>
        public void ClosePopupMenu(bool notify = true)
        {
            // no popup? or not open...
            if (_popup == null || !_opened)
                return;

            _popupOpening = false;
            _popupClosing = false;

            // should we notify the parent menu base?
            // if we are attached to a menu base, we let it handle the "deactivation"
            var menu = OwnerList as MenuBase;
            if (notify && menu != null)
            {
                // only if the menu base does not allow multiple popups
                if (!menu.IsMultiplePopupsAllowed())
                {
                    menu.ChangePopupMenuItem(null);
                    return; // the rest is handled when the menu base eventually call us again itself
                }
            }
                // otherwise we do ourselves
            else
            {
                // match up with Menubar::changePopupMenu
                //d_popup->hide();
                _popup.ClosePopupMenu(false);
            }

            _opened = false;
            Invalidate(false);
        }

        /// <summary>
        /// Toggles the PopupMenu.
        /// </summary>
        /// <returns>
        /// true if the popup was opened. false if it was closed.
        /// </returns>
        public bool TogglePopupMenu()
        {
            if (_opened)
            {
                ClosePopupMenu();
                return false;
            }

            OpenPopupMenu();
            return true;
        }

        /// <summary>
        /// starts the closing timer for the popup, which will close it if the timer is enabled.
        /// </summary>
        public void StartPopupClosing()
        {
            _popupOpening = false;

            if (_opened)
            {
                _autoPopupTimeElapsed = 0.0f;
                _popupClosing = true;
                Invalidate(false);
            }
            else
            {
                _popupClosing = false;
            }
        }

        /// <summary>
        /// starts the opening timer for the popup, which will open it if the timer is enabled.
        /// </summary>
        public void StartPopupOpening()
        {
            _popupClosing = false;

            if (_opened)
            {
                _popupOpening = false;
            }
            else
            {
                _autoPopupTimeElapsed = 0.0f;
                _popupOpening = true;
            }
        }

        /// <summary>
        /// Constructor for MenuItem objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public MenuItem(string type, string name)
            : base(type, name)
        {
            _pushed = false;
            _hovering = false;
            _opened = false;
            _popupClosing = false;
            _popupOpening = false;
            _autoPopupTimeout = 0.0f;
            _autoPopupTimeElapsed = 0.0f;
            _popup = null;

            // menuitems dont want multi-click events
            SetWantsMultiClickEvents(false);
            
            // add the new properties
            AddMenuItemProperties();

            _popupOffset.d_x = UDim.Absolute(0);
            _popupOffset.d_y = UDim.Absolute(0);
        }

        /// <summary>
        /// handler invoked internally when the MenuItem is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClicked(WindowEventArgs e)
        {
            // close the popup if we did'nt spawn a child
            if (!_opened && !_popupWasClosed)
            {
                CloseAllMenuItemPopups();
            }

            _popupWasClosed = false;
            FireEvent(EventClicked, e, EventNamespace);
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // this is needed to discover whether mouse is in the widget area or not.
            // The same thing used to be done each frame in the rendering method,
            // but in this version the rendering method may not be called every frame
            // so we must discover the internal widget state here - which is actually
            // more efficient anyway.

            // base class processing
            base.OnCursorMove(e);

            UpdateInternalState(e.Position);
            ++e.handled;
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // default processing
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                _popupWasClosed = false;

                if (CaptureInput())
                {
                    _pushed = true;
                    UpdateInternalState(e.Position);
                    _popupWasClosed = !TogglePopupMenu();
                    Invalidate(false);
                }

                // event was handled by us.
                ++e.handled;
            }
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // default processing
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                ReleaseInput();

                // was the button released over this window?
                // (use mouse position, as e.position in args has been unprojected)
                if (!_popupWasClosed &&
                    GetGUIContext()
                        .GetRootWindow()
                        .GetTargetChildAtPosition(GetGUIContext().GetCursor().GetPosition()) == this)
                {
                    OnClicked(new WindowEventArgs(this));
                }

                // event was handled by us.
                ++e.handled;
            }
        }
        
        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // Default processing
            base.OnCaptureLost(e);

            _pushed = false;
            UpdateInternalState(GetUnprojectedPosition(GetGUIContext().GetCursor().GetPosition()));
            Invalidate(false);

            // event was handled by us.
            ++e.handled;
        }

        protected internal override void OnCursorLeaves(CursorInputEventArgs e)
        {
            // deafult processing
            base.OnCursorLeaves(e);

            _hovering = false;
            Invalidate(false);

            ++e.handled;
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            base.OnTextChanged(e);

            // if we are attached to a ItemListBase, we make it update as necessary
            var parent = GetParent();
            var ilb = parent as ItemListBase;
    
            if (ilb!=null)
                ilb.HandleUpdatedItemData();
            
            ++e.handled;
        }

        protected override void UpdateSelf(float elapsed)
        {
            base.UpdateSelf(elapsed);

            //handle delayed popup closing/opening when hovering with the mouse
            if (_autoPopupTimeout != 0.0f && (_popupOpening || _popupClosing))
            {
                // stop timer if the hovering state isn't set appropriately anymore
                if (_hovering)
                {
                    _popupClosing = false;
                }
                else
                {
                    _popupOpening = false;
                }

                //check if the timer elapsed and take action appropriately
                _autoPopupTimeElapsed += elapsed;

                if (_autoPopupTimeElapsed > _autoPopupTimeout)
                {
                    if (_popupOpening)
                    {
                        _popupOpening = false;
                        OpenPopupMenu();
                    }
                    else if (_popupClosing)
                    {
                        _popupClosing = false;
                        ClosePopupMenu();
                    }
                }
            }
        }

        /// <summary>
        /// Update the internal state of the widget with the mouse at the given position.
        /// </summary>
        /// <param name="mousePos">
        /// Point object describing, in screen pixel co-ordinates, the location of the mouse cursor.
        /// </param>
        protected void UpdateInternalState(Lunatics.Mathematics.Vector2 mousePos)
        {
            bool oldstate = _hovering;

            // assume not hovering
            _hovering = false;

            // if input is captured, but not by 'this', then we never hover highlight
            var captureWnd = GetCaptureWindow();

            if (captureWnd == null)
                _hovering = (GetGUIContext().GetWindowContainingCursor() == this && IsHit(mousePos));
            else
                _hovering = (captureWnd == this && IsHit(mousePos));

            // if state has changed, trigger a re-draw
            // and possible make the parent menu open another popup
            if (oldstate != _hovering)
            {
                // are we attached to a menu ?
                var menu = OwnerList as MenuBase;
                if (menu!=null)
                {
                    if (_hovering)
                    {
                        // does this menubar only allow one popup open? and is there a popup open?
                        var curpopup = menu.GetPopupMenuItem();

                        if (!menu.IsMultiplePopupsAllowed())
                        {
                            if (curpopup != this && curpopup != null)
                            {
                                if (!HasAutoPopup())
                                {
                                    // open this popup instead
                                    OpenPopupMenu();
                                }
                                else
                                {
                                    // start close timer on current popup
                                    menu.SetPopupMenuItemClosing();
                                    StartPopupOpening();
                                }
                            }
                            else
                            {
                                StartPopupOpening();
                            }
                        }
                    }
                }

                Invalidate(false);
            }
        }

        /// <summary>
        /// Recursive function that closes all popups down the hierarcy starting with this one.
        /// </summary>
        protected void CloseAllMenuItemPopups()
        {
            // are we attached to a PopupMenu?
            if (OwnerList == null)
                return;

            if (OwnerList is Menubar)
            {
                ClosePopupMenu();
                return;
            }

            var pop = OwnerList as PopupMenu;
            if (pop != null)
            {
                // is this parent popup attached to a menu item?
                var popParent = pop.GetParent();
                var mi = popParent as MenuItem;

                if (mi != null)
                {
                    // recurse
                    mi.CloseAllMenuItemPopups();
                }
                // otherwise we just hide the parent popup
                else
                {
                    pop.ClosePopupMenu(false);
                }
            }
        }

        /// <summary>
        /// Set the popup menu for this item.
        /// </summary>
        /// <param name="popup">
        /// popupmenu window to attach to this item
        /// </param>
        /// <param name="addAsChild"></param>
        protected void SetPopupMenuImpl(PopupMenu popup, bool addAsChild = true)
        {
            // is it the one we have already ?
            if (popup == _popup)
            {
                // then do nothing;
                return;
            }

            // keep the old one around
            var oldPopup = _popup;
            // update the internal state pointer
            _popup = popup;
            _opened = false;

            // is there already a popup ?
            if (oldPopup!=null)
            {
                RemoveChild(oldPopup);

                // should we destroy it as well?
                if (oldPopup.IsDestroyedByParent())
                {
                    // then do so
                    WindowManager.GetSingleton().DestroyWindow(oldPopup);
                }
            }

            // we are setting a new popup and not just clearing. and we are told to add the child
            if (popup != null && addAsChild)
            {
                AddChild(popup);
            }

            Invalidate(false);
        }
        
        protected sealed override void AddChildImpl(Element element)
        {
             var wnd = element as Window;

            if (wnd == null)
                throw new InvalidRequestException(
                    "MenuItem can only have Elements of type Window added as children " +
                    "(Window path: " + GetNamePath() + ").");
    
            base.AddChildImpl(wnd);

            var pop = wnd as PopupMenu;
            // if this is a PopupMenu we add it like one
            if (pop!=null)
            {
                SetPopupMenuImpl(pop, false);
            }
        }

        private void AddMenuItemProperties()
        {
            AddProperty(new TplWindowProperty<MenuItem, UVector2>(
                            "PopupOffset",
                            "Property to specify an offset for the popup menu position. Value is a UVector2 property value.",
                            (x, v) => x.SetPopupOffset(v), x => x.GetPopupOffset(), WidgetTypeName, UVector2.Zero));

            AddProperty(new TplWindowProperty<MenuItem, float>(
                            "AutoPopupTimeout",
                            "Property to specify the time, which has to elapse before the popup window is opened/closed if the hovering state changes. Value is a float property value.",
                            (x, v) => x.SetAutoPopupTimeout(v), x => x.GetAutoPopupTimeout(), WidgetTypeName));
        }

        #region Fields

        /// <summary>
        /// true when widget is pushed
        /// </summary>
        private bool _pushed;

        /// <summary>
        /// true when the button is in 'hover' state and requires the hover rendering.
        /// </summary>
        private bool _hovering;

        /// <summary>
        /// true when the menu item's popup menu is in its opened state.
        /// </summary>
        private bool _opened;

        /// <summary>
        /// true when the d_popupTimerTimeElapsed timer is running to close the popup (another menu item of our container is hovered)
        /// </summary>
        private bool _popupClosing;

        /// <summary>
        /// true when the d_popupTimerTimeElapsed timer is running to open the popup (the menu item is hovered)
        /// </summary>
        private bool _popupOpening;

        /// <summary>
        /// the time in seconds, to wait before opening / closing the popup if the mouse is over the item / over another item in our container
        /// </summary>
        private float _autoPopupTimeout;

        /// <summary>
        /// the current time, which is already elapsed if the timer is running (d_popupClosing or d_popupOpening is true)
        /// </summary>
        float _autoPopupTimeElapsed;

        /// <summary>
        /// PopupMenu that this item displays when activated.
        /// </summary>
        PopupMenu _popup;

        /// <summary>
        /// Used internally to determine if a popup was just closed on a Clicked event
        /// </summary>
        bool _popupWasClosed;

        /// <summary>
        /// current offset for popup placement.
        /// </summary>
        UVector2 _popupOffset;

        #endregion
    }
}