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

using System.Linq;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// A Layout Container window layouting it's children vertically
    /// </summary>
    public class VerticalLayoutContainer : SequentialLayoutContainer
    {
        /// <summary>
        /// The unique typename of this widget
        /// </summary>
        public const string WidgetTypeName = "VerticalLayoutContainer";

        /// <summary>
        /// Constructor for GUISheet windows.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public VerticalLayoutContainer(string type, string name)
            : base(type, name)
        {

        }

        public override void Layout()
        {
            LayoutIcehole();
            //LayoutCeGui();
        }

        public void LayoutIcehole()
        {
            // used to compare UDims
            var absWidth = GetChildContentArea().Get().Width;

            // this is where we store the top offset
            // we continually increase this number as we go through the windows
            var topOffset = UDim.Zero;
            var layoutWidth = UDim.Zero;

            foreach (var window in d_children.Cast<Window>())
            {
                var offset = GetOffsetForWindow(window);
                var boundingSize = GetBoundingSizeForWindow(window);

                // full child window width, including margins
                var childWidth = boundingSize.d_x;

                if (CoordConverter.AsAbsolute(layoutWidth, absWidth) < CoordConverter.AsAbsolute(childWidth, absWidth))
                {
                    layoutWidth = childWidth;
                }

                window.SetPosition(offset + new UVector2(UDim.Zero, topOffset));
                topOffset += boundingSize.d_y;
            }

            //SetSize(new USize(layoutWidth, topOffset));
            SetHeight(topOffset);
        }

        public void LayoutCeGui()
        {
            // used to compare UDims
            var absWidth = GetChildContentArea().Get().Width;

            // this is where we store the top offset
            // we continually increase this number as we go through the windows
            var topOffset = UDim.Zero;
            var layoutWidth = UDim.Zero;

            foreach (var window in d_children.Cast<Window>())
            {
                var offset = GetOffsetForWindow(window);
                var boundingSize = GetBoundingSizeForWindow(window);

                // full child window width, including margins
                var childWidth = boundingSize.d_x;

                if (CoordConverter.AsAbsolute(layoutWidth, absWidth) < CoordConverter.AsAbsolute(childWidth, absWidth))
                {
                    layoutWidth = childWidth;
                }

                window.SetPosition(offset + new UVector2(UDim.Zero, topOffset));
                topOffset += boundingSize.d_y;
            }

            SetSize(new USize(layoutWidth, topOffset));
            //SetHeight(topOffset);
        }
    }
}