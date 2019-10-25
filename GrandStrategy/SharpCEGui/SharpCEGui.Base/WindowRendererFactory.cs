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
    /// Base-class for WindowRendererFactory.
    /// </summary>
    public abstract class WindowRendererFactory
    {
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="name">
        /// Type name for this window renderer factory
        /// </param>
        protected WindowRendererFactory(string name)
        {
            d_factoryName = name;
        }

        // TODO: Destructor
        // TODO: virtual ~WindowRendererFactory() {}

        /// <summary>
        /// Returns the type name of this window renderer factory.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return d_factoryName;
        }

        /// <summary>
        /// Creates and returns a new window renderer object.
        /// </summary>
        /// <returns></returns>
        public abstract WindowRenderer Create();

        /// <summary>
        /// Destroys a window renderer object previously created by us.
        /// </summary>
        /// <param name="wr"></param>
        public abstract void Destroy(WindowRenderer wr);

        /// <summary>
        /// Our factory type name.
        /// </summary>
        protected string d_factoryName;
    }
}