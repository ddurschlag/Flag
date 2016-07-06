using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flag.Parse.Instructions;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class MultiLoopViewModel : ViewModelType
    {
        public MultiLoopViewModel(string typeName, IEnumerable<string> enumerableTypeNames, IEnumerable<ViewModelType> innerTypes)
        : base(typeName, innerTypes)
        {
            EnumerableTypeNames = enumerableTypeNames.ToArray();
        }

        public IEnumerable<string> EnumerableTypeNames { get; private set; }

        public override void Accept(ViewModelTypeVisitor v)
        {
            v.Visit(this);
        }

        public override T Accept<T>(ViewModelTypeVisitor<T> v)
        {
            return v.Visit(this);
        }
    }
}
