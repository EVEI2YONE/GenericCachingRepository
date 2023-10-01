using System.Reflection;

namespace GenericCachingRepository.Helpers
{
    public static class PaginationHelper
    {
        public static IEnumerable<string> GetKeyPropertyNames<T>() where T : class
            => typeof(T).GetProperties().Where(prop => prop.GetCustomAttributes().Any(attr => SharedLists.ValidKeyAttributes.Contains(attr.GetType()))).Select(prop => prop.Name);
    }
}
