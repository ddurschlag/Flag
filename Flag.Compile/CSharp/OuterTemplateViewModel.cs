using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using Flag.Parse.Instructions;
    using Flag.Compile.CSharp.ViewModelTypes;
    public class OuterTemplateViewModel : TemplateViewModel
    {
        public OuterTemplateViewModel(IEnumerable<Instruction> template, string contextVariable, string viewModelName)
        :base(template,contextVariable)
        {
            _ViewModels = new ViewModelTypeFactory().Manufacture(viewModelName, template).Select(vmt => new ViewModelViewModel(vmt)).ToArray();
        }

        private ViewModelViewModel[] _ViewModels;
        public IEnumerable<ViewModelViewModel> ViewModels {  get { return _ViewModels; } }
    }
}

