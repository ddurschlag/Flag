using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public abstract class ViewModelType
    {
        protected ViewModelType(string typeName, IEnumerable<ViewModelType> innerTypes)
        {
            TypeName = typeName;
            InnerTypes = innerTypes.ToArray();
        }

        public string TypeName { get; private set; }

        public IEnumerable<ViewModelType> InnerTypes { get; private set; }

        public abstract void Accept(ViewModelTypeVisitor v);
        public abstract T Accept<T>(ViewModelTypeVisitor<T> v);
    }
}
