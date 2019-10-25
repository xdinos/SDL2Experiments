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
using System.Linq;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class GUIContext : RenderingSurface, IInputEventReceiver, IDisposable
    {
        #region Events

        /// <summary>
        /// Name of Event fired when the root window is changed to a different
        /// Window.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the @e old root window (the new one is
        /// obtained by calling GUIContext::getRootWindow).
        /// </summary>
        public event EventHandler<WindowEventArgs> RootWindowChanged;

        /// <summary>
        /// Name of Event fired when the RenderTarget for the GUIContext is changed.
        /// Handlers are passed a const GUIContextRenderTargetEventArgs struct, with
        /// the renderTarget member set to the old RenderTarget.
        /// </summary>
        public event EventHandler<GUIContextRenderTargetEventArgs> RenderTargetChanged;

        /// <summary>
        /// Event fired when the default font changes.
        /// Handlers are passed a const reference to a generic EventArgs struct.
        /// </summary>
        public event EventHandler<EventArgs> DefaultFontChanged;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public GUIContext(IRenderTarget target)
            : base(target)
        {
            _rootWindow = null;
            _isDirty = false;
            d_defaultTooltipObject = null;
            d_weCreatedTooltipObject = false;
            d_defaultFont = null;
            d_surfaceSize = target.GetArea().Size;
            d_modalWindow = null;
            d_captureWindow = null;
            
            //d_areaChangedEventConnection(target.subscribeEvent(RenderTarget::EventAreaChanged,Event::Subscriber(&GUIContext::areaChangedHandler, this)));
            target.AreaChanged += AreaChangedHandler;

            //d_windowDestroyedEventConnection(WindowManager::getSingleton().subscribeEvent(WindowManager::EventWindowDestroyed,Event::Subscriber(&GUIContext::windowDestroyedHandler, this)));
            WindowManager.GetSingleton().WindowDestroyed += WindowDestroyedHandler;
            d_windowNavigator = null;

            ResetWindowContainingCursor();
            InitializeSemanticEventHandlers();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            DestroyDefaultTooltipWindowInstance();
            DeleteSemanticEventHandlers();

            if (_rootWindow != null)
                _rootWindow.SetGUIContext(null);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Window GetRootWindow()
        {
            return _rootWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRoot"></param>
        public void SetRootWindow(Window newRoot)
        {
            if (_rootWindow == newRoot)
                return;

            if (_rootWindow != null)
                _rootWindow.SetGUIContext(null);

            var args = new WindowEventArgs(_rootWindow);

            _rootWindow = newRoot;

            if (_rootWindow != null)
            {
                _rootWindow.SetGUIContext(this);
                UpdateRootWindowAreaRects();
				//_rootWindow.SyncTargetSurface();
            }

            OnRootWindowChanged(args);
        }

        /// <summary>
        /// Internal function to directly set the current modal window.
        /// </summary>
        /// <param name="window">
        /// This function is called internally by Window, and should not be called
        /// by client code.  Doing so will likely not give the expected results.
        /// </param>
        public void SetModalWindow(Window window)
        {
            d_modalWindow = window;
        }

        /// <summary>
        /// Return a pointer to the Window that is currently set as modal.
        /// </summary>
        /// <returns></returns>
        public Window GetModalWindow()
        {
            return d_modalWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Window GetWindowContainingCursor()
        {
            if (!d_windowContainingCursorIsUpToDate)
            {
                UpdateWindowContainingCursorImpl();
                d_windowContainingCursorIsUpToDate = true;
            }

            return d_windowContainingCursor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Sizef GetSurfaceSize()
        {
            return d_surfaceSize;
        }

        /// <summary>
        /// call to indicate that some redrawing is required.
        /// </summary>
        public void MarkAsDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            return _isDirty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Cursor GetCursor()
        {
            return _cursor;
        }
        
        /// <summary>
        /// Tell the context to reconsider which window it thinks the mouse is in.
        /// </summary>
        /// <returns></returns>
        public void UpdateWindowContainingCursor()
        {
            d_windowContainingCursorIsUpToDate = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Window GetInputCaptureWindow()
        {
            return d_captureWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public void SetInputCaptureWindow(Window window)
        {
            d_captureWindow = window;
        }

        /// <summary>
        /// Set the default Tooltip object for this GUIContext. This value may be 0
        /// to indicate that no default Tooltip object will be available.
        /// </summary>
        /// <param name="tooltip">
        /// Pointer to a valid Tooltip based object which should be used as the
        /// default tooltip for the GUIContext, or 0 to indicate that no default
        /// Tooltip is required.
        /// </param>
        /// <remarks>
        /// When passing a pointer to a Tooltip object, ownership of the Tooltip
        /// does not pass to the GUIContext.
        /// </remarks>
        public void SetDefaultTooltipObject(Tooltip tooltip)
        {
            DestroyDefaultTooltipWindowInstance();

            d_defaultTooltipObject = tooltip;

            if (d_defaultTooltipObject != null)
                d_defaultTooltipObject.SetWritingXmlAllowed(false);
        }

        /// <summary>
        /// Set the default Tooltip to be used by specifying a Window type.
        /// <para>
        /// The GUIContext will internally attempt to create an instance of the
        /// specified window type (which must be derived from the base Tooltip
        /// class).  If the Tooltip creation fails, the error is logged and no
        /// default Tooltip will be available on the GUIContext.
        /// </para>
        /// </summary>
        /// <param name="tooltipType">
        /// String holding the name of a Tooltip based Window type.
        /// </param>
        public void SetDefaultTooltipType(string tooltipType)
        {
            DestroyDefaultTooltipWindowInstance();
            d_defaultTooltipType = tooltipType;
        }

        /// <summary>
        /// Returns a pointer to the context's default tooltip object.  May return 0.
        /// </summary>
        /// <returns></returns>
        public Tooltip GetDefaultTooltipObject()
        {
            if (d_defaultTooltipObject == null && !String.IsNullOrEmpty(d_defaultTooltipType))
                CreateDefaultTooltipWindowInstance();

            return d_defaultTooltipObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void SetRenderTarget(IRenderTarget target)
        {
            if (d_target == target)
                return;

            var oldTarget = d_target;
            d_target = target;

            //if (_rootWindow != null)
            //    _rootWindow.SyncTargetSurface();

            oldTarget.AreaChanged -= AreaChangedHandler;
            d_target.AreaChanged += AreaChangedHandler;

            AreaChangedHandler(this, new EventArgs());

            OnRenderTargetChanged(new GUIContextRenderTargetEventArgs(this, oldTarget));
        }

        /// <summary>
        /// Set the default font to be used by the GUIContext.
        /// </summary>
        /// <param name="name">
        /// String object containing the name of the font to be used as the
        /// default for this GUIContext
        /// </param>
        public void SetDefaultFont(string name)
        {
            if (String.IsNullOrEmpty(name))
                SetDefaultFont((Font) null);
            else
                SetDefaultFont(FontManager.GetSingleton().Get(name));
        }

        /// <summary>
        /// Set the default font to be used by the GUIContext
        /// </summary>
        /// <param name="font">
        /// Pointer to the font to be used as the default for this GUIContext.
        /// </param>
        public void SetDefaultFont(Font font)
        {
            d_defaultFont = font;

            OnDefaultFontChanged(new EventArgs());
        }

        /// <summary>
        /// Return a pointer to the default Font for the GUIContext
        /// </summary>
        /// <returns>
        /// Pointer to a Font object that is the default for this GUIContext.
        /// </returns>
        public Font GetDefaultFont()
        {
            if (d_defaultFont != null)
                return d_defaultFont;

            // if no explicit default, we will return the first font we can get from
            // the font manager
            return FontManager.GetSingleton().Iterator.FirstOrDefault();
        }

        /// <summary>
        /// Function to inject time pulses into the receiver.
        /// </summary>
        /// <param name="timeElapsed">
        /// float value indicating the amount of time passed, in seconds, since the last time this method was called.
        /// </param>
        /// <returns>
        /// Currently, this method always returns true unless there is no root
        /// window or it is not effectively visible
        /// </returns>
        public bool InjectTimePulse(float timeElapsed)
        {
            // if no visible active sheet, input can't be handled
            if (_rootWindow==null || !_rootWindow.IsEffectiveVisible())
                return false;

            // ensure window containing mouse is now valid
            GetWindowContainingCursor();

            // else pass to sheet for distribution.
            _rootWindow.Update(timeElapsed);

            // this input is then /always/ considered handled.
            return true;
        }

        #region Implementation of IInputEventReceiver

        public bool InjectInputEvent(InputEvent @event)
        {
            if (@event.d_eventType == InputEventType.IET_TextInputEventType)
                return HandleTextInputEvent((TextInputEvent)@event);

            if (@event.d_eventType == InputEventType.IET_SemanticInputEventType)
            {
                var semanticEvent = (SemanticInputEvent)@event;

                if (d_windowNavigator != null)
                    d_windowNavigator.HandleSemanticEvent(semanticEvent);

                return HandleSemanticInputEvent(semanticEvent);
            }

            return false;
        }
        
        #endregion

        #region Implementation of IInjectedInputReceiver

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="deltaX"></param>
        ///// <param name="deltaY"></param>
        ///// <returns></returns>
        //public bool InjectMouseMove(float deltaX, float deltaY)
        //{
        //    var ma = new CursorInputEventArgs(null)
        //             {
        //                     moveDelta = new Lunatics.Mathematics.Vector2(deltaX*_mouseMovementScalingFactor,
        //                                                                 deltaY*_mouseMovementScalingFactor)
        //             };

        //    // no movement means no event
        //    if ((Math.Abs(ma.moveDelta.X - 0) < float.Epsilon) && (Math.Abs(ma.moveDelta.Y- 0) < float.Epsilon))
        //        return false;

        //    _cursor.OffsetPosition(ma.moveDelta);

        //    ma.Position = _cursor.GetPosition();
        //    ma.sysKeys = _systemKeys.Get();
        //    ma.wheelChange = 0;
        //    ma.clickCount = 0;
        //    ma.button = MouseButton.NoButton;

        //    return MouseMoveInjectionImpl(ma);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool InjectMouseLeaves()
        //{
        //    if (GetWindowContainingMouse() == null)
        //        return false;

        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = GetWindowContainingMouse().GetUnprojectedPosition(_cursor.GetPosition()),
        //                     moveDelta = Lunatics.Mathematics.Vector2.Zero,
        //                     button = MouseButton.NoButton,
        //                     sysKeys = _systemKeys.Get(),
        //                     wheelChange = 0,
        //                     window = GetWindowContainingMouse(),
        //                     clickCount = 0
        //                 };

        //    GetWindowContainingMouse().OnMouseLeaves(ma);
        //    ResetWindowContainingMouse();

        //    return ma.handled != 0;
        //}

        //public bool InjectMouseButtonDown(MouseButton button)
        //{
        //    _systemKeys.MouseButtonPressed(button);

        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = _cursor.GetPosition(),
        //                     moveDelta = Lunatics.Mathematics.Vector2.Zero,
        //                     button = button,
        //                     sysKeys = _systemKeys.Get(),
        //                     wheelChange = 0
        //                 };
        //    ma.window = GetTargetWindow(ma.Position, false);
        //    // make mouse position sane for this target window
        //    if (ma.window != null)
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);

        //    //
        //    // Handling for multi-click generation
        //    //
        //    var tkr = d_mouseClickTrackers[(int) button];

        //    tkr.d_click_count++;

        //    // if multi-click requirements are not met
        //    if (((_mouseButtonMultiClickTimeout > 0) && (tkr.d_timer.Elapsed() > _mouseButtonMultiClickTimeout)) ||
        //        (!tkr.d_click_area.IsPointInRect(ma.Position)) ||
        //        (tkr.d_target_window != ma.window) ||
        //        (tkr.d_click_count > 3))
        //    {
        //        // reset to single down event.
        //        tkr.d_click_count = 1;

        //        // build new allowable area for multi-clicks
        //        tkr.d_click_area.Position = ma.Position;
        //        tkr.d_click_area.Size = _mouseButtonMultiClickTolerance;
        //        tkr.d_click_area.Offset(new Lunatics.Mathematics.Vector2(-(_mouseButtonMultiClickTolerance.d_width/2),
        //                                                                -(_mouseButtonMultiClickTolerance.d_height/2)));

        //        // set target window for click events on this tracker
        //        tkr.d_target_window = ma.window;
        //    }

        //    // set click count in the event args
        //    ma.clickCount = tkr.d_click_count;

        //    if (ma.window != null)
        //    {
        //        if (_generateMouseClickEvents && ma.window.WantsMultiClickEvents())
        //        {
        //            switch (tkr.d_click_count)
        //            {
        //                case 1:
        //                    ma.window.InjectMouseButtonDown(ma);
        //                    break;

        //                case 2:
        //                    ma.window.OnMouseDoubleClicked(ma);
        //                    break;

        //                case 3:
        //                    ma.window.OnMouseTripleClicked(ma);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            // click generation disabled, or current target window does not want
        //            // multi-clicks, so just send a mouse down event instead.

        //            ma.window.InjectMouseButtonDown(ma);
        //        }
        //    }

        //    // reset timer for this tracker.
        //    tkr.d_timer.Restart();
            
        //    return ma.handled != 0;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="button"></param>
        ///// <returns></returns>
        //public bool InjectMouseButtonUp(MouseButton button)
        //{
        //    _systemKeys.MouseButtonReleased(button);

        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = _cursor.GetPosition(),
        //                     moveDelta = Lunatics.Mathematics.Vector2.Zero,
        //                     button = button,
        //                     sysKeys = _systemKeys.Get(),
        //                     wheelChange = 0,
        //                 };
        //    ma.window = GetTargetWindow(ma.Position, false);
        //    // make mouse position sane for this target window
        //    if (ma.window != null)
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);

        //    // get the tracker that holds the number of down events seen so far for this button
        //    var tkr = d_mouseClickTrackers[(int) button];
        //    // set click count in the event args
        //    ma.clickCount = tkr.d_click_count;

        //    // if there is no window, inputs can not be handled.
        //    if (ma.window == null)
        //        return false;

        //    // store original window becase we re-use the event args.
        //    var targetWnd = ma.window;

        //    // send 'up' input to the window
        //    ma.window.InjectMouseButtonUp(ma);
        //    // store whether the 'up' part was handled so we may reuse the EventArgs
        //    var upHandled = ma.handled;

        //    // restore target window (because Window::on* may have propagated input)
        //    ma.window = targetWnd;

        //    // send MouseClicked event if the requirements for that were met
        //    if (_generateMouseClickEvents &&
        //        ((Math.Abs(_mouseButtonClickTimeout - 0f) < float.Epsilon) ||
        //         (tkr.d_timer.Elapsed() <= _mouseButtonClickTimeout)) &&
        //        (tkr.d_click_area.IsPointInRect(ma.Position)) &&
        //        (tkr.d_target_window == ma.window))
        //    {
        //        ma.handled = 0;
        //        ma.window.OnMouseClicked(ma);
        //    }

        //    return (ma.handled + upHandled) != 0;
        //}

        //public bool InjectKeyDown(Key.Scan scanCode)
        //{
        //    _systemKeys.KeyPressed(scanCode);

        //    var args = new KeyEventArgs(GetKeyboardTargetWindow());

        //    // if there's no destination window, input can't be handled.
        //    if (args.window == null)
        //        return false;

        //    args.scancode = scanCode;
        //    args.sysKeys = _systemKeys.Get();

        //    args.window.OnKeyDown(args);
        //    return args.handled != 0;
        //}

        //public bool InjectKeyUp(Key.Scan scanCode)
        //{
        //    _systemKeys.KeyReleased(scanCode);

        //    var args=new KeyEventArgs(GetKeyboardTargetWindow());

        //    // if there's no destination window, input can't be handled.
        //    if (args.window==null)
        //        return false;

        //    args.scancode = scanCode;
        //    args.sysKeys = _systemKeys.Get();

        //    args.window.OnKeyUp(args);
        //    return args.handled != 0;
        //}

        //public bool InjectChar(char codePoint)
        //{
        //    var args=new KeyEventArgs(GetKeyboardTargetWindow());

        //    // if there's no destination window, input can't be handled.
        //    if (args.window == null)
        //        return false;

        //    args.codepoint = codePoint;
        //    args.sysKeys = _systemKeys.Get();

        //    args.window.OnCharacter(args);
        //    return args.handled != 0;
        //}
        
        //public bool InjectMouseWheelChange(float delta)
        //{
        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = _cursor.GetPosition(),
        //                     moveDelta = Lunatics.Mathematics.Vector2.Zero,
        //                     button = MouseButton.NoButton,
        //                     sysKeys = _systemKeys.Get(),
        //                     wheelChange = delta,
        //                     clickCount = 0
        //                 };
        //    ma.window = GetTargetWindow(ma.Position, false);
        //    // make mouse position sane for this target window
        //    if (ma.window != null)
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);

        //    // if there is no target window, input can not be handled.
        //    if (ma.window == null)
        //        return false;

        //    ma.window.OnMouseWheel(ma);
        //    return ma.handled != 0;
        //}

        //public bool InjectMousePosition(float xPos, float yPos)
        //{
        //    var newPosition = new Lunatics.Mathematics.Vector2(xPos, yPos);

        //    // setup mouse movement event args object.
        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     moveDelta = newPosition - _cursor.GetPosition()
        //                 };

        //    // no movement means no event
        //    if ((Math.Abs(ma.moveDelta.X - 0) < float.Epsilon) &&
        //        (Math.Abs(ma.moveDelta.Y- 0) < float.Epsilon))
        //    {
        //        return false;
        //    }

        //    ma.sysKeys = _systemKeys.Get();
        //    ma.wheelChange = 0;
        //    ma.clickCount = 0;
        //    ma.button = MouseButton.NoButton;

        //    // move mouse cursor to new position
        //    _cursor.SetPosition(newPosition);
        //    // update position in args (since actual position may be constrained)
        //    ma.Position = _cursor.GetPosition();

        //    return MouseMoveInjectionImpl(ma);
        //}

        //public bool InjectMouseButtonClick(MouseButton button)
        //{
        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = _cursor.GetPosition()
        //                 };
        //    ma.window = GetTargetWindow(ma.Position, false);

        //    if (ma.window != null)
        //    {
        //        // initialise remainder of args struct.
        //        ma.moveDelta = Lunatics.Mathematics.Vector2.Zero;
        //        ma.button = button;
        //        ma.sysKeys = _systemKeys.Get();
        //        ma.wheelChange = 0;
        //        // make mouse position sane for this target window
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);
        //        // tell the window about the event.
        //        ma.window.OnMouseClicked(ma);
        //    }

        //    return ma.handled != 0;
        //}

        //public bool InjectMouseButtonDoubleClick(MouseButton button)
        //{
        //    var ma = new CursorInputEventArgs(null)
        //               {
        //                   Position = _cursor.GetPosition()
        //               };
        //    ma.window = GetTargetWindow(ma.Position, false);

        //    if (ma.window!=null && ma.window.WantsMultiClickEvents())
        //    {
        //        // initialise remainder of args struct.
        //        ma.moveDelta = Lunatics.Mathematics.Vector2.Zero;
        //        ma.button = button;
        //        ma.sysKeys = _systemKeys.Get();
        //        ma.wheelChange = 0;
        //        // make mouse position sane for this target window
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);
        //        // tell the window about the event.
        //        ma.window.OnMouseDoubleClicked(ma);
        //    }

        //    return ma.handled != 0;
        //}

        //public bool InjectMouseButtonTripleClick(MouseButton button)
        //{
        //    var ma = new CursorInputEventArgs(null)
        //                 {
        //                     Position = _cursor.GetPosition()
        //                 };
        //    ma.window = GetTargetWindow(ma.Position, false);

        //    if (ma.window != null && ma.window.WantsMultiClickEvents())
        //    {
        //        // initialise remainder of args struct.
        //        ma.moveDelta = Lunatics.Mathematics.Vector2.Zero;
        //        ma.button = button;
        //        ma.sysKeys = _systemKeys.Get();
        //        ma.wheelChange = 0;
        //        // make mouse position sane for this target window
        //        ma.Position = ma.window.GetUnprojectedPosition(ma.Position);
        //        // tell the window about the event.
        //        ma.window.OnMouseTripleClicked(ma);
        //    }

        //    return ma.handled != 0;
        //}

        //public bool InjectCopyRequest()
        //{
        //    var source = GetKeyboardTargetWindow();
        //    return source != null && source.PerformCopy(System.GetSingleton().GetClipboard());
        //}

        //public bool InjectCutRequest()
        //{
        //    var source = GetKeyboardTargetWindow();
        //    return source != null && source.PerformCut(System.GetSingleton().GetClipboard());
        //}

        //public bool InjectPasteRequest()
        //{
        //    var source = GetKeyboardTargetWindow();
        //    return source != null && source.PerformPaste(System.GetSingleton().GetClipboard());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool InjectUndoRequest()
        //{
        //    var target = GetKeyboardTargetWindow();
        //    return target!=null && target.PerformUndo();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool InjectRedoRequest()
        //{
        //    var target = GetKeyboardTargetWindow();
        //    return target != null && target.PerformRedo();
        //}

        #endregion

        public override void Draw()
        {
            if (_isDirty)
                DrawWindowContentToTarget();

            base.Draw();
        }

        /// <summary>
        /// Sets a window navigator to be used for navigating in this context
        /// </summary>
        /// <param name="navigator"></param>
        public void SetWindowNavigator(WindowNavigator navigator)
        {
            d_windowNavigator = navigator;
        }

        protected void UpdateRootWindowAreaRects()
        {
            _rootWindow.OnParentSized(new ElementEventArgs(null));
        }

        protected void DrawWindowContentToTarget()
        {
            if (_rootWindow != null)
                RenderWindowHierarchyToSurfaces();
            else
                ClearGeometry();

            _isDirty = false;
        }

        protected void RenderWindowHierarchyToSurfaces()
        {
            var rs = _rootWindow.GetTargetRenderingSurface();
            rs.ClearGeometry();

            if (rs.IsRenderingWindow())
                ((RenderingWindow) rs).GetOwner().ClearGeometry();

            _rootWindow.Draw();
        }

        protected void CreateDefaultTooltipWindowInstance()
        {
            var winmgr = WindowManager.GetSingleton();

            if (winmgr.IsLocked())
                return;

            d_defaultTooltipObject = winmgr.CreateWindow(d_defaultTooltipType, "CEGUI::System::default__auto_tooltip__") as Tooltip;

            if (d_defaultTooltipObject!=null)
            {
                d_defaultTooltipObject.SetAutoWindow(true);
                d_defaultTooltipObject.SetWritingXmlAllowed(false);
                d_weCreatedTooltipObject = true;
            }
        }

        protected void DestroyDefaultTooltipWindowInstance()
        {
            if (d_defaultTooltipObject!=null && d_weCreatedTooltipObject)
            {
                WindowManager.GetSingleton().DestroyWindow(d_defaultTooltipObject);
                d_defaultTooltipObject = null;
            }

            d_weCreatedTooltipObject = false;
        }

        /// <summary>
        /// notify windows in a hierarchy using default font, when font changes.
        /// </summary>
        /// <param name="hierarchyRoot"></param>
        protected void NotifyDefaultFontChanged(Window hierarchyRoot)
        {
            var evtArgs = new WindowEventArgs(hierarchyRoot);

            if (hierarchyRoot.GetFont(false) == null)
                hierarchyRoot.OnFontChanged(evtArgs);

            for (var i = 0; i < hierarchyRoot.GetChildCount(); ++i)
                NotifyDefaultFontChanged(hierarchyRoot.GetChildAtIdx(i));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="allowDisabled"></param>
        /// <returns></returns>
        protected Window GetTargetWindow(Lunatics.Mathematics.Vector2 pt, bool allowDisabled)
        {
            // if there is no GUI sheet visible, then there is nowhere to send input
            if (_rootWindow == null || !_rootWindow.IsEffectiveVisible())
                return null;

            var destWindow = d_captureWindow;

            if (destWindow == null)
            {
                destWindow = _rootWindow.GetTargetChildAtPosition(pt, allowDisabled) ?? _rootWindow;
            }
            else
            {
                if (destWindow.DistributesCapturedInputs())
                {
                    var childWindow = destWindow.GetTargetChildAtPosition(pt, allowDisabled);

                    if (childWindow != null)
                        destWindow = childWindow;
                }
            }

            // modal target overrules
            if (d_modalWindow != null && destWindow != d_modalWindow)
                if (!destWindow.IsAncestor(d_modalWindow))
                    destWindow = d_modalWindow;

            return destWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Window GetInputTargetWindow()
        {
            // if no active sheet, there is no target widow.
            if (_rootWindow == null || !_rootWindow.IsEffectiveVisible())
                return null;

            // handle normal non-modal situations
            if (d_modalWindow == null)
                return _rootWindow.GetActiveChild();

            // handle possible modal window.
            var target = d_modalWindow.GetActiveChild();
            return target ?? d_modalWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        protected Window GetCommonAncestor(Window w1, Window w2)
        {
            if (w2 == null)
                return null;

            if (w1 == w2)
                return w1;

            // make sure w1 is always further up
            if (w1 != null && w1.IsAncestor(w2))
                return w2;

            while (w1 != null)
            {
                if (w2.IsAncestor(w1))
                    break;

                w1 = w1.GetParent();
            }

            return w1;
        }

        /// <summary>
        /// call some function for a chain of windows: (top, bottom]
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="func"></param>
        /// <param name="args"></param>
        protected void NotifyCursorTransition(Window top, Window bottom, Action<Window, CursorInputEventArgs> func, CursorInputEventArgs args)
        {
            if (top == bottom)
                return;

            var parent = bottom.GetParent();

            if (parent != null && parent != top)
                NotifyCursorTransition(top, parent, func, args);

            args.handled = 0;
            args.Window = bottom;

            func(bottom, args);
        }

        protected void/*bool*/ AreaChangedHandler(object sender, EventArgs args)
        {
            d_surfaceSize = d_target.GetArea().Size;
            _cursor.NotifyDisplaySizeChanged(d_surfaceSize);

            if (_rootWindow!=null)
                UpdateRootWindowAreaRects();

            //return true;
        }

        protected void/*bool*/ WindowDestroyedHandler(object sender, WindowEventArgs args)
        {
            var window = args.Window;

            if (window == _rootWindow)
                _rootWindow = null;

            if (window == GetWindowContainingCursor())
                ResetWindowContainingCursor();

            if (window == d_modalWindow)
                d_modalWindow = null;

            if (window == d_captureWindow)
                d_captureWindow = null;

            if (window == d_defaultTooltipObject)
            {
                d_defaultTooltipObject = null;
                d_weCreatedTooltipObject = false;
            }

            // TODO: return true;
        }

        /// <summary>
        /// returns whether the window containing the mouse had changed.
        /// </summary>
        /// <returns></returns>
        protected bool UpdateWindowContainingCursorImpl()
        {
            var ciea=new CursorInputEventArgs(null);
            var cursor_pos = _cursor.GetPosition();

            var window_with_cursor = GetTargetWindow(cursor_pos, true);

            // exit if window containing cursor has not changed.
            if (window_with_cursor == d_windowContainingCursor)
                return false;

            ciea.scroll = 0;
            ciea.Source = CursorInputSource.None;

            var oldWindow = d_windowContainingCursor;
            d_windowContainingCursor = window_with_cursor;

            // inform previous window the cursor has left it
            if (oldWindow!=null)
            {
                ciea.Window = oldWindow;
                ciea.Position = oldWindow.GetUnprojectedPosition(cursor_pos);
                oldWindow.OnCursorLeaves(ciea);
            }

            // inform window containing cursor that cursor has entered it
            if (d_windowContainingCursor!=null)
            {
                ciea.handled = 0;
                ciea.Window = d_windowContainingCursor;
                ciea.Position = d_windowContainingCursor.GetUnprojectedPosition(cursor_pos);
                d_windowContainingCursor.OnCursorEnters(ciea);
            }

            // do the 'area' version of the events
            var root = GetCommonAncestor(oldWindow, d_windowContainingCursor);

            if (oldWindow != null)
                NotifyCursorTransition(root, oldWindow, (w, a) => w.OnCursorLeavesArea(a), ciea);

            if (d_windowContainingCursor!=null)
                NotifyCursorTransition(root, d_windowContainingCursor, (w, a) => w.OnCursorEntersArea(a), ciea);

            return true;
        }

        protected void ResetWindowContainingCursor()
        {
            d_windowContainingCursor = null;
            d_windowContainingCursorIsUpToDate = true;
        }

        // event trigger functions.
        protected virtual void OnRootWindowChanged(WindowEventArgs args)
        {
            if (_rootWindow != null)
                UpdateRootWindowAreaRects();

            MarkAsDirty();

            var handler = RootWindowChanged;
            if (handler != null)
                handler(this, args);
        }
        
        protected virtual void OnRenderTargetChanged(GUIContextRenderTargetEventArgs args)
        {
            var handler = RenderTargetChanged;
            if (handler != null)
                handler(this, args);
        }

        protected virtual void OnDefaultFontChanged(EventArgs args)
        {
            if (_rootWindow != null)
                NotifyDefaultFontChanged(_rootWindow);

            var handler = DefaultFontChanged;
            if (handler != null)
                handler(this, args);
        }

        // protected overrides
        protected override void DrawContent()
        {
            base.DrawContent();

            _cursor.Draw();
        }

        private bool handleCopyRequest(SemanticInputEvent @event)
        {
            var target = GetInputTargetWindow();
            return target!=null && target.PerformCopy(System.GetSingleton().GetClipboard());
        }

        private bool handleCutRequest(SemanticInputEvent @event)
        {
            var target = GetInputTargetWindow();
            return target!=null && target.PerformCut(System.GetSingleton().GetClipboard());
        }

        private bool handlePasteRequest(SemanticInputEvent @event)
        {
            var target = GetInputTargetWindow();
            return target!=null && target.PerformPaste(System.GetSingleton().GetClipboard());
        }

        private bool handleUndoRequest(SemanticInputEvent @event)
        {
            var target = GetInputTargetWindow();
            return target!=null && target.PerformUndo();
        }

        private bool handleRedoRequest(SemanticInputEvent @event)
        {
            var target = GetInputTargetWindow();
            return target!=null && target.PerformRedo();
        }

        private bool HandleTextInputEvent(TextInputEvent @event)
        {
            var args = new TextEventArgs(GetInputTargetWindow());

            // if there's no destination window, input can't be handled.
            if (args.Window == null)
                return false;

            args.d_character = @event.d_character;

            args.Window.OnCharacter(args);
            return args.handled != 0;
        }

        private bool HandleSemanticInputEvent(SemanticInputEvent @event)
        {
            if (d_semanticEventHandlers.ContainsKey(@event.d_value))
                return d_semanticEventHandlers[@event.d_value](@event);
            
            var targetWindow = GetInputTargetWindow();
            // window navigator's window takes precedence
            if (d_windowNavigator != null)
                targetWindow = d_windowNavigator.GetCurrentFocusedWindow();

            if (targetWindow != null)
            {
                var args = new SemanticEventArgs(targetWindow);

                args.d_payload = @event.d_payload;
                args.d_semanticValue = (SemanticValue) @event.d_value;

                args.Window.OnSemanticInputEvent(args);

                return args.handled != 0;
            }

            return false;
        }

        private void InitializeSemanticEventHandlers()
        {
            d_semanticEventHandlers.Add((int) SemanticValue.SV_Undo, e => handleUndoRequest(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_Redo, e => handleRedoRequest(e as SemanticInputEvent));

            d_semanticEventHandlers.Add((int) SemanticValue.SV_Cut, e => handleCutRequest(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_Copy, e => handleCopyRequest(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_Paste, e => handlePasteRequest(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_VerticalScroll, e => handleScrollEvent(e as SemanticInputEvent));

            d_semanticEventHandlers.Add((int) SemanticValue.SV_CursorActivate, e => handleCursorActivateEvent(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_CursorPressHold, e => handleCursorPressHoldEvent(e as SemanticInputEvent));
            d_semanticEventHandlers.Add((int) SemanticValue.SV_CursorMove, e => handleCursorMoveEvent(e as SemanticInputEvent));
        }

        private void DeleteSemanticEventHandlers()
        {
            //for (std::map<int, SlotFunctorBase<InputEvent>*>::iterator itor = d_semanticEventHandlers.begin();
            //    itor != d_semanticEventHandlers.end(); ++itor)
            //{
            //    delete itor->second;
            //}
        }

        private bool handleCursorActivateEvent(SemanticInputEvent @event)
        {
            var ciea = new CursorInputEventArgs(null);
            ciea.Position = _cursor.GetPosition();
            ciea.moveDelta = Lunatics.Mathematics.Vector2.Zero;
            ciea.Source = @event.d_payload.source;
            ciea.scroll = 0;
            ciea.Window = GetTargetWindow(ciea.Position, false);
            // make cursor position sane for this target window
            if (ciea.Window != null)
                ciea.Position = ciea.Window.GetUnprojectedPosition(ciea.Position);

            // if there is no target window, input can not be handled.
            if (ciea.Window == null)
                return false;

            if (d_windowNavigator != null)
                d_windowNavigator.SetCurrentFocusedWindow(ciea.Window);

            ciea.Window.OnCursorActivate(ciea);
            return ciea.handled != 0;
        }

        private bool handleCursorPressHoldEvent(SemanticInputEvent @event)
        {
            var ciea = new CursorInputEventArgs(null);
            ciea.Position = _cursor.GetPosition();
            ciea.moveDelta = Lunatics.Mathematics.Vector2.Zero;
            ciea.Source = @event.d_payload.source;
            ciea.scroll = 0;
            ciea.Window = GetTargetWindow(ciea.Position, false);
            // make cursor position sane for this target window
            if (ciea.Window != null)
                ciea.Position = ciea.Window.GetUnprojectedPosition(ciea.Position);

            if (d_windowNavigator != null)
                d_windowNavigator.SetCurrentFocusedWindow(ciea.Window);

            ciea.Window.OnCursorPressHold(ciea);
            return ciea.handled != 0;
        }

        private bool handleScrollEvent(SemanticInputEvent @event)
        {
            CursorInputEventArgs ciea = new CursorInputEventArgs(null);
            ciea.Position = _cursor.GetPosition();
            ciea.moveDelta = Lunatics.Mathematics.Vector2.Zero;
            ciea.Source = CursorInputSource.None;
            ciea.scroll = @event.d_payload.single;
            ciea.Window = GetTargetWindow(ciea.Position, false);
            // make cursor position sane for this target window
            if (ciea.Window != null)
                ciea.Position = ciea.Window.GetUnprojectedPosition(ciea.Position);

            // if there is no target window, input can not be handled.
            if (ciea.Window == null)
                return false;

            ciea.Window.OnScroll(ciea);
            return ciea.handled != 0;
        }

        private bool HandleCursorMoveImpl(CursorInputEventArgs pa)
    {
        UpdateWindowContainingCursor();

        // input can't be handled if there is no window to handle it.
        if (GetWindowContainingCursor()==null)
            return false;

        // make cursor position sane for this target window
        pa.Position = GetWindowContainingCursor().GetUnprojectedPosition(pa.Position);
        // inform window about the input.
        pa.Window = GetWindowContainingCursor();
        pa.handled = 0;
        pa.Window.OnCursorMove(pa);

        // return whether window handled the input.
        return pa.handled != 0;
    }

        private bool handleCursorMoveEvent(SemanticInputEvent @event)
        {
            var new_position= new Lunatics.Mathematics.Vector2(
                @event.d_payload.array[0],
                @event.d_payload.array[1]);

            // setup cursor movement event args object.
            var ciea=new CursorInputEventArgs(null);
            ciea.moveDelta = new_position - _cursor.GetPosition();

            // no movement means no event
            if ((ciea.moveDelta.X == 0) && (ciea.moveDelta.Y == 0))
                return false;

            ciea.scroll = 0;
            ciea.Source = CursorInputSource.None;
            ciea.state = d_cursorsState;

            // move cursor to new position
            _cursor.SetPosition(new_position);
            // update position in args (since actual position may be constrained)
            ciea.Position= _cursor.GetPosition();

            return HandleCursorMoveImpl(ciea);
        }
    
        private bool handleCursorLeave(SemanticInputEvent @event)
        {
            if (GetWindowContainingCursor()==null)
                return false;

            var ciea = new CursorInputEventArgs(null);
            ciea.Position = GetWindowContainingCursor().GetUnprojectedPosition(_cursor.GetPosition());
            ciea.moveDelta = Lunatics.Mathematics.Vector2.Zero;
            ciea.Source = CursorInputSource.None;
            ciea.scroll = 0;
            ciea.Window = GetWindowContainingCursor();

            GetWindowContainingCursor().OnCursorLeaves(ciea);
            ResetWindowContainingCursor();

            return ciea.handled != 0;
        }

        #region Fields

        private Window _rootWindow;

        private bool _isDirty;

        private readonly Cursor _cursor = new Cursor();

        protected Tooltip d_defaultTooltipObject;
        protected bool d_weCreatedTooltipObject;
        protected string d_defaultTooltipType;

        protected Font d_defaultFont;

        /// <summary>
        /// a cache of the target surface size, allows returning by ref.
        /// </summary>
        protected Sizef d_surfaceSize;

        protected /*mutable*/ Window d_windowContainingCursor;
        protected /*mutable*/ bool d_windowContainingCursorIsUpToDate;

        protected Window d_modalWindow;
        protected Window d_captureWindow;

        protected CursorsState d_cursorsState = new CursorsState();

        //Event::ScopedConnection d_areaChangedEventConnection;
        //Event::ScopedConnection d_windowDestroyedEventConnection;
        private readonly Dictionary<int, Func<InputEvent, bool>> d_semanticEventHandlers = new Dictionary<int, Func<InputEvent, bool>>();

        /// <summary>
        /// the window navigator (if any) used to navigate the GUI
        /// </summary>
        protected WindowNavigator d_windowNavigator;
        
        #endregion
    }
}