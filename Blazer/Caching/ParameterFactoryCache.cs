namespace Blazer.Caching
{
    using System;
    using System.Collections.Concurrent;

    using static ParameterFactory;

    internal static class ParameterFactoryCache
    {
        static readonly ConcurrentDictionary<Type, ParameterFactoryFunc> m_factoryCache = new ConcurrentDictionary<Type, ParameterFactoryFunc>();

        public static bool TryGet(Type key, out ParameterFactoryFunc factory)
        {
            return m_factoryCache.TryGetValue(key, out factory);
        }

        public static void Add(Type key, ParameterFactoryFunc value)
        {
            m_factoryCache.AddOrUpdate(key, value, (k, old) => value);
        }
    }
}
