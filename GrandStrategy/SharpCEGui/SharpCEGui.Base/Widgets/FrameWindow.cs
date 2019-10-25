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
    /// Abstract base class for a movable, sizable, window with a title-bar and a frame.
    /// </summary>
    public class FrameWindow : Window
    {
        #region Constants
        
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/FrameWindow";

        /// <summary>
        /// Default size for the sizing border (in pixels)
        /// </summary>
        public const float DefaultSizingBorderSize = 8.0f;

        /// <summary>
        /// Widget name for the titlebar component.
        /// </summary>
        public const string TitlebarName = "__auto_titlebar__";

        /// <summary>
        /// Widget name for the close button component.
        /// </summary>
        public const string CloseButtonName = "__auto_closebutton__";

        #endregion

        #region Events


        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "FrameWindow";

        public const string EventRollupToggled = "RollupToggled";
        public const string EventCloseClicked = "CloseClicked";
        public const string EventDragSizingStarted = "DragSizingStarted";
        public const string EventDragSizingEnded = "DragSizingEnded";

        /// <summary>
        /// Event fired when the rollup (shade) state of the window is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the FrameWindow whose rolled up state
        /// has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> RollupToggled
        {
            add { SubscribeEvent(EventRollupToggled, value); }
            remove { UnsubscribeEvent(EventRollupToggled, value); }
        }

        /// <summary>
        /// Event fired when the close button for the window is clicked.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the FrameWindow whose close button was
        /// clicked.
        /// </summary>
        public event GuiEventHandler<EventArgs> CloseClicked
        {
            add { SubscribeEvent(EventCloseClicked, value); }
            remove { UnsubscribeEvent(EventCloseClicked, value); }
        }

        /// <summary>
        /// Event fired when drag-sizing of the window starts.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the FrameWindow that has started to be
        /// drag sized.
        ///  </summary>
        public event GuiEventHandler<EventArgs> DragSizingStarted
        {
            add { SubscribeEvent(EventDragSizingStarted, value); }
            remove { UnsubscribeEvent(EventDragSizingStarted, value); }
        }

        /// <summary>
        /// Event fired when drag-sizing of the window ends.
        ///  Handlers are passed a const WindowEventArgs reference with
        ///  WindowEventArgs::window set to the FrameWindow for which 
        /// drag sizing has ended.
        /// </summary>
        public event GuiEventHandler<EventArgs> DragSizingEnded
        {
            add { SubscribeEvent(EventDragSizingEnded, value); }
            remove { UnsubscribeEvent(EventDragSizingEnded, value); }
        }

        #endregion

        /// <summary>
        /// Enumeration that defines the set of possible locations for the mouse on a frame windows sizing border. 
        /// </summary>
        public enum SizingLocation
        {
            /// <summary>
            /// Position is not a sizing location.
            /// </summary>
            SizingNone,

            /// <summary>
            /// Position will size from the top-left.
            /// </summary>
            SizingTopLeft,

            /// <summary>
            /// Position will size from the top-right.
            /// </summary>
            SizingTopRight,

            /// <summary>
            /// Position will size from the bottom left.
            /// </summary>
            SizingBottomLeft,

            /// <summary>
            /// Position will size from the bottom right.
            /// </summary>
            SizingBottomRight,

            /// <summary>
            /// Position will size from the top.
            /// </summary>
            SizingTop,

            /// <summary>
            /// Position will size from the left.
            /// </summary>
            SizingLeft,

            /// <summary>
            /// Position will size from the bottom.
            /// </summary>
            SizingBottom,

            /// <summary>
            /// Position will size from the right.
            /// </summary>
            SizingRight
        };

        /// <summary>
        /// Initialises the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.  
        /// Normally this is handled automatically by the WindowFactory for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            // get component windows
            var titlebar = GetTitlebar();
            var closeButton = GetCloseButton();

            // configure titlebar
            titlebar.SetDraggingEnabled(_dragMovable);
            titlebar.SetText(GetText());

            // ban some properties on components, since they are linked to settings
            // defined here.
            titlebar.BanPropertyFromXML("Text");
            titlebar.BanPropertyFromXML("Visible");
            titlebar.BanPropertyFromXML("Disabled");
            closeButton.BanPropertyFromXML("Visible");
            closeButton.BanPropertyFromXML("Disabled");

            // bind handler to close button 'Click' event
            //closeButton->subscribeEvent(PushButton::EventClicked, Event::Subscriber(&CEGUI::FrameWindow::closeClickHandler, this));
            closeButton.Clicked += CloseClickHandler;

            PerformChildWindowLayout();
        }

        /// <summary>
        /// Return whether this window is sizable.  
        /// Note that this requires that the window have an enabled frame and that sizing itself is enabled
        /// </summary>
        /// <returns>
        /// true if the window can be sized, false if the window can not be sized
        /// </returns>
        public bool IsSizingEnabled()
        {
            return _sizingEnabled && IsFrameEnabled();
        }

        /// <summary>
        /// Return whether the frame for this window is enabled.
        /// </summary>
        /// <returns>
        /// true if the frame for this window is enabled, false if the frame for this window is disabled.
        /// </returns>
        public bool IsFrameEnabled()
        {
            return _frameEnabled;
        }

        /// <summary>
        /// Return whether the title bar for this window is enabled.
        /// </summary>
        /// <returns>
        /// true if the window has a title bar and it is enabled, 
        /// false if the window has no title bar or if the title bar is disabled.
        /// </returns>
        public bool IsTitleBarEnabled()
        {
            return !GetTitlebar().IsDisabled();
        }

        /// <summary>
        /// Return whether this close button for this window is enabled.
        /// </summary>
        /// <returns>
        /// true if the window has a close button and it is enabled, 
        /// false if the window either has no close button or if the close button is disabled.
        /// </returns>
        public bool IsCloseButtonEnabled()
        {
            return !GetCloseButton().IsDisabled();
        }

        /// <summary>
        /// Return whether roll up (a.k.a shading) is enabled for this window.
        /// </summary>
        /// <returns>
        /// true if roll up is enabled, false if roll up is disabled.
        /// </returns>
        public bool IsRollupEnabled()
        {
            return _rollupEnabled;
        }

        /// <summary>
        /// Sets whether the window is currently rolled up (a.k.a shaded).
        /// </summary>
        /// <param name="val"></param>
        /// <seealso cref="IsRolledup"/>
        public void SetRolledup(bool val)
        {
            if (val != IsRolledup())
            {
                ToggleRollup();
            }
        }

        /// <summary>
        /// Return whether the window is currently rolled up (a.k.a shaded).
        /// </summary>
        /// <returns>
        /// true if the window is rolled up, false if the window is not rolled up.
        /// </returns>
        public bool IsRolledup()
        {
            return _rolledup;
        }

        /// <summary>
        /// Return the thickness of the sizing border.
        /// </summary>
        /// <returns>
        /// float value describing the thickness of the sizing border in screen pixels.
        /// </returns>
        public float GetSizingBorderThickness()
        {
            return _borderSize;
        }

        /// <summary>
        /// Enables or disables sizing for this window.
        /// </summary>
        /// <param name="setting">
        /// set to true to enable sizing (also requires frame to be enabled), 
        /// or false to disable sizing.
        /// </param>
        public void SetSizingEnabled(bool setting)
        {
            _sizingEnabled = setting;
        }

        /// <summary>
        /// Enables or disables the frame for this window.
        /// </summary>
        /// <param name="setting">
        /// set to true to enable the frame for this window, 
        /// or false to disable the frame for this window.
        /// </param>
        public void SetFrameEnabled(bool setting)
        {
            _frameEnabled = setting;
            Invalidate(false);
        }

        /// <summary>
        /// Enables or disables the title bar for the frame window.
        /// </summary>
        /// <param name="setting">
        /// set to true to enable the title bar (if one is attached), 
        /// or false to disable the title bar.
        /// </param>
        public void SetTitleBarEnabled(bool setting)
        {
            var titlebar = GetTitlebar();
            titlebar.SetEnabled(setting);
            titlebar.SetVisible(setting);
        }

        /// <summary>
        /// Enables or disables the close button for the frame window.
        /// </summary>
        /// <param name="setting">
        /// Set to true to enable the close button (if one is attached), 
        /// or false to disable the close button.
        /// </param>
        public void SetCloseButtonEnabled(bool setting)
        {
            var closebtn = GetCloseButton();
            closebtn.SetEnabled(setting);
            closebtn.SetVisible(setting);
        }

        /// <summary>
        /// Enables or disables roll-up (shading) for this window.
        /// </summary>
        /// <param name="setting">
        /// Set to true to enable roll-up for the frame window, or false to disable roll-up.
        /// </param>
        public void SetRollupEnabled(bool setting)
        {
            if ((setting == false) && IsRolledup())
                ToggleRollup();

            _rollupEnabled = setting;
        }

        /// <summary>
        /// Toggles the state of the window between rolled-up (shaded) and normal sizes.  
        /// This requires roll-up to be enabled.
        /// </summary>
        public void ToggleRollup()
        {
            if (IsRollupEnabled())
            {
                _rolledup ^= true;

                // event notification.
                OnRollupToggled(new WindowEventArgs(this));

                GetGUIContext().UpdateWindowContainingCursor();
            }
        }

        /// <summary>
        /// Set the size of the sizing border for this window.
        /// </summary>
        /// <param name="pixels">
        /// float value specifying the thickness for the sizing border in screen pixels.
        /// </param>
        public void SetSizingBorderThickness(float pixels)
        {
            _borderSize = pixels;
        }

        /// <summary>
        /// Move the window by the pixel offsets specified in \a offset.
        /// This is intended for internal system use - it is the method by 
        /// which the title bar moves the frame window.
        /// </summary>
        /// <param name="offset">
        /// Vector2 object containing the offsets to apply (offsets are in screen pixels).
        /// </param>
        public void OffsetPixelPosition(Lunatics.Mathematics.Vector2 offset)
        {
            var uOffset = new UVector2(UDim.Absolute( /*PixelAligned(*/offset.X /*)*/),
                                       UDim.Absolute( /*PixelAligned(*/offset.Y /*)*/));

            SetPosition(d_area.Position + uOffset);
        }

        /// <summary>
        /// Return whether this FrameWindow can be moved by dragging the title bar.
        /// </summary>
        /// <returns>
        /// true if the Window will move when the user drags the title bar, false if the window will not move.
        /// </returns>
        public bool IsDragMovingEnabled()
        {
            return _dragMovable;
        }

        /// <summary>
        /// Set whether this FrameWindow can be moved by dragging the title bar.
        /// </summary>
        /// <param name="setting">
        /// true if the Window should move when the user drags the title bar, false if the window should not move.
        /// </param>
        public void SetDragMovingEnabled(bool setting)
        {
            if (_dragMovable != setting)
            {
                _dragMovable = setting;

                GetTitlebar().SetDraggingEnabled(setting);
            }
        }

        /// <summary>
        /// Return a pointer to the currently set Image to be used for the north-south
        /// sizing mouse cursor.
        /// </summary>
        /// <returns>
        /// Pointer to an Image object, or 0 for none.
        /// </returns>
        public Image GetNSSizingCursorImage()
        {
            return _nsSizingCursor;
        }

        /// <summary>
        /// Return a pointer to the currently set Image to be used for the east-west
        /// sizing mouse cursor.
        /// </summary>
        /// <returns>
        /// Pointer to an Image object, or 0 for none.
        /// </returns>
        public Image GetEWSizingCursorImage()
        {
            return _ewSizingCursor;
        }

        /// <summary>
        /// Return a pointer to the currently set Image to be used for the northwest-southeast
        /// sizing mouse cursor.
        /// </summary>
        /// <returns>
        /// Pointer to an Image object, or 0 for none.
        /// </returns>
        public Image GetNWSESizingCursorImage()
        {
            return _nwseSizingCursor;
        }

        /// <summary>
        /// Return a pointer to the currently set Image to be used for the northeast-southwest
        /// sizing mouse cursor.
        /// </summary>
        /// <returns>
        /// Pointer to an Image object, or 0 for none.
        /// </returns>
        public Image GetNESWSizingCursorImage()
        {
            return _neswSizingCursor;
        }


        /// <summary>
        /// Set the Image to be used for the north-south sizing mouse cursor.
        /// </summary>
        /// <param name="image">
        /// Pointer to an Image object, or null for none.
        /// </param>
        public void SetNSSizingCursorImage(Image image)
        {
            _nsSizingCursor = image;
        }

        /// <summary>
        /// Set the Image to be used for the east-west sizing mouse cursor.
        /// </summary>
        /// <param name="image">
        /// Pointer to an Image object, or 0 for none.
        /// </param>
        public void SetEWSizingCursorImage(Image image)
        {
            _ewSizingCursor = image;
        }

        /// <summary>
        /// Set the Image to be used for the northwest-southeast sizing mouse cursor.
        /// </summary>
        /// <param name="image">
        /// Pointer to an Image object, or 0 for none.
        /// </param>
        public void SetNWSESizingCursorImage(Image image)
        {
            _nwseSizingCursor = image;
        }

        /// <summary>
        /// Set the Image to be used for the northeast-southwest sizing mouse cursor.
        /// </summary>
        /// <param name="image">
        /// Pointer to an Image object, or 0 for none.
        /// </param>
        public void SetNESWSizingCursorImage(Image image)
        {
            _neswSizingCursor = image;
        }

        /// <summary>
        /// Set the image to be used for the north-south sizing mouse cursor.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Image to be used.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if either \a imageset or \a image refer to non-existant entities.
        /// </exception>
        public void SetNSSizingCursorImage(string name)
        {
            _nsSizingCursor = ImageManager.GetSingleton().Get(name);
        }

        /// <summary>
        /// Set the image to be used for the east-west sizing mouse cursor.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Image to be used.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if either \a imageset or \a image refer to non-existant entities.
        /// </exception>
        public void SetEWSizingCursorImage(string name)
        {
            _ewSizingCursor = ImageManager.GetSingleton().Get(name);
        }

        /// <summary>
        /// Set the image to be used for the northwest-southeast sizing mouse cursor.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Image to be used.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if either \a imageset or \a image refer to non-existant entities.
        /// </exception>
        public void SetNWSESizingCursorImage(string name)
        {
            _nwseSizingCursor = ImageManager.GetSingleton().Get(name);
        }

        /// <summary>
        /// Set the image to be used for the northeast-southwest sizing mouse cursor.
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Image to be used.
        /// </param>
        /// <exception cref="UnknownObjectException">
        /// thrown if either \a imageset or \a image refer to non-existant entities.
        /// </exception>
        public void SetNESWSizingCursorImage(string name)
        {
            _neswSizingCursor = ImageManager.GetSingleton().Get(name);
        }

        // overridden from Window class
        public override bool IsHit(Lunatics.Mathematics.Vector2 position, bool allowDisabled = false)
        {
            return base.IsHit(position, allowDisabled) && !_rolledup;
        }

        /// <summary>
        /// Return a pointer to the Titlebar component widget for this FrameWindow.
        /// </summary>
        /// <returns>
        /// Pointer to a Titlebar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the Titlebar component does not exist.
        /// </exception>
        public Titlebar GetTitlebar()
        {
            return (Titlebar) GetChild(TitlebarName);
        }

        /// <summary>
        /// Return a pointer to the close button component widget for this
        /// FrameWindow.
        /// </summary>
        /// <returns>
        /// Pointer to a PushButton object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the close button component does not exist.
        /// </exception>
        public PushButton GetCloseButton()
        {
            return (PushButton) GetChild(CloseButtonName);
        }


        #region Constructros

        /// <summary>
        /// Constructor for FrameWindow objects.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public FrameWindow(string type, string name)
            : base(type, name)
        {
            _frameEnabled = true;
            _rollupEnabled = true;
            _rolledup = false;
            _sizingEnabled = true;
            _beingSized = false;
            _dragMovable = true;

            _borderSize = DefaultSizingBorderSize;

            _nsSizingCursor = _ewSizingCursor = _neswSizingCursor = _nwseSizingCursor = null;

            AddFrameWindowProperties();
        }

        #endregion

        /// <summary>
        /// move the window's left edge by 'delta'.  
        /// The rest of the window does not move, thus this changes the size of the Window.
        /// </summary>
        /// <param name="delta">
        /// float value that specifies the amount to move the window edge, and in which direction.
        /// Positive values make window smaller.
        /// </param>
        /// <param name="outArea"></param>
        /// <returns></returns>
        protected bool MoveLeftEdge(float delta, ref URect outArea)
        {
            var orgWidth = d_pixelSize.Width;

            // ensure that we only size to the set constraints.
            //
            // NB: We are required to do this here due to our virtually unique sizing nature; the
            // normal system for limiting the window size is unable to supply the information we
            // require for updating our internal state used to manage the dragging, etc.
            float maxWidth = CoordConverter.AsAbsolute(d_maxSize.d_width, GetRootContainerSize().Width);
            float minWidth = CoordConverter.AsAbsolute(d_minSize.d_width, GetRootContainerSize().Width);
            float newWidth = orgWidth - delta;

            if (Math.Abs(maxWidth - 0.0f) > float.Epsilon && newWidth > maxWidth)
                delta = orgWidth - maxWidth;
            else if (newWidth < minWidth)
                delta = orgWidth - minWidth;

            // ensure adjustment will be whole pixel
            float adjustment = /*PixelAligned(*/ delta /*)*/;

            if (d_horizontalAlignment == HorizontalAlignment.Right)
            {
                outArea.d_max.d_x.d_offset -= adjustment;
            }
            else if (d_horizontalAlignment == HorizontalAlignment.Centre)
            {
                outArea.d_max.d_x.d_offset -= adjustment*0.5f;
                outArea.d_min.d_x.d_offset += adjustment*0.5f;
            }
            else
            {
                outArea.d_min.d_x.d_offset += adjustment;
            }

            return d_horizontalAlignment == HorizontalAlignment.Left;
        }

        /// <summary>
        /// move the window's right edge by 'delta'.  
        /// The rest of the window does not move, thus this changes the size of the Window.
        /// </summary>
        /// <param name="delta">
        /// float value that specifies the amount to move the window edge, and in which direction.  
        /// Positive values make window larger.
        /// </param>
        /// <param name="outArea"></param>
        /// <returns></returns>
        protected bool MoveRightEdge(float delta, ref URect outArea)
        {
            // store this so we can work out how much size actually changed
            float orgWidth = d_pixelSize.Width;

            // ensure that we only size to the set constraints.
            //
            // NB: We are required to do this here due to our virtually unique sizing nature; the
            // normal system for limiting the window size is unable to supply the information we
            // require for updating our internal state used to manage the dragging, etc.
            var maxWidth = CoordConverter.AsAbsolute(d_maxSize.d_width, GetRootContainerSize().Width);
            var minWidth = CoordConverter.AsAbsolute(d_minSize.d_width, GetRootContainerSize().Width);
            var newWidth = orgWidth + delta;

            if (Math.Abs(maxWidth - 0.0f) > float.Epsilon && newWidth > maxWidth)
                delta = maxWidth - orgWidth;
            else if (newWidth < minWidth)
                delta = minWidth - orgWidth;

            // ensure adjustment will be whole pixel
            float adjustment = /*PixelAligned(*/ delta /*)*/;

            outArea.d_max.d_x.d_offset += adjustment;

            if (d_horizontalAlignment == HorizontalAlignment.Right)
            {
                outArea.d_max.d_x.d_offset += adjustment;
                outArea.d_min.d_x.d_offset += adjustment;
            }
            else if (d_horizontalAlignment == HorizontalAlignment.Centre)
            {
                outArea.d_max.d_x.d_offset += adjustment*0.5f;
                outArea.d_min.d_x.d_offset += adjustment*0.5f;
            }

            // move the dragging point so mouse remains 'attached' to edge of window
            _dragPoint.X += adjustment;

            return d_horizontalAlignment == HorizontalAlignment.Right;
        }

        /// <summary>
        /// move the window's top edge by 'delta'.  
        /// The rest of the window does not move, thus this changes the size of the Window.
        /// </summary>
        /// <param name="delta">
        /// float value that specifies the amount to move the window edge, and in which direction.
        /// Positive values make window smaller.
        /// </param>
        /// <param name="outArea"></param>
        /// <returns></returns>
        protected bool MoveTopEdge(float delta, ref URect outArea)
        {
            var orgHeight = d_pixelSize.Height;

            // ensure that we only size to the set constraints.
            //
            // NB: We are required to do this here due to our virtually unique sizing nature; the
            // normal system for limiting the window size is unable to supply the information we
            // require for updating our internal state used to manage the dragging, etc.
            float maxHeight = CoordConverter.AsAbsolute(d_maxSize.d_height, GetRootContainerSize().Height);
            float minHeight = CoordConverter.AsAbsolute(d_minSize.d_height, GetRootContainerSize().Height);
            float newHeight = orgHeight - delta;

            if (Math.Abs(maxHeight - 0.0f) > float.Epsilon && newHeight > maxHeight)
                delta = orgHeight - maxHeight;
            else if (newHeight < minHeight)
                delta = orgHeight - minHeight;

            // ensure adjustment will be whole pixel
            float adjustment = /*PixelAligned(*/ delta /*)*/;

            if (d_verticalAlignment == VerticalAlignment.Bottom)
            {
                outArea.d_max.d_y.d_offset -= adjustment;
            }
            else if (d_verticalAlignment == VerticalAlignment.Centre)
            {
                outArea.d_max.d_y.d_offset -= adjustment*0.5f;
                outArea.d_min.d_y.d_offset += adjustment*0.5f;
            }
            else
            {
                outArea.d_min.d_y.d_offset += adjustment;
            }

            return d_verticalAlignment == VerticalAlignment.Top;
        }

        /// <summary>
        /// move the window's bottom edge by 'delta'.  
        /// The rest of the window does not move, thus this changes the size of the Window.
        /// </summary>
        /// <param name="delta">
        /// float value that specifies the amount to move the window edge, and in which direction.  
        /// Positive values make window larger.
        /// </param>
        /// <param name="outArea"></param>
        /// <returns></returns>
        protected bool MoveBottomEdge(float delta, ref URect outArea)
        {
            // store this so we can work out how much size actually changed
            var orgHeight = d_pixelSize.Height;

            // ensure that we only size to the set constraints.
            //
            // NB: We are required to do this here due to our virtually unique sizing nature; the
            // normal system for limiting the window size is unable to supply the information we
            // require for updating our internal state used to manage the dragging, etc.
            var maxHeight = CoordConverter.AsAbsolute(d_maxSize.d_height, GetRootContainerSize().Height);
            var minHeight = CoordConverter.AsAbsolute(d_minSize.d_height, GetRootContainerSize().Height);
            var newHeight = orgHeight + delta;

            if (Math.Abs(maxHeight - 0.0f) > float.Epsilon && newHeight > maxHeight)
                delta = maxHeight - orgHeight;
            else if (newHeight < minHeight)
                delta = minHeight - orgHeight;

            // ensure adjustment will be whole pixel
            var adjustment = /*PixelAligned(*/ delta /*)*/;

            outArea.d_max.d_y.d_offset += adjustment;

            if (d_verticalAlignment == VerticalAlignment.Bottom)
            {
                outArea.d_max.d_y.d_offset += adjustment;
                outArea.d_min.d_y.d_offset += adjustment;
            }
            else if (d_verticalAlignment == VerticalAlignment.Centre)
            {
                outArea.d_max.d_y.d_offset += adjustment*0.5f;
                outArea.d_min.d_y.d_offset += adjustment*0.5f;
            }

            // move the dragging point so mouse remains 'attached' to edge of window
            _dragPoint.Y += adjustment;

            return d_verticalAlignment == VerticalAlignment.Bottom;
        }

        /// <summary>
        /// check local pixel co-ordinate point 'pt' and return one of the
        /// SizingLocation enumerated values depending where the point falls on
        /// the sizing border.
        /// </summary>
        /// <param name="pt">
        /// Point object describing, in pixels, the window relative offset to check.
        /// </param>
        /// <returns>
        /// One of the SizingLocation enumerated values that describe which part of
        /// the sizing border that \a pt corresponded to, if any.
        /// </returns>
        protected SizingLocation GetSizingBorderAtPoint(Lunatics.Mathematics.Vector2 pt)
        {
            var frame = GetSizingRect();

            // we can only size if the frame is enabled and sizing is on
            if (IsSizingEnabled() && IsFrameEnabled())
            {
                // point must be inside the outer edge
                if (frame.IsPointInRect(pt))
                {
                    // adjust rect to get inner edge
                    frame.d_min.X += _borderSize;
                    frame.d_min.Y += _borderSize;
                    frame.d_max.X -= _borderSize;
                    frame.d_max.Y -= _borderSize;

                    // detect which edges we are on
                    var top = (pt.Y < frame.d_min.Y);
                    var bottom = (pt.Y >= frame.d_max.Y);
                    var left = (pt.X < frame.d_min.X);
                    var right = (pt.X >= frame.d_max.X);

                    // return appropriate 'SizingLocation' value
                    if (top && left)
                        return SizingLocation.SizingTopLeft;

                    if (top && right)
                        return SizingLocation.SizingTopRight;

                    if (bottom && left)
                        return SizingLocation.SizingBottomLeft;

                    if (bottom && right)
                        return SizingLocation.SizingBottomRight;

                    if (top)
                        return SizingLocation.SizingTop;

                    if (bottom)
                        return SizingLocation.SizingBottom;

                    if (left)
                        return SizingLocation.SizingLeft;

                    if (right)
                        return SizingLocation.SizingRight;
                }

            }

            // deafult: None.
            return SizingLocation.SizingNone;
        }

        /// <summary>
        /// return true if given SizingLocation is on left edge.
        /// </summary>
        /// <param name="loc">
        /// SizingLocation value to be checked.
        /// </param>
        /// <returns>
        /// true if \a loc is on the left edge.  false if \a loc is not on the left edge.
        /// </returns>
        protected bool IsLeftSizingLocation(SizingLocation loc)
        {
            return ((loc == SizingLocation.SizingLeft) ||
                    (loc == SizingLocation.SizingTopLeft) ||
                    (loc == SizingLocation.SizingBottomLeft));
        }

        /// <summary>
        /// return true if given SizingLocation is on right edge.
        /// </summary>
        /// <param name="loc">
        /// SizingLocation value to be checked.
        /// </param>
        /// <returns>
        /// true if \a loc is on the right edge.  false if \a loc is not on the right edge.
        /// </returns>
        protected bool IsRightSizingLocation(SizingLocation loc)
        {
            return ((loc == SizingLocation.SizingRight) ||
                    (loc == SizingLocation.SizingTopRight) ||
                    (loc == SizingLocation.SizingBottomRight));
        }

        /// <summary>
        /// return true if given SizingLocation is on top edge.
        /// </summary>
        /// <param name="loc">
        /// SizingLocation value to be checked.
        /// </param>
        /// <returns>
        /// true if \a loc is on the top edge.  false if \a loc is not on the top edge.
        /// </returns>
        protected bool IsTopSizingLocation(SizingLocation loc)
        {
            return ((loc == SizingLocation.SizingTop) ||
                    (loc == SizingLocation.SizingTopLeft) ||
                    (loc == SizingLocation.SizingTopRight));
        }

        /// <summary>
        /// return true if given SizingLocation is on bottom edge.
        /// </summary>
        /// <param name="loc">
        /// SizingLocation value to be checked.
        /// </param>
        /// <returns>
        /// true if \a loc is on the bottom edge.  false if \a loc is not on the bottom edge.
        /// </returns>
        protected bool IsBottomSizingLocation(SizingLocation loc)
        {
            return ((loc == SizingLocation.SizingBottom) ||
                    (loc == SizingLocation.SizingBottomLeft) ||
                    (loc == SizingLocation.SizingBottomRight));
        }

        /// <summary>
        /// Method to respond to close button click events and fire our close event
        /// </summary>
        /// <param name="e"></param>
        protected bool CloseClickHandler(EventArgs e)
        {
            OnCloseClicked(new WindowEventArgs(this));
            return true;
        }

        /// <summary>
        /// Set the appropriate mouse cursor for the given window-relative pixel point.
        /// </summary>
        /// <param name="pt"></param>
        protected void SetCursorForPoint(Lunatics.Mathematics.Vector2 pt)
        {
            switch (GetSizingBorderAtPoint(pt))
            {
                case SizingLocation.SizingTop:
                case SizingLocation.SizingBottom:
                    GetGUIContext().GetCursor().SetImage(_nsSizingCursor);
                    break;

                case SizingLocation.SizingLeft:
                case SizingLocation.SizingRight:
                    GetGUIContext().GetCursor().SetImage(_ewSizingCursor);
                    break;

                case SizingLocation.SizingTopLeft:
                case SizingLocation.SizingBottomRight:
                    GetGUIContext().GetCursor().SetImage(_nwseSizingCursor);
                    break;

                case SizingLocation.SizingTopRight:
                case SizingLocation.SizingBottomLeft:
                    GetGUIContext().GetCursor().SetImage(_neswSizingCursor);
                    break;

                default:
                    GetGUIContext().GetCursor().SetImage(GetCursor());
                    break;
            }
        }

        /// <summary>
        /// Return a Rect that describes, in window relative pixel co-ordinates, 
        /// the outer edge of the sizing area for this window.
        /// </summary>
        /// <returns></returns>
        protected virtual Rectf GetSizingRect()
        {
            return new Rectf(0, 0, d_pixelSize.Width, d_pixelSize.Height);
        }

        /// <summary>
        /// Event generated internally whenever the roll-up / shade state of the window
        /// changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRollupToggled(WindowEventArgs e)
        {
            Invalidate(true);
            NotifyScreenAreaChanged();
            OnSized(new ElementEventArgs(e.Window));

            FireEvent(EventRollupToggled, e, EventNamespace);
        }

        /// <summary>
        /// Event generated internally whenever the close button is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCloseClicked(WindowEventArgs e)
        {
            FireEvent(EventCloseClicked, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when drag-sizing of the FrameWindow starts.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragSizingStarted(WindowEventArgs e)
        {
            FireEvent(EventDragSizingStarted, e, EventNamespace);
        }

        /// <summary>
        /// Handler called when drag-sizing of the FrameWindow ends.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragSizingEnded(WindowEventArgs e)
        {
            FireEvent(EventDragSizingEnded, e, EventNamespace);
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // default processing (this is now essential as it controls event firing).
            base.OnCursorMove(e);

            // if we are not the window containing the mouse, do NOT change the cursor
            if (GetGUIContext().GetWindowContainingCursor() != this)
            {
                return;
            }

            if (IsSizingEnabled())
            {
                var localMousePos = CoordConverter.ScreenToWindow(this, e.Position);

                if (_beingSized)
                {
                    var dragEdge = GetSizingBorderAtPoint(_dragPoint);

                    // calculate sizing deltas...
                    var deltaX = localMousePos.X - _dragPoint.X;
                    var deltaY = localMousePos.Y - _dragPoint.Y;

                    var newArea = d_area;
                    var topLeftSizing = false;
                    // size left or right edges
                    if (IsLeftSizingLocation(dragEdge))
                    {
                        topLeftSizing |= MoveLeftEdge(deltaX, ref newArea);
                    }
                    else if (IsRightSizingLocation(dragEdge))
                    {
                        topLeftSizing |= MoveRightEdge(deltaX, ref newArea);
                    }

                    // size top or bottom edges
                    if (IsTopSizingLocation(dragEdge))
                    {
                        topLeftSizing |= MoveTopEdge(deltaY, ref newArea);
                    }
                    else if (IsBottomSizingLocation(dragEdge))
                    {
                        topLeftSizing |= MoveBottomEdge(deltaY, ref newArea);
                    }

                    SetAreaImpl(newArea.d_min, newArea.Size, topLeftSizing);
                }
                else
                {
                    SetCursorForPoint(localMousePos);
                }

            }

            // mark event as handled
            ++e.handled;
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // default processing (this is now essential as it controls event firing).
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                if (IsSizingEnabled())
                {
                    // get position of mouse as co-ordinates local to this window.
                    var localPos = CoordConverter.ScreenToWindow(this, e.Position);

                    // if the mouse is on the sizing border
                    if (GetSizingBorderAtPoint(localPos) != SizingLocation.SizingNone)
                    {
                        // ensure all inputs come to us for now
                        if (CaptureInput())
                        {
                            // setup the 'dragging' state variables
                            _beingSized = true;
                            _dragPoint = localPos;

                            // do drag-sizing started notification
                            OnDragSizingStarted(new WindowEventArgs(this));

                            ++e.handled;
                        }
                    }
                }
            }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // default processing (this is now essential as it controls event firing).
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left && IsCapturedByThis())
            {
                // release our capture on the input data
                ReleaseInput();
                ++e.handled;
            }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // default processing (this is now essential as it controls event firing).
            base.OnCaptureLost(e);

            // reset sizing state
            _beingSized = false;

            // do drag-sizing ended notification
            OnDragSizingEnded(new WindowEventArgs(this));

            ++e.handled;
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            base.OnTextChanged(e);
            // pass this onto titlebar component.
            GetTitlebar().SetText(GetText());
            // maybe the user is using a fontdim for titlebar dimensions ;)
            PerformChildWindowLayout();
        }

        protected override void OnActivated(ActivationEventArgs e)
        {
            base.OnActivated(e);
            GetTitlebar().Invalidate(false);
        }

        protected override void OnDeactivated(ActivationEventArgs e)
        {
            base.OnDeactivated(e);
            GetTitlebar().Invalidate(false);
        }


        private void AddFrameWindowProperties()
        {
            DefineProperty("SizingEnabled",
                           "Property to get/set the state of the sizable setting for the FrameWindow. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetSizingEnabled(v), x => x.IsSizingEnabled(), true);

            DefineProperty("FrameEnabled",
                           "Property to get/set the setting for whether the window frame will be displayed. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetFrameEnabled(v), x => x.IsFrameEnabled(), true);

            // TODO: Inconsistency between Titlebar and TitleBar
            DefineProperty("TitlebarEnabled",
                           "Property to get/set the setting for whether the window title-bar will be enabled (or displayed depending upon choice of final widget type). Value is either \"True\" or \"False\".",
                           (x, v) => x.SetTitleBarEnabled(v), x => x.IsTitleBarEnabled(), true);

            DefineProperty("CloseButtonEnabled",
                           "Property to get/set the setting for whether the window close button will be enabled (or displayed depending upon choice of final widget type). Value is either \"True\" or \"False\".",
                           (x, v) => x.SetCloseButtonEnabled(v), x => x.IsCloseButtonEnabled(), true);

            // TODO: Inconsistency between RollUp and Rollup
            DefineProperty("RollUpEnabled",
                           "Property to get/set the setting for whether the user is able to roll-up / shade the window. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetRollupEnabled(v), x => x.IsRollupEnabled(), true);

            // TODO: Inconsistency
            DefineProperty("RollUpState",
                           "Property to get/set the roll-up / shade state of the window.  Value is either \"True\" or \"False\".",
                           (x, v) => x.SetRolledup(v), x => x.IsRolledup(), false);

            DefineProperty("DragMovingEnabled",
                           "Property to get/set the setting for whether the user may drag the window around by its title bar. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetDragMovingEnabled(v), x => x.IsDragMovingEnabled(), true);

            DefineProperty("SizingBorderThickness",
                           "Property to get/set the setting for the sizing border thickness. Value is a float specifying the border thickness in pixels.",
                           (x, v) => x.SetSizingBorderThickness(v), x => x.GetSizingBorderThickness(), 8.0f);

            DefineProperty("NSSizingCursorImage",
                           "Property to get/set the N-S (up-down) sizing cursor image for the FrameWindow. Value should be \"set:[imageset name] image:[image name]\".",
                           (x, v) => x.SetNSSizingCursorImage(v), x => x.GetNSSizingCursorImage(), null);

            DefineProperty("EWSizingCursorImage",
                           "Property to get/set the E-W (left-right) sizing cursor image for the FrameWindow. Value should be \"set:[imageset name] image:[image name]\".",
                           (x, v) => x.SetEWSizingCursorImage(v), x => x.GetEWSizingCursorImage(), null);

            DefineProperty("NWSESizingCursorImage",
                           "Property to get/set the NW-SE diagonal sizing cursor image for the FrameWindow. Value should be \"set:[imageset name] image:[image name]\".",
                           (x, v) => x.SetNWSESizingCursorImage(v), x => x.GetNWSESizingCursorImage(), null);

            DefineProperty("NESWSizingCursorImage",
                           "Property to get/set the NE-SW diagonal sizing cursor image for the FramwWindow. Value should be \"set:[imageset name] image:[image name]\".",
                           (x, v) => x.SetNESWSizingCursorImage(v), x => x.GetNESWSizingCursorImage(), null);
        }

        private void DefineProperty<T>(string name, string help, Action<FrameWindow, T> setter,
                                       Func<FrameWindow, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<FrameWindow, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        // frame data
        private bool _frameEnabled; //!< true if window frame should be drawn.

        // window roll-up data
        private bool _rollupEnabled; //!< true if roll-up of window is allowed.
        private bool _rolledup; //!< true if window is rolled up.

        // drag-sizing data
        private bool _sizingEnabled; //!< true if sizing is enabled for this window.
        private bool _beingSized; //!< true if window is being sized.
        private float _borderSize; //!< thickness of the sizing border around this window
        private Lunatics.Mathematics.Vector2 _dragPoint; //!< point window is being dragged at.

        // images for cursor when on sizing border
        private Image _nsSizingCursor; //!< North/South sizing cursor image.
        private Image _ewSizingCursor; //!< East/West sizing cursor image.
        private Image _nwseSizingCursor; //!< North-West/South-East cursor image.
        private Image _neswSizingCursor; //!< North-East/South-West cursor image.

        private bool _dragMovable; //!< true if the window will move when dragged by the title bar.

        #endregion
    }
}