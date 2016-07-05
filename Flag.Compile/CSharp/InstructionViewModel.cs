using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using Flag.Parse.Instructions;
    public class InstructionViewModel
    {
        public InstructionViewModel(Instruction instruction, string contextVariable)
        {
            new Assigner(this, contextVariable).Visit(instruction);
        }

        public LoopViewModel Loop { get; private set; }
        public CallViewModel Call { get; private set; }
        public CallInlineViewModel CallInline { get; private set; }
        public LoopInlineViewModel LoopInline { get; private set; }
        public RenderViewModel Render { get; private set; }
        public OutputViewModel Output { get; private set; }

        private class Assigner : InstructionVisitor
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }

            public Assigner(InstructionViewModel me, string contextVariable)
            {
                Me = me;
                ContextVariable = contextVariable;
            }

            private InstructionViewModel Me;
            private string ContextVariable;

            public override void Visit(LoopInstruction i)
            {
                Me.Loop = new LoopViewModel { Name = i.Name, ContextVariable = ContextVariable };
            }
            public override void Visit(LoopInlineInstruction i)
            {
                var childVariable = GetVar();
                Me.LoopInline = new LoopInlineViewModel { Template = new TemplateViewModel(i.Instructions, childVariable), ChildVariable = childVariable, ContextVariable = ContextVariable };
            }

            public override void Visit(CallInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
                Me.Call = new CallViewModel { Name = i.Name, ChildVariable = childVariable };
            }

            public override void Visit(CallInlineInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
                Me.CallInline = new CallInlineViewModel { Template = new TemplateViewModel(i.Instructions, childVariable), ChildVariable = childVariable };
            }


            public override void Visit(RenderInstruction i)
            {
                Me.Render = new RenderViewModel { ContextVariable = ContextVariable };
            }

            public override void Visit(OutputInstruction i)
            {
                Me.Output = new OutputViewModel { Text = i.Text.Replace("\"", "\"\"") };
            }
        }

        public class LoopViewModel { public string Name { get; set; } public string ContextVariable { get; set; } }
        public class LoopInlineViewModel { public TemplateViewModel Template { get; set; } public string ChildVariable { get; set; } public string ContextVariable { get; set; } }
        public class CallViewModel { public string ChildVariable { get; set; } public string Name { get; set; } }
        public class CallInlineViewModel { public string ChildVariable { get; set; } public TemplateViewModel Template { get; set; } }
        public class RenderViewModel { public string ContextVariable { get; set; } }
        public class OutputViewModel { public string Text { get; set; } }

    }
}
