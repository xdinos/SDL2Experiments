using System;
using System.Collections.Generic;
using System.Linq;
using SharpCEGui.Base.Collections;

namespace SharpCEGui.Base
{
    public delegate bool GuiEventHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;

    //public class SubscriberSlot
    //{
    //    public SubscriberSlot() : this(null)
    //    {
            
    //    }

    //    public SubscriberSlot(GuiEventHandler<EventArgs> func)
    //    {
    //        _func = func;
    //    }

    //    public bool Invoke(EventArgs args)
    //    {
    //        return _func(args);
    //    }

    //    public bool Connected()
    //    {
    //        return _func != null;
    //    }

    //    public void CleanUp()
    //    {
    //        _func = null;
    //    }

    //    private GuiEventHandler<EventArgs> _func; 
    //}

    public class BoundSlot
    {
        public BoundSlot(int group, GuiEventHandler<EventArgs> func, Event @event)
        {
            _group = group;
            _func = func;
            _event = @event;
        }

        public bool Connected()
        {
            return _func != null;
        }

        public void Disconnect()
        {
            if (Connected())
                _func = null;
            
            if (_event != null)
            {
                _event.Unsubscribe(this);
                _event = null;
            }
        }

        private int _group;
        internal GuiEventHandler<EventArgs> _func;
        private Event _event;
    }

    public class Event : IDisposable 
    {
        public string Name { get { return _name; }}

        public event GuiEventHandler<EventArgs> Invocation;

        public Event(string name)
        {
            _name = name;
        }

        void IDisposable.Dispose()
        {

        }

        public BoundSlot Subscribe(GuiEventHandler<EventArgs> slot)
        {
            return Subscribe(-1, slot);
        }

        public BoundSlot Subscribe(int group, GuiEventHandler<EventArgs> slot)
        {
            var c = new BoundSlot(group, slot, this);
            _slots.Add(group, c);
            return c;
        }

        public void Invoke(EventArgs args)
        {
            //var handler = Invocation;
            //if (handler != null)
            //{
            //    foreach (var @delegate in handler.GetInvocationList())
            //        if ((bool) @delegate.DynamicInvoke(args))
            //            args.handled++;
            //}

            foreach (var value in _slots.Values.SelectMany(x=>x))
            {
                if (value._func(args))
                    args.handled++;
            }
        }

        internal void Unsubscribe(GuiEventHandler<EventArgs> slot)
        {
            // TODO: optimize this
            var curr =
                _slots.SelectMany(x => x.Value.Select(y => new Tuple<int, BoundSlot>(x.Key, y)))
                      .SingleOrDefault(x => x.Item2._func == slot);
            if (curr != null)
                _slots.Remove(curr.Item1, curr.Item2);
        }

        internal void Unsubscribe(BoundSlot slot)
        {
            // TODO: optimize this
            var curr =
                _slots.SelectMany(x => x.Value.Select(y => new Tuple<int, BoundSlot>(x.Key, y)))
                      .SingleOrDefault(x => x.Item2 == slot);
            if (curr != null)
                _slots.Remove(curr.Item1, curr.Item2);
        }
        
        private readonly string _name;

        private readonly MultiValueDictionary<int, BoundSlot> _slots =
            new MultiValueDictionary<int, BoundSlot>();
    }

    internal interface IEventSet
    {
        /// <summary>
        /// Return whether the EventSet is muted or not.
        /// </summary>
        /// <returns>
        /// - true if the EventSet is muted.  All requests to fire events will be ignored.
        /// - false if the EventSet is not muted.  Requests to fire events are processed as normal.
        /// </returns>
        bool IsMuted();

        /// <summary>
        /// Set the mute state for this EventSet.
        /// </summary>
        /// <param name="setting">
        /// - true if the EventSet is to be muted (no further event firing requests
        /// will be honoured until EventSet is unmuted).
        /// - false if the EventSet is not to be muted and all events should fired
        /// as requested.
        /// </param>
        void SetMutedState(bool setting);
        
        BoundSlot SubscribeEvent(string name, GuiEventHandler<EventArgs> function);
        

        void UnsubscribeEvent(string name, GuiEventHandler<EventArgs> function);

        void FireEvent(string name, EventArgs args, string eventNamespace = "");
    }

