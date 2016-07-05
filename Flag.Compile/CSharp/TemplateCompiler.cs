using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flag.Compile.CSharp
{
    using Parse;
    using Parse.Instructions;
    using Parse.Structures;
    using Templates;
    public class TemplateCompiler
    {
        public TemplateCompiler(string text, string @namespace, string name)
        {
            Text = text;
            Namespace = @namespace;
            Name = name;
        }

        public void Compile(TextWriter writer)
        {
            var ic = new InstructionConverter("tArg");
            Templates.Templates.Class(
            new Templates.Templates.ClassViewModel()
                {
                    Name = "Templates",
                    Namespace = Namespace,
                    Templates = { new Templates.Templates.InnerType_6() {
                        Item1 = Name,
                        Item2 = new Templates.Templates.TemplateViewModel() { Instructions = Load(Text).Select(ic.Visit).ToList() }
                    } }
                },
                writer
            );
        }

        private string Text;
        private string Name;
        private string Namespace;

        private static IEnumerable<Instruction> Load(string s)
        {
            return new Parser().Parse(new Structurizer().Structurize(new Tokenizer().Tokenize(s)));
        }

        private class InstructionConverter : InstructionVisitor<Templates.Templates.InnerType_16>
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }

            public InstructionConverter(string contextVariable)
            {
                ContextVariable = contextVariable;
            }

            private string ContextVariable;

            public override Templates.Templates.InnerType_16 Visit(LoopInstruction i)
            {
                return new Templates.Templates.InnerType_16
                {
                    Loop = new Templates.Templates.LoopViewModel
                    {
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override Templates.Templates.InnerType_16 Visit(CallInstruction i)
            {
                return new Templates.Templates.InnerType_16
                {
                    Call = new Templates.Templates.CallViewModel
                    {
                        Name = i.Name,
                        ChildVariable = string.Format("{0}.{1}", ContextVariable, i.Key)
                    }
                };
            }

            public override Templates.Templates.InnerType_16 Visit(CallInlineInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
                return new Templates.Templates.InnerType_16
                {
                    CallInline = new Templates.Templates.CallInlineViewModel
                    {
                        ChildVariable = childVariable,
                        Template = new Templates.Templates.TemplateViewModel
                        {
                            Instructions = i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList()
                        }
                    }
                };
            }

            public override Templates.Templates.InnerType_16 Visit(LoopInlineInstruction i)
            {
                var childVariable = GetVar();
                return new Templates.Templates.InnerType_16
                {
                    LoopInline = new Templates.Templates.LoopInlineViewModel
                    {
                        Template = new Templates.Templates.TemplateViewModel
                        {
                            Instructions = i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList()
                        },
                        ChildVariable = childVariable,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override Templates.Templates.InnerType_16 Visit(RenderInstruction i)
            {
                return new Templates.Templates.InnerType_16
                {
                    Render = new Templates.Templates.RenderViewModel
                    {
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override Templates.Templates.InnerType_16 Visit(OutputInstruction i)
            {
                return new Templates.Templates.InnerType_16
                {
                    Output = new Templates.Templates.OutputViewModel
                    {
                        Text = i.Text
                    }
                };
            }
        }

    }
}
