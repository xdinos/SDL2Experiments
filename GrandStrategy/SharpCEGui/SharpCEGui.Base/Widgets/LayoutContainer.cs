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

using System.Diagnostics;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// An abstract base class providing common functionality and specifying the
    /// required interface for derived classes.
    /// 
    /// Layout Container provide means for automatic positioning based on sizes of
    /// it's child Windows. This is useful for dynamic UIs.
    /// </summary>
    public abstract class LayoutContainer : Window
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "LayoutContainer";

        /// <summary>
        /// Constructor for Window base class
        /// </summary>
        /// <param name="type">
        /// String object holding Window type (usually provided by WindowFactory).
        /// </param>
        /// <param name="name">
        /// String object holding unique name for the Window.
        /// </param>
        protected LayoutContainer(string type, string name) : base(type, name)
        {
            _needsLayouting = false;
            _clientChildContentArea = new CachedRectf(this,
                                                       (e, v) => ((LayoutContainer) e).GetClientChildContentAreaImpl(v));

            // layout should take the whole window by default I think
            SetSize(new USize(UDim.Relative(1), UDim.Relative(1)));

            ChildAdded += HandleChildAdded;
            ChildRemoved += HandleChildRemoved;
        }

        /// <summary>
        /// marks this layout container for relayouting before drawing
        /// </summary>
        public void MarkNeedsLayouting()
        {
            _needsLayouting = true;
        }

        /// <summary>
        /// returns true if this layout container will be relayouted before drawing
        /// </summary>
        /// <returns></returns>
        public bool NeedsLayouting()
        {
            return _needsLayouting;
        }

        /// <summary>
        /// (re)layouts all windows inside this layout container immediately
        /// </summary>
        public abstract void Layout();

        /// <summary>
        /// (re)layouts all windows inside this layout container if it was marked necessary
        /// </summary>
        public virtual void LayoutIfNecessary()
        {
            if (_needsLayouting)
            {
                Layout();

                _needsLayouting = false;
            }
        }

        public override void Update(float elapsed)
        {
            base.Update(elapsed);

            LayoutIfNecessary();
        }

        public override CachedRectf GetClientChildContentArea()
        {
            return _clientChildContentArea;
        }

        public override void NotifyScreenAreaChanged(bool recursive = true)
        {
            _clientChildContentArea.InvalidateCache();

            base.NotifyScreenAreaChanged(recursive);
        }
        
        protected override Rectf GetUnclippedInnerRectImpl(bool skipAllPixelAlignment)
        {
            return d_parent != null
                       ? (skipAllPixelAlignment
                              ? d_parent.GetUnclippedInnerRect().GetFresh(true)
                              : d_parent.GetUnclippedInnerRect().Get())
                       : base.GetUnclippedInnerRectImpl(skipAllPixelAlignment);
        }

        protected Rectf GetClientChildContentAreaImpl(bool skipAllPixelAlignment)
        {
            if (d_parent==null)
                return skipAllPixelAlignment ? 
                        base.GetClientChildContentArea().GetFresh(true) : 
                        base.GetClientChildContentArea().Get();

            return skipAllPixelAlignment
                       ? new Rectf(GetUnclippedOuterRect().GetFresh(true).Position,
                                   d_parent.GetUnclippedInnerRect().GetFresh(true).Size)
                       : new Rectf(GetUnclippedOuterRect().Get().Position,
                                   d_parent.GetUnclippedInnerRect().Get().Size);

        }

        protected int GetIdxOfChild(Window wnd)
        {
            for (var i = 0; i < GetChildCount(); ++i)
            {
                if (GetChildAtIdx(i) == wnd)
                    return i;
            }

            Debug.Assert(false);
            return 0;
        }

        /// @copydoc Window::addChild_impl
        protected override void AddChildImpl(Element element)
        {
            var wnd = element as Window;

            if (wnd == null)
                throw new InvalidRequestException("LayoutContainer can only have Elements of type Window added as " +
                                                  "children (Window path: " + GetNamePath() + ").");

            base.AddChildImpl(wnd);
            
            // we have to subscribe to the EventSized for layout updates
            wnd.Sized += HandleChildSized;
            wnd.MarginChanged += HandleChildMarginChanged;
        }

        protected override void RemoveChildImpl(Element element)
        {
            var wnd = (Window)element;
    
            // we want to get rid of the subscription, because the child window could
            // get removed and added somewhere else, we would be wastefully updating
            // layouts if it was sized inside other Window

            wnd.Sized -= HandleChildSized;
            wnd.MarginChanged -= HandleChildMarginChanged;

            base.RemoveChildImpl(wnd);
        }

        /// <summary>
        /// Handler called when child window gets sized
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object whose 'window' pointer field is set to the
        /// window that triggered the event.  For this event the trigger window is
        /// the one that was sized.
        /// </param>
        /// <returns></returns>
        protected virtual bool HandleChildSized(/*ElementEventArgs*/EventArgs e)
        {
            MarkNeedsLayouting();
            return true;
        }

        /// <summary>
        /// Handler called when child window changes margin(s)
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object whose 'window' pointer field is set to the
        /// window that triggered the event.  For this event the trigger window is
        /// the one that has had it's margin(s) changed.</param>
        protected virtual bool HandleChildMarginChanged(EventArgs e)
        {
            MarkNeedsLayouting();
            return true;
        }

        /// <summary>
        /// Handler called when child window gets added
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object whose 'window' pointer field is set to the
        /// window that triggered the event.  For this event the trigger window is
        /// the one that was added.
        /// </param>
        /// <returns></returns>
        protected virtual bool HandleChildAdded(EventArgs e)
        {
            MarkNeedsLayouting();
            return true;
        }

        /// <summary>
        /// Handler called when child window gets removed
        /// </summary>
        /// <param name="e"></param>
        /// <returns>
        /// WindowEventArgs object whose 'window' pointer field is set to the
        /// window that triggered the event.  For this event the trigger window is
        /// the one that was removed.
        /// </returns>
        protected virtual bool HandleChildRemoved(EventArgs e)
        {
            MarkNeedsLayouting();
            return true;
        }

        /// <summary>
        /// returns margin offset for given window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        protected virtual UVector2 GetOffsetForWindow(Window window)
        {
            var margin = window.GetMargin();

            return new UVector2(margin.d_left, margin.d_top);
        }

        /// <summary>
        /// returns bounding size for window, including margins
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        protected virtual UVector2 GetBoundingSizeForWindow(Window window)
        {
            var pixelSize = window.GetPixelSize();

            // we rely on pixelSize rather than mixed absolute and relative getSize
            // this seems to solve problems when windows overlap because their size
            // is constrained by min size
            var size = new UVector2(new UDim(0, pixelSize.Width),
                                    new UDim(0, pixelSize.Height));

            // TODO: we still do mixed absolute/relative margin, should we convert the value to absolute?
            var margin = window.GetMargin();

            return new UVector2(margin.d_left + size.d_x + margin.d_right,
                                margin.d_top + size.d_y + margin.d_bottom);
        }

        protected internal override void OnParentSized(ElementEventArgs e)
        {
            // This is intentionally not Window::onParentSized.
            base.OnParentSizedInner(e);

            // force update of child positioning.
            NotifyScreenAreaChanged();
            PerformChildWindowLayout(true, true);
        }
        
        #region Fields

        /// <summary>
        /// if true, we will relayout before rendering of this window starts 
        /// </summary>
        private bool _needsLayouting;

        private readonly CachedRectf _clientChildContentArea;

        #endregion
    }
}