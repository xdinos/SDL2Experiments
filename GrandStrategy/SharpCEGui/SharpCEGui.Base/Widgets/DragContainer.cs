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
    /// Generic drag & drop enabled window class
    /// </summary>
    public class DragContainer : Window
    {
        /// <summary>
        /// Type name for DragContainer.
        /// </summary>
        public const string WidgetTypeName = "DragContainer";

        #region Events

        public new const string EventNamespace = "DragContainer";
        public const string EventDragStarted="DragStarted";
        public const string EventDragEnded="DragEnded";
        public const string EventDragPositionChanged="DragPositionChanged";
        public const string EventDragEnabledChanged="DragEnabledChanged";
        public const string EventDragAlphaChanged="DragAlphaChanged";
        public const string EventDragMouseCursorChanged="DragMouseCursorChanged";
        public const string EventDragThresholdChanged="DragThresholdChanged";
        public const string EventDragDropTargetChanged="DragDropTargetChanged";

        /// <summary>
        /// Event fired when the user begins dragging the DragContainer.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer that the user
        /// has started to drag.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragStarted
        {
            add { SubscribeEvent(EventDragStarted, value); }
            remove { UnsubscribeEvent(EventDragStarted, value); }
        }

        /// <summary>
        /// Event fired when the user releases the DragContainer.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer that the user has
        /// released.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragEnded
        {
            add { SubscribeEvent(EventDragEnded, value); }
            remove { UnsubscribeEvent(EventDragEnded, value); }
        }

        /// <summary>
        /// Event fired when the drag position has changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer whose position has
        /// changed due to the user dragging it.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragPositionChanged
        {
            add { SubscribeEvent(EventDragPositionChanged, value); }
            remove { UnsubscribeEvent(EventDragPositionChanged, value); }
        }

        /// <summary>
        /// Event fired when dragging is enabled or disabled.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer whose setting has
        /// been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragEnabledChanged
        {
            add { SubscribeEvent(EventDragEnabledChanged, value); }
            remove { UnsubscribeEvent(EventDragEnabledChanged, value); }
        }

        /// <summary>
        /// Event fired when the alpha value used when dragging is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer whose drag alpha
        /// value has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragAlphaChanged
        {
            add { SubscribeEvent(EventDragAlphaChanged, value); }
            remove { UnsubscribeEvent(EventDragAlphaChanged, value); }
        }

        /// <summary>
        /// Event fired when the mouse cursor to used when dragging is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer whose dragging
        /// mouse cursor image has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragMouseCursorChanged
        {
            add { SubscribeEvent(EventDragMouseCursorChanged, value); }
            remove { UnsubscribeEvent(EventDragMouseCursorChanged, value); }
        }

        /// <summary>
        /// Event fired when the drag pixel threshold is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the DragContainer whose dragging pixel
        /// threshold has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragThresholdChanged
        {
            add { SubscribeEvent(EventDragThresholdChanged, value); }
            remove { UnsubscribeEvent(EventDragThresholdChanged, value); }
        }

        /// <summary>
        /// Event fired when the drop target changes.
        /// Handlers are passed a const DragDropEventArgs reference with
        /// WindowEventArgs::window set to the Window that is now the target
        /// window  and DragDropEventArgs::dragDropItem set to the DragContainer
        /// whose target has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragDropTargetChanged
        {
            add { SubscribeEvent(EventDragDropTargetChanged, value); }
            remove { UnsubscribeEvent(EventDragDropTargetChanged, value); }
        }

        #endregion

        /// <summary>
        /// Constructor for DragContainer objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public DragContainer(string type, string name)
            : base(type, name)
        {
            _draggingEnabled = true;
            _leftPointerHeld = false;
            _dragging = false;
            _dragThreshold = 8.0f;
            _dragAlpha = 0.5f;
            _dropTarget = null;
            _dragCursorImage = null;
            _dropflag = false;
            _stickyMode = false;
            _pickedUp = false;
            _usingFixedDragOffset = false;
            _fixedDragOffset = UVector2.Zero;

            AddDragContainerProperties();
        }

        /// <summary>
        /// Return whether dragging is currently enabled for this DragContainer.
        /// </summary>
        /// <returns>
        /// - true if dragging is enabled and the DragContainer may be dragged.
        /// - false if dragging is disabled and the DragContainer may not be dragged.
        /// </returns>
        public bool IsDraggingEnabled()
        {
            return _draggingEnabled;
        }

        /// <summary>
        /// Set whether dragging is currently enabled for this DragContainer.
        /// </summary>
        /// <param name="setting">
        /// - true to enable dragging so that the DragContainer may be dragged.
        /// - false to disabled dragging so that the DragContainer may not be dragged.
        /// </param>
        public void SetDraggingEnabled(bool setting)
        {
            if (_draggingEnabled != setting)
            {
                _draggingEnabled = setting;
                OnDragEnabledChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return whether the DragContainer is currently being dragged.
        /// </summary>
        /// <returns>
        /// - true if the DragContainer is being dragged.
        /// - false if te DragContainer is not being dragged.
        /// </returns>
        public bool IsBeingDragged()
        {
            return _dragging;
        }

        /// <summary>
        /// Return the current drag threshold in pixels.
        /// <para>
        /// The drag threshold is the number of pixels that the mouse must be
        /// moved with the left button held down in order to commence a drag
        /// operation.
        /// </para>
        /// </summary>
        /// <returns>
        /// float value indicating the current drag threshold value.
        /// </returns>
        public float GetPixelDragThreshold()
        {
            return _dragThreshold;
        }

        /// <summary>
        /// Set the current drag threshold in pixels.
        /// <para>
        /// The drag threshold is the number of pixels that the mouse must be
        /// moved with the left button held down in order to commence a drag
        /// operation.
        /// </para>
        /// </summary>
        /// <param name="pixels">
        /// float value indicating the new drag threshold value.
        /// </param>
        public void SetPixelDragThreshold(float pixels)
        {
            if (_dragThreshold != pixels)
            {
                _dragThreshold = pixels;
                OnDragThresholdChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the alpha value that will be set on the DragContainer while a drag operation is
        /// in progress.
        /// </summary>
        /// <returns>
        /// Current alpha value to use whilst dragging.
        /// </returns>
        public float GetDragAlpha()
        {
            return _dragAlpha;
        }

        /// <summary>
        /// Set the alpha value to be set on the DragContainer when a drag operation is
        /// in progress.
        /// <para>
        /// This method can be used while a drag is in progress to update the alpha.  Note that
        /// the normal setAlpha method does not affect alpha while a drag is in progress, but
        /// once the drag operation has ended, any value set via setAlpha will be restored.
        /// </para>
        /// </summary>
        /// <param name="alpha">
        /// Alpha value to use whilst dragging.
        /// </param>
        public void SetDragAlpha(float alpha)
        {
            if (_dragAlpha != alpha)
            {
                _dragAlpha = alpha;
                OnDragAlphaChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the Image currently set to be used for the mouse cursor when a 
        /// drag operation is in progress.
        /// </summary>
        /// <returns>
        /// Image object currently set to be used as the mouse cursor when dragging.
        /// </returns>
        public Image GetDragCursorImage()
        {
            return _dragCursorImage ?? GetGUIContext().GetCursor().GetDefaultImage();
        }

        /// <summary>
        /// Set the Image to be used for the mouse cursor when a drag operation is
        /// in progress.
        /// <para>
        /// This method may be used during a drag operation to update the current mouse
        /// cursor image.
        /// </para>
        /// </summary>
        /// <param name="image">
        /// Image object to be used as the mouse cursor while dragging.
        /// </param>
        public void SetDragCursorImage(Image image)
        {
            if (_dragCursorImage != image)
            {
                _dragCursorImage = image;
                OnDragMouseCursorChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the Image to be used for the mouse cursor when a drag operation is
        /// in progress.
        /// <para>
        /// This method may be used during a drag operation to update the current mouse
        /// cursor image.
        /// </para>
        /// </summary>
        /// <param name="name">
        /// Image to be used as the mouse cursor when dragging.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if Image \name is unknown.
        /// </exception>
        public void SetDragCursorImage(string name)
        {
            SetDragCursorImage(ImageManager.GetSingleton().Get(name));
        }

        /// <summary>
        /// Return the Window object that is the current drop target for the DragContainer.
        /// <para>
        /// The drop target for a DragContainer is basically the Window that the DragContainer
        /// is within while being dragged.  The drop target may be 0 to indicate no target.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to a Window object that contains the DragContainer whilst being dragged, or
        /// 0 to indicate no current target.
        /// </returns>
        public Window GetCurrentDropTarget()
        {
            return _dropTarget;
        }

        /// <summary>
        /// Return whether sticky mode is enable or disabled.
        /// </summary>
        /// <returns>
        /// - true if sticky mode is enabled.
        /// - false if sticky mode is disabled.
        /// </returns>
        public bool IsStickyModeEnabled()
        {
            return _stickyMode;
        }

        /// <summary>
        /// Enable or disable sticky mode.
        /// </summary>
        /// <param name="setting">
        /// - true to enable sticky mode.
        /// - false to disable sticky mode.
        /// </param>
        public void SetStickyModeEnabled(bool setting)
        {
            _stickyMode = setting;
        }

        /// <summary>
        /// Immediately pick up the DragContainer and optionally set the sticky 
        /// mode in order to allow this to happen.  Any current interaction
        /// (i.e. mouse capture) will be interrupted.
        /// </summary>
        /// <param name="forceSticky">
        /// - true to automatically enable the sticky mode in order to facilitate picking up the DragContainer.
        /// - false to ignore the pick up request if the sticky mode is not alraedy enabled (default).
        /// </param>
        /// <returns>
        /// - true if the DragContainer was successfully picked up.
        /// - false if the DragContainer was not picked up.
        /// </returns>
        public bool PickUp(bool forceSticky = false)
        {
            // check if we're already picked up or if dragging is disabled.
            if (_pickedUp || !_draggingEnabled)
                return true;

            // see if we need to force sticky mode switch
            if (!_stickyMode && forceSticky)
                SetStickyModeEnabled(true);

            // can only pick up if sticky
            if (_stickyMode)
            {
                // force immediate release of any current input capture (unless it's us)
                if (GetCaptureWindow() != null && GetCaptureWindow() != this)
                    GetCaptureWindow().ReleaseInput();
                // activate ourselves and try to capture input
                Activate();
                if (CaptureInput())
                {
                    // set the dragging point to the centre of the container.
                    _dragPoint.d_x = UDim.Absolute(d_pixelSize.Width/2);
                    _dragPoint.d_y = UDim.Absolute(d_pixelSize.Height/2);

                    // initialise the dragging state
                    InitialiseDragging();

                    // get position of mouse as co-ordinates local to this window.
                    var localMousePos = CoordConverter.ScreenToWindow(this,
                                                                      GetGUIContext().GetCursor().GetPosition());
                    DoDragging(localMousePos);

                    _pickedUp = true;
                }
            }

            return _pickedUp;
        }

        /// <summary>
        /// Set the fixed mouse cursor dragging offset to be used for this DragContainer.
        /// </summary>
        /// <param name="offset">
        /// UVector2 describing the fixed offset to be used when dragging this DragContainer.
        /// </param>
        /// <remarks>
        /// This offset is only used if it's use is enabled via the setUsingFixedDragOffset function.
        /// </remarks>
        public void SetFixedDragOffset(UVector2 offset)
        {
            _fixedDragOffset = offset;
        }

        /// <summary>
        /// Return the fixed mouse cursor dragging offset to be used for this DragContainer.
        /// </summary>
        /// <returns>
        /// UVector2 describing the fixed offset used when dragging this DragContainer.
        /// </returns>
        /// <remarks>
        /// This offset is only used if it's use is enabled via the setUsingFixedDragOffset function.
        /// </remarks>
        public UVector2 GetFixedDragOffset()
        {
            return _fixedDragOffset;
        }

        /// <summary>
        /// Set whether the fixed dragging offset - as set with the
        /// setFixedDragOffset - function will be used, or whether the built-in
        /// positioning will be used.
        /// </summary>
        /// <param name="enable">
        /// - true to enabled the use of the fixed offset.
        /// - false to use the regular logic.
        /// </param>
        public void SetUsingFixedDragOffset(bool enable)
        {
            _usingFixedDragOffset = enable;
        }

        /// <summary>
        /// Return whether the fixed dragging offset - as set with the
        /// setFixedDragOffset function - will be used, or whether the built-in
        /// positioning will be used.
        /// </summary>
        /// <returns>
        /// - true to enabled the use of the fixed offset.
        /// - false to use the regular logic.
        /// </returns>
        public bool IsUsingFixedDragOffset()
        {
            return _usingFixedDragOffset;
        }

        public override void GetRenderingContextImpl(out RenderingContext ctx)
        {
            // if not dragging, do the default thing.
            if (!_dragging)
                base.GetRenderingContextImpl(out ctx);

            // otherwise, switch rendering onto root rendering surface
            var root = GetRootWindow();
            ctx.surface = root.GetTargetRenderingSurface();
            // ensure root window is only used as owner if it really is.
            ctx.owner = root.GetRenderingSurface() == ctx.surface ? root : null;
            // ensure use of correct offset for the surface we're targetting
            ctx.offset = ctx.owner != null
                             ? ctx.owner.GetOuterRectClipper().Position
                             : Lunatics.Mathematics.Vector2.Zero;
            // draw to overlay queue
            ctx.queue = RenderQueueId.RQ_OVERLAY;
        }

        /// <summary>
        /// Return whether the required minimum movement threshold before initiating dragging
        /// has been exceeded.
        /// </summary>
        /// <param name="localMouse">
        /// Mouse position as a pixel offset from the top-left corner of this window.
        /// </param>
        /// <returns>
        /// - true if the threshold has been exceeded and dragging should be initiated.
        /// - false if the threshold has not been exceeded.
        /// </returns>
        protected bool IsDraggingThresholdExceeded(Lunatics.Mathematics.Vector2 localMouse)
        {
            // calculate amount mouse has moved.
            var deltaX = Math.Abs(localMouse.X - CoordConverter.AsAbsolute(_dragPoint.d_x, d_pixelSize.Width));
            var deltaY = Math.Abs(localMouse.Y - CoordConverter.AsAbsolute(_dragPoint.d_y, d_pixelSize.Height));

            // see if mouse has moved far enough to start dragging operation
            return (deltaX > _dragThreshold || deltaY > _dragThreshold);
        }

        /// <summary>
        /// Initialise the required states to put the window into dragging mode.
        /// </summary>
        protected void InitialiseDragging()
        {
            // only proceed if dragging is actually enabled
            if (_draggingEnabled)
            {
                // initialise drag moving state
                _storedClipState = d_clippedByParent;
                SetClippedByParent(false);
                _storedAlpha = d_alpha;
                SetAlpha(_dragAlpha);
                _startPosition = GetPosition();

                _dragging = true;

                NotifyScreenAreaChanged();

                // Now drag mode is set, change cursor as required
                UpdateActiveMouseCursor();
            }
        }

        /// <summary>
        /// Update state for window dragging.
        /// </summary>
        /// <param name="localMouse">
        /// Mouse position as a pixel offset from the top-left corner of this window.
        /// </param>
        protected void DoDragging(Lunatics.Mathematics.Vector2 localMouse)
        {
            // calculate amount to move
            var offset = new UVector2(UDim.Absolute(localMouse.X), UDim.Absolute(localMouse.Y));
            offset -= (_usingFixedDragOffset) ? _fixedDragOffset : _dragPoint;
            // set new position
            SetPosition(GetPosition() + offset);

            // Perform event notification
            OnDragPositionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Method to update mouse cursor image
        /// </summary>
        protected void UpdateActiveMouseCursor()
        {
            GetGUIContext().GetCursor().SetImage(_dragging ? GetDragCursorImage() : GetCursor());
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                // ensure all inputs come to us for now
                if (CaptureInput())
                {
                    // get position of mouse as co-ordinates local to this window.
                    var localPos = CoordConverter.ScreenToWindow(this, e.Position);

                    // store drag point for possible sizing or moving operation.
                    _dragPoint.d_x = UDim.Absolute(localPos.X);
                    _dragPoint.d_y = UDim.Absolute(localPos.Y);
                    _leftPointerHeld = true;
                }

                ++e.handled;
            }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                if (_dragging)
                {
                    // release picked up state
                    if (_pickedUp)
                        _pickedUp = false;

                    // fire off event
                    OnDragEnded(new WindowEventArgs(this));
                }
                    // check for sticky pick up
                else if (_stickyMode && !_pickedUp)
                {
                    InitialiseDragging();
                    _pickedUp = true;
                    // in this case, do not proceed to release inputs.
                    return;
                }

                // release our capture on the input data
                ReleaseInput();
                ++e.handled;
            }
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            base.OnCursorMove(e);

            // get position of mouse as co-ordinates local to this window.
            var localMousePos = CoordConverter.ScreenToWindow(this, e.Position);

            // handle dragging
            if (_dragging)
            {
                DoDragging(localMousePos);
            }
                // not dragging
            else
            {
                // if mouse button is down (but we're not yet being dragged)
                if (_leftPointerHeld)
                {
                    if (IsDraggingThresholdExceeded(localMousePos))
                    {
                        // Trigger the event
                        OnDragStarted(new WindowEventArgs(this));
                    }
                }
            }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            base.OnCaptureLost(e);

            // reset state
            if (_dragging)
            {
                // restore windows 'normal' state.
                _dragging = false;
                SetPosition(_startPosition);
                SetClippedByParent(_storedClipState);
                SetAlpha(_storedAlpha);

                NotifyScreenAreaChanged();

                // restore normal mouse cursor
                UpdateActiveMouseCursor();
            }

            _leftPointerHeld = false;
            _dropTarget = null;

            ++e.handled;
        }

        protected override void OnAlphaChanged(WindowEventArgs e)
        {
            // store new value and re-set dragging alpha as required.
            if (_dragging)
            {
                _storedAlpha = d_alpha;
                d_alpha = _dragAlpha;
            }

            base.OnAlphaChanged(e);
        }

        protected override void OnClippingChanged(WindowEventArgs e)
        {
            // store new value and re-set clipping for drag as required.
            if (_dragging)
            {
                _storedClipState = d_clippedByParent;
                d_clippedByParent = false;
            }

            base.OnClippingChanged(e);
        }

        protected override void OnMoved(ElementEventArgs e)
        {
            base.OnMoved(e);
            if (_dropflag)
            {
                _startPosition = GetPosition();
            }
        }

        /// <summary>
        /// Method called when dragging commences
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnDragStarted(WindowEventArgs e)
        {
            InitialiseDragging();

            FireEvent(EventDragStarted, e, EventNamespace);
        }

        /// <summary>
        /// Method called when dragging ends.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnDragEnded(WindowEventArgs e)
        {
            FireEvent(EventDragEnded, e, EventNamespace);

            // did we drop over a window?
            if (_dropTarget != null)
            {
                // set flag - we need to detect if the position changed in a DragDropItemDropped
                _dropflag = true;
                // Notify that item was dropped in the target window
                _dropTarget.NotifyDragDropItemDropped(this);
                // reset flag
                _dropflag = false;
            }
        }

        /// <summary>
        /// Method called when the dragged object position is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnDragPositionChanged(WindowEventArgs e)
        {
            FireEvent(EventDragPositionChanged, e, EventNamespace);

            var root = GetGUIContext().GetRootWindow();

            if (root != null)
            {
                // this hack with the 'enabled' state is so that getChildAtPosition
                // returns something useful instead of a pointer back to 'this'.
                // This hack is only acceptable because I am CrazyEddie!
                var wasEnabled = d_enabled;
                d_enabled = false;
                // find out which child of root window has the mouse in it
                var eventWindow = root.GetTargetChildAtPosition(GetGUIContext().GetCursor().GetPosition());
                d_enabled = wasEnabled;

                // use root itself if no child was hit
                if (eventWindow == null)
                    eventWindow = root;

                // if the window with the mouse is different to current drop target
                if (eventWindow != _dropTarget)
                {
                    OnDragDropTargetChanged(new DragDropEventArgs(eventWindow) {dragDropItem = this});
                }
            }
        }

        /// <summary>
        /// Method called when the dragging state is enabled or disabled
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnDragEnabledChanged(WindowEventArgs e)
        {
            FireEvent(EventDragEnabledChanged, e, EventNamespace);

            // abort current drag operation if dragging gets disabled part way through
            if (!_draggingEnabled && _dragging)
            {
                ReleaseInput();
            }
        }

        /// <summary>
        /// Method called when the alpha value to use when dragging is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnDragAlphaChanged(WindowEventArgs e)
        {
            FireEvent(EventDragAlphaChanged, e, EventNamespace);

            if (_dragging)
            {
                d_alpha = _storedAlpha;
                OnAlphaChanged(e);
            }
        }

        /// <summary>
        /// Method called when the mouse cursor to use when dragging is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnDragMouseCursorChanged(WindowEventArgs e)
        {
            FireEvent(EventDragMouseCursorChanged, e, EventNamespace);

            UpdateActiveMouseCursor();
        }

        /// <summary>
        /// Method called when the movement threshold required to trigger dragging is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnDragThresholdChanged(WindowEventArgs e)
        {
            FireEvent(EventDragThresholdChanged, e, EventNamespace);
        }

        /// <summary>
        /// Method called when the current drop target of this DragContainer changes.
        /// </summary>
        /// <param name="e">
        /// DragDropEventArgs object initialised as follows:
        /// - dragDropItem is initialised to the DragContainer triggering the event (typically 'this').
        /// - window is initialised to point to the Window which will be the new drop target.
        /// </param>
        /// <remarks>
        /// This event fires just prior to the target field being changed.  The default implementation
        /// changes the drop target, you can examine the old and new targets before calling the default
        /// implementation to make the actual change (and fire appropriate events for the Window objects
        /// involved).
        /// </remarks>
        protected virtual void OnDragDropTargetChanged(DragDropEventArgs e)
        {
            FireEvent(EventDragDropTargetChanged, e, EventNamespace);

            // Notify old target that drop item has left
            if (_dropTarget != null)
            {
                _dropTarget.NotifyDragDropItemLeaves(this);
            }

            // update to new target
            _dropTarget = e.Window;

            while ((_dropTarget != null) && !_dropTarget.IsDragDropTarget())
                _dropTarget = _dropTarget.GetParent();

            // Notify new target window that someone has dragged a DragContainer over it
            if (_dropTarget != null)
                _dropTarget.NotifyDragDropItemEnters(this);
        }

        /// <summary>
        /// Adds properties specific to the DragContainer base class.
        /// </summary>
        private void AddDragContainerProperties()
        {
            DefineProperty(
                "DraggingEnabled",
                "Property to get/set the state of the dragging enabled setting for the DragContainer.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetDraggingEnabled(v), x => x.IsDraggingEnabled(), true);

            DefineProperty(
                "DragAlpha", "Property to get/set the dragging alpha value.  Value is a float.",
                (x, v) => x.SetDragAlpha(v), x => x.GetDragAlpha(), 0.5f);

            // TODO: Inconsistency
            DefineProperty(
                "DragThreshold", "Property to get/set the dragging threshold value.  Value is a float.",
                (x, v) => x.SetPixelDragThreshold(v), x => x.GetPixelDragThreshold(), 8.0f);

            DefineProperty(
                "DragCursorImage",
                "Property to get/set the mouse cursor image used when dragging.  Value should be \"set:<imageset name> image:<image name>\".",
                (x, v) => x.SetDragCursorImage(v), x => x.GetDragCursorImage(), null);

            // TODO: Inconsistency
            DefineProperty(
                "StickyMode",
                "Property to get/set the state of the sticky mode setting for the DragContainer.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetStickyModeEnabled(v), x => x.IsStickyModeEnabled(), true);

            DefineProperty(
                "FixedDragOffset",
                "Property to get/set the state of the fixed dragging offset setting for the DragContainer.  Value is a UVector2 property value.",
                (x, v) => x.SetFixedDragOffset(v), x => x.GetFixedDragOffset(), UVector2.Zero);

            // TODO: Inconsistency
            DefineProperty(
                "UseFixedDragOffset",
                "Property to get/set the setting that control whether the fixed dragging offset will be used.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetUsingFixedDragOffset(v), x => x.IsUsingFixedDragOffset(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<DragContainer, T> setter,
                                       Func<DragContainer, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<DragContainer, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// True when dragging is enabled.
        /// </summary>
        private bool _draggingEnabled;

        /// <summary>
        /// True when left mouse button is down.
        /// </summary>
        private bool _leftPointerHeld;

        /// <summary>
        /// true when being dragged.
        /// </summary>
        private bool _dragging;

        /// <summary>
        /// point we are being dragged at.
        /// </summary>
        private UVector2 _dragPoint;

        /// <summary>
        /// position prior to dragging.
        /// </summary>
        private UVector2 _startPosition;

        /// <summary>
        /// Pixels mouse must move before dragging commences.
        /// </summary>
        private float _dragThreshold;

        /// <summary>
        /// Alpha value to set when dragging.
        /// </summary>
        private float _dragAlpha;

        /// <summary>
        /// Alpha value to re-set when dragging ends.
        /// </summary>
        private float _storedAlpha;

        /// <summary>
        /// Parent clip state to re-set.
        /// </summary>
        private bool _storedClipState;

        /// <summary>
        /// Target window for possible drop operation.
        /// </summary>
        private Window _dropTarget;

        /// <summary>
        /// Image to use for mouse cursor when dragging.
        /// </summary>
        private Image _dragCursorImage;

        /// <summary>
        /// True when we're being dropped
        /// </summary>
        private bool _dropflag;

        /// <summary>
        /// true when we're in 'sticky' mode.
        /// </summary>
        private bool _stickyMode;

        /// <summary>
        /// true after been picked-up / dragged via sticky mode
        /// </summary>
        private bool _pickedUp;

        /// <summary>
        /// true if fixed mouse offset is used for dragging position.
        /// </summary>
        private bool _usingFixedDragOffset;

        /// <summary>
        /// current fixed mouse offset value.
        /// </summary>
        private UVector2 _fixedDragOffset;

        #endregion
    }
}