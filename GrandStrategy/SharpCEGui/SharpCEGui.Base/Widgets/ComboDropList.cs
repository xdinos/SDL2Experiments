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
using SharpCEGui.Base.Views;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for the combo box drop down list.  This is a specialisation of the Listbox class.
    /// </summary>
    public class ComboDropList : ListWidget
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ComboDropList";

        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/ComboDropList";

        #region Events

        public const string EventListSelectionAccepted = "ListSelectionAccepted";

        /// <summary>
        /// Event fired when the user confirms the selection by clicking the mouse.
        /// Handlers are passed aWindowEventArgs reference with
        /// WindowEventArgs::window set to the ComboDropList whose selection has been
        /// confirmed by the user.
        /// </summary>
        public event GuiEventHandler<EventArgs> ListSelectionAccepted
        {
            add { SubscribeEvent(EventListSelectionAccepted, value); }
            remove { UnsubscribeEvent(EventListSelectionAccepted, value); }
        }

        #endregion

        /// <summary>
        /// Initialise the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.
        /// Normally this is handled automatically by the WindowFactory for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            ThrowIfDisposed();

            base.InitialiseComponents();

	        // set-up scroll bars so they return capture to us.
	        GetVertScrollbar().SetRestoreOldCapture(true);
	        GetHorzScrollbar().SetRestoreOldCapture(true);

            // ban these properties from being written
            GetVertScrollbar().BanPropertyFromXML(RestoreOldCapturePropertyName);
            GetHorzScrollbar().BanPropertyFromXML(RestoreOldCapturePropertyName);
        }

        /// <summary>
        /// Set whether the drop-list is 'armed' for selection.
        /// </summary>
        /// <param name="setting">
        /// - true to arm the box; items will be highlighted and the next left button up event
        ///   will cause dismissal and possible item selection.
        /// - false to disarm the box; items will not be highlighted or selected until the box is armed.
        /// </param>
        /// <remarks>
        /// This setting is not exclusively under client control; the ComboDropList will auto-arm in
        /// response to certain left mouse button events.  This is also dependant upon the autoArm
        /// setting of the ComboDropList.
        /// </remarks>
	    public void SetArmed(bool setting)
	    {
            ThrowIfDisposed();

	        _armed = setting;
	    }

        /// <summary>
        /// Return the 'armed' state of the ComboDropList.
        /// </summary>
        /// <returns>
        /// - true if the box is armed; items will be highlighted and the next left button up event
        ///   will cause dismissal and possible item selection.
        /// - false if the box is not armed; items will not be highlighted or selected until the box is armed.
        /// </returns>
	    public bool IsArmed()
	    {
            ThrowIfDisposed();

	        return _armed;
	    }

        /// <summary>
        /// Set the mode of operation for the ComboDropList.
        /// </summary>
        /// <param name="setting">
        /// - true if the ComboDropList auto-arms when the mouse enters the box.
        /// - false if the user must click to arm the box.
        /// </param>
	    public void SetAutoArmEnabled(bool setting)
	    {
            ThrowIfDisposed();

	        _autoArm = setting;
	    }

        /// <summary>
        /// returns the mode of operation for the drop-list
        /// </summary>
        /// <returns>
        /// - true if the ComboDropList auto-arms when the mouse enters the box.
        /// - false if the user must click to arm the box.
        /// </returns>
	    public bool IsAutoArmEnabled()
	    {
            ThrowIfDisposed();

	        return _autoArm;
	    }

        /// <summary>
        /// resize the widget such that the content is shown without scrollbars.
        /// </summary>
        /// <param name="fitWidth"></param>
        /// <param name="fitHeight"></param>
        public void ResizeToContent(bool fitWidth, bool fitHeight)
        {
            ThrowIfDisposed();

            if (d_windowRenderer == null)
                throw new InvalidRequestException("Function requires a valid WindowRenderer object to be set.");

            ((ItemViewWindowRenderer)d_windowRenderer).ResizeViewToContent(fitWidth, fitHeight);
        }

        /// <summary>
        /// Constructor for ComboDropList base class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ComboDropList(string type, string name)
            : base(type, name)
        {
            _autoArm = false;
            _armed = false;
            _lastItemSelected = null;

            Hide();
            // pass captured inputs to children to enable scrollbars
            SetDistributesCapturedInputs(true);
            BanPropertyFromXML("DistributeCapturedInputs");
        }

        /// <summary>
        /// Handler for when list selection is confirmed.
        /// </summary>
        /// <param name="e"></param>
	    protected void OnListSelectionAccepted(WindowEventArgs e)
	    {
            _lastItemSelected = GetFirstSelectedItem();
            FireEvent(EventListSelectionAccepted, e, EventNamespace);
	    }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            base.OnCursorMove(e);

	        // if mouse is within our area (but not our children)
            if (IsHit(e.Position))
            {
                if (GetChildAtPosition(e.Position) == null)
                {
                    // handle auto-arm
                    if (_autoArm)
                        _armed = true;

                    if (_armed)
                    {
                        // check for an item under the mouse
                        StandardItem item = d_itemModel.GetItemForIndex(IndexAt(e.Position));

                        // if an item is under cursor, select it
                        if (item != null)
                        {
                            SetIndexSelectionState(item, true);
                        }
                        else
                        {
                            ClearSelections();
                        }
                    }
                }

                ++e.handled;
            }
            else
            {
                // not within the list area

                // if left mouse button is down, clear any selection
                if (e.state.IsHeld(CursorInputSource.Left))
                    ClearSelections();
            }
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                if (!IsHit(e.Position))
                {
                    ClearSelections();
                    ReleaseInput();
                }
                else
                {
                    _armed = true;
                }

                ++e.handled;
            }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                if (_armed && (GetChildAtPosition(e.Position) == null))
                {
                    // if something was selected, confirm that selection.
                    if (GetIndexSelectionStates().Any())
                    {
                        OnListSelectionAccepted(new WindowEventArgs(this));
                    }

                    ReleaseInput();
                }
                else
                {
                    // if we are not already armed, in response to a left button up event, we auto-arm.
                    _armed = true;
                }

                ++e.handled;
            }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            base.OnCaptureLost(e);
	        _armed = false;
	        Hide();
	        ++e.handled;

            // ensure 'sticky' selection remains.
            if (_lastItemSelected != null && IsItemSelected(_lastItemSelected))
            {
                ClearSelections();
                SetIndexSelectionState(_lastItemSelected, true);
            }
        }

        protected override void OnViewContentsChanged(WindowEventArgs e)
        {
            // basically see if our 'sticky' selection was removed
            if (_lastItemSelected != null && !IsItemInList(_lastItemSelected))
                _lastItemSelected = null;

            // base class processing
            base.OnViewContentsChanged(e);
        }
        protected override void OnSelectionChanged(ItemViewEventArgs e)
        {
            if (!IsActive())
                _lastItemSelected = GetFirstSelectedItem();

            base.OnSelectionChanged(e);
        }
        
        #region Fields

        /// <summary>
        /// true if the box auto-arms when the mouse enters it.
        /// </summary>
        private bool	_autoArm;
	    
        /// <summary>
        /// true when item selection has been armed.
        /// </summary>
        private bool	_armed;

        /// <summary>
        /// Item last accepted by user.
        /// </summary>
        private StandardItem _lastItemSelected;

        #endregion
    }
}