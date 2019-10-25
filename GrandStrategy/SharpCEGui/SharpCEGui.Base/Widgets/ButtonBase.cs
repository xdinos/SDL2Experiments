#region Copyright
//////////////////////////////////////////////////////////////////////////
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The CEGuiSharp Development Team
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
//////////////////////////////////////////////////////////////////////////
#endregion

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for all the 'button' type widgets (push button, radio button, check-box, etc)
    /// </summary>
    public abstract class ButtonBase : Window
    {
        /// <summary>
        /// return true if user is hovering over this widget (or it's pushed and user is not over it for highlight)
        /// </summary>
        /// <returns>
        /// true if the user is hovering or if the button is pushed and the mouse is not over the button.
        /// Otherwise return false.
        /// </returns>
        public bool IsHovering()
        {
            return _hovering;
        }

        /// <summary>
        /// Return true if the button widget is in the pushed state.
        /// </summary>
        /// <returns>
        /// true if the button-type widget is pushed, 
        /// false if the widget is not pushed.
        /// </returns>
        public bool IsPushed()
        {
            return _pushed;
        }

        /// <summary>
        /// Internal function to set button's pushed state.  Normally you would
        /// not call this, except perhaps when building compound widgets.
        /// </summary>
        /// <param name="pushed"></param>
        public void SetPushedState(bool pushed)
        {
            _pushed = pushed;

            if (!pushed)
            {
                UpdateInternalState(GetUnprojectedPosition(GetGUIContext().GetCursor().GetPosition()));
            }
            else
            {
                _hovering = true;
            }

            Invalidate(false);
        }

        /// <summary>
        /// Constructor for ButtonBase objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        protected ButtonBase(string type, string name)
            : base(type, name)
        {
            _pushed = false;
            _hovering = false;
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

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // default processing
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                if (CaptureInput())
                {
                    _pushed = true;
                    UpdateInternalState(e.Position);
                    Invalidate(false);
                }

                // event was handled by us.
                ++e.handled;
            }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // default processing
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                ReleaseInput();

                // event was handled by us.
                ++e.handled;
            }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // Default processing
            base.OnCaptureLost(e);

            _pushed = false;
            GetGUIContext().UpdateWindowContainingCursor();
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

        /// <summary>
        /// Update the internal state of the widget with the mouse at the given position.
        /// </summary>
        /// <param name="mousePos">
        /// Point object describing, in screen pixel co-ordinates, the location of the mouse cursor.
        /// </param>
        protected void UpdateInternalState(Lunatics.Mathematics.Vector2 mousePos)
        {
            var oldstate = _hovering;

            _hovering = CalculateCurrentHoverState(mousePos);

            if (oldstate != _hovering)
                Invalidate(false);
        }

        protected bool CalculateCurrentHoverState(Lunatics.Mathematics.Vector2 mousePos)
        {
            var captureWnd = GetCaptureWindow();
            if (captureWnd != null)
            {
                return (captureWnd == this ||
                        (captureWnd.DistributesCapturedInputs() && IsAncestor(captureWnd))) &&
                       IsHit(mousePos);
            }


            return GetGUIContext().GetWindowContainingCursor() == this;
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

        #endregion
    }
}