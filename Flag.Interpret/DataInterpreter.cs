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
    public abstract class DataInterpreter<T> : InstructionVisitor
                where T : Element
    {
        public DataInterpreter(T e, TextWriter writer, ElementInterpreterFactory eiFactory, Func<string, IEnumerable<Instruction>> templateLookup)
        {
            Element = e;
            Writer = writer;
            EIFactory = eiFactory;
            TemplateLookup = templateLookup;
        }

        protected void Write(string s)
        {
            Writer.Write(s);
        }

        public override void Visit(LoopInstruction i)
        {
            Fail(i);
        }

        public override void Visit(CallInstruction i)
        {
            Fail(i);
        }

        public override void Visit(CallInlineInstruction i)
        {
            Fail(i);
        }

        public override void Visit(LoopInlineInstruction i)
        {
            Fail(i);
        }

        public override void Visit(RenderInstruction i)
        {
            Fail(i);
        }

        public override void Visit(OutputInstruction i)
        {
            Write(i.Text);
        }

        protected T Element { get; private set; }
        protected void Interpret(Element e, Instruction i)
        {
            EIFactory.Visit(e).Visit(i);
        }
        protected IEnumerable<Instruction> Lookup(string s) { return TemplateLookup(s); }
        private TextWriter Writer;
        private ElementInterpreterFactory EIFactory;
        private Func<string, IEnumerable<Instruction>> TemplateLookup;
        private void Fail(Instruction i)
        {
            var error = new Exception("Unexpected instruction");
            error.Data.Add("Element", Element);
            error.Data.Add("Instruction", i);
            throw error;
        }
    }
}
