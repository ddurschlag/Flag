using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Flag.Compile
{
    using Parse;

    public class BuntingCompiler
    {
        public BuntingCompiler(ITemplateCompiler compiler)
        {
            Compiler = compiler;
        }

        public void Compile(string text, TextWriter writer)
        {
            var templates = new BuntingStructurizer().Structurize(new Tokenizer().Tokenize(text)).ToDictionary(
                t => t.Item1,
                t => new Parser().Parse(t.Item2).ToArray()
            );

            foreach (var t in templates)
            {
                Compiler.Compile(t.Key, t.Value, writer);
            }
        }

        private ITemplateCompiler Compiler;
    }
}
