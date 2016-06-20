using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Flag.Parse.Tokens;

namespace Flag.Parse
{
    public class Tokenizer
    {
        public IEnumerable<Token> Tokenize(IEnumerable<char> input)
        {
            StringBuilder Builder = new StringBuilder();
            bool Escaped = false;

            foreach (var c in input)
            {
                if (Escaped)
                {
                    switch (c)
                    {
                        case '\\':
                        case '|':
                        case '~':
                            Builder.Append(c);
                            break;
                        default:
                            var error = new Exception("Unrecognized escape sequence");
                            error.Data.Add("Character", c);
                            throw error;
                    }
                    Escaped = false;
                }
                else {
                    string s;
                    switch (c)
                    {
                        case '\\':
                            Escaped = true;
                            break;
                        case '~':
                            s = Builder.ToString();
                            Builder.Clear();
                            if (!string.IsNullOrEmpty(s))
                                yield return new StringToken(s);
                            yield return new FlagToken();
                            break;
                        case '|':
                            s = Builder.ToString();
                            Builder.Clear();
                            if (!string.IsNullOrEmpty(s))
                                yield return new StringToken(s);
                            yield return new PoleToken();
                            break;
                        default:
                            Builder.Append(c);
                            break;
                    }
                }
            }
            string text = Builder.ToString();
            Builder.Clear();
            if (!string.IsNullOrEmpty(text))
                yield return new StringToken(text);
            yield return new EndToken();
        }
    }
}
