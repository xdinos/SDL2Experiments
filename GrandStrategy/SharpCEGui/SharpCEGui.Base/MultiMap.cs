using System.Collections.Generic;

namespace SharpCEGui.Base
{
    public class MultiMap<TKey, TValue>
    {
        Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TValue>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<TValue> this[TKey key]
        {
            get
            {
                List<TValue> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<TValue>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

}