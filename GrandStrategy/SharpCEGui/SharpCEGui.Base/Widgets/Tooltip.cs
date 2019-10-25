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
    /// Base class for Tooltip widgets.
    /// <para>
    /// The Tooltip class shows a simple pop-up window around the mouse position
    /// with some text information.  The tool-tip fades in when the user hovers
    /// with the mouse over a window which has tool-tip text set, and then fades
    /// out after some pre-set time.
    /// </para>
    /// <remarks>
    /// For Tooltip to work properly, you must specify a default tool-tip widget
    /// type via System::setTooltip, or by setting a custom tool-tip object for
    /// your Window(s).  Additionally, you need to ensure that time pulses are
    /// properly passed to the system via System::injectTimePulse.
    /// </remarks>
    /// </summary>
    public class Tooltip : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Tooltip";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Tooltip";

        public const string EventHoverTimeChanged = "HoverTimeChanged";
        public const string EventDisplayTimeChanged = "DisplayTimeChanged";
        public const string EventFadeTimeChanged = "FadeTimeChanged";
        public const string EventTooltipActive = "TooltipActive";
        public const string EventTooltipInactive = "TooltipInactive";
        public const string EventTooltipTransition = "TooltipTransition";

        /// <summary>
        /// Event fired when the hover timeout for the tool tip gets changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip whose hover timeout has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> HoverTimeChanged;

        /// <summary>
        /// Event fired when the display timeout for the tool tip gets changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip whose display timeout has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> DisplayTimeChanged;

        /// <summary>
        /// Event fired when the fade timeout for the tooltip gets changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip whose fade timeout has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> FadeTimeChanged;

        /// <summary>
        /// Event fired when the tooltip is about to get activated.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip that is about to become
        /// active.
        /// </summary>
        public event GuiEventHandler<EventArgs> TooltipActive
        {
            add { SubscribeEvent(EventTooltipActive, value); }
            remove { UnsubscribeEvent(EventTooltipActive, value); }
        }

        /// <summary>
        /// Event fired when the tooltip has been deactivated.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip that has become inactive.
        /// </summary>
        public event GuiEventHandler<EventArgs> TooltipInactive
        {
            add { SubscribeEvent(EventTooltipInactive, value); }
            remove { UnsubscribeEvent(EventTooltipInactive, value); }
        }

        /// <summary>
        /// Event fired when the tooltip changes target window but stays active.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Tooltip that has transitioned.
        /// </summary>
        public event GuiEventHandler<EventArgs> TooltipTransition
        {
            add { SubscribeEvent(EventTooltipTransition, value); }
            remove { UnsubscribeEvent(EventTooltipTransition, value); }
        }

        /// <summary>
        /// Constructor for the Tooltip base class constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Tooltip(string type, string name)
                : base(type, name)
        {
            _hoverTime = 0.4f;
            _displayTime = 7.5f;
            _inPositionSelf = false;

            AddTooltipProperties();

            SetClippedByParent(false);
            SetDestroyedByParent(false);
            SetAlwaysOnTop(true);

            // we need updates even when not visible
            SetUpdateMode(WindowUpdateMode.WUM_ALWAYS);

            SwitchToInactiveState();
            Hide();
        }

        /// <summary>
        /// ets the target window for the tooltip.  
        /// This used internally to manage tooltips, you  should not have to call this yourself.
        /// </summary>
        /// <param name="wnd">
        /// Window object that the tooltip should be associated with (for now).
        /// </param>
        public void SetTargetWindow(Window wnd)
        {
            if (wnd == null)
            {
                _target = wnd;
            }
            else if (wnd != this)
            {
                if (_target != wnd)
                {
                    wnd.GetGUIContext().GetRootWindow().AddChild(this);
                    _target = wnd;
                }

                // set text to that of the tooltip text of the target
                SetText(wnd.GetTooltipText());

                // set size and position of the tooltip window.
                SizeSelf();
                PositionSelf();
            }

            ResetTimer();

            if (_active)
                OnTooltipTransition(new WindowEventArgs(this));
        }

        /// <summary>
        /// return the current target window for this Tooltip.
        /// </summary>
        /// <returns>
        /// Pointer to the target window for this Tooltip or 0 for none.
        /// </returns>
        public Window GetTargetWindow()
        {
            return _target;
        }

        /// <summary>
        /// Resets the timer on the tooltip when in the Active / Inactive states.  This is used internally
        /// to control the tooltip, it is not normally necessary to call this method yourself.
        /// </summary>
        public void ResetTimer()
        {
            _elapsed = 0;
        }

        /// <summary>
        /// Return the number of seconds the mouse should hover stationary over the target window before
        /// the tooltip gets activated.
        /// </summary>
        /// <returns>
        /// float value representing a number of seconds.
        /// </returns>
        public float GetHoverTime()
        {
            return _hoverTime;
        }

        /// <summary>
        /// Set the number of seconds the tooltip should be displayed for before it automatically
        /// de-activates itself.  0 indicates that the tooltip should never timesout and auto-deactivate.
        /// </summary>
        /// <param name="seconds">
        /// float value representing a number of seconds.
        /// </param>
        public void SetDisplayTime(float seconds)
        {
            if (_displayTime != seconds)
            {
                _displayTime = seconds;

                OnDisplayTimeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the number of seconds the mouse should hover stationary over the target window before
        /// the tooltip gets activated.
        /// </summary>
        /// <param name="seconds">
        /// float value representing a number of seconds.
        /// </param>
        public void SetHoverTime(float seconds)
        {
            if (_hoverTime != seconds)
            {
                _hoverTime = seconds;

                OnHoverTimeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the number of seconds the tooltip should be displayed for before it automatically
        /// de-activates itself.  0 indicates that the tooltip never timesout and auto-deactivates.
        /// </summary>
        /// <returns>
        /// float value representing a number of seconds.
        /// </returns>
        public float GetDisplayTime()
        {
            return _displayTime;
        }

        /// <summary>
        /// Causes the tooltip to position itself appropriately.
        /// </summary>
        public void PositionSelf()
        {
            // no recusion allowed for this function!
            if (_inPositionSelf)
                return;

            _inPositionSelf = true;

            var cursor = GetGUIContext().GetCursor();
            var screen = new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize());
            var tipRect = GetUnclippedOuterRect().Get();
            var mouseImage = cursor.GetImage();

            var mousePos = cursor.GetPosition();
            var mouseSz = Sizef.Zero;

            if (mouseImage != null)
                mouseSz = mouseImage.GetRenderedSize();

            var tmpPos = new Lunatics.Mathematics.Vector2(mousePos.X + mouseSz.Width, mousePos.Y + mouseSz.Height);
            tipRect.Position = tmpPos;

            // if tooltip would be off the right of the screen,
            // reposition to the other side of the mouse cursor.
            if (screen.Right < tipRect.Right)
                tmpPos.X = mousePos.X - tipRect.Width - 5;

            // if tooltip would be off the bottom of the screen,
            // reposition to the other side of the mouse cursor.
            if (screen.Bottom < tipRect.Bottom)
                tmpPos.Y = mousePos.Y - tipRect.Height - 5;

            // set final position of tooltip window.
            SetPosition(new UVector2(UDim.Absolute(tmpPos.X), UDim.Absolute(tmpPos.Y)));

            _inPositionSelf = false;
        }

        /// <summary>
        /// Causes the tooltip to resize itself appropriately.
        /// </summary>
        public void SizeSelf()
        {
            var textSize = GetTextSize();

            SetSize(new USize(UDim.Absolute(textSize.Width), UDim.Absolute(textSize.Height)));
        }

        /// <summary>
        /// Return the size of the area that will be occupied by the tooltip text, given
        /// any current formatting options.
        /// </summary>
        /// <returns>
        /// Size object describing the size of the rendered tooltip text in pixels.</returns>
        public Sizef GetTextSize()
        {
            if (d_windowRenderer != null)
            {
                var wr = (TooltipWindowRenderer) d_windowRenderer;
                return wr.GetTextSize();
            }

            return GetTextSizeImpl();
        }

        /// <summary>
        /// Return the size of the area that will be occupied by the tooltip text, given
        /// any current formatting options.
        /// </summary>
        /// <returns>
        /// Size object describing the size of the rendered tooltip text in pixels.
        /// </returns>
        public virtual Sizef GetTextSizeImpl()
        {
            var rs = GetRenderedString();
            var sz = Sizef.Zero;

            for (var i = 0; i < rs.GetLineCount(); ++i)
            {
                var lineSize = rs.GetPixelSize(this, i);
                sz.Height += lineSize.Height;

                if (lineSize.Width > sz.Width)
                    sz.Width = lineSize.Width;
            }

            return sz;
        }

        /// <summary>
        /// methods to perform processing for each of the widget states 
        /// </summary>
        /// <param name="elapsed"></param>
        protected void DoActiveState(float elapsed)
        {
            // if no target, switch immediately to inactive state.
            if (_target == null || String.IsNullOrEmpty(_target.GetTooltipText()))
            {
                // hide immediately since the text is empty
                Hide();

                SwitchToInactiveState();
            }
                    // else see if display timeout has been reached
            else
            {
                _elapsed += elapsed;
                if ((_displayTime > 0) && ((_elapsed) >= _displayTime))
                {
                    // display time is up, switch states
                    SwitchToInactiveState();
                }
            }
        }

        protected void DoInactiveState(float elapsed)
        {
            _elapsed += elapsed;
            if (_target != null && !String.IsNullOrEmpty(_target.GetTooltipText()) && ((_elapsed) >= _hoverTime))
            {
                SwitchToActiveState();
            }
        }

        // methods to switch widget states
        protected void SwitchToInactiveState()
        {
            _active = false;
            _elapsed = 0;

            // fire event before target gets reset in case that information is required in handler.
            OnTooltipInactive(new WindowEventArgs(this));

            _target = null;
        }

        protected override void DrawSelf(RenderingContext ctx)
        {
            base.DrawSelf(ctx);
        }

        protected void SwitchToActiveState()
        {
            PositionSelf();
            Show();

            _active = true;
            _elapsed = 0;

            OnTooltipActive(new WindowEventArgs(this));
        }

        /// <summary>
        /// validate window renderer 
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as TooltipWindowRenderer) != null;
        }

        /// <summary>
        /// Event trigger method called when the hover timeout gets changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnHoverTimeChanged(WindowEventArgs e)
        {
            FireEvent(HoverTimeChanged, e);
        }

        /// <summary>
        /// Event trigger method called when the display timeout gets changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnDisplayTimeChanged(WindowEventArgs e)
        {
            FireEvent(DisplayTimeChanged, e);
        }

        /// <summary>
        /// Event trigger method called just before the tooltip becomes active.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnTooltipActive(WindowEventArgs e)
        {
            FireEvent(EventTooltipActive, e, EventNamespace);
        }

        /// <summary>
        /// Event trigger method called just after the tooltip is deactivated.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnTooltipInactive(WindowEventArgs e)
        {
            FireEvent(EventTooltipInactive, e, EventNamespace);
        }

        /// <summary>
        /// Event trigger method called just after the tooltip changed target window but remained active.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnTooltipTransition(WindowEventArgs e)
        {
            FireEvent(EventTooltipTransition, e, EventNamespace);
        }

        protected override void UpdateSelf(float elapsed)
        {
            // base class processing.
            base.UpdateSelf(elapsed);

            // do something based upon current Tooltip state.
            if (_active)
            {
                DoActiveState(elapsed);
            }
            else
            {
                DoInactiveState(elapsed);
            }
        }

        protected override void OnHidden(WindowEventArgs e)
        {
            base.OnHidden(e);

            // The animation will take care of fade out or whatnot,
            // at the end it will hide the tooltip to let us know the transition
            // is done. At this point we will remove the tooltip from the
            // previous parent.

            // NOTE: There has to be a fadeout animation! Even if it's a 0 second
            //       immediate hide animation.

            if (GetParent() != null)
                GetParent().RemoveChild(this);
        }

        protected internal override void OnCursorEnters(CursorInputEventArgs e)
        {
            PositionSelf();

            base.OnCursorEnters(e);
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            // base class processing
            base.OnTextChanged(e);

            // set size and position of the tooltip window to consider new text
            SizeSelf();
            PositionSelf();

            // we do not signal we handled it, in case user wants to hear
            // about text changes too.
        }

        private void AddTooltipProperties()
        {
            DefineProperty(
                    "HoverTime",
                    "Property to get/set the hover timeout value in seconds.  Value is a float.",
                    (x, v) => x.SetHoverTime(v), x => x.GetHoverTime(), 0.4f);

            DefineProperty(
                    "DisplayTime",
                    "Property to get/set the display timeout value in seconds.  Value is a float.",
                    (x, v) => x.SetDisplayTime(v), x => x.GetDisplayTime(), 7.5f);
        }

        private void DefineProperty<T>(string name, string help, Action<Tooltip, T> setter, Func<Tooltip, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<Tooltip, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// true if the tooltip is active
        /// </summary>
        private bool _active;

        /// <summary>
        /// Used to track state change timings
        /// </summary>
        private float _elapsed;

        /// <summary>
        /// Current target Window for this Tooltip.
        /// </summary>
        private Window _target;

        /// <summary>
        /// tool-tip hover time (seconds mouse must stay stationary before tip shows).
        /// </summary>
        private float _hoverTime;

        /// <summary>
        /// tool-tip display time (seconds that tip is showsn for).
        /// </summary>
        private float _displayTime;

        /// <summary>
        /// tool-tip fade time (seconds it takes for tip to fade in and/or out).
        /// </summary>
        private float _fadeTime;

        /// <summary>
        /// are in positionSelf function? (to avoid infinite recursion issues)
        /// </summary>
        private bool _inPositionSelf;

        #endregion
    }
}