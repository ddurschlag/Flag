using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class PurePropertyViewModel : ViewModelType
    {
        public PurePropertyViewModel(string typeName, IEnumerable<PropertyInfo> propertyTypePairs, IEnumerable<ViewModelType> innerTypes)
        : base(typeName, innerTypes)
        {
            PropertyTypePairs = propertyTypePairs.ToArray();
        }

        public IEnumerable<PropertyInfo> PropertyTypePairs { get; private set; }

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
