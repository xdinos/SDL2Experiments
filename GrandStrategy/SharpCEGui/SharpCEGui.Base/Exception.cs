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
    /// Root exception class used within the GUI system.
    /// </summary>
    public class Exception : global::System.Exception
    {
        public Exception(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// Exception class used when none of the other classes are applicable.
    /// </summary>
    public class GenericException : Exception
    {
        public GenericException(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// Exception class used when a request was made for an unknown object.
    /// </summary>
    public class UnknownObjectException : Exception
    {
        public UnknownObjectException(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// Exception class used when some impossible request was made of the system.
    /// </summary>
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message)
            :base(message)
        {}
    }

    /// <summary>
    /// Exception class used when an attempt is made create a named object of a
    /// particular type when an object of the same type already exists with the same
    /// name.
    /// </summary>
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message)
            :base(message)
        {}
    }
}