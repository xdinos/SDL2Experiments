using System;
using System.Collections.Generic;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// This renderer provides extra TreeView-specific rendering information.
    /// </summary>
    public abstract class TreeViewWindowRenderer : ItemViewWindowRenderer
    {
        protected TreeViewWindowRenderer(string type)
                : base(type)
        { }

        /// <summary>
        /// Returns a Size object describing, in unclipped pixels, the size of the
        /// expander that opens (expands) or closes a subtree. This includes
        /// any potential padding around it.
        /// </summary>
        /// <returns></returns>
        public abstract Sizef GetSubtreeExpanderSize();

        /// <summary>
        /// Returns the horizontal (X-axis) indentation size for the subtree expander.
        /// The indent specifies the X-coordinate where the expander is positioned.
        /// </summary>
        /// <param name="depth">
        /// The depth of the item for which to compute the actual indentation.
        /// Depth 0 is the depth for the root's children items.
        /// </param>
        /// <returns></returns>
        public abstract float GetSubtreeExpanderXIndent(int depth);
    }

    /// <summary>
    /// This struct represents the internal rendering state of the TreeView. It should
    /// not be used to manipulate the TreeView or its items unless a TreeView's
    /// method requires it. This struct is exposed only because it's cheaper to use
    /// this for specific operations rather than compute it based of a ModelIndex
    /// each request.
    /// 
    /// Access to the root state is done via TreeView::getRootItemState().
    /// </summary>
    public class TreeViewItemRenderingState
    {
        /// <summary>
        /// These children are rendered via the renderer. If sorting is enabled,
        /// this vector will be sorted.
        /// </summary>
        public List<TreeViewItemRenderingState> d_renderedChildren;

        /// <summary>
        /// This represents the total children count this item has, even if not rendered yet.
        /// This is the case when this node has not been opened/expanded yet.
        /// </summary>
        public int d_totalChildCount;

        public string d_text;

        /// <summary>
        /// The name of the image that represents the icon
        /// </summary>
        public string d_icon;

        public RenderedString d_string;
        public Sizef d_size;
        public bool d_isSelected;
        public ModelIndex d_parentIndex;
        public int d_childId;
        public bool d_subtreeIsExpanded;
        public int d_nestedLevel;

        public TreeView d_attachedTreeView;

        public TreeViewItemRenderingState(TreeView attached_tree_view)
        {
            d_totalChildCount = 0;
            d_size = Sizef.Zero;
            d_isSelected = false;
            d_childId = 0;
            d_subtreeIsExpanded = false;
            d_nestedLevel = 0;
            d_attachedTreeView = attached_tree_view;
        }

        public static bool operator <(TreeViewItemRenderingState lhs, TreeViewItemRenderingState rhs)
        {
            var model = lhs.d_attachedTreeView.GetModel();

            return model.CompareIndices(model.MakeIndex(lhs.d_childId, lhs.d_parentIndex),
                                        model.MakeIndex(rhs.d_childId, rhs.d_parentIndex)) < 0;
        }

        public static bool operator >(TreeViewItemRenderingState lhs, TreeViewItemRenderingState rhs)
        {
            var model = lhs.d_attachedTreeView.GetModel();

            return model.CompareIndices(model.MakeIndex(lhs.d_childId, lhs.d_parentIndex),
                                        model.MakeIndex(rhs.d_childId, rhs.d_parentIndex)) > 0;
        }

        /// <summary>
        /// Holds the unsorted children on which all tree operations are done.
        /// </summary>
        protected internal List<TreeViewItemRenderingState> d_children;

        protected internal void SortChildren()
        {
            d_renderedChildren.Clear();

            foreach (var itor in d_children)
            {
                d_renderedChildren.Add(itor);
                itor.SortChildren();
            }

            if (d_attachedTreeView.GetSortMode() == ViewSortMode.None)
                return;

            // TODO: sort(d_renderedChildren.begin(), d_renderedChildren.end(), d_attachedTreeView->getSortMode() == VSM_Ascending ? &treeViewItemPointerLess : &treeViewItemPointerGreater);
        }
    }

    /// <summary>
    /// View that displays items in a tree fashion. A list-only ItemModel can be
    /// provided as well as the ItemModel of this view.
    /// </summary>
    public class TreeView : ItemView
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public const string WidgetTypeName = "CEGUI/TreeView";

        /// <summary>
        /// Namespace for global events
        /// </summary>
        public new const string EventNamespace = "TreeView";
    
        public const string EventSubtreeExpanded = "SubtreeExpanded";
        public const string EventSubtreeCollapsed = "SubtreeCollapsed";

        public TreeView(string type, string name)
            :base(type, name)
        {
            d_rootItemState = new TreeViewItemRenderingState(this);
            d_subtreeExpanderMargin = DefaultSubtreeExpanderMargin;
            
            AddTreeViewProperties();
        }
        // TODO: virtual ~TreeView();

        public TreeViewItemRenderingState GetRootItemState()
        {
            return d_rootItemState;
        }

        public override void PrepareForRender()
        {
            base.PrepareForRender();
            //TODO: better way of ignoring the null item model? E.g.: warn? Throw an exception?
            if (d_itemModel == null || !IsDirty())
                return;

            if (d_needsFullRender)
            {
                var rootIndex = d_itemModel.GetRootIndex();
                d_renderedMaxWidth = 0;
                d_renderedTotalHeight = 0;

                d_rootItemState = new TreeViewItemRenderingState(this)
                                  {
                                          // root item isn't a proper item so it does not have a nested level.
                                          d_nestedLevel = -1,
                                          d_subtreeIsExpanded = true
                                  };
                

                ComputeRenderedChildrenForItem(d_rootItemState, rootIndex, ref d_renderedMaxWidth, ref d_renderedTotalHeight);
            }
            else
            {
                UpdateRenderingStateForItem(d_rootItemState, ref d_renderedMaxWidth, ref d_renderedTotalHeight);
            }

            UpdateScrollbars();
            SetIsDirty(false);
            d_needsFullRender = false;
        }

        public override ModelIndex IndexAt(Lunatics.Mathematics.Vector2 position)
        {
            return IndexAtWithAction(position, NoopAction);
        }

        public float GetSubtreeExpanderMargin()
        {
            return d_subtreeExpanderMargin;
        }
        
        /// <summary>
        /// Allows setting extra margin around the subtree expander component.
        /// </summary>
        /// <param name="value"></param>
        public void SetSubtreeExpanderMargin(float value)
        {
            d_subtreeExpanderMargin = value;
        }

        public TreeViewItemRenderingState GetTreeViewItemForIndex(ModelIndex index)
        {
            var idsStack = new Stack<int>();
            var rootIndex = d_itemModel.GetRootIndex();
            var tempIndex = index;

            // we create a stack of child ids which will allow us to drill back
            // in the right hierarchy.
            do
            {
                var id = d_itemModel.GetChildId(tempIndex);
                if (id == -1)
                    break;

                idsStack.Push(id);
                tempIndex = d_itemModel.GetParentIndex(tempIndex);
            } while (d_itemModel.IsValidIndex(tempIndex) && !d_itemModel.AreIndicesEqual(tempIndex, rootIndex));

            var item = d_rootItemState;
            while (idsStack.Count != 0)
            {
                var childId = idsStack.Pop();

                if (childId >= item.d_children.Count)
                    return null;

                item = item.d_children[childId];
            }

            return item;
        }

        /// <summary>
        /// Triggers the \b recursive expansion of all subtrees in the TreeView.
        /// This will raise the EventSubtreeExpanded event for all non-expanded
        /// subtrees.
        /// 
        /// This function invokes the expandSubtreeRecursive(TreeViewItemRenderingState)
        /// recursively on each item.
        /// </summary>
        public void ExpandAllSubtrees()
        {
            ExpandSubtreeRecursive(d_rootItemState);
        }

        public void ExpandSubtreeRecursive(TreeViewItemRenderingState item)
        {
            if (!item.d_subtreeIsExpanded)
                ToggleSubtree(item);

            if (item.d_totalChildCount == 0)
                return;

            foreach (var itor in item.d_children)
                ExpandSubtreeRecursive(itor);
        }

        /// <summary>
        /// Toggles the expanded/collapsed state of the specified tree item.
        /// This will raise the EventSubtreeExpanded or EventSubtreeCollapsed
        /// events based on the current item's state.
        /// </summary>
        /// <param name="item"></param>
        public void ToggleSubtree(TreeViewItemRenderingState item)
        {
            if (d_itemModel == null)
                return;

            item.d_subtreeIsExpanded = !item.d_subtreeIsExpanded;

            var args = new ItemViewEventArgs(this, d_itemModel.MakeIndex(item.d_childId, item.d_parentIndex));

            if (item.d_subtreeIsExpanded)
            {
                ComputeRenderedChildrenForItem(item, d_itemModel.MakeIndex(item.d_childId, item.d_parentIndex), ref d_renderedMaxWidth, ref d_renderedTotalHeight);
                OnSubtreeExpanded(args);
            }
            else
            {
                ClearItemRenderedChildren(item, ref d_renderedTotalHeight);
                OnSubtreeCollapsed(args);
            }

            UpdateScrollbars();
            // we need just a simple invalidation. No need to redo the render state
            // as we modified it ourself directly.
            Invalidate(false);
        }

        protected new virtual TreeViewWindowRenderer GetViewRenderer()
        {
            return (TreeViewWindowRenderer) base.GetViewRenderer();
        }

        protected override bool HandleSelection(Lunatics.Mathematics.Vector2 position, bool shouldSelect, bool isCumulative, bool isRange)
        {
            return HandleSelection(IndexAtWithAction(position, HandleSelectionAction), shouldSelect, isCumulative, isRange);
        }

        protected override bool HandleSelection(ModelIndex index, bool shouldSelect, bool isCumulative, bool isRange)
        {
            return base.HandleSelection(index, shouldSelect, isCumulative, isRange);
        }

        protected override bool OnChildrenRemoved(EventArgs args)
        {
            base.OnChildrenRemoved(args);

            var margs = (ModelEventArgs)args;
            var item = GetTreeViewItemForIndex(margs.d_parentIndex);

            if (item == null)
                return true;

            item.d_totalChildCount -= margs.d_count;

            if (!item.d_subtreeIsExpanded)
                return true;

            var begin = margs.d_startId;
            var end = begin + margs.d_count;

            // update existing child ids
            for (var i=begin;i<item.d_children.Count;i++)
            {
                var itor = item.d_children[i];
                itor.d_childId -= margs.d_count;

                if (i < end)
                    d_renderedTotalHeight -= itor.d_size.Height;
            }

            item.d_children.RemoveRange(begin, margs.d_count);

            item.SortChildren();
            InvalidateView(false);
            return true;
        }

        protected override bool OnChildrenAdded(EventArgs args)
        {
            base.OnChildrenAdded(args);

            var margs = (ModelEventArgs) args;
            var item = GetTreeViewItemForIndex(margs.d_parentIndex);

            if (item == null)
                return true;

            item.d_totalChildCount += margs.d_count;

            if (!item.d_subtreeIsExpanded)
                return true;

            var states = new List<TreeViewItemRenderingState>();
            for (var id = margs.d_startId; id < margs.d_startId + margs.d_count; ++id)
            {
                states.Add(ComputeRenderingStateForIndex(margs.d_parentIndex, id, item.d_nestedLevel + 1, ref d_renderedMaxWidth, ref d_renderedTotalHeight));
            }

            // update existing child ids
            for (var i =  margs.d_startId; i<item.d_children.Count;i++)
                item.d_children[i].d_childId += margs.d_count;

            item.d_children.InsertRange(margs.d_startId, states);

            item.SortChildren();
            InvalidateView(false);
            return true;
        }

        protected virtual void OnSubtreeExpanded(ItemViewEventArgs args)
        {
            FireEvent(EventSubtreeExpanded, args, EventNamespace);
        }

        protected virtual void OnSubtreeCollapsed(ItemViewEventArgs args)
        {
            FireEvent(EventSubtreeCollapsed, args, EventNamespace);
        }

        //typedef void (TreeView::*TreeViewItemAction)(TreeViewItemRenderingState& item, bool toggles_expander);
        //typedef std::vector<TreeViewItemRenderingState> ItemStateVector;

        private TreeViewItemRenderingState d_rootItemState;

        private TreeViewItemRenderingState ComputeRenderingStateForIndex(ModelIndex parentIndex, int childId, int nestedLevel, ref float renderedMaxWidth, ref float renderedTotalHeight)
        {
            var index = d_itemModel.MakeIndex(childId, parentIndex);
            var state= new TreeViewItemRenderingState(this)
                       {
                               d_nestedLevel = nestedLevel,
                               d_parentIndex = parentIndex,
                               d_childId = childId
                       };

            FillRenderingState(state, index, ref renderedMaxWidth, ref renderedTotalHeight);

            ComputeRenderedChildrenForItem(state, index, ref renderedMaxWidth, ref renderedTotalHeight);

            return state;
        }

        private float d_subtreeExpanderMargin;

        private void AddTreeViewProperties()
        {
            DefineProperty("SubtreeExpanderMargin",
                           "Property to access the margin around the subtree expander. Value is a float. Default is 5.0f",
                           (x, v) => x.SetSubtreeExpanderMargin(v),
                           x => x.GetSubtreeExpanderMargin(),
                           DefaultSubtreeExpanderMargin);
        }

        private void DefineProperty<T>(string name, string help, Action<TreeView, T> setter, Func<TreeView, T> getter,
                                       T defaultValue)
        {
            const string propertyOrigin = "TreeView";
            AddProperty(new TplWindowProperty<TreeView, T>(name, help, setter, getter, propertyOrigin, defaultValue));
        }

        private void ComputeRenderedChildrenForItem(TreeViewItemRenderingState item, ModelIndex index,
                                                    ref float renderedMaxWidth, ref float renderedTotalHeight)
        {
            var childCount = d_itemModel.GetChildCount(index);
            item.d_totalChildCount = childCount;

            if (!item.d_subtreeIsExpanded)
                return;

            for (var child = 0; child < childCount; ++child)
            {
                item.d_children.Add(ComputeRenderingStateForIndex(index, child, item.d_nestedLevel + 1, ref renderedMaxWidth, ref renderedTotalHeight));
            }

            item.SortChildren();
        }

        private void UpdateRenderingStateForItem(TreeViewItemRenderingState item, ref float renderedMaxWidth, ref float renderedTotalHeight)
        {
            // subtract the previous height
            renderedTotalHeight -= item.d_size.Height;

            FillRenderingState(item, d_itemModel.MakeIndex(item.d_childId, item.d_parentIndex), ref renderedMaxWidth, ref renderedTotalHeight);

            foreach (var itor in item.d_children)
            {
                itor.d_nestedLevel = item.d_nestedLevel + 1;
                UpdateRenderingStateForItem(itor, ref renderedMaxWidth, ref renderedTotalHeight);
            }
        }

        private void FillRenderingState(TreeViewItemRenderingState item, ModelIndex index, ref float renderedMaxWidth, ref float renderedTotalHeight)
        {
            var text = d_itemModel.GetData(index);
            var renderedString = GetRenderedStringParser().Parse(text, GetFont(), d_textColourRect);
            item.d_string = renderedString;
            item.d_text = text;
            item.d_icon = d_itemModel.GetData(index, ItemDataRole.IDR_Icon);

            item.d_size = new Sizef(renderedString.GetHorizontalExtent(this),
                                    renderedString.GetVerticalExtent(this));

            float indent = GetViewRenderer().GetSubtreeExpanderXIndent(item.d_nestedLevel) + GetViewRenderer().GetSubtreeExpanderSize().Width;
            renderedMaxWidth = Math.Max(renderedMaxWidth, item.d_size.Width + indent);
            renderedTotalHeight += item.d_size.Height;

            item.d_isSelected = IsIndexSelected(index);
        }

        private ModelIndex IndexAtWithAction(Lunatics.Mathematics.Vector2 position, Action<TreeViewItemRenderingState, bool> action)
        {
            if (d_itemModel == null)
                return new ModelIndex();

            //TODO: add prepareForLayout() as a cheaper operation alternative?
            PrepareForRender();

            var windowPosition = CoordConverter.ScreenToWindow(this, position);
            var renderArea = GetViewRenderer().GetViewRenderArea();
            if (!renderArea.IsPointInRect(windowPosition))
                return new ModelIndex();

            var curHeight = renderArea.d_min.Y - GetVertScrollbar().GetScrollPosition();
            var handled = false;
            return IndexAtRecursive(d_rootItemState, ref curHeight, windowPosition, ref handled, action);
        }

        private ModelIndex IndexAtRecursive(TreeViewItemRenderingState item, ref float curHeight,
                                            Lunatics.Mathematics.Vector2 windowPosition, ref bool handled,
                                            Action<TreeViewItemRenderingState, bool> action)
        {
            var nextHeight = curHeight + item.d_size.Height;

            if (windowPosition.Y >= curHeight &&
                windowPosition.Y <= nextHeight)
            {
                handled = true;

                var expanderWidth = GetViewRenderer().GetSubtreeExpanderSize().Width;
                var baseX = GetViewRenderer().GetSubtreeExpanderXIndent(item.d_nestedLevel);
                baseX -= GetHorzScrollbar().GetScrollPosition();
                if (windowPosition.X >= baseX &&
                    windowPosition.X <= baseX + expanderWidth)
                {
                    action(item, true);
                    return new ModelIndex();
                }

                action(item, false);
                return new ModelIndex(d_itemModel.MakeIndex(item.d_childId, item.d_parentIndex));
            }

            curHeight = nextHeight;

            for (var i = 0; i < item.d_renderedChildren.Count; ++i)
            {
                var index = IndexAtRecursive(item.d_renderedChildren[i], ref curHeight, windowPosition, ref handled, action);
                if (handled)
                    return index;
            }

            return new ModelIndex();
        }

        private void ClearItemRenderedChildren(TreeViewItemRenderingState item, ref float renderedTotalHeight)
        {
            foreach (var itor in item.d_children)
            {
                ClearItemRenderedChildren(itor, ref renderedTotalHeight);

                // ??? d_renderedTotalHeight -= item.d_size.d_height;
                renderedTotalHeight -= item.d_size.Height;
            }

            item.d_children.Clear();
            item.SortChildren();
        }

        private void HandleSelectionAction(TreeViewItemRenderingState item, bool togglesExpander)
        {
            if (!togglesExpander)
                return;

            ToggleSubtree(item);
        }

        private void NoopAction(TreeViewItemRenderingState item, bool togglesExpander)
        {
            
        }

        protected override void ResortView()
        {
            d_rootItemState.SortChildren();
            InvalidateView(false);
        }

        protected override Rectf GetIndexRect(ModelIndex index)
        {
            //TODO: implement for tree view. What do we do for indices in closed subtrees?
            throw new InvalidRequestException("Not implemented for tree view yet.");
        }

        private const float DefaultSubtreeExpanderMargin = 5.0f;
    }
}