using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base
{
    /// <summary>
    /// EventArgs class passed to subscribers for (most) InputAggregator events.
    /// </summary>
    public class InputAggregatorEventArgs : EventArgs
    {
        public InputAggregatorEventArgs(InputAggregator aggregator)
        {
            d_aggregator = aggregator;
        }

        //! pointer to the InputAggregator that triggered the event.
        public InputAggregator d_aggregator;
    }

    /// <summary>
    /// Aggregates the input from multiple input devices and processes it to generate
    /// input events which are then fed to the (optional) \ref InputEventReceiver.
    /// </summary>
    public class InputAggregator : /*EventSet,*/ IInjectedInputReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        public const float DefaultMouseButtonClickTimeout = 0.0f;
        /// <summary>
        /// 
        /// </summary>
        public const float DefaultMouseButtonMultiClickTimeout = 0.3333f;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Sizef DefaultMouseButtonMultiClickTolerance = new Sizef(0.01f, 0.01f);

        #region Events

        /// <summary>
        /// Name of Event fired when the mouse click timeout is changed.
        /// Handlers are passed a const reference to a GUIContextEventArgs struct.
        /// </summary>
        public const string EventMouseButtonClickTimeoutChanged = "MouseButtonClickTimeoutChanged";
        public event GuiEventHandler<EventArgs> MouseButtonClickTimeoutChanged;
        //{
        //    add { SubscribeEvent(EventMouseButtonClickTimeoutChanged, value); }
        //    remove { UnsubscribeEvent(EventMouseButtonClickTimeoutChanged, value); }
        //}

        /// <summary>
        /// Name of Event fired when the mouse multi-click timeout is changed.
        /// Handlers are passed a const reference to a GUIContextEventArgs struct.
        /// </summary>
        public const string EventMouseButtonMultiClickTimeoutChanged = "MouseButtonMultiClickTimeoutChanged";
        public event GuiEventHandler<EventArgs> MouseButtonMultiClickTimeoutChanged;
        //{
        //    add { SubscribeEvent(EventMouseButtonMultiClickTimeoutChanged, value); }
        //    remove { UnsubscribeEvent(EventMouseButtonMultiClickTimeoutChanged, value); }
        //}

        /// <summary>
        /// Name of Event fired when the mouse multi-click movement tolerance area
        /// size is changed.
        /// Handlers are passed a const reference to a GUIContextEventArgs struct.
        /// </summary>
        public const string EventMouseButtonMultiClickToleranceChanged = "MouseButtonMultiClickToleranceChanged";
        public event GuiEventHandler<EventArgs> MouseButtonMultiClickToleranceChanged;
        //{
        //    add { SubscribeEvent(EventMouseButtonMultiClickToleranceChanged, value); }
        //    remove { UnsubscribeEvent(EventMouseButtonMultiClickToleranceChanged, value); }
        //}

        /// <summary>
        /// Name of Event fired when the mouse movement scaling factor is changed.
        /// Handlers are passed a const reference to a GUIContextEventArgs struct.
        /// </summary>
        public const string EventMouseMoveScalingFactorChanged = "MouseMoveScalingFactorChanged";
        public event GuiEventHandler<EventArgs> MouseMoveScalingFactorChanged;
        //{
        //    add { SubscribeEvent(EventMouseMoveScalingFactorChanged, value); }
        //    remove { UnsubscribeEvent(EventMouseMoveScalingFactorChanged, value); }
        //}

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputReceiver"></param>
        public InputAggregator(IInputEventReceiver inputReceiver)
        {
            System.GetSingleton().DisplaySizeChanged += OnDisplaySizeChanged;
            d_inputReceiver = inputReceiver;
            d_mouseButtonClickTimeout = DefaultMouseButtonClickTimeout;
            d_mouseButtonMultiClickTimeout = DefaultMouseButtonMultiClickTimeout;
            d_mouseButtonMultiClickTolerance = DefaultMouseButtonMultiClickTolerance;
            d_generateMouseClickEvents = true;
            d_mouseClickTrackers.AddRange(Enumerable.Range(0, (int) MouseButton.MouseButtonCount)
                                                    .Select(x => new MouseClickTracker()));
            d_handleInKeyUp = true;
            d_mouseMovementScalingFactor = 1.0f;
            d_pointerPosition = Lunatics.Mathematics.Vector2.Zero;
           
            // Initialize the array
            for (var i = 0; i < 0xff; i++)
                d_keyValuesMappings[i] = (int)SemanticValue.SV_NoValue;
            
            // initial absolute tolerance
            RecomputeMultiClickAbsoluteTolerance();
        }

        // TODO:
        //virtulal ~InputAggregator()
        //{
        //    d_displaySizeChangedConnection->disconnect();
        //    delete[] d_mouseClickTrackers;
        //}

        /// <summary>
        /// Initialises this InputAggregator with some default simple-key mappings
        /// </summary>
        /// <param name="handleOnKeyup"></param>
        public virtual void Initialise(bool handleOnKeyup = true)
        {
            d_handleInKeyUp = handleOnKeyup;

            d_keyValuesMappings[(int)Key.Scan.Backspace] = (int)SemanticValue.SV_DeletePreviousCharacter;
            d_keyValuesMappings[(int)Key.Scan.Delete] = (int)SemanticValue.SV_DeleteNextCharacter;

            d_keyValuesMappings[(int)Key.Scan.NumpadEnter] = (int)SemanticValue.SV_Confirm;
            d_keyValuesMappings[(int)Key.Scan.Return] = (int)SemanticValue.SV_Confirm;

            d_keyValuesMappings[(int)Key.Scan.Tab] = (int)SemanticValue.SV_NavigateToNext;

            d_keyValuesMappings[(int)Key.Scan.ArrowLeft] = (int)SemanticValue.SV_GoToPreviousCharacter;
            d_keyValuesMappings[(int)Key.Scan.ArrowRight] = (int)SemanticValue.SV_GoToNextCharacter;
            d_keyValuesMappings[(int)Key.Scan.ArrowDown] = (int)SemanticValue.SV_GoDown;
            d_keyValuesMappings[(int)Key.Scan.ArrowUp] = (int)SemanticValue.SV_GoUp;

            d_keyValuesMappings[(int)Key.Scan.End] = (int)SemanticValue.SV_GoToEndOfLine;
            d_keyValuesMappings[(int)Key.Scan.Home] = (int)SemanticValue.SV_GoToStartOfLine;
            d_keyValuesMappings[(int)Key.Scan.PageDown] = (int)SemanticValue.SV_GoToNextPage;
            d_keyValuesMappings[(int)Key.Scan.PageUp] = (int)SemanticValue.SV_GoToPreviousPage;
        }

        /// <summary>
        /// Set whether automatic mouse button click and multi-click (i.e.
        /// double-click and treble-click) event generation will occur.
        /// </summary>
        /// <param name="enable">
        /// - true to have mouse button click and multi-click events automatically
        /// generated by the system from the basic button up and down event
        /// injections.
        /// - false if no automatic generation of events should occur.  In this
        /// instance the user may wish to use the additional event injectors to
        /// manually inform the system of such events.
        /// </param>
        public void SetMouseClickEventGenerationEnabled(bool enable)
        {
            d_generateMouseClickEvents = enable;
        }

        /// <summary>
        /// Return whether automatic mouse button click and multi-click (i.e.
        /// double-click and treble-click) event generation is enabled.
        /// </summary>
        /// <returns>
        /// - true if mouse button click and multi-click events will be
        /// automatically generated by the system from the basic button up and down
        /// event injections.
        /// - false if no automatic generation of events will occur.  In this
        /// instance the user may wish to use the additional event injectors to
        /// manually inform the system of such events.
        /// </returns>
        public bool IsMouseClickEventGenerationEnabled()
        {
            return d_generateMouseClickEvents;
        }

        public void SetMouseButtonClickTimeout(float seconds)
        {
            d_mouseButtonClickTimeout = seconds;
            OnMouseButtonClickTimeoutChanged(new InputAggregatorEventArgs(this));
        }

        public float GetMouseButtonClickTimeout()
        {
            return d_mouseButtonClickTimeout;
        }

        public void SetMouseButtonMultiClickTimeout(float seconds)
        {
            d_mouseButtonMultiClickTimeout = seconds;
            OnMouseButtonMultiClickTimeoutChanged(new InputAggregatorEventArgs(this));
        }

        public float GetMouseButtonMultiClickTimeout()
        {
            return d_mouseButtonMultiClickTimeout;
        }

        /// <summary>
        /// Sets the mouse multi-click tolerance size
        /// </summary>
        /// <param name="sz">
        /// The size of the tolerance in percent of the display's size
        /// </param>
        public void SetMouseButtonMultiClickTolerance(Sizef sz)
        {
            d_mouseButtonMultiClickTolerance = sz;
            OnMouseButtonMultiClickToleranceChanged(new InputAggregatorEventArgs(this));
        }

        /// <summary>
        /// Returns the mouse multi-click tolerance size
        /// </summary>
        /// <returns>
        /// A size structure with the zone's width and height in percent of the
        /// display's size
        /// </returns>
        public Sizef GetMouseButtonMultiClickTolerance()
        {
            return d_mouseButtonMultiClickTolerance;
        }

        public void SetMouseMoveScalingFactor(float factor)
        {
            d_mouseMovementScalingFactor = factor;
            OnMouseMoveScalingFactorChanged(new InputAggregatorEventArgs(this));
        }

        public float GetMouseMoveScalingFactor()
        {
            return d_mouseMovementScalingFactor;
        }

        /// <summary>
        /// Returns a semantic action matching the scan_code
        /// </summary>
        /// <param name="scanCode"></param>
        /// <param name="shiftDown"></param>
        /// <param name="altDown"></param>
        /// <param name="ctrlDown"></param>
        /// <returns></returns>
        public int GetSemanticAction(Key.Scan scanCode, bool shiftDown, bool altDown, bool ctrlDown)
        {
            int value = d_keyValuesMappings[(int)scanCode];

            // handle combined keys
            if (ctrlDown && shiftDown)
            {
                if (scanCode == Key.Scan.ArrowLeft)
                    value = (int)SemanticValue.SV_SelectPreviousWord;
                else if (scanCode == Key.Scan.ArrowRight)
                    value = (int)SemanticValue.SV_SelectNextWord;
                else if (scanCode == Key.Scan.End)
                    value = (int)SemanticValue.SV_SelectToEndOfDocument;
                else if (scanCode == Key.Scan.Home)
                    value = (int)SemanticValue.SV_SelectToStartOfDocument;
                else if (scanCode == Key.Scan.Z)
                    value = (int)SemanticValue.SV_Redo;
            }
            else if (ctrlDown)
            {
                if (scanCode == Key.Scan.ArrowLeft)
                    value = (int)SemanticValue.SV_GoToPreviousWord;
                else if (scanCode == Key.Scan.ArrowRight)
                    value = (int)SemanticValue.SV_GoToNextWord;
                else if (scanCode == Key.Scan.End)
                    value = (int)SemanticValue.SV_GoToEndOfDocument;
                else if (scanCode == Key.Scan.Home)
                    value = (int)SemanticValue.SV_GoToStartOfDocument;
                else if (scanCode == Key.Scan.A)
                    value = (int)SemanticValue.SV_SelectAll;
                else if (scanCode == Key.Scan.C)
                    value = (int)SemanticValue.SV_Copy;
                else if (scanCode == Key.Scan.V)
                    value = (int)SemanticValue.SV_Paste;
                else if (scanCode == Key.Scan.X)
                    value = (int)SemanticValue.SV_Cut;
                else if (scanCode == Key.Scan.Tab)
                    value = (int)SemanticValue.SV_NavigateToPrevious;
                else if (scanCode == Key.Scan.Z)
                    value = (int)SemanticValue.SV_Undo;
                else if (scanCode == Key.Scan.Y)
                    value = (int)SemanticValue.SV_Redo;
            }
            else if (shiftDown)
            {
                if (scanCode == Key.Scan.ArrowLeft)
                    value = (int)SemanticValue.SV_SelectPreviousCharacter;
                else if (scanCode == Key.Scan.ArrowRight)
                    value = (int)SemanticValue.SV_SelectNextCharacter;
                else if (scanCode == Key.Scan.ArrowUp)
                    value = (int)SemanticValue.SV_SelectUp;
                else if (scanCode == Key.Scan.ArrowDown)
                    value = (int)SemanticValue.SV_SelectDown;
                else if (scanCode == Key.Scan.End)
                    value = (int)SemanticValue.SV_SelectToEndOfLine;
                else if (scanCode == Key.Scan.Home)
                    value = (int)SemanticValue.SV_SelectToStartOfLine;
                else if (scanCode == Key.Scan.PageUp)
                    value = (int)SemanticValue.SV_SelectPreviousPage;
                else if (scanCode == Key.Scan.PageDown)
                    value = (int)SemanticValue.SV_SelectNextPage;
            }
            if (altDown)
            {
                if (scanCode == Key.Scan.Backspace)
                    value = (int)SemanticValue.SV_Undo;
            }

            return value;
        }
        
        /// <summary>
        /// Gets semantic action for scan_code and sends the event
        /// </summary>
        /// <param name="scanCode"></param>
        /// <param name="shiftDown"></param>
        /// <param name="altDown"></param>
        /// <param name="ctrlDown"></param>
        /// <returns>
        /// True if the semantic action was handled
        /// </returns>
        public bool HandleScanCode(Key.Scan scanCode, bool shiftDown, bool altDown, bool ctrlDown)
        {
            var value = GetSemanticAction(scanCode, shiftDown, altDown, ctrlDown);

            if (value != (int)SemanticValue.SV_NoValue)
                return d_inputReceiver.InjectInputEvent(new SemanticInputEvent(value));

            return false;
        }

        /// <summary>
        /// Sets the status of modifier keys to the specified values.
        /// 
        /// Call this before injectKeyDown if InputAggregator is set to handle
        /// actions on keydown.
        /// </summary>
        /// <param name="shiftDown"></param>
        /// <param name="altDown"></param>
        /// <param name="ctrlDown"></param>
        public void SetModifierKeys(bool shiftDown, bool altDown, bool ctrlDown)
        {
            d_keysPressed[(int)Key.Scan.LeftShift] = shiftDown;
            d_keysPressed[(int)Key.Scan.RightShift] = shiftDown;

            d_keysPressed[(int)Key.Scan.LeftAlt] = altDown;
            d_keysPressed[(int)Key.Scan.RightAlt] = altDown;

            d_keysPressed[(int)Key.Scan.LeftControl] = ctrlDown;
            d_keysPressed[(int)Key.Scan.RightControl] = ctrlDown;
        }

        #region Implementation of IInjectedInputReceiver

        public virtual bool InjectMouseMove(float deltaX, float deltaY)
        {
            return InjectMousePosition(deltaX + d_pointerPosition.X*d_mouseMovementScalingFactor,
                                       deltaY + d_pointerPosition.Y*d_mouseMovementScalingFactor);
        }

        public virtual bool InjectMouseLeaves()
        {
            if (d_inputReceiver == null)
                return false;

            var semanticEvent = new SemanticInputEvent((int) SemanticValue.SV_PointerLeave);

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMouseButtonDown(MouseButton button)
        {
            if (d_inputReceiver == null)
                return false;

            //
            // Handling for multi-click generation
            //
            var tkr = d_mouseClickTrackers[(int)button];

            tkr.d_click_count++;

            // TODO: re-add the check for different windows?
            // if multi-click requirements are not met
            if (((d_mouseButtonMultiClickTimeout > 0) && (tkr.d_timer.Elapsed() > d_mouseButtonMultiClickTimeout)) ||
                (!tkr.d_click_area.IsPointInRect(d_pointerPosition)) ||
                (tkr.d_click_count > 3))
            {
                // reset to single down event.
                tkr.d_click_count = 1;

                // build new allowable area for multi-clicks
                tkr.d_click_area.Position = d_pointerPosition;
                tkr.d_click_area.Size = d_mouseButtonMultiClickAbsoluteTolerance;
                tkr.d_click_area.Offset(new Lunatics.Mathematics.Vector2(
                    -(d_mouseButtonMultiClickAbsoluteTolerance.Width / 2),
                    -(d_mouseButtonMultiClickAbsoluteTolerance.Height / 2)));
            }

            // reset timer for this tracker.
            tkr.d_timer.Restart();

            if (d_generateMouseClickEvents)
            {
                switch (tkr.d_click_count)
                {
                    case 2:
                        return InjectMouseButtonDoubleClick(button);

                    case 3:
                        return InjectMouseButtonTripleClick(button);
                }
            }

            var value = SemanticValue.SV_CursorPressHold;
            if (IsControlPressed())
                value = SemanticValue.SV_SelectCumulative;
            else if (IsShiftPressed())
                value = SemanticValue.SV_SelectRange;

            var semanticEvent = new SemanticInputEvent((int)value);
            semanticEvent.d_payload.source = PointerSourceHelper.ConvertToPointerSource(button);

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMouseButtonUp(MouseButton button)
        {
            if (d_inputReceiver == null) 
                return false;

            SemanticInputEvent semanticEvent = new SemanticInputEvent((int) SemanticValue.SV_CursorActivate)
                                               {
                                                       d_payload = { source = PointerSourceHelper.ConvertToPointerSource(button) }
                                               };

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectKeyDown(Key.Scan scanCode)
        {
            if (d_inputReceiver == null)
                return false;

            d_keysPressed[(int)scanCode] = true;
            
            if (d_handleInKeyUp)
                return true;

            return HandleScanCode(scanCode, IsShiftPressed(), IsAltPressed(), IsControlPressed());
        }

        public virtual bool InjectKeyUp(Key.Scan scanCode)
        {
            if (d_inputReceiver == null)
                return false;

            d_keysPressed[(int)scanCode] = false;
            
            if (!d_handleInKeyUp)
                return true;

            return HandleScanCode(scanCode, IsShiftPressed(), IsAltPressed(), IsControlPressed());
        }

        public virtual bool InjectChar(char codePoint)
        {
            if (d_inputReceiver == null)
                return false;

            return d_inputReceiver.InjectInputEvent(new TextInputEvent {d_character = codePoint});
        }

        public virtual bool InjectMouseWheelChange(float delta)
        {
            if (d_inputReceiver == null)
                return false;

            var semanticEvent = new SemanticInputEvent((int) SemanticValue.SV_VerticalScroll) { d_payload = { single = delta } };

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMousePosition(float xPos, float yPos)
        {
            if (d_inputReceiver == null)
                return false;

            d_pointerPosition = new Lunatics.Mathematics.Vector2(xPos, yPos);

            var semanticEvent = new SemanticInputEvent((int) SemanticValue.SV_CursorMove) { d_payload = { array = new[] { xPos, yPos } } };

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMouseButtonClick(MouseButton button)
        {
            if (d_inputReceiver == null)
                return false;

            var semanticEvent = new SemanticInputEvent((int)SemanticValue.SV_CursorActivate);

            if (IsControlPressed())
                semanticEvent.d_value = (int)SemanticValue.SV_SelectCumulative;

            semanticEvent.d_payload.source = PointerSourceHelper.ConvertToPointerSource(button);

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMouseButtonDoubleClick(MouseButton button)
        {
            if (d_inputReceiver == null)
                return false;

            var semanticEvent=new SemanticInputEvent((int)SemanticValue.SV_SelectWord);
            semanticEvent.d_payload.source = PointerSourceHelper.ConvertToPointerSource(button);

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectMouseButtonTripleClick(MouseButton button)
        {
            if (d_inputReceiver == null)
                return false;

            var semanticEvent=new SemanticInputEvent((int)SemanticValue.SV_SelectAll);
            semanticEvent.d_payload.source = PointerSourceHelper.ConvertToPointerSource(button);

            return d_inputReceiver.InjectInputEvent(semanticEvent);
        }

        public virtual bool InjectCopyRequest()
        {
            if (d_inputReceiver == null)
                return false;

            return d_inputReceiver.InjectInputEvent(new SemanticInputEvent((int) SemanticValue.SV_Copy));
        }

        public virtual bool InjectCutRequest()
        {
            if (d_inputReceiver == null)
                return false;

            return d_inputReceiver.InjectInputEvent(new SemanticInputEvent((int)SemanticValue.SV_Cut));
        }

        public virtual bool InjectPasteRequest()
        {
            if (d_inputReceiver == null)
                return false;

            return d_inputReceiver.InjectInputEvent(new SemanticInputEvent((int)SemanticValue.SV_Paste));
        }

        #endregion

        protected virtual void OnMouseButtonClickTimeoutChanged(InputAggregatorEventArgs args)
        {
            // TODO: fireEvent(EventMouseButtonClickTimeoutChanged, args);
            var handler = MouseButtonClickTimeoutChanged;
            if (handler != null)
                handler(args);
        }

        protected virtual void OnMouseButtonMultiClickTimeoutChanged(InputAggregatorEventArgs args)
        {
            // TODO: fireEvent(EventMouseButtonMultiClickTimeoutChanged, args);
            var handler = MouseButtonMultiClickTimeoutChanged;
            if (handler != null)
                handler(args);
        }

        protected virtual void OnMouseButtonMultiClickToleranceChanged(InputAggregatorEventArgs args)
        {
            // TODO: fireEvent(EventMouseButtonMultiClickToleranceChanged, args);
            var handler = MouseButtonMultiClickToleranceChanged;
            if (handler != null)
                handler(args);
        }

        protected virtual void OnMouseMoveScalingFactorChanged(InputAggregatorEventArgs args)
        {
            // TODO: fireEvent(EventMouseMoveScalingFactorChanged, args);
            var handler = MouseMoveScalingFactorChanged;
            if (handler != null)
                handler(args);
        }

        protected virtual bool IsControlPressed()
        {
            return d_keysPressed[(int)Key.Scan.LeftControl] || d_keysPressed[(int)Key.Scan.RightControl];
        }

        protected virtual bool IsAltPressed()
        {
            return d_keysPressed[(int)Key.Scan.LeftAlt] || d_keysPressed[(int)Key.Scan.RightAlt];
        }

        protected virtual bool IsShiftPressed()
        {
            return d_keysPressed[(int)Key.Scan.LeftShift] || d_keysPressed[(int)Key.Scan.RightShift];
        }
        
        protected void RecomputeMultiClickAbsoluteTolerance()
        {
            var displaySize = System.GetSingleton().GetRenderer().GetDisplaySize();
            d_mouseButtonMultiClickAbsoluteTolerance = new Sizef(
                    d_mouseButtonMultiClickTolerance.Width*displaySize.Width,
                    d_mouseButtonMultiClickTolerance.Height*displaySize.Height);
        }

        protected virtual void /*bool*/ OnDisplaySizeChanged(object sender, DisplayEventArgs args)
        {
            RecomputeMultiClickAbsoluteTolerance();
            // TODO: return true;
        }

        // TODO: protected Event::Connection d_displaySizeChangedConnection;

        protected IInputEventReceiver d_inputReceiver;

        //! Timeout used to when detecting a single-click.
        protected float d_mouseButtonClickTimeout;

        //! Timeout used when detecting multi-click events.
        protected float d_mouseButtonMultiClickTimeout;

        //! Movement tolerance (percent) used when detecting multi-click events.
        protected Sizef d_mouseButtonMultiClickTolerance;

        //! Movement tolerance (absolute) used when detecting multi-click events.
        protected Sizef d_mouseButtonMultiClickAbsoluteTolerance;

        //! should mouse click/multi-click events be automatically generated.
        protected bool d_generateMouseClickEvents;

        protected List<MouseClickTracker> d_mouseClickTrackers = new List<MouseClickTracker>((int)MouseButton.MouseButtonCount);

        //! When set to true will handle semantic actions on key up
        protected bool d_handleInKeyUp;

        //! Scaling factor applied to injected pointer move deltas.
        protected float d_mouseMovementScalingFactor;

        protected Lunatics.Mathematics.Vector2 d_pointerPosition;

        //!< Mapping from a key to its semantic
        protected int[] d_keyValuesMappings = new int[KeyValueCount];
        protected bool[] d_keysPressed = new bool[KeyValueCount];

        private const int KeyValueCount = 0xFF;
    }
}