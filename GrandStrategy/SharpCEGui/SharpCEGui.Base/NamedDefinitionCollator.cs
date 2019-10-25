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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Helper container used to implement inherited collections of component
    /// definitions specified in a WidgetLook.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class NamedDefinitionCollator<K, V> : IEnumerable<V>
    {
        /// <summary>
        /// Return total number of values in the collection.
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return d_values.Count;
        }

        /// <summary>
        /// return reference to value at given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public V At(int idx)
        {
            return d_values[idx].Item2;
        }

        /// <summary>
        /// Set value for a given key.  If a value is already associated with the
        /// given key, it is replaced with the new value and the value is moved to
        /// the end of the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Set(K key, V val)
        {
            //typename ValueArray::iterator i =
            //    std::find_if(d_values.begin(), d_values.end(), pred(key));

            //if (i != d_values.end())
            //    d_values.erase(i);

            //d_values.push_back(std::make_pair(key, val));

            var item = d_values.SingleOrDefault(x => x.Item1.Equals(key));
            if (item != null)
                d_values.Remove(item);

            d_values.Add(new Tuple<K, V>(key, val));
        }

        #region Implementation of IEnumerable<V>

        public IEnumerator<V> GetEnumerator()
        {
            return d_values.Select(item => item.Item2).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        protected List<Tuple<K, V>> d_values = new List<Tuple<K, V>>();
    }
}