    /// <summary>
    /// Class that collects together a set of Event objects.
    /// 
    /// The EventSet is a means for code to attach a handler function to some
    /// named event, and later, for that event to be fired and the subscribed
    /// handler(s) called.
    /// <para>
    /// As of 0.5, the EventSet no longer needs to be filled with available events.
    /// Events are now added to the set as they are first used; that is, the first
    /// time a handler is subscribed to an event for a given EventSet, an Event
    /// object is created and added to the EventSet.
    /// </para>
    /// <para>
    /// Instead of throwing an exception when firing an event that does not actually
    /// exist in the set, we now do nothing (if the Event does not exist, then it
    /// has no handlers subscribed, and therefore doing nothing is the correct
    /// course action).
    /// </para>
    /// </summary>
    public class EventSet : IEventSet
    {
        #region Implementation of IEventSet
        public bool IsMuted()
        {
            return _muted;
        }

        public void SetMutedState(bool value)
        {
            _muted = value;
        }

        /// <summary>
        /// Subscribes a handler to the named Event.  
        /// If the named Event is not yet present in the EventSet, it is created and added.
        /// </summary>
        /// <param name="name">
        /// String object containing the name of the Event to subscribe to.
        /// </param>
        /// <param name="function">
        /// Function or object that is to be subscribed to the Event.
        /// </param>
        /// <returns>
        ///  Connection object that can be used to check the status of the Event 
        /// connection and to disconnect (unsubscribe) from the Event.
        /// </returns>

        public BoundSlot SubscribeEvent(string name, GuiEventHandler<EventArgs> function)
        {
            //return SubscribeEvent(name, new SubscriberSlot(function));
            return GetEventObject(name, true).Subscribe(function);
        }

        public void UnsubscribeEvent(string name, GuiEventHandler<EventArgs> function)
        {
            var @event = GetEventObject(name);
            if (@event != null)
                @event.Unsubscribe(function);
        }

        /// <summary>
        /// Fires the named event passing the given EventArgs object.
        /// </summary>
        /// <param name="name">
        /// String object holding the name of the Event that is to be fired (triggered)
        /// </param>
        /// <param name="args">
        /// The EventArgs (or derived) object that is to be bassed to each subscriber of the Event.
        /// Once all subscribers have been called the 'handled' field of the event is updated appropriately.
        /// </param>
        /// <param name="eventNamespace">
        /// String object describing the global event namespace prefix for this event.
        /// </param>
        public virtual void FireEvent(string name, EventArgs args, string eventNamespace = "")
        {
            GlobalEventSet ges = GlobalEventSet.GetSingleton();
            if (ges!=null)
                ges.FireEvent(name, args, eventNamespace);

            FireEventImpl(name, args);
        }
        
        #endregion

        internal void AddEvent(string name)
        {
            AddEvent(new Event(name));
        }

        internal void AddEvent(Event @event)
        {
            if (IsEventPresent(@event.Name))
                throw new AlreadyExistsException("An event named '" + @event.Name + "' already exists in the EventSet.");

            _events.Add(@event.Name, @event);
        }

        internal bool IsEventPresent(string name)
        {
            return _events.ContainsKey(name);
        }

        /// <summary>
        /// Implementation event firing member
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="eventNamespace"></param>
        protected void FireEventImpl(string name, EventArgs args, string eventNamespace = "")
        {
            var ev = GetEventObject(name);
            if (ev != null && !_muted)
                ev.Invoke(args);
        }

        private Event GetEventObject(string name, bool autoAdd = false)
        {
            if (!_events.ContainsKey(name))
            {
                if (!autoAdd)
                    return null;

                AddEvent(name);
            }

            return _events[name];
        }

        /// <summary>
        /// true if events for this EventSet have been muted.
        /// </summary>
        private bool _muted;

        private readonly Dictionary<string, Event> _events = new Dictionary<string, Event>();

        //     public:
    //    /*!
    //    \brief
    //        Constructor for EventSet objects
    //    */
    //    EventSet();

    //    /*!
    //    \brief
    //        Destructor for EventSet objects
    //    */
    //    virtual ~EventSet(void);

    //    /*!
    //    \brief
    //        Creates a new Event object with the given name and adds it to the
    //        EventSet.

    //    \param name
    //        String object containing the name to give the new Event.  The name must
    //        be unique for the EventSet.

    //    \exception AlreadyExistsException
    //        Thrown if an Event already exists named \a name.
    //    */
    //    void addEvent(const String& name);

    //    /*!
    //    \brief
    //        Adds the given Event object to the EventSet.  Ownership of the object
    //        passes to EventSet and it will be deleted when it is removed from the
    //        EventSet - whether explicitly via removeEvent or when the EventSet
    //        is destroyed.

    //    \param event
    //        Reference to an Event or Event based object that is to be added to the
    //        EventSaet

