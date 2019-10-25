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
    /// Lightweight interface providing access to the system-wide native clipboard.
    /// </summary>
    public interface INativeClipboardProvider
    {
        // TODO: virtual ~NativeClipboardProvider();

        /// <summary>
        /// sends given data to native clipboard
        /// </summary>
        /// <param name="mimeType"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        void SendToClipboard(string mimeType, byte[] buffer, int size);

        /// <summary>
        /// retrieves given data from native clipboard
        /// </summary>
        /// <param name="mimeType"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        void RetrieveFromClipboard(string mimeType, out byte[] buffer, out int size);
    }
}