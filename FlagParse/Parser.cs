using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse
{
    using Instructions;
    using Structures;

    public class Parser : StructureVisitor<Instruction>
    {
        public IEnumerable<Instruction> Parse(IEnumerable<Structure> input)
        {
            return input.Select(Visit);
        }

        public override Instruction Visit(CommandStructure s)
        {
            bool keyp = !string.IsNullOrEmpty(s.Key);
            bool inlinep = s.Inline.Any();
            bool namep = !string.IsNullOrEmpty(s.Name);
            if (!keyp && !inlinep && !namep)
                return new RenderInstruction();
            if (!inlinep && !namep)
            {
                var error = new Exception("Missing template");
                error.Data.Add("Structure", s);
                throw error;
            }
            if (inlinep && namep)
            {
                var error = new Exception("Ambiguous template");
                error.Data.Add("Structure", s);
                throw error;
            }
            if (inlinep)
                if (keyp)
                    return new CallInlineInstruction(s.Key, s.Inline.Select(Visit));
                else
                    return new LoopInlineInstruction(s.Inline.Select(Visit));
            else {
                if (keyp)
                    return new CallInstruction(s.Name, s.Key);
                else
                    return new LoopInstruction(s.Name);
            }

        }

        public override Instruction Visit(OutputStructure s)
        {
            return new OutputInstruction(s.Text);
        }
    }
}
