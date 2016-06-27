﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SimpleParse.Instructions;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.IO;
using Flag.Parse.Structures;
using Flag.Parse.Tokens;
using Flag.Parse.Instructions;
using Flag.Parse;
using Flag.Interpret;
using System.Collections;

namespace SimpleParse
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test("abc");
            //Test("a~||~b");
            //Test(@"\\\|\~");
            //Test(@"a~||t~b");
            //Test(@"a~|t|~b");
            //Test(@"a~k||t~b");
            //Test(@"a~k|t|~b");

            var de = new StronglyTypedDataSource().Adapt(
                new Dictionary<string, string[]> {
                    {"First List", new[] {"Item A", "Item B" } },
                    {"Second List", new [] {"Item C", "Item D" } }
                });

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                new DirectoryCompiler(@"CSharp\Templates", "Flag.Compile.CSharp.Templates").Compile(sw);
            }

            File.WriteAllText("CompiledTemplate.cs", sb.ToString());

            sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
                new Flag.Compile.CSharp.TemplateCompiler(File.ReadAllText(@"CSharp\Templates\Call.flag"), "Flag.Compile.CSharp.Templates", "Call").Compile(tw);

            File.WriteAllText("Call.cs", sb.ToString());

            Console.ReadLine();
        }

        public class VMTest
        {
            public class SecretList<T> : IEnumerable<T>
            {
                public SecretList(List<T> wrapped) { Wrapped = wrapped; }
                private List<T> Wrapped;
                public static implicit operator SecretList<T>(List<T> l) { return new SecretList<T>(l); }
                public static implicit operator List<T>(SecretList<T> l) { return l.Wrapped; }

                public void Add(T item) { Wrapped.Add(item); }

                public IEnumerator<T> GetEnumerator()
                {
                    return Wrapped.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return Wrapped.GetEnumerator();
                }
            }

            public class SecretString
            {
                public SecretString(string wrapped) { Wrapped = wrapped; }
                private string Wrapped;

                public static implicit operator SecretString(string l) { return new SecretString(l); }
                public static implicit operator string(SecretString l) { return l.Wrapped; }
            }

            public static Root Example = new Root
            {
                Simple = { Text = "Foo" },
                Weird = { Text = "Bar", RenderAndProperty = 3 },
                SuperWeird = new NonSimpleList("A")
                {
                    "A",
                    new Simple {Text="BAR" }
                },
                Multi = new MultiList {
                "A",
                new Simple{Text = "FOO" },
                "B"
                }
            };

            public class Root
            {
                public Simple Simple { get; set; }
                public MapWithUnknowns Weird { get; set; }
                public NonSimpleList SuperWeird { get; set; }
                public MultiList Multi { get; set; }
            }

            public class Simple
            {
                public SecretString Text { get; set; }
            }

            public class MapWithUnknowns
            {
                public SecretString Text
                {
                    get; set;
                }

#warning Could not generate ViewModel for "RenderAndProperty"; falling back to dynamic
                public dynamic RenderAndProperty { get; set; }
            }

            public class MultiList : IEnumerable<Simple>, IEnumerable<SecretString>
            {
                public SecretList<Simple> Simples = new List<Simple>();
                public SecretList<SecretString> Strings = new List<SecretString>();

                public void Add(Simple @simple)
                {
                    Simples.Add(@simple);
                }

                public void Add(string @string)
                {
                    Strings.Add(@string);
                }

                public IEnumerator<Simple> GetEnumerator()
                {
                    return Simples.GetEnumerator();
                }

#warning Multiple loop types
                IEnumerator IEnumerable.GetEnumerator()
                {
                    var error = new Exception("Conflicting list types");
                    error.Data.Add("Types", string.Join(", ", new[] { typeof(Simple).ToString(), typeof(string).ToString() }));
                    error.Data.Add("Class", typeof(NonSimpleList).ToString());
                    throw error;
                }

                IEnumerator<SecretString> IEnumerable<SecretString>.GetEnumerator()
                {
                    foreach (var x in Strings)
                        yield return x;
                }
            }

            public class NonSimpleList : IEnumerable<Simple>, IEnumerable<string>
            {
                public NonSimpleList(string text = null, SecretList<Simple> simples = null, SecretList<string> strings = null)
                {
                    Text = text;
                    Simples = simples ?? new List<Simple>();
                    Strings = strings ?? new List<string>();
                }

                public string Text { get; set; }
                public SecretList<Simple> Simples = new List<Simple>();
                public SecretList<string> Strings = new List<string>();

                public void Add(Simple @simple)
                {
                    Simples.Add(@simple);
                }

                public void Add(string @string)
                {
                    Strings.Add(@string);
                }

                public IEnumerator<Simple> GetEnumerator()
                {
                    return Simples.GetEnumerator();
                }

#warning Multiple loop types
                IEnumerator IEnumerable.GetEnumerator()
                {
                    var error = new Exception("Conflicting list types");
                    error.Data.Add("Types", string.Join(", ", new[] { typeof(Simple).ToString(), typeof(string).ToString() }));
                    error.Data.Add("Class", typeof(NonSimpleList).ToString());
                    throw error;
                }

                IEnumerator<string> IEnumerable<string>.GetEnumerator()
                {
                    return Strings.GetEnumerator();
                }
            }
        }

        public class DirectoryCompiler
        {
            public DirectoryCompiler(string path, string @namespace)
            {
                Path = path;
                Namespace = @namespace;
                Name = new DirectoryInfo(path).Name;
            }

            public void Compile(TextWriter writer)
            {
                Flag.Compile.CSharp.Templates.Templates.Class(new Flag.Compile.CSharp.TypeViewModel(Name, Namespace,
                    Directory.GetFiles(Path, "*.flag").Select(fileName =>
                        Tuple.Create(System.IO.Path.GetFileNameWithoutExtension(fileName), (IEnumerable<Instruction>)Load(File.ReadAllText(fileName)))
                    )
                ), writer);
            }

            private string Path;
            private string Name;
            private string Namespace;

            private static Instruction[] Load(string s)
            {
                var tokens = new Tokenizer().Tokenize(s);
                var structures = new Structurizer().Structurize(tokens).ToArray();
                var instructions = new Parser().Parse(structures).ToArray();
                Console.WriteLine("input: " + s);
                Console.WriteLine("tokens: " + string.Join(" ", tokens));
                Console.WriteLine("structures: " + string.Join<Structure>(" ", structures));
                Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
                Console.WriteLine();
                return instructions;
            }
        }

        public class DirectoryInterpreter
        {
            public DirectoryInterpreter(string path) { Path = path; }

            public void Interpret(TextWriter tw, Flag.Interpret.Data.Element de, string name)
            {
                new Interpreter().Interpret(tw, de, Lookup(name), Lookup);
            }

            public string Path { get; private set; }

            public IEnumerable<Instruction> Lookup(string name)
            {
                return Load(System.IO.File.ReadAllText(System.IO.Path.Combine(Path, name + ".flag")));
            }

            private static Instruction[] Load(string s)
            {
                var tokens = new Tokenizer().Tokenize(s);
                var structures = new Structurizer().Structurize(tokens).ToArray();
                var instructions = new Parser().Parse(structures).ToArray();
                Console.WriteLine("input: " + s);
                Console.WriteLine("tokens: " + string.Join(" ", tokens));
                Console.WriteLine("structures: " + string.Join<Structure>(" ", structures));
                Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
                Console.WriteLine();
                return instructions;
            }
        }

        private static Instruction[] Test(string s)
        {
            var tokens = new Tokenizer().Tokenize(s);
            var structures = new Structurizer().Structurize(tokens).ToArray();
            var instructions = new Parser().Parse(structures).ToArray();
            Console.WriteLine("input: " + s);
            Console.WriteLine("tokens: " + string.Join(" ", tokens));
            Console.WriteLine("structures: " + string.Join<Structure>(" ", structures));
            Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
            Console.WriteLine();
            return instructions;
        }
    }
}
