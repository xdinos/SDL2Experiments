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
    /// 
    /// </summary>
    public enum AutoScaledMode
    {
        /// <summary>
        /// No auto scaling takes place
        /// </summary>
        Disabled,

        /// <summary>
        /// Objects are auto scaled depending on their original height and current
        /// resolution height. Horizontal scaling is computed to keep aspect ratio.
        /// </summary>
        Vertical,

        /// <summary>
        /// Objects are auto scaled depending on their original width and current
        /// resolution width. Horizontal scaling is computed to keep aspect ratio.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Objects are auto scaled by the smaller scaling factor of the two.
        /// </summary>
        Min,

        /// <summary>
        /// Objects are auto scaled by the larger scaling factor of the two.
        /// </summary>
        Max,

        /// <summary>
        /// Objects are auto scaled depending on their original size and current
        /// resolution. Both dimensions are scaled accordingly.
        /// <para>
        /// This was the only auto scaling behavior available in 0.7 and it might
        /// break aspect ratios!
        /// </para>
        /// </summary>
        Both
    }
}