using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Flag.Compile.CSharp
{
    using Parse;

    public class BuntingCompiler
    {
        public BuntingCompiler(string text, string @namespace, string className)
        {
            Text = text;
            Namespace = @namespace;
            ClassName = className;
        }

        public void Compile(TextWriter writer)
        {
            var templates = new BuntingStructurizer().Structurize(new Tokenizer().Tokenize(Text)).ToDictionary(
                t => t.Item1,
                t => new Parser().Parse(t.Item2).ToArray()
            );

            foreach (var t in templates)
            {
                new TemplateCompiler(t.Value, Namespace, t.Key, ClassName).Compile(writer);
            }
        }

        private string Text;
        private string Namespace;
        private string ClassName;
    }
}
