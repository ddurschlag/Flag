using System.IO;

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
            var templates = new BuntingStructurizer().Structurize(new Tokenizer().Tokenize(Text));

            foreach (var t in templates)
            {
                new TemplateCompiler(new Parser().Parse(t.Item2), Namespace, t.Item1, ClassName).Compile(writer);
            }
        }

        private string Text;
        private string Namespace;
        private string ClassName;
    }
}
