using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DictionaryNamespace
{
    public class FilterExpressionTreeDictionary : Dictionary<string, FilterExpressionTree>, IDictionary<string, FilterExpressionTree>
    {
        private readonly IDictionary<string, FilterExpressionTree> _treeDictionary = new Dictionary<string, FilterExpressionTree>();

        public FilterExpressionTree? this[string root, string key]
        {
            get { return _treeDictionary[root]; }
            set 
            { 
                if(string.IsNullOrWhiteSpace(root))
                    throw new ArgumentNullException(nameof(root));
                if(string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));
                _treeDictionary[$"{root.Trim()}:{key.Trim()}"] = value; 
            }
        }
        
        public new void Add(string rootName, string key, FilterExpressionTree treeNode)
            => this[rootName, key] = treeNode;
    }
}
