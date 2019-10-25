namespace SharpCEGui.Base
{
    /// <summary>
    /// Interface to be implemented by classes that handle input events
    /// </summary>
    public interface IInputEventReceiver
    {
        /// <summary>
        /// Injects a new input event.
        /// </summary>
        /// <param name="event">
        /// The input event to be injected.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the input receiver.
        /// - false if the input was not processed by the input receiver.
        /// </returns>
        bool InjectInputEvent(InputEvent @event);
    }
}