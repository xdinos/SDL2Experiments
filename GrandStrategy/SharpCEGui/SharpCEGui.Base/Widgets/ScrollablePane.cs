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
    /// Base class for ScrollablePane window renderer objects.
    /// </summary>
    public abstract class ScrollablePaneWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ScrollablePaneWindowRenderer(string name)
                : base(name, ScrollablePane.EventNamespace)
        {

        }

        /// <summary>
        /// Return a Rect that described the pane's viewable area, relative
        /// to this Window, in pixels.
        /// </summary>
        /// <returns>
        /// Rect object describing the ScrollablePane's viewable area.
        /// </returns>
        public abstract Rectf GetViewableArea();
    }

    /// <summary>
    /// Base class for the ScrollablePane widget.
    /// <para>
    /// The ScrollablePane widget allows child windows to be attached which cover an
    /// area larger than the ScrollablePane itself and these child windows can be
    /// scrolled into view using the scrollbars of the scrollable pane.
    /// </para>
    /// </summary>
    public class ScrollablePane : Window
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/ScrollablePane";

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "ScrollablePane";


        /// <summary>
        /// Event fired when an area on the content pane has been updated.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrollablePane whose content pane
        /// has been updated. 
        /// </summary>
        public event EventHandler<WindowEventArgs> ContentPaneChanged;

        /// <summary>
        /// Event fired when the vertical scroll bar 'force' setting is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrollablePane whose vertical scroll
        /// bar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> VertScrollbarModeChanged;

        /// <summary>
        /// Event fired when the horizontal scroll bar 'force' setting is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrollablePane whose horizontal scroll
        /// bar mode has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> HorzScrollbarModeChanged;

        /// <summary>
        /// Event fired when the auto size setting for the pane is changed.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrollablePane whose auto size
        /// setting has been changed.
        /// </summary>
        public event EventHandler<WindowEventArgs> AutoSizeSettingChanged;

        /// <summary>
        /// Event fired when the pane gets scrolled.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the ScrollablePane that has been scrolled.
        /// </summary>
        public event EventHandler<WindowEventArgs> ContentPaneScrolled;

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        /// <summary>
        /// Widget name for the scrolled container component.
        /// </summary>
        public const string ScrolledContainerName = "__auto_container__";


        /// <summary>
        /// Constructor for the ScrollablePane base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public ScrollablePane(string type, string name)
            : base(type, name)
        {
            _forceVertScroll = false;
            _forceHorzScroll = false;
            _contentRect = Rectf.Zero;
            _vertStep = 0.1f;
            _vertOverlap = 0.01f;
            _horzStep = 0.1f;
            _horzOverlap = 0.01f;

            AddScrollablePaneProperties();

            // create scrolled container widget
            var container =
                (ScrolledContainer)
                WindowManager.GetSingleton().CreateWindow(ScrolledContainer.WidgetTypeName, ScrolledContainerName);
            container.SetAutoWindow(true);

            // add scrolled container widget as child
            AddChild(container);
        }

        /// <summary>
        /// Returns a pointer to the window holding the pane contents.
        /// <para>
        /// The purpose of this is so that attached windows may be inspected,
        /// client code may not modify the returned window in any way.
        /// </para>
        /// </summary>
        /// <returns>
        /// Pointer to the ScrolledContainer that is acting as the container for the
        /// scrollable pane content.  The returned window is const, client code
        /// should not modify the ScrolledContainer settings directly.
        /// </returns>
        public ScrolledContainer GetContentPane()
        {
            return GetScrolledContainer();
        }

        /// <summary>
        /// Return whether the vertical scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsVertScrollbarAlwaysShown()
        {
            return _forceVertScroll;
        }

        /// <summary>
        /// Set whether the vertical scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// - true if the vertical scroll bar should be shown even when it is not required.
        /// - false if the vertical scroll bar should only be shown when it is required.
        /// </param>
        public void SetShowVertScrollbar(bool setting)
        {
            if (_forceVertScroll != setting)
            {
                _forceVertScroll = setting;

                ConfigureScrollbars();
                OnVertScrollbarModeChanged(new WindowEventArgs(this));
            }
        }

        /// <summary>
        /// Return whether the horizontal scroll bar is always shown.
        /// </summary>
        /// <returns>
        /// - true if the scroll bar will be shown even if it is not required.
        /// - false if the scroll bar will only be shown when it is required.
        /// </returns>
        public bool IsHorzScrollbarAlwaysShown()
        {
            return _forceHorzScroll;
        }

        /// <summary>
        /// Set whether the horizontal scroll bar should always be shown.
        /// </summary>
        /// <param name="setting">
        /// - true if the horizontal scroll bar should be shown even when it is not required.
        /// - false if the horizontal scroll bar should only be shown when it is required.
        /// </param>
        public void SetShowHorzScrollbar(bool setting)
        {
            if (_forceHorzScroll != setting)
            {
                _forceHorzScroll = setting;

                ConfigureScrollbars();
                OnHorzScrollbarModeChanged(new WindowEventArgs(this));
            }
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
            return GetScrolledContainer().IsContentPaneAutoSized();
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
            GetScrolledContainer().SetContentPaneAutoSized(setting);
        }

        /// <summary>
        /// Return the current content pane area for the ScrollablePane.
        /// </summary>
        /// <returns>
        /// Rect object that details the current pixel extents of the content
        /// pane attached to this ScrollablePane.
        /// </returns>
        public Rectf GetContentPaneArea()
        {
            return GetScrolledContainer().GetContentArea();
        }

        /// <summary>
        /// Set the current content pane area for the ScrollablePane.
        /// <para>
        /// If the ScrollablePane is configured to auto-size the content pane
        /// this call will have no effect.
        /// </para>
        /// </summary>
        /// <param name="area">
        /// Rect object that details the pixel extents to use for the content
        /// pane attached to this ScrollablePane.
        /// </param>
        public void SetContentPaneArea(Rectf area)
        {
            GetScrolledContainer().SetContentArea(area);
        }

        /// <summary>
        /// Returns the horizontal scrollbar step size as a fraction of one complete view page.
        /// </summary>
        /// <returns>
        /// float value specifying the step size where 1.0f would be the width of the viewing area.
        /// </returns>
        public float GetHorizontalStepSize()
        {
            return _horzStep;
        }

        /// <summary>
        /// Sets the horizontal scrollbar step size as a fraction of one complete view page.
        /// </summary>
        /// <param name="step">
        /// float value specifying the step size, where 1.0f would be the width of the viewing area.
        /// </param>
        public void SetHorizontalStepSize(float step)
        {
            _horzStep = step;
            ConfigureScrollbars();
        }

        /// <summary>
        /// Returns the horizontal scrollbar overlap size as a fraction of one complete view page.
        /// </summary>
        /// <returns>
        /// float value specifying the overlap size where 1.0f would be the width of the viewing area.
        /// </returns>
        public float GetHorizontalOverlapSize()
        {
            return _horzOverlap;
        }

        /// <summary>
        /// Sets the horizontal scrollbar overlap size as a fraction of one complete view page.
        /// </summary>
        /// <param name="overlap">
        /// float value specifying the overlap size, where 1.0f would be the width of the viewing area.
        /// </param>
        public void SetHorizontalOverlapSize(float overlap)
        {
            _horzOverlap = overlap;
            ConfigureScrollbars();
        }

        /// <summary>
        /// Returns the horizontal scroll position as a fraction of the complete scrollable width.
        /// </summary>
        /// <returns>
        /// float value specifying the scroll position.
        /// </returns>
        public float GetHorizontalScrollPosition()
        {
            return GetHorzScrollbar().GetUnitIntervalScrollPosition();
        }

        /// <summary>
        /// Sets the horizontal scroll position as a fraction of the complete scrollable width.
        /// </summary>
        /// <param name="position">
        /// float value specifying the new scroll position.
        /// </param>
        public void SetHorizontalScrollPosition(float position)
        {
            GetHorzScrollbar().SetUnitIntervalScrollPosition(position);
        }

        /// <summary>
        /// Returns the vertical scrollbar step size as a fraction of one complete view page.
        /// </summary>
        /// <returns>
        /// float value specifying the step size where 1.0f would be the height of the viewing area.
        /// </returns>
        public float GetVerticalStepSize()
        {
            return _vertStep;
        }

        /// <summary>
        /// Sets the vertical scrollbar step size as a fraction of one complete view page.
        /// </summary>
        /// <param name="step">
        /// float value specifying the step size, where 1.0f would be the height of the viewing area.
        /// </param>
        public void SetVerticalStepSize(float step)
        {
            _vertStep = step;
            ConfigureScrollbars();
        }

        /// <summary>
        /// Returns the vertical scrollbar overlap size as a fraction of one complete view page.
        /// </summary>
        /// <returns>
        /// float value specifying the overlap size where 1.0f would be the height of the viewing area.
        /// </returns>
        public float GetVerticalOverlapSize()
        {
            return _vertOverlap;
        }

        /// <summary>
        /// Sets the vertical scrollbar overlap size as a fraction of one complete view page.
        /// </summary>
        /// <param name="overlap">
        /// float value specifying the overlap size, where 1.0f would be the height of the viewing area.
        /// </param>
        public void SetVerticalOverlapSize(float overlap)
        {
            _vertOverlap = overlap;
            ConfigureScrollbars();
        }

        /// <summary>
        /// Returns the vertical scroll position as a fraction of the complete scrollable height.
        /// </summary>
        /// <returns>
        /// float value specifying the scroll position.
        /// </returns>
        public float GetVerticalScrollPosition()
        {
            return GetVertScrollbar().GetUnitIntervalScrollPosition();
        }

        /// <summary>
        /// Sets the vertical scroll position as a fraction of the complete scrollable height.
        /// </summary>
        /// <param name="position">
        /// float value specifying the new scroll position.
        /// </param>
        public void SetVerticalScrollPosition(float position)
        {
            GetVertScrollbar().SetUnitIntervalScrollPosition(position);
        }

        /// <summary>
        /// Return a Rect that described the pane's viewable area, relative to this Window, in pixels.
        /// </summary>
        /// <returns>
        /// Rect object describing the ScrollablePane's viewable area.
        /// </returns>
        public Rectf GetViewableArea()
        {
            if (d_windowRenderer == null)
                throw new InvalidRequestException("This function must be implemented by the window renderer module");

            var wr = (ScrollablePaneWindowRenderer) d_windowRenderer;
            return wr.GetViewableArea();
        }

        /// <summary>
        /// Return a pointer to the vertical scrollbar component widget for this ScrollablePane.
        /// </summary>
        /// <returns>
        /// Pointer to a Scrollbar object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the vertical Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetVertScrollbar()
        {
            return (Scrollbar) GetChild(VertScrollbarName);
        }

        /// <summary>
        /// Return a pointer to the horizontal scrollbar component widget for this ScrollablePane.
        /// </summary>
        /// <returns>Pointer to a Scrollbar object.</returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the horizontal Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar) GetChild(HorzScrollbarName);
        }

        // Overridden from Window
        protected override void InitialiseComponents()
        {
            // get horizontal scrollbar
            var horzScrollbar = GetHorzScrollbar();

            // get vertical scrollbar
            var vertScrollbar = GetVertScrollbar();

            // get scrolled container widget
            var container = GetScrolledContainer();

            // do a bit of initialisation
            container.BanPropertyFromXML("MouseInputPropagationEnabled");
            container.BanPropertyFromXML("ContentArea");
            container.BanPropertyFromXML("ContentPaneAutoSized");
            horzScrollbar.BanPropertyFromXML("AlwaysOnTop");
            vertScrollbar.BanPropertyFromXML("AlwaysOnTop");

            horzScrollbar.SetAlwaysOnTop(true);
            vertScrollbar.SetAlwaysOnTop(true);
            // container pane is always same size as this parent pane,
            // scrolling is actually implemented via positioning and clipping tricks.
            container.SetSize(new USize(UDim.Relative(1.0f), UDim.Relative(1.0f)));

            // subscribe to events we need to hear about
            vertScrollbar.ScrollPositionChanged += HandleScrollChange;
            horzScrollbar.ScrollPositionChanged += HandleScrollChange;

            container.ContentChanged += HandleContentAreaChange;
            container.AutoSizeSettingChanged += HandleAutoSizePaneChanged;

            // finalise setup
            ConfigureScrollbars();
        }

        public override void Destroy()
        {
            var container = GetScrolledContainer();

            // detach from events on content pane
            container.ContentChanged += HandleContentAreaChange;
            container.AutoSizeSettingChanged += HandleAutoSizePaneChanged;

            // now do the cleanup
            base.Destroy();
        }

        /// <summary>
        /// display required integrated scroll bars according to current size of
        /// the ScrollablePane view area and the size of the attached ScrolledContainer.
        /// </summary>
        protected void ConfigureScrollbars()
        {
            // controls should all be valid by this stage
            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            var horzScrollBarWasVisible = horzScrollbar.IsVisible();
            var vertScrollBarWasVisible = vertScrollbar.IsVisible();

            // enable required scrollbars
            vertScrollbar.SetVisible(IsVertScrollbarNeeded());
            horzScrollbar.SetVisible(IsHorzScrollbarNeeded());

            // Check if the addition of the horizontal scrollbar means we
            // now also need the vertical bar.
            if (horzScrollbar.IsVisible())
                vertScrollbar.SetVisible(IsVertScrollbarNeeded());

            if (horzScrollBarWasVisible != horzScrollbar.IsVisible() ||
                vertScrollBarWasVisible != vertScrollbar.IsVisible())
            {
                OnSized(new ElementEventArgs(this));
            }

            PerformChildWindowLayout();

            // get viewable area
            var viewableArea = GetViewableArea();

            // set up vertical scroll bar values
            vertScrollbar.SetDocumentSize(Math.Abs(_contentRect.Height));
            vertScrollbar.SetPageSize(viewableArea.Height);
            vertScrollbar.SetStepSize(Math.Max(1.0f, viewableArea.Height*_vertStep));
            vertScrollbar.SetOverlapSize(Math.Max(1.0f, viewableArea.Height*_vertOverlap));
            vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition());

            // set up horizontal scroll bar values
            horzScrollbar.SetDocumentSize(Math.Abs(_contentRect.Width));
            horzScrollbar.SetPageSize(viewableArea.Width);
            horzScrollbar.SetStepSize(Math.Max(1.0f, viewableArea.Width*_horzStep));
            horzScrollbar.SetOverlapSize(Math.Max(1.0f, viewableArea.Width*_horzOverlap));
            horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition());
        }

        /// <summary>
        /// Return whether the vertical scrollbar is needed.
        /// </summary>
        /// <returns>
        /// - true if the scrollbar is either needed or forced via setting.
        /// - false if the scrollbar should not be shown.
        /// </returns>
        protected bool IsVertScrollbarNeeded()
        {
            return ((Math.Abs(_contentRect.Height) > GetViewableArea().Height) || _forceVertScroll);
        }

        /// <summary>
        /// Return whether the horizontal scrollbar is needed.
        /// </summary>
        /// <returns>
        /// - true if the scrollbar is either needed or forced via setting.
        /// - false if the scrollbar should not be shown.
        /// </returns>
        protected bool IsHorzScrollbarNeeded()
        {
            return ((Math.Abs(_contentRect.Width) > GetViewableArea().Width) || _forceHorzScroll);
        }

        /// <summary>
        /// Update the content container position according to the current 
        /// state of the widget (like scrollbar positions, etc).
        /// </summary>
        protected void UpdateContainerPosition()
        {
            // basePos is the position represented by the scrollbars
            // (these are negated so pane is scrolled in the correct directions)
            var basePos = new UVector2(UDim.Absolute(-GetHorzScrollbar().GetScrollPosition()),
                                       UDim.Absolute(-GetVertScrollbar().GetScrollPosition()));

            // this bias is the absolute position that 0 on the scrollbars represent.
            // Allows the pane to function correctly with negatively positioned content.
            var bias = new UVector2(UDim.Absolute(_contentRect.d_min.X),
                                    UDim.Absolute(_contentRect.d_min.Y));

            // set the new container pane position to be what the scrollbars request
            // minus any bias generated by the location of the content.
            GetScrolledContainer().SetPosition(basePos - bias);
        }

        /// <summary>
        /// Return a pointer to the ScrolledContainer component widget for this ScrollablePane.
        /// </summary>
        /// <returns>Pointer to a ScrolledContainer object.</returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the scrolled container component does not exist.
        /// </exception>
        protected ScrolledContainer GetScrolledContainer()
        {
            return (ScrolledContainer) GetChild(ScrolledContainerName);
        }


        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as ScrollablePaneWindowRenderer) != null;
        }

        /// <summary>
        /// Event trigger method called when some pane content has changed size or location.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnContentPaneChanged(WindowEventArgs e)
        {
            FireEvent(ContentPaneChanged, e);
        }

        /// <summary>
        /// Event trigger method called when the setting that controls whether the 
        /// vertical scrollbar is always shown or not, is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object.
        /// </param>
        protected virtual void OnVertScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(VertScrollbarModeChanged, e);
        }

        /// <summary>
        /// Event trigger method called when the setting that controls whether the 
        /// horizontal scrollbar is always shown or not, is changed.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnHorzScrollbarModeChanged(WindowEventArgs e)
        {
            FireEvent(HorzScrollbarModeChanged, e);
        }

        /// <summary>
        /// Notification method called whenever the setting that controls whether
        /// the content pane is automatically sized is changed.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnAutoSizeSettingChanged(WindowEventArgs e)
        {
            FireEvent(AutoSizeSettingChanged, e);
        }

        /// <summary>
        /// Notification method called whenever the content pane is scrolled via
        /// changes in the scrollbar positions.
        /// </summary>
        /// <param name="e">WindowEventArgs object.</param>
        protected virtual void OnContentPaneScrolled(WindowEventArgs e)
        {
            UpdateContainerPosition();
            FireEvent(ContentPaneScrolled, e);
        }

        /// <summary>
        /// Handler method which gets subscribed to the scrollbar position change events.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleScrollChange(EventArgs e)
        {
            OnContentPaneScrolled(new WindowEventArgs(this));
            return true;
        }

        /// <summary>
        /// Handler method which gets subscribed to the ScrolledContainer content change events.
        /// </summary>
        /// <param name="e"></param>
        protected bool HandleContentAreaChange(EventArgs e)
        {
            // get updated extents of the content
            var contentArea = GetScrolledContainer().GetContentArea();

            // calculate any change on the top and left edges.
            var xChange = contentArea.d_min.X - _contentRect.d_min.X;
            var yChange = contentArea.d_min.Y - _contentRect.d_min.Y;

            // store new content extents information
            _contentRect = contentArea;

            ConfigureScrollbars();

            // update scrollbar positions (which causes container pane to be moved as needed).
            var horzScrollbar = GetHorzScrollbar();
            horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() - xChange);
            var vertScrollbar = GetVertScrollbar();
            vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() - yChange);

            // this call may already have been made if the scroll positions changed.  The call
            // is required here for cases where the top/left 'bias' has changed; in which
            // case the scroll position notification may or may not have been fired.
            if (xChange != 0f || yChange != 0f)
                UpdateContainerPosition();

            // fire event
            OnContentPaneChanged(new WindowEventArgs(this));

            return true;
        }

        /// <summary>
        /// Handler method which gets subscribed to the ScrolledContainer auto-size 
        /// setting changes.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool HandleAutoSizePaneChanged(EventArgs e)
        {
            // just forward event to client.
            var args = new WindowEventArgs(this);
            FireEvent(AutoSizeSettingChanged, args);
            return args.handled > 0;
        }

        protected override void AddChildImpl(Element element)
        {
            var wnd = element as Window;

            if (wnd == null)
                throw new InvalidRequestException(
                    "ScrollablePane can only have Elements of type Window added as children (Window path: " +
                    GetNamePath() + ").");

            if (wnd.IsAutoWindow())
            {
                // This is an internal widget, so should be added normally.
                base.AddChildImpl(wnd);
            }
            else
            {
                // this is a client window/widget, so should be added to the pane container.

                // container should always be valid by the time we're adding client controls
                GetScrolledContainer().AddChild(wnd);
            }
        }

        protected override void RemoveChildImpl(Element element)
        {
            var wnd = (Window) element;

            if (wnd.IsAutoWindow())
            {
                // This is an internal widget, so should be removed normally.
                base.RemoveChildImpl(wnd);
            }
            else
            {
                // this is a client window/widget, so should be removed from the pane container.
                // container should always be valid by the time we're handling client controls
                GetScrolledContainer().RemoveChild(wnd);
            }
        }

        protected override void OnSized(ElementEventArgs e)
        {
            ConfigureScrollbars();
            UpdateContainerPosition();
            base.OnSized(e);

            ++e.handled;
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            // base class processing.
            base.OnScroll(e);

            var vertScrollbar = GetVertScrollbar();
            var horzScrollbar = GetHorzScrollbar();

            if (vertScrollbar.IsEffectiveVisible() &&
                (vertScrollbar.GetDocumentSize() > vertScrollbar.GetPageSize()))
            {
                vertScrollbar.SetScrollPosition(vertScrollbar.GetScrollPosition() +
                                                vertScrollbar.GetStepSize()*-e.scroll);
            }
            else if (horzScrollbar.IsEffectiveVisible() &&
                     (horzScrollbar.GetDocumentSize() > horzScrollbar.GetPageSize()))
            {
                horzScrollbar.SetScrollPosition(horzScrollbar.GetScrollPosition() +
                                                horzScrollbar.GetStepSize()*-e.scroll);
            }

            ++e.handled;
        }

        protected override NamedElement GetChildByNamePathImpl(string namePath)
        {
            // FIXME: This is horrible
            //
            if (namePath.Length > 7 && namePath.Substring(0, 7) == "__auto_")
                return base.GetChildByNamePathImpl(namePath);

            return base.GetChildByNamePathImpl(ScrolledContainerName + '/' + namePath);
        }

        private void AddScrollablePaneProperties()
        {
            // TODO: Inconsistency
            DefineProperty(
                "ForceVertScrollbar",
                "Property to get/set the 'always show' setting for the vertical scroll bar of the tree.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetShowVertScrollbar(v), x => x.IsVertScrollbarAlwaysShown(), false);

            // TODO: Inconsistency
            DefineProperty(
                "ForceHorzScrollbar",
                "Property to get/set the 'always show' setting for the horizontal scroll bar of the tree.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetShowHorzScrollbar(v), x => x.IsHorzScrollbarAlwaysShown(), false);

            // TODO: Inconsistency
            DefineProperty(
                "HorzStepSize",
                "Property to get/set the step size for the horizontal Scrollbar.  Value is a float.",
                (x, v) => x.SetHorizontalStepSize(v), x => x.GetHorizontalStepSize(), 0.1f);

            // TODO: Inconsistency
            DefineProperty(
                "HorzOverlapSize",
                "Property to get/set the overlap size for the horizontal Scrollbar.  Value is a float.",
                (x, v) => x.SetHorizontalOverlapSize(v), x => x.GetHorizontalOverlapSize(), 0.01f);

            DefineProperty(
                "HorzScrollPosition",
                "Property to get/set the scroll position of the horizontal Scrollbar as a fraction.  Value is a float.",
                (x, v) => x.SetHorizontalScrollPosition(v), x => x.GetHorizontalScrollPosition(), 0.0f);

            // TODO: Inconsistency
            DefineProperty(
                "VertStepSize", "Property to get/set the step size for the vertical Scrollbar.  Value is a float.",
                (x, v) => x.SetVerticalStepSize(v), x => x.GetVerticalStepSize(), 0.1f);

            // TODO: Inconsistency
            DefineProperty(
                "VertOverlapSize", "Property to get/set the overlap size for the vertical Scrollbar.  Value is a float.",
                (x, v) => x.SetVerticalOverlapSize(v), x => x.GetVerticalOverlapSize(), 0.01f);

            // TODO: Inconsistency
            DefineProperty(
                "VertScrollPosition",
                "Property to get/set the scroll position of the vertical Scrollbar as a fraction.  Value is a float.",
                (x, v) => x.SetVerticalScrollPosition(v), x => x.GetVerticalScrollPosition(), 0.0f);

            DefineProperty(
                "ContentPaneAutoSized",
                "Property to get/set the setting which controls whether the content pane will auto-size itself.  Value is either \"True\" or \"False\".",
                (x, v) => x.SetContentPaneAutoSized(v), x => x.IsContentPaneAutoSized(), true);

            // TODO: Inconsistency
            DefineProperty(
                "ContentArea",
                "Property to get/set the current content area rectangle of the content pane.  Value is \"l:[float] t:[float] r:[float] b:[float]\" (where l is left, t is top, r is right, and b is bottom).",
                (x, v) => x.SetContentPaneArea(v), x => x.GetContentPaneArea(), Rectf.Zero);
        }

        private void DefineProperty<T>(string name, string help, Action<ScrollablePane, T> setter,
                                       Func<ScrollablePane, T> getter, T defaultValue)
        {
            AddProperty(new TplWindowProperty<ScrollablePane, T>(name, help, setter, getter, WidgetTypeName,
                                                                 defaultValue));
        }

        #region Fields

        /// <summary>
        /// true if vertical scrollbar should always be displayed
        /// </summary>
        private bool _forceVertScroll;

        /// <summary>
        /// true if horizontal scrollbar should always be displayed
        /// </summary>
        private bool _forceHorzScroll;
        
        /// <summary>
        /// holds content area so we can track changes.
        /// </summary>
        private Rectf _contentRect;
        
        /// <summary>
        /// vertical scroll step fraction.
        /// </summary>
        private float _vertStep;
        
        /// <summary>
        /// vertical scroll overlap fraction.
        /// </summary>
        private float _vertOverlap;
        
        /// <summary>
        /// horizontal scroll step fraction.
        /// </summary>
        private float _horzStep;
        
        /// <summary>
        /// horizontal scroll overlap fraction.
        /// </summary>
        private float _horzOverlap;

        #endregion
    }
}