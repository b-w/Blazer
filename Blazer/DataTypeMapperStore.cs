namespace Blazer
{
    using System;
    using System.Collections.Concurrent;
    using Blazer.DataTypeMappers;

    internal static class DataTypeMapperStore
    {
        static readonly ConcurrentDictionary<Type, IDataTypeMapper> m_mappers = new ConcurrentDictionary<Type, IDataTypeMapper>()
        {
            [typeof(byte[])] = new ByteArrayMapper()
        };

        public static bool TryGetMapper(Type forType, out IDataTypeMapper mapper)
        {
            return m_mappers.TryGetValue(forType, out mapper);
        }

        public static void RegisterMapper(Type forType, IDataTypeMapper mapper)
        {
            m_mappers.AddOrUpdate(forType, mapper, (key, oldValue) => mapper);
        }
    }
}
