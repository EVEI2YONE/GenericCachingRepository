using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GenericCachingRepository.Helpers
{
    public static class CacheKeyHelper
    {        
        public static object?[]? GetIds<T>(T? item) where T : class
            => item == null ? null : typeof(T).GetProperties().Where(prop => prop.GetCustomAttributes().Any(attr => SharedLists.ValidKeyAttributes.Contains(attr.GetType()))).Select(prop => prop.GetValue(item)).ToArray();

        public static string? GetKeyIds<T>(params object?[]? ids) where T : class
            => ids == null || !ids.Any() ? null : $"{typeof(T).Name}:{string.Join(",", ids)}";
        public static string? GetKey<T>(T? item) where T : class
            => GetKeyIds<T>(GetIds(item));
    }
}