    //    \exception AlreadyExistsException
    //        Thrown if the EventSet already contains an Event with the same name
    //        as \a event.  Note that \a event will be destroyed under this scenario.
    //    */
    //    void addEvent(Event& event);

    //    /*!
    //    \brief
    //        Removes the Event with the given name.  All connections to the event
    //        are disconnected, and the underlying Event object is destroyed.

    //    \param name
    //        String object containing the name of the Event to remove.  If no such
    //        Event exists, nothing happens.
    //    */
    //    void removeEvent(const String& name);

    //    /*!
    //    \brief
    //        Removes the given event from the EventSet.  All connections to the event
    //        are disconnected, and the event object is destroyed.

    //    \param event
    //        Reference to the Event or Event based object to be removed from the
    //        EventSet.
    //    */
    //    void removeEvent(Event& event);

    //    /*!
    //    \brief
    //        Remove all Event objects from the EventSet.  Add connections will be
    //        disconnected, and all Event objects destroyed.
    //    */
    //    void removeAllEvents(void);

    //    /*!
    //    \brief
    //        Checks to see if an Event with the given name is present in this
    //        EventSet.

    //    \return
    //        - true if an Event named \a name is defined for this EventSet.
    //        - false if no Event named \a name is defined for this EventSet.
    //    */
    //    bool isEventPresent(const String& name);

    //    /*!
    //    \brief
    //        Subscribes a handler to the named Event.  If the named Event is not yet
    //        present in the EventSet, it is created and added.

    //    \param name
    //        String object containing the name of the Event to subscribe to.

    //    \param subscriber
    //        Function or object that is to be subscribed to the Event.

    //    \return
    //        Connection object that can be used to check the status of the Event
    //        connection and to disconnect (unsubscribe) from the Event.
    //    */
    //    virtual Event::Connection subscribeEvent(const String& name,
    //                                             Event::Subscriber subscriber);

    //    /*!
    //    \brief
    //        Subscribes a handler to the specified group of the named Event.  If the
    //        named Event is not yet present in the EventSet, it is created and added.

    //    \param name
    //        String object containing the name of the Event to subscribe to.

    //    \param group
    //        Group which is to be subscribed to.  Subscription groups are called in
    //        ascending order.

    //    \param subscriber
    //        Function or object that is to be subscribed to the Event.

    //    \return
    //        Connection object that can be used to check the status of the Event
    //        connection and to disconnect (unsubscribe) from the Event.
    //    */
    //    virtual Event::Connection subscribeEvent(const String& name,
    //                                             Event::Group group,
    //                                             Event::Subscriber subscriber);

    //    /*!
    //    \copydoc EventSet::subscribeEvent
    
    //    \internal This is there just to make the syntax a tad easier
    //    */
    //    template<typename Arg1, typename Arg2>
    //    inline Event::Connection subscribeEvent(const String& name, Arg1 arg1, Arg2 arg2)
    //    {
    //        return subscribeEvent(name, Event::Subscriber(arg1, arg2));
    //    }
    
    //    /*!
    //    \copydoc EventSet::subscribeEvent
    
    //    \internal This is there just to make the syntax a tad easier
    //    */
    //    template<typename Arg1, typename Arg2>
    //    inline Event::Connection subscribeEvent(const String& name, Event::Group group, Arg1 arg1, Arg2 arg2)
    //    {
    //        return subscribeEvent(name, group, Event::Subscriber(arg1, arg2));
    //    }
    
    //    /*!
    //    \brief
    //        Subscribes the named Event to a scripted funtion

    //    \param name
    //        String object containing the name of the Event to subscribe to.

    //    \param subscriber_name
    //        String object containing the name of the script funtion that is to be
    //        subscribed to the Event.

    //    \return
    //        Connection object that can be used to check the status of the Event
    //        connection and to disconnect (unsubscribe) from the Event.
    //    */
    //    virtual Event::Connection subscribeScriptedEvent(const String& name,
    //                                                     const String& subscriber_name);

    //    /*!
    //    \brief
    //        Subscribes the specified group of the named Event to a scripted funtion.

    //    \param name
    //        String object containing the name of the Event to subscribe to.

    //    \param group
    //        Group which is to be subscribed to.  Subscription groups are called in
    //        ascending order.

    //    \param subscriber_name
    //        String object containing the name of the script funtion that is to be
    //        subscribed to the Event.

    //    \return
    //        Connection object that can be used to check the status of the Event
    //        connection and to disconnect (unsubscribe) from the Event.
    //    */
    //    virtual Event::Connection subscribeScriptedEvent(const String& name,
    //                                                     Event::Group group,
    //                                                     const String& subscriber_name);

