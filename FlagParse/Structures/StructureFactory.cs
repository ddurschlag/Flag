using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    using Tokens;

    internal abstract class StructureFactory<TState, TResult> : TokenVisitor<IEnumerable<TResult>>
    {
        public StructureFactory(TState s)
        {
            State = s;
        }

        protected TState State { get; private set; }

        public override IEnumerable<TResult> Visit(PoleToken t)
        {
            Fail(t);
            yield break;
        }

        public override IEnumerable<TResult> Visit(EndToken t)
        {
            Fail(t);
            yield break;
        }

        public override IEnumerable<TResult> Visit(FlagToken t)
        {
            Fail(t);
            yield break;
        }

        public override IEnumerable<TResult> Visit(StringToken t)
        {
            Fail(t);
            yield break;
        }

        protected void Fail(Token t)
        {
            Fail("Unexpected token type", t);
        }

        protected void Fail(string message, Token t)
        {
            var error = new Exception(message);
            error.Data.Add("Token", t);
            error.Data.Add("Context", State);
            throw error;
        }
    }
}
