namespace Blazer.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;

    internal sealed class BlazerDynamicObject : IDynamicMetaObjectProvider, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> m_values;

        public BlazerDynamicObject()
        {
            m_values = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public BlazerDynamicObject(int initialCapacity)
        {
            m_values = new Dictionary<string, object>(initialCapacity, StringComparer.Ordinal);
        }

        public object Set(string key, object value)
        {
            m_values[key] = value;
            return value;
        }

        public object Get(string key)
        {
            return m_values[key];
        }

        #region IDynamicMetaObjectProvider

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new BlazerDynamicMetaObject(parameter, this);
        }

        #endregion

        #region IDictionary<string, object>

        public ICollection<string> Keys
        {
            get
            {
                return m_values.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return m_values.Values;
            }
        }

        public int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_values.IsReadOnly;
            }
        }

        public object this[string key]
        {
            get
            {
                return m_values[key];
            }

            set
            {
                m_values[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return m_values.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            m_values.Add(key, value);
        }

        public bool Remove(string key)
        {
            return m_values.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return m_values.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            m_values.Add(item);
        }

        public void Clear()
        {
            m_values.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return m_values.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            m_values.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return m_values.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return m_values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
        }

        #endregion
    }
}
