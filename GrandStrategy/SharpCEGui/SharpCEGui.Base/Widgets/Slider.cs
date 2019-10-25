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
    /// Base class for ItemEntry window renderer objects.
    /// </summary>
    public abstract class SliderWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected SliderWindowRenderer(string name)
            : base(name, Slider.EventNamespace)
        {

        }

        /// <summary>
        /// update the size and location of the thumb to properly represent the current state of the slider
        /// </summary>
        public abstract void UpdateThumb();

        /// <summary>
        /// return value that best represents current slider value given the current location of the thumb.
        /// </summary>
        /// <returns>
        /// float value that, given the thumb widget position, best represents the current value for the slider.
        /// </returns>
        public abstract float GetValueFromThumb();

        /// <summary>
        /// Given window location \a pt, return a value indicating what change should be made to the slider.
        /// </summary>
        /// <param name="pt">
        /// Point object describing a pixel position in window space.
        /// </param>
        /// <returns>
        /// - -1 to indicate slider should be moved to a lower setting.
        /// -  0 to indicate slider should not be moved.
        /// - +1 to indicate slider should be moved to a higher setting.
        /// </returns>
        public abstract float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt);
    }

    /// <summary>
    /// Base class for Slider widgets.
    /// <para>
    /// The slider widget has a default range of 0.0f - 1.0f.  This enables use of the slider value to scale
    /// any value needed by way of a simple multiplication.
    /// </para>
    /// </summary>
    public class Slider : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Slider";

#region Events
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Slider";

        public const string EventValueChanged = "ValueChanged";
        public const string EventThumbTrackStarted = "ThumbTrackStarted";
        public const string EventThumbTrackEnded="ThumbTrackEnded";

        /// <summary>
        /// Event fired when the slider value changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Slider whose value has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ValueChanged
        {
            add { SubscribeEvent(EventValueChanged, value); }
            remove { UnsubscribeEvent(EventValueChanged, value); }
        }

        /// <summary>
        /// Event fired when the user begins dragging the thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Slider whose thumb has started to
        /// be dragged.
        /// </summary>
        public event GuiEventHandler<EventArgs> ThumbTrackStarted
        {
            add { SubscribeEvent(EventThumbTrackStarted, value); }
            remove { UnsubscribeEvent(EventThumbTrackStarted, value); }
        }

        /// <summary>
        /// Event fired when the user releases the thumb.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Slider whose thumb has been released.
        /// </summary>
        public event GuiEventHandler<EventArgs> ThumbTrackEnded
        {
            add { SubscribeEvent(EventThumbTrackEnded, value); }
            remove { UnsubscribeEvent(EventThumbTrackEnded, value); }
        }
        
