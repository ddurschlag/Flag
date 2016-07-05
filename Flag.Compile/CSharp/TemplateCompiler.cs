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
        {
            Text = text;
            Namespace = @namespace;
            Name = name;
        }

        public void Compile(TextWriter writer)
        {
            var ic = new InstructionConverter("tArg");
            var vc = new ViewModelConverter();

            var instructions = Load(Text);
            var templates = new List<CS.InnerType_6>
            {
                new CS.InnerType_6 { Item1 = Name, Item2 = (CS.InstructionsViewModel)instructions.Select(ic.Visit).ToList() }
            };

            var viewModels = new ViewModelTypeFactory().Manufacture(Name + "ViewModel", instructions).Select(vc.Visit).ToList();


            CS.Class(
            new CS.ClassViewModel()
            {
                Name = "Templates",
                Namespace = Namespace,
                Templates = templates,
                ViewModels = viewModels
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

        private class ViewModelConverter : ViewModelTypes.ViewModelTypeVisitor<CS.ViewModelViewModel>
        {
            public override CS.ViewModelViewModel Visit(MultiLoopViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    MultiLoop = new Templates.Templates.MultiLoopViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<CS.InnerType_29>().ToList(),
                        TypeName = m.TypeName
                    }
                };
            }

            public override CS.ViewModelViewModel Visit(ComplexViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    Complex = new CS.ComplexViewModelViewModel
                    {
                        EnumerableTypeNames = m.EnumerableTypeNames.Cast<CS.InnerType_11>().ToList(),
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new CS.InnerType_13 { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                };
            }

            public override CS.ViewModelViewModel Visit(PurePropertyViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    PureProperty = new CS.PurePropertyViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        PropertyTypePairs = m.PropertyTypePairs.Select(ptp => new CS.InnerType_33 { Name = ptp.Name, Type = ptp.Type }).ToList()
                    }
                };
            }

            public override CS.ViewModelViewModel Visit(ListViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    List = new CS.ListViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        EnumerableTypeName = m.EnumerableTypeName
                    }
                };
            }

            public override CS.ViewModelViewModel Visit(StringViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    String = (CS.TypeName_InnerType_37)m.TypeName
                };
            }

            public override CS.ViewModelViewModel Visit(LabelViewModel m)
            {
                return new CS.ViewModelViewModel
                {
                    Label = new Templates.Templates.LabelViewModelViewModel
                    {
                        TypeName = m.TypeName,
                        Property = new Templates.Templates.Property_InnerType_18 { Name = m.Property.Name, Type = m.Property.Type }
                    }
                };
            }
        }

        private class InstructionConverter : InstructionVisitor<CS.InnerType_16>
        {
            static int i = 0;

            private static string GetVar() { return "anon_" + (i++); }

            public InstructionConverter(string contextVariable)
            {
                ContextVariable = contextVariable;
            }

            private string ContextVariable;

            public override CS.InnerType_16 Visit(LoopInstruction i)
            {
                return new CS.InnerType_16
                {
                    Loop = new CS.LoopViewModel
                    {
                        Name = i.Name,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override CS.InnerType_16 Visit(CallInstruction i)
            {
                return new CS.InnerType_16
                {
                    Call = new CS.CallViewModel
                    {
                        Name = i.Name,
                        ChildVariable = string.Format("{0}.{1}", ContextVariable, i.Key)
                    }
                };
            }

            public override CS.InnerType_16 Visit(CallInlineInstruction i)
            {
                var childVariable = string.Format("{0}.{1}", ContextVariable, i.Key);
                return new CS.InnerType_16
                {
                    CallInline = new CS.CallInlineViewModel
                    {
                        ChildVariable = childVariable,
                        Template = (CS.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList()
                    }
                };
            }

            public override CS.InnerType_16 Visit(LoopInlineInstruction i)
            {
                var childVariable = GetVar();
                return new CS.InnerType_16
                {
                    LoopInline = new CS.LoopInlineViewModel
                    {
                        Template = (CS.InstructionsViewModel)i.Instructions.Select(new InstructionConverter(childVariable).Visit).ToList(),
                        ChildVariable = childVariable,
                        ContextVariable = ContextVariable
                    }
                };
            }

            public override CS.InnerType_16 Visit(RenderInstruction i)
            {
                return new CS.InnerType_16
                {
                    Render = (CS.ContextVariable_InnerType_36)ContextVariable
                };
            }

            public override CS.InnerType_16 Visit(OutputInstruction i)
            {
                return new CS.InnerType_16
                {
                    Output = (CS.Text_InnerType_30)i.Text.Replace("\"", "\"\"")
                };
            }
        }

    }
}
