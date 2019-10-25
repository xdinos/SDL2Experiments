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
    /// Base class for list header segment window
    /// </summary>
    public class ListHeaderSegment : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ListHeaderSegment";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ListHeaderSegment";

        /// <summary>
        /// Event fired when the segment is clicked.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that was clicked.
        /// </summary>
        public event EventHandler<WindowEventArgs> SegmentClicked;

        /// <summary>
        /// Event fired when the sizer/splitter is double-clicked.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose
        /// sizer / splitter area was double-clicked.
        /// </summary>
        public event EventHandler<WindowEventArgs> SplitterDoubleClicked;

        /// <summary>
        /// Event fired when the user drag-sizable setting is changed.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose user sizable
        /// setting has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> SizingSettingChanged;

        /// <summary>
        /// Event fired when the sort direction value is changed.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose sort direction
        /// has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> SortDirectionChanged;

        /// <summary>
        /// Event fired when the user drag-movable setting is changed.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose user
        /// drag-movable setting has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> MovableSettingChanged;

        /// <summary>
        /// Event fired when the segment has started to be dragged.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that has started to
        /// be dragged.
        /// </summary>
        public event EventHandler<WindowEventArgs> SegmentDragStart;

        /// <summary>
        /// Event fired when segment dragging has stopped (via mouse release).
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that is no longer
        /// being dragged.
        /// </summary>
        public event EventHandler<WindowEventArgs> SegmentDragStop;

        /// <summary>
        /// Event fired when the segment drag position has changed.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose position has
        /// changed due to being dragged.
        /// </summary>
        public event EventHandler<WindowEventArgs> SegmentDragPositionChanged;

        /// <summary>
        /// Event fired when the segment is sized by the user.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment that has been
        /// resized by the user dragging. 
        /// </summary>
        public event EventHandler<WindowEventArgs> SegmentSized;

        /// <summary>
        /// Event fired when the clickable setting for the segment is changed.
        /// Hanlders are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ListHeaderSegment whose setting that
        /// controls whether the segment is clickable has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> ClickableSettingChanged;

        /// <summary>
        /// Default size of the sizing area.
        /// </summary>
        public const float DefaultSizingArea = 8.0f;

        /// <summary>
        /// Amount the mouse must be dragged before drag-moving is initiated.
        /// </summary>
        public const float SegmentMoveThreshold = 12.0f;

        /// <summary>
        /// Enumeration of possible values for sorting direction used with ListHeaderSegment classes
        /// </summary>
        public enum SortDirection
        {
            /// <summary>
            /// Items under this segment should not be sorted.
            /// </summary>
            None,

            /// <summary>
            /// Items under this segment should be sorted in ascending order.
            /// </summary>
            Ascending,

            /// <summary>
            /// Items under this segment should be sorted in descending order.
            /// </summary>
            Descending
        }

        /// <summary>
        /// Return whether this segment can be sized.
        /// </summary>
        /// <returns>
        /// true if the segment can be horizontally sized, false if the segment can not be horizontally sized.
        /// </returns>
        public bool IsSizingEnabled()
        {
            return _sizingEnabled;
        }

        /// <summary>
        /// Return the current sort direction set for this segment.
        /// <para>
        /// Note that this has no impact on the way the segment functions (aside from the possibility
        /// of varied rendering).  This is intended as a 'helper setting' to classes that make use of
        /// the ListHeaderSegment objects.
        /// </para>
        /// </summary>
        /// <returns>
        /// One of the SortDirection enumerated values indicating the current sort direction set for this
        /// segment.
        /// </returns>
        public SortDirection GetSortDirection()
        {
            return _sortDir;
        }

        /// <summary>
        /// Return whether drag moving is enabled for this segment.
        /// </summary>
        /// <returns>
        /// true if the segment can be drag moved, false if the segment can not be drag moved.
        /// </returns>
        public bool IsDragMovingEnabled()
        {
            return _movingEnabled;
        }

        /// <summary>
        /// Return the current drag move position offset (in pixels relative to the top-left corner of the segment).
        /// </summary>
        /// <returns>
        /// Point object describing the drag move offset position.
        /// </returns>
        public Lunatics.Mathematics.Vector2 GetDragMoveOffset()
        {
            return _dragPosition;
        }

        /// <summary>
        /// Return whether the segment is clickable.
        /// </summary>
        /// <returns>
        /// true if the segment can be clicked, false of the segment can not be clicked (so no highlighting or events will happen).
        /// </returns>
        public bool IsClickable()
        {
            return _allowClicks;
        }

        /// <summary>
        /// Return whether the segment is currently in its hovering state.
        /// </summary>
        /// <returns></returns>
        public bool IsSegmentHovering()
        {
            return _segmentHover;
        }

        /// <summary>
        /// Return whether the segment is currently in its pushed state.
        /// </summary>
        /// <returns></returns>
        public bool IsSegmentPushed()
        {
            return _segmentPushed;
        }

        /// <summary>
        /// Return whether the splitter is currently in its hovering state.
        /// </summary>
        /// <returns></returns>
        public bool IsSplitterHovering()
        {
            return _splitterHover;
        }

        /// <summary>
        /// Return whether the segment is currently being drag-moved.
        /// </summary>
        /// <returns></returns>
        public bool IsBeingDragMoved()
        {
            return _dragMoving;
        }

        /// <summary>
        /// Return whether the segment is currently being drag-moved.
        /// </summary>
        /// <returns></returns>
        public bool IsBeingDragSized()
        {
            return _dragSizing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image GetSizingCursorImage()
        {
            return _sizingMouseCursor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image GetMovingCursorImage()
        {
            return _movingMouseCursor;
        }

        /// <summary>
        /// Set whether this segment can be sized.
        /// </summary>
        /// <param name="setting">
        /// true if the segment may be horizontally sized, false if the segment may not be horizontally sized.
        /// </param>
        public void SetSizingEnabled(bool setting)
        {
            if (_sizingEnabled != setting)
            {
                _sizingEnabled = setting;

                // if sizing is now disabled, ensure sizing operation is cancelled
                if (!_sizingEnabled && _dragSizing)
                    ReleaseInput();

                OnSizingSettingChanged(new WindowEventArgs(this));
            }

        }

        /// <summary>
        /// Set the current sort direction set for this segment.
        /// <para>
        /// Note that this has no impact on the way the segment functions (aside from the possibility
        /// of varied rendering).  This is intended as a 'helper setting' to classes that make use of
        /// the ListHeaderSegment objects.
        /// </para>
        /// </summary>
        /// <param name="sortDirerction">
        /// One of the SortDirection enumerated values indicating the current sort direction set for this segment.
        /// </param>
        public void SetSortDirection(SortDirection sortDirerction)
        {
            if (_sortDir != sortDirerction)
            {
                _sortDir = sortDirerction;
                OnSortDirectionChanged(new WindowEventArgs(this));
                Invalidate(false);
            }
        }

        /// <summary>
        /// Set whether drag moving is allowed for this segment.
        /// </summary>
        /// <param name="setting">
        /// true if the segment may be drag moved, false if the segment may not be drag moved.
        /// </param>
        public void SetDragMovingEnabled(bool setting)
        {
            if (_movingEnabled != setting)
            {
                _movingEnabled = setting;
                OnMovableSettingChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set whether the segment is clickable.
        /// </summary>
        /// <param name="setting">
        /// true if the segment may be clicked, false of the segment may not be clicked (so no highlighting or events will happen).
        /// </param>
        public void SetClickable(bool setting)
        {
            if (_allowClicks != setting)
            {
                _allowClicks = setting;

                OnClickableSettingChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void SetSizingCursorImage(Image image)
        {
            _sizingMouseCursor = image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetSizingCursorImage(String name)
        {
            _sizingMouseCursor = ImageManager.GetSingleton().Get(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void SetMovingCursorImage(Image image)
        {
            _movingMouseCursor = image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetMovingCursorImage(String name)
        {
            _movingMouseCursor = ImageManager.GetSingleton().Get(name);
        }

        /// <summary>
        /// Constructor for list header segment base class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ListHeaderSegment(string type, string name)
            : base(type, name)
        {
            _sizingMouseCursor = null;
            _movingMouseCursor = null;
            _splitterSize = DefaultSizingArea;
            _splitterHover = false;
            _dragSizing = false;
            _sortDir = SortDirection.None;
            _segmentHover = false;
            _segmentPushed = false;
            _sizingEnabled = true;
            _movingEnabled = true;
            _dragMoving = false;
            _allowClicks = true;

            AddHeaderSegmentProperties();
        }

        /// <summary>
        /// Update state for drag sizing.
        /// </summary>
        /// <param name="localMouse">
        /// Mouse position as a pixel offset from the top-left corner of this window.
        /// </param>
        protected void DoDragSizing(Lunatics.Mathematics.Vector2 localMouse)
        {
            var delta = localMouse.X - _dragPoint.X;
            // store this so we can work out how much size actually changed
            var orgWidth = d_pixelSize.Width;

            // ensure that we only size to the set constraints.
            //
            // NB: We are required to do this here due to our virtually unique sizing nature; the
            // normal system for limiting the window size is unable to supply the information we
            // require for updating our internal state used to manage the dragging, etc.
            var maxWidth = CoordConverter.AsAbsolute(d_maxSize.d_width, GetRootContainerSize().Width);
            var minWidth = CoordConverter.AsAbsolute(d_minSize.d_width, GetRootContainerSize().Width);
            var newWidth = orgWidth + delta;

            if (maxWidth != 0.0f && newWidth > maxWidth)
                delta = maxWidth - orgWidth;
            else if (newWidth < minWidth)
                delta = minWidth - orgWidth;

            // update segment area rect
            // URGENT FIXME: The pixel alignment will be done automatically again, right? Why is it done here? setArea_impl will do it!
            var area = new URect(d_area.d_min.d_x, d_area.d_min.d_y,
                                 d_area.d_max.d_x + new UDim(0, /*PixelAligned(*/delta /*)*/), d_area.d_max.d_y);
            SetAreaImpl(area.d_min, area.Size);

            // move the dragging point so mouse remains 'attached' to edge of segment
            _dragPoint.X += d_pixelSize.Width - orgWidth;

            OnSegmentSized(new WindowEventArgs(this));
        }

        /// <summary>
        /// Update state for drag moving.
        /// </summary>
        /// <param name="localMouse">
        /// Mouse position as a pixel offset from the top-left corner of this window.
        /// </param>
        protected void DoDragMoving(Lunatics.Mathematics.Vector2 localMouse)
        {
            // calculate movement deltas.
            var deltaX = localMouse.X - _dragPoint.X;
            var deltaY = localMouse.Y - _dragPoint.Y;

            // update 'ghost' position
            _dragPosition.X += deltaX;
            _dragPosition.Y += deltaY;

            // update drag point.
            _dragPoint.X += deltaX;
            _dragPoint.Y += deltaY;

            OnSegmentDragPositionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Initialise the required states to put the widget into drag-moving mode.
        /// </summary>
        protected void InitDragMoving()
        {
            if (_movingEnabled)
            {
                // initialise drag moving state
                _dragMoving = true;
                _segmentPushed = false;
                _segmentHover = false;
                _dragPosition.X = 0.0f;
                _dragPosition.Y = 0.0f;

                // setup new cursor
                GetGUIContext().GetCursor().SetImage(_movingMouseCursor);

                // Trigger the event
                OnSegmentDragStart(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Initialise the required states when we are hovering over the sizing area.
        /// </summary>
        protected void InitSizingHoverState()
        {
            // only react if settings are changing.
            if (!_splitterHover && !_segmentPushed)
            {
                _splitterHover = true;

                // change the mouse cursor.
                GetGUIContext().GetCursor().SetImage(_sizingMouseCursor);

                // trigger redraw so 'sizing' area can be highlighted if needed.
                Invalidate(false);
            }

            // reset segment hover as needed.
            if (_segmentHover)
            {
                _segmentHover = false;
                Invalidate(false);
            }
        }

        /// <summary>
        /// Initialise the required states when we are hovering over the main segment area.
        /// </summary>
        protected void InitSegmentHoverState()
        {
            // reset sizing area hover state if needed.
            if (_splitterHover)
            {
                _splitterHover = false;
                GetGUIContext().GetCursor().SetImage(GetCursor());
                Invalidate(false);
            }

            // set segment hover state if not already set.
            if ((!_segmentHover) && IsClickable())
            {
                _segmentHover = true;
                Invalidate(false);
            }
        }

        /// <summary>
        /// Return whether the required minimum movement threshold before initiating drag-moving
        /// has been exceeded.
        /// </summary>
        /// <param name="localMouse">
        /// Mouse position as a pixel offset from the top-left corner of this window.
        /// </param>
        /// <returns>
        /// true if the threshold has been exceeded and drag-moving should be initiated, or false
        /// if the threshold has not been exceeded.
        /// </returns>
        protected bool IsDragMoveThresholdExceeded(Lunatics.Mathematics.Vector2 localMouse)
        {
            // see if mouse has moved far enough to start move operation
            // calculate movement deltas.
            var deltaX = localMouse.X - _dragPoint.X;
            var deltaY = localMouse.Y - _dragPoint.Y;

            if ((deltaX > SegmentMoveThreshold) || (deltaX < -SegmentMoveThreshold) ||
                (deltaY > SegmentMoveThreshold) || (deltaY < -SegmentMoveThreshold))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handler called when segment is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentClicked(WindowEventArgs e)
        {
            FireEvent(SegmentClicked, e);
        }

        /// <summary>
        /// Handler called when the sizer/splitter is double-clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSplitterDoubleClicked(WindowEventArgs e)
        {
            FireEvent(SplitterDoubleClicked, e);
        }

        /// <summary>
        /// Handler called when sizing setting changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSizingSettingChanged(WindowEventArgs e)
        {
            FireEvent(SizingSettingChanged, e);
        }

        /// <summary>
        /// Handler called when the sort direction value changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSortDirectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SortDirectionChanged, e);
        }

        /// <summary>
        /// Handler called when the drag-movable setting is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMovableSettingChanged(WindowEventArgs e)
        {
            FireEvent(MovableSettingChanged, e);
        }

        /// <summary>
        /// Handler called when the user starts dragging the segment.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentDragStart(WindowEventArgs e)
        {
            FireEvent(SegmentDragStart, e);
        }

        /// <summary>
        /// Handler called when the user stops dragging the segment (releases mouse button)
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentDragStop(WindowEventArgs e)
        {
            FireEvent(SegmentDragStop, e);
        }

        /// <summary>
        /// Handler called when the drag position changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentDragPositionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SegmentDragPositionChanged, e);
        }

        /// <summary>
        /// Handler called when the segment is sized.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSegmentSized(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SegmentSized, e);
        }

        /// <summary>
        /// Handler called when the clickable setting for the segment changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClickableSettingChanged(WindowEventArgs e)
        {
            FireEvent(ClickableSettingChanged, e);
        }


        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorMove(e);

            // convert mouse position to something local
            var localMousePos = CoordConverter.ScreenToWindow(this, e.Position);

            if (_dragSizing)
            {
                // handle drag sizing
                DoDragSizing(localMousePos);
            }
            else if (_dragMoving)
            {
                // handle drag moving
                DoDragMoving(localMousePos);
            }
            else if (IsHit(e.Position))
            {
                // not sizing, is mouse in the widget area?

                // mouse in sizing area & sizing is enabled
                if ((localMousePos.X > (d_pixelSize.Width - _splitterSize)) && _sizingEnabled)
                {
                    InitSizingHoverState();
                }
                    // mouse not in sizing area and/or sizing not enabled
                else
                {
                    InitSegmentHoverState();

                    // if we are pushed but not yet drag moving
                    if (_segmentPushed && !_dragMoving)
                    {
                        if (IsDragMoveThresholdExceeded(localMousePos))
                        {
                            InitDragMoving();
                        }
                    }
                }
            }
            else
            {
                // mouse is no longer within the widget area...

                // only change settings if change is required
                if (_splitterHover)
                {
                    _splitterHover = false;
                    GetGUIContext().GetCursor().SetImage(GetCursor());
                    Invalidate(false);
                }

                // reset segment hover state if not already done.
                if (_segmentHover)
                {
                    _segmentHover = false;
                    Invalidate(false);
                }
            }
            ++e.handled;
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                // ensure all inputs come to us for now
                if (CaptureInput())
                {
                    // get position of mouse as co-ordinates local to this window.
                    var localPos = CoordConverter.ScreenToWindow(this, e.Position);

                    // store drag point for possible sizing or moving operation.
                    _dragPoint = localPos;

                    // if the mouse is in the sizing area
                    if (_splitterHover)
                    {
                        if (IsSizingEnabled())
                        {
                            // setup the 'dragging' state variables
                            _dragSizing = true;
                        }

                    }
                    else
                    {
                        _segmentPushed = true;
                    }

                }

                ++e.handled;
            }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                // if we were pushed and mouse was released within our segment area
                if (_segmentPushed && _segmentHover)
                {
                    OnSegmentClicked(new WindowEventArgs(this));
                }
                else if (_dragMoving)
                {
                    GetGUIContext().GetCursor().SetImage(GetCursor());

                    OnSegmentDragStop(new WindowEventArgs(this));
                }

                // release our capture on the input data
                ReleaseInput();
                ++e.handled;
            }
        }

        protected internal override void OnCursorLeaves(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorLeaves(e);

            _splitterHover = false;
            _dragSizing = false;
            _segmentHover = false;
            Invalidate(false);
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // base class processing
            base.OnCaptureLost(e);

            // reset segment state
            _dragSizing = false;
            _segmentPushed = false;
            _dragMoving = false;

            ++e.handled;
        }

        private void AddHeaderSegmentProperties()
        {
            // TODO: Inconsistency
            DefineProperty(
                "Sizable",
                "Property to get/set the sizable setting of the header segment.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetSizingEnabled(v), x => x.IsSizingEnabled(), true);

            DefineProperty(
                "Clickable",
                "Property to get/set the click-able setting of the header segment.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetClickable(v), x => x.IsClickable(), true);

            // TODO: Inconsistency
            DefineProperty(
                "Dragable",
                "Property to get/set the drag-able setting of the header segment.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetDragMovingEnabled(v), x => x.IsDragMovingEnabled(), true);

            DefineProperty(
                "SortDirection",
                "Property to get/set the sort direction setting of the header segment.  Value is the text of one of the SortDirection enumerated value names.",
                (x, v) => x.SetSortDirection(v), x => x.GetSortDirection(), SortDirection.None);

            DefineProperty(
                "SizingCursorImage",
                "Property to get/set the sizing cursor image for the List Header Segment.  Value should be \"set:[imageset name] image:[image name]\".",
                (x, v) => x.SetSizingCursorImage(v), x => x.GetSizingCursorImage(), null);

            DefineProperty(
                "MovingCursorImage",
                "Property to get/set the moving cursor image for the List Header Segment.  Value should be \"set:[imageset name] image:[image name]\".",
                (x, v) => x.SetMovingCursorImage(v), x => x.GetMovingCursorImage(), null);
        }

        private void DefineProperty<T>(string name, string help, Action<ListHeaderSegment, T> setter,
                                       Func<ListHeaderSegment, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<ListHeaderSegment, T>(name, help, setter, getter, WidgetTypeName,
                                                                    defaultValue));
        }

        #region Fields

        /// <summary>
        /// Image to use for mouse when sizing (typically set by derived class).
        /// </summary>
        private Image _sizingMouseCursor;

        /// <summary>
        /// Image to use for mouse when moving (typically set by derived class).
        /// </summary>
        private Image _movingMouseCursor;

        /// <summary>
        /// pixel width of the sizing area.
        /// </summary>
        private float _splitterSize;

        /// <summary>
        /// True if the mouse is over the splitter
        /// </summary>
        private bool _splitterHover;

        /// <summary>
        /// true when we are being sized.
        /// </summary>
        private bool _dragSizing;

        /// <summary>
        /// point we are being dragged at when sizing or moving.
        /// </summary>
        private Lunatics.Mathematics.Vector2 _dragPoint;

        /// <summary>
        /// Direction for sorting (used for deciding what icon to display).
        /// </summary>
        private SortDirection _sortDir;

        /// <summary>
        /// true when the mouse is within the segment area (and not in sizing area).
        /// </summary>
        private bool _segmentHover;

        /// <summary>
        /// true when the left mouse button has been pressed within the confines of the segment.
        /// </summary>
        private bool _segmentPushed;

        /// <summary>
        /// true when sizing is enabled for this segment.
        /// </summary>
        private bool _sizingEnabled;

        /// <summary>
        /// True when drag-moving is enabled for this segment;
        /// </summary>
        private bool _movingEnabled;

        /// <summary>
        /// true when segment is being drag moved.
        /// </summary>
        private bool _dragMoving;

        /// <summary>
        /// position of dragged segment.
        /// </summary>
        private Lunatics.Mathematics.Vector2 _dragPosition;

        /// <summary>
        /// true if the segment can be clicked.
        /// </summary>
        private bool _allowClicks;

        #endregion
    }
}