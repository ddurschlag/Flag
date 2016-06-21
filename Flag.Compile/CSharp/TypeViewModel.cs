using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using Flag.Parse.Instructions;

    public class TypeViewModel
    {
        public TypeViewModel(string name, string @namespace, IEnumerable<Tuple<string, IEnumerable<Instruction>>> templates)
        {
            Name = name;
            Namespace = @namespace;
            Templates = templates.Select(t => Tuple.Create(t.Item1, new TemplateViewModel(t.Item2, "tArg")));
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public IEnumerable<Tuple<string, TemplateViewModel>> Templates;
    }
}
