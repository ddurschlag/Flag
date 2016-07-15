using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp.ViewModelTypes
{
    public abstract class ViewModelTypeVisitor
    {
        public void Visit(ViewModelType a)
        {
            a.Accept(this);
        }

        public abstract void Visit(StringViewModel m);
        public abstract void Visit(ListViewModel m);
        public abstract void Visit(MultiLoopViewModel m);
        public abstract void Visit(PurePropertyViewModel m);
        public abstract void Visit(ComplexViewModel m);
        public abstract void Visit(LabelViewModel m);
        public abstract void Visit(EmptyViewModel m);
    }

    public abstract class ViewModelTypeVisitor<T>
    {
        public T Visit(ViewModelType a)
        {
            return a.Accept(this);
        }

        public abstract T Visit(StringViewModel m);
        public abstract T Visit(ListViewModel m);
        public abstract T Visit(MultiLoopViewModel m);
        public abstract T Visit(PurePropertyViewModel m);
        public abstract T Visit(ComplexViewModel m);
        public abstract T Visit(LabelViewModel m);
        public abstract T Visit(EmptyViewModel m);
    }
}
