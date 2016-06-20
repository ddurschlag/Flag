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
    public class StringInterpreter : DataInterpreter<StringElement>
    {
        public StringInterpreter(StringElement e, TextWriter writer, ElementInterpreterFactory eiFactory, Func<string, IEnumerable<Instruction>> templateLookup) : base(e, writer, eiFactory, templateLookup)
        {
        }

        public override void Visit(RenderInstruction i)
        {
            Write(Element.Value);
        }
    }
}
