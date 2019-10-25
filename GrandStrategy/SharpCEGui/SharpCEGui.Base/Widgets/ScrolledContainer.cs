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
    /// Helper container window class which is used in the implementation of the
    /// ScrollablePane widget class.
    /// </summary>
    public class ScrolledContainer : Window
    {
        /// <summary>
        /// Type name for ScrolledContainer.
        /// </summary>
        public const string WidgetTypeName = "ScrolledContainer";

        #region Events

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ScrolledContainer";
        public const string EventContentChanged = "ContentChanged";
        public const string EventAutoSizeSettingChanged = "AutoSizeSettingChanged";


        /// <summary>
        /// Event fired whenever some child changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrolledContainer for which a child
        /// window has changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> ContentChanged
        {
            add { SubscribeEvent(EventContentChanged, value); }
            remove { UnsubscribeEvent(EventContentChanged, value); }
        }

        /// <summary>
        /// Event fired when the autosize setting changes.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrolledContainer whose auto size
        /// setting has been changed.
        /// </summary>
        public event GuiEventHandler<EventArgs> AutoSizeSettingChanged
        {
            add { SubscribeEvent(EventAutoSizeSettingChanged, value); }
            remove { UnsubscribeEvent(EventAutoSizeSettingChanged, value); }
        }

        #endregion

        /// <summary>
        /// Constructor for ScrolledContainer objects.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ScrolledContainer(string type, string name) : base(type, name)
        {
            _contentArea = Rectf.Zero;
            _autoSizePane = true;

            _clientChildContentArea =
                new CachedRectf(this, (e, b) => ((ScrolledContainer) e).GetClientChildContentAreaImpl(b));

            AddScrolledContainerProperties();
            SetCursorInputPropagationEnabled(true);

        }

        /// <summary>
        /// Return whether the content pane is auto sized.
        /// </summary>
        /// <returns>
        /// - true to indicate the content pane will automatically resize itself.
        /// - false to indicate the content pane will not automatically resize itself.
        /// </returns>
        public bool IsContentPaneAutoSized()
        {
            return _autoSizePane;
        }

        /// <summary>
        /// Set whether the content pane should be auto-sized.
        /// </summary>
        /// <param name="setting">
        /// - true to indicate the content pane should automatically resize itself.
        /// - false to indicate the content pane should not automatically resize itself.
        /// </param>
        public void SetContentPaneAutoSized(bool setting)
        {
            if (_autoSizePane != setting)
            {
                _autoSizePane = setting;

                // Fire events
                OnAutoSizeSettingChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the current content pane area for the ScrolledContainer.
        /// </summary>
        /// <returns>
        /// Rect object that details the current pixel extents of the content
        /// pane represented by this ScrolledContainer.
        /// </returns>
        public Rectf GetContentArea()
        {
            return _contentArea;
        }

        /// <summary>
        /// Set the current content pane area for the ScrolledContainer.
        /// </summary>
        /// <param name="area">
        /// Rect object that details the pixel extents to use for the content
        /// pane represented by this ScrolledContainer.
        /// </param>
        /// <remarks>
        /// If the ScrolledContainer is configured to auto-size the content pane
        /// this call will have no effect.
        /// </remarks>
        public void SetContentArea(Rectf area)
        {
            if (!_autoSizePane)
            {
                _contentArea = area;
                SetSize(new USize(UDim.Absolute(_contentArea.Width), UDim.Absolute(_contentArea.Height)));
        
                // Fire event
                OnContentChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return the current extents of the attached content.
        /// </summary>
        /// <returns>
        /// Rect object that describes the pixel extents of the attached
        /// child windows.  This is effectively the smallest bounding box
        /// that could contain all the attached windows.
        /// </returns>
        public Rectf GetChildExtentsArea()
        {
            var extents = Rectf.Zero;

            var childCount = GetChildCount();
            if (childCount == 0)
                return extents;

            for (var i = 0; i < childCount; ++i)
            {
                var wnd = GetChildAtIdx(i);
                var area = new Rectf(CoordConverter.AsAbsolute(wnd.GetPosition(), d_pixelSize), wnd.GetPixelSize());

                if (wnd.GetHorizontalAlignment() == HorizontalAlignment.Centre)
                    area.Position = area.Position - new Lunatics.Mathematics.Vector2(area.Width*0.5f - d_pixelSize.Width*0.5f, 0.0f);
                if (wnd.GetVerticalAlignment() == VerticalAlignment.Centre)
                    area.Position = area.Position - new Lunatics.Mathematics.Vector2(0.0f, area.Height*0.5f - d_pixelSize.Height*0.5f);

                if (area.d_min.X < extents.d_min.X)
                    extents.d_min.X = area.d_min.X;

                if (area.d_min.Y < extents.d_min.Y)
                    extents.d_min.Y = area.d_min.Y;

                if (area.d_max.X > extents.d_max.X)
                    extents.d_max.X = area.d_max.X;

                if (area.d_max.Y > extents.d_max.Y)
                    extents.d_max.Y = area.d_max.Y;
            }

            return extents;
        }

        public override CachedRectf GetClientChildContentArea()
        {
            return _clientChildContentArea;
        }

        public override CachedRectf GetNonClientChildContentArea()
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
                return skipAllPixelAlignment ? base.GetUnclippedInnerRect().GetFresh(true) : base.GetUnclippedInnerRect().Get();
            
            if (skipAllPixelAlignment)
            {
                return new Rectf(GetUnclippedOuterRect().GetFresh(true).Position,
                                 GetParent().GetUnclippedInnerRect().GetFresh(true).Size);
            }
            
            return new Rectf(GetUnclippedOuterRect().Get().Position,
                             GetParent().GetUnclippedInnerRect().Get().Size);
        }
    
        /// <summary>
        /// Notification method called whenever the content size may have changed.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnContentChanged(WindowEventArgs e)
        {
            if (_autoSizePane)
                _contentArea = GetChildExtentsArea();

            FireEvent(EventContentChanged, e, EventNamespace);
        }

        /// <summary>
        /// Notification method called whenever the setting that controls whether
        /// the content pane is automatically sized is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnAutoSizeSettingChanged(WindowEventArgs e)
        {
            if (_autoSizePane)
            {
                foreach (var child in d_children)
                    MakeSureChildUsesAbsoluteArea(child);
            }

            FireEvent(EventAutoSizeSettingChanged, e, EventNamespace);

            if (_autoSizePane)
                OnContentChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// handles notifications about child windows being moved.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleChildSized(EventArgs e)
        {
            if (_autoSizePane)
                MakeSureChildUsesAbsoluteArea(((ElementEventArgs)e).element);

            // Fire event that notifies that a child's area has changed.
            OnContentChanged(new WindowEventArgs(this));
            return true;
        }

        /// <summary>
        /// handles notifications about child windows being sized.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleChildMoved(EventArgs e)
        {
            if (_autoSizePane)
                MakeSureChildUsesAbsoluteArea(((ElementEventArgs)e).element);

            // Fire event that notifies that a child's area has changed.
            OnContentChanged(new WindowEventArgs(this));
            return true;
        }

        protected override void DrawSelf(RenderingContext ctx)
        {

        }

        protected override Rectf GetInnerRectClipperImpl()
        {
            return d_parent != null
                       ? GetParent().GetInnerRectClipper()
                       : base.GetInnerRectClipperImpl();
        }

        protected override void SetAreaImpl(UVector2 pos, USize size, bool topLeftSizing = false, bool fireEvents = true)
        {
            _clientChildContentArea.InvalidateCache();
            base.SetAreaImpl(pos, size, topLeftSizing, fireEvents);
        }

        protected override Rectf GetHitTestRectImpl()
        {
            return d_parent != null
                       ? GetParent().GetHitTestRect()
                       : base.GetHitTestRectImpl();
        }


        protected override void OnChildAdded(ElementEventArgs e)
        {
            base.OnChildAdded(e);

            if (_autoSizePane)
                MakeSureChildUsesAbsoluteArea(e.element);

            // subscribe to some events on this child
            e.element.Sized += HandleChildSized;
            e.element.Moved += HandleChildMoved;

            // force window to update what it thinks it's screen / pixel areas are.
            e.element.NotifyScreenAreaChanged(false);

            // perform notification.
            OnContentChanged(new WindowEventArgs(this));
        }

        protected override void OnChildRemoved(ElementEventArgs e)
        {
            base.OnChildRemoved(e);

            // disconnect from events for this window.

            e.element.Sized -= HandleChildSized;
            e.element.Moved -= HandleChildMoved;

            // perform notification only if we're not currently being destroyed
            if (!d_destructionStarted)
            {
                OnContentChanged(new WindowEventArgs(this));
            }
        }

        protected internal override void OnParentSized(ElementEventArgs e)
        {
            base.OnParentSized(e);

            // perform notification.
            OnContentChanged(new WindowEventArgs(this));
        }

        private void AddScrolledContainerProperties()
        {
            AddProperty(new TplWindowProperty<ScrolledContainer, bool>(
                            "ContentPaneAutoSized",
                            "Property to get/set the setting which controls whether the content pane will auto-size itself. Value is either \"True\" or \"False\".",
                            (x, v) => x.SetContentPaneAutoSized(v), x => x.IsContentPaneAutoSized(), WidgetTypeName, true));

            AddProperty(new TplWindowProperty<ScrolledContainer, Rectf>(
                            "ContentArea",
                            "Property to get/set the current content area rectangle of the content pane. Value is \"l:[float] t:[float] r:[float] b:[float]\" (where l is left, t is top, r is right, and b is bottom).",
                            (x, v) => x.SetContentArea(v), x => x.GetContentArea(), WidgetTypeName, Rectf.Zero));

            AddProperty(new TplWindowProperty<ScrolledContainer, Rectf>(
                            "ChildExtentsArea",
                            "Property to get the current content extents rectangle. Value is \"l:[float] t:[float] r:[float] b:[float]\" (where l is left, t is top, r is right, and b is bottom).",
                            null, x => x.GetChildExtentsArea(), WidgetTypeName, Rectf.Zero));
        }

        private void MakeSureChildUsesAbsoluteArea(Element child)
        {
            //if (!child.GetArea().IsAbsolute())
            //    throw new InvalidRequestException(
            //            "A relative component in a child's area is taken relative to the size of content area, not the size of the " +
            //            "scrollable pane. Therefore, when a child uses a non-absolute area (i.e. which has any unified dimension " +
            //            "with a non-zero relative component) while auto-size is set to \"true\", this creates a circular dependency " +
            //            "and is therefore not allowed.");
        }

        #region Fields

        /// <summary>
        /// Holds extents of the content pane.
        /// </summary>
        private Rectf _contentArea;

        /// <summary>
        /// true if the pane auto-sizes itself.
        /// </summary>
        private bool _autoSizePane;

        private readonly CachedRectf _clientChildContentArea;
        
        #endregion
    }
}