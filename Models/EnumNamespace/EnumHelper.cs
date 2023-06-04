using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EnumNamespace
{
    public static class EnumHelper
    {
        public static string? MapEnum<T>(string? enumName) where T : Enum
            => string.IsNullOrWhiteSpace(enumName) ? null : typeof(T).GetEnumNames().FirstOrDefault(_enum => string.Compare(_enum, enumName.Trim(), StringComparison.InvariantCultureIgnoreCase) == 0);

        public static string MapEnumOrDefault<T>(string? enumName, T defaultEnum) where T : Enum
        {
            var mappedName = MapEnum<T>(enumName);
            if (string.IsNullOrWhiteSpace(mappedName))
                return defaultEnum.ToString();
            else
                return mappedName;
        }

        public static T? GetEnum<T>(string? enumName) where T : Enum
        {
            Enum.TryParse(typeof(T), MapEnum<T>(enumName), out var e);
            return (T?)e;
        }
    }
}
