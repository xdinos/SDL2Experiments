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

using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Abstract class to wrap a Bidi visual mapping of a text string.
    /// </summary>
    public abstract class BidiVisualMapping
    {
        /// <summary>
        /// Gets the Bidi char type of a char.
        /// </summary>
        /// <param name="charToCheck">
        /// The char character code that will be checked.
        /// </param>
        /// <returns>
        /// One of the BidiCharType enumerated values indicating the Bidi char type.
        /// </returns>
        public abstract BidiCharType GetBidiCharType(char charToCheck);

        /// <summary>
        /// Reorder a string from a logical (type order) bidi string to a visual (the way it displayed) string.
        /// </summary>
        /// <param name="logical">
        /// String object to be reordered.
        /// </param>
        /// <param name="visual">
        /// String object containing the result reordered string.
        /// </param>
        /// <param name="l2v">
        /// List of integers that map the pos of each char from logical string in the visual string.
        /// </param>
        /// <param name="v2l">
        /// List of integers that map the pos of each char from visual string in the logical string.
        /// </param>
        /// <returns>
        /// - true if successful.
        /// - false if the operation failed.
        /// </returns>
        public abstract bool ReorderFromLogicalToVisual(string logical,
                                                        out string visual,
                                                        List<int> l2v,
                                                        List<int> v2l);

        /// <summary>
        /// Use reorderFromLogicalToVisual to update the internal visual mapping
        /// data and visual string representation based upon the logical string
        /// \a logical.
        /// </summary>
        /// <param name="logical">
        /// String object representing the logical text order.
        /// </param>
        /// <returns>
        /// - true if the update was a success.
        /// - false if something went wrong.
        /// </returns>
        public bool UpdateVisual(string logical)
        {
            return ReorderFromLogicalToVisual(logical, out d_textVisual, d_l2vMapping, d_v2lMapping);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetL2vMapping()
        {
            return d_l2vMapping;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetV2lMapping()
        {
            return d_v2lMapping;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTextVisual()
        {
            return d_textVisual;
        }

        protected List<int> d_l2vMapping = new List<int>();
        protected List<int> d_v2lMapping = new List<int>();
        protected string d_textVisual;
    }
}