namespace Blazer.Caching
{
    using System;
    using System.Collections.Concurrent;

    using static ParameterFactory;

    internal static class ParameterFactoryCache
    {
        private static readonly ConcurrentDictionary<Type, ParameterFactoryFunc> m_factoryCache = new ConcurrentDictionary<Type, ParameterFactoryFunc>();

#if FEATURE_FORMATTABLE_STRING
        private static readonly ConcurrentDictionary<string, ParameterFactoryFuncFormattableString> m_factoryCacheFormattableString = new ConcurrentDictionary<string, ParameterFactoryFuncFormattableString>();

        public static bool TryGet(string key, out ParameterFactoryFuncFormattableString factory) => m_factoryCacheFormattableString.TryGetValue(key, out factory);

        public static void Add(string key, ParameterFactoryFuncFormattableString value) => m_factoryCacheFormattableString.AddOrUpdate(key, value, (k, old) => value);
#endif

        public static bool TryGet(Type key, out ParameterFactoryFunc factory) => m_factoryCache.TryGetValue(key, out factory);

        public static void Add(Type key, ParameterFactoryFunc value) => m_factoryCache.AddOrUpdate(key, value, (k, old) => value);
    }
}
