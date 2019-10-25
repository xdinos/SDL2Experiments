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
    /// ListHeaderSegment class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide the following:
    /// 
    /// States:
    ///     - Disabled
    ///     - Normal
    ///     - Hover
    ///     - SplitterHover
    ///     - DragGhost
    ///     - AscendingSortIcon
    ///     - DescendingSortDown
    ///     - GhostAscendingSortIcon
    ///     - GhostDescendingSortDown
    /// </summary>
    public class FalagardListHeaderSegment : WindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/ListHeaderSegment";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardListHeaderSegment(string type)
            : base(type)
        {

        }

        public override void CreateRenderGeometry()
        {
            var w = (ListHeaderSegment) Window;

            // get WidgetLookFeel for the assigned look.
            var wlf = GetLookNFeel();

            StateImagery imagery;

            // get imagery for main state.
            if (w.IsEffectiveDisabled())
            {
                imagery = wlf.GetStateImagery("Disabled");
            }
            else if ((w.IsSegmentHovering() != w.IsSegmentPushed()) && !w.IsSplitterHovering() && w.IsClickable())
            {
                imagery = wlf.GetStateImagery("Hover");
            }
            else if (w.IsSplitterHovering())
            {
                imagery = wlf.GetStateImagery("SplitterHover");
            }
            else
            {
                imagery = wlf.GetStateImagery("Normal");
            }

            // do main rendering
            imagery.Render(w);

            // Render sorting icon as needed
            var sortDir = w.GetSortDirection();
            if (sortDir == ListHeaderSegment.SortDirection.Ascending)
            {
                imagery = wlf.GetStateImagery("AscendingSortIcon");
                imagery.Render(w);
            }
            else if (sortDir == ListHeaderSegment.SortDirection.Descending)
            {
                imagery = wlf.GetStateImagery("DescendingSortIcon");
                imagery.Render(w);
            }

            // draw ghost copy if the segment is being dragged.
            if (w.IsBeingDragMoved())
            {
                var pixelSize = w.GetPixelSize();
                var targetArea = new Rectf(0, 0, pixelSize.Width, pixelSize.Height);
                targetArea.Offset(w.GetDragMoveOffset());
                imagery = wlf.GetStateImagery("DragGhost");
                imagery.Render(w, targetArea);

                // Render sorting icon as needed
                if (sortDir == ListHeaderSegment.SortDirection.Ascending)
                {
                    imagery = wlf.GetStateImagery("GhostAscendingSortIcon");
                    imagery.Render(w, targetArea);
                }
                else if (sortDir == ListHeaderSegment.SortDirection.Descending)
                {
                    imagery = wlf.GetStateImagery("GhostDescendingSortIcon");
                    imagery.Render(w, targetArea);
                }
            }
        }
    }
}