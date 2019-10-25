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
    /// Slider class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Enabled
    ///     - Disabled
    /// 
    /// Named Areas:
    ///     - ThumbTrackArea
    /// 
    /// Child Widgets:
    ///     Thumb based widget with name suffix "__auto_thumb__"
    /// 
    /// Property initialiser definitions:
    ///     - VerticalSlider - boolean property.
    ///       Indicates whether this slider will operate in the vertical or
    ///       horizontal direction.  Default is for horizontal.  Optional.
    /// </summary>
    public class FalagardSlider : SliderWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Slider";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardSlider(string type)
            : base(type)
        {
            _vertical = false;
            _reversed = false;

            RegisterProperty(new TplWindowRendererProperty<FalagardSlider, bool>(
                                 "VerticalSlider",
                                 "Property to get/set whether the Slider operates in the vertical direction. Value is either \"True\" or \"False\".",
                                 (x, v) => x.SetVertical(v), x => x.IsVertical(), TypeName));

            RegisterProperty(new TplWindowRendererProperty<FalagardSlider, bool>(
                                 "ReversedDirection",
                                 "Property to get/set whether the Slider operates in reversed direction. Value is either \"True\" or \"False\".",
                                 (x, v) => x.SetReversedDirection(v), x => x.IsReversedDirection(), TypeName));
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
        /// <param name="setting"></param>
        public void SetVertical(bool setting)
        {
            _vertical = setting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsReversedDirection()
        {
            return _reversed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public void SetReversedDirection(bool setting)
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
        }

        public override void PerformChildWindowLayout()
        {
            UpdateThumb();
        }


        // overridden from Slider base class.
        public override void UpdateThumb()
        {
            var w = (Slider) Window;

            // get area the thumb is supposed to use as it's area.
            var wlf = GetLookNFeel();
            var area = wlf.GetNamedArea("ThumbTrackArea").GetArea().GetPixelRect(w);

            // get accesss to the thumb
            var theThumb = w.GetThumb();

            var pixelSize = w.GetPixelSize();

            var thumbRelXPos = pixelSize.Width == 0.0f ? 0.0f : (area.Left/pixelSize.Width);
            var thumbRelYPos = pixelSize.Height == 0.0f ? 0.0f : (area.Top/pixelSize.Height);

            // get base location for thumb widget
            var thumbPosition = new UVector2(UDim.Relative(thumbRelXPos), UDim.Relative(thumbRelYPos));

            // Is this a vertical slider
            if (_vertical)
            {
                // pixel extent of total available area the thumb moves in
                var slideExtent = area.Height - theThumb.GetPixelSize().Height;

                // Set range of motion for the thumb widget
                if (pixelSize.Height != 0.0f)
                    theThumb.SetVertRange(area.Top/pixelSize.Height, (area.Top + slideExtent)/pixelSize.Height);
                else
                    theThumb.SetVertRange(0.0f, 0.0f);

                // calculate vertical positon for thumb
                var thumbOffset = w.GetCurrentValue()*(slideExtent/w.GetMaxValue());

                if (pixelSize.Height != 0.0f)
                    thumbPosition.d_y.d_scale += (_reversed ? thumbOffset : slideExtent - thumbOffset)/
                                                 pixelSize.Height;
            }
                // Horizontal slider
            else
            {
                // pixel extent of total available area the thumb moves in
                var slideExtent = area.Width - theThumb.GetPixelSize().Width;

                // Set range of motion for the thumb widget
                if (pixelSize.Width != 0.0f)
                    theThumb.SetHorzRange(area.Left/pixelSize.Width, (area.Left + slideExtent)/pixelSize.Width);
                else
                    theThumb.SetHorzRange(0.0f, 0.0f);


                // calculate horizontal positon for thumb
                var thumbOffset = w.GetCurrentValue()*(slideExtent/w.GetMaxValue());

                if (pixelSize.Width != 0.0f)
                    thumbPosition.d_x.d_scale += (_reversed ? slideExtent - thumbOffset : thumbOffset)/pixelSize.Width;
            }

            // set new position for thumb.
            theThumb.SetPosition(thumbPosition);
        }

        public override float GetValueFromThumb()
        {
            var w = (Slider) Window;

            // get area the thumb is supposed to use as it's area.
            var wlf = GetLookNFeel();
            var area = wlf.GetNamedArea("ThumbTrackArea").GetArea().GetPixelRect(w);
            // get accesss to the thumb
            var theThumb = w.GetThumb();

            // slider is vertical
            if (_vertical)
            {
                // pixel extent of total available area the thumb moves in
                var slideExtent = area.Height - theThumb.GetPixelSize().Height;
                // calculate value represented by current thumb position
                var thumbValue = (CoordConverter.AsAbsolute(theThumb.GetYPosition(), w.GetPixelSize().Height) -
                                  area.Top)/(slideExtent/w.GetMaxValue());
                // return final thumb value according to slider settings
                return _reversed ? thumbValue : w.GetMaxValue() - thumbValue;
            }
                // slider is horizontal
            else
            {
                // pixel extent of total available area the thumb moves in
                var slideExtent = area.Width - theThumb.GetPixelSize().Width;
                // calculate value represented by current thumb position
                var thumbValue = (CoordConverter.AsAbsolute(theThumb.GetXPosition(), w.GetPixelSize().Width) -
                                  area.Left)/(slideExtent/w.GetMaxValue());
                // return final thumb value according to slider settings
                return _reversed ? w.GetMaxValue() - thumbValue : thumbValue;
            }
        }

        public override float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt)
        {
            var w = (Slider) Window;
            var absrect = w.GetThumb().GetUnclippedOuterRect().Get();

            if ((_vertical && (pt.Y < absrect.Top)) || (!_vertical && (pt.X > absrect.Right)))
                return _reversed ? -1.0f : 1.0f;

            if ((_vertical && (pt.Y > absrect.Bottom)) || (!_vertical && (pt.X < absrect.Left)))
                return _reversed ? 1.0f : -1.0f;

            return 0;
        }

        #region Fields

        /// <summary>
        /// True if slider operates in vertical direction.
        /// </summary>
        private bool _vertical;

        /// <summary>
        /// true if slider operates in reversed direction to 'normal'.
        /// </summary>
        private bool _reversed;

        #endregion
    }
}