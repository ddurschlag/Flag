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

            Console.ReadLine();
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
                new Flag.Compile.CSharp.Templates.Templates(writer).Class(new Flag.Compile.CSharp.TypeViewModel(Name, Namespace,
                    Directory.GetFiles(Path, "*.flag").Select(fileName =>
                        Tuple.Create(System.IO.Path.GetFileNameWithoutExtension(fileName), (IEnumerable<Instruction>)Load(File.ReadAllText(fileName)))
                    )
                ));


                //new Flag.Compile.CSharp.ClassContents(Name,
                //    Directory.GetFiles(Path, "*.flag").Select(fileName =>
                //        Tuple.Create(System.IO.Path.GetFileNameWithoutExtension(fileName), (IEnumerable<Instruction>)Load(File.ReadAllText(fileName)))
                //    )
                //).Write(writer);
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
