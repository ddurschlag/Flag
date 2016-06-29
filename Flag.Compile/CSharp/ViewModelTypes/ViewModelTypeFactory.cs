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
            var analysis = new ViewModelAnalysis(this);
            foreach (var instruction in instructions)
                analysis.Visit(instruction);

            if (analysis.RenderP)
            {
                if (analysis.LoopCount == 0 && !analysis.PropertyP)
                    return new StringViewModel(name);
                else
                    throw new Exception("Rendering is not compatible with other operations");
            }
            if (!analysis.PropertyP && analysis.LoopCount == 1)
            {
                return new ListViewModel(name, analysis.LoopTypes.Single());
            }

            if (analysis.LoopCount == 0 && analysis.PropertyP)
            {
                return new PurePropertyViewModel(name, analysis.Properties.Select(CreatePropertyInfo));
            }

            if (analysis.LoopCount > 1 && !analysis.PropertyP)
            {
                return new MultiLoopViewModel(name, analysis.LoopTypes);
            }

            return new ComplexViewModel(name, analysis.Properties.Select(CreatePropertyInfo), analysis.LoopTypes);
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
            int i = 0;

            private string GetInnerTypeName() { return "InnerType_" + (i++); }

            public int LoopCount { get { return _LoopTypes.Count; } }
            public IEnumerable<string> LoopTypes { get { return _LoopTypes; } }

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
                _LoopTypes.Add(i.Name);
            }

            public override void Visit(LoopInlineInstruction i)
            {
                _LoopTypes.Add(ManufactureInnerType(i.Instructions));
            }

            private string ManufactureInnerType(IEnumerable<Instruction> instructions)
            {
                var loopType = Parent.Manufacture(GetInnerTypeName(), instructions);
                InnerTypes.Add(loopType);
                return loopType.TypeName;
            }

            public override void Visit(CallInstruction i)
            {
                MapTypes.Add(i.Key, i.Name);
            }

            public override void Visit(CallInlineInstruction i)
            {
                MapTypes.Add(i.Key, ManufactureInnerType(i.Instructions));
            }
        }
    }
}
