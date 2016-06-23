using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class PurePropertyViewModel : ViewModelType
    {
        public PurePropertyViewModel(string typeName, IEnumerable<Tuple<string,string>> propertyTypePairs)
        {
            _TypeName = typeName;
            PropertyTypePairs = propertyTypePairs.ToArray();
        }

        private string _TypeName;
        public IEnumerable<Tuple<string, string>> PropertyTypePairs { get; private set; }

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
