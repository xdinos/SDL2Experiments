using System;
using System.Collections.Generic;
using System.Linq;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// This renderer interface provides data for the views to aid in the rendering
    /// process.
    /// </summary>
    public abstract class ItemViewWindowRenderer : WindowRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected ItemViewWindowRenderer(string name)
                : base(name, "ItemView")
        { }

        /// <summary>
        /// Returns a Rectf object describing, in unclipped pixels, the window
        /// relative area that is to be used for rendering the view.
        /// </summary>
        /// <returns></returns>
        public abstract Rectf GetViewRenderArea();

        /// <summary>
        /// Resize the view such that its content can be displayed without needing
        /// scrollbars if there is enough space, otherwise make the view as
        /// large as possible (without moving it).
        /// </summary>
        /// <param name="fitWidth"></param>
        /// <param name="fitHeight"></param>
        public abstract void ResizeViewToContent(bool fitWidth, bool fitHeight);

        public virtual void AutoPositionSize(Lunatics.Mathematics.Vector2 position, Sizef size) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ItemView GetView()
        {
            return (ItemView)Window;
        }
    }

    /// <summary>
    /// This enumeration controls the \b display mode of the scrollbar, that is,
    /// it's visibility. This does not affect the scrollbar behaviour. For example,
    /// even if the scrollbar is SDM_Hidden, one can still scroll it if the size
    /// allows it (content is bigger than the view).
    /// </summary>
    public enum ScrollbarDisplayMode
    {
        /// <summary>
        /// The scrollbar will be shown always, even if the content is smaller than
        /// the view.
        /// </summary>
        Shown,

        /// <summary>
        /// The scrollbar will be hidden, even if the content is bigger than the
        /// view and scrolling is possible.
        /// </summary>
        Hidden,

        /// <summary>
        /// The scrollbar will be shown only if the underlining view's size is too
        /// small to contain its items.
        /// </summary>
        WhenNeeded
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ViewSortMode
    {
        /// <summary>
        /// Items are not sorted, but shown in the same order as they are provided by the model.
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Ascending,

        /// <summary>
        /// 
        /// </summary>
        Descending
    }

    // TODO: ...
    //template<>
    //class CEGUIEXPORT PropertyHelper<ScrollbarDisplayMode>
    //{
    //public:
    //    typedef ScrollbarDisplayMode return_type;
    //    typedef return_type safe_method_return_type;
    //    typedef ScrollbarDisplayMode pass_type;
    //    typedef String string_return_type;

    //    static const String& getDataTypeName();

    //    static return_type fromString(const String& str);
    //    static string_return_type toString(pass_type val);
    //};

    //template<>
    //class CEGUIEXPORT PropertyHelper<ViewSortMode>
    //{
    //public:
    //    typedef ViewSortMode return_type;
    //    typedef return_type safe_method_return_type;
    //    typedef ViewSortMode pass_type;
    //    typedef String string_return_type;

    //    static const String& getDataTypeName();

    //    static return_type fromString(const String& str);
    //    static string_return_type toString(pass_type val);
    //};

    /// <summary>
    /// Stores the selection state of a ModelIndex. This is used to regenerate
    /// the new proper selection index when the model changes in any way (e.g.:
    /// new item, removed item).
    /// </summary>
    public class/*struct*/ ModelIndexSelectionState
    {
        public ModelIndex d_parentIndex;
        public int d_childId;

        public ModelIndex d_selectedIndex;
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public class ItemViewEventArgs : WindowEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wnd"></param>
        public ItemViewEventArgs(ItemView wnd)
                : this(wnd, new ModelIndex())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="index"></param>
        public ItemViewEventArgs(ItemView wnd, ModelIndex index)
                : base(wnd)
        {
            d_index = index;
        }

        /// <summary>
        /// The index affected by the event.
        /// </summary>
        private ModelIndex d_index;
    }

    /// <summary>
    /// Abstract base class for all view classes that use an ItemModel to provide
    /// the data to be rendered. In order for a view to properly display data,
    /// the setModel(ItemModel*) function should be called with an instance of the
    /// model.
    /// 
    /// This class is mean to be inherited by a specific view before being used.
    /// </summary>
    public abstract class ItemView : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        protected ItemView(string type, string name)
                : base(type, name)
        {
            d_itemModel = null;
            d_textColourRect = new ColourRect(DefaultTextColour);
            _selectionTextColourRect = new ColourRect(DefaultTextColour);
            d_selectionColourRect = new ColourRect(DefaultSelectionColour);
            d_isDirty = true;
            d_needsFullRender = true;
            d_lastSelectedIndex = null;
            d_selectionBrush = null;
            d_vertScrollbarDisplayMode = ScrollbarDisplayMode.WhenNeeded;
            d_horzScrollbarDisplayMode = ScrollbarDisplayMode.WhenNeeded;
            d_isItemTooltipsEnabled = false;
            d_isMultiSelectEnabled = false;
            d_sortMode = ViewSortMode.None;
            d_isAutoResizeHeightEnabled = false;
            d_isAutoResizeWidthEnabled = false;
            d_renderedMaxWidth = 0;
            d_renderedTotalHeight = 0;
            // TODO: d_eventChildrenAddedConnection = null;
            // TODO: d_eventChildrenRemovedConnection = null;

            AddItemViewProperties();
        }

        // TODO: 
        //virtual ~ItemView()
        //{
        //    DisconnectModelEvents();
        //}

        public readonly Colour DefaultTextColour = new Colour(0xFFFFFFFF);
        public readonly Colour DefaultSelectionColour = new Colour(0xFF4444AA);

        /// <summary>
        /// Widget name for the vertical scrollbar component.
        /// </summary>
        public const string VertScrollbarName = "__auto_vscrollbar__";

        /// <summary>
        /// Widget name for the horizontal scrollbar component.
        /// </summary>
        public const string HorzScrollbarName = "__auto_hscrollbar__";

        public const string EventVertScrollbarDisplayModeChanged = "VertScrollbarDisplayModeChanged";
        public event GuiEventHandler<EventArgs> VertScrollbarDisplayModeChanged
        {
            add { SubscribeEvent(EventVertScrollbarDisplayModeChanged, value); }
            remove { UnsubscribeEvent(EventVertScrollbarDisplayModeChanged, value); }
        }

        public const string EventHorzScrollbarDisplayModeChanged = "HorzScrollbarDisplayModeChanged";
        public event GuiEventHandler<EventArgs> HorzScrollbarDisplayModeChanged
        {
            add { SubscribeEvent(EventHorzScrollbarDisplayModeChanged, value); }
            remove { UnsubscribeEvent(EventHorzScrollbarDisplayModeChanged, value); }
        }
        
        public const string EventSelectionChanged = "SelectionChanged";
        public event GuiEventHandler<EventArgs> SelectionChanged
        {
            add { SubscribeEvent(EventSelectionChanged, value); }
            remove { UnsubscribeEvent(EventSelectionChanged, value); }
        }

        public const string EventMultiselectModeChanged = "MultiselectModeChanged";
        
        public const string EventSortModeChanged = "SortModeChanged";
        public event GuiEventHandler<EventArgs> SortModeChanged
        {
            add { SubscribeEvent(EventSortModeChanged, value); }
            remove { UnsubscribeEvent(EventSortModeChanged, value); }
        }

        /// <summary>
        /// Triggered when items are added, removed or when the view's item are cleared.
        /// </summary>
        public const string EventViewContentsChanged = "ViewContentsChanged";
        public event GuiEventHandler<EventArgs> ViewContentsChanged
        {
            add { SubscribeEvent(EventViewContentsChanged, value); }
            remove { UnsubscribeEvent(EventViewContentsChanged, value); }
        }

        /// <summary>
        /// Sets the ItemModel to be used inside this view.
        /// </summary>
        /// <param name="itemModel"></param>
        public virtual void SetModel(ItemModel itemModel)
        {
            if (itemModel == d_itemModel)
                return;

            if (d_itemModel != null)
            {
                DisconnectModelEvents();
                d_lastSelectedIndex = new ModelIndex(0);
            }

            d_itemModel = itemModel;

            ConnectToModelEvents();
            d_indexSelectionStates.Clear();
            d_needsFullRender = true;

            OnSelectionChanged(new ItemViewEventArgs(this));
        }

        /// <summary>
        /// Returns the current ItemModel of this view.
        /// </summary>
        /// <returns></returns>
        public virtual ItemModel GetModel()
        {
            return d_itemModel;
        }

        /// <summary>
        /// Instructs this ItemView to prepare its rendering state for rendering.
        /// This is usually done by updating the rendering state if it got dirty
        /// in the meantime.
        /// </summary>
        public virtual void PrepareForRender()
        {
            
        }

        /// <summary>
        /// Gets the colour used for rendering the text.
        /// </summary>
        /// <returns></returns>
        public ColourRect GetTextColourRect()
        {
            return d_textColourRect;
        }

        /// <summary>
        /// Sets the colour used for rendering the text.
        /// </summary>
        /// <param name="colourRect"></param>
        public void SetTextColourRect(ColourRect colourRect)
        {
            d_textColourRect = colourRect;
        }

        public void SetTextColour(Colour colour)
        {
            SetTextColourRect(new ColourRect(colour));
        }

        public void SetSelectionTextColour(Colour colour)
        {
            SetSelectionTextColourRect(new ColourRect(colour));
        }

        public void SetSelectionTextColourRect(ColourRect colourRect)
        {
            _selectionTextColourRect = colourRect;
        }

        /// <summary>
        /// Gets the colour used for highlighting the selection.
        /// </summary>
        /// <returns></returns>
        public ColourRect GetSelectionColourRect()
        {
            return d_selectionColourRect;
        }

        public void SetSelectionColour(Colour colour)
        {
            SetSelectionColourRect(new ColourRect(colour));
        }

        /// <summary>
        /// Sets the colour used for highlighting the selection.
        /// </summary>
        /// <param name="colourRect"></param>
        public void SetSelectionColourRect(ColourRect colourRect)
        {
            d_selectionColourRect = colourRect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            return d_isDirty;
        }

        /// <summary>
        /// Specifies whether this view requires processing before being able to render it.
        /// </summary>
        /// <param name="value"></param>
        public void SetIsDirty(bool value)
        {
            d_isDirty = value;
        }

        /// <summary>
        /// 
        /// Invalidates this view by marking the rendering state as dirty. That means
        /// that the next call to prepareForRender() will have to reconstruct it's
        /// rendering state.
        /// 
        /// This also calls the base Window::invalidate.
        /// </summary>
        /// <param name="recursive"></param>

        public virtual void InvalidateView(bool recursive)
        {
            //TODO: allow invalidation only of certain parts (e.g.: items/indices)
            UpdateScrollbars();
            ResizeToContent();
            SetIsDirty(true);
            Invalidate(recursive);
        }

        /// <summary>
        /// Gets the current state of the indices used for selection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This vector's iterator might get invalidated in the case when items are
        /// removed from the underlying ItemModel while iterating over this vector.
        /// 
        /// In the following example you can see a way of removing the selecting items:
        /// \code
        /// while(!view->getIndexSelectionStates().empty())
        /// {
        ///     ModelIndexSelectionState& state = view->getIndexSelectionStates().back();
        ///     view->getIndexSelectionStates().pop_back();
        ///     // remove item from model
        /// }
        /// \endcode
        /// </remarks>
        public IEnumerable<ModelIndexSelectionState> GetIndexSelectionStates()
        {
            return d_indexSelectionStates;
        }

        /*!
    \brief
        Returns the ModelIndex of the item at the specified position.

    \param position
        The position is expected to be in screen coordinates - it will be converted
        to window coordinates internally.

    \return
        The ModelIndex for the position or a default-constructed ModelIndex
        if nothing was found at that position or if the position is outside
        the view's rendering area.
    */
        public abstract ModelIndex IndexAt(Lunatics.Mathematics.Vector2 position);

        /// <summary>
        /// Sets the item specified by the \a index as the currently selected one.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// True if the index has been successfully selected, false otherwise.
        /// </returns>
        public virtual bool SetSelectedIndex(ModelIndex index)
        {
            // simple calls of this method shouldn't do cumulative selection
            return HandleSelection(index, true, false, false);
        }

        public virtual bool SetIndexSelectionState(ModelIndex index, bool selected)
        {
            return HandleSelection(index, selected, d_isMultiSelectEnabled, false);
        }

        public virtual bool IsIndexSelected(ModelIndex index)
        {
            return GetSelectedIndexPosition(index) != -1;
        }

        /// <summary>
        /// Ensures that the item specified by the \a index is visible by setting
        /// the proper the vertical and horizontal scrollbars' position.
        /// </summary>
        /// <param name="index"></param>
        public virtual void EnsureIndexIsVisible(ModelIndex index)
        {
            var vertScroll = GetVertScrollbar();
            var horzScroll = GetHorzScrollbar();
            var renderArea = GetViewRenderer().GetViewRenderArea();
            var viewHeight = renderArea.Height;
            var viewWidth = renderArea.Width;

            var rect = GetIndexRect(index);
            var bottom = rect.Bottom;
            var top = rect.Top;

            // account for current scrollbar value
            var currPos = vertScroll.GetScrollPosition();
            top -= currPos;
            bottom -= currPos;

            // if top is above the view area, or if item is too big to fit
            if ((top < 0.0f) || ((bottom - top) > viewHeight))
            {
                // scroll top of item to top of box.
                vertScroll.SetScrollPosition(currPos + top);
            }
            // if bottom is below the view area
            else if (bottom >= viewHeight)
            {
                // position bottom of item at the bottom of the list
                vertScroll.SetScrollPosition(currPos + bottom - viewHeight);
            }

            var left = rect.Left - currPos;
            var right = left + rect.Width;

            // if left is left of the view area, or if item too big
            if ((left < renderArea.d_min.X) || ((right - left) > viewWidth))
            {
                // scroll item to left
                horzScroll.SetScrollPosition(currPos + left);
            }
            // if right is right of the view area
            else if (right >= renderArea.d_max.X)
            {
                // scroll item to right of list
                horzScroll.SetScrollPosition(currPos + right - viewWidth);
            }
        }

        /// <summary>
        /// Clears all selected indices.
        /// </summary>
        public void ClearSelections()
        {
            d_indexSelectionStates.Clear();
        }

        public void SetSelectionBrushImage(Image image)
        {
            d_selectionBrush = image;
            InvalidateView(false);
        }

        /// <summary>
        /// Sets the image represented by the specified \name as the selection brush.
        /// 
        /// This call is the same as:
        /// \code{.cpp}
        /// Image* img = ImageManager::getSingleton().getImage(name);
        /// view->setSelectionBrushImage(img);
        /// \endcode
        /// </summary>
        /// <param name="name"></param>
        public void SetSelectionBrushImage(string name)
        {
            SetSelectionBrushImage(ImageManager.GetSingleton().Get(name));
        }

        public Image GetSelectionBrushImage()
        {
            return d_selectionBrush;
        }

        /// <summary>
        /// Returns a pointer to the vertical Scrollbar component widget for this view.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the vertical Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetVertScrollbar()
        {
            return (Scrollbar) GetChild(VertScrollbarName);
        }

        public void SetVertScrollbarDisplayMode(ScrollbarDisplayMode mode)
        {
            UpdateScrollbarDisplayMode(ref d_vertScrollbarDisplayMode, mode, EventVertScrollbarDisplayModeChanged);
        }

        public ScrollbarDisplayMode GetVertScrollbarDisplayMode()
        {
            return d_vertScrollbarDisplayMode;
        }

        /// <summary>
        /// Returns a pointer to the horizontal Scrollbar component widget for this view.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnknownObjectException">
        /// Thrown if the horizontal Scrollbar component does not exist.
        /// </exception>
        public Scrollbar GetHorzScrollbar()
        {
            return (Scrollbar)GetChild(HorzScrollbarName);
        }

        public void SetHorzScrollbarDisplayMode(ScrollbarDisplayMode mode)
        {
            UpdateScrollbarDisplayMode(ref d_horzScrollbarDisplayMode, mode, EventHorzScrollbarDisplayModeChanged);
        }

        public ScrollbarDisplayMode GetHorzScrollbarDisplayMode()
        {
            return d_horzScrollbarDisplayMode;
        }

        /// <summary>
        /// Specifies whether the view will vertically auto-resize to match the
        /// content's height or not. This happens when the parent size is changed,
        /// items are added or removed.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetAutoResizeHeightEnabled(bool enabled)
        {
            UpdateAutoResizeFlag(ref d_isAutoResizeHeightEnabled, enabled);
        }

        public bool IsAutoResizeHeightEnabled()
        {
            return d_isAutoResizeHeightEnabled;
        }

        /// <summary>
        /// Specifies whether the view will horizontally auto-resize to match the
        /// content's height or not. This happens when the parent size is changed,
        /// items are added or removed.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetAutoResizeWidthEnabled(bool enabled)
        {
            UpdateAutoResizeFlag(ref d_isAutoResizeWidthEnabled, enabled);
        }

        public bool IsAutoResizeWidthEnabled()
        {
            return d_isAutoResizeWidthEnabled;
        }

        /// <summary>
        /// Specifies whether this view should show tooltips for its items or not.
        /// </summary>
        /// <returns></returns>
        public bool IsItemTooltipsEnabled()
        {
            return d_isItemTooltipsEnabled;
        }

        public void SetItemTooltipsEnabled(bool enabled)
        {
            d_isItemTooltipsEnabled = enabled;
        }

        /// <summary>
        /// Specifies whether one can select multiple items in the view or not.
        /// </summary>
        /// <returns></returns>
        public bool IsMultiSelectEnabled()
        {
            return d_isMultiSelectEnabled;
        }

        public void SetMultiSelectEnabled(bool enabled)
        {
            if (d_isMultiSelectEnabled == enabled)
                return;

            d_isMultiSelectEnabled = enabled;

            // deselect others
            if (!d_isItemTooltipsEnabled && d_indexSelectionStates.Count > 1)
            {
                //SetIndexSelectionState(d_indexSelectionStates.front().d_selectedIndex, true);
                SetIndexSelectionState(d_indexSelectionStates[0].d_selectedIndex, true);
            }

            OnMultiselectModeChanged(new WindowEventArgs(this));
        }

        public ViewSortMode GetSortMode()
        {
            return d_sortMode;
        }

        /// <summary>
        /// Setting a new sorting mode will trigger the instant sorting of this view.
        /// </summary>
        /// <param name="sortMode"></param>
        public void SetSortMode(ViewSortMode sortMode)
        {
            if (d_sortMode == sortMode)
                return;

            d_sortMode = sortMode;

            ResortView();

            OnSortModeChanged(new WindowEventArgs(this));
        }

        /// <summary>
        /// Returns the width of the rendered contents.
        /// </summary>
        /// <returns></returns>
        public float GetRenderedMaxWidth()
        {
            return d_renderedMaxWidth;
        }

        //! Returns the height of the rendered contents.
        public float GetRenderedTotalHeight()
        {
            return d_renderedTotalHeight;
        }

        protected ItemModel d_itemModel;
        protected ColourRect d_textColourRect;
        protected ColourRect _selectionTextColourRect;
        protected ColourRect d_selectionColourRect;
        protected bool d_isDirty;
        protected bool d_needsFullRender;
        protected List<ModelIndexSelectionState> d_indexSelectionStates = new List<ModelIndexSelectionState>();
        protected ModelIndex d_lastSelectedIndex;
        protected Image d_selectionBrush;
        protected ScrollbarDisplayMode d_vertScrollbarDisplayMode;
        protected ScrollbarDisplayMode d_horzScrollbarDisplayMode;
        protected bool d_isItemTooltipsEnabled;
        protected bool d_isMultiSelectEnabled;
        protected ViewSortMode d_sortMode;
        protected bool d_isAutoResizeHeightEnabled;
        protected bool d_isAutoResizeWidthEnabled;

        //TODO: move this into the renderer instead?
        protected float d_renderedMaxWidth;
        protected float d_renderedTotalHeight;

        protected void AddItemViewProperties()
        {
            DefineProperty("TextColour",
                           "",
                           (x, v) => x.SetTextColour(v),
                           null,
                           DefaultTextColour);
            DefineProperty("SelectionTextColour",
                           "",
                           (x, v) => x.SetSelectionTextColour(v),
                           null,
                           DefaultTextColour);
            DefineProperty("SelectionColour",
                           "",
                           (x, v) => x.SetSelectionColour(v),
                           null,
                           DefaultSelectionColour);

            DefineProperty("SelectionBrushImage",
                           "Property to get/set the selection brush image for the item view. Value should be \"set:[imageset name] image:[image name]\".",
                           (x, v) => x.SetSelectionBrushImage(v),
                           x => x.GetSelectionBrushImage(),
                           null);

            DefineProperty("VertScrollbarDisplayMode",
                           "Property to get/set the display mode of the vertical scroll bar of the item view. Value can be \"Shown\", \"Hidden\" or \"WhenNeeded\".",
                           (x, v) => x.SetVertScrollbarDisplayMode(v),
                           x => x.GetVertScrollbarDisplayMode(),
                           ScrollbarDisplayMode.WhenNeeded);

            DefineProperty("HorzScrollbarDisplayMode",
                           "Property to get/set the display mode of the horizontal scroll bar of the item view. Value can be \"Shown\", \"Hidden\" or \"WhenNeeded\".",
                           (x, v) => x.SetHorzScrollbarDisplayMode(v),
                           x => x.GetHorzScrollbarDisplayMode(),
                           ScrollbarDisplayMode.WhenNeeded);

            DefineProperty("ItemTooltips",
                           "Property to access the show item tooltips setting of the item view. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetItemTooltipsEnabled(v),
                           x => x.IsItemTooltipsEnabled(),
                           false);

            DefineProperty("MultiSelect",
                           "Property to get/set the multi-select setting of the item view. Value is either \"True\" or \"False\".",
                           (x, v) => x.SetMultiSelectEnabled(v),
                           x => x.IsMultiSelectEnabled(),
                           false);

            DefineProperty("SortMode",
                           "Property to get/set how the item view is sorting its items. Value is either \"None\", \"Ascending\" or \"Descending\".",
                           (x, v) => x.SetSortMode(v),
                           x => x.GetSortMode(),
                           ViewSortMode.None);

            DefineProperty("AutoSizeHeight",
                           "Property to get/set whether the item view will vertically auto-size itself to fit its content. Value is either \"true\" or \"false\".",
                           (x, v) => x.SetAutoResizeHeightEnabled(v),
                           x => x.IsAutoResizeHeightEnabled(),
                           false);

            DefineProperty("AutoSizeWidth",
                           "Property to get/set whether the item view will vertically auto-size itself to fit its content. Value is either \"true\" or \"false\".",
                           (x, v) => x.SetAutoResizeWidthEnabled(v),
                           x => x.IsAutoResizeWidthEnabled(),
                           false);
        }

        private void DefineProperty<T>(string name, string help, Action<ItemView, T> setter, Func<ItemView, T> getter,
                                       T defaultValue)
        {
            const string propertyOrigin = "ItemView";
            AddProperty(new TplWindowProperty<ItemView, T>(name, help, setter, getter, propertyOrigin, defaultValue));
        }

        protected virtual void UpdateScrollbars()
        {
            var renderArea = GetViewRenderer().GetViewRenderArea();

            UpdateScrollbar(GetVertScrollbar(), renderArea.Height, d_renderedTotalHeight, d_vertScrollbarDisplayMode);
            UpdateScrollbar(GetHorzScrollbar(), renderArea.Width, d_renderedMaxWidth, d_horzScrollbarDisplayMode);
        }

        protected void UpdateScrollbar(Scrollbar scrollbar, float availableArea, float renderedArea, ScrollbarDisplayMode displayMode)
        {
            scrollbar.SetDocumentSize(renderedArea);
            scrollbar.SetPageSize(availableArea);
            scrollbar.SetStepSize(Math.Max(1.0f, renderedArea / 10.0f));
            scrollbar.SetScrollPosition(scrollbar.GetScrollPosition());

            if (displayMode == ScrollbarDisplayMode.Hidden)
            {
                scrollbar.Hide();
                return;
            }

            if (displayMode == ScrollbarDisplayMode.Shown || renderedArea > availableArea)
            {
                scrollbar.Show();
                return;
            }

            scrollbar.Hide();
        }

        protected virtual ItemViewWindowRenderer GetViewRenderer()
        {
            if (d_windowRenderer == null)
                throw new InvalidRequestException("The view should have a window renderer attached!");

            return (ItemViewWindowRenderer) d_windowRenderer;
        }

        protected void UpdateScrollbarDisplayMode(ref ScrollbarDisplayMode targetMode, ScrollbarDisplayMode newMode, string changeEvent)
        {
            if (targetMode == newMode)
                return;

            targetMode = newMode;

            UpdateScrollbars();
            InvalidateView(false);
            FireEvent(changeEvent, new WindowEventArgs(this));
        }

        protected override void InitialiseComponents()
        {
            GetVertScrollbar().ScrollPositionChanged += OnScrollPositionChanged;
            GetHorzScrollbar().ScrollPositionChanged += OnScrollPositionChanged;
            PerformChildWindowLayout();
        }

        protected virtual bool OnChildrenWillBeAdded(EventArgs args)
        {
            return true;
        }

        protected virtual bool OnChildrenAdded(EventArgs args)
        {
            var modelArgs = (ModelEventArgs) args;

            foreach (var state in d_indexSelectionStates)
            {
                if (state.d_childId >= modelArgs.d_startId &&
                    d_itemModel.AreIndicesEqual(state.d_parentIndex, modelArgs.d_parentIndex))
                {
                    state.d_childId += modelArgs.d_count;
                    state.d_selectedIndex = d_itemModel.MakeIndex(state.d_childId, state.d_parentIndex);
                }
            }

            InvalidateView(false);
            OnViewContentsChanged(new WindowEventArgs(this));

            return true;
        }

        protected virtual bool OnChildrenWillBeRemoved(EventArgs args)
        {
            if (d_itemModel == null)
                return false;

            var model_args = (ModelEventArgs) args;
            var itemsToBeRemoved = new List<ModelIndexSelectionState>();

            foreach (var state in d_indexSelectionStates)
            {
                if (state.d_childId >= model_args.d_startId &&
                    state.d_childId <= model_args.d_startId + model_args.d_count)
                {
                    if (d_itemModel.AreIndicesEqual(d_lastSelectedIndex, state.d_selectedIndex))
                        d_lastSelectedIndex = new ModelIndex(0);

                    itemsToBeRemoved.Add(state);
                }
            }

            foreach (var state in itemsToBeRemoved)
                d_indexSelectionStates.Remove(state);

            return true;
        }

        protected virtual bool OnChildrenRemoved(EventArgs args)
        {
            OnSelectionChanged(new ItemViewEventArgs(this));
            OnViewContentsChanged(new WindowEventArgs(this));

            return true;
        }

        protected virtual bool OnChildrenDataWillChange(EventArgs args)
        {
            return true;
        }

        protected virtual bool OnChildrenDataChanged(EventArgs args)
        {
            InvalidateView(false);
            return true;
        }

        protected virtual bool OnScrollPositionChanged(EventArgs args)
        {
            InvalidateView(false);
            return true;
        }

        protected virtual void OnSelectionChanged(ItemViewEventArgs args)
        {
            InvalidateView(false);
            FireEvent(EventSelectionChanged, args);
        }

        protected virtual void OnMultiselectModeChanged(WindowEventArgs args)
        {
            FireEvent(EventMultiselectModeChanged, args, EventNamespace);
        }

        protected virtual void OnSortModeChanged(WindowEventArgs args)
        {
            InvalidateView(false);
            //TODO: make all events be triggered on view's event namespace.
            FireEvent(EventSortModeChanged, args);
        }

        protected virtual void OnViewContentsChanged(WindowEventArgs args)
        {
            FireEvent(EventViewContentsChanged, args, EventNamespace);
        }

        protected internal override void OnScroll(CursorInputEventArgs e)
        {
            HandleOnScroll(GetVertScrollbar(), e.scroll);

            ++e.handled;
            base.OnScroll(e);
        }
        protected internal override void OnCursorPressHold(CursorInputEventArgs e)
        {
            base.OnCursorPressHold(e);
            if (e.Source != CursorInputSource.Left)
                return;

            HandleSelection(e.Position, true, false, false);
            ++e.handled;
        }
        protected internal override void OnCursorMove(CursorInputEventArgs e)
        {
            base.OnCursorMove(e);
            if (d_isItemTooltipsEnabled)
                SetupTooltip(e.Position);

            ++e.handled;
        }
        protected internal override void OnSemanticInputEvent(SemanticEventArgs e)
        {
            if (e.d_semanticValue == SemanticValue.SV_SelectRange ||
                e.d_semanticValue == SemanticValue.SV_SelectCumulative)
            {
                HandleSelection(GetGUIContext().GetCursor().GetPosition(),
                    true, d_isMultiSelectEnabled, e.d_semanticValue == SemanticValue.SV_SelectRange);
            }

            HandleSelectionNavigation(e);

            ++e.handled;
            base.OnSemanticInputEvent(e);
        }

        protected internal override void OnParentSized(ElementEventArgs e)
        {
            base.OnParentSized(e);
            ResizeToContent();
        }

        // TODO: 
        //Event::Connection d_eventChildrenWillBeAddedConnection;
        //Event::Connection d_eventChildrenAddedConnection;
        //Event::Connection d_eventChildrenWillBeRemovedConnection;
        //Event::Connection d_eventChildrenRemovedConnection;
        //Event::Connection d_eventChildrenDataWillChangeConnection;
        //Event::Connection d_eventChildrenDataChangedConnection;
        protected void ConnectToModelEvents()
        {
            if (d_itemModel == null)
                return;

            d_itemModel.ChildrenWillBeAdded += OnChildrenWillBeAdded;
            d_itemModel.ChildrenAdded += OnChildrenAdded;

            d_itemModel.ChildrenWillBeRemoved += OnChildrenWillBeRemoved;
            d_itemModel.ChildrenRemoved += OnChildrenRemoved;

            d_itemModel.ChildrenDataWillChange += OnChildrenDataWillChange;
            d_itemModel.ChildrenDataChanged += OnChildrenDataChanged;
        }

        protected void DisconnectModelEvents()
        {
            if (d_itemModel == null)
                return;

            d_itemModel.ChildrenWillBeAdded -= OnChildrenWillBeAdded;
            d_itemModel.ChildrenAdded -= OnChildrenAdded;

            d_itemModel.ChildrenWillBeRemoved -= OnChildrenWillBeRemoved;
            d_itemModel.ChildrenRemoved -= OnChildrenRemoved;

            d_itemModel.ChildrenDataWillChange -= OnChildrenDataWillChange;
            d_itemModel.ChildrenDataChanged -= OnChildrenDataChanged;
        }

        protected void HandleOnScroll(Scrollbar scrollbar, float scroll)
        {
            if (scrollbar.IsEffectiveVisible() && 
                scrollbar.GetDocumentSize() > scrollbar.GetPageSize())
            {
                scrollbar.SetScrollPosition(scrollbar.GetScrollPosition() + scrollbar.GetStepSize()*-scroll);
            }
        }


        static ModelIndex _lastModelIndex;
        protected void SetupTooltip(Lunatics.Mathematics.Vector2 position)
        {
            if (d_itemModel == null)
                return;
            
            var index = IndexAt(position);
            if (d_itemModel.AreIndicesEqual(index, _lastModelIndex))
                return;

            var tooltip = GetTooltip();
            if (tooltip == null)
                return;

            if (tooltip.GetTargetWindow() != this)
                tooltip.SetTargetWindow(this);
            else
                tooltip.PositionSelf();

            _lastModelIndex = index;

            if (!d_itemModel.IsValidIndex(index))
                SetTooltipText("");
            else
                SetTooltipText(d_itemModel.GetData(index, ItemDataRole.IDR_Tooltip));
        }

        protected int GetSelectedIndexPosition(ModelIndex index)
        {
            if (d_itemModel == null)
                return 0;

            for (var i = 0; i < d_indexSelectionStates.Count; ++i)
            {
                if (d_itemModel.AreIndicesEqual(index, d_indexSelectionStates[i].d_selectedIndex))
                    return i;
            }

            return -1;
        }

        protected virtual bool HandleSelection(Lunatics.Mathematics.Vector2 position, bool shouldSelect, bool isCumulative, bool isRange)
        {
            return HandleSelection(IndexAt(position), shouldSelect, isCumulative, isRange);
        }

        protected virtual bool HandleSelection(ModelIndex index, bool shouldSelect, bool isCumulative, bool isRange)
        {
            if (d_itemModel == null ||
                !d_itemModel.IsValidIndex(index))
                return false;

            var indexPosition = GetSelectedIndexPosition(index);
            if (indexPosition != -1)
            {
                if (!shouldSelect)
                {
                    d_indexSelectionStates.RemoveAt( /*d_indexSelectionStates.begin() + */indexPosition);

                    OnSelectionChanged(new ItemViewEventArgs(this, index));
                    return true;
                }

                // if we select the node again, and we don't cumulate selection, we need
                // to make just that one be selected now
                if (isCumulative)
                    return true;
            }

            if (!isCumulative)
                d_indexSelectionStates.Clear();

            var parentIndex = d_itemModel.GetParentIndex(index);
            var endChildId = d_itemModel.GetChildId(index);
            var startChildId = endChildId;
            if (isRange && isCumulative && d_lastSelectedIndex.d_modelData != null)
            {
                startChildId = d_itemModel.GetChildId(d_lastSelectedIndex);
            }

            for (var id = startChildId; id <= endChildId; ++id)
            {
                var selectionState = new ModelIndexSelectionState
                                     {
                                             d_selectedIndex = d_itemModel.MakeIndex(id, parentIndex)
                                     };

                // ignore already selected indices
                if (GetSelectedIndexPosition(selectionState.d_selectedIndex) != -1)
                    continue;

                selectionState.d_childId = id;
                selectionState.d_parentIndex = parentIndex;

                d_indexSelectionStates.Add(selectionState);
            }

            d_lastSelectedIndex = index;

            OnSelectionChanged(new ItemViewEventArgs(this, index));
            return true;
        }

        protected abstract void ResortView();

        protected void HandleSelectionNavigation(SemanticEventArgs e)
        {
            var parentIndex = d_itemModel.GetRootIndex();
            var lastSelectedChildId = -1;
            if (d_indexSelectionStates.Count!=0)
            {
                var lastSelection = d_indexSelectionStates.Last();
                lastSelectedChildId = lastSelection.d_childId;
                parentIndex = lastSelection.d_parentIndex;
            }

            var childrenCount = d_itemModel.GetChildCount(parentIndex);
            if (childrenCount == 0)
                return;

            var nextSelectedChildId = lastSelectedChildId;
            if (e.d_semanticValue == SemanticValue.SV_GoDown)
            {
                nextSelectedChildId = Math.Min(nextSelectedChildId + 1, childrenCount-1);
            }
            else if (e.d_semanticValue == SemanticValue.SV_GoUp)
            {
                nextSelectedChildId = Math.Max(0, nextSelectedChildId - 1);
            }

            if (nextSelectedChildId == -1 ||
                nextSelectedChildId == lastSelectedChildId)
                return;

            SetSelectedIndex(d_itemModel.MakeIndex(nextSelectedChildId, parentIndex));
        }

        /// <summary>
        /// Returns the Rectf that contains the specified \a index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract Rectf GetIndexRect(ModelIndex index);

        protected void UpdateAutoResizeFlag(ref bool flag, bool enabled)
        {
            if (flag != enabled)
                return;

            flag = enabled;
            ResizeToContent();
        }

        protected void ResizeToContent()
        {
            if (d_initialising ||
                !(d_isAutoResizeWidthEnabled || d_isAutoResizeHeightEnabled))
                return;

            GetViewRenderer().ResizeViewToContent(d_isAutoResizeWidthEnabled, d_isAutoResizeHeightEnabled);
        }
    }
}