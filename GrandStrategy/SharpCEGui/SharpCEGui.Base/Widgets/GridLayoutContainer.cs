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
using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// A Layout Container window layouting it's children into a grid
    /// </summary>
    public class GridLayoutContainer : LayoutContainer
    {
        /// <summary>
        /// enumerates auto positioning methods for the grid - these allow you to
        /// fill the grid without specifying gridX and gridY positions for each
        /// addChild.
        /// </summary>
        public enum AutoPositioning
        {
            /// <summary>
            /// no auto positioning!
            /// </summary>
            Disabled,

            /// <summary>
            /// Left to right positioning:
            ///  - 1 2 3
            ///  - 4 5 6
            /// </summary>
            LeftToRight,

            /// <summary>
            /// Top to bottom positioning
            ///  - 1 3 5
            ///  - 2 4 6
            /// </summary>
            TopToBottom
        }

        /// <summary>
        /// The unique typename of this widget
        /// </summary>
        public const string WidgetTypeName = "GridLayoutContainer";

        /// <summary>
        /// Widget name for dummies.
        /// </summary>
        public const string DummyName = "__auto_dummy_";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "GridLayoutContainer";

        public const string EventChildOrderChanged = "ChildOrderChanged";

        /// <summary>
        /// fired when child windows get rearranged
        /// </summary>
        public event GuiEventHandler<EventArgs> ChildOrderChanged
        {
            add { SubscribeEvent(EventChildOrderChanged, value); }
            remove { UnsubscribeEvent(EventChildOrderChanged, value); }
        }

        #endregion

        /// <summary>
        /// Constructor for GUISheet windows.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public GridLayoutContainer(string type, string name)
            : base(type, name)
        {
            _gridWidth = 0;
            _gridHeight = 0;

            _autoPositioning = AutoPositioning.LeftToRight;
            _nextAutoPositioningIdx = 0;

            _nextGridX = Int32.MaxValue;
            _nextGridY = Int32.MaxValue;

            _nextDummyIdx = 0;

            // grid size is 0x0 that means 0 child windows,
            // no need to populate d_children with dummies

            AddGridLayoutContainerProperties();
        }

        /// <summary>
        /// Sets grid's dimensions.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetGridDimensions(int width, int height)
        {
            // copy the old children list
            var oldChildren = new List<Element>(d_children);

            // remove all child windows
            while (GetChildCount() != 0)
            {
                var wnd = (Window) d_children[0];
                RemoveChild(wnd);
            }

            // we simply fill the grid with dummies to ensure everything works smoothly
            // when something is added to the grid, it simply replaces the dummy
            for (var i = 0; i < width*height; ++i)
            {
                var dummy = CreateDummy();
                AddChild(dummy);
            }

            var oldWidth = _gridWidth;

            var oldHeight = _gridHeight;

            var oldAO = _autoPositioning;

            _gridWidth = width;

            _gridHeight = height;

            // now we have to map oldChildren to new children
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    // we have to skip if we are out of the old grid
                    if (x >= oldWidth || y >= oldHeight)
                        continue;

                    var oldIdx = MapFromGridToIdx(x, y, oldWidth, oldHeight);
                    var previous = (Window) oldChildren[oldIdx];

                    if (IsDummy(previous))
                    {
                        WindowManager.GetSingleton().DestroyWindow(previous);
                    }
                    else
                    {
                        AddChildToPosition(previous, x, y);
                    }

                    oldChildren[oldIdx] = null;
                }
            }

            SetAutoPositioning(oldAO);
            // oldAOIdx could mean something completely different now!
            // todo: perhaps convert oldAOOdx to new AOIdx?
            SetNextAutoPositioningIdx(0);

            // we have to destroy windows that don't fit the new grid if they are set
            // to be destroyed by parent
            foreach (var item in oldChildren)
            {
                if (item != null && ((Window) item).IsDestroyedByParent())
                {
                    WindowManager.GetSingleton().DestroyWindow((Window) item);
                }
            }
        }

        /// <summary>
        /// Sets grid's dimensions.
        /// </summary>
        /// <param name="size"></param>
        public void SetGrid(Sizef size)
        {
            SetGridDimensions((int) Math.Ceiling(Math.Max(0.0f, size.Width)),
                              (int) Math.Ceiling(Math.Max(0.0f, size.Height)));
        }

        /// <summary>
        /// Retrieves grid width, the amount of cells in one row
        /// </summary>
        /// <returns></returns>
        public int GetGridWidth()
        {
            return _gridWidth;
        }

        /// <summary>
        /// Retrieves grid height, the amount of rows in the grid
        /// </summary>
        /// <returns></returns>
        public int GetGridHeight()
        {
            return _gridHeight;
        }

        /// <summary>
        /// Retrieves grid width, the amount of cells in one row
        /// </summary>
        /// <returns></returns>
        public Sizef GetGrid()
        {
            return new Sizef(GetGridWidth(), GetGridHeight());
        }

        /// <summary>
        /// Sets new auto positioning method.
        /// <para>
        /// The newly set auto positioning sequence will start over!
        /// Use setAutoPositioningIdx to set it's starting point
        /// </para>
        /// </summary>
        /// <param name="positioning"></param>
        public void SetAutoPositioning(AutoPositioning positioning)
        {
            _autoPositioning = positioning;
            _nextAutoPositioningIdx = 0;
        }

        /// <summary>
        /// Retrieves current auto positioning method.
        /// </summary>
        /// <returns></returns>
        public AutoPositioning GetAutoPositioning()
        {
            return _autoPositioning;
        }

        /// <summary>
        /// Sets the next auto positioning "sequence position", this will be used
        /// next time when addChild is called.
        /// </summary>
        /// <param name="idx"></param>
        public void SetNextAutoPositioningIdx(int idx)
        {
            _nextAutoPositioningIdx = idx;
        }

        /// <summary>
        /// Retrieves auto positioning "sequence position", this will be used next
        /// time when addChild is called.
        /// </summary>
        /// <returns></returns>
        public int GetNextAutoPositioningIdx()
        {
            return _nextAutoPositioningIdx;
        }

        /// <summary>
        /// Skips given number of cells in the auto positioning sequence
        /// </summary>
        /// <param name="cells"></param>
        public void AutoPositioningSkipCells(int cells)
        {
            SetNextAutoPositioningIdx(GetNextAutoPositioningIdx() + cells);
        }

        /// <summary>
        /// Add the specified Window to specified grid position as a child of
        /// this Grid Layout Container.  If the Window \a window is already
        /// attached to a Window, it is detached before being added to this Window.
        /// <para>
        /// If something is already in given grid cell, it gets removed!
        /// </para>
        /// <para>
        /// This disabled auto positioning from further usage! You need to call
        /// setAutoPositioning(..) to set it back to your desired value and use
        /// setAutoPositioningIdx(..) to set it's starting point back
        /// </para>
        /// </summary>
        /// <param name="window"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <seealso cref="Window.AddChild"/>
        public void AddChildToPosition(Window window, int gridX, int gridY)
        {
            // when user starts to add windows to specific locations, AO has to be disabled
            SetAutoPositioning(AutoPositioning.Disabled);
            _nextGridX = gridX;
            _nextGridY = gridY;

            AddChild(window);
        }

        /// <summary>
        /// Retrieves child window that is currently at given grid position
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public Window GetChildAtPosition(int gridX, int gridY)
        {
            global::System.Diagnostics.Debug.Assert(gridX < _gridWidth, "out of bounds");
            global::System.Diagnostics.Debug.Assert(gridY < _gridHeight, "out of bounds");

            return GetChildAtIdx(MapFromGridToIdx(gridX, gridY, _gridWidth, _gridHeight));
        }

        /// <summary>
        /// Removes the child window that is currently at given grid position
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <seealso cref="Window.RemoveChild(Element)"/>
        public void RemoveChildFromPosition(int gridX, int gridY)
        {
            RemoveChild(GetChildAtPosition(gridX, gridY));
        }

        /// <summary>
        /// Swaps positions of 2 windows given by their index
        /// <para>
        /// For advanced users only!
        /// </para>
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public virtual void SwapChildPositions(int wnd1, int wnd2)
        {
            if (wnd1 < d_children.Count && wnd2 < d_children.Count)
            {
                var tmp = d_children[wnd2];
                d_children[wnd2] = d_children[wnd1];
                d_children[wnd1] = tmp;

                OnChildOrderChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Swaps positions of 2 windows given by grid positions
        /// </summary>
        /// <param name="gridX1"></param>
        /// <param name="gridY1"></param>
        /// <param name="gridX2"></param>
        /// <param name="gridY2"></param>
        public void SwapChildPositions(int gridX1, int gridY1, int gridX2, int gridY2)
        {
            SwapChildPositions(MapFromGridToIdx(gridX1, gridY1, _gridWidth, _gridHeight),
                               MapFromGridToIdx(gridX2, gridY2, _gridWidth, _gridHeight));
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(Window wnd1, Window wnd2)
        {
            SwapChildPositions(GetIdxOfChild(wnd1), GetIdxOfChild(wnd2));
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(Window wnd1, String wnd2)
        {
            SwapChildren(wnd1, GetChild(wnd2));
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(String wnd1, Window wnd2)
        {
            SwapChildren(GetChild(wnd1), wnd2);
        }

        /// <summary>
        /// Moves given child window to given grid position
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        public void MoveChildToPosition(Window wnd, int gridX, int gridY)
        {
            RemoveChild(wnd);
            AddChildToPosition(wnd, gridX, gridY);
        }

        /// <summary>
        /// Moves named child window to given grid position
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        public void MoveChildToPosition(String wnd, int gridX, int gridY)
        {
            MoveChildToPosition(GetChild(wnd), gridX, gridY);
        }

        public override void Layout()
        {
            var colSizes = new List<UDim>(Enumerable.Repeat(UDim.Zero, _gridWidth));
            var rowSizes = new List<UDim>(Enumerable.Repeat(UDim.Zero, _gridHeight));

            // used to compare UDims
            var absWidth = GetChildContentArea().Get().Width;
            var absHeight = GetChildContentArea().Get().Height;

            // first, we need to determine rowSizes and colSizes, this is needed before
            // any layouting work takes place
            for (var y = 0; y < _gridHeight; ++y)
            {
                for (var x = 0; x < _gridWidth; ++x)
                {
                    // x and y is the position of window in the grid
                    var childIdx = MapFromGridToIdx(x, y, _gridWidth, _gridHeight);

                    var window = GetChildAtIdx(childIdx);
                    var size = GetBoundingSizeForWindow(window);

                    if (CoordConverter.AsAbsolute(colSizes[x], absWidth) <
                        CoordConverter.AsAbsolute(size.d_x, absWidth))
                    {
                        colSizes[x] = size.d_x;
                    }

                    if (CoordConverter.AsAbsolute(rowSizes[y], absHeight) <
                        CoordConverter.AsAbsolute(size.d_y, absHeight))
                    {
                        rowSizes[y] = size.d_y;
                    }
                }
            }

            // OK, now in rowSizes[y] is the height of y-th row
            //         in colSizes[x] is the width of x-th column

            // second layouting phase starts now
            for (var y = 0; y < _gridHeight; ++y)
            {
                for (var x = 0; x < _gridWidth; ++x)
                {
                    // x and y is the position of window in the grid
                    var childIdx = MapFromGridToIdx(x, y, _gridWidth, _gridHeight);
                    var window = GetChildAtIdx(childIdx);
                    var offset = GetOffsetForWindow(window);
                    var gridCellOffset = GetGridCellOffset(colSizes, rowSizes, x, y);

                    window.SetPosition(gridCellOffset + offset);
                }
            }

            // now we just need to determine the total width and height and set it
            SetSize(GetGridSize(colSizes, rowSizes));
        }

        /// <summary>
        /// Handler called when children of this window gets rearranged in any way
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object whose 'window' field is set this layout container.
        /// </param>
        protected virtual void OnChildOrderChanged(WindowEventArgs e)
        {
            MarkNeedsLayouting();
            FireEvent(EventChildOrderChanged, e, EventNamespace);
        }

        //! converts from grid cell position to idx
        protected int MapFromGridToIdx(int gridX, int gridY, int gridWidth, int gridHeight)
        {
            // example:
            // d_children = {1, 2, 3, 4, 5, 6}
            // grid is 3x2
            // 1 2 3
            // 4 5 6

            global::System.Diagnostics.Debug.Assert(gridX < gridWidth);
            global::System.Diagnostics.Debug.Assert(gridY < gridHeight);

            return gridY*gridWidth + gridX;
        }

        /// <summary>
        /// converts from idx to grid cell position
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridWidth"></param>
        /// <param name="gridHeight"></param>
        protected void MapFromIdxToGrid(int idx, out int gridX, out int gridY, int gridWidth, int gridHeight)
        {
            gridY = 0;

            while (idx >= gridWidth)
            {
                idx -= gridWidth;
                ++gridY;
            }

            global::System.Diagnostics.Debug.Assert(gridY < gridHeight);

            gridX = idx;
        }


        /// <summary>
        /// calculates grid cell offset (relative to position of this layout container)
        /// </summary>
        /// <param name="colSizes"></param>
        /// <param name="rowSizes"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        protected UVector2 GetGridCellOffset(List<UDim> colSizes, List<UDim> rowSizes, int gridX, int gridY)
        {
            global::System.Diagnostics.Debug.Assert(gridX < _gridWidth);
            global::System.Diagnostics.Debug.Assert(gridY < _gridHeight);

            var ret = UVector2.Zero;

            for (var i = 0; i < gridX; ++i)
                ret.d_x += colSizes[i];

            for (var i = 0; i < gridY; ++i)
                ret.d_y += rowSizes[i];

            return ret;
        }

        /// <summary>
        /// calculates total grid size
        /// </summary>
        /// <param name="colSizes"></param>
        /// <param name="rowSizes"></param>
        /// <returns></returns>
        protected USize GetGridSize(List<UDim> colSizes, List<UDim> rowSizes)
        {
            var ret = USize.Zero;

            foreach (var t in colSizes)
                ret.d_width += t;

            foreach (var t in rowSizes)
                ret.d_height += t;

            return ret;
        }

        /// <summary>
        /// translates auto positioning index to absolute grid index
        /// </summary>
        /// <param name="apIdx"></param>
        /// <returns></returns>
        protected int TranslateAutoPositionToGridIdx(int apIdx)
        {
            // todo: more auto positioning variants? will someone use them?

            if (_autoPositioning == AutoPositioning.Disabled)
            {
                global::System.Diagnostics.Debug.Assert(false);
            }
            else if (_autoPositioning == AutoPositioning.LeftToRight)
            {
                // this is the same positioning as implementation
                return apIdx;
            }
            else if (_autoPositioning == AutoPositioning.TopToBottom)
            {
                // we want
                // 1 3 5
                // 2 4 6

                int x, y = 0;
                var done = false;

                for (x = 0; x < _gridWidth; ++x)
                {
                    for (y = 0; y < _gridHeight; ++y)
                    {
                        if (apIdx == 0)
                        {
                            done = true;
                            break;
                        }

                        --apIdx;
                    }

                    if (done)
                    {
                        break;
                    }
                }

                global::System.Diagnostics.Debug.Assert(apIdx == 0);
                return MapFromGridToIdx(x, y, _gridWidth, _gridHeight);
            }

            // should never happen
            global::System.Diagnostics.Debug.Assert(false);
            return apIdx;
        }

        /// <summary>
        /// creates a dummy window
        /// </summary>
        /// <returns></returns>
        protected Window CreateDummy()
        {
            var dummy = WindowManager.GetSingleton().CreateWindow("DefaultWindow", DummyName + _nextDummyIdx++);

            dummy.SetAutoWindow(true);
            dummy.SetVisible(false);
            dummy.SetSize(USize.Zero);
            dummy.SetDestroyedByParent(true);

            return dummy;
        }

        /// <summary>
        /// checks whether given window is a dummy
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        protected bool IsDummy(Window wnd)
        {
            // all auto windows inside grid are dummies
            return wnd.IsAutoWindow();
        }

        protected override void AddChildImpl(Element element)
        {
            var wnd = element as Window;

            if (wnd == null)
            {
                throw new InvalidRequestException(
                    "GridLayoutContainer can only have Elements of type Window added " +
                    "as children (Window path: " + GetNamePath() + ").");
            }

            if (IsDummy(wnd))
            {
                base.AddChildImpl(wnd);
            }
            else
            {
                base.AddChildImpl(wnd);

                // OK, wnd is already in d_children

                // idx is the future index of the child that's being added
                int idx;

                if (_autoPositioning == AutoPositioning.Disabled)
                {
                    if ((_nextGridX == Int32.MaxValue) &&
                        (_nextGridY == Int32.MaxValue))
                    {
                        throw new InvalidRequestException(
                            "Unable to add child without explicit grid position " +
                            "because auto positioning is disabled.  Consider using the " +
                            "GridLayoutContainer::addChildToPosition functions.");
                    }

                    idx = MapFromGridToIdx(_nextGridX, _nextGridY,
                                           _gridWidth, _gridHeight);

                    // reset location to sentinel values.
                    _nextGridX = _nextGridY = Int32.MaxValue;
                }
                else
                {
                    idx = TranslateAutoPositionToGridIdx(_nextAutoPositioningIdx);
                    ++_nextAutoPositioningIdx;
                }

                // we swap the dummy and the added child
                // this essentially places the added child to it's right position and
                // puts the dummy at the end of d_children it will soon get removed from
                var tmp = d_children[idx];
                d_children[idx] = d_children[d_children.Count - 1];
                d_children[d_children.Count - 1] = tmp;

                var toBeRemoved = (Window) d_children[d_children.Count - 1];
                RemoveChild(toBeRemoved);

                if (toBeRemoved.IsDestroyedByParent())
                {
                    WindowManager.GetSingleton().DestroyWindow(toBeRemoved);
                }
            }
        }

        protected override void RemoveChildImpl(Element element)
        {
            var wnd = (Window) element;

            if (!IsDummy(wnd) && !WindowManager.GetSingleton().IsLocked())
            {
                // before we remove the child, we must add new dummy and place it
                // instead of the removed child
                var dummy = CreateDummy();
                AddChild(dummy);

                var i = GetIdxOfChild(wnd);
                var tmp = d_children[i];
                d_children[i] = d_children[d_children.Count - 1];
                d_children[d_children.Count - 1] = tmp;
            }

            base.RemoveChildImpl(wnd);
        }

        private void AddGridLayoutContainerProperties()
        {
            AddProperty(
                new TplWindowProperty<GridLayoutContainer, Sizef>(
                    "GridSize",
                    "Size of the grid of this layout container. Value uses the 'w:# h:#' format and will be rounded up because only integer values are valid as grid size.",
                    (x, v) => x.SetGrid(v), x => x.GetGrid(), WidgetTypeName, Sizef.Zero));

            AddProperty(
                new TplWindowProperty<GridLayoutContainer, AutoPositioning>(
                    "AutoPositioning",
                    "Sets the method used for auto positioning. Possible values: 'Disabled', 'Left to Right', 'Top to Bottom'.",
                    (x, v) => x.SetAutoPositioning(v), x => x.GetAutoPositioning(), WidgetTypeName,
                    AutoPositioning.LeftToRight));
        }

        #region Fields

        /// <summary>
        /// stores grid width - amount of columns
        /// </summary>
        private int _gridWidth;

        /// <summary>
        /// stores grid height - amount of rows
        /// </summary>
        private int _gridHeight;

        /// <summary>
        /// stores currently used auto positioning method
        /// </summary>
        private AutoPositioning _autoPositioning;

        /// <summary>
        /// stores next auto positioning index (will be used for next
        /// added window if d_autoPositioning != AP_Disabled)
        /// </summary>
        private int _nextAutoPositioningIdx;

        /// <summary>
        /// stores next used grid X position
        /// (only used if d_autoPositioning == AP_Disabled)
        /// </summary>
        private int _nextGridX;

        /// <summary>
        /// stores next used grid Y position
        /// (only used if d_autoPositioning == AP_Disabled)
        /// </summary>
        private int _nextGridY;

        /// <summary>
        /// stores next used dummy suffix index
        /// (used to generate unique dummy names)
        /// </summary>
        private int _nextDummyIdx;

        #endregion
    }
}