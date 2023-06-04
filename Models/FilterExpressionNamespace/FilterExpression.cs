using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionNamespace
{
    public partial class FilterExpression
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }
}
