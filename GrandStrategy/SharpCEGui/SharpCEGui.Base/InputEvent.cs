namespace SharpCEGui.Base
{
    /// <summary>
    /// The default input events used inside CEGUI
    /// </summary>
    public enum InputEventType
    {
        IET_TextInputEventType = 0x0001, //!< Text was inputted.
        IET_SemanticInputEventType = 0x0002, //!< An event with a certain semantic

        IET_UserDefinedInputEventType = 0x5000, //!< This marks the beginning of user-defined events.
    };

    /// <summary>
    /// The base class for all input events.
    /// </summary>
    public class InputEvent
    {
        public InputEvent(InputEventType event_type)
        {
            d_eventType = event_type;
        }

        // TODO: virtual ~InputEvent() {}

        public InputEventType d_eventType; //!< The type of the input event
    }

    /// <summary>
    /// Represents the input of a character
    /// </summary>
    public class TextInputEvent : InputEvent
    {
        public TextInputEvent() : base(InputEventType.IET_TextInputEventType)
        {
        }

        public char d_character; //!< The character inputted
    }

    /// <summary>
    /// Event arguments used by semantic input event handlers
    /// </summary>
    public class SemanticEventArgs : WindowEventArgs
    {
        public SemanticEventArgs(Window wnd) 
            : base(wnd)
        {

        }

        /// <summary>
        /// The type of the semantic value
        /// </summary>
        public SemanticValue d_semanticValue;

        /// <summary>
        /// The payload of the event
        /// </summary>
        public SemanticPayload d_payload;
    }

    ///*!
    //\brief
    //    Slot template class that creates a functor that calls back via a class
    //    member function and send a casted input event subclass as the parameter.

    //\tparam T
    //    The type of the class the contains the handler

    //\tparam TInput
    //    A subclass of InputEvent or InputEvent itself to cast the input event to
    //    before calling the functor
    //*/
    //template<typename T, typename TInput>
    //class InputEventHandlerSlot : public SlotFunctorBase<InputEvent>
    //{
    //public:
    //    //! Member function slot type.
    //    typedef bool(T::*MemberFunctionType)(const TInput&);

    //    InputEventHandlerSlot(MemberFunctionType func, T* obj) :
    //        d_function(func),
    //        d_object(obj)
    //    {}

    //    virtual bool operator()(const InputEvent& arg)
    //    {
    //        return (d_object->*d_function)(static_cast<const TInput&>(arg));
    //    }

    //private:
    //    MemberFunctionType d_function;
    //    T* d_object;
    //};
}