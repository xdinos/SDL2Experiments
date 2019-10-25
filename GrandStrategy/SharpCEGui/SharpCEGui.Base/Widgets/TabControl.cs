using System;
using System.Collections.Generic;

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class for TabControl window renderer objects.
    /// </summary>
    public abstract class TabControlWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected TabControlWindowRenderer(string name)
            : base(name, TabControl.EventNamespace)
        {

        }

        /// <summary>
        /// create and return a pointer to a TabButton widget for use as a clickable tab header
        /// </summary>
        /// <param name="name">
        /// Button name
        /// </param>
        /// <returns>
        /// Pointer to a TabButton to be used for changing tabs.
        /// </returns>
        public abstract TabButton CreateTabButton(string name);
    }

    /// <summary>
    /// Base class for standard Tab Control widget.
    /// </summary>
    public class TabControl : Window
    {
        static TabControl()
        {
            PropertyHelper.RegisterFunction<TabPanePosition>(
                x => (TabPanePosition) Enum.Parse(typeof (TabPanePosition), x, true),
                x => ((TabPanePosition) x).ToString());
        }

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "TabControl";

        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/TabControl";

        /// <summary>
        /// 
        /// </summary>
        public enum TabPanePosition
        {
            /// <summary>
            /// 
            /// </summary>
            Top,

            /// <summary>
            /// 
            /// </summary>
            Bottom
        }

        #region Events

        /// <summary>
        /// Event fired when a different tab is selected.
        /// Handlers are passed a const WindowEventArgs reference with
        /// WindowEventArgs::window set to the TabControl that has a newly
        /// selected tab.
        /// </summary>
        public event EventHandler<WindowEventArgs> SelectionChanged;

        #endregion

        /// <summary>
        /// Widget name for the tab content pane component.
        /// </summary>
        public const string ContentPaneName = "__auto_TabPane__";

        /// <summary>
        /// Widget name for the tab button components.
        /// </summary>
        public const string TabButtonName = "__auto_btn";

        /// <summary>
        /// Widget name for the tab button pane component.
        /// </summary>
        public const string TabButtonPaneName = "__auto_TabPane__Buttons";

        /// <summary>
        /// Widget name for the scroll tabs to right pane component.
        /// </summary>
        public const string ButtonScrollLeft = "__auto_TabPane__ScrollLeft";

        /// <summary>
        /// Widget name for the scroll tabs to left pane component.
        /// </summary>
        public const string ButtonScrollRight = "__auto_TabPane__ScrollRight";

        /// <summary>
        /// Return number of tabs
        /// </summary>
        /// <returns>
        /// the number of tabs currently present.
        /// </returns>
        public int GetTabCount()
        {
            return GetTabPane().GetChildCount();
        }

        /// <summary>
        /// Return the positioning of the tab pane.
        /// </summary>
        /// <returns>
        /// The positioning of the tab window within the tab control.
        /// </returns>
        public TabPanePosition GetTabPanePosition()
        {
            return d_tabPanePos;
        }

        /// <summary>
        /// Change the positioning of the tab button pane.
        /// </summary>
        /// <param name="pos">
        /// The new positioning of the tab pane
        /// </param>
        public void SetTabPanePosition(TabPanePosition pos)
        {
            d_tabPanePos = pos;
            PerformChildWindowLayout();
        }

        /// <summary>
        /// Set the selected tab by the name of the root window within it.
        /// Also ensures that the tab is made visible (tab pane is scrolled if required).
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if there's no tab named \a name.
        /// </exception>
        public void SetSelectedTab(string name)
        {
            SelectTabImpl(GetTabPane().GetChild(name));
        }

        /// <summary>
        /// Set the selected tab by the ID of the root window within it.
        /// Also ensures that the tab is made visible (tab pane is scrolled if required).
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public void SetSelectedTab(int id)
        {
            SelectTabImpl(GetTabPane().GetChild(id));
        }

        /// <summary>
        /// Set the selected tab by the index position in the tab control.
        /// Also ensures that the tab is made visible (tab pane is scrolled if required).
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public void SetSelectedTabAtIndex(int index)
        {
            SelectTabImpl(GetTabContentsAtIndex(index));
        }

        /// <summary>
        /// Ensure that the tab by the name of the root window within it is visible.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if there's no tab named \a name.
        /// </exception>
        public void MakeTabVisible(string name)
        {
            MakeTabVisibleImpl(GetTabPane().GetChild(name));
        }

        /// <summary>
        /// Ensure that the tab by the ID of the root window within it is visible.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public void MakeTabVisible(int id)
        {
            MakeTabVisibleImpl(GetTabPane().GetChild(id));
        }

        /// <summary>
        /// Ensure that the tab by the index position in the tab control is visible.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public void MakeTabVisibleAtIndex(int index)
        {
            MakeTabVisibleImpl(GetTabContentsAtIndex(index));
        }

        /// <summary>
        /// Return the Window which is the first child of the tab at index position \a index.
        /// </summary>
        /// <param name="index">
        /// Zero based index of the item to be returned.
        /// </param>
        /// <returns>
        /// Pointer to the Window at index position \a index in the tab control.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a index is out of range.
        /// </exception>
        public Window GetTabContentsAtIndex(int index)
        {
            return index >= d_tabButtonVector.Count
                       ? null
                       : d_tabButtonVector[index].GetTargetWindow();
        }

        /// <summary>
        /// Return the Window which is the tab content with the given name.
        /// </summary>
        /// <param name="name">
        /// Name of the Window which was attached as a tab content.
        /// </param>
        /// <returns>
        /// Pointer to the named Window in the tab control.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if content is not found.
        /// </exception>
        public Window GetTabContents(string name)
        {
            return GetTabPane().GetChild(name);
        }

        /// <summary>
        /// Return the Window which is the tab content with the given ID.
        /// </summary>
        /// <param name="id">
        /// ID of the Window which was attached as a tab content.
        /// </param>
        /// <returns>
        /// Pointer to the Window with the given ID in the tab control.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if content is not found.
        /// </exception>
        public Window GetTabContents(int id)
        {
            return GetTabPane().GetChild(id);
        }

        /// <summary>
        /// Return whether the tab contents window is currently selected.
        /// </summary>
        /// <param name="wnd">
        /// The tab contents window to query.
        /// </param>
        /// <returns>
        /// true if the tab is currently selected, false otherwise.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a wnd is not a valid tab contents window.
        /// </exception>
        public bool IsTabContentsSelected(Window wnd)
        {
            return GetButtonForTabContents(wnd).IsSelected();
        }

        /// <summary>
        /// Return the index of the currently selected tab.
        /// </summary>
        /// <returns>
        /// index of the currently selected tab.
        /// </returns>
        public int GetSelectedTabIndex()
        {
            for (var i = 0; i < d_tabButtonVector.Count; ++i)
                if (d_tabButtonVector[i].IsSelected())
                    return i;

            throw new UnknownObjectException("Current tab not in list?");
        }

        /// <summary>
        /// Return the height of the tabs
        /// </summary>
        /// <returns></returns>
        public UDim GetTabHeight()
        {
            return d_tabHeight;
        }

        /// <summary>
        /// Return the amount of padding to add either side of the text in the tab
        /// </summary>
        /// <returns></returns>
        public UDim GetTabTextPadding()
        {
            return d_tabPadding;
        }

        /// <summary>
        /// Initialise the Window based object ready for use.
        /// </summary>
        /// <remarks>
        /// This must be called for every window created.  
        /// Normally this is handled automatically by the WindowFactory for each Window type.
        /// </remarks>
        protected override void InitialiseComponents()
        {
            PerformChildWindowLayout();

            if (IsChild(ButtonScrollLeft))
                ((PushButton) GetChild(ButtonScrollLeft)).Clicked += HandleScrollPane;

            if (IsChild(ButtonScrollRight))
                ((PushButton) GetChild(ButtonScrollRight)).Clicked += HandleScrollPane;
        }

        /// <summary>
        /// Set the height of the tabs
        /// </summary>
        /// <param name="height"></param>
        public void SetTabHeight(UDim height)
        {
            d_tabHeight = height;
            PerformChildWindowLayout();
        }

        /// <summary>
        /// Set the amount of padding to add either side of the text in the tab
        /// </summary>
        /// <param name="padding"></param>
        public void SetTabTextPadding(UDim padding)
        {
            d_tabPadding = padding;

            PerformChildWindowLayout();
        }

        /// <summary>
        /// Add a new tab to the tab control.
        /// <para>
        /// The new tab will be added with the same text as the window passed in.
        /// </para>
        /// </summary>
        /// <param name="wnd">
        /// The Window which will be placed in the content area of this new tab.
        /// </param>
        public void AddTab(Window wnd)
        {
            // abort attempts to add null window pointers, but log it for tracking.
            if (wnd == null)
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          "Attempt to add null window pointer as tab to TabControl '" + GetName() + "'.  Ignoring!",
                          LoggingLevel.Informative);

                return;
            }

            // Create a new TabButton
            AddButtonForTabContent(wnd);
            // Add the window to the content pane
            GetTabPane().AddChild(wnd);
            // Auto-select?
            if (GetTabCount() == 1)
                SetSelectedTab(wnd.GetName());
            else
                // initialise invisible content
                wnd.SetVisible(false);

            // when adding the 1st page, autosize tab pane height
            if (d_tabHeight.d_scale == 0 && d_tabHeight.d_offset == -1)
                d_tabHeight.d_offset = 8 + GetFont().GetFontHeight();

            // Just request redraw
            PerformChildWindowLayout();
            Invalidate(false);

            // Subscribe to text changed event so that we can resize as needed
            wnd.TextChanged += HandleContentWindowTextChanged;
            //d_eventConnections[wnd] =
            //    wnd->subscribeEvent(Window::EventTextChanged,
            //        Event::Subscriber(&TabControl::handleContentWindowTextChanged,
            //                          this));
        }

        /// <summary>
        /// Remove the named tab from the tab control.
        /// <para>
        /// The tab content will be destroyed.
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTab(string name)
        {
            // do nothing if given window is not attached as a tab.
            if (GetTabPane().IsChild(name))
                RemoveTabImpl(GetTabPane().GetChild(name));
        }

        /// <summary>
        /// Remove the tab with the given ID from the tab control.
        /// <para>
        /// The tab content will be destroyed.
        /// </para>
        /// </summary>
        /// <param name="id"></param>
        public void RemoveTab(int id)
        {
            // do nothing if given window is not attached as a tab.
            if (GetTabPane().IsChild(id))
                RemoveTabImpl(GetTabPane().GetChild(id));
        }

        /// <summary>
        /// Constructor for TabControl base class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public TabControl(string type, string name)
            : base(type, name)
        {
            d_tabHeight = new UDim(0, -1f); // means 'to be initialized later'
            d_tabPadding = new UDim(0, 5f);
            d_firstTabOffset = 0;
            d_tabPanePos = TabPanePosition.Top;
            AddTabControlProperties();
        }

        /// <summary>
        /// Perform the actual rendering for this Window.
        /// </summary>
        /// <param name="ctx">
        /// float value specifying the base Z co-ordinate that should be used when rendering
        /// </param>
        protected override void DrawSelf(RenderingContext ctx)
        {
            // do nothing; rendering handled by children
        }

        /// <summary>
        /// Add a TabButton for the specified child Window.
        /// </summary>
        /// <param name="wnd"></param>
        protected virtual void AddButtonForTabContent(Window wnd)
        {
            // Create the button
            var tb = CreateTabButton(MakeButtonName(wnd));

            // Copy font
            tb.SetFont(GetFont());

            // Set target window
            tb.SetTargetWindow(wnd);

            // Instert into map
            d_tabButtonVector.Add(tb);

            // add the button
            GetTabButtonPane().AddChild(tb);

            // Subscribe to clicked event so that we can change tab
            tb.Clicked += HandleTabButtonClicked;
            tb.Dragged += HandleDraggedPane;
            tb.Scrolled += HandleWheeledPane;
        }

        /// <summary>
        /// Remove the TabButton for the specified child Window.
        /// </summary>
        /// <param name="wnd"></param>
        protected virtual void RemoveButtonForTabContent(Window wnd)
        {
            // get
            var tb = (TabButton) GetTabButtonPane().GetChild(MakeButtonName(wnd));

            // remove
            d_tabButtonVector.Remove(tb);
            GetTabButtonPane().RemoveChild(tb);

            // destroy
            WindowManager.GetSingleton().DestroyWindow(tb);
        }

        /// <summary>
        /// Return the TabButton associated with this Window.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if content is not found.
        /// </exception>
        protected TabButton GetButtonForTabContents(Window wnd)
        {
            foreach (var tabButton in d_tabButtonVector)
                if (tabButton.GetTargetWindow() == wnd)
                    return tabButton;

            throw new UnknownObjectException("The Window object is not a tab contents.");
        }

        /// <summary>
        /// Construct a button name to handle a window.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        protected String MakeButtonName(Window wnd)
        {
            return TabButtonName + wnd.GetName();
        }

        /// <summary>
        /// Internal implementation of select tab.
        /// </summary>
        /// <param name="wnd">
        /// Pointer to a Window which is the root of the tab content to select
        /// </param>
        protected virtual void SelectTabImpl(Window wnd)
        {
            MakeTabVisibleImpl(wnd);

            var modified = false;
            // Iterate in order of tab index
            foreach (var tb in d_tabButtonVector)
            {
                var child = tb.GetTargetWindow();

                // Should we be selecting?
                var selectThis = (child == wnd);

                // Are we modifying this tab?
                modified = modified || (tb.IsSelected() != selectThis);

                // Select tab & set visible if this is the window, not otherwise
                tb.SetSelected(selectThis);
                child.SetVisible(selectThis);
            }

            // Trigger event?
            if (modified)
                OnSelectionChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Internal implementation of make tab visible.
        /// </summary>
        /// <param name="wnd">
        /// Pointer to a Window which is the root of the tab content to make visible
        /// </param>
        protected virtual void MakeTabVisibleImpl(Window wnd)
        {
            TabButton tb = null;

            foreach (var item in d_tabButtonVector)
            {
                // get corresponding tab button and content window
                tb = item;
                var child = tb.GetTargetWindow();
                if (child == wnd)
                    break;

                tb = null;
            }

            if (tb == null)
                return;

            var ww = GetPixelSize().Width;
            var x = CoordConverter.AsAbsolute(tb.GetPosition().d_x, ww);
            var w = tb.GetPixelSize().Width;
            float lx = 0f, rx = ww;

            if (IsChild(ButtonScrollLeft))
            {
                var scrollLeftBtn = GetChild(ButtonScrollLeft);
                lx = CoordConverter.AsAbsolute(scrollLeftBtn.GetArea().d_max.d_x, ww);
                scrollLeftBtn.SetWantsMultiClickEvents(false);
            }

            if (IsChild(ButtonScrollRight))
            {
                var scrollRightBtn = GetChild(ButtonScrollRight);
                rx = CoordConverter.AsAbsolute(scrollRightBtn.GetPosition().d_x, ww);
                scrollRightBtn.SetWantsMultiClickEvents(false);
            }

            if (x < lx)
                d_firstTabOffset += lx - x;
            else
            {
                if (x + w <= rx)
                    return; // nothing to do

                d_firstTabOffset += rx - (x + w);
            }

            PerformChildWindowLayout();
        }

        /// <summary>
        /// Return a pointer to the tab button pane (Window) for this TabControl.
        /// </summary>
        /// <returns>
        /// Pointer to a Window object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the component does not exist.
        /// </exception>
        protected Window GetTabButtonPane()
        {
            return GetChild(TabButtonPaneName);
        }

        /// <summary>
        /// Return a pointer to the content component widget for this TabControl.
        /// </summary>
        /// <returns>
        /// Pointer to a Window object.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the component does not exist.
        /// </exception>
        protected Window GetTabPane()
        {
            return GetChild(ContentPaneName);
        }

        public override void PerformChildWindowLayout(bool nonClientSizedHint = false, bool clientSizedHint = false)
        {
            var tabButtonPane = GetTabButtonPane();
            var tabContentPane = GetTabPane();

            // Enable top/bottom edges of the tabPane control,
            // if supported by looknfeel
            if (tabContentPane.IsPropertyPresent(EnableTop))
                tabContentPane.SetProperty(EnableTop, (d_tabPanePos == TabPanePosition.Top) ? n0 : n1);
            if (tabContentPane.IsPropertyPresent(EnableBottom))
                tabContentPane.SetProperty(EnableBottom, (d_tabPanePos == TabPanePosition.Top) ? n1 : n0);
            if (tabButtonPane.IsPropertyPresent(EnableTop))
                tabButtonPane.SetProperty(EnableTop, (d_tabPanePos == TabPanePosition.Top) ? n0 : n1);
            if (tabButtonPane.IsPropertyPresent(EnableBottom))
                tabButtonPane.SetProperty(EnableBottom, (d_tabPanePos == TabPanePosition.Top) ? n1 : n0);

            base.PerformChildWindowLayout(nonClientSizedHint, clientSizedHint);

            // Calculate the size & position of the tab scroll buttons
            Window scrollLeftBtn = null, scrollRightBtn = null;
            if (IsChild(ButtonScrollLeft))
                scrollLeftBtn = GetChild(ButtonScrollLeft);

            if (IsChild(ButtonScrollRight))
                scrollRightBtn = GetChild(ButtonScrollRight);

            // Calculate the positions and sizes of the tab buttons
            if (d_firstTabOffset > 0)
                d_firstTabOffset = 0;

            for (;;)
            {
                int i;
                for (i = 0; i < d_tabButtonVector.Count; ++i)
                    CalculateTabButtonSizePosition(i);

                if (d_tabButtonVector.Count == 0)
                {
                    if (scrollRightBtn != null)
                        scrollRightBtn.SetVisible(false);
                    if (scrollLeftBtn != null)
                        scrollLeftBtn.SetVisible(false);
                    break;
                }

                // Now check if tab pane wasn't scrolled too far
                --i;
                var xmax = d_tabButtonVector[i].GetXPosition().d_offset + d_tabButtonVector[i].GetPixelSize().Width;
                var width = tabButtonPane.GetPixelSize().Width;

                // If right button margin exceeds right window margin,
                // or leftmost button is at offset 0, finish
                if ((xmax > (width - 0.5)) || (d_firstTabOffset == 0f))
                {
                    if (scrollLeftBtn != null)
                        scrollLeftBtn.SetVisible(d_firstTabOffset < 0f);
                    if (scrollRightBtn != null)
                        scrollRightBtn.SetVisible(xmax > width);
                    break;
                }

                // Scroll the tab pane until the rightmost button
                // touches the right margin
                d_firstTabOffset += width - xmax;
                // If we scrolled it too far, set leftmost button offset to 0
                if (d_firstTabOffset > 0)
                    d_firstTabOffset = 0;
            }
        }

        protected override int WriteChildWindowsXML(XMLSerializer xmlStream)
        {
            int childOutputCount = base.WriteChildWindowsXML(xmlStream);

            // since TabControl content is actually added to the component tab
            // content pane window, this overridden function exists to dump those
            // out as if they were our own children.
            for (var i = 0; i < GetTabCount(); ++i)
            {
                GetTabContentsAtIndex(i).WriteXMLToStream(xmlStream);
                ++childOutputCount;
            }

            return childOutputCount;
        }

        protected override bool ValidateWindowRenderer(WindowRenderer renderer)
        {
            return (renderer as TabControlWindowRenderer) != null;
        }

        /// <summary>
        /// create and return a pointer to a TabButton widget for use as a clickable tab header
        /// </summary>
        /// <param name="name">
        /// Button name
        /// </param>
        /// <returns>
        /// Pointer to a TabButton to be used for changing tabs.
        /// </returns>
        protected TabButton CreateTabButton(string name)
        {
            if (d_windowRenderer != null)
            {
                var wr = (TabControlWindowRenderer) d_windowRenderer;
                return wr.CreateTabButton(name);
            }

            throw new InvalidRequestException("This function must be implemented by the window renderer module");
        }

        /// <summary>
        /// Implementation function to do main work of removing a tab.
        /// </summary>
        /// <param name="window"></param>
        protected void RemoveTabImpl(Window window)
        {
            // silently abort if window to be removed is 0.
            if (window == null)
                return;

            // delete connection to event we subscribed earlier
            window.TextChanged -= HandleContentWindowTextChanged;
            // d_eventConnections.erase(window);

            // Was this selected?
            bool reselect = window.IsEffectiveVisible();
            // Tab buttons are the 2nd onward children
            GetTabPane().RemoveChild(window);

            // remove button too
            RemoveButtonForTabContent(window);

            if (reselect && (GetTabCount() > 0))
                // Select another tab
                SetSelectedTab(GetTabPane().GetChildAtIdx(0).GetName());

            PerformChildWindowLayout();

            Invalidate(false);
        }

        /// <summary>
        /// Handler called internally when the currently selected item or items changes.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(WindowEventArgs e)
        {
            Invalidate(false);
            FireEvent(SelectionChanged, e);
        }

        /// <summary>
        /// Handler called when the window's font is changed.
        /// </summary>
        /// <param name="e">
        /// WindowEventArgs object whose 'window' pointer field is set to the window that triggered the event.  
        /// For this event the trigger window is always 'this'.
        /// </param>
        protected internal override void OnFontChanged(WindowEventArgs e)
        {
            // Propagate font change to buttons
            foreach (var tabButton in d_tabButtonVector)
                tabButton.SetFont(GetFont());
        }

        /// <summary>
        /// Calculate the correct position and size of a tab button, based on the
        /// index it is due to be placed at.
        /// </summary>
        /// <param name="index">
        /// The index of the tab button
        /// </param>
        protected void CalculateTabButtonSizePosition(int index)
        {
            var btn = d_tabButtonVector[index];

            // relative height is always 1.0 for buttons since they are embedded in a
            // panel of the correct height already
            var position = new UVector2(UDim.Absolute(0.0f), UDim.Absolute(0.0f));
            var size = new USize(UDim.Absolute(0.0f), UDim.Relative(1.0f));

            // x position is based on previous button
            if (index == 0)
            {
                // First button
                position.d_x = UDim.Absolute(d_firstTabOffset);
            }
            else
            {
                var prevButton = d_tabButtonVector[index - 1];

                // position is prev pos + width
                position.d_x = prevButton.GetArea().d_max.d_x;
            }

            size.d_width = UDim.Absolute(btn.GetRenderedString().GetHorizontalExtent(btn)) + GetTabTextPadding() +
                           GetTabTextPadding();

            btn.SetPosition(position);
            btn.SetSize(size);

            var leftX = position.d_x.d_offset;
            btn.SetVisible((leftX < GetPixelSize().Width) && (leftX + btn.GetPixelSize().Width > 0));
            btn.Invalidate(false);
        }

        private void AddTabControlProperties()
        {
            AddProperty(new TplWindowProperty<TabControl, UDim>(
                            "TabHeight",
                            "Property to get/set the height of the tabs.",
                            (x, v) => x.SetTabHeight(v), x => x.GetTabHeight(), WidgetTypeName, new UDim(0.05f, 0.0f)));

            AddProperty(new TplWindowProperty<TabControl, UDim>(
                            "TabTextPadding",
                            "Property to get/set the padding either side of the tab buttons.",
                            (x, v) => x.SetTabTextPadding(v), x => GetTabTextPadding(), WidgetTypeName,
                            new UDim(0.0f, 5f)));

            AddProperty(new TplWindowProperty<TabControl, TabPanePosition>(
                            "TabPanePosition",
                            "Property to get/set the position of the buttons pane.",
                            (x, v) => x.SetTabPanePosition(v), x => x.GetTabPanePosition(), WidgetTypeName,
                            TabPanePosition.Top));
        }

        protected override void AddChildImpl(Element element)
        {
            var wnd = element as Window;

            if (wnd == null)
                throw new InvalidRequestException(
                    "TabControl can only have Elements of type Window added as children (Window path: " + GetNamePath() +
                    ").");

            if (wnd.IsAutoWindow())
            {
                // perform normal addChild
                base.AddChildImpl(wnd);
            }
            else
            {
                // This is another control, therefore add as a tab
                AddTab(wnd);
            }
        }

        protected override void RemoveChildImpl(Element element)
        {
            var wnd = element as Window;

            // protect against possible null pointers
            if (wnd == null) return;

            if (wnd.IsAutoWindow())
            {
                // perform normal removeChild
                base.RemoveChildImpl(wnd);
            }
            else
            {
                // This is some user window, therefore remove as a tab
                RemoveTab(wnd.GetName());
            }
        }

        protected override NamedElement GetChildByNamePathImpl(string namePath)
        {
            // FIXME: This is horrible
            //
            if (namePath.Length > 7 && namePath.Substring(0, 7) == "__auto_")
                return base.GetChildByNamePathImpl(namePath);
            else
                return base.GetChildByNamePathImpl(ContentPaneName + '/' + namePath);
        }

        #region Event Handlres

        protected bool HandleContentWindowTextChanged(EventArgs args)
        {
            // update text
            var wa = (WindowEventArgs) args;
            var tabButton = GetTabButtonPane().GetChild(MakeButtonName(wa.Window));
            tabButton.SetText(wa.Window.GetText());
            // sort out the layout
            PerformChildWindowLayout();
            Invalidate(false);

            return true;
        }

        protected void /*bool*/ HandleTabButtonClicked(object sender, WindowEventArgs args)
        {
            var tabButton = (TabButton) args.Window;
            SetSelectedTab(tabButton.GetTargetWindow().GetName());

            // TODO: return true;
        }

        protected bool HandleScrollPane(EventArgs e)
        {
            int i;
            float delta = 0;

            // Find the leftmost visible button
            for (i = 0; i < d_tabButtonVector.Count; ++i)
            {
                if (d_tabButtonVector[i].IsVisible())
                    break;
                delta = d_tabButtonVector[i].GetPixelSize().Width;
            }

            if (((WindowEventArgs) e).Window.GetName() == ButtonScrollLeft)
            {
                if (delta == 0.0f && i < d_tabButtonVector.Count)
                    delta = d_tabButtonVector[i].GetPixelSize().Width;

                // scroll button pane to the right
                d_firstTabOffset += delta;
            }
            else if (i < d_tabButtonVector.Count)
                // scroll button pane to left
                d_firstTabOffset -= d_tabButtonVector[i].GetPixelSize().Width;

            PerformChildWindowLayout();

            return true;
        }

        protected void /*bool*/ HandleDraggedPane(object sender, CursorInputEventArgs e)
        {
            if (e.Source == CursorInputSource.Left)
            {
                // This is the middle-mouse-click event, remember initial drag position
                var butPane = GetTabButtonPane();
                d_btGrabPos = (e.Position.X - butPane.GetOuterRectClipper().d_min.X) - d_firstTabOffset;
            }
            else if (e.Source == CursorInputSource.None)
            {
                // Regular mouse move event
                var butPane = GetTabButtonPane();
                var newTo = (e.Position.X - butPane.GetOuterRectClipper().d_min.X) - d_btGrabPos;
                if ((newTo < d_firstTabOffset - 0.9) ||
                    (newTo > d_firstTabOffset + 0.9))
                {
                    d_firstTabOffset = newTo;
                    PerformChildWindowLayout();
                }
            }

            // TODO: return true;
        }

        protected void /*bool*/ HandleWheeledPane(object sender, CursorInputEventArgs e)
        {
            var butPane = GetTabButtonPane();
            var delta = butPane.GetOuterRectClipper().Width/20f;

            d_firstTabOffset += e.scroll*delta;
            PerformChildWindowLayout();

            // TODO: return true;
        }

        #endregion

        #region Fields

        private UDim d_tabHeight; //!< The height of the tabs in pixels
        private UDim d_tabPadding; //!< The padding of the tabs relative to parent

        private float d_firstTabOffset; //!< The offset in pixels of the first tab

        private TabPanePosition d_tabPanePos; //!< The position of the tab pane

        private float d_btGrabPos; //!< The position on the button tab where user grabbed

        /// <summary>
        /// Sorting for tabs
        /// </summary>
        private readonly List<TabButton> d_tabButtonVector = new List<TabButton>();

        ////! Container used to track event subscriptions to added tab windows.
        // TODO: std::map<Window, Event::ScopedConnection> d_eventConnections;

        #endregion

        #region Constants

        private const string EnableTop = "EnableTop";
        private const string EnableBottom = "EnableBottom";
        private const string n0 = "0";
        private const string n1 = "1";

        #endregion
    }
}