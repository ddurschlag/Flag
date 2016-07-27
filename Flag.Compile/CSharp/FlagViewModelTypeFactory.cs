using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using ViewModelTypes;
    using Parse.Instructions;
    using System.Collections;

    public class FlagViewModelTypeFactory
    {
        public ViewModelType Manufacture(string name, IEnumerable<Instruction> instructions)
        {
            var analysis = new ViewModelAnalysis(this, name);
            foreach (var instruction in instructions)
                analysis.Visit(instruction);

            if (analysis.RenderP)
            {
                if (analysis.LoopCount == 0 && !analysis.PropertyP)
                    return new StringViewModel(name, analysis.NestedTypes);
                else
                    throw new Exception("Rendering is not compatible with other operations");
            }
            if (!analysis.PropertyP && analysis.LoopCount == 1)
            {
                return new ListViewModel(name, analysis.LoopTypes.Single(), analysis.NestedTypes);
            }

            if (analysis.LoopCount == 0 && analysis.PropertyP)
            {
                if (analysis.Properties.Count == 1)
                    return new LabelViewModel(name, analysis.Properties.Select(CreatePropertyInfo).Single(), analysis.NestedTypes);
                else
                    return new PurePropertyViewModel(name, analysis.Properties.Select(CreatePropertyInfo), analysis.NestedTypes);
            }

            if (analysis.LoopCount > 1 && !analysis.PropertyP)
            {
                return new MultiLoopViewModel(name, analysis.LoopTypes, analysis.NestedTypes);
            }

            if (analysis.LoopCount > 0 && analysis.PropertyP)
                return new ComplexViewModel(name, analysis.Properties.Select(CreatePropertyInfo), analysis.LoopTypes, analysis.NestedTypes);

            if (analysis.LoopCount == 0 && !analysis.PropertyP && !analysis.RenderP)
                return new EmptyViewModel(name, analysis.NestedTypes);

            throw new Exception("Unknown template pattern.");
        }

        private static PropertyInfo CreatePropertyInfo(KeyValuePair<string, string> kvp)
        {
            return new PropertyInfo(kvp.Value, kvp.Key);
        }

        private class ViewModelAnalysis : InstructionVisitor
        {
            public ViewModelAnalysis(FlagViewModelTypeFactory parent, string name)
            {
                Parent = parent;
                Name = name;
            }

            private string Name;
            private FlagViewModelTypeFactory Parent;
            private List<string> _NamedLoopTypes = new List<string>();
            private string InlineLoopType = null;
            private Dictionary<string, string> MapTypes = new Dictionary<string, string>();
            private SubTypeProcessor Processor = new SubTypeProcessor();

            public int LoopCount { get { return _NamedLoopTypes.Count + (InlineLoopType != null ? 1 : 0); } }
            public IEnumerable<string> LoopTypes
            {
                get
                {
                    IEnumerable<string> result = _NamedLoopTypes;
                    if (InlineLoopType != null)
                        result = result.Concat(new[] { InlineLoopType });
                    return result;
                }
            }
            public IEnumerable<ViewModelType> NestedTypes { get { return Processor; } }

            public bool RenderP { get; private set; }

            public bool PropertyP { get { return MapTypes.Any(); } }

            public IReadOnlyDictionary<string, string> Properties
            {
                get
                {
                    return MapTypes;
                }
            }

            public override void Visit(OutputInstruction i)
            {
                //Output requires no data
            }

            public override void Visit(RenderInstruction i)
            {
                RenderP = true;
            }

            public override void Visit(LoopInstruction i)
            {
                _NamedLoopTypes.Add(i.Name + "ViewModel");
            }

            public override void Visit(LoopInlineInstruction i)
            {
                if (InlineLoopType != null)
                {
                    Console.WriteLine("Assuming inline loops have same structure");
                }
                else
                {
                    InlineLoopType = ManufactureInnerType(Name + "_Loop", i.Instructions);
                }
            }

            private string ManufactureInnerType(string name, IEnumerable<Instruction> instructions)
            {
                return Processor.Visit(Parent.Manufacture(name, instructions));
            }

            private class SubTypeProcessor : ViewModelTypeVisitor<string>, IEnumerable<ViewModelType>
            {
                public SubTypeProcessor()
                {
                    InnerTypeList = new List<ViewModelType>();
                }

                private List<ViewModelType> InnerTypeList;

                public override string Visit(StringViewModel m)
                {
                    InnerTypeList.AddRange(m.InnerTypes);
                    return "string";
                }

                public override string Visit(ListViewModel m)
                {
                    InnerTypeList.AddRange(m.InnerTypes);
                    return string.Format("List<{0}>", m.EnumerableTypeName);
                }

                public override string Visit(MultiLoopViewModel m)
                {
                    return Process(m);
                }

                public override string Visit(PurePropertyViewModel m)
                {
                    return Process(m);
                }

                public override string Visit(ComplexViewModel m)
                {
                    return Process(m);
                }

                public override string Visit(LabelViewModel m)
                {
                    return Process(m);
                }

                public override string Visit(EmptyViewModel m)
                {
                    return Process(m);
                }

                private string Process(ViewModelType m)
                {
                    InnerTypeList.Add(m);
                    return m.TypeName;
                }

                public IEnumerator<ViewModelType> GetEnumerator()
                {
                    return InnerTypeList.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            public override void Visit(CallInstruction i)
            {
                if (MapTypes.ContainsKey(i.Key))
                {
                    Console.WriteLine("Assuming named call has same structure as existing call");
                }
                else
                {
                    MapTypes.Add(i.Key, i.Name + "ViewModel");
                }
            }

            public override void Visit(CallInlineInstruction i)
            {
                if (MapTypes.ContainsKey(i.Key))
                {
                    Console.WriteLine("Assuming inline call has same structure as existing call");
                }
                else
                {
                    MapTypes.Add(i.Key, ManufactureInnerType(Name + "_" + i.Key + "_Call", i.Instructions));
                }
            }
        }
    }
}
