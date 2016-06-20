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
    public class MapInterpreter : DataInterpreter<MapElement>
    {
        public MapInterpreter(MapElement e, TextWriter writer, ElementInterpreterFactory eiFactory, Func<string, IEnumerable<Instruction>> templateLookup) : base(e, writer, eiFactory, templateLookup)
        {
        }

        public override void Visit(CallInstruction i)
        {
            Call(i.Key, Lookup(i.Name));
        }

        public override void Visit(CallInlineInstruction i)
        {
            Call(i.Key, i.Instructions);
        }

        private void Call(string key, IEnumerable<Instruction> instructions)
        {
            Data.Element child;
            if (Element.Value.TryGetValue(key, out child))
                foreach (var instruction in instructions)
                    Interpret(child, instruction);
        }
    }
}
