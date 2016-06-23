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
        public MultiLoopViewModel(string typeName, IEnumerable<string> enumerableTypeNames)
        {
            _TypeName = typeName;
            EnumerableTypeNames = enumerableTypeNames.ToArray();
        }

        private string _TypeName;
        public IEnumerable<string> EnumerableTypeNames { get; private set; }

        public override string TypeName
        {
            get
            {
                return _TypeName;
            }
        }

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
