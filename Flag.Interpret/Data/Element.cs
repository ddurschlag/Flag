using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Interpret.Data
{
    public abstract class Element
    {
        public abstract void Accept(ElementVisitor v);
        public abstract T Accept<T>(ElementVisitor<T> v);
    }
}
