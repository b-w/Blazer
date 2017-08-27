namespace Blazer.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;

    using static DataMapperFactory;

    internal static class DataMapperCache
    {
        public sealed class Key : IEquatable<Key>
        {
            private readonly IEqualityComparer<string> m_stringComparer;
            private readonly int m_hashCode;

            private readonly string m_connectionString;
            private readonly string m_command;
            private readonly Type m_returnType;

            public Key(IDbCommand command, Type returnType)
            {
                m_stringComparer = StringComparer.Ordinal;

                m_connectionString = command.Connection.ConnectionString;
                m_command = command.CommandText;
                m_returnType = returnType;

                unchecked
                {
                    var h = (int)2166136261;
                    for (int i = 0; i < m_connectionString.Length; i++)
                    {
                        h = (h * 16777619) ^ m_connectionString[i];
                    }
                    for (int i = 0; i < m_command.Length; i++)
                    {
                        h = (h * 16777619) ^ m_command[i];
                    }
                    h = (h * 16777619) ^ returnType.GetHashCode();
                    m_hashCode = h;
                }
            }

            public override int GetHashCode()
            {
                return m_hashCode;
            }

            public bool Equals(Key other)
            {
                if (other == null)
                {
                    return false;
                }

                return m_returnType.Equals(other.m_returnType)
                    && m_stringComparer.Equals(m_command, other.m_command)
                    && m_stringComparer.Equals(m_connectionString, other.m_connectionString);
            }
        }

        private static readonly ConcurrentDictionary<Key, DataMapper> m_mapperCache = new ConcurrentDictionary<Key, DataMapper>();

        public static bool TryGet(Key key, out DataMapper mapper) => m_mapperCache.TryGetValue(key, out mapper);

        public static void Add(Key key, DataMapper value) => m_mapperCache.AddOrUpdate(key, value, (k, old) => value);
    }
}
