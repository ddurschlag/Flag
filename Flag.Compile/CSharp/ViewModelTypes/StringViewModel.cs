using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class StringViewModel : ViewModelType
    {
        public StringViewModel(string typeName)
        {
            _TypeName = typeName;
        }

        private string _TypeName;

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
