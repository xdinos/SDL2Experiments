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

using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Button class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States (missing states will default to 'Normal'):
    ///     - Normal    - Rendering for when the button is neither pushed or has the mouse hovering over it.
    ///     - Hover     - Rendering for then the button has the mouse hovering over it.
    ///     - Pushed    - Rendering for when the button is pushed and mouse is over it.
    ///     - PushedOff - Rendering for when the button is pushed and mouse is not over it.
    ///     - Disabled  - Rendering for when the button is disabled.
    /// </summary>
    public class FalagardButton : WindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Button";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardButton(string type) : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            var w = (ButtonBase)Window;
            var wlf = GetLookNFeel();

            var norm = false;
            string state;

		    if (w.IsEffectiveDisabled())
		    {
		        state = "Disabled";
		    }
		    else if (w.IsPushed())
		    {
                state = w.IsHovering() ? "Pushed" : "PushedOff";
		    }
            else if (w.IsHovering())
		    {
		        state = "Hover";
		    }
		    else
		    {
		        state = "Normal";
		        norm = true;
		    }

            if (!norm && !wlf.IsStateImageryPresent(ActualStateName(state)))
            {
                state = "Normal";
            }

            wlf.GetStateImagery(ActualStateName(state)).Render(w);
        }

        protected virtual string ActualStateName(string name)
        {
            return name;
        }
    }
}