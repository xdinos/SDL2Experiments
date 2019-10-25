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
    /// Window class intended to be used as a simple, generic Window.
    /// 
    /// This class does no rendering and so appears totally transparent.  This window defaults
    /// to position 0.0f, 0.0f with a size of 1.0f x 1.0f.
    /// <para>
    /// This Window has been used as the root GUI-sheet (root window) but it's usage has been extended
    /// beyond that. That's why it's name has been changed to "DefaultWindow" for 0.8.</para>
    /// </summary>
    public class DefaultWindow : Window
    {
        /// <summary>
        /// The unique typename of this widget
        /// </summary>
        public const string WidgetTypeName = "DefaultWindow";

        /// <summary>
        /// Constructor for DefaultWindows.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public DefaultWindow(string type, string name)
            : base(type, name)
        {
            //SetMaxSize(new USize(UDim.Absolute(Single.MaxValue), UDim.Absolute(Single.MaxValue)));
            SetSize(new USize(UDim.Relative(1.0f), UDim.Relative(1.0f)));
        }

        #region overridden functions from Window base class
        
        protected override bool  MoveToFrontImpl(bool wasClicked)
        {
            var tookAction = base.MoveToFrontImpl(wasClicked);

            if (d_parent==null && _cursorPassThroughEnabled)
                return false;

            return tookAction;
        }
        
        #endregion
        
        #region override the mouse event handlers

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // always call the base class handler
            base.OnCursorMove(e);
            UpdatePointerEventHandled(e);
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // always call the base class handler
            base.OnScroll(e);
            UpdatePointerEventHandled(e);
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // always call the base class handler
            base.OnCursorPressHold(e);
            UpdatePointerEventHandled(e);
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // always call the base class handler
            base.OnCursorActivate(e);
            UpdatePointerEventHandled(e);
        }
        
        #endregion
        
        /// <summary>
        /// helper to update mouse input handled state
        /// </summary>
        /// <param name="e"></param>
        protected void UpdatePointerEventHandled(CursorInputEventArgs e)
        {
            // by default, if we are a root window (no parent) with pass-though enabled
            // we do /not/ mark mouse events as handled.
            if (d_parent==null && e.handled!=0 && _cursorPassThroughEnabled)
                --e.handled;
        }
    }
}