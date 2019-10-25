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
    /// ScrolledItemListBase window class
    /// </summary>
    public abstract class ScrolledItemListBase : ItemListBase
    {
        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ScrolledItemListBase";

        /// <summary>
        /// Name for vertical scrollbar component
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Name for horizontal scrollbar component
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        /// <summary>
        /// Name for the content pane component
        /// </summary>
        public const string ContentPaneName = "__auto_content_pane__";

        /// <summary>
        /// Event fired when the vertical scroll bar mode changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrolledItemListBase whose vertical
        /// scroll bar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> VertScrollbarModeChanged;

        /// <summary>
        /// Event fired when the horizontal scroll bar mode change.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrolledItemListBase whose horizontal
        /// scroll bar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> HorzScrollbarModeChanged;

        /// <summary>
        /// Returns whether the vertical scrollbar is being forced visible. Despite content size.
        /// </summary>
        /// <returns></returns>
        public bool IsVertScrollbarAlwaysShown()
        {
            return _forceVScroll;
        }

        /// <summary>
        /// Returns whether the horizontal scrollbar is being forced visible. Despite content size.
        /// </summary>
        /// <returns></returns>
        public bool IsHorzScrollbarAlwaysShown()
        {
            return _forceHScroll;
        }

        /// <summary>
        /// Get the vertical scrollbar component attached to this window.
        /// </summary>
        /// <returns></returns>
        public Scrollbar GetVertScrollbar()
        {
            return (Scrollbar) GetChild(VertScrollbarName);
        }

        /// <summary>
        /// Get the horizontal scrollbar component attached to this window.
        /// </summary>
        /// <returns></returns>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar) GetChild(HorzScrollbarName);
        }

        /// <summary>
        /// Sets whether the vertical scrollbar should be forced visible. Despite content size.
        /// </summary>
        /// <param name="mode"></param>
        public void SetShowVertScrollbar(bool mode)
        {
            if (mode != _forceVScroll)
            {
                _forceVScroll = mode;
                OnVertScrollbarModeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Sets whether the horizontal scrollbar should be forced visible. Despite content size.
        /// </summary>
        /// <param name="mode"></param>
        public void SetShowHorzScrollbar(bool mode)
        {
            if (mode != _forceHScroll)
            {
                _forceHScroll = mode;
                OnHorzScrollbarModeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Scroll the vertical list position if needed to ensure that the ItemEntry
        /// \a item is, if possible,  fully visible witin the ScrolledItemListBase
        /// viewable area.
        /// </summary>
        /// <param name="item">
        /// const reference to an ItemEntry attached to this ScrolledItemListBase
        /// that should be made visible in the view area.
        /// </param>
        public void EnsureItemIsVisibleVert(ItemEntry item)
        {
            var renderArea = GetItemRenderArea();
            var v = GetVertScrollbar();
            var currPos = v.GetScrollPosition();

            var top = CoordConverter.AsAbsolute(item.GetYPosition(), GetPixelSize().Height) - currPos;
            var bottom = top + item.GetItemPixelSize().Height;

            // if top is above the view area, or if item is too big, scroll item to top
            if ((top < renderArea.d_min.Y) || ((bottom - top) > renderArea.Height))
                v.SetScrollPosition(currPos + top);
                // if bottom is below the view area, scroll item to bottom of list
            else if (bottom >= renderArea.d_max.Y)
                v.SetScrollPosition(currPos + bottom - renderArea.Height);
        }

        /// <summary>
        /// Scroll the horizontal list position if needed to ensure that the
        /// ItemEntry \a item is, if possible, fully visible witin the
        /// ScrolledItemListBase viewable area.
        /// </summary>
        /// <param name="item">
        /// const reference to an ItemEntry attached to this ScrolledItemListBase
        /// that should be made visible in the view area.
        /// </param>
        public void EnsureItemIsVisibleHorz(ItemEntry item)
        {
            var renderArea = GetItemRenderArea();
            var h = GetHorzScrollbar();
            var currPos = h.GetScrollPosition();

            var left = CoordConverter.AsAbsolute(item.GetXPosition(), GetPixelSize().Width) - currPos;
            var right = left + item.GetItemPixelSize().Width;

            // if left is left of the view area, or if item too big, scroll item to left
            if ((left < renderArea.d_min.X) || ((right - left) > renderArea.Width))
                h.SetScrollPosition(currPos + left);
                // if right is right of the view area, scroll item to right of list
            else if (right >= renderArea.d_max.X)
                h.SetScrollPosition(currPos + right - renderArea.Width);
        }

        /// <summary>
        /// Constructor for the ScrolledItemListBase base class constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        protected ScrolledItemListBase(string type, string name)
            : base(type, name)
        {
            _forceVScroll = false;
            _forceHScroll = false;

            Pane = WindowManager.GetSingleton().CreateWindow("ClippedContainer", ContentPaneName);

            Pane.SetAutoWindow(true);
            ((ClippedContainer) Pane).SetClipperWindow(this);
            Pane.SetCursorInputPropagationEnabled(true);

            AddChild(Pane);

            // add properties for this class
            AddScrolledItemListBaseProperties();
        }

        protected override void Dispose(bool disposing)
        {
            if (Pane != null && Pane != this)
                WindowManager.GetSingleton().DestroyWindow(Pane);

            base.Dispose(disposing);
        }

        protected override void InitialiseComponents()
        {
            // base class handling
            base.InitialiseComponents();

            // init scrollbars
            var v = GetVertScrollbar();
            var h = GetHorzScrollbar();

            v.SetAlwaysOnTop(true);
            h.SetAlwaysOnTop(true);

            v.ScrollPositionChanged += HandleVScroll;
            h.ScrollPositionChanged += HandleHScroll;

            v.Hide();
            h.Hide();
        }

        /// <summary>
        /// Configure scrollbars
        /// </summary>
        /// <param name="docSize"></param>
        protected void ConfigureScrollbars(Sizef docSize)
        {
            var v = GetVertScrollbar();
            var h = GetHorzScrollbar();

            var oldVertVisible = v.IsVisible();
            var oldHorzVisible = h.IsVisible();

            var renderAreaSize = GetItemRenderArea().Size;

            // setup the pane size
            var paneSizeWidth = Math.Max(docSize.Width, renderAreaSize.Width);
            var paneSize = new USize(UDim.Absolute(paneSizeWidth), UDim.Absolute(docSize.Height));

            Pane.SetMinSize(paneSize);
            Pane.SetMaxSize(paneSize);

            // "fix" scrollbar visibility
            if (_forceVScroll || docSize.Height > renderAreaSize.Height)
            {
                v.Show();
            }
            else
            {
                v.Hide();
            }

            if (_forceHScroll || docSize.Width > renderAreaSize.Width)
            {
                h.Show();
            }
            else
            {
                h.Hide();
            }

            // if some change occurred, invalidate the inner rect area caches.
            if ((oldVertVisible != v.IsVisible()) ||
                (oldHorzVisible != h.IsVisible()))
            {
                d_unclippedInnerRect.InvalidateCache();
                d_innerRectClipperValid = false;
            }

            // get a fresh item render area
            var renderArea = GetItemRenderArea();
            renderAreaSize = renderArea.Size;

            // update the pane clipper area
            ((ClippedContainer) Pane).SetClipArea(renderArea);

            // setup vertical scrollbar
            v.SetDocumentSize(docSize.Height);
            v.SetPageSize(renderAreaSize.Height);
            v.SetStepSize(Math.Max(1.0f, renderAreaSize.Height/10.0f));
            v.SetScrollPosition(v.GetScrollPosition());

            // setup horizontal scrollbar
            h.SetDocumentSize(docSize.Width);
            h.SetPageSize(renderAreaSize.Width);
            h.SetStepSize(Math.Max(1.0f, renderAreaSize.Width/10.0f));
            h.SetScrollPosition(h.GetScrollPosition());
        }

        protected virtual void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(VertScrollbarModeChanged, e);
        }

        protected virtual void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(HorzScrollbarModeChanged, e);
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            base.OnScroll(e);

            var count = GetItemCount();
            var v = GetVertScrollbar();

            // dont do anything if we are no using scrollbars
            // or have'nt got any items
            if (!v.IsVisible() || count == 0)
                return;

            var pixH = Pane.GetUnclippedOuterRect().Get().Height;
            var delta = (pixH/count)*-e.scroll;
            v.SetScrollPosition(v.GetScrollPosition() + delta);

            ++e.handled;
        }

        protected bool HandleVScroll(EventArgs e)
        {
            var v = (Scrollbar) ((WindowEventArgs)e).Window;
            var newpos = -v.GetScrollPosition();
            Pane.SetYPosition(UDim.Absolute(newpos));
            return true;
        }

        protected bool HandleHScroll(EventArgs e)
        {
            var h = (Scrollbar)((WindowEventArgs)e).Window;
            var newpos = -h.GetScrollPosition();
            Pane.SetXPosition(UDim.Absolute(newpos));
            return true;
        }

        private void AddScrolledItemListBaseProperties()
        {
            const string propertyOrigin = "CEGUI/ScrolledItemListBase";

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<ScrolledItemListBase, bool>(
                            "ForceVertScrollbar",
                            "Property to get/set the state of the force vertical scrollbar setting for the ScrolledItemListBase.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), propertyOrigin));

            // TODO: Inconsistency
            AddProperty(new TplWindowProperty<ScrolledItemListBase, bool>(
                            "ForceHorzScrollbar",
                            "Property to get/set the state of the force horizontal scrollbar setting for the ScrolledItemListBase.  Value is either \"True\" or \"False\".",
                            (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), propertyOrigin));
        }

        #region Fields

        private bool _forceVScroll;
        private bool _forceHScroll;

        #endregion
    }
}