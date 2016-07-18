using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flag.Compile.CSharp
{
    using ViewModelTypes;
    using Parse;
    using Parse.Instructions;
    using Parse.Structures;
    public class TemplateCompiler
    {
        public TemplateCompiler(string text, string @namespace, string name)
        : this(text, @namespace, name, "Templates")
        { }

        public TemplateCompiler(string text, string @namespace, string name, string className)
        : this(Load(text), @namespace, name, className)
        {
        }

        internal TemplateCompiler(IEnumerable<Instruction> instructions, string @namespace, string name, string className)
        {
            Instructions = instructions;
            Namespace = @namespace;
            Name = name;
            ClassName = className;
        }

        public void Compile(TextWriter writer)
        {
            var ic = new InstructionConverter("tArg");
            var templates = new List<Flag.ClassViewModel_Templates_Call_Loop>
            {
                new Flag.ClassViewModel_Templates_Call_Loop { Item1 = Name, Item2 = (Flag.InstructionsViewModel)Instructions.Select(ic.Visit).ToList() }
            };

            var viewModels = new ViewModelConverter().Visit(new ViewModelTypeFactory().Manufacture(Name + "ViewModel", Instructions)).ToList();


            Flag.Class(
            new Flag.ClassViewModel()
            {
                Name = ClassName,
                Namespace = Namespace,
                Templates = templates,
                ViewModels = viewModels
            },
                writer
            );
        }

        private IEnumerable<Instruction> Instructions;
        private string Name;
        private string Namespace;
        private string ClassName;

        private static IEnumerable<Instruction> Load(string s)
        {
            return new Parser().Parse(new Structurizer().Structurize(new Tokenizer().Tokenize(s)));
        }

        private class ViewModelConverter : ViewModelTypes.ViewModelTypeVisitor<IEnumerable<Flag.ViewModelViewModel>>
        {
            private IEnumerable<Flag.ViewModelViewModel> Recurse(ViewModelType m, Flag.ViewModelViewModel result)
            {
                return m.InnerTypes.SelectMany(Visit).Concat(new[] { result });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(MultiLoopViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    MultiLoop = new Flag.MultiLoopViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<Flag.MultiLoopViewModelViewModel_EnumerableTypeNames_Call_Loop>().ToList(),
                        TypeName = m.TypeName
                    }
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(ComplexViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    Complex = new Flag.ComplexViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<Flag.ComplexViewModelViewModel_EnumerableTypeNames_Call_Loop>().ToList(),
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new Flag.ComplexViewModelViewModel_PropertyTypePairs_Call_Loop { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(PurePropertyViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    PureProperty = new Flag.PurePropertyViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new Flag.PurePropertyViewModelViewModel_PropertyTypePairs_Call_Loop { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(ListViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    List = new Flag.ListViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        EnumerableTypeName = m.EnumerableTypeName
                    }
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(StringViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    String = (Flag.StringViewModelViewModel_TypeName_Call)m.TypeName
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(LabelViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    Label = new Flag.LabelViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        Property = new Flag.LabelViewModelViewModel_Property_Call { Name = m.Property.Name, Type = m.Property.Type }
                    }
                });
            }

            public override IEnumerable<Flag.ViewModelViewModel> Visit(EmptyViewModel m)
            {
                return Recurse(m, new Flag.ViewModelViewModel
                {
                    Empty = (Flag.EmptyViewModelViewModel_TypeName_Call)m.TypeName
                });
            }
        }

        private class InstructionConverter : InstructionVisitor<Flag.InstructionsViewModel_Loop>
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }

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
                        ChildVariable = string.Format("{0}.{1}", ContextVariable, i.Key)
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(CallInlineInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
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
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(RenderInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Render = (Flag.RenderViewModel_ContextVariable_Call)ContextVariable
                };
            }

            public override Flag.InstructionsViewModel_Loop Visit(OutputInstruction i)
            {
                return new Flag.InstructionsViewModel_Loop
                {
                    Output = (Flag.OutputViewModel_Text_Call)i.Text.Replace("\"", "\"\"")
                };
            }
        }

    }
}
