namespace Blazer
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    internal static class DbTypeStore
    {
        static readonly ConcurrentDictionary<Type, DbType> m_typeMap = new ConcurrentDictionary<Type, DbType>()
        {
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(bool)] = DbType.Boolean,
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(object)] = DbType.Object,
            [typeof(byte[])] = DbType.Binary
        };

        public static bool TryGetDbType(Type forType, out DbType dbType)
        {
            var type = forType;
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }
            if (type.IsEnum && !m_typeMap.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            return m_typeMap.TryGetValue(type, out dbType);
        }

        public static void RegisterType(Type type, DbType dbType)
        {
            m_typeMap.AddOrUpdate(type, dbType, (key, oldValue) => dbType);
        }
    }
}
