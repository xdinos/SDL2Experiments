namespace SharpCEGui.Base
{
    /// <summary>
    /// Represents the source of an event that uses the pointer.
    /// </summary>
    public enum CursorInputSource
    {
        /// <summary>
        /// No specific pointer source
        /// </summary>
        None,

        /// <summary>
        /// Left pointer source
        /// </summary>
        Left,

        /// <summary>
        /// Right pointer source
        /// </summary>
        Right,

        /// <summary>
        /// Middle pointer source
        /// </summary>
        Middle
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PointerSourceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static CursorInputSource ConvertToPointerSource(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return CursorInputSource.Left;
                case MouseButton.RightButton:
                    return CursorInputSource.Right;
                case MouseButton.MiddleButton:
                    return CursorInputSource.Middle;
            }

            return CursorInputSource.None;
        }
    }

    /// <summary>
    /// Holds the state of the pointers (hold or not)
    /// </summary>
    public  class CursorsState
    {
        /// <summary>
        /// 
        /// </summary>
        public CursorsState()
        {
            _state = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Get()
        {
            return _state;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsHeld(CursorInputSource source)
        {
            return (_state & (1 << (int)source)) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void PointerHold(CursorInputSource source)
        {
            _state |= (1 << (int)source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void PointerDeactivated(CursorInputSource source)
        {
            _state &= ~(1 << (int)source);
        }

        private int _state;
    }

    /// <summary>
    /// Represents the value of a semantic input event, generated from a specific
    /// operation or sequence of operations.
    /// </summary>
    public enum SemanticValue
    {
        SV_NoValue = 0x0000,
        SV_CursorActivate,
        SV_PointerDeactivate,
        SV_CursorPressHold,
        SV_CursorMove,
        SV_PointerLeave,
        SV_SelectRange,
        SV_SelectCumulative,
        SV_SelectWord,
        SV_SelectAll,
        SV_SelectPreviousCharacter,
        SV_SelectNextCharacter,
        SV_SelectPreviousWord,
        SV_SelectNextWord,
        SV_SelectToStartOfLine,
        SV_SelectToEndOfLine,
        SV_GoToPreviousCharacter,
        SV_GoToNextCharacter,
        SV_GoToPreviousWord,
        SV_GoToNextWord,
        SV_GoToStartOfLine,
        SV_GoToEndOfLine,
        SV_GoToStartOfDocument,
        SV_GoToEndOfDocument,
        SV_GoToNextPage,
        SV_GoToPreviousPage,
        SV_DeleteNextCharacter,
        SV_DeletePreviousCharacter,
        SV_Confirm,
        SV_Back,
        SV_Undo,
        SV_Redo,
        SV_Cut,
        SV_Copy,
        SV_Paste,
        SV_HorizontalScroll,
        SV_VerticalScroll,
        SV_SelectToStartOfDocument,
        SV_SelectToEndOfDocument,
        SV_SelectToNextPage,
        SV_SelectToPreviousPage,
        SV_SelectNextPage,
        SV_SelectPreviousPage,
        SV_GoUp,
        SV_GoDown,
        SV_SelectUp,
        SV_SelectDown,
        SV_NavigateToNext,
        SV_NavigateToPrevious,

        SV_UserDefinedSemanticValue = 0x5000, //!< This marks the beginning of user-defined semantic values.
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SemanticValueHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsSelectionSemanticValue(int value)
        {
            return (value >= (int)SemanticValue.SV_SelectRange && value <= (int)SemanticValue.SV_SelectToEndOfLine) ||
                   (value >= (int)SemanticValue.SV_SelectToStartOfDocument && value <= (int)SemanticValue.SV_SelectToPreviousPage);
        }
    }

    /// <summary>
    /// The type of the payload used in the semantic input events
    /// </summary>
    public /*union*/ class SemanticPayload
    {
        public float[] array = new float[2];
        public float single;
        public CursorInputSource source;
    }

    /// <summary>
    /// Represents a semantic input event (e.g.: delete a previous character, confirm)
    /// </summary>
    public class SemanticInputEvent : InputEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SemanticInputEvent(int value)
                : base(InputEventType.IET_SemanticInputEventType)
        {
            d_value = value;
            d_payload = new SemanticPayload();
        }

        /// <summary>
        /// The semantic value of this event
        /// </summary>
        public int d_value;

        /// <summary>
        /// Extra data associated to this event
        /// </summary>
        public SemanticPayload d_payload;
    }
}