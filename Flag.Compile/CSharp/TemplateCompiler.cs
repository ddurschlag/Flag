﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flag.Compile.CSharp
{
    using Parse;
    using Parse.Instructions;
    using Parse.Structures;
    public class TemplateCompiler
    {
        public TemplateCompiler(string text, string @namespace, string name)
        {
            Text = text;
            Namespace = @namespace;
            Name = name;
        }

        public void Compile(TextWriter writer)
        {
            Templates.Templates.Class(
                new TypeViewModel(
                    "Templates",
                    Namespace,
                    new[] { Tuple.Create(Name, Load(Text)) }),
                writer
            );
        }

        private string Text;
        private string Name;
        private string Namespace;

        private static IEnumerable<Instruction> Load(string s)
        {
            return new Parser().Parse(new Structurizer().Structurize(new Tokenizer().Tokenize(s)));
        }
    }
}