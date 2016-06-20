using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public abstract class InstructionVisitor
    {
        public void Visit(Instruction i) { i.Accept(this); }

        public abstract void Visit(OutputInstruction i);
        public abstract void Visit(RenderInstruction i);
        public abstract void Visit(LoopInstruction i);
        public abstract void Visit(LoopInlineInstruction i);
        public abstract void Visit(CallInstruction i);
        public abstract void Visit(CallInlineInstruction i);
    }

    public abstract class InstructionVisitor<T>
    {
        public T Visit(Instruction i) { return i.Accept(this); }

        public abstract T Visit(OutputInstruction i);
        public abstract T Visit(RenderInstruction i);
        public abstract T Visit(LoopInstruction i);
        public abstract T Visit(LoopInlineInstruction i);
        public abstract T Visit(CallInstruction i);
        public abstract T Visit(CallInlineInstruction i);
    }
}
