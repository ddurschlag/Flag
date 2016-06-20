using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Flag.Interpret
{
    using Data;
    using Parse.Instructions;
    public class ListInterpreter : DataInterpreter<ListElement>
    {
        public ListInterpreter(ListElement e, TextWriter writer, ElementInterpreterFactory eiFactory, Func<string, IEnumerable<Instruction>> templateLookup) : base(e, writer, eiFactory, templateLookup)
        {
        }

        public override void Visit(LoopInlineInstruction i)
        {
            Loop(i.Instructions);
        }

        public override void Visit(LoopInstruction i)
        {
            Loop(Lookup(i.Name));
        }

        private void Loop(IEnumerable<Instruction> instructions)
        {
            foreach (var childElement in Element.Value)
            {
                foreach (var childInstruction in instructions)
                {
                    Interpret(childElement, childInstruction);
                }
            }
        }
    }
}
