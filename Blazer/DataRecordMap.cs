namespace Blazer
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;

    internal static class DataRecordMap
    {
        static readonly ConcurrentDictionary<Type, MethodInfo> m_getMethodMap = new ConcurrentDictionary<Type, MethodInfo>()
        {
            [typeof(string)] = typeof(IDataRecord).GetMethod("GetString", new[] { typeof(int) }),
            [typeof(char)] = typeof(IDataRecord).GetMethod("GetChar", new[] { typeof(int) }),
            [typeof(bool)] = typeof(IDataRecord).GetMethod("GetBoolean", new[] { typeof(int) }),
            [typeof(byte)] = typeof(IDataRecord).GetMethod("GetByte", new[] { typeof(int) }),
            [typeof(short)] = typeof(IDataRecord).GetMethod("GetInt16", new[] { typeof(int) }),
            [typeof(int)] = typeof(IDataRecord).GetMethod("GetInt32", new[] { typeof(int) }),
            [typeof(long)] = typeof(IDataRecord).GetMethod("GetInt64", new[] { typeof(int) }),
            [typeof(float)] = typeof(IDataRecord).GetMethod("GetFloat", new[] { typeof(int) }),
            [typeof(double)] = typeof(IDataRecord).GetMethod("GetDouble", new[] { typeof(int) }),
            [typeof(decimal)] = typeof(IDataRecord).GetMethod("GetDecimal", new[] { typeof(int) }),
            [typeof(Guid)] = typeof(IDataRecord).GetMethod("GetGuid", new[] { typeof(int) }),
            [typeof(DateTime)] = typeof(IDataRecord).GetMethod("GetDateTime", new[] { typeof(int) }),
            [typeof(object)] = typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) })
        };

        public static bool TryGetGetMethod(Type forType, out MethodInfo method)
        {
            var type = forType;
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }
#if NETSTANDARD
            if (type.GetTypeInfo().IsEnum && !m_getMethodMap.ContainsKey(type))
#else
            if (type.IsEnum && !m_getMethodMap.ContainsKey(type))
#endif
            {
                type = Enum.GetUnderlyingType(type);
            }
            return m_getMethodMap.TryGetValue(type, out method);
        }
    }
}
