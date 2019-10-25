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
    /// Enumeration of values representing mathematical operations on dimensions.
    /// </summary>
    public enum DimensionOperator
    {
        /// <summary>
        /// Do nothing operator.
        /// </summary>
        Noop,       // DOP_NOOP,

        /// <summary>
        /// Dims should be added.
        /// </summary>
        Add,        // DOP_ADD,

        /// <summary>
        /// Dims should be subtracted.
        /// </summary>
        Subtract,   // DOP_SUBTRACT,

        /// <summary>
        /// Dims should be multiplied.
        /// </summary>
        Multiply,   // DOP_MULTIPLY,

        /// <summary>
        /// Dims should be divided.
        /// </summary>
        Divide,     // DOP_DIVIDE
    };
}