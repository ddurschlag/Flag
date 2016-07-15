using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flag.Compile.CSharp
{
    using Templates;
    using ViewModelTypes;
    using Parse;
    using Parse.Instructions;
    using Parse.Structures;
    using CS = Templates.Templates;
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
            var templates = new List<CS.ClassViewModel_Templates_Call_Loop>
            {
                new CS.ClassViewModel_Templates_Call_Loop { Item1 = Name, Item2 = (CS.InstructionsViewModel)Instructions.Select(ic.Visit).ToList() }
            };

            var viewModels = new ViewModelConverter().Visit(new ViewModelTypeFactory().Manufacture(Name + "ViewModel", Instructions)).ToList();


            CS.Class(
            new CS.ClassViewModel()
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

        private class ViewModelConverter : ViewModelTypes.ViewModelTypeVisitor<IEnumerable<CS.ViewModelViewModel>>
        {
            private IEnumerable<CS.ViewModelViewModel> Recurse(ViewModelType m, CS.ViewModelViewModel result)
            {
                return m.InnerTypes.SelectMany(Visit).Concat(new[] { result });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(MultiLoopViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    MultiLoop = new Templates.Templates.MultiLoopViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<CS.MultiLoopViewModelViewModel_EnumerableTypeNames_Call_Loop>().ToList(),
                        TypeName = m.TypeName
                    }
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(ComplexViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    Complex = new CS.ComplexViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<CS.ComplexViewModelViewModel_EnumerableTypeNames_Call_Loop>().ToList(),
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new CS.ComplexViewModelViewModel_PropertyTypePairs_Call_Loop { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(PurePropertyViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    PureProperty = new CS.PurePropertyViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new CS.PurePropertyViewModelViewModel_PropertyTypePairs_Call_Loop { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(ListViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    List = new CS.ListViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        EnumerableTypeName = m.EnumerableTypeName
                    }
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(StringViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    String = (CS.StringViewModelViewModel_TypeName_Call)m.TypeName
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(LabelViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    Label = new Templates.Templates.LabelViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        Property = new CS.LabelViewModelViewModel_Property_Call { Name = m.Property.Name, Type = m.Property.Type }
                    }
                });
            }

            public override IEnumerable<CS.ViewModelViewModel> Visit(EmptyViewModel m)
            {
                return Recurse(m, new CS.ViewModelViewModel
                {
                    Empty = (CS.EmptyViewModelViewModel_TypeName_Call)m.TypeName
                });
            }
        }

        private class InstructionConverter : InstructionVisitor<CS.InstructionsViewModel_Loop>
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }

            public InstructionConverter(string contextVariable)
            {
                ContextVariable = contextVariable;
            }

            private string ContextVariable;

            public override CS.InstructionsViewModel_Loop Visit(LoopInstruction i)
            {
                return new CS.InstructionsViewModel_Loop
                {
                    Loop = new CS.LoopViewModel
                    {
                        Name = i.Name,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override CS.InstructionsViewModel_Loop Visit(CallInstruction i)
            {
                return new CS.InstructionsViewModel_Loop
                {
                    Call = new CS.CallViewModel
                    {
                        Name = i.Name,
                        ChildVariable = string.Format("{0}.{1}", ContextVariable, i.Key)
                    }
                };
            }

            public override CS.InstructionsViewModel_Loop Visit(CallInlineInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
                return new CS.InstructionsViewModel_Loop
                {
                    CallInline = new CS.CallInlineViewModel
                    {
                        ChildVariable = childVariable,
                        Template = (CS.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList()
                    }
                };
            }

            public override CS.InstructionsViewModel_Loop Visit(LoopInlineInstruction i)
            {
                var childVariable = GetVar();
                return new CS.InstructionsViewModel_Loop
                {
                    LoopInline = new CS.LoopInlineViewModel
                    {
                        Template = (CS.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList(),
                        ChildVariable = childVariable,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override CS.InstructionsViewModel_Loop Visit(RenderInstruction i)
            {
                return new CS.InstructionsViewModel_Loop
                {
                    Render = (CS.RenderViewModel_ContextVariable_Call)ContextVariable
                };
            }

            public override CS.InstructionsViewModel_Loop Visit(OutputInstruction i)
            {
                return new CS.InstructionsViewModel_Loop
                {
                    Output = (CS.OutputViewModel_Text_Call)i.Text.Replace("\"", "\"\"")
                };
            }
        }

    }
}
