using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public class CallInlineInstruction : Instruction
    {
        public CallInlineInstruction(string key, IEnumerable<Instruction> instructions)
        {
            Key = key;
            Instructions = instructions.ToArray();
        }

        public string Key { get; private set; }
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
            return string.Concat("[CALL ON ", Key, ": ", string.Join(" ", Instructions), " ]");
        }
    }
}
