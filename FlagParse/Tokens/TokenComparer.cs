using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Tokens
{
    public class TokenComparer : IEqualityComparer<Token>
    {
        public bool Equals(Token x, Token y)
        {
            return new TokenComparerFactory().Visit(x).Equals(x,y);
        }

        public int GetHashCode(Token obj)
        {
            return new TokenComparerFactory().Visit(obj).GetHashCode(obj);
        }

        private class TokenComparerFactory : TokenVisitor<IEqualityComparer<Token>>
        {
            public override IEqualityComparer<Token> Visit(PoleToken t)
            {
                return new PoleComparer();
            }

            public override IEqualityComparer<Token> Visit(EndToken t)
            {
                return new EndComparer();
            }

            public override IEqualityComparer<Token> Visit(FlagToken t)
            {
                return new FlagComparer();
            }

            public override IEqualityComparer<Token> Visit(StringToken t)
            {
                return new StringComparer();
            }

            private class PoleComparer : TokenComparer<PoleToken>
            {
                protected override bool Equals(PoleToken a, PoleToken b)
                {
                    return true;
                }

                protected override int GetHashCode(PoleToken obj)
                {
                    return 0;
                }
            }

            private class FlagComparer : TokenComparer<FlagToken>
            {
                protected override bool Equals(FlagToken a, FlagToken b)
                {
                    return true;
                }

                protected override int GetHashCode(FlagToken obj)
                {
                    return 0;
                }
            }

            private class EndComparer : TokenComparer<EndToken>
            {
                protected override bool Equals(EndToken a, EndToken b)
                {
                    return true;
                }

                protected override int GetHashCode(EndToken obj)
                {
                    return 0;
                }
            }

            private class StringComparer : TokenComparer<StringToken>
            {
                protected override bool Equals(StringToken a, StringToken b)
                {
                    return a.Text == b.Text;
                }

                protected override int GetHashCode(StringToken obj)
                {
                    return obj.Text.GetHashCode();
                }
            }

            private abstract class TokenComparer<T> : IEqualityComparer<Token>
            where T : Token
            {
                protected abstract bool Equals(T a, T b);
                protected abstract int GetHashCode(T obj);

                public bool Equals(Token x, Token y)
                {
                    return x is T && y is T && Equals((T)x, (T)y);
                }

                public int GetHashCode(Token obj)
                {
                    if (obj is T)
                        return GetHashCode((T)obj) ^ typeof(T).GetHashCode();
                    throw new Exception("Bad type in GetHashCode");
                }
            }
        }
    }
}
