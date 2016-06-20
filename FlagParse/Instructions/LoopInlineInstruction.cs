using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public class LoopInlineInstruction : Instruction
    {
        public LoopInlineInstruction(IEnumerable<Instruction> instructions)
        {
            Instructions = instructions.ToArray();
        }

        public IEnumerable<Instruction> Instructions { get; private set; }

        internal override void Accept(InstructionVisitor v)
        {
            v.Visit(this);
        }

        internal override T Accept<T>(InstructionVisitor<T> v)
        {
            return v.Visit(this);
        }

        public override string ToString()
        {
            return string.Concat("[LOOP: ", string.Join(" ", Instructions), " ]");
        }
    }
}
