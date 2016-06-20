using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Interpret.Data
{
    public class StringElement : Element
    {
        public StringElement(string value) { Value = value; }
        public string Value { get; private set; }
        public override void Accept(ElementVisitor v)
        {
            v.Visit(this);
        }
        public override T Accept<T>(ElementVisitor<T> v)
        {
            return v.Visit(this);
        }
    }
}
