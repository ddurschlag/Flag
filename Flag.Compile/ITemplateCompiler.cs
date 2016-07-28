using System.IO;
using System.Collections.Generic;

namespace Flag.Compile
{
    using Parse.Instructions;
    public interface ITemplateCompiler
    {
        void Compile(string Name, string text, TextWriter writer);
        void Compile(string Name, IEnumerable<Instruction> instructions, TextWriter writer);
    }
}
