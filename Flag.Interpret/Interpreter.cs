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
    public class Interpreter
    {
        public void Interpret(TextWriter writer, Element data, IEnumerable<Instruction> template, Func<string, IEnumerable<Instruction>> templateLookup)
        {
            var e = new ElementInterpreterFactory(writer, templateLookup).Visit(data);
            foreach (var instruction in template)
                e.Visit(instruction);
        }
    }
}
