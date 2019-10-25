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
    /// Class representing the title bar for Frame Windows.
    /// </summary>
    public class Titlebar : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Titlebar";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Titlebar";

        /// <summary>
        /// Return whether this title bar will respond to dragging.
        /// </summary>
        /// <returns>
        /// true if the title bar will respond to dragging, 
        /// false if the title bar will not respond.
        /// </returns>
	    public bool IsDraggingEnabled()
	    {
	        return _dragEnabled;
	    }
        
        /// <summary>
        /// Set whether this title bar widget will respond to dragging.
        /// </summary>
        /// <param name="setting">
        /// true if the title bar should respond to being dragged, 
        /// false if it should not respond.
        /// </param>
	    public void SetDraggingEnabled(bool setting)
	    {
	        if (_dragEnabled != setting)
	        {
		        _dragEnabled = setting;

		        // stop dragging now if the setting has been disabled.
		        if ((!_dragEnabled) && _dragging)
		        {
			        ReleaseInput();
		        }

		        // call event handler.
		        OnDraggingModeChanged(new WindowEventArgs(this));
	        }
	    }

        /// <summary>
        /// Checks whether the title bar widget is being dragged at the moment
        /// </summary>
        /// <returns></returns>
        public bool IsDragged()
        {
            return _dragging;
        }

        /// <summary>
        /// Gets the point at which the title bar widget is/was being dragged
        /// </summary>
        /// <returns></returns>
        public Lunatics.Mathematics.Vector2 GetDragPoint()
        {
            return _dragPoint;
        }

        /// <summary>
        /// Constructor for Titlebar base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Titlebar(string type, string name)
            : base(type, name)
        {
            AddTitlebarProperties();
            SetAlwaysOnTop(true);

            // basic initialisation
            _dragging = false;
            _dragEnabled = true;
        }

        // TODO: Destructor for Titlebar base class.
	    // TODO: virtual ~Titlebar() {}


        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // Base class processing.
	        base.OnCursorMove(e);

	        if (_dragging && (d_parent != null))
	        {
	            var delta = CoordConverter.ScreenToWindow(this, e.Position);

		        // calculate amount that window has been moved
		        delta -= _dragPoint;

		        // move the window.  *** Again: Titlebar objects should only be attached to FrameWindow derived classes. ***
		        ((FrameWindow)d_parent).OffsetPixelPosition(delta);

		        ++e.handled;
	        }
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // Base class processing
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
	        {
		        if ((d_parent != null) && _dragEnabled)
		        {
                    // we want all cursor inputs from now on
			        if (CaptureInput())
			        {
				        // initialise the dragging state
				        _dragging = true;
				        _dragPoint = CoordConverter.ScreenToWindow(this, e.Position);

                        // store old constraint area
                        _oldCursorArea = GetGUIContext().GetCursor().GetConstraintArea();

				        // setup new constraint area to be the intersection of the old area and our grand-parent's clipped inner-area
				        Rectf constrainArea;

				        if ((d_parent == null) || (GetParent().GetParent() == null))
				        {
				            var screen = new Rectf(Lunatics.Mathematics.Vector2.Zero, GetRootContainerSize());
					        constrainArea = screen.GetIntersection(_oldCursorArea);
				        }
				        else
				        {
					        constrainArea = GetParent().GetParent().GetInnerRectClipper().GetIntersection(_oldCursorArea);
				        }

			            GetGUIContext().GetCursor().SetConstraintArea(constrainArea);
			        }
		        }

		        ++e.handled;
	        }
        }

        protected internal override void OnCursorActivate(CursorInputEventArgs e)
        {
            // Base class processing
            base.OnCursorActivate(e);

            if (e.Source == CursorInputSource.Left)
            {
                ReleaseInput();
                ++e.handled;
            }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // Base class processing
	        base.OnCaptureLost(e);

	        // when we lose out hold on the mouse inputs, we are no longer dragging.
	        _dragging = false;

	        // restore old constraint area
            GetGUIContext().GetCursor().SetConstraintArea(_oldCursorArea);
        }

        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            base.OnFontChanged(e);

            if (d_parent != null)
                GetParent().PerformChildWindowLayout();
        }

        /// <summary>
        /// Event handler called when the 'draggable' state for the title bar is changed.
        /// 
        /// Note that this is for 'internal' use at the moment and as such does not add or
        /// fire a public Event that can be subscribed to.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDraggingModeChanged(WindowEventArgs e)
        {
            
        }

        private void AddTitlebarProperties()
        {
            AddProperty(
                new TplWindowProperty<Titlebar, bool>(
                    "DraggingEnabled",
                    "Property to get/set the state of the dragging enabled setting for the Titlebar.  Value is either \"True\" or \"False\".",
                    (x, v) => x.SetDraggingEnabled(v), x => x.IsDraggingEnabled(), WidgetTypeName, true));
        }

        #region Fields

        /// <summary>
        /// set to true when the window is being dragged.
        /// </summary>
        private bool _dragging;

        /// <summary>
        /// Point at which we are being dragged.
        /// </summary>
        private Lunatics.Mathematics.Vector2 _dragPoint;

        /// <summary>
        /// true when dragging for the widget is enabled.
        /// </summary>
        private bool _dragEnabled;

        /// <summary>
        /// Used to backup cursor restraint area.
        /// </summary>
        private Rectf _oldCursorArea;

        #endregion
    }
}