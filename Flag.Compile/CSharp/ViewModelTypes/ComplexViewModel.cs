using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class ComplexViewModel : ViewModelType
    {
        public ComplexViewModel(string typeName, IEnumerable<PropertyInfo> propertyTypePairs, IEnumerable<string> enumerableTypeNames)
        {
            _TypeName = typeName;
            PropertyTypePairs = propertyTypePairs.ToArray();
            EnumerableTypeNames = enumerableTypeNames.ToArray();
        }

        private string _TypeName;
        public IEnumerable<PropertyInfo> PropertyTypePairs { get; private set; }
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
