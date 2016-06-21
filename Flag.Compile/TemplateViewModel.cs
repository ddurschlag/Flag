using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile
{
    using Flag.Parse.Instructions;
    public class TemplateViewModel
    {
        public TemplateViewModel(IEnumerable<Instruction> template, string contextVariable)
        {
            Instructions = template.Select(i => new InstructionViewModel(i, contextVariable)).ToArray();
        }

        public InstructionViewModel[] Instructions { get; set; }
    }
}
