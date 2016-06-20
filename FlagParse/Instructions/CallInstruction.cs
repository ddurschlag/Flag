using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public class CallInstruction : Instruction
    {
        public CallInstruction(string name, string key)
        {
            Name = name;
            Key = key;
        }
        public string Name { get; }
        public string Key { get; }

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
            return string.Format("[CALL {0} ON {1}]", Name, Key);
        }
    }
}
