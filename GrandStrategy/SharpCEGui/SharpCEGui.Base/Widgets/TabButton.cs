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
    /// Base class for TabButtons.  A TabButton based class is used internally as
    /// the button that appears at the top of a TabControl widget to select the
    /// active tab pane.
    /// </summary>
    public class TabButton : ButtonBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace="TabButton";

        #region Events

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/TabButton";

        /// <summary>
        /// Event fired when the button is clicked.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the TabButton that was clicked.
        /// </summary>
        public event EventHandler<WindowEventArgs> Clicked;
        
        /// <summary>
        /// Event fired when use user attempts to drag the button with middle mouse
        /// button.
        /// Handlers are passed a const MouseEventArgs reference with all fields
        /// valid.
        /// </summary>
        public event EventHandler<CursorInputEventArgs> Dragged;
        
        /// <summary>
        /// Event fired when the scroll wheel is used on top of the button.
        /// Handlers are passed a const MouseEventArgs reference with all fields
        /// valid.
        /// </summary>
        public event EventHandler<CursorInputEventArgs> Scrolled;

        #endregion

        /// <summary>
        /// Constructor for base TabButton class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public TabButton(string type, string name) : base(type, name)
        {
            _selected = false;
            _dragging = false;
        }

        /// <summary>
        /// Set whether this tab button is selected or not
        /// </summary>
        /// <param name="selected"></param>
        public virtual void SetSelected(bool selected)
        {
            _selected = selected;
            Invalidate(false);
        }

        /// <summary>
        /// Return whether this tab button is selected or not
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return _selected;
        }

        /// <summary>
        /// Set the target window which is the content pane which this button is covering.
        /// </summary>
        /// <param name="wnd"></param>
        public void SetTargetWindow(Window wnd)
        {
            _targetWindow = wnd;
            
            // Copy initial text
            SetText(wnd.GetText());

            // Parent control will keep text up to date, since changes affect layout
        }

        /// <summary>
        /// Get the target window which is the content pane which this button is covering.
        /// </summary>
        /// <returns></returns>
        public Window GetTargetWindow()
        {
            return _targetWindow;
        }

        /// <summary>
        /// handler invoked internally when the button is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClicked(WindowEventArgs e)
        {
            FireEvent(Clicked, e);
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            if ((e.Source == CursorInputSource.Left) && IsPushed())
	        {
		        var sheet = GetGUIContext().GetRootWindow();

		        if (sheet!=null)
		        {
			        // if mouse was released over this widget
                    // (use mouse position, as e.position has been unprojected)
			        if (this == sheet.GetTargetChildAtPosition(GetGUIContext().GetCursor().GetPosition()))
			        {
				        // fire event
				        OnClicked(new WindowEventArgs(this));
			        }
		        }

		        ++e.handled;
            }
            else if (e.Source == CursorInputSource.Middle)
            {
                _dragging = false;
                ReleaseInput ();
                ++e.handled;
            }

	        // default handling
            base.OnCursorActivate(e);
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            if (e.Source == CursorInputSource.Middle)
            {
                CaptureInput();
                ++e.handled;
                _dragging = true;

                FireEvent(Dragged, e);
            }

            // default handling
            base.OnCursorPressHold(e);
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            FireEvent(Scrolled, e);

	        // default handling
	        base.OnCursorMove(e);
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            if (_dragging)
            {
                FireEvent(Dragged, e);
                ++e.handled;
            }

	        // default handling
	        base.OnCursorMove(e);
        }

        #region Fields

        /// <summary>
        /// Is this button selected?
        /// </summary>
        private bool _selected;

        /// <summary>
        /// In drag mode or not
        /// </summary>
        private bool _dragging;

        /// <summary>
        /// The target window which this button is representing
        /// </summary>
        private Window _targetWindow;

        #endregion
    }
}