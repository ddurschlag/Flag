using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public class ListViewModel : ViewModelType
    {
        public ListViewModel(string typeName, string enumerableTypeName)
        {
            _TypeName = typeName;
            EnumerableTypeName = enumerableTypeName;
        }

        private string _TypeName;
        public string EnumerableTypeName { get; private set; }

        public override string TypeName
        {
            get { return _TypeName; }
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
