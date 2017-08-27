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

        public object Get(string key) => m_values[key];

        #region IDynamicMetaObjectProvider

        public DynamicMetaObject GetMetaObject(Expression parameter) => new BlazerDynamicMetaObject(parameter, this);

        #endregion

        #region IDictionary<string, object>

        public ICollection<string> Keys => m_values.Keys;

        public ICollection<object> Values => m_values.Values;

        public int Count => m_values.Count;

        public bool IsReadOnly => m_values.IsReadOnly;

        public object this[string key]
        {
            get => m_values[key];
            set => m_values[key] = value;
        }

        public bool ContainsKey(string key) => m_values.ContainsKey(key);

        public void Add(string key, object value) => m_values.Add(key, value);

        public bool Remove(string key) => m_values.Remove(key);

        public bool TryGetValue(string key, out object value) => m_values.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object> item) => m_values.Add(item);

        public void Clear() => m_values.Clear();

        public bool Contains(KeyValuePair<string, object> item) => m_values.Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => m_values.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object> item) => m_values.Remove(item);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => m_values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_values.GetEnumerator();

        #endregion
    }
}
