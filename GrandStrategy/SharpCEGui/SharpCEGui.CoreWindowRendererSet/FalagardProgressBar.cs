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
    /// ProgressBar class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    ///     - EnabledProgress
    ///     - DisabledProgress
    /// 
    /// Named Areas:
    ///     - ProgressArea
    /// 
    /// Property initialiser definitions:
    ///     - VerticalProgress - boolean property.
    ///       Determines whether the progress widget is horizontal or vertical.
    ///       Default is horizontal.  Optional.
    /// 
    ///     - ReversedProgress - boolean property.
    ///       Determines whether the progress grows in the opposite direction to
    ///       what is considered 'usual'.  Set to "True" to have progress grow
    ///       towards the left or bottom of the progress area.  Optional.
    /// </summary>
    public class FalagardProgressBar : WindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/ProgressBar";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardProgressBar(string type)
            : base(type, ProgressBar.EventNamespace)
        {
            _vertical = false;
            _reversed = false;

            RegisterProperty(new TplWindowRendererProperty<FalagardProgressBar, bool>(
                                 "VerticalProgress",
                                 "Property to get/set whether the ProgressBar operates in the vertical direction. Value is either \"True\" or \"False\".",
                                 (x, v) => x.SetVertical(v), x => x.IsVertical(), TypeName));

            RegisterProperty(new TplWindowRendererProperty<FalagardProgressBar, bool>(
                                 "ReversedProgress",
                                 "Property to get/set whether the ProgressBar operates in reversed direction. Value is either \"True\" or \"False\".",
                                 (x, v) => x.SetReversed(v), x => x.IsReversed(), TypeName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsVertical()
        {
            return _vertical;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsReversed()
        {
            return _reversed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public void SetVertical(bool setting)
        {
            _vertical = setting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public void SetReversed(bool setting)
        {
            _reversed = setting;
        }

        public override void CreateRenderGeometry()
        {
            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            // try and get imagery for our current state
            var imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "Disabled" : "Enabled");
            // peform the rendering operation.
            imagery.Render(Window);

            // get imagery for actual progress rendering
            imagery = wlf.GetStateImagery(Window.IsEffectiveDisabled() ? "DisabledProgress" : "EnabledProgress");

            // get target rect for this imagery
            var progressRect = wlf.GetNamedArea("ProgressArea").GetArea().GetPixelRect(Window);

            // calculate a clipper according to the current progress.
            var progressClipper = progressRect;

            var w = (ProgressBar) Window;
            if (_vertical)
            {
                float height = CoordConverter.AlignToPixels(progressClipper.Height*w.GetProgress());

                if (_reversed)
                {
                    progressClipper.Height = height;
                }
                else
                {
                    progressClipper.Top = progressClipper.Bottom - height;
                }
            }
            else
            {
                var width = CoordConverter.AlignToPixels(progressClipper.Width*w.GetProgress());

                if (_reversed)
                {
                    progressClipper.Left = progressClipper.Right - width;
                }
                else
                {
                    progressClipper.Width = width;
                }
            }

            // peform the rendering operation.
            imagery.Render(Window, progressRect, null, progressClipper);
        }

        // settings to make this class universal.

        /// <summary>
        /// True if progress bar operates on the vertical plane.
        /// </summary>
        private bool _vertical;

        /// <summary>
        /// True if progress grows in the opposite direction to usual (i.e. to the left / downwards).
        /// </summary>
        private bool _reversed;
    }
}