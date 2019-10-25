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
    /// Base class for Scrollbar window renderer objects.
    /// </summary>
    public abstract class ScrollbarWindowRenderer : WindowRenderer
    {
        protected ScrollbarWindowRenderer(string name)
            : base(name, Scrollbar.EventNamespace)
        {

        }

        /// <summary>
        /// update the size and location of the thumb to properly represent the
        /// current state of the scroll bar
        /// </summary>
        public abstract void UpdateThumb();

        /// <summary>
        /// return value that best represents current scroll bar position given the
        /// current location of the thumb.
        /// </summary>
        /// <returns>
        /// float value that, given the thumb widget position, best represents the
        /// current position for the scroll bar.
        /// </returns>
        public abstract float GetValueFromThumb();

        /// <summary>
        /// Given window location \a pt, return a value indicating what change
        /// should be made to the scroll bar.
        /// </summary>
        /// <param name="pt">
        /// Point object describing a pixel position in window space.
        /// </param>
        /// <returns>
        /// - -1 to indicate scroll bar position should be moved to a lower value.
        /// -  0 to indicate scroll bar position should not be changed.
        /// - +1 to indicate scroll bar position should be moved to a higher value.
        /// </returns>
        public abstract float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt);
    }

    /// <summary>
    /// Base scroll bar class.
    /// 
    /// This base class for scroll bars does not have any idea of direction - a
    /// derived class would add whatever meaning is appropriate according to what
    /// that derived class represents to the user.
    /// </summary>
    public class Scrollbar : Window
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Scrollbar";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Scrollbar";

        public const string EventScrollPositionChanged = "ScrollPositionChanged";
        public const string EventThumbTrackStarted = "ThumbTrackStarted";
        public const string EventThumbTrackEnded = "ThumbTrackEnded";
        public const string EventScrollConfigChanged = "ScrollConfigChanged";

        /// <summary>
        /// Event fired when the scroll bar position value changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Scrollbar whose position value had
        /// changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ScrollPositionChanged
        {
            add { SubscribeEvent(EventScrollPositionChanged, value); }
            remove { UnsubscribeEvent(EventScrollPositionChanged, value); }
        }

        /// <summary>
        /// Event fired when the user begins dragging the scrollbar thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Scrollbar whose thumb is being
        /// dragged.
        /// </summary>
        public event GuiEventHandler<EventArgs> ThumbTrackStarted
        {
            add { SubscribeEvent(EventThumbTrackStarted, value); }
            remove { UnsubscribeEvent(EventThumbTrackStarted, value); }
        }

        /// <summary>
        /// Event fired when the user releases the scrollbar thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Scrollbar whose thumb has been
        /// released.
        /// </summary>
        public event GuiEventHandler<EventArgs> ThumbTrackEnded
        {
            add { SubscribeEvent(EventThumbTrackEnded, value); }
            remove { UnsubscribeEvent(EventThumbTrackEnded, value); }
        }

        /// <summary>
        /// Event fired when the scroll bar configuration data is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Scrollbar whose configuration
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ScrollConfigChanged
        {
            add { SubscribeEvent(EventScrollConfigChanged, value); }
            remove { UnsubscribeEvent(EventScrollConfigChanged, value); }
        }

        #endregion

        /// <summary>
        /// Widget name for the thumb component.
        /// </summary>
        public const string ThumbName = "__auto_thumb__";

        /// <summary>
        /// Widget name for the increase button component.
        /// </summary>
        public const string IncreaseButtonName = "__auto_incbtn__";

        /// <summary>
        /// Widget name for the decrease button component.
        /// </summary>
        public const string DecreaseButtonName = "__auto_decbtn__";

        /// <summary>
        /// Return the size of the document or data.
        /// 
        /// The document size should be thought of as the total size of the data
        /// that is being scrolled through (the number of lines in a text file for
        /// example).
        /// </summary>
        /// <returns>
        /// float value specifying the currently set document size.
        /// </returns>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public float GetDocumentSize()
        {
            return _documentSize;
        }

        /// <summary>
        /// Set the size of the document or data.
        /// 
        /// The document size should be thought of as the total size of the data
        /// that is being scrolled through (the number of lines in a text file for
        /// example).
        /// </summary>
        /// <param name="documentSize">
        /// float value specifying the document size.
        /// </param>
        /// <remarks>
        /// The value set has no meaning within the Gui system, it is left up to
        /// the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public void SetDocumentSize(float documentSize)
        {
            if (Math.Abs(_documentSize - documentSize) > float.Epsilon)
            {
                var resetMaxPosition = _endLockPosition && IsAtEnd();

                _documentSize = documentSize;

                if (resetMaxPosition)
                    SetScrollPosition(GetMaxScrollPosition());
                else
                    UpdateThumb();

                OnScrollConfigChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the page size for this scroll bar.
        /// 
        /// The page size is typically the amount of data that can be displayed at
        /// one time.  This value is also used when calculating the amount the
        /// position will change when you click either side of the scroll bar
        /// thumb, the amount the position changes will is (pageSize - overlapSize).
        /// </summary>
        /// <returns>
        /// float value specifying the currently set page size.
        /// </returns>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public float GetPageSize()
        {
            return _pageSize;
        }

        /// <summary>
        /// Set the page size for this scroll bar.
        /// 
        /// The page size is typically the amount of data that can be displayed at
        /// one time.  This value is also used when calculating the amount the
        /// position will change when you click either side of the scroll bar
        /// thumb, the amount the position changes will is (pageSize - overlapSize).
        /// </summary>
        /// <param name="pageSize">
        /// float value specifying the page size.
        /// </param>
        /// <remarks>
        /// The value set has no meaning within the Gui system, it is left up to the
        /// application to assign appropriate values for the application specific
        /// use of the scroll bar.
        /// </remarks>
        public void SetPageSize(float pageSize)
        {
            if (Math.Abs(_pageSize - pageSize) > float.Epsilon)
            {
                var resetMaxPosition = _endLockPosition && IsAtEnd();

                _pageSize = pageSize;

                if (resetMaxPosition)
                    SetScrollPosition(GetMaxScrollPosition());
                else
                    UpdateThumb();

                OnScrollConfigChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the step size for this scroll bar.
        /// 
        /// The step size is typically a single unit of data that can be displayed,
        /// this is the amount the position will change when you click either of
        /// the arrow buttons on the scroll bar.  (this could be 1 for a single
        /// line of text, for example).
        /// </summary>
        /// <returns>
        /// float value specifying the currently set step size.
        /// </returns>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public float GetStepSize()
        {
            return _stepSize;
        }

        /// <summary>
        /// Set the step size for this scroll bar.
        /// 
        /// The step size is typically a single unit of data that can be displayed,
        /// this is the amount the position will change when you click either of the
        /// arrow buttons on the scroll bar.  (this could be 1 for a single line of
        /// text, for example).
        /// </summary>
        /// <param name="stepSize">
        /// float value specifying the step size.
        /// </param>
        /// <remarks>
        /// The value set has no meaning within the Gui system, it is left up to the
        /// application to assign appropriate values for the application specific
        /// use of the scroll bar.
        /// </remarks>
        public void SetStepSize(float stepSize)
        {
            if (Math.Abs(_stepSize - stepSize) > float.Epsilon)
            {
                _stepSize = stepSize;

                OnScrollConfigChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the overlap size for this scroll bar.
        /// 
        /// The overlap size is the amount of data from the end of a 'page' that
        /// will remain visible when the position is moved by a page.  This is
        /// usually used so that the user keeps some context of where they were
        /// within the document's data when jumping a page at a time.
        /// </summary>
        /// <returns>
        /// float value specifying the currently set overlap size.
        /// </returns>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public float GetOverlapSize()
        {
            return _overlapSize;
        }

        /// <summary>
        /// Set the overlap size for this scroll bar.
        /// 
        /// The overlap size is the amount of data from the end of a 'page' that
        /// will remain visible when the position is moved by a page.  This is
        /// usually used so that the user keeps some context of where they were
        /// within the document's data when jumping a page at a time.
        /// </summary>
        /// <param name="overlapSize">
        /// float value specifying the overlap size.
        /// </param>
        /// <remarks>
        /// The value set has no meaning within the Gui system, it is left up to the
        /// application to assign appropriate values for the application specific
        /// use of the scroll bar.
        /// </remarks>
        public void SetOverlapSize(float overlapSize)
        {
            if (Math.Abs(_overlapSize - overlapSize) > float.Epsilon)
            {
                _overlapSize = overlapSize;

                OnScrollConfigChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the current position of scroll bar within the document.
        /// 
        /// The range of the scroll bar is from 0 to the size of the document minus
        /// the size of a page <c>(0 lt;= position lt;= (documentSize - pageSize)).</c>
        /// </summary>
        /// <returns>
        /// float value specifying the current position of the scroll bar within its document.
        /// </returns>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public float GetScrollPosition()
        {
            return _position;
        }

        /// <summary>
        /// Set the current position of scroll bar within the document.
        /// 
        /// The range of the scroll bar is from 0 to the size of the document minus
        /// the size of a page <c>(0 lt;= position lt;= (documentSize - pageSize))</c>, any
        /// attempt to set the position outside this range will be adjusted so that
        /// it falls within the legal range.
        /// </summary>
        /// <param name="position">
        /// float value specifying the position of the scroll bar within its
        /// document.
        /// </param>
        /// <remarks>
        /// The returned value has no meaning within the Gui system, it is left up
        /// to the application to assign appropriate values for the application
        /// specific use of the scroll bar.
        /// </remarks>
        public void SetScrollPosition(float position)
        {
            var modified = SetScrollPositionImpl(position);
            UpdateThumb();

            // notification if required
            if (modified)
            {
                OnScrollPositionChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// return the current scroll position as a value in the interval [0, 1]
        /// </summary>
        /// <returns></returns>
        public float GetUnitIntervalScrollPosition()
        {
            var range = _documentSize - _pageSize;
            return (range > 0) ? _position/range : 0.0f;
        }

        /// <summary>
        /// set the current scroll position as a value in the interval [0, 1]
        /// </summary>
        /// <param name="position"></param>
        public void SetUnitIntervalScrollPosition(float position)
        {
            var range = _documentSize - _pageSize;
            SetScrollPosition(range > 0 ? position*range : 0.0f);
        }

        /// <summary>
        /// Return a pointer to the 'increase' PushButtoncomponent widget for this
        /// Scrollbar.
        /// </summary>
        /// <returns>
        /// Pointer to a PushButton object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the increase PushButton component does not exist.
        /// </exception>
        public PushButton GetIncreaseButton()
        {
            return (PushButton) GetChild(IncreaseButtonName);
        }

        /// <summary>
        /// Return a pointer to the 'decrease' PushButton component widget for this
        /// Scrollbar.
        /// </summary>
        /// <returns>
        /// Pointer to a PushButton object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the 'decrease' PushButton component does not exist.
        /// </exception>
        public PushButton GetDecreaseButton()
        {
            return (PushButton) GetChild(DecreaseButtonName);
        }

        /// <summary>
        /// Return a pointer to the Thumb component widget for this Scrollbar.
        /// </summary>
        /// <returns>
        /// Pointer to a Thumb object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the Thumb component does not exist.
        /// </exception>
        public Thumb GetThumb()
        {
            return (Thumb) GetChild(ThumbName);
        }

        /// <summary>
        /// Sets multiple scrollbar configuration parameters simultaneously.
        /// 
        /// This function is provided in order to be able to minimise the number
        /// of internal state updates that occur when modifying the scrollbar
        /// parameters.
        /// </summary>
        /// <param name="documentSize">
        /// Pointer to a float value holding the new value to be used for the
        /// scroll bar document size.  If this is 0 the document size is left
        /// unchanged.
        /// </param>
        /// <param name="pageSize">
        /// Pointer to a float value holding the new value to be used for the scroll
        /// bar page size.  If this is 0 the page size is left unchanged.
        /// </param>
        /// <param name="stepSize">
        /// Pointer to a float value holding the new value to be used for the scroll
        /// bar step size.  If this is 0 the step size is left unchanged.
        /// </param>
        /// <param name="overlapSize">
        /// Pointer to a float value holding the new value to be used for the scroll
        /// bar overlap size.  If this is 0 then overlap size is left unchanged.
        /// </param>
        /// <param name="position">
        /// Pointer to a float value holding the new value to be used for the scroll
        /// bar current scroll position.  If this is 0 then the current position is
        /// left unchanged.
        /// </param>
        /// <remarks>
        /// Even if \a position is 0, the scrollbar position may still change
        /// depending on how the other changes affect the scrollbar.
        /// </remarks>
        public void SetConfig(ref float documentSize,
                              ref float pageSize,
                              ref float stepSize,
                              ref float overlapSize,
                              ref float position)
        {
            var resetMaxPosition = _endLockPosition && IsAtEnd();
            var configChanged = false;
            var positionChanged = false;

            if (Math.Abs(documentSize - 0) > float.Epsilon &&
                (Math.Abs(_documentSize - documentSize) > float.Epsilon))
            {
                _documentSize = documentSize;
                configChanged = true;
            }

            if (Math.Abs(pageSize - 0) > float.Epsilon &&
                (Math.Abs(_pageSize - pageSize) > float.Epsilon))
            {
                _pageSize = pageSize;
                configChanged = true;
            }

            if (Math.Abs(stepSize - 0) > float.Epsilon &&
                (Math.Abs(_stepSize - stepSize) > float.Epsilon))
            {
                _stepSize = stepSize;
                configChanged = true;
            }

            if (Math.Abs(overlapSize - 0) > float.Epsilon &&
                (Math.Abs(_overlapSize - overlapSize) > float.Epsilon))
            {
                _overlapSize = overlapSize;
                configChanged = true;
            }

            if (Math.Abs(position - 0) > float.Epsilon)
                positionChanged = SetScrollPositionImpl(position);
            else if (resetMaxPosition)
                positionChanged = SetScrollPositionImpl(GetMaxScrollPosition());

            // _always_ update the thumb to keep things in sync.  (though this
            // can cause a double-trigger of EventScrollPositionChanged, which
            // also happens with setScrollPosition anyway).
            UpdateThumb();

            //
            // Fire appropriate events based on actions we took.
            //
            if (configChanged)
            {
                OnScrollConfigChanged(new WindowEventArgs(this));
            }

            if (positionChanged)
            {
                OnScrollPositionChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Enable or disable the 'end lock' mode for the scrollbar.
        /// 
        /// When enabled and the current position of the scrollbar is at the end of
        /// it's travel, the end lock mode of the scrollbar will automatically
        /// update the position when the document and/or page size change in order
        /// that the scroll position will remain at the end of it's travel.  This
        /// can be used to implement auto-scrolling in certain other widget types.
        /// </summary>
        /// <param name="enabled">
        /// - true to indicate that end lock mode should be enabled.
        /// - false to indicate that end lock mode should be disabled.
        /// </param>
        public void SetEndLockEnabled(bool enabled)
        {
            _endLockPosition = enabled;
        }

        /// <summary>
        /// Returns whether the 'end lock'mode for the scrollbar is enabled.
        /// 
        /// When enabled, and the current position of the scrollbar is at the end of
        /// it's travel, the end lock mode of the scrollbar will automatically
        /// update the scrollbar position when the document and/or page size change
        /// in order that the scroll position will remain at the end of it's travel.
        /// This can be used to implement auto-scrolling in certain other widget
        /// types.
        /// </summary>
        /// <returns>
        /// - true to indicate that the end lock mode is enabled.
        /// - false to indicate that the end lock mode is disabled.
        /// </returns>
        public bool IsEndLockEnabled()
        {
            return _endLockPosition;
        }

        /// <summary>
        /// move scroll position forwards by the current step size
        /// </summary>
        public void ScrollForwardsByStep()
        {
            SetScrollPosition(_position + _stepSize);
        }

        /// <summary>
        /// move scroll position backwards by the current step size
        /// </summary>
        public void ScrollBackwardsByStep()
        {
            SetScrollPosition(_position - _stepSize);
        }

        /// <summary>
        /// move scroll position forwards by a page (uses appropriate overlap)
        /// </summary>
        public void ScrollForwardsByPage()
        {
            SetScrollPosition(_position + (_pageSize - _overlapSize));
        }

        /// <summary>
        /// move scroll position backwards by a page (uses appropriate overlap)
        /// </summary>
        public void ScrollBackwardsByPage()
        {
            SetScrollPosition(_position - (_pageSize - _overlapSize));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Scrollbar(string type, string name)
            : base(type, name)
        {
            _documentSize = 1.0f;
            _pageSize = 0.0f;
            _stepSize = 1.0f;
            _overlapSize = 0.0f;
            _position = 0.0f;
            _endLockPosition = false;
            AddScrollbarProperties();
        }

        /// <summary>
        /// Returns whether the current scroll position is at the end of the range or not.
        /// </summary>
        /// <returns>
        /// True if the current scroll position is at the end. False if it is not at the end.
        /// </returns>
        protected bool IsAtEnd()
        {
            return _position >= GetMaxScrollPosition();
        }

        /// <summary>
        /// Returns the maximal scroll position value that is allowed, depending
        /// on the document size and page size. 
        /// </summary>
        /// <returns>
        /// The maximal scroll position value that is allowed
        /// </returns>
        protected float GetMaxScrollPosition()
        {
            // max position is (docSize - pageSize)
            // but must be at least 0 (in case doc size is very small)
            return Math.Max((_documentSize - _pageSize), 0.0f);
        }

        protected override void InitialiseComponents()
        {
            // Set up thumb
            var t = GetThumb();
            t.ThumbPositionChanged += HandleThumbMoved;
            t.ThumbTrackStarted += HandleThumbTrackStarted;
            t.ThumbTrackEnded += HandleThumbTrackEnded;

            // set up Increase button
            GetIncreaseButton().CursorPressHold += HandleIncreaseClicked;

            // set up Decrease button
            GetDecreaseButton().CursorPressHold += HandleDecreaseClicked;

            // do initial layout
            PerformChildWindowLayout();
        }

        /// <summary>
        /// update the size and location of the thumb to properly represent the
        /// current state of the scroll bar
        /// </summary>
        protected void UpdateThumb()
        {
            if (d_windowRenderer == null)
                throw new InvalidRequestException(
                    "This function must be implemented by the window renderer object (no window renderer is assigned.)");

            ((ScrollbarWindowRenderer) d_windowRenderer).UpdateThumb();
        }

        /// <summary>
        /// return value that best represents current scroll bar position given the
        /// current location of the thumb.
        /// </summary>
        /// <returns>
        /// float value that, given the thumb widget position, best represents the
        /// current position for the scroll bar.
        /// </returns>
        protected float GetValueFromThumb()
        {
            if (d_windowRenderer == null)
                throw new InvalidRequestException(
                    "This function must be implemented by the window renderer object (no window renderer is assigned.)");

            return ((ScrollbarWindowRenderer) d_windowRenderer).GetValueFromThumb();
        }

        /// <summary>
        /// Given window location \a pt, return a value indicating what change
        /// should be made to the scroll bar.
        /// </summary>
        /// <param name="pt">
        /// Point object describing a pixel position in window space.
        /// </param>
        /// <returns>
        /// - -1 to indicate scroll bar position should be moved to a lower value.
        /// -  0 to indicate scroll bar position should not be changed.
        /// - +1 to indicate scroll bar position should be moved to a higher value.
        /// </returns>
        protected float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt)
        {
            if (d_windowRenderer == null)
                throw new InvalidRequestException(
                    "This function must be implemented by the window renderer object (no window renderer is assigned.)");

            return ((ScrollbarWindowRenderer) d_windowRenderer).GetAdjustDirectionFromPoint(pt);
        }

        /// <summary>
        /// implementation func that updates scroll position value, returns true if
        /// value was changed.  NB: Fires no events and does no other updates.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool SetScrollPositionImpl(float position)
        {
            var oldPos = _position;
            var maxPos = GetMaxScrollPosition();

            // limit position to valid range:  0 <= position <= max_pos
            _position = (position >= 0)
                            ? ((position <= maxPos)
                                   ? position
                                   : maxPos)
                            : 0.0f;

            return Math.Abs(_position - oldPos) > float.Epsilon;
        }

        /// <summary>
        /// handler function for when thumb moves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected /*bool*/ void HandleThumbMoved(object sender, WindowEventArgs e)
        {
            // adjust scroll bar position as required.
            SetScrollPosition(GetValueFromThumb());
            //TODO: return true;
        }

        /// <summary>
        /// handler function for when the increase button is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleIncreaseClicked(EventArgs e)
        {
            if (((CursorInputEventArgs)e).Source != CursorInputSource.Left)
                return false;

            ScrollForwardsByStep();
            return true;
        }

        /// <summary>
        /// handler function for when the decrease button is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleDecreaseClicked(EventArgs e)
        {
            if (((CursorInputEventArgs)e).Source != CursorInputSource.Left)
                return false;

            ScrollBackwardsByStep();
            return true;
        }

        /// <summary>
        /// handler function for when thumb tracking begins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected /*bool*/ void HandleThumbTrackStarted(object sender, WindowEventArgs e)
        {
            // simply trigger our own version of this event
            OnThumbTrackStarted(new WindowEventArgs(this));

            // TODO: return true;
        }

        /// <summary>
        /// handler function for when thumb tracking begins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected /*bool*/ void HandleThumbTrackEnded(object sender, WindowEventArgs e)
        {
            // simply trigger our own version of this event
            OnThumbTrackEnded(new WindowEventArgs(this));
        }

        /// <summary>
        /// validate window renderer
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ScrollbarWindowRenderer) != null;
        }

        // New event handlers for slider widget

        /// <summary>
        /// Handler triggered when the scroll position changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnScrollPositionChanged(WindowEventArgs e)
        {
            FireEvent(EventScrollPositionChanged, e, EventNamespace);
        }

        //! 
        /// <summary>
        /// Handler triggered when the user begins to drag the scroll bar thumb.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackStarted(WindowEventArgs e)
        {
            FireEvent(EventThumbTrackStarted, e, EventNamespace);
        }

        /// <summary>
        /// Handler triggered when the scroll bar thumb is released
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackEnded(WindowEventArgs e)
        {
            FireEvent(EventThumbTrackEnded, e, EventNamespace);
        }

        /// <summary>
        /// Handler triggered when the scroll bar data configuration changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnScrollConfigChanged(WindowEventArgs e)
        {
            PerformChildWindowLayout();
            FireEvent(EventScrollConfigChanged, e, EventNamespace);
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorPressHold(e);

            if (e.Source != CursorInputSource.Left)
                return;

            var adj = GetAdjustDirectionFromPoint(e.Position);

            if (adj > 0)
                ScrollForwardsByPage();
            else if (adj < 0)
                ScrollBackwardsByPage();

            ++e.handled;
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing
            base.OnScroll(e);

            // scroll by e.wheelChange * stepSize
            SetScrollPosition(_position + _stepSize*-e.scroll);

            // ensure the message does not go to our parent.
            ++e.handled;
        }

        protected override void BanPropertiesForAutoWindow()
        {
            base.BanPropertiesForAutoWindow();

            BanPropertyFromXML("DocumentSize");
            BanPropertyFromXML("PageSize");
            BanPropertyFromXML("StepSize");
            BanPropertyFromXML("OverlapSize");
            BanPropertyFromXML("ScrollPosition");
            BanPropertyFromXML("Visible");
        }

        /// <summary>
        /// Adds scrollbar specific properties.
        /// </summary>
        private void AddScrollbarProperties()
        {
            DefineProperty("DocumentSize",
                           "Property to get/set the document size for the Scrollbar.  Value is a float.",
                           (w, v) => w.SetDocumentSize(v), w => w.GetDocumentSize(), 1.0f);

            DefineProperty(
                "PageSize", "Property to get/set the page size for the Scrollbar.  Value is a float.",
                (w, v) => w.SetPageSize(v), w => w.GetPageSize(), 0.0f);

            DefineProperty(
                "StepSize", "Property to get/set the step size for the Scrollbar.  Value is a float.",
                (w, v) => w.SetStepSize(v), w => w.GetStepSize(), 1.0f);

            DefineProperty(
                "OverlapSize", "Property to get/set the overlap size for the Scrollbar.  Value is a float.",
                (w, v) => w.SetOverlapSize(v), w => w.GetOverlapSize(), 0.0f);

            DefineProperty(
                "ScrollPosition", "Property to get/set the scroll position of the Scrollbar.  Value is a float.",
                (w, v) => w.SetScrollPosition(v), w => w.GetScrollPosition(), 0.0f);

            DefineProperty(
                "UnitIntervalScrollPosition",
                "Property to access the scroll position of the Scrollbar as a value in the interval [0, 1].  Value is a float.",
                (w, v) => w.SetUnitIntervalScrollPosition(v), w => w.GetUnitIntervalScrollPosition(), 0.0f);

            DefineProperty(
                "EndLockEnabled",
                "Property to get/set the 'end lock' mode setting for the Scrollbar. Value is either \"True\" or \"False\".",
                (w, v) => w.SetEndLockEnabled(v), w => w.IsEndLockEnabled(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<Scrollbar, T> setter,
                                       Func<Scrollbar, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<Scrollbar, T>(
                            name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// The size of the document / data being scrolled thorugh.
        /// </summary>
        private float _documentSize;

        /// <summary>
        /// The size of a single 'page' of data.
        /// </summary>
        private float _pageSize;

        /// <summary>
        /// Step size used for increase / decrease button clicks.
        /// </summary>
        private float _stepSize;

        /// <summary>
        /// Amount of overlap when jumping by a page.
        /// </summary>
        private float _overlapSize;

        /// <summary>
        /// Current scroll position.
        /// </summary>
        private float _position;

        /// <summary>
        /// whether 'end lock' mode is enabled.
        /// </summary>
        private bool _endLockPosition;

        #endregion
    }
}