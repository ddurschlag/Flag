using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class LabelViewModel : ViewModelType
    {
        public LabelViewModel(string typeName, PropertyInfo property, IEnumerable<ViewModelType> innerTypes)
        :base(typeName, innerTypes)
        {
            Property = property;
        }

        public PropertyInfo Property { get; private set; }

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
