using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    using Flag.Parse.Instructions;
    public class ViewModelTypeFactory
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

            throw new Exception("Unknown template pattern.");
        }

        private static PropertyInfo CreatePropertyInfo(KeyValuePair<string, string> kvp)
        {
            return new PropertyInfo(kvp.Value, kvp.Key);
        }

        private class ViewModelAnalysis : InstructionVisitor
        {
            public ViewModelAnalysis(ViewModelTypeFactory parent, string name)
            {
                Parent = parent;
                Name = name;
            }

            private string Name;
            private ViewModelTypeFactory Parent;
            private List<string> _NamedLoopTypes = new List<string>();
            private ViewModelType InlineLoopType = null;
            private Dictionary<string, string> MapTypes = new Dictionary<string, string>();
            private List<ViewModelType> InnerTypes = new List<ViewModelType>();

            public int LoopCount { get { return _NamedLoopTypes.Count + (InlineLoopType != null ? 1 : 0); } }
            public IEnumerable<string> LoopTypes
            {
                get
                {
                    IEnumerable<string> result = _NamedLoopTypes;
                    if (InlineLoopType != null)
                        result = result.Concat(new[] { InlineLoopType.TypeName });
                    return result;
                }
            }
            public IEnumerable<ViewModelType> NestedTypes { get { return InnerTypes; } }

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

            private ViewModelType ManufactureInnerType(string name, IEnumerable<Instruction> instructions)
            {
                var innerType = Parent.Manufacture(name, instructions);
                InnerTypes.Add(innerType);
                return innerType; //This is a hack. Should really return the requested type in a more "privileged" position
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
                    MapTypes.Add(i.Key, ManufactureInnerType(Name + "_" + i.Key + "_Call", i.Instructions).TypeName);
                }
            }
        }
    }
}
