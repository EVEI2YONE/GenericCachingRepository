using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Helpers
{
    public static class CacheKeyHelper
    {
        private static readonly List<Type> ValidKeyAttributes = new List<Type>() { typeof(KeyAttribute) };
        public static object?[]? GetIds<T>(T? item) where T : class
            => item == null ? null : typeof(T).GetProperties().Where(prop => prop.GetCustomAttributes().Any(attr => ValidKeyAttributes.Contains(attr.GetType()))).Select(prop => prop.GetValue(item)).ToArray();

        public static string? GetKeyIds<T>(params object?[]? ids) where T : class
            => ids == null || !ids.Any() ? null : $"{typeof(T).Name}:{string.Join(",", ids)}";
        public static string? GetKey<T>(T? item) where T : class
            => GetKeyIds<T>(GetIds(item));
    }
}
