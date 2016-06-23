using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public abstract class ViewModelType
    {
        public abstract string TypeName { get; }
        public abstract void Accept(ViewModelTypeVisitor v);
        public abstract T Accept<T>(ViewModelTypeVisitor<T> v);
    }
}
