using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterRuleNamespace
{
    public partial class FilterRule
    {
        public FilterRule? GetRule(IEnumerable<FilterRule> rules)
            => null;
        public string ToString()
            => !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Value)) ? $"{Name}: {Value}" : string.Empty;
            //=> rules.FirstOrDefault(rule => Value == null ? false : Value.Split(typeof(LogicalOperator).GetEnumNames(), StringSplitOptions.None).Select(x => x?.Trim()).Any(split => split == null ? false : split.IndexOf(rule.Name, StringComparison.InvariantCultureIgnoreCase) > -1));
    }
}
