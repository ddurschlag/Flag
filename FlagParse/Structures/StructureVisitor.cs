using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    public abstract class StructureVisitor
    {
        public void Visit(Structure s)
        {
            s.Accept(this);
        }

        public abstract void Visit(OutputStructure s);
        public abstract void Visit(CommandStructure s);
    }

    public abstract class StructureVisitor<T>
    {
        public T Visit(Structure s)
        {
            return s.Accept(this);
        }

        public abstract T Visit(OutputStructure s);
        public abstract T Visit(CommandStructure s);
    }
}
