namespace Blazer
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;

    internal static class DataRecordMap
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> m_getMethodMap = new ConcurrentDictionary<Type, MethodInfo>()
        {
            [typeof(string)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetString), new[] { typeof(int) }),
            [typeof(char)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChar), new[] { typeof(int) }),
            [typeof(bool)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBoolean), new[] { typeof(int) }),
            [typeof(byte)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte), new[] { typeof(int) }),
            [typeof(short)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt16), new[] { typeof(int) }),
            [typeof(int)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new[] { typeof(int) }),
            [typeof(long)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt64), new[] { typeof(int) }),
            [typeof(float)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetFloat), new[] { typeof(int) }),
            [typeof(double)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDouble), new[] { typeof(int) }),
            [typeof(decimal)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDecimal), new[] { typeof(int) }),
            [typeof(Guid)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetGuid), new[] { typeof(int) }),
            [typeof(DateTime)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDateTime), new[] { typeof(int) }),
            [typeof(object)] = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue), new[] { typeof(int) })
        };

        public static bool TryGetGetMethod(Type forType, out MethodInfo method)
        {
            var type = forType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
            {
                type = nullableType;
            }

#if FEATURE_TYPE_INFO
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
