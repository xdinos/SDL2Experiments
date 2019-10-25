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
    /// An abstract base class providing common functionality and specifying the
    /// required interface for derived classes.
    /// 
    /// Sequential Layout Container provide means for automatic positioning of
    /// windows in sequence
    /// </summary>
    public abstract class SequentialLayoutContainer : LayoutContainer
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "SequentialLayoutContainer";

        /// <summary>
        /// fired when child windows get rearranged
        /// </summary>
        public event EventHandler<WindowEventArgs> ChildOrderChanged;

        /// <summary>
        /// Constructor for Window base class
        /// </summary>
        /// <param name="type">
        /// String object holding Window type (usually provided by WindowFactory).
        /// </param>
        /// <param name="name">
        /// String object holding unique name for the Window.
        /// </param>
        protected SequentialLayoutContainer(string type, string name)
            : base(type, name)
        {
        }

        /// <summary>
        /// Gets the position of given child window
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public int GetPositionOfChild(Window wnd)
        {
            return GetIdxOfChild(wnd);
        }

        /// <summary>
        /// Gets the position of given child window
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public int GetPositionOfChild(string wnd)
        {
            return GetPositionOfChild(GetChild(wnd));
        }

        /// <summary>
        /// Gets the child window that currently is at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Window GetChildAtPosition(int position)
        {
            return GetChildAtIdx(position);
        }

        /// <summary>
        /// Swaps windows at given positions
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
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(Window wnd1, Window wnd2)
        {
            if (IsChild(wnd1) && IsChild(wnd2))
            {
                SwapChildPositions(GetPositionOfChild(wnd1), GetPositionOfChild(wnd2));
            }
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(string wnd1, Window wnd2)
        {
            SwapChildren(GetChild(wnd1), wnd2);
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(Window wnd1, string wnd2)
        {
            SwapChildren(wnd1, GetChild(wnd2));
        }

        /// <summary>
        /// Swaps positions of given windows
        /// </summary>
        /// <param name="wnd1"></param>
        /// <param name="wnd2"></param>
        public void SwapChildren(string wnd1, string wnd2)
        {
            SwapChildren(GetChild(wnd1), GetChild(wnd2));
        }

        /// <summary>
        /// Moves a window that is alrady child of thi layout container
        /// to given position (if the window is currently in a position
        /// that is smaller than given position, given position is
        /// automatically decremented
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="position"></param>
        public virtual void MoveChildToPosition(Window wnd, int position)
        {
            if (!IsChild(wnd))
                return;

            position = Math.Min(position, d_children.Count - 1);

            var oldPosition = GetPositionOfChild(wnd);

            if (oldPosition == position)
                return;

            d_children.RemoveAt(oldPosition);
            if (oldPosition < position)
                --position;
            d_children.Insert(position, wnd);
            //throw new NotImplementedException();

            //// we get the iterator of the old position
            //ChildList::iterator it = d_children.begin();
            //std::advance(it, oldPosition);

            //// we are the window from it's old position
            //d_children.erase(it);

            //// if the window comes before the point we want to insert to,
            //// we have to decrement the position
            //if (oldPosition < position)
            //{
            //    --position;
            //}

            //// find iterator of the new position
            //it = d_children.begin();
            //std::advance(it, position);
            //// and insert the window there
            //d_children.insert(it, wnd);

            OnChildOrderChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Moves a window that is alrady child of thi layout container
        /// to given position (if the window is currently in a position
        /// that is smaller than given position, given position is
        /// automatically decremented
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="position"></param>
        public void MoveChildToPosition(string wnd, int position)
        {
            MoveChildToPosition(GetChild(wnd), position);
        }

        /// <summary>
        /// Moves a window forward or backward, depending on delta
        /// (-1 moves it backward one step, 1 moves it forward one step)
        /// </summary>
        /// <param name="window"></param>
        /// <param name="delta">
        /// The amount of steps the window will be moved
        /// (old position + delta = new position)
        /// </param>
        public void MoveChild(Window window, int delta = 1)
        {
            var oldPosition = GetPositionOfChild(window);
            var newPosition = oldPosition + delta;
            newPosition = Math.Max(newPosition, 0);

            MoveChildToPosition(window, newPosition);
        }

        /// <summary>
        /// Adds a window to given position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="position"></param>
        public void AddChildToPosition(Window window, int position)
        {
            AddChild(window);
            MoveChildToPosition(window, position);
        }

        /// <summary>
        /// Removes a window from given position
        /// </summary>
        /// <param name="position"></param>
        public void RemoveChildFromPosition(int position)
        {
            RemoveChild(GetChildAtPosition(position));
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
            FireEvent(ChildOrderChanged, e);
        }
    }
}