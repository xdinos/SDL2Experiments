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
    /// Base class for Thumb widget.
    /// 
    /// The thumb widget is used to compose other widgets (like sliders and scroll bars).  You would
    /// not normally need to use this widget directly unless you are making a new widget of some type.
    /// </summary>
    public class Thumb : PushButton
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace="Thumb";

        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/Thumb";

        #region Events
        
        /// <summary>
        /// Event fired when the position of the thumb widget has changed (this
        /// event is only fired when hot tracking is enabled).
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Thumb whose position has changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> ThumbPositionChanged;

        /// <summary>
        /// Event fired when the user begins dragging the thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Thumb that has started to be dragged
        /// by the user.
        /// </summary>
        public event EventHandler<WindowEventArgs> ThumbTrackStarted;

        /// <summary>
        /// Event fired when the user releases the thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Thumb that has been released.
        /// </summary>
        public event EventHandler<WindowEventArgs> ThumbTrackEnded;

        #endregion
        
        /// <summary>
        /// return whether hot-tracking is enabled or not.
        /// </summary>
        /// <returns>
        /// true if hot-tracking is enabled.  
        /// false if hot-tracking is disabled.
        /// </returns>
        public bool IsHotTracked()
        {
            return _hotTrack;
        }

        /// <summary>
        /// return whether the thumb is movable on the vertical axis.
        /// </summary>
        /// <returns>
        /// true if the thumb is movable along the vertical axis.
        /// false if the thumb is fixed on the vertical axis.
        /// </returns>
        public bool IsVertFree()
        {
            return _vertFree;
        }

        /// <summary>
        /// return whether the thumb is movable on the horizontal axis.
        /// </summary>
        /// <returns>
        /// true if the thumb is movable along the horizontal axis.
        /// false if the thumb is fixed on the horizontal axis.
        /// </returns>
        public bool IsHorzFree()
        {
            return _horzFree;
        }

        /// <summary>
        /// Return a std::pair that describes the current range set for the vertical movement.
        /// </summary>
        /// <returns>
        /// a std::pair describing the current vertical range.  
        /// The first element is the minimum value, the second element is the maximum value.
        /// </returns>
        public Tuple<float, float> GetVertRange()
        {
            return new Tuple<float, float>(_vertMin, _vertMax);
        }

        /// <summary>
        /// Return a std::pair that describes the current range set for the horizontal movement.
        /// </summary>
        /// <returns>
        /// a std::pair describing the current horizontal range.  
        /// The first element is the minimum value, the second element is the maximum value.
        /// </returns>
        public Tuple<float, float> GetHorzRange()
        {
            return new Tuple<float, float>(_horzMin, _horzMax);
        }

        /// <summary>
        /// set whether the thumb uses hot-tracking.
        /// </summary>
        /// <param name="setting">
        /// true to enable hot-tracking.
        /// false to disable hot-tracking.
        /// </param>
        public void SetHotTracked(bool setting)
        {
            _hotTrack = setting;
        }

        /// <summary>
        /// set whether thumb is movable on the vertical axis.
        /// </summary>
        /// <param name="setting">
        /// true to allow movement of thumb along the vertical axis.  false to fix thumb on the vertical axis.
        /// </param>
        public void SetVertFree(bool setting)
        {
            _vertFree = setting;
        }

        /// <summary>
        /// set whether thumb is movable on the horizontal axis.
        /// </summary>
        /// <param name="setting">
        /// true to allow movement of thumb along the horizontal axis.  false to fix thumb on the horizontal axis.
        /// </param>
        public void SetHorzFree(bool setting)
        {
            _horzFree = setting;
        }

        /// <summary>
        /// set the movement range of the thumb for the vertical axis.
        /// 
        /// The values specified here are relative to the parent window for the thumb, and are specified in whichever
        /// metrics mode is active for the widget.
        /// </summary>
        /// <param name="min">
        /// the minimum setting for the thumb on the vertical axis.
        /// </param>
        /// <param name="max">
        /// the maximum setting for the thumb on the vertical axis.
        /// </param>
        public void SetVertRange(float min, float max)
        {
            // ensure min <= max, swap if not.
	        if (min > max)
	        {
		        var tmp = min;
		        max = min;
		        min = tmp;
	        }

	        _vertMax = max;
	        _vertMin = min;

	        // validate current position.
	        var cp = CoordConverter.AsRelative(GetYPosition(), GetParentPixelSize().Height);

	        if (cp < min)
	        {
		        SetYPosition(UDim.Relative(min));
	        }
	        else if (cp > max)
	        {
                SetYPosition(UDim.Relative(max));
	        }
        }

        /// <summary>
        /// set the movement range of the thumb for the vertical axis.
        /// 
        /// The values specified here are relative to the parent window for the thumb, 
        /// and are specified in whichever metrics mode is active for the widget.
        /// </summary>
        /// <param name="range">
        /// the setting for the thumb on the vertical axis.
        /// </param>
        public void SetVertRange(Tuple<float, float> range)
        {
            SetVertRange(range.Item1, range.Item2);
        }

        /// <summary>
        /// set the movement range of the thumb for the horizontal axis.
        /// 
        /// The values specified here are relative to the parent window for the thumb, and are specified in whichever
        /// metrics mode is active for the widget.
        /// </summary>
        /// <param name="min">
        /// the minimum setting for the thumb on the horizontal axis.
        /// </param>
        /// <param name="max">
        /// the maximum setting for the thumb on the horizontal axis.
        /// </param>
        public void SetHorzRange(float min, float max)
        {
            var parentSize = GetParentPixelSize();

            // ensure min <= max, swap if not.
            if (min > max)
            {
                var tmp = min;
                max = min;
                min = tmp;
            }

            _horzMax = max;
            _horzMin = min;

            // validate current position.
            var cp = CoordConverter.AsAbsolute(GetXPosition(), parentSize.Width);

            if (cp < min)
            {
                SetXPosition(UDim.Absolute(min));
            }
            else if (cp > max)
            {
                SetXPosition(UDim.Absolute(max));
            }
        }


        /// <summary>
        /// set the movement range of the thumb for the horizontal axis.
        /// 
        /// The values specified here are relative to the parent window for the thumb, 
        /// and are specified in whichever metrics mode is active for the widget.
        /// </summary>
        /// <param name="range">
        /// the setting for the thumb on the horizontal axis.
        /// </param>
        public void SetHorzRange(Tuple<float, float> range)
        {
            SetHorzRange(range.Item1, range.Item2);
        }
        
        /// <summary>
        /// Constructor for Thumb objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Thumb(string type, string name)
            : base(type, name)
        {
            _hotTrack = true;
            _vertFree = false;
            _horzFree = false;
            _vertMin = 0.0f;
            _vertMax = 1.0f;
            _horzMin = 0.0f;
            _horzMax = 1.0f;
            _beingDragged = false;

            AddThumbProperties();
        }

        protected override void BanPropertiesForAutoWindow()
        {
            base.BanPropertiesForAutoWindow();

            BanPropertyFromXML("VertRange");
            BanPropertyFromXML("HorzRange");
            BanPropertyFromXML("VertFree");
            BanPropertyFromXML("HorzFree");
        }

        /// <summary>
        /// event triggered internally when the position of the thumb
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbPositionChanged(WindowEventArgs e)
        {
            FireEvent(ThumbPositionChanged, e);
        }

        /// <summary>
        /// Handler triggered when the user begins to drag the thumb. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackStarted(WindowEventArgs e)
        {
            FireEvent(ThumbTrackStarted, e);
        }

        /// <summary>
        /// Handler triggered when the thumb is released
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackEnded(WindowEventArgs e)
        {
            FireEvent(ThumbTrackEnded, e);
        }

        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            // default processing
	        base.OnCursorMove(e);

	        // only react if we are being dragged
	        if (_beingDragged)
	        {
	            var parentSize = GetParentPixelSize();

	            var delta = CoordConverter.ScreenToWindow(this, e.Position);

                var hmin = _horzMin;
                var hmax = _horzMax;
                var vmin = _vertMin;
                var vmax = _vertMax;

		        // calculate amount of movement      
		        delta -= _dragPoint;
                delta.X /= parentSize.Width;
                delta.Y /= parentSize.Height;

		        //
		        // Calculate new (pixel) position for thumb
		        //
	            var newPos = GetPosition();

		        if (_horzFree)
		        {
			        newPos.d_x.d_scale += delta.X;

			        // limit value to within currently set range
			        newPos.d_x.d_scale = (newPos.d_x.d_scale < hmin) ? hmin : (newPos.d_x.d_scale > hmax) ? hmax : newPos.d_x.d_scale;
		        }

		        if (_vertFree)
		        {
			        newPos.d_y.d_scale += delta.Y;

			        // limit new position to within currently set range
			        newPos.d_y.d_scale = (newPos.d_y.d_scale < vmin) ? vmin : (newPos.d_y.d_scale > vmax) ? vmax : newPos.d_y.d_scale;
		        }

		        // update thumb position if needed
		        if (newPos != GetPosition())
		        {
			        SetPosition(newPos);

			        // send notification as required
			        if (_hotTrack)
			        {
				        OnThumbPositionChanged(new WindowEventArgs(this));
			        }
		        }
	        }

	        ++e.handled;
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // default processing
            base.OnCursorPressHold(e);

	        if (e.Source == CursorInputSource.Left)
	        {
		        // initialise the dragging state
		        _beingDragged = true;
		        _dragPoint = CoordConverter.ScreenToWindow(this, e.Position);

		        // trigger tracking started event
		        OnThumbTrackStarted(new WindowEventArgs(this));

		        ++e.handled;
	        }
        }

        protected override void OnCaptureLost(WindowEventArgs e)
        {
            // default handling
	        base.OnCaptureLost(e);

	        _beingDragged = false;

	        // trigger tracking ended event
	        var args=new WindowEventArgs(this);
	        OnThumbTrackEnded(args);

	        // send notification whenever thumb is released
	        OnThumbPositionChanged(args);
        }

        #region Dynamic Properties

        private void AddThumbProperties()
        {
            DefineProperty(
                "HotTracked",
                "Property to get/set the state of the state of the 'hot-tracked' setting for the thumb. Value is either \"True\" or \"False\".",
                (w, v) => w.SetHotTracked(v), w => w.IsHotTracked(), true);
    
            DefineProperty(
                "VertRange",
                "Property to get/set the vertical movement range for the thumb.  Value is \"min:[float] max:[float]\".",
                (w, v) => w.SetVertRange(v), w => w.GetVertRange(), new Tuple<float, float>(0.0f, 1.0f));

            DefineProperty(
                "HorzRange",
                "Property to get/set the horizontal movement range for the thumb.  Value is \"min:[float] max:[float]\".",
                (w, v) => w.SetHorzRange(v), w => w.GetHorzRange(), new Tuple<float, float>(0.0f, 1.0f));

            DefineProperty(
                "VertFree",
                "Property to get/set the state the setting to free the thumb vertically.  Value is either \"True\" or \"False\".",
                (w, v) => w.SetVertFree(v), w => w.IsVertFree(), false);

            DefineProperty(
                "HorzFree",
                "Property to get/set the state the setting to free the thumb horizontally.  Value is either \"True\" or \"False\".",
                (w, v) => w.SetHorzFree(v), w => IsHorzFree(), false);
        }

        private void DefineProperty<T>(string name, string help, Action<Thumb, T> setter, Func<Thumb, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<Thumb, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #endregion

        #region Fields

        /// <summary>
        /// true if events are to be sent real-time, else just when thumb is released
        /// </summary>
        private bool _hotTrack;

        /// <summary>
        /// true if thumb is movable vertically
        /// </summary>
        private bool _vertFree;

        /// <summary>
        /// true if thumb is movable horizontally
        /// </summary>
        private bool _horzFree;

        /// <summary>
        /// vertical range
        /// </summary>
        private float _vertMin, _vertMax;

        /// <summary>
        /// horizontal range
        /// </summary>
        private float _horzMin, _horzMax;

        /// <summary>
        /// true if thumb is being dragged
        /// </summary>
        private bool _beingDragged;

        /// <summary>
        /// point where we are being dragged at.
        /// </summary>
        private Lunatics.Mathematics.Vector2 _dragPoint;

        #endregion
    }
}