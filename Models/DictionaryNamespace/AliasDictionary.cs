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
    public partial class AliasDictionary : Dictionary<string, string>, IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _aliases = new Dictionary<string, string>();

        public AliasDictionary(StringComparer? stringComparer = null) : base(stringComparer) { }

        //A: A  //first come first serve
        //D = A //duplicate expression evaluation detected
        //D: A  //alias A as D
        public string? this[string? name]
        {
            get { return string.IsNullOrWhiteSpace(name) ? null : _aliases.ContainsKey(name) ? _aliases[name] : null; }
            set {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));
                if (_aliases.ContainsKey(name))
                    throw new ArgumentException($"'{name}' already exists as an alias '{_aliases[name]}'");
                _aliases[name] = value; 
            }
        }

        public new void Add(string key, string value) => this[key] = value;
    }
}
