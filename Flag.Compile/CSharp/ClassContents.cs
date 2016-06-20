using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Compile.CSharp
{
    using Parse.Instructions;
    public class ClassContents
    {
        public ClassContents(string name, IEnumerable<Tuple<string, IEnumerable<Instruction>>> templates)
        {
            Name = name;
            Templates = templates.ToArray();
        }

        public void Write(System.IO.TextWriter writer)
        {
            writer.Write("using System;");
            writer.Write("using System.IO;");
            writer.Write(string.Format("public class {0} {{", Name));
            writer.Write("private TextWriter Writer; private void Write(string s){Writer.Write(s);}");
            writer.Write(string.Format("public {0}(TextWriter writer){{Writer=writer;}}", Name));
            foreach (var template in Templates)
            {
                writer.Write(string.Format("public void {0}(dynamic tArg) {{", template.Item1));
                var bc = new BlockContents("tArg", writer);
                foreach (var i in template.Item2)
                    bc.Visit(i);
                writer.Write("}");
            }
            writer.Write("}");
        }

        public string Name { get; private set; }
        private Tuple<string, IEnumerable<Instruction>>[] Templates;
    }
}
