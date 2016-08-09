using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    [Serializable]
    public abstract class Structure
    {
        public abstract void Accept(StructureVisitor v);
        public abstract T Accept<T>(StructureVisitor<T> v);
    }
}
