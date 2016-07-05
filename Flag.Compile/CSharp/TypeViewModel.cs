using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using Flag.Parse.Instructions;
    using ViewModelTypes;

    public class TypeViewModel
    {
        public TypeViewModel(string name, string @namespace, IEnumerable<Tuple<string, IEnumerable<Instruction>>> templates)
        {
            var factory = new ViewModelTypeFactory();

            Name = name;
            Namespace = @namespace;
            Templates = templates.Select(t => Tuple.Create(t.Item1, new TemplateViewModel(t.Item2, "tArg")));
            ViewModels = templates.SelectMany(t => factory.Manufacture(t.Item1 + "ViewModel", t.Item2)).GroupBy(vmt=>vmt.TypeName).Select(g=>new ViewModelViewModel(g.First())).ToArray();
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public IEnumerable<Tuple<string, TemplateViewModel>> Templates;
        public IEnumerable<ViewModelViewModel> ViewModels { get; private set; }
    }
}
