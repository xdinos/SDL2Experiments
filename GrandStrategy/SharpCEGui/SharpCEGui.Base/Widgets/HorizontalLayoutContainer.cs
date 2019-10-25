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
    /// A Layout Container window layouting it's children Horizontally
    /// </summary>
    public class HorizontalLayoutContainer : SequentialLayoutContainer
    {
        /// <summary>
        /// The unique typename of this widget
        /// </summary>
        public const string WidgetTypeName = "HorizontalLayoutContainer";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public HorizontalLayoutContainer(string type, string name)
            : base(type, name)
        {

        }

        public override void Layout()
        {
            LayoutIcehole();
            //LayoutCeGui();
        }

        private void LayoutIcehole()
        {
            // used to compare UDims
            var absHeight = GetChildContentArea().Get().Height;

            // this is where we store the left offset
            // we continually increase this number as we go through the windows
            var leftOffset = UDim.Zero;
            var layoutHeight = UDim.Zero;

            // Calculate child sizes
            foreach (Window window in d_children)
            {
                if (!window.IsVisible()) continue;

                var offset = GetOffsetForWindow(window);
                var boundingSize = GetBoundingSizeForWindow(window);

                // full child window width, including margins
                var childHeight = boundingSize.d_y;

                if (CoordConverter.AsAbsolute(layoutHeight, absHeight) < 
                    CoordConverter.AsAbsolute(childHeight, absHeight))
                {
                    layoutHeight = childHeight;
                }

                window.SetPosition(offset + new UVector2(leftOffset, UDim.Zero));
                leftOffset += boundingSize.d_x;
            }

            
            SetSize(new USize(leftOffset, layoutHeight));

            leftOffset = UDim.Zero;

            foreach (Window window in d_children)
            {
                if (!window.IsVisible()) continue;
                var offset = GetOffsetForWindow(window);
                var boundingSize = GetBoundingSizeForWindow(window);

                window.SetPosition(offset + new UVector2(leftOffset, UDim.Zero));
                leftOffset += boundingSize.d_x;
            }
        }

        private void LayoutCeGui()
        {
            // used to compare UDims
            var absHeight = GetChildContentArea().Get().Height;

            // this is where we store the left offset
            // we continually increase this number as we go through the windows
            var leftOffset = UDim.Zero;
            var layoutHeight = UDim.Zero;

            foreach (Window window in d_children)
            {
                //Window* window = static_cast<Window*>(*it);

                var offset = GetOffsetForWindow(window);
                var boundingSize = GetBoundingSizeForWindow(window);

                // full child window width, including margins
                var childHeight = boundingSize.d_y;

                if (CoordConverter.AsAbsolute(layoutHeight, absHeight) <
                    CoordConverter.AsAbsolute(childHeight, absHeight))
                {
                    layoutHeight = childHeight;
                }

                window.SetPosition(offset + new UVector2(leftOffset, UDim.Zero));
                leftOffset += boundingSize.d_x;
            }

            SetSize(new USize(leftOffset, layoutHeight));
        }
    }
}