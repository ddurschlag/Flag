using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Tokens
{
    public abstract class Token
    {
        internal abstract void Accept(TokenVisitor v);
        internal abstract T Accept<T>(TokenVisitor<T> v);
    }
}
