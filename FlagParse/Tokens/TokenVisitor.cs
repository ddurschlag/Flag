using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Tokens
{
    public abstract class TokenVisitor
    {
        public void Visit(Token t)
        {
            t.Accept(this);
        }

        public abstract void Visit(StringToken t);
        public abstract void Visit(FlagToken t);
        public abstract void Visit(PoleToken t);
        public abstract void Visit(EndToken t);
    }
    public abstract class TokenVisitor<T>
    {
        public T Visit(Token t)
        {
            return t.Accept(this);
        }

        public abstract T Visit(StringToken t);
        public abstract T Visit(FlagToken t);
        public abstract T Visit(PoleToken t);
        public abstract T Visit(EndToken t);
    }
}
