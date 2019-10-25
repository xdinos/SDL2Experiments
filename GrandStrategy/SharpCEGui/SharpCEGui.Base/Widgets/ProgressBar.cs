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
    /// Base class for progress bars.
    /// </summary>
    public class ProgressBar : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ProgressBar";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ProgressBar";

        #region Events

        /// <summary>
        /// Event fired whenever the progress value is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ProgressBar whose value has been
        /// changed.
        /// </summary>
	    public event EventHandler<WindowEventArgs> ProgressChanged;

        /// <summary>
        /// Event fired when the progress bar's value reaches 100%.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ProgressBar whose progress value
        /// has reached 100%.
        /// </summary>
	    public event EventHandler<WindowEventArgs> ProgressDone;

        #endregion

        /// <summary>
        /// return the current progress value
        /// </summary>
        /// <returns></returns>
	    public float GetProgress()
	    {
	        return _progress;
	    }

        /// <summary>
        /// return the current step size
        /// </summary>
        /// <returns></returns>
	    public float GetStepSize()
	    {
	        return _step;
	    }

        /// <summary>
        /// set the current progress.
        /// </summary>
        /// <param name="progress">
        /// The level of progress to set.  
        /// If this value is >1.0f (100%) progress will be limited to 1.0f.
        /// </param>
        public void SetProgress(float progress)
        {
            // legal progress rangeis : 0.0f <= progress <= 1.0f
            progress = (progress < 0.0f) ? 0.0f : (progress > 1.0f) ? 1.0f : progress;

            if (progress != _progress)
            {
                // update progress and fire off event.
                _progress = progress;
                var args = new WindowEventArgs(this);
                OnProgressChanged(args);

                // if new progress is 100%, fire off the 'done' event as well.
                if (_progress == 1.0f)
                {
                    OnProgressDone(args);
                }
            }
        }

        /// <summary>
        /// set the size of the 'step' in percentage points (default is 0.01f or 1%).
        /// </summary>
        /// <param name="value">
        /// Amount to increase the progress by each time the step method is called.
        /// </param>
	    public void SetStepSize(float value)
	    {
	        _step = value;
	    }

        /// <summary>
        /// cause the progress to step
        /// <para>
        /// The amount the progress bar will step can be changed by calling the setStepSize method.  
        /// The default step size is 0.01f which is equal to 1%.
        /// </para>
        /// </summary>
	    public void Step()
	    {
	        SetProgress(_progress + _step);
	    }

        /// <summary>
        /// Modify the progress level by a specified delta.
        /// </summary>
        /// <param name="delta">
        /// amount to adjust the progress by.  
        /// Whatever this value is, the progress of the bar will be kept
        /// within the range: 0.0f lt;= progress lt;= 1.0f.
        /// </param>
	    public void AdjustProgress(float delta)
	    {
	        SetProgress(_progress + delta);
	    }

        /// <summary>
        /// Constructor for ProgressBar class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
	    public ProgressBar(string type, string name):base(type,name)
        {
            _progress = 0f;
            _step = 0.01f;
            AddProgressBarProperties();
	    }

        /// <summary>
        /// event triggered when progress changes
        /// </summary>
        /// <param name="e"></param>
	    protected virtual void OnProgressChanged(WindowEventArgs e)
	    {
            Invalidate(false);
            FireEvent(ProgressChanged, e);
	    }

        /// <summary>
        /// event triggered when progress reaches 100%
        /// </summary>
        /// <param name="e"></param>
	    protected virtual void OnProgressDone(WindowEventArgs e)
	    {
            FireEvent(ProgressDone, e);
	    }
        
	    private void AddProgressBarProperties()
	    {
            // TODO: Inconsistency
	        AddProperty(new TplWindowProperty<ProgressBar, float>(
	                        "CurrentProgress",
	                        "Property to get/set the current progress of the progress bar.  Value is a float  value between 0.0 and 1.0 specifying the progress.",
	                        (x, v) => x.SetProgress(v), x => x.GetProgress(), WidgetTypeName));

	        AddProperty(new TplWindowProperty<ProgressBar, float>(
	                        "StepSize",
	                        "Property to get/set the step size setting for the progress bar.  Value is a float value.",
	                        (x, v) => x.SetStepSize(v), x => x.GetStepSize(), WidgetTypeName));
	    }

        #region Fields

        /// <summary>
        /// current progress (from 0.0f to 1.0f)
        /// </summary>
        private float _progress;

        /// <summary>
        /// amount to 'step' progress by on a call to step()
        /// </summary>
        private float _step;

        #endregion
    }
}