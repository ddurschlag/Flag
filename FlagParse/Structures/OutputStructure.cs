using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    public class OutputStructure : Structure
    {
        public OutputStructure(string text) { Text = text; }

        public string Text { get; private set; }

        public override void Accept(StructureVisitor v)
        {
            v.Visit(this);
        }

        public override T Accept<T>(StructureVisitor<T> v)
        {
            return v.Visit(this);
        }

        public override string ToString()
        {
            return string.Concat("\"", Text, "\"");
        }
    }
}
