using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public abstract class Instruction
    {
        internal abstract void Accept(InstructionVisitor v);
        internal abstract T Accept<T>(InstructionVisitor<T> v);
    }
}