#endregion

        /// <summary>
        /// Widget name for the thumb component.
        /// </summary>
        public const string ThumbName = "__auto_thumb__";

        /// <summary>
        /// return the current slider value.
        /// </summary>
        /// <returns>
        /// float value equal to the sliders current value.
        /// </returns>
        public float GetCurrentValue()
        {
            return _value;
        }

        /// <summary>
        /// return the maximum value set for this widget
        /// </summary>
        /// <returns>
        /// float value equal to the currently set maximum value for this slider.
        /// </returns>
        public float GetMaxValue()
        {
            return _maxValue;
        }

        /// <summary>
        /// return the current click step setting for the slider.
        /// <para>
        /// The click step size is the amount the slider value will be adjusted when the widget
        /// is clicked wither side of the slider thumb.
        /// </para>
        /// </summary>
        /// <returns>
        /// float value representing the current click step setting.
        /// </returns>
        public float GetClickStep()
        {
            return _step;
        }

        /// <summary>
        /// Return a pointer to the Thumb component widget for this Slider.
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

        protected override void InitialiseComponents()
        {
            // get thumb
	        var thumb = GetThumb();

	        // bind handler to thumb events
            thumb.ThumbPositionChanged += HandleThumbMoved;
            thumb.ThumbTrackStarted += HandleThumbTrackStarted;
            thumb.ThumbTrackEnded += HandleThumbTrackEnded;

	        PerformChildWindowLayout();
        }

        /// <summary>
        /// set the maximum value for the slider.  Note that the minimum value is fixed at 0.
        /// </summary>
        /// <param name="maxVal">
        /// float value specifying the maximum value for this slider widget.
        /// </param>
        public void SetMaxValue(float maxVal)
        {
            _maxValue = maxVal;

	        var oldval = _value;

	        // limit current value to be within new max
	        if (_value > _maxValue) {
		        _value = _maxValue;
	        }

	        UpdateThumb();

	        // send notification if slider value changed.
	        if (_value != oldval)
	        {
		        OnValueChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// set the current slider value.
        /// </summary>
        /// <param name="value">
        /// float value specifying the new value for this slider widget.
        /// </param>
        public void SetCurrentValue(float value)
        {
            var oldval = _value;

	        // range for value: 0 <= value <= maxValue
	        _value = (value >= 0.0f) ? ((value <= _maxValue) ? value : _maxValue) : 0.0f;

	        UpdateThumb();

	        // send notification if slider value changed.
	        if (_value != oldval)
	        {
		        OnValueChanged(new WindowEventArgs(this));
	        }
        }

        /// <summary>
        /// set the current click step setting for the slider.
        /// <para>
        /// The click step size is the amount the slider value will be adjusted when the widget
        /// is clicked wither side of the slider thumb.
        /// </para>
        /// </summary>
        /// <param name="step">
        /// float value representing the click step setting to use.
        /// </param>
        public void SetClickStep(float step)
        {
            _step = step;
        }

        /// <summary>
        /// Slider base class constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Slider(string type, string name) : base(type, name)
        {
            _value = 0.0f;
            _maxValue = 1.0f;
            _step = 0.01f;

            AddSliderProperties();
        }

        /// <summary>
        /// update the size and location of the thumb to properly represent the current state of the slider
        /// </summary>
        protected virtual void UpdateThumb()
        {
            if (d_windowRenderer != null)
            {
                ((SliderWindowRenderer) d_windowRenderer).UpdateThumb();
            }
            else
            {
                throw new InvalidRequestException("This function must be implemented by the window renderer module");
            }
        }

        /// <summary>
        /// return value that best represents current slider value given the current location of the thumb.
        /// </summary>
        /// <returns>
        /// float value that, given the thumb widget position, best represents the current value for the slider.
        /// </returns>
        protected virtual float GetValueFromThumb()
        {
            if (d_windowRenderer != null)
                return ((SliderWindowRenderer) d_windowRenderer).GetValueFromThumb();

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }


        /*!
	    \brief
		    Given window location \a pt, return a value indicating what change should be 
		    made to the slider.

	    \param pt
		    Point object describing a pixel position in window space.

	    \return
		    - -1 to indicate slider should be moved to a lower setting.
		    -  0 to indicate slider should not be moved.
		    - +1 to indicate slider should be moved to a higher setting.
	    */

        protected virtual float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt)
        {
            if (d_windowRenderer != null)
                return ((SliderWindowRenderer) d_windowRenderer).GetAdjustDirectionFromPoint(pt);

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// handler function for when thumb moves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void /*bool*/ HandleThumbMoved(object sender, WindowEventArgs e)
        {
            SetCurrentValue(GetValueFromThumb());
            // TODO: return true;
        }

        /// <summary>
        /// handler function for when thumb tracking begins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void /*bool*/ HandleThumbTrackStarted(object sender, WindowEventArgs e)
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
        /// <returns></returns>
        protected void /*bool*/ HandleThumbTrackEnded(object sender, WindowEventArgs e)
        {
            // simply trigger our own version of this event
	        OnThumbTrackEnded(new WindowEventArgs(this));
	        // TODO: return true;
        }

        // validate window renderer
        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as SliderWindowRenderer) != null;
        }

        /// <summary>
        /// Handler triggered when the slider value changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(WindowEventArgs e)
        {
            FireEvent(EventValueChanged, e, EventNamespace);
        }

        /// <summary>
        /// Handler triggered when the user begins to drag the slider thumb. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackStarted(WindowEventArgs e)
        {
            FireEvent(EventThumbTrackStarted, e, EventNamespace);
        }

        /// <summary>
        /// Handler triggered when the slider thumb is released
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbTrackEnded(WindowEventArgs e)
        {
            FireEvent(EventThumbTrackEnded, e, EventNamespace);
        }

        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            // base class processing
            base.OnCursorPressHold(e);

            if (e.Source == CursorInputSource.Left)
            {
                var adj = GetAdjustDirectionFromPoint(e.Position);

                // adjust slider position in whichever direction as required.
                if (Math.Abs(adj) > float.Epsilon)
                {
                    SetCurrentValue(_value + adj*_step);
                }

                ++e.handled;
            }
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing
	        base.OnScroll(e);

	        // scroll by e.wheelChange * stepSize
	        SetCurrentValue(_value + _step * e.scroll);

	        // ensure the message does not go to our parent.
	        ++e.handled;
        }

        private void AddSliderProperties()
        {
            AddProperty(new TplWindowProperty<Slider, float>(
                            "CurrentValue",
                            "Property to get/set the current value of the slider.  Value is a float.",
                            (x, v) => x.SetCurrentValue(v), x => x.GetCurrentValue(), WidgetTypeName));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Slider, float>(
                            "MaximumValue",
                            "Property to get/set the maximum value of the slider.  Value is a float.",
                            (x, v) => x.SetMaxValue(v), x => x.GetMaxValue(), WidgetTypeName, 1.0f));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<Slider, float>(
                            "ClickStepSize",
                            "Property to get/set the click-step size for the slider.  Value is a float.",
                            (x, v) => x.SetClickStep(v), x => x.GetClickStep(), WidgetTypeName, 0.01f));
        }

        #region Fields

        /// <summary>
        /// current slider value
        /// </summary>
        private float _value;

        /// <summary>
        /// slider maximum value (minimum is fixed at 0)
        /// </summary>
        private float _maxValue;

        /// <summary>
        /// amount to adjust slider by when clicked (and not dragged).
        /// </summary>
        private float _step;

        #endregion
    }
}