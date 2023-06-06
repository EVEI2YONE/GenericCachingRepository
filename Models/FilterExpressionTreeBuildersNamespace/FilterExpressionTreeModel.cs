using Models.DictionaryNamespace;
using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeBuildersNamespace
{
    public partial class FilterExpressionTreeBuilder
    {
        private List<FilterExpressionTree> _roots = new List<FilterExpressionTree>();
        public IEnumerable<FilterExpressionTree> Roots { get { return _roots; } }
        private AliasDictionary _aliases { get; set; } = new AliasDictionary();
        public IEnumerable<KeyValuePair<string, string>> Aliases { get => _aliases.AsEnumerable(); }
    }
}
