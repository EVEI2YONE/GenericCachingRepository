using Models.FilterExpressionTreesNamespace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DictionaryNamespace
{
    public class FilterExpressionTreeDictionary
    {
        private readonly IDictionary<string, FilterExpressionTree> _treeDictionary = new Dictionary<string, FilterExpressionTree>();
        private readonly List<string> _rootKeys = new List<string>();

        public IEnumerable<string> RootKeys { get { return _rootKeys.AsEnumerable(); } }
        public IEnumerable<KeyValuePair<string, FilterExpressionTree>> Entries => _treeDictionary.AsEnumerable();
        public int Count => _treeDictionary.Count();

        public FilterExpressionTree? this[string key]
        {
            get => _treeDictionary.ContainsKey(key) ? _treeDictionary[key] : null;
            set
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));
                var rootKey = ExtractRootKey(key);
                if (!_rootKeys.Contains(rootKey))
                    _rootKeys.Add(rootKey);
                _treeDictionary[key] = value;
            }
        }

        public FilterExpressionTree? FindNode(FilterExpressionTree root, string childNode)
            => (string.IsNullOrWhiteSpace(root?.Expression?.Name) || string.IsNullOrWhiteSpace(childNode)
            ? null
            : this[$"{ExtractRootKey(root.Expression.Name)}:{childNode}"]);

        private string? ExtractRootKey(string key) => key.Split(":").FirstOrDefault();
        private string? ExtractNonRootKey(string key) => string.Join(":", key.Split(':').Skip(1));
        private string? CreateRootKey(string? rootKey, string? nodeKey) => (string.IsNullOrWhiteSpace(rootKey + nodeKey)) ? null : $"{rootKey}:{nodeKey}";
        private string? CreateMigrateKey(string? rootKey, string? existingKey) => (string.IsNullOrWhiteSpace(rootKey + existingKey)) ? null : $"{rootKey}:{ExtractNonRootKey(existingKey)}";

        //SourceRootLabel:ChildLabel => TargetRootLabel:ChidLabel
        public void UpdateRoot(string rootSource, string rootTarget)
        {
            //Source is the old tree whose children nodes need to be updated,
            //While Target node is a leaf node of a parent tree node which is connecting to Source tree

            //Get root keys
            var extractedSourceKey = ExtractRootKey(rootSource);
            var extractedTargetKey = ExtractRootKey(rootTarget);
            //Get source nodes
            var rootSourceNodes = _treeDictionary.Where(node => ExtractRootKey(node.Key) == extractedSourceKey);
            //Get target root to connect to source nodes
            var rootTargetNode = _treeDictionary.FirstOrDefault(node => ExtractRootKey(node.Key) == extractedTargetKey && node.Value.Parent == null);

            rootSourceNodes.ToList().ForEach(node =>
            {
                //Update Keys from source tree to target tree
                _treeDictionary.Remove(node.Key);
                var targetKey = CreateMigrateKey(extractedTargetKey, node.Key);
                _treeDictionary.TryAdd(targetKey, node.Value);
            });
            _rootKeys.Remove(extractedSourceKey);
        }
        public void Add(string? rootKey, string? key, FilterExpressionTree treeNode)
            => this[CreateRootKey(rootKey, key)] = treeNode;
        private void MigrateTreeNode(string? rootKey, FilterExpressionTree node)
        {
            if(string.IsNullOrWhiteSpace(node?.Expression?.Name))
                throw new ArgumentNullException(nameof(node), $"Migrating node may be null. Unable to construct key to migrate to '{ExtractRootKey(rootKey)}' tree");
            if(string.IsNullOrWhiteSpace(rootKey))
                throw new ArgumentNullException(nameof(rootKey), $"Root key is null. Cannot migrate node {node.Expression.Name}");
            var existingRoot = _rootKeys.FirstOrDefault(x => x == ExtractRootKey(rootKey));
        }
    }
}
