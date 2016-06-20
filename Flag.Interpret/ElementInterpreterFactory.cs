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
    public class ElementInterpreterFactory : ElementVisitor<InstructionVisitor>
    {
        public ElementInterpreterFactory(System.IO.TextWriter writer, Func<string, IEnumerable<Instruction>> templateLookup)
        {
            Writer = writer;
            TemplateLookup = templateLookup;
        }

        private TextWriter Writer;
        private Func<string, IEnumerable<Instruction>> TemplateLookup;

        public override InstructionVisitor Visit(StringElement e)
        {
            return new StringInterpreter(e, Writer, this, TemplateLookup);
        }

        public override InstructionVisitor Visit(ListElement e)
        {
            return new ListInterpreter(e, Writer, this, TemplateLookup);
        }

        public override InstructionVisitor Visit(MapElement e)
        {
            return new MapInterpreter(e, Writer, this, TemplateLookup);
        }
    }
}
