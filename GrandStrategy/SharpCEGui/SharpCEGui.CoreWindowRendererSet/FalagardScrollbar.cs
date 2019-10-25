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
    /// Scrollbar class for the FalagardBase module.
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
    ///     PushButton based widget with name suffix "__auto_incbtn__"
    ///     PushButton based widget with name suffix "__auto_decbtn__"
    /// 
    /// Property initialiser definitions:
    ///     - VerticalScrollbar - boolean property.
    ///         Indicates whether this scrollbar will operate in the vertical or
    ///         horizontal direction.  Default is for horizontal.  Optional.
    /// </summary>
    public class FalagardScrollbar : ScrollbarWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Scrollbar";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardScrollbar(string type)
            : base(type)
        {
            _vertical = false;

            RegisterProperty(new TplWindowRendererProperty<FalagardScrollbar, bool>(
                                 "VerticalScrollbar",
                                 "Property to get/set whether the Scrollbar operates in the vertical direction. Value is either \"True\" or \"False\".",
                                 (w, v) => w.SetVertical(v), w => w.IsVertical(), TypeName));
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

        public override void UpdateThumb()
        {
            var w = (Scrollbar) Window;
            var wlf = GetLookNFeel();
            var area = wlf.GetNamedArea("ThumbTrackArea").GetArea().GetPixelRect(w);

            var theThumb = w.GetThumb();

            var posExtent = w.GetDocumentSize() - w.GetPageSize();
            float slideExtent;

            if (_vertical)
            {
                slideExtent = area.Height - theThumb.GetPixelSize().Height;
                theThumb.SetVertRange(area.Top/w.GetPixelSize().Height,
                                      (area.Top + slideExtent)/w.GetPixelSize().Height);
                theThumb.SetPosition(
                    new UVector2(UDim.Absolute(area.Left),
                                 UDim.Relative((area.Top +
                                                (w.GetScrollPosition()*(slideExtent/posExtent)))/
                                               w.GetPixelSize().Height)));
            }
            else
            {
                slideExtent = area.Width - theThumb.GetPixelSize().Width;
                theThumb.SetHorzRange(area.Left/w.GetPixelSize().Width,
                                      (area.Left + slideExtent)/w.GetPixelSize().Width);
                theThumb.SetPosition(
                    new UVector2(
                        UDim.Relative((area.Left + (w.GetScrollPosition()*(slideExtent/posExtent)))/
                                      w.GetPixelSize().Width),
                        UDim.Absolute(area.Top)));
            }
        }

        public override float GetValueFromThumb()
        {
            var w = (Scrollbar) Window;
            var wlf = GetLookNFeel();
            var area = wlf.GetNamedArea("ThumbTrackArea").GetArea().GetPixelRect(w);

            var theThumb = w.GetThumb();
            var posExtent = w.GetDocumentSize() - w.GetPageSize();

            if (_vertical)
            {
                var slideExtent = area.Height - theThumb.GetPixelSize().Height;
                return (CoordConverter.AsAbsolute(theThumb.GetYPosition(), w.GetPixelSize().Height) - area.Top)/
                       (slideExtent/posExtent);
            }
            else
            {
                var slideExtent = area.Width - theThumb.GetPixelSize().Width;
                return (CoordConverter.AsAbsolute(theThumb.GetXPosition(), w.GetPixelSize().Width) - area.Left)/
                       (slideExtent/posExtent);
            }
        }

        public override float GetAdjustDirectionFromPoint(Lunatics.Mathematics.Vector2 pt)
        {
            var w = (Scrollbar) Window;
            var absrect = w.GetThumb().GetUnclippedOuterRect().Get();

            if ((_vertical && (pt.Y > absrect.Bottom)) || (!_vertical && (pt.X > absrect.Right)))
                return 1;

            if ((_vertical && (pt.Y < absrect.Top)) || (!_vertical && (pt.X < absrect.Left)))
                return -1;

            return 0;
        }

        #region Fields

        private bool _vertical; //!< True if slider operates in vertical direction.

        #endregion
    }
}