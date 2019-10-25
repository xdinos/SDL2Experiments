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
using System.Text;

namespace SharpCEGui.Base
{
    // TODO: redisign this to be more .net-sh
    /// <summary>
    /// Defines a clipboard handling class
    /// <para>
    /// Usually, there is just one instance of this class, owned by CEGUI::System,
    /// it contains internal CEGUI clipboard that may be (optionally) synchronised
    /// with native clipboard if user sets NativeClipboardProvider with:
    /// <code>
    /// CEGUI::System::getSingleton()->getClipboard()->setNativeProvider(customProvider)
    /// </code>
    /// Where customProvider is of course user implemented clipboard provider.
    /// </para>
    /// </summary>
    public class Clipboard
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Clipboard()
        {
            d_mimeType = "text/plain"; // reasonable default I think
            d_buffer = null;
            d_bufferSize = 0;
            d_nativeProvider = null;
        }

        // TODO: destructor
        // TODO: ~Clipboard();

        /// <summary>
        /// sets native clipboard provider
        /// </summary>
        /// <param name="provider">
        /// provider the native clipboard provider to set
        /// </param>
        /// <seealso cref="INativeClipboardProvider"/>
        /// <remarks>
        /// You are required to deallocate given provider, this class doesn't take ownership!)
        /// </remarks>
        public void SetNativeProvider(INativeClipboardProvider provider)
        {
            d_nativeProvider = provider;
        }

        /// <summary>
        /// retrieves currently set native clipboard provider
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="INativeClipboardProvider"/>
        public INativeClipboardProvider GetNativeProvider()
        {
            return d_nativeProvider;
        }

        /*!
        \brief sets contents of this clipboard to given raw data
    
        \param mimeType describes type of the data in the clipboard
        \param buffer raw buffer containing data to push into the clipboard
        \param size size (in bytes) of given data
        */
        public void SetData(string mimeType, byte[] buffer, int size)
        {
            d_mimeType = mimeType;

            d_buffer = buffer;

            if (size != d_bufferSize)
            {
            //    if (d_buffer != null)
            //    {
            //        CEGUI_DELETE_ARRAY_PT(d_buffer, BufferElement, d_bufferSize, Clipboard);
            //        d_buffer = null;
            //    }

            //    d_bufferSize = size;
            //    d_buffer = CEGUI_NEW_ARRAY_PT(BufferElement, d_bufferSize, Clipboard);
            }

            //memcpy(d_buffer, buffer, d_bufferSize);

            // we have set the data to the internal clipboard, now sync it with the
            // system-wide native clipboard if possible
            if (d_nativeProvider!=null)
            {
                d_nativeProvider.SendToClipboard(d_mimeType, d_buffer, d_bufferSize);
            }
        }
    
        /*!
        \brief retrieves contents of this clipboard as raw data
    
        \param mimeType current mime type
        \param buffer the raw data buffer (can be 0 if size == 0!)
        \param size size of the returned buffer
    
        You shan't change the buffer contents, only read from it!
        */

        public void GetData(out string mimeType, out byte[] buffer, out int size)
        {
            // first make sure we are in sync with system-wide native clipboard
            // (if possible)
            if (d_nativeProvider!=null)
            {
                int retrievedSize;
                byte[] retrievedBuffer;

                d_nativeProvider.RetrieveFromClipboard(d_mimeType, out retrievedBuffer, out retrievedSize);

                if (retrievedSize != d_bufferSize)
                {
                //    if (d_buffer != 0)
                //    {
                //        CEGUI_DELETE_ARRAY_PT(d_buffer, BufferElement, d_bufferSize, Clipboard);
                //        d_buffer = 0;
                //    }

                    d_bufferSize = retrievedSize;
                //    d_buffer = CEGUI_NEW_ARRAY_PT(BufferElement, d_bufferSize, Clipboard);
                }

                //memcpy(d_buffer, retrievedBuffer, retrievedSize);
                d_buffer = retrievedBuffer;
            }

            mimeType = d_mimeType;
            buffer = d_buffer;
            size = d_bufferSize;
        }

        /// <summary>
        /// convenience method that sets contents to given string
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            //throw new NotImplementedException();
            //// could be just ASCII if std::string is used as CEGUI::String
            //const char* utf8_bytes = text.c_str();
    
            //// we don't want the actual string length, that might not be the buffer size
            //// in case of utf8!
            //// this gets us the number of bytes until \0 is encountered
            //const size_t size = strlen(utf8_bytes);

            var utf8_bytes = Encoding.UTF8.GetBytes(text);
            
            SetData("text/plain", utf8_bytes, utf8_bytes.Length);
        }

        /// <summary>
        /// convenience method that retrieves contents as a string
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            string mimeType;
            byte[] buffer;
            int size;
   
            // we have to use this, can't use the member variables directly because of
            // the native clipboard provider!
            GetData(out mimeType, out buffer, out size);
    
            if (mimeType == "text/plain" && size != 0)
            {
                // d_buffer an utf8 or ASCII C string (ASCII if std::string is used)
        
                // !!! However it is not null terminated !!! So we have to tell String
                // how many code units (not code points!) there are.
                return Encoding.UTF8.GetString(buffer);
            }
            else
            {
                // the held mime type differs, it's not plain text so we can't
                // return it as just string
                return String.Empty;
            }
        }

        #region Fields

        /// <summary>
        /// mime type of the current content
        /// </summary>
        private string d_mimeType;

        // just implementation specific
        //typedef char BufferElement;
        
        // raw data buffer containing current clipboard contents
        private byte[] d_buffer;

        // size (in bytes) of the raw buffer
        private int d_bufferSize;
    
        // native clipboard provider if any
        private INativeClipboardProvider d_nativeProvider;

        #endregion
    }
}