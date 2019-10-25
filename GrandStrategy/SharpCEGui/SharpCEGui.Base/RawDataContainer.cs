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
using System.IO;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class used as the databuffer for loading files throughout the library.
    /// </summary>
    public class RawDataContainer : IDisposable
    {
        /// <summary>
        /// Constructor for RawDataContainer class
        /// </summary>
        public RawDataContainer()
        {
            mData = null;
            mSize = 0;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Release();
        }
        
        #endregion

        /// <summary>
        /// Set a pointer to the external data.
        /// </summary>
        /// <param name="data">
        /// Pointer to the uint8 data buffer.
        /// </param>
        public void SetData(byte[] data)
        {
            mData = data;
        }

        public void SetData(MemoryStream memoryStream)
        {
            if (_memoryStream!=null)
                _memoryStream.Dispose();
            
            _memoryStream = memoryStream;
        }

        // TODO: ...
        /*!
	    \brief
		    Return a pointer to the external data

	    \return
		    Pointer to an the uint8 data buffer.
	    */
        //uint8* getDataPtr(void) { return mData; }
        //const uint8* getDataPtr(void) const { return mData; }

        public Stream Stream()
        {
            return _memoryStream;
        }

        public byte[] GetBuffer()
        {
            return _memoryStream.GetBuffer();
        }

        /// <summary>
        /// Set the size of the external data.
        /// </summary>
        /// <param name="size">
        /// size_t containing the size of the external data
        /// </param>
        public void SetSize(long size)
        {
            mSize = size;
        }

        /// <summary>
        /// Get the size of the external data.
        /// </summary>
        /// <returns>
        /// size_t containing the size of the external data
        /// </returns>
        public long GetSize()
        {
            if (_memoryStream != null)
                return _memoryStream.Length;

            return mSize;
        }

        /// <summary>
        /// Release supplied data.
        /// </summary>
        public void Release()
        {
            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }
        }

        private byte[] mData;
        private long mSize;
        private MemoryStream _memoryStream;
    }
}