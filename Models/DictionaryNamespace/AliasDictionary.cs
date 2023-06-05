using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models.DictionaryNamespace
{
    public partial class AliasDictionary
    {
        private readonly IDictionary<string, string> _aliases = new Dictionary<string, string>();

        //A: A  //first come first serve
        //D = A //duplicate expression evaluation detected
        //D: A  //alias A as D
        public string? this[string? name]
        {
            get => MapAlias(name);
            set {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));
                if (_aliases.ContainsKey(value) && !_aliases.ContainsKey(name))
                    _aliases[name] = value;
                else if(_aliases.ContainsKey(name))
                    throw new ArgumentException($"Invalid operation: Alias['{name}'] = '{value}', because '{name}' already exists"); 
                else
                    _aliases[name] = value;
            }
        }
        private string? MapAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return null;
            if (_aliases.ContainsKey(alias))
                return _aliases[alias];
            return alias;
        }
        public bool IsMapped(string alias) => !string.IsNullOrWhiteSpace(alias) && alias != MapAlias(alias);
        public void AddAlias(string? original, string? alias) => this[alias] = original;
        public bool TryAddAlias(string? original, string? alias)
        {
            try
            {
                this[alias] = original;
                return true;
            }
            catch(Exception) { return false; }
        }
        public void RemoveAlias(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias) && _aliases.ContainsKey(alias))
                _aliases.Remove(alias);
        }
        public bool ContainsAlias(string? name) => _aliases.ContainsKey(name);
        public IEnumerable<KeyValuePair<string, string>> AsEnumerable() => _aliases;
    }
}
