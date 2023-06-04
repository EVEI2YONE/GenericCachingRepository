using Models.EnumNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RuleNamespace
{
    public partial class Rule
    {
        public Rule? GetRule(IEnumerable<Rule> rules)
            => null;
            //=> rules.FirstOrDefault(rule => Value == null ? false : Value.Split(typeof(LogicalOperator).GetEnumNames(), StringSplitOptions.None).Select(x => x?.Trim()).Any(split => split == null ? false : split.IndexOf(rule.Name, StringComparison.InvariantCultureIgnoreCase) > -1));
    }
}
