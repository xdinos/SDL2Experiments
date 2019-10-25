#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace SharpCEGui.Base
{
    /// <summary>
    /// Input injection interface to be inplemented by classes that take raw inputs
    /// </summary>
    public interface IInjectedInputReceiver
    {
        /// <summary>
        /// Function that injects a mouse movement event into the receiver.
        /// </summary>
        /// <param name="deltaX">amount the mouse moved on the x axis.</param>
        /// <param name="deltaY">amount the mouse moved on the y axis.</param>
        /// <returns>
        /// - true if the input was processed by the input receiver.
        /// - false if the input was not processed by the input receiver.
        /// </returns>
        bool InjectMouseMove(float deltaX, float deltaY);

        /// <summary>
        /// Function that notifies that the mouse has left the host area that the
        /// receiver receives input for.
        /// </summary>
        /// <returns>
        /// - true if the event was handled.
        /// - false if the event was not handled.
        /// </returns>
        bool InjectMouseLeaves();

        /// <summary>
        /// Function that injects a mouse button down event into the receiver.
        /// </summary>
        /// <param name="button">
        /// One of the MouseButton values indicating which button was pressed.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectMouseButtonDown(MouseButton button);

        /// <summary>
        /// Function that injects a mouse button up event into the receiver.
        /// </summary>
        /// <param name="button">
        /// One of the MouseButton values indicating which button was released.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectMouseButtonUp(MouseButton button);

        /// <summary>
        /// Function that injects a key down event into the receiver.
        /// </summary>
        /// <param name="scanCode">
        /// value indicating which key was pressed.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectKeyDown(Key.Scan scanCode);

        /// <summary>
        /// Function that injects a key up event into the receiver.
        /// </summary>
        /// <param name="scanCode">
        /// Key::Scan value indicating which key was released.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectKeyUp(Key.Scan scanCode);

        /// <summary>
        /// Function that injects a typed character event into the receiver.
        /// </summary>
        /// <param name="codePoint">
        /// Unicode or ASCII (depends on used String class) code point of the character that was typed.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectChar(char codePoint);

        /// <summary>
        /// Function that injects a mouse-wheel / scroll-wheel event into the receiver.
        /// </summary>
        /// <param name="delta">
        /// float value representing the amount the wheel moved.
        /// </param>
        /// <returns>
        /// - true if the input was processed by the receiver.
        /// - false if the input was not processed by the receiver.
        /// </returns>
        bool InjectMouseWheelChange(float delta);

        /// <summary>
        /// Function that injects a new position for the mouse cursor.
        /// </summary>
        /// <param name="xPos">
        /// New absolute pixel position of the mouse cursor on the x axis.
        /// </param>
        /// <param name="yPos">
        /// New absolute pixel position of the mouse cursoe in the y axis.
        /// </param>
        /// <returns>
        /// - true if the generated mouse move event was handled.
        /// - false if the generated mouse move event was not handled.
        /// </returns>
        bool InjectMousePosition(float xPos, float yPos);

        ///// <summary>
        ///// Function to inject time pulses into the receiver.
        ///// </summary>
        ///// <param name="timeElapsed">
        ///// float value indicating the amount of time passed, in seconds, since the last time this method was called.
        ///// </param>
        ///// <returns>
        ///// Currently, this method always returns true.
        ///// </returns>
        //bool InjectTimePulse(float timeElapsed);

        /// <summary>
        /// Function to directly inject a mouse button click event.
        /// <para>
        /// Here 'click' means a mouse button down event followed by a mouse
        /// button up event.
        /// </para>
        /// </summary>
        /// <param name="button">
        /// One of the MouseButton enumerated values.
        /// </param>
        /// <returns>
        /// - true if some window or handler reported that it handled the event.
        /// - false if nobody handled the event.
        /// </returns>
        /// <remarks>
        /// Under normal, default settings, this event is automatically generated by
        /// the system from the regular up and down events you inject.  You may use
        /// this function directly, though you'll probably want to disable the
        /// automatic click event generation first by using the
        /// setMouseClickEventGenerationEnabled function - this setting controls the
        /// auto-generation of events and also determines the default 'handled'
        /// state of the injected click events according to the rules used for
        /// mouse up/down events.
        /// </remarks>
        bool InjectMouseButtonClick(MouseButton button);

        /// <summary>
        /// Function to directly inject a mouse button double-click event.
        /// <para>
        /// Here 'double-click' means a single mouse button had the sequence down,
        /// up, down within a predefined period of time.
        /// </para>
        /// </summary>
        /// <param name="button">
        /// One of the MouseButton enumerated values.
        /// </param>
        /// <returns>
        /// - true if some window or handler reported that it handled the event.
        /// - false if nobody handled the event.
        /// </returns>
        /// <remarks>
        /// Under normal, default settings, this event is automatically generated by
        /// the system from the regular up and down events you inject.  You may use
        /// this function directly, though you'll probably want to disable the
        /// automatic click event generation first by using the
        /// setMouseClickEventGenerationEnabled function - this setting controls the
        /// auto-generation of events and also determines the default 'handled'
        /// state of the injected click events according to the rules used for
        /// mouse up/down events.
        /// </remarks>
        bool InjectMouseButtonDoubleClick(MouseButton button);

        /// <summary>
        /// Function to directly inject a mouse button triple-click event.
        /// <para>
        /// Here 'triple-click' means a single mouse button had the sequence down,
        /// up, down, up, down within a predefined period of time.
        /// </para>
        /// </summary>
        /// <param name="button">
        /// One of the MouseButton enumerated values.
        /// </param>
        /// <returns>
        /// - true if some window or handler reported that it handled the event.
        /// - false if nobody handled the event.
        /// </returns>
        /// <remarks>
        /// Under normal, default settings, this event is automatically generated by
        /// the system from the regular up and down events you inject.  You may use
        /// this function directly, though you'll probably want to disable the
        /// automatic click event generation first by using the
        /// setMouseClickEventGenerationEnabled function - this setting controls the
        /// auto-generation of events and also determines the default 'handled'
        /// state of the injected click events according to the rules used for
        /// mouse up/down events.
        /// </remarks>
        bool InjectMouseButtonTripleClick(MouseButton button);

        /// <summary>
        /// Tells the receiver to perform a clipboard copy operation.
        /// </summary>
        /// <returns>
        /// - true if the copy was successful
        /// - false if the copy was denied
        /// </returns>
        bool InjectCopyRequest();

        /// <summary>
        /// Tells the system to perform a clipboard cut operation.
        /// </summary>
        /// <returns>
        /// - true if the cut was successful
        /// - false if the cut was denied
        /// </returns>
        bool InjectCutRequest();

        /// <summary>
        /// Tells the system to perform a clipboard paste operation. 
        /// </summary>
        /// <returns>
        /// - true if the paste was successful
        /// - false if the paste was denied
        /// </returns>
        bool InjectPasteRequest();
    }
}