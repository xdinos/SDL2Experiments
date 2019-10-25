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

using System;

namespace SharpCEGui.Base
{
    public class SystemKeys
    {
        public enum SystemKey
        {
            None          = 0x0000,
            LeftMouse     = 0x0001,
            RightMouse    = 0x0002,
            Shift         = 0x0004,
            Control       = 0x0008,
            MiddleMouse   = 0x0010,
            X1Mouse       = 0x0020,
            X2Mouse       = 0x0040,
            Alt           = 0x0080
        };

        public SystemKeys()
        {
            d_current = 0;
            d_leftShift = false;
            d_rightShift = false;
            d_leftCtrl = false;
            d_rightCtrl = false;
            d_leftAlt = false;
            d_rightAlt = false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public uint Get()
        {
            return d_current;
        }

        public bool IsPressed(SystemKey key)
        {
            throw new NotImplementedException();
            //return (d_current & key) || (!key && !d_current);
        }

        /// <summary>
        /// notify that the given key was presed
        /// </summary>
        /// <param name="key"></param>
        public void KeyPressed(Key.Scan key)
        {
            UpdatePressedStateForKey(key, true);
            UpdateSystemKeyState(KeyCodeToSystemKey(key));
        }

        //! notify that the given key was released.
        public void KeyReleased(Key.Scan key)
        {
            UpdatePressedStateForKey(key, false);
            UpdateSystemKeyState(KeyCodeToSystemKey(key));
        }

        /// <summary>
        /// notify that the given mouse button was pressed.
        /// </summary>
        /// <param name="button"></param>
        public void MouseButtonPressed(MouseButton button)
        {
            d_current |= (uint) MouseButtonToSystemKey(button);
        }

        /// <summary>
        /// notify that the given mouse button was released.
        /// </summary>
        /// <param name="button"></param>
        public void MouseButtonReleased(MouseButton button)
        {
            d_current &= ~((uint)MouseButtonToSystemKey(button));
        }

        public static SystemKey MouseButtonToSystemKey(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return SystemKey.LeftMouse;

                case MouseButton.RightButton:
                    return SystemKey.RightMouse;

                case MouseButton.MiddleButton:
                    return SystemKey.MiddleMouse;

                case MouseButton.X1Button:
                    return SystemKey.X1Mouse;

                case MouseButton.X2Button:
                    return SystemKey.X2Mouse;

                default:
                    return SystemKey.None;
            }
        }

        public static SystemKey KeyCodeToSystemKey(Key.Scan key)
        {
            switch (key)
            {
            case Key.Scan.RightShift:
            case Key.Scan.LeftShift:
                return SystemKey.Shift;

            case Key.Scan.LeftControl:
            case Key.Scan.RightControl:
                return SystemKey.Control;

            case Key.Scan.LeftAlt:
            case Key.Scan.RightAlt:
                return SystemKey.Alt;

            default:
                return SystemKey.None;
            }
        }

        private void UpdatePressedStateForKey(Key.Scan key, bool state)
        {
            switch (key)
            {
                case Key.Scan.LeftShift:
                    d_leftShift = state;
                    break;

                case Key.Scan.RightShift:
                    d_rightShift = state;
                    break;

                case Key.Scan.LeftControl:
                    d_leftCtrl = state;
                    break;

                case Key.Scan.RightControl:
                    d_rightCtrl = state;
                    break;

                case Key.Scan.LeftAlt:
                    d_leftAlt = state;
                    break;

                case Key.Scan.RightAlt:
                    d_rightAlt = state;
                    break;
            }
        }

        private void UpdateSystemKeyState(SystemKey syskey)
        {
            switch (syskey)
            {
                case SystemKey.Shift:
                    if (d_leftShift || d_rightShift)
                        d_current |= (uint) SystemKey.Shift;
                    else
                        d_current &= ~(uint) SystemKey.Shift;
                    break;

                case SystemKey.Control:
                    if (d_leftCtrl || d_rightCtrl)
                        d_current |= (uint) SystemKey.Control;
                    else
                        d_current &= ~(uint) SystemKey.Control;
                    break;

                case SystemKey.Alt:
                    if (d_leftAlt || d_rightAlt)
                        d_current |= (uint) SystemKey.Alt;
                    else
                        d_current &= ~(uint) SystemKey.Alt;
                    break;
            }
        }

        private uint d_current;

        private bool d_leftShift;
        private bool d_rightShift;
        private bool d_leftCtrl;
        private bool d_rightCtrl;
        bool d_leftAlt;
        bool d_rightAlt;
    }
}