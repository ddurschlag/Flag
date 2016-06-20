using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    public class CommandStructure : Structure
    {
        public CommandStructure(string key, IEnumerable<Structure> inline, string name)
        {
            Key = key;
            Inline = inline.ToArray();
            Name = name;
        }

        public string Key { get; private set; }
        public Structure[] Inline { get; private set; }
        public string Name { get; private set; }

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
            return string.Format("~{0}|{1}|{2}~", Key, string.Join<Structure>(" ", Inline), Name);
        }
    }
}
