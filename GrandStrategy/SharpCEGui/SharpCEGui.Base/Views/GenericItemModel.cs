using System;
using System.Collections.Generic;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// An abstract implementation of the ItemModel that works with
    /// a tree structure using items of class that inherits from GenericItem.
    /// 
    /// Users of this class can either create a template instantiation of this
    /// implementation, or inherit it and augment with custom operations or
    /// overwrite certain methods to provide more functionality (e.g.: getData).
    /// 
    /// One such example is the following, where we instantiate a model for our
    /// own MyItem type:
    /// \code{.cpp}
    /// class MyItem : public GenericItem
    /// {
    /// };
    /// 
    /// typedef GenericItemModel<MyItem> MyItemModel;
    /// \endcode
    /// </summary>
    /// <remarks>
    /// Items added to the model are taken over and managed by the model, which
    /// means they are deleted by it. delete is used for that, so
    /// new should be used for already created objects that are added.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the item this model operates on. The type \b needs to inherit
    /// directly or indirectly from GenericItem.
    /// </typeparam>
    public class GenericItemModel<T> : ItemModel where T : GenericItem, new()
    {
        /// <summary>
        /// Creates a new model instance using the specified \a root item, which
        /// must not be NULL - otherwise an InvalidRequestException is thrown.
        /// 
        /// The \a root will be taken ownership of, and cleaned when the model is
        /// destructed (using delete).
        /// 
        /// An example of initializing such GenericItemModel implementation is the
        /// following:
        /// \code{.cpp}
        /// class MyItemModel : public GenericItemModel<MyItem>
        /// {
        ///     MyItemModel() : GenericItemModel<MyItem>(new MyItem)
        ///     {
        ///     }
        /// };
        /// \endcode
        /// 
        /// Alternatively, if you want to create a root upon declaring the model, you
        /// can let the caller provide it, in which case our \b MyItemModel constructor
        /// will receive the root TGenericItem instance.
        /// </summary>
        /// <param name="root"></param>
        public /*explicit*/ GenericItemModel(T root)
        {
            if (root == null) 
                throw new InvalidOperationException("Root cannot be null");

            d_root = root;
        }

        // TODO: 
        //virtual ~GenericItemModel()
        //{
        //    clear(false);
        //    delete d_root;
        //}

        /// <summary>
        /// Adds a new item in the model with the specified text, taking ownership
        /// of it.
        /// 
        /// If this method is used, the TGenericItem type should have a string
        /// constructor in order to successfully compile. This method is equivalent
        /// with:
        /// \code{.cpp}
        /// GenericItem* item = new GenericItem("MyText");
        /// model->addItem(item);
        /// \endcode
        /// </summary>
        /// <param name="text"></param>
        public virtual void AddItem(string text)
        {
            //AddItem(new T(text));
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the item as child of the root and takes ownership of it.
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(GenericItem item)
        {
            if (item == null)
                throw new InvalidRequestException("Cannot add a NULL item to the model!");

            AddItemAtPosition(item, d_root.GetChildren().Count== 0 ? 0 : d_root.GetChildren().Count);
        }
        
        /// <summary>
        /// Adds the item as child of the root at the specified position.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        public virtual void AddItemAtPosition(GenericItem item, int pos)
        {
            AddItemAtPosition(item, GetRootIndex(), pos);
        }

        /// <summary>
        /// Adds the item as child of the specified parent, at the specified position.
        /// 
        /// After the addition, the \a parent's node will contain \a item as child
        /// at the \a position.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="parentIndex"></param>
        /// <param name="position"></param>
        public virtual void AddItemAtPosition(GenericItem newItem, ModelIndex parentIndex, int position)
        {
            NotifyChildrenWillBeAdded(parentIndex, position, 1);

            var parent = (GenericItem)parentIndex.d_modelData;
            if (position > parent.GetChildren().Count)
                throw new InvalidRequestException("The specified position is out of range.");

            newItem.SetParent(parent);
            parent.GetChildren().Insert(position, newItem);

            NotifyChildrenAdded(parentIndex, position, 1);
        }

        /// <summary>
        /// Inserts the specified \a item before the specified \a position item.
        /// If \a position is NULL the new item will be added at the beginning of
        /// the root's children list.
        /// 
        /// This method allows for arbitrary level (nestedness/depth) insertions.
        /// Another function which can be used instead of this, is
        /// addItemAtPosition(GenericItem*, const ModelIndex&, size_t).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="position"></param>
        public virtual void InsertItem(GenericItem item, GenericItem position)
        {
            var childId = position == null ? -1 : GetChildId(position);

            var parentIndex = GetRootIndex();
            if (position != null)
                parentIndex = GetParentIndex(GetIndexForItem(position));

            AddItemAtPosition(item, parentIndex, childId <= 0 ? 0 : childId);
        }

        public virtual void RemoveItem(GenericItem item)
        {
            var parentItem = item.GetParent();

            var childId = parentItem.GetChildren().IndexOf(item);
            
            NotifyChildrenWillBeRemoved(new ModelIndex(parentItem), childId, 1);

            DeleteChildren(item, true);
            // TODO: delete* itor;
            parentItem.GetChildren().Remove(item);

            NotifyChildrenRemoved(new ModelIndex(parentItem), childId, 1);

            //std::vector<GenericItem*>::iterator itor = std::find(
            //    parent_item->getChildren().begin(), parent_item->getChildren().end(),
            //    item);

            //if (itor != parent_item->getChildren().end())
            //{
            //    size_t child_id = std::distance(parent_item->getChildren().begin(), itor);

            //    notifyChildrenWillBeRemoved(ModelIndex(parent_item), child_id, 1);

            //    deleteChildren(*itor, true);
            //    delete *itor;
            //    parent_item->getChildren().erase(itor);

            //    notifyChildrenRemoved(ModelIndex(parent_item), child_id, 1);
            //}
        }
        
        public virtual void RemoveItem(ModelIndex index)
        {
            RemoveItem(GetItemForIndex(index));
        }

        /// <summary>
        /// Clears the items of this ItemModel, deleting them, using delete
        /// on each, optionally notifying any listeners of the removal of the items.
        /// </summary>
        /// <param name="notify">
        /// If true, it will raise the EventChildrenWillBeRemoved and
        /// EventChildrenRemoved events for each deleted item.
        /// </param>
        public virtual void Clear(bool notify = true)
        {
            DeleteChildren(d_root, notify);
        }

        /// <summary>
        /// Gets the underlying TGenericItem represented by the given \a index or
        /// NULL if the index does not represent a valid item.
        /// 
        /// This method is the inverse of to getIndexForItem(const GenericItem*).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T GetItemForIndex(ModelIndex index)
        {
            return (T) index.d_modelData;
        }

        /// <summary>
        /// Creates a ModelIndex that represents the specified index.
        /// 
        /// This method is the inverse of getItemForIndex(const ModelIndex&).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual ModelIndex GetIndexForItem(GenericItem item)
        {
            // TODO: this is annoying. We should be able to just hand out the ModelIndex
            // Right now we can't, since we're in a const method, operating on a const item
            // In hindsight, the index we hand out will be modifiable so maybe we need
            // to change the const-ness of the item parameter.
            
            var parent = item.GetParent();
            var child_id = GetChildId(item);
            return new ModelIndex(child_id != -1 ? parent.GetChildren()[child_id] : null);
        }
        
        /// <summary>
        /// Gets the ordinal id (index) of the specified item in parent's children list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int GetChildId(GenericItem item)
        {
            if (item == null || item.GetParent() == null)
                return -1;

            var parentItem = item.GetParent();

            return parentItem.GetChildren().IndexOf(item);
            //std::vector<GenericItem*>::iterator itor = std::find(
            //    parent_item->getChildren().begin(), parent_item->getChildren().end(), item);

            //if (itor == parent_item->getChildren().end())
            //    return -1;

            //return std::distance(parent_item->getChildren().begin(), itor);
        }

        /// <summary>
        /// This represents the conceptual root of this model. This root will be
        /// ignored in normal operations. It's merely used in order to provide
        /// an access point to the items tree.
        /// </summary>
        /// <returns></returns>
        public T GetRoot() { return (T)d_root; }

        public override bool IsValidIndex(ModelIndex modelIndex)
        {
            return modelIndex.d_modelData != null;
        }
        public override ModelIndex MakeIndex(int child, ModelIndex parentIndex)
        {
            if (parentIndex.d_modelData == null)
                return new ModelIndex();

            var item = (GenericItem)parentIndex.d_modelData;
            return MakeValidIndex(child, item.GetChildren());
        }
    
        public override bool AreIndicesEqual(ModelIndex index1, ModelIndex index2)
        {
            return CompareIndices(index1, index2) == 0;
        }

        public override int CompareIndices(ModelIndex index1, ModelIndex index2)
        {
            if (!IsValidIndex(index1) || !IsValidIndex(index2) ||
                index1.d_modelData == index2.d_modelData)
                return 0;

            /// TODO: ...
            //if (GetItemForIndex(index1) < GetItemForIndex(index2))
            //    return -1;

            return GetItemForIndex(index1) == GetItemForIndex(index2) ? 0 : 1;
        }

        public override ModelIndex GetParentIndex(ModelIndex modelIndex)
        {
            if (modelIndex.d_modelData == d_root)
                return new ModelIndex();

            var item = (GenericItem)modelIndex.d_modelData;
            if (item.GetParent() == null)
                return GetRootIndex();

            return new ModelIndex(item.GetParent());
        }

        public override int GetChildId(ModelIndex modelIndex)
        {
            return GetChildId(GetItemForIndex(modelIndex));
        }
    
        public override ModelIndex GetRootIndex()
        {
            return new ModelIndex(d_root);
        }

        public override int GetChildCount(ModelIndex modelIndex)
        {
            if (modelIndex.d_modelData == null)
                return d_root.GetChildren().Count;

            return ((GenericItem) modelIndex.d_modelData).GetChildren().Count;
        }
    
        public override string GetData(ModelIndex modelIndex, ItemDataRole role = ItemDataRole.IDR_Text)
        {
            if (!IsValidIndex(modelIndex))
                return "";

            var item = (GenericItem)modelIndex.d_modelData;
            if (role == ItemDataRole.IDR_Text) return item.GetText();
            if (role == ItemDataRole.IDR_Icon) return item.GetIcon();
            if (role == ItemDataRole.IDR_Tooltip) return "Tooltip for " + item.GetText();

            return "";
        }

        /// <summary>
        /// Deletes all children of the specified item, optionally invoking the
        /// EventChildren(WillBe)Removed event
        /// </summary>
        /// <param name="item"></param>
        /// <param name="notify"></param>
        protected void DeleteChildren(GenericItem item, bool notify)
        {
            if (item == null)
                throw new InvalidRequestException("Cannot delete children of a NULL item!");

            var itemsCount = item.GetChildren().Count;
            //std::vector<GenericItem*>::iterator itor = item->getChildren().begin();

            if (notify)
                NotifyChildrenWillBeRemoved(new ModelIndex(item), 0, itemsCount);

            while (item.GetChildren().Count!=0)
            {
                var itor = item.GetChildren()[0];
                DeleteChildren(itor, notify);
                // TODO: delete *itor;
                item.GetChildren().Remove(itor);
            }

            //while (itor != item->getChildren().end())
            //{
            //    DeleteChildren(*itor, notify);
            //    // TODO: delete *itor;
            //    itor = item->getChildren().erase(itor);
            //}

            if (notify)
                NotifyChildrenRemoved(new ModelIndex(item), 0, itemsCount);
        }

        //! Makes a valid index if \a id is withing \a vector's bounds.
        protected ModelIndex MakeValidIndex(int id, List<GenericItem> vector)
        {
            if (id >= 0 && id < vector.Count)
                return new ModelIndex(vector[id]);
            
            return new ModelIndex();
        }

        protected GenericItem d_root;

    }
}