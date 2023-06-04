using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FilterExpressionTreeBuildersNamespace
{
    public partial class FilterExpressionTreeBuilder
    {
        public (string Name, string Value) RegisterAlias
        {
            set 
            {
                try { _aliases[value.Name] = value.Value; } catch (Exception) { }; 
            }
        }
    }
}
