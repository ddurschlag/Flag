using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flag.Compile.Javascript
{
    using Parse;
    using Parse.Instructions;
    using Parse.Structures;
    public class TemplateCompiler : ITemplateCompiler
    {
        public TemplateCompiler(string @namespace)
            : this(@namespace, "Templates")
        { }

        public TemplateCompiler(string @namespace, string className)
        {
            Namespace = @namespace;
            ClassName = className;
        }

        public void Compile(string Name, string text, TextWriter writer)
        {
            Compile(Name, Load(text), writer);
        }

        public void Compile(string name, IEnumerable<Instruction> instructions, TextWriter writer)
        {
            var ic = new InstructionConverter("tArg");
            var templates = new List<Flag.ClassViewModel_Templates_Call_Loop>
            {
                new Flag.ClassViewModel_Templates_Call_Loop { Item1 = name, Item2 = (Flag.InstructionsViewModel)instructions.Select(ic.Visit).ToList() }
            };

            var spaces = new List<Flag.EnsureViewModel_Loop>();
            var prefix = new List<string>();
            foreach (var space in Namespace.Split('.'))
            {
                spaces.Add(new Flag.EnsureViewModel_Loop { Name = space, Prefix = ContentOrNull(string.Join(".", prefix)) });
                prefix.Add(space);
            }

            Flag.Ensure(spaces, writer);

            Flag.Class(
                new Flag.ClassViewModel()
                {
                    Name = ClassName,
                    Namespace = Namespace,
                    Templates = templates
                },
                writer
            );
        }

        private static string ContentOrNull(string s)
        {
            if (!string.IsNullOrEmpty(s)) return s;
            return null;
        }

        private string Namespace;
        private string ClassName;

        private static IEnumerable<Instruction> Load(string s)
        {
            return new Parser().Parse(new Structurizer().Structurize(new Tokenizer().Tokenize(s)));
        }

        private class InstructionConverter : InstructionVisitor<Flag.InstructionsViewModel_Loop>
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }
            private static string GetLoopVar() { return "loopIndex_" + (i++); }
            private static string GetPropertyExpression(string var, string prop) { return string.Format("{0}[\"{1}\"]", var, prop); }

            public InstructionConverter(string contextVariable)
            {
                ContextVariable = contextVariable;
            }

            private string ContextVariable;

            public override Flag.InstructionsViewModel_Loop Visit(LoopInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Loop = new Flag.LoopViewModel
                    {
                        Name = i.Name,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(CallInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Call = new Flag.CallViewModel
                    {
                        Name = i.Name,
                        ChildVariable = GetPropertyExpression(ContextVariable, i.Key)
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(CallInlineInstruction i)
            {
                var childVariable = GetPropertyExpression(ContextVariable, i.Key);
                return new Flag.InstructionsViewModel_Loop
                {
                    CallInline = new Flag.CallInlineViewModel
                    {
                        ChildVariable = childVariable,
                        Template = (Flag.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList()
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(LoopInlineInstruction i)
            {
                var childVariable = GetVar();
                return new Flag.InstructionsViewModel_Loop
                {
                    LoopInline = new Flag.LoopInlineViewModel
                    {
                        Template = (Flag.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList(),
                        ChildVariable = childVariable,
                        ContextVariable = ContextVariable,
                        LoopVariable = GetLoopVar()
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(RenderInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Render = ContextVariable
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(OutputInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Output = i.Text.Replace("\"", "\"\"")
                };
            }
        }

    }
}
