using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Interpret.Data
{
    public abstract class ElementVisitor
    {
        public void Visit(Element e)
        {
            e.Accept(this);
        }

        public abstract void Visit(StringElement e);
        public abstract void Visit(ListElement e);
        public abstract void Visit(MapElement e);
    }

    public abstract class ElementVisitor<T>
    {
        public T Visit(Element e)
        {
            return e.Accept(this);
        }

        public abstract T Visit(StringElement e);
        public abstract T Visit(ListElement e);
        public abstract T Visit(MapElement e);
    }
}
