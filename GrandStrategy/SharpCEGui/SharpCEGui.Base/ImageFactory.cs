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
    /// Interface for factory objects that create instances of classes
    /// derived from the Image class.
    /// </summary>
    /// <remarks>This interface is intended for internal use only.</remarks>
    public interface ImageFactory
    {
        //! base class virtual destructor.
        // TODO: virtual ~ImageFactory() {}

        /// <summary>
        /// Create an instance of the Image subclass that this factory creates.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Image Create(string name);

        /// <summary>
        /// Create an instance of the Image subclass that this factory creates
        /// using the given XMLAttributes object.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        Image Create(XMLAttributes attributes);

        /// <summary>
        /// Destroy an instance of the Image subclass that this factory creates.
        /// </summary>
        /// <param name="image"></param>
        void Destroy(Image image);
    }

    //! Templatised ImageFactory subclass used internally by the system.
    public class TplImageFactory<T> : ImageFactory where T : Image
    {
        public TplImageFactory(Func<string, T> namedCreator,
                               Func<XMLAttributes, T> attribCreator)
        {
            _namedCreator = namedCreator;
            _attribCreator = attribCreator;
        }

        public Image Create(string name)
        {
            // TODO: return new T(name);
            return _namedCreator(name);
        }

        public Image Create(XMLAttributes attributes)
        {
            // TODO: return new T(attributes);
            return _attribCreator(attributes);
        }

        public void Destroy(Image image)
        {
            image.Dispose();
        }

        private readonly Func<string, T> _namedCreator;
        private readonly Func<XMLAttributes, T> _attribCreator;
    };
}