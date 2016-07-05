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
        public IEnumerable<ViewModelType> Manufacture(string name, IEnumerable<Instruction> instructions)
        {
            var analysis = new ViewModelAnalysis(this);
            foreach (var instruction in instructions)
                analysis.Visit(instruction);

            if (analysis.RenderP)
            {
                if (analysis.LoopCount == 0 && !analysis.PropertyP)
                    yield return new StringViewModel(name);
                else
                    throw new Exception("Rendering is not compatible with other operations");
            }
            if (!analysis.PropertyP && analysis.LoopCount == 1)
            {
                yield return new ListViewModel(name, analysis.LoopTypes.Single());
            }

            if (analysis.LoopCount == 0 && analysis.PropertyP)
            {
                yield return new PurePropertyViewModel(name, analysis.Properties.Select(CreatePropertyInfo));
            }

            if (analysis.LoopCount > 1 && !analysis.PropertyP)
            {
                yield return new MultiLoopViewModel(name, analysis.LoopTypes);
            }

            if ( analysis.LoopCount > 0 && analysis.PropertyP)
                yield return new ComplexViewModel(name, analysis.Properties.Select(CreatePropertyInfo), analysis.LoopTypes);

            foreach (var innerType in analysis.NestedTypes)
                yield return innerType;
        }

        private static PropertyInfo CreatePropertyInfo(KeyValuePair<string, string> kvp)
        {
            return new PropertyInfo(kvp.Value, kvp.Key);
        }

        private class ViewModelAnalysis : InstructionVisitor
        {
            public ViewModelAnalysis(ViewModelTypeFactory parent)
            {
                Parent = parent;
            }

            private ViewModelTypeFactory Parent;
            private List<string> _LoopTypes = new List<string>();
            private Dictionary<string, string> MapTypes = new Dictionary<string, string>();
            private List<ViewModelType> InnerTypes = new List<ViewModelType>();
            private static int i = 0;

            private static string GetInnerTypeName() { return "InnerType_" + (i++); }

            public int LoopCount { get { return _LoopTypes.Count; } }
            public IEnumerable<string> LoopTypes { get { return _LoopTypes; } }
            public IEnumerable<ViewModelType> NestedTypes { get { return InnerTypes; } }

            public bool RenderP { get; private set; }

            public bool PropertyP { get { return MapTypes.Any(); } }

            public IReadOnlyDictionary<string, string> Properties { get { return MapTypes; } }

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
                _LoopTypes.Add(i.Name + "ViewModel");
            }

            public override void Visit(LoopInlineInstruction i)
            {
                _LoopTypes.Add(ManufactureInnerType(GetInnerTypeName(), i.Instructions));
            }

            private string ManufactureInnerType(string name, IEnumerable<Instruction> instructions)
            {
                var innerTypes = Parent.Manufacture(name, instructions).ToArray();
                InnerTypes.AddRange(innerTypes);
                return innerTypes.First().TypeName; //This is a hack. Should really return the requested type in a more "privileged" position
            }

            public override void Visit(CallInstruction i)
            {
                MapTypes.Add(i.Key, i.Name + "ViewModel");
            }

            public override void Visit(CallInlineInstruction i)
            {
                //TODO: Different calls on same property
                if (!MapTypes.ContainsKey(i.Key))//We've already handled this property.
                    MapTypes.Add(i.Key, ManufactureInnerType(i.Key + "_" + GetInnerTypeName(), i.Instructions));
            }
        }
    }
}
