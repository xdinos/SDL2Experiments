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
using System.Linq.Expressions;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Template based WindowRendererFactory that can be used to automatically
    /// generate a WindowRendererFactory given a WindowRenderer based class.
    /// 
    /// The advantage of this over the previous macro based methods is that there is
    /// no longer any need to have any supporting code or structure in order to add
    /// new WindowRenderer types to the system, rather you can just do:
    /// \code
    /// CEGUI::WindowRendererManager::addFactory<TplWindowRendererFactory<MyWindowRenderer> >();
    /// \endcode
    /// </summary>
    /// <typeparam name="T">
    /// Specifies the WindowRenderer based class that the factory will create and
    /// destroy instances of.
    /// </typeparam>
    public class TplWindowRendererFactory<T> : WindowRendererFactory where T : WindowRenderer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TplWindowRendererFactory()
            : base((string) typeof (T).GetField("TypeName").GetValue(null))
        {
            var objectConstructor = typeof(T).GetConstructor(new[] { typeof(string) });
            if (objectConstructor == null)
                throw new InvalidOperationException("Constructor not found.");

            var parameterType = Expression.Parameter(typeof(string), "type");

            _constructor = Expression.Lambda<Func<string, WindowRenderer>>(
                Expression.Convert(
                    Expression.New(objectConstructor, parameterType),
                    typeof(WindowRenderer)),
                parameterType)
                .Compile();
        }

        // Implement WindowRendererFactory interface

        public override WindowRenderer Create()
        {
            //return CEGUI_NEW_AO T(T::TypeName);
            //throw new NotImplementedException();
            return _constructor(GetName());
        }

        public override void Destroy(WindowRenderer wr)
        {
        }

        private readonly Func<string, WindowRenderer> _constructor;
    }
}