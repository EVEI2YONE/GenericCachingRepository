using System.Reflection;

namespace GenericCachingRepository
{
    public static class Extensions
    {
        private static string format = "yyyy, MM, dd, HH, mm, ss";
        public static string DateTimeLINQFormat(this DateTime? time) => (!time.HasValue) ? string.Empty : time.Value.ToString(format);
        public static string DateTimeLINQFormat(this DateTime time) => time.ToString(format);

        public static Type GetUnderlyingPropertyType(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type);
            }
            return propertyInfo.PropertyType;
        }
    }
}
