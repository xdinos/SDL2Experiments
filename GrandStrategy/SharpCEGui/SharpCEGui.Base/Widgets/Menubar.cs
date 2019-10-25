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
    /// Base class for menu bars.
    /// </summary>
    public class Menubar : MenuBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "Menubar";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/Menubar";

        /// <summary>
        /// Constructor for Menubar objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public Menubar(string type, string name)
            : base(type, name)
        {
            ItemSpacing = 10;
        }

        /// <summary>
        /// Setup size and position for the item widgets attached to this Menubar
        /// </summary>
        protected override void LayoutItemWidgets()
        {
            var renderRect = GetItemRenderArea();
            var x0 = CoordConverter.AlignToPixels(renderRect.Left);

            foreach (var item in ListItems)
            {
                var optimal = item.GetItemPixelSize();

                item.SetVerticalAlignment(VerticalAlignment.Centre);
                var rect = new URect
                               {
                                   Position = new UVector2(UDim.Absolute(x0), UDim.Absolute(0)),
                                   Size = new USize(UDim.Absolute(CoordConverter.AlignToPixels(optimal.Width)),
                                                    UDim.Absolute(CoordConverter.AlignToPixels(optimal.Height)))
                               };

                item.SetArea(rect);

                x0 += optimal.Width + ItemSpacing;
            }
        }

        /// <summary>
        /// Returns the Size in unclipped pixels of the content attached to this ItemListBase that is attached to it.
        /// </summary>
        /// <returns>
        /// Size object describing in unclipped pixels the size of the content ItemEntries attached to this menu.
        /// </returns>
        protected override Sizef GetContentSize()
        {
            // find the content sizes
            var tallest = 0f;
            var totalWidth = 0f;

            var i = 0;
            var max = ListItems.Count;
            while (i < max)
            {
                var sz = ListItems[i].GetItemPixelSize();
                if (sz.Height > tallest)
                    tallest = sz.Height;
                totalWidth += sz.Width;

                i++;
            }

            var count = (float) i;

            // horz item spacing
            if (count >= 2)
            {
                totalWidth += (count - 1)*ItemSpacing;
            }

            // return the content size
            return new Sizef(totalWidth, tallest);
        }
    }
}