    //    /*!
    //    \brief
    //        Fires the named event passing the given EventArgs object.

    //    \param name
    //        String object holding the name of the Event that is to be fired
    //        (triggered)

    //    \param args
    //        The EventArgs (or derived) object that is to be bassed to each
    //        subscriber of the Event.  Once all subscribers
    //        have been called the 'handled' field of the event is updated
    //        appropriately.

    //    \param eventNamespace
    //        String object describing the global event namespace prefix for this
    //        event.
    //    */
    //    virtual void fireEvent(const String& name, EventArgs& args,
    //                           const String& eventNamespace = "");


    //    /*!
    //    \brief
    //        Return whether the EventSet is muted or not.

    //    \return
    //        - true if the EventSet is muted.  All requests to fire events will be
    //          ignored.
    //        - false if the EventSet is not muted.  Requests to fire events are
    //          processed as normal.
    //    */
    //    bool isMuted(void) const;

    //    /*!
    //    \brief
    //        Set the mute state for this EventSet.

    //    \param setting
    //        - true if the EventSet is to be muted (no further event firing requests
    //          will be honoured until EventSet is unmuted).
    //        - false if the EventSet is not to be muted and all events should fired
    //          as requested.
    //    */
    //    void    setMutedState(bool setting);

    //    /*!
    //    \brief
    //        Return a pointer to the Event object with the given name, optionally
    //        adding such an Event object to the EventSet if it does not already
    //        exist.

    //    \param name
    //        String object holding the name of the Event to return.

    //    \param autoAdd
    //        - true if an Event object named \a name should be added to the set
    //          if such an Event does not currently exist.
    //        - false if no object should automatically be added to the set.  In this
    //          case, if the Event does not already exist 0 will be returned.

    //    \return
    //        Pointer to the Event object in this EventSet with the specifed name.
    //        Or 0 if such an Event does not exist and \a autoAdd was false.
    //    */
    //    Event* getEventObject(const String& name, bool autoAdd = false);

    //protected:
    //    //! Implementation event firing member
    //    void fireEvent_impl(const String& name, EventArgs& args);
    //    //! Helper to return the script module pointer or throw.
    //    ScriptModule* getScriptModule() const;

    //    // Do not allow copying, assignment, or any other usage than simple creation.
    //    EventSet(EventSet&) {}
    //    EventSet& operator=(EventSet&)
    //    {
    //        return *this;
    //    }

    //    typedef std::map<String, Event*, StringFastLessCompare
    //        CEGUI_MAP_ALLOC(String, Event*)> EventMap;
    //    EventMap    d_events;

    //    bool    d_muted;    //!< true if events for this EventSet have been muted.

    //public:
    //    /*************************************************************************
    //        Iterator stuff
    //    *************************************************************************/
    //    typedef ConstMapIterator<EventMap> EventIterator;

    //    /*!
    //    \brief
    //        Return a EventSet::EventIterator object to iterate over the events currently
    //        added to the EventSet.
    //    */
    //    EventIterator getEventIterator(void) const;
    }

    /// <summary>
    /// The GlobalEventSet singleton allows you to subscribe to an event for all
    /// instances of a class.  The GlobalEventSet effectively supports "late binding"
    /// to events; which means you can subscribe to some event that does not actually
    /// exist (yet).
    /// </summary>
    public class GlobalEventSet : EventSet
    {
        #region Implementation of Singleton
        private static readonly Lazy<GlobalEventSet> Instance = new Lazy<GlobalEventSet>(() => new GlobalEventSet());

        /// <summary>
        /// Return singleton GlobalEventSet object
        /// </summary>
        /// <returns></returns>
        public static GlobalEventSet GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        /// <summary>
        /// Fires the named event passing the given EventArgs object.
        /// </summary>
        /// <param name="name">
        /// String object holding the name of the Event that is to be fired (triggered)
        /// </param>
        /// <param name="args">
        /// The EventArgs (or derived) object that is to be bassed to each subscriber of the Event.  
        /// Once all subscribers have been called the 'handled' field of the event is updated appropriately.
        /// </param>
        /// <param name="eventNamespace">
        /// String object describing the namespace prefix to use when firing the global event.
        /// </param>
        public override void FireEvent(string name, EventArgs args, string eventNamespace = "")
        {
            FireEventImpl(String.Format("{0}/{1}",eventNamespace,name), args);
        }

        private GlobalEventSet()
        {
            System.GetSingleton().Logger.
                LogEvent(String.Format("CEGUI::GlobalEventSet singleton created. ({0:X8})", GetHashCode()));
        }
    }
}