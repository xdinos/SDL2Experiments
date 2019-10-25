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
    // TODO: Convert to interface ??

    /// <summary>
    /// Abstract class that defines the required interface for all WindowFactory
    /// objects.
    /// 
    /// A WindowFactory is used to create and destroy windows of a specific type.
    /// For every type of Window object wihin the system (widgets, dialogs, movable
    /// windows etc) there must be an associated WindowFactory registered with the
    /// WindowFactoryManager so that the system knows how to create and destroy
    /// those types of Window base object.
    /// </summary>
    public abstract class WindowFactory
    {
        /// <summary>
        /// Create a new Window object of whatever type this WindowFactory produces.
        /// </summary>
        /// <param name="name">
        /// A unique name that is to be assigned to the newly created Window object
        /// </param>
        /// <returns>
        /// Pointer to the new Window object.
        /// </returns>
        public abstract Window CreateWindow(string name);

        /// <summary>
        /// Destroys the given Window object.
        /// </summary>
        /// <param name="window">
        /// Pointer to the Window object to be destroyed.
        /// </param>
        public abstract void DestroyWindow(Window window);

        /// <summary>
        /// Get the string that describes the type of Window object this
        /// WindowFactory produces.
        /// </summary>
        /// <returns>
        /// String object that contains the unique Window object type produced by
        /// this WindowFactory
        /// </returns>
        public string GetTypeName()
        {
            return d_type;
        }

        // TODO:
        ////! Destructor.
        //virtual ~WindowFactory()
        //{}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        protected WindowFactory(string type)
        {
            d_type = type;
        }

        /// <summary>
        /// String holding the type of object created by this factory.
        /// </summary>
        protected string d_type;
    }
}