using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCachingRepository.Helpers
{
    public static class SharedLists
    {
        public static readonly List<Type> ValidKeyAttributes = new List<Type>() { typeof(KeyAttribute) };
    }
}
