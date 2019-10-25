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
using System.Globalization;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for the Spinner widget.
    /// <para>
    /// The spinner widget has a text area where numbers may be entered
    /// and two buttons which may be used to increase or decrease the
    /// value in the text area by a user specified amount.
    /// </para>
    /// </summary>
    public class Spinner : Window
    {
        /// <summary>
        /// Enumerated type specifying possible input and/or display modes for the spinner.
        /// </summary>
        public enum TextInputMode
        {
            /// <summary>
            /// Floating point decimal.
            /// </summary>
            FloatingPoint,

            /// <summary>
            /// Integer decimal.
            /// </summary>
            Integer,

            /// <summary>
            /// Hexadecimal.
            /// </summary>
            Hexadecimal,

            /// <summary>
            /// Octal
            /// </summary>
            Octal
        }

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Spinner";

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Spinner";

        #region Events

        /// <summary>
        /// Event fired when the spinner current value changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Spinner whose current value has
        /// changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> ValueChanged;

        /// <summary>
        /// Event fired when the spinner step value is changed.
        /// Handlers area passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Spinner whose step value has
        /// changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> StepChanged;

        /// <summary>
        /// Event fired when the maximum spinner value is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Spinner whose maximum value has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> MaximumValueChanged;

        /// <summary>
        /// Event fired when the minimum spinner value is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::windows set to the Spinner whose minimum value has
        /// been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> MinimumValueChanged;

        /// <summary>
        /// Event fired when the spinner text input &amp; display mode is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the Spinner whose text mode has been
        /// changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> TextInputModeChanged;

        #endregion

        /// <summary>
        /// Widget name for the editbox thumb component.
        /// </summary>
        public const string EditboxName = "__auto_editbox__";

        /// <summary>
        /// Widget name for the increase button component.
        /// </summary>
        public const string IncreaseButtonName = "__auto_incbtn__";

        /// <summary>
        /// Widget name for the decrease button component.
        /// </summary>
        public const string DecreaseButtonName = "__auto_decbtn__";

        /// <summary>
        /// Constructor for Spinner objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Spinner(string type, string name) :
            base(type, name)
        {
            _stepSize = 1.0f;
            _currentValue = 1.0f;
            _maxValue = Int16.MaxValue;
            _minValue = Int16.MinValue;
            _inputMode = TextInputMode.FloatingPoint;

            AddSpinnerProperties();
        }

        protected override void InitialiseComponents()
        {
            // get all the component widgets
            var increaseButton = GetIncreaseButton();
            var decreaseButton = GetDecreaseButton();
            var editbox = GetEditbox();

            // setup component controls
            increaseButton.SetWantsMultiClickEvents(false);
            increaseButton.SetMouseAutoRepeatEnabled(true);
            decreaseButton.SetWantsMultiClickEvents(false);
            decreaseButton.SetMouseAutoRepeatEnabled(true);

            // perform event subscriptions.
            increaseButton.CursorPressHold += HandleIncreaseButton;
            decreaseButton.CursorPressHold += HandleDecreaseButton;
            editbox.TextChanged += HandleEditTextChange;

            // final initialisation
            SetTextInputMode(TextInputMode.Integer);
            SetCurrentValue(0.0f);
            PerformChildWindowLayout();
        }

        /// <summary>
        /// Return the current spinner value.
        /// </summary>
        /// <returns>
        /// current value of the Spinner.
        /// </returns>
        public double GetCurrentValue()
        {
            return _currentValue;
        }

        /// <summary>
        /// Return the current step value.
        /// </summary>
        /// <returns>
        /// Step value.  
        /// <para>This is the value added to the spinner vaue when the up / down buttons are clicked.</para>
        /// </returns>
        public double GetStepSize()
        {
            return _stepSize;
        }

        /// <summary>
        /// Return the current maximum limit value for the Spinner.
        /// </summary>
        /// <returns>
        /// Maximum value that is allowed for the spinner.
        /// </returns>
        public double GetMaximumValue()
        {
            return _maxValue;
        }

        /// <summary>
        /// Return the current minimum limit value for the Spinner.
        /// </summary>
        /// <returns>
        /// Minimum value that is allowed for the spinner.
        /// </returns>
        public double GetMinimumValue()
        {
            return _minValue;
        }

        /// <summary>
        /// Return the current text input / display mode setting.
        /// </summary>
        /// <returns>
        /// One of the TextInputMode enumerated values indicating the current text input and display mode.
        /// </returns>
        public TextInputMode GetTextInputMode()
        {
            return _inputMode;
        }

        /// <summary>
        /// Set the current spinner value.
        /// </summary>
        /// <param name="value">
        /// value to be assigned to the Spinner.
        /// </param>
        public void SetCurrentValue(double value)
        {
            if (value != _currentValue)
            {
                // limit input value to within valid range for spinner
                value = Math.Max(Math.Min(value, _maxValue), _minValue);

                _currentValue = value;

                OnValueChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the current step value.
        /// </summary>
        /// <param name="step">
        /// The value added to be the spinner value when the up / down buttons are clicked.
        /// </param>
        public void SetStepSize(double step)
        {
            if (step != _stepSize)
            {
                _stepSize = step;

                OnStepChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the spinner maximum value.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value to be allowed by the spinner.
        /// </param>
        public void SetMaximumValue(double maxValue)
        {
            if (maxValue != _maxValue)
            {
                _maxValue = maxValue;

                OnMaximumValueChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the spinner minimum value.
        /// </summary>
        /// <param name="minVaue">
        /// The minimum value to be allowed by the spinner.
        /// </param>
        public void SetMinimumValue(double minVaue)
        {
            if (minVaue != _minValue)
            {
                _minValue = minVaue;

                OnMinimumValueChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Set the spinner input / display mode.
        /// </summary>
        /// <param name="mode">
        /// One of the TextInputMode enumerated values indicating the text
        /// input / display mode to be used by the spinner.
        /// </param>
        public void SetTextInputMode(TextInputMode mode)
        {
            if (mode != _inputMode)
            {
                switch (mode)
                {
                    case TextInputMode.FloatingPoint:
                        GetEditbox().SetValidationString(FloatValidator);
                        break;
                    case TextInputMode.Integer:
                        GetEditbox().SetValidationString(IntegerValidator);
                        break;
                    case TextInputMode.Hexadecimal:
                        GetEditbox().SetValidationString(HexValidator);
                        break;
                    case TextInputMode.Octal:
                        GetEditbox().SetValidationString(OctalValidator);
                        break;
                    default:
                        throw new InvalidRequestException("An unknown TextInputMode was specified.");
                }

                _inputMode = mode;

                OnTextInputModeChanged(new WindowEventArgs(this));
            }
        }

        protected const string FloatValidator = "-?\\d*\\.?\\d*"; //!< Validator regex used for floating point mode.
        protected const string IntegerValidator = "-?\\d*"; //!< Validator regex used for decimal integer mode.
        protected const string HexValidator = "[0-9a-fA-F]*"; //!< Validator regex used for hexadecimal mode.
        protected const string OctalValidator = "[0-7]*"; //!< Validator regex used for octal mode.

        /// <summary>
        /// Returns the numerical representation of the current editbox text.
        /// </summary>
        /// <returns>
        /// double value that is the numerical equivalent of the editbox text.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if the text can not be converted.
        /// </exception>
        protected virtual double GetValueFromText()
        {
            var tmpTxt = GetEditbox().GetText();

            // handle empty and lone '-' or '.' cases
            if (String.IsNullOrEmpty(tmpTxt) || (tmpTxt == "-") || (tmpTxt == "."))
                return 0.0;

            double val;

            try
            {
                switch (_inputMode)
                {
                    case TextInputMode.FloatingPoint:
                        val = Double.Parse(tmpTxt);
                        break;
                    case TextInputMode.Integer:
                        val = Int64.Parse(tmpTxt);
                        break;
                    case TextInputMode.Hexadecimal:
                        val = Int64.Parse(tmpTxt, NumberStyles.HexNumber);
                        break;
                    case TextInputMode.Octal:
                        throw new NotImplementedException();
                    default:
                        throw new InvalidRequestException("An unknown TextInputMode was encountered.");
                }
            }
            catch
            {
                throw new InvalidRequestException("The string '" + GetEditbox().GetText() +
                                                  "' can not be converted to numerical representation.");
            }

            return val;
        }

        /// <summary>
        /// Returns the textual representation of the current spinner value.
        /// </summary>
        /// <returns>
        /// String object that is equivalent to the the numerical value of the spinner.
        /// </returns>
        protected virtual string GetTextFromValue()
        {
            switch (_inputMode)
            {
                case TextInputMode.FloatingPoint:
                    return _currentValue.ToString("N9");
                case TextInputMode.Integer:
                    return ((int) _currentValue).ToString(CultureInfo.InvariantCulture);
                case TextInputMode.Hexadecimal:
                    return ((int) _currentValue).ToString("X");
                case TextInputMode.Octal:
                    throw new NotImplementedException();
                default:
                    throw new InvalidRequestException("An unknown TextInputMode was encountered.");
            }
        }

        /// <summary>
        /// Return a pointer to the 'increase' PushButtoncomponent widget for this Spinner.
        /// </summary>
        /// <returns>
        /// Pointer to a PushButton object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the increase PushButton component does not exist.
        /// </exception>
        protected PushButton GetIncreaseButton()
        {
            return (PushButton) GetChild(IncreaseButtonName);
        }

        /// <summary>
        /// Return a pointer to the 'decrease' PushButton component widget for this Spinner.
        /// </summary>
        /// <returns>Pointer to a PushButton object.</returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the 'decrease' PushButton component does not exist.
        /// </exception>
        protected PushButton GetDecreaseButton()
        {
            return (PushButton) GetChild(DecreaseButtonName);
        }

        /// <summary>
        /// Return a pointer to the Editbox component widget for this Spinner.
        /// </summary>
        /// <returns>Pointer to a Editbox object.</returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the Editbox component does not exist.
        /// </exception>
        protected Editbox GetEditbox()
        {
            return (Editbox) GetChild(EditboxName);
        }

        /*************************************************************************
        	Overrides for Event handler methods
        *************************************************************************/

        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            // Propagate to children
            GetEditbox().SetFont(GetFont());
            // Call base class handler
            base.OnFontChanged(e);
        }

        protected override void OnTextChanged(WindowEventArgs e)
        {
            var editbox = GetEditbox();

            // update only if needed
            if (editbox.GetText() != GetText())
            {
                // done before doing base class processing so event subscribers see
                // 'updated' version.
                editbox.SetText(GetText());
                ++e.handled;

                base.OnTextChanged(e);
            }
        }

        protected override void OnActivated(ActivationEventArgs e)
        {
            if (!IsActive())
            {
                base.OnActivated(e);

                var editbox = GetEditbox();

                if (!editbox.IsActive())
                {
                    editbox.Activate();
                }
            }
        }

        /// <summary>
        /// Method called when the spinner value changes.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnValueChanged(WindowEventArgs e)
        {
            var editbox = GetEditbox();

            // mute to save doing unnecessary events work.
            var wasMuted = editbox.IsMuted();
            editbox.SetMutedState(true);

            // Update text with new value.
            // (allow empty and '-' cases to equal 0 with no text change required)
            if (!(_currentValue == 0f && (String.IsNullOrEmpty(editbox.GetText()) || editbox.GetText() == "-")))
            {
                editbox.SetText(GetTextFromValue());
            }

            // restore previous mute state.
            editbox.SetMutedState(wasMuted);

            FireEvent(ValueChanged, e);
        }

        /// <summary>
        /// Method called when the step value changes.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnStepChanged(WindowEventArgs e)
        {
            FireEvent(StepChanged, e);
        }

        /// <summary>
        /// Method called when the maximum value setting changes.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnMaximumValueChanged(WindowEventArgs e)
        {
            FireEvent(MaximumValueChanged, e);

            if (_currentValue > _maxValue)
                SetCurrentValue(_maxValue);
        }

        /// <summary>
        /// Method called when the minimum value setting changes.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnMinimumValueChanged(WindowEventArgs e)
        {
            FireEvent(MinimumValueChanged, e);

            if (_currentValue < _minValue)
                SetCurrentValue(_minValue);
        }

        /// <summary>
        /// Method called when the text input/display mode is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object containing any relevant data.
        /// </param>
        protected virtual void OnTextInputModeChanged(WindowEventArgs e)
        {
            var editbox = GetEditbox();
            // update edit box text to reflect new mode.

            // mute to save doing unnecessary events work.
            var wasMuted = editbox.IsMuted();
            editbox.SetMutedState(true);

            // Update text with new value.
            editbox.SetText(GetTextFromValue());

            // restore previous mute state.
            editbox.SetMutedState(wasMuted);

            FireEvent(TextInputModeChanged, e);
        }

        protected bool HandleIncreaseButton(EventArgs e)
        {
            if (((CursorInputEventArgs)e).Source == CursorInputSource.Left)
            {
                SetCurrentValue(_currentValue + _stepSize);
                return true;
            }

            return false;
        }

        protected bool HandleDecreaseButton(EventArgs e)
        {
            if (((CursorInputEventArgs)e).Source == CursorInputSource.Left)
            {
                SetCurrentValue(_currentValue - _stepSize);
                return true;
            }

            return false;
        }

        protected bool HandleEditTextChange(EventArgs e)
        {
            // set this windows text to match
            SetText(GetEditbox().GetText());

            // update value
            SetCurrentValue(GetValueFromText());

            return true;
        }

        /// <summary>
        /// Adds properties supported by the Spinner class.
        /// </summary>
        private void AddSpinnerProperties()
        {
            DefineProperty(
                "CurrentValue",
                "Property to get/set the current value of the spinner.  Value is a float.",
                (x, v) => x.SetCurrentValue(v), x => x.GetCurrentValue(), 0.0f);

            DefineProperty(
                "StepSize",
                "Property to get/set the step size of the spinner.  Value is a float.",
                (x, v) => x.SetStepSize(v), x => x.GetStepSize(), 1.0f);

            DefineProperty(
                "MinimumValue",
                "Property to get/set the minimum value setting of the spinner.  Value is a float.",
                (x, v) => x.SetMinimumValue(v), x => x.GetMinimumValue(), Int16.MinValue);

            DefineProperty(
                "MaximumValue",
                "Property to get/set the maximum value setting of the spinner.  Value is a float.",
                (x, v) => x.SetMaximumValue(v), x => x.GetMaximumValue(), Int16.MaxValue);

            DefineProperty(
                "TextInputMode",
                "Property to get/set the TextInputMode setting for the spinner.  Value is \"FloatingPoint\", \"Integer\", \"Hexadecimal\", or \"Octal\".",
                (x, v) => x.SetTextInputMode(v), x => x.GetTextInputMode(), TextInputMode.Integer);
        }

        private void DefineProperty<T>(string name, string help, Action<Spinner, T> setter, Func<Spinner, T> getter,
                                       T defaultValue)
        {
            AddProperty(new TplWindowProperty<Spinner, T>(name, help, setter, getter, WidgetTypeName, defaultValue));
        }

        #region Fields

        /// <summary>
        /// Step size value used y the increase &amp; decrease buttons.
        /// </summary>
        private double _stepSize;

        /// <summary>
        /// Numerical copy of the text in d_editbox.
        /// </summary>
        private double _currentValue;

        /// <summary>
        /// Maximum value for spinner.
        /// </summary>
        private double _maxValue;

        /// <summary>
        /// Minimum value for spinner.
        /// </summary>
        private double _minValue;

        /// <summary>
        /// Current text display/input mode.
        /// </summary>
        private TextInputMode _inputMode;

        #endregion
    }
}