using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Tokens
{
    [Serializable]
    public class StringToken : Token
    {
        public StringToken(string text) { Text = text; }
        public string Text { get; private set; }
        internal override void Accept(TokenVisitor v)
        {
            v.Visit(this);
        }

        internal override T Accept<T>(TokenVisitor<T> v)
        {
            return v.Visit(this);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
