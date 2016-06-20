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
        public TypeViewModel(string name, IEnumerable<Tuple<string, IEnumerable<Instruction>>> templates)
        {
            Name = name;
            Templates = templates;
        }

        public string Name { get; private set; }
        private IEnumerable<Tuple<string, IEnumerable<Instruction>>> Templates;
    }
}
