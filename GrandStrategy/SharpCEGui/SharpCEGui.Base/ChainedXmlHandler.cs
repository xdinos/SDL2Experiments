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
    /// <summary>
    /// Abstract XMLHandler based class
    /// </summary>
    public abstract class ChainedXmlHandler : XmlHandler
    {
        protected ChainedXmlHandler()
        {
            ChainedHandler = null;
            d_completed = false;
            DeleteChaniedHandler = true;
        }

        // TODO: virtual ~ChainedXMLHandler()
        //{
        //    CleanupChainedHandler();
        //}

        public override string GetSchemaName()
        {
            // this isn't likely to be used in ChainedXMLHandler instances
            return String.Empty;
        }

        public override string GetDefaultResourceGroup()
        {
            return String.Empty;
        }

        public override void ElementStart(string element, XMLAttributes attributes)
        {
            // chained handler gets first crack at this element
            if (ChainedHandler!=null)
            {
                ChainedHandler.ElementStart(element, attributes);
                // clean up if completed
                if (ChainedHandler.Completed())
                    CleanupChainedHandler();
            }
            else
                ElementStartLocal(element, attributes);
        }

        public override void ElementEnd(string element)
        {
            // chained handler gets first crack at this element
            if (ChainedHandler!=null)
            {
                ChainedHandler.ElementEnd(element);
                // clean up if completed
                if (ChainedHandler.Completed())
                    CleanupChainedHandler();
            }
            else
                ElementEndLocal(element);
        }

        /// <summary>
        /// returns whether this chained handler has completed.
        /// </summary>
        /// <returns></returns>
        public bool Completed()
        {
            return d_completed;
        }

        /// <summary>
        /// Function that handles elements locally (used at end of handler chain)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributes"></param>
        protected abstract void ElementStartLocal(string element, XMLAttributes attributes);

        /// <summary>
        /// Function that handles elements locally (used at end of handler chain)
        /// </summary>
        /// <param name="element"></param>
        protected abstract void ElementEndLocal(string element);

        /// <summary>
        /// clean up any chained handler.
        /// </summary>
        protected void CleanupChainedHandler()
        {
            if (DeleteChaniedHandler)
            {
                // TODO: CEGUI_DELETE_AO d_chainedHandler;
            }

            ChainedHandler = null;
        }

        /// <summary>
        /// should the chained handler be deleted by us?
        /// </summary>
        protected bool DeleteChaniedHandler;

        /// <summary>
        /// chained xml handler object.
        /// </summary>
        protected ChainedXmlHandler ChainedHandler;

        /// <summary>
        /// is the chained handler completed.
        /// </summary>
        protected bool d_completed;
        
        
    }
}