namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// ModelIndex is CEGUI's way of representing user data in the views. It uses
    /// a \b void* pointer to store a reference to arbitrary, user-owned data.
    /// 
    /// Even though the naming might suggest a particular order, it doesn't have any.
    /// For comparing two indices ItemModel::compareIndices(const ModelIndex&, const ModelIndex&)
    /// should be used.
    /// 
    /// The index should not be stored because the pointed data might be deleted in
    /// the meantime. The only entity that should interpret the contents of this
    /// class is the ItemModel (and its descendants). All decisions (e.g.: comparisons,
    /// data providing) based on an index need to go through the ItemModel.
    /// 
    /// All operations with the view are done using the ModelIndex. For example, if you want
    /// to select something you need to give it the ModelIndex of that item's. What that means
    /// is that the view doesn't know about the actual model items.
    /// </summary>
    public class ModelIndex
    {
        /// <summary>
        /// Constructs an index with the specified model data.
        /// </summary>
        /// <param name="modelData">
        /// Optional model data associated with this index.
        /// </param>
        public /*explicit*/ ModelIndex(object modelData = null)
        {
            d_modelData = modelData;
        }

        /// <summary>
        /// Pointer to model-specific data.
        /// </summary>
        /// <remarks>
        /// DO NOT USE/INTERPRET in view. This is just a simple way for the model
        /// to be able to manage its data and logic.
        /// </remarks>
        public object d_modelData;

        // TODO: CEGUIEXPORT friend std::ostream& operator<< (std::ostream& os, const ModelIndex& arg);
    }

    /// <summary>
    /// Enumeration that specifies which type of data is required from the ItemModel
    /// in order for the view to render it.
    /// 
    /// Users can use the IDR_User member as a starting base for custom roles. Anything
    /// below is reserved for CEGUI usage.
    /// </summary>
    public enum ItemDataRole
    {
        IDR_Text,

        /// <summary>
        /// The string returned for the icon must be an image name that can
        /// retrieved from ImageManager
        /// </summary>
        IDR_Icon,
        IDR_Tooltip,

        /// <summary>
        /// This marks the beginning of the user-defined item data roles
        /// </summary>
        IDR_User = 0x1000
    }

    // TODO: The count thing is very ugly and counter-intuitive to the whole design.
    // We should either:
    //  * Fire an event per index
    //  * Store the actual indices in a vector.

    /// <summary>
    /// Arguments class for events that happened with regard to the specified ItemModel
    /// on the specified indices.
    /// </summary>
    public class ModelEventArgs : EventArgs
    {
        public ModelEventArgs(ItemModel item_model, ModelIndex parent_index, int start_id, int count = 1)
        {
            d_itemModel = item_model;
            d_parentIndex = parent_index;
            d_startId = start_id;
            d_count = count;
        }

        //! The source ItemModel that triggered the event.
        public ItemModel d_itemModel;

        //! The parent ModelIndex the event happened under.
        public ModelIndex d_parentIndex;

        //! The starting id the event happened on.
        public int d_startId;

        //! The number of items after the start index that have been affected by the event.
        public int d_count;
    };

    /// <summary>
    /// Abstract class defining the interface between the view and the user data.
    /// This is used by views to query data that is to be shown.
    /// 
    /// A view will require a custom implementation of this provider. CEGUI provides
    /// a basic implementation in form of GenericItemModel.
    /// 
    /// This model provides events to notify listeners (usually the view it is
    /// attached to) of different events that will happen or happened. It is the
    /// implementer's job to properly raise the events with the proper arguments so
    /// that the view can correctly process those and render the view. For convenience,
    /// there are the notifyChildren* method which raise the specified events
    /// with the proper event arguments.
    /// </summary>
    public abstract class ItemModel : EventSet
    {
        // TODO: public virtual ~ItemModel();

        /// <summary>
        /// Name of the event triggered \b before children will be added
        /// </summary>
        public const string EventChildrenWillBeAdded = "ChildrenWillBeAdded";
        public event GuiEventHandler<EventArgs> ChildrenWillBeAdded
        {
            add { SubscribeEvent(EventChildrenWillBeAdded, value); }
            remove { UnsubscribeEvent(EventChildrenWillBeAdded, value); }
        }

        /// <summary>
        /// Name of the event triggered \b after new children were added
        /// </summary>
        public const string EventChildrenAdded = "ChildrenAdded";
        public event GuiEventHandler<EventArgs> ChildrenAdded
        {
            add { SubscribeEvent(EventChildrenAdded, value); }
            remove { UnsubscribeEvent(EventChildrenAdded, value); }
        }

        /// <summary>
        /// Name of the event triggered \b before existing children will be removed
        /// </summary>
        public const string EventChildrenWillBeRemoved = "ChildrenWillBeRemoved";
        public event GuiEventHandler<EventArgs> ChildrenWillBeRemoved
        {
            add { SubscribeEvent(EventChildrenWillBeRemoved, value); }
            remove { UnsubscribeEvent(EventChildrenWillBeRemoved, value); }
        }

        /// <summary>
        /// Name of the event triggered \b after existing children were removed
        /// </summary>
        public const string EventChildrenRemoved = "ChildrenRemoved";
        public event GuiEventHandler<EventArgs> ChildrenRemoved
        {
            add { SubscribeEvent(EventChildrenRemoved, value); }
            remove { UnsubscribeEvent(EventChildrenRemoved, value); }
        }

        /// <summary>
        /// Name of the event triggered \b before existing children's data will be changed
        /// </summary>
        public const string EventChildrenDataWillChange = "ChildrenDataWillChange";
        public event GuiEventHandler<EventArgs> ChildrenDataWillChange
        {
            add { SubscribeEvent(EventChildrenDataWillChange, value); }
            remove { UnsubscribeEvent(EventChildrenDataWillChange, value); }
        }

        /// <summary>
        /// Name of the event triggered \b after existing children's data was changed
        /// </summary>
        public const string EventChildrenDataChanged = "ChildrenDataChanged";
        public event GuiEventHandler<EventArgs> ChildrenDataChanged
        {
            add { SubscribeEvent(EventChildrenDataChanged, value); }
            remove { UnsubscribeEvent(EventChildrenDataChanged, value); }
        }

        /// <summary>
        /// Returns true if the specified ModelIndex is valid, false otherwise.
        /// 
        /// Usually, an index is valid if at least it does not contain a reference
        /// to a NULL object. Extra logic can be added to check that the referenced
        /// object is actually part of the model. 
        /// </summary>
        /// <param name="modelIndex"></param>
        /// <returns></returns>
        public abstract bool IsValidIndex(ModelIndex modelIndex);

        /*!
    \brief
        Creates a new ModelIndex that points to the specified child of the
        specified parent index.

        To create an index for a nested item, you need to chain the index creation.
        Given a model for the following tree:
        <pre>
        A
        |
        |--B
        |  |--C
        |
        |--D
        </pre>

        We can compute the indices for the nodes in the following way:
        \code{.cpp}
        ModelIndex a = makeIndex(0, getRootIndex());
        ModelIndex b = makeIndex(0, a);
        ModelIndex c = makeIndex(0, b);
        ModelIndex d = makeIndex(1, a);
        \endcode

    \param child
        The ordinal child id (index), which is a number between 0 and
        getChildrenCount(parentIndex). This will specify which children in the
        parent's index list of children should be referenced.
    */
        public abstract ModelIndex MakeIndex(int child, ModelIndex parentIndex);

        /// <summary>
        /// Compares semantically two indices and returns true if they are equal,
        /// false otherwise.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public virtual bool AreIndicesEqual(ModelIndex index1, ModelIndex index2)
        {
            return CompareIndices(index1, index2) == 0;
        }

        /*!
    \brief
        Compares semantically the contents of the specified two indices and returns:
        - 0 if they are equal.
        - -1 if first index is less than the second index.
        - 1 if the first index is bigger than the second index.

        This function is used by the view when it needs to sort the items.
    */
        public abstract int CompareIndices(ModelIndex index1, ModelIndex index2);

        /*!
    \brief
        Returns the ModelIndex which is parent for the specified ModelIndex.

    \return
        The returned index should be equal to getRootIndex() for direct children
        of the root index. If this is called on the root index, it returns
        an invalid (empty/default) ModelIndex.
    */
        public abstract ModelIndex GetParentIndex(ModelIndex modelIndex);

        /*!
    \brief
        Returns the ordinal id (index) of the child represented by the given
        index, in its parent's list or -1 if no such child exists.
    */
        public abstract int GetChildId(ModelIndex modelIndex);

        /*!
    \brief
        Returns the root ModelIndex of this model.

    \remark
        This index is not used for any rendering, but only to provide the initial
        access to the items. For example, in the case of a list this might contain
        all the items in the list.
    */
        public abstract ModelIndex GetRootIndex();

        /*!
    \brief
        Returns the number of direct children of the specified ModelIndex.
    */
        public abstract int GetChildCount(ModelIndex modelIndex);

        /*!
    \brief
        Returns the string representation of the specified ModelIndex based on
        the specified role.
        It is up to the view to interpret the string based on the role requested.

    \remark
        For example, in the case of an image decoration, the name of the image
        could be returned, and the view would use ImageManager to retrieve the
        specific Image instance by the name and render that.

        An example implementation for the IDR_Icon role could be:
        \code{.cpp}
        Image* img; // get the image from somewhere

        String getData(const ModelIndex& modelIndex, ItemDataRole role)
        {
            if (role == IDR_Icon) return img->getName();
        }
        \endcode
    */
        public abstract string GetData(ModelIndex modelIndex, ItemDataRole role = ItemDataRole.IDR_Text);

        /// <summary>
        /// Notifies any listeners of the EventChildrenWillBeAdded event that new children
        /// will be added to this model.
        /// </summary>
        /// <param name="parentIndex">
        /// The parent index under which children will be added.
        /// </param>
        /// <param name="startId">
        /// The id of the child starting from which children will be added.
        /// </param>
        /// <param name="count">
        /// The number of children that will be added.
        /// </param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildrenWillBeAdded event.
        /// </remarks>
        public virtual void NotifyChildrenWillBeAdded(ModelIndex parentIndex, int startId, int count)
        {   
            FireEvent(EventChildrenWillBeAdded, new ModelEventArgs(this, parentIndex, startId, count));
        }

        /// <summary>
        /// Notifies any listeners of the EventChildrenAdded event that new children
        /// have been added to this model.
        /// </summary>
        /// <param name="parent_index">
        /// The parent index under which children have been added.
        /// </param>
        /// <param name="start_id">
        /// The id of the child starting from which children have been added.
        /// </param>
        /// <param name="count">
        /// The number of children that have been added.
        /// </param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildrenAdded event.
        /// </remarks>
        public virtual void NotifyChildrenAdded(ModelIndex parent_index, int start_id, int count)
        {
            FireEvent(EventChildrenAdded, new ModelEventArgs(this, parent_index, start_id, count));
        }

        /// <summary>
        /// Notifies any listeners of the EventChildrenWillBeRemoved event that existing
        /// children will be removed from this model.
        /// </summary>
        /// <param name="parent_index">
        /// The parent index under which children will be removed.
        /// </param>
        /// <param name="start_id">
        /// The id of the child starting from which children will be removed.
        /// </param>
        /// <param name="count">
        /// The number of children that will be removed.
        /// </param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildrenWillBeRemoved event.
        /// </remarks>
        public virtual void NotifyChildrenWillBeRemoved(ModelIndex parent_index, int start_id, int count)
        {
            FireEvent(EventChildrenWillBeRemoved, new ModelEventArgs(this, parent_index, start_id, count));
        }
        
        /// <summary>
        /// Notifies any listeners of the EventChildrenRemoved event that existing
        /// children have been removed from this model.
        /// </summary>
        /// <param name="parent_index">
        /// The parent index under which children have been removed.
        /// </param>
        /// <param name="start_id">
        /// The id of the child starting from which children have been removed.
        /// </param>
        /// <param name="count">
        /// The number of children that have been removed.
        /// </param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildrenRemoved event.
        /// </remarks>
        public virtual void NotifyChildrenRemoved(ModelIndex parent_index, int start_id, int count)
        {
            FireEvent(EventChildrenRemoved, new ModelEventArgs(this, parent_index, start_id, count));
        }

        /// <summary>
        /// Notifies any listeners of the EventChildDataWillChange event that existing
        /// children's data will change.
        /// </summary>
        /// <param name="parent_index"></param>
        /// <param name="start_id"></param>
        /// <param name="count"></param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildDataWillChange event.
        /// </remarks>
        public virtual void NotifyChildrenDataWillChange(ModelIndex parent_index, int start_id, int count)
        {
            FireEvent(EventChildrenDataWillChange, new ModelEventArgs(this, parent_index, start_id, count));
        }
        
        /// <summary>
        /// Notifies any listeners of the EventChildDataChanged event that existing
        /// children's data has been updated.
        /// </summary>
        /// <param name="parent_index"></param>
        /// <param name="start_id"></param>
        /// <param name="count"></param>
        /// <remarks>
        /// If this method is overridden, it *needs* to call this base method or invoke
        /// manually the EventChildDataChanged event.
        /// </remarks>
        public virtual void NotifyChildrenDataChanged(ModelIndex parent_index, int start_id, int count)
        {
            FireEvent(EventChildrenDataChanged, new ModelEventArgs(this, parent_index, start_id, count));
        }
    }
}