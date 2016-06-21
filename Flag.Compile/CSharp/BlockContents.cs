using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Flag.Compile.CSharp
{
    using Parse.Instructions;
    public class BlockContents : InstructionVisitor
    {
        static int i = 0;

        private static string GetVar() { return "anon_" + (i++); }

        private string ElementName;
        private TextWriter Writer;

        private void Write(string s) { Writer.Write(s); }

        public BlockContents(string elementName, TextWriter tw)
        {
            ElementName = elementName;
            Writer = tw;
        }

        public override void Visit(LoopInlineInstruction i)
        {
            var childElement = GetVar();
            Write(string.Format(@"if ( {1} != null ) foreach ( var {0} in {1} ) {{", childElement, ElementName));
            var contents = new BlockContents(childElement, Writer);
            foreach (var subInstruction in i.Instructions)
                contents.Visit(subInstruction);
            Write("}");

        }

        public override void Visit(CallInlineInstruction i)
        {
            var childElement = string.Format("{0}.{1}", ElementName, i.Key);
            var contents = new BlockContents(childElement, Writer);

            Write(string.Format(@"if ( {0} != null ) {{", childElement));
            foreach (var subInstruction in i.Instructions)
                contents.Visit(subInstruction);
            Write("}");
        }

        public override void Visit(CallInstruction i)
        {
            var childElement = string.Format("{0}.{1}", ElementName, i.Key);

            Write(string.Format(@"if ( {0} != null ) {{", childElement));
            Write(string.Format(@"{0}({1});", i.Name, childElement));
            Write("}");
        }

        public override void Visit(LoopInstruction i)
        {
            var childElement = GetVar();
            Write(string.Format(@"if ( {1} != null ) foreach ( var {0} in {1} ) {{", childElement, ElementName));
            Write(string.Format(@"{0}({1});", i.Name, childElement));
            Write("}");
        }

        public override void Visit(RenderInstruction i)
        {
            Write(string.Format("Write({0});", ElementName));
        }

        public override void Visit(OutputInstruction i)
        {
            Write(string.Format(@"Write(@""{0}"");", i.Text.Replace("\"", "\"\"")));
        }
    }
}
