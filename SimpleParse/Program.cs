using System;
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
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                new Flag.Compile.CSharp.BuntingCompiler(@"FIRST_TEST~
~abc~

~SECOND_TEST~

~a~||~b~

~THIRD_TEST~

~\\\|\~~

~FOURTH_TEST~

~a~||t~b~

~FIFTH_TEST~~a~|t|~b~

~SIXTH_TEST~~a~k||t~b~

~SEVENTH_TEST~

~a~k|t|~EOF", "Test.Bunting", "BuntingTests").Compile(sw);
            }
            Console.WriteLine(sb.ToString());
            Console.ReadLine();


            var buntingTestText = @"FIRST TEST~
~abc~

~SECOND TEST~

~a~||~b~

~THIRD TEST~

~\\\|\~~

~FOURTH TEST~

~a~||t~b~

~FIFTH TEST~~a~|t|~b~

~SIXTH TEST~~a~k||t~b~

~SEVENTH TEST~

~a~k|t|~EOF";

            var buntingTest = BuntingTest(buntingTestText);

            Action<string, string> ba = (name, text) =>
             {
                 Console.WriteLine(string.Format("Testing bunting on {0}: {1}", name, text));

                 var bunting = buntingTest[name];
                 var flag = Test(text);

                 if (new InstructionSequenceComparer().Equals(bunting, flag))
                 {
                     Console.WriteLine("Success");
                 }
                 else {
                     Console.WriteLine("Fail");
                 }
             };

            ba("FIRST TEST", @"abc");
            ba("SECOND TEST", @"a~||~b");
            ba("THIRD TEST", @"\\\|\~");
            ba("FOURTH TEST", @"a~||t~b");
            ba("FIFTH TEST", @"a~|t|~b");
            ba("SIXTH TEST", @"a~k||t~b");
            ba("SEVENTH TEST", @"a~k|t|~EOF");

            Console.ReadLine();

            var dir = new TemplateDirectory();

            foreach (var instStream in Directory
                .GetFiles(@"C:\VP\playpens\Hoodlums\SimpleParse\Flag.Compile\CSharp\Templates", "*.flag")
                .Select(fileName =>
                        (IEnumerable<Instruction>)Test(File.ReadAllText(fileName)
                ))
                .SelectMany(seq =>
                    new SequenceFinder(seq).Sequences
                )
            )
            {
                dir.Add(instStream);
            }

            foreach (var pair in dir.Counts.Where(seq => seq.Item2 > 2).OrderByDescending(seq => seq.Item2))
            {
                var s = string.Join<Instruction>(" ", pair.Item1);
                if (s.Length >= 70)
                    s = s.Substring(0, 67) + "...";
                Console.WriteLine(pair.Item2 + ": " + s);
                Console.WriteLine();
            }

            Console.ReadLine();

            Test("abc");
            Test("a~||~b");
            Test(@"\\\|\~");
            Test(@"a~||t~b");
            Test(@"a~|t|~b");
            Test(@"a~k||t~b");
            Test(@"a~k|t|~b");
            Console.ReadLine();
            var ins = Test(@"using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ~Namespace|~||~|~ {
    public partial class ~Name|~||~|~ {
        ~Templates|~|

        public static void ~Item1|~||~|~(dynamic tArg, TextWriter writer) {
            ~Item2||Template~
        }   

        |~|~

        ~ViewModels|~||ViewModel~|~
    }
}");

            //var types = new Flag.Compile.CSharp.ViewModelTypes.ViewModelTypeFactory().Manufacture("Class", ins).GroupBy(vmt => vmt.TypeName).ToDictionary(g => g.Key, g => g.ToArray());


            Console.ReadLine();

            var de = new StronglyTypedDataSource().Adapt(
                new Dictionary<string, string[]> {
                    {"First List", new[] {"Item A", "Item B" } },
                    {"Second List", new [] {"Item C", "Item D" } }
                });

            sb = new StringBuilder();

            //sb = new StringBuilder();
            //using (var sw = new StringWriter(sb))
            //{
            //    new DirectoryCompiler(@"CSharp\Templates", "Flag.Compile.CSharp.Templates").Compile(sw);
            //}

            //File.WriteAllText("CompiledTemplate.cs", sb.ToString());
        }

        public class SequenceFinder : InstructionVisitor<IEnumerable<Instruction>>
        {
            public SequenceFinder(IEnumerable<Instruction> root)
            {
                Find(root);
            }

            private void Find(IEnumerable<Instruction> seq)
            {
                _Sequences.Add(seq);
                foreach (var s in seq.Select(Visit).Where(subSeq => subSeq != null))
                    Find(s);
            }

            public override IEnumerable<Instruction> Visit(OutputInstruction i)
            {
                return null;
            }

            public override IEnumerable<Instruction> Visit(RenderInstruction i)
            {
                return null;
            }

            public override IEnumerable<Instruction> Visit(LoopInstruction i)
            {
                return null;
            }

            public override IEnumerable<Instruction> Visit(LoopInlineInstruction i)
            {
                return i.Instructions;
            }

            public override IEnumerable<Instruction> Visit(CallInstruction i)
            {
                return null;
            }

            public override IEnumerable<Instruction> Visit(CallInlineInstruction i)
            {
                return i.Instructions;
            }

            private List<IEnumerable<Instruction>> _Sequences = new List<IEnumerable<Instruction>>();

            public IEnumerable<IEnumerable<Instruction>> Sequences { get { return _Sequences; } }
        }

        public class TemplateDirectory
        {
            public void Add(IEnumerable<Instruction> sequence)
            {
                Sequences.Add(sequence.ToArray());
            }

            private List<Instruction[]> Sequences = new List<Instruction[]>();
            public IEnumerable<Tuple<Instruction[], int>> Counts
            {
                get { return Sequences.GroupBy(s => s, new InstructionSequenceComparer()).Select(g => Tuple.Create(g.Key.ToArray(), g.Count())); }
            }
        }


        //public class DirectoryCompiler
        //{
        //    public DirectoryCompiler(string path, string @namespace)
        //    {
        //        Path = path;
        //        Namespace = @namespace;
        //        Name = new DirectoryInfo(path).Name;
        //    }

        //    public void Compile(TextWriter writer)
        //    {
        //        Flag.Compile.CSharp.Templates.Templates.Class(new Flag.Compile.CSharp.TypeViewModel(Name, Namespace,
        //            Directory.GetFiles(Path, "*.flag").Select(fileName =>
        //                Tuple.Create(System.IO.Path.GetFileNameWithoutExtension(fileName), (IEnumerable<Instruction>)Load(File.ReadAllText(fileName)))
        //            )
        //        ), writer);
        //    }

        //    private string Path;
        //    private string Name;
        //    private string Namespace;

        //    private static Instruction[] Load(string s)
        //    {
        //        var tokens = new Tokenizer().Tokenize(s);
        //        var structures = new Structurizer().Structurize(tokens).ToArray();
        //        var instructions = new Parser().Parse(structures).ToArray();
        //        Console.WriteLine("input: " + s);
        //        Console.WriteLine("tokens: " + string.Join(" ", tokens));
        //        Console.WriteLine("structures: " + string.Join<Structure>(" ", structures));
        //        Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
        //        Console.WriteLine();
        //        return instructions;
        //    }
        //}

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
            var tokens = new Tokenizer().Tokenize(s).ToArray();
            var structures = new Structurizer().Structurize(tokens).ToArray();
            var instructions = new Parser().Parse(structures).ToArray();
            Console.WriteLine("input: " + s);
            Console.WriteLine("tokens: " + string.Join<Token>(" ", tokens));
            Console.WriteLine("structures: " + string.Join<Structure>(" ", structures));
            Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
            Console.WriteLine();
            return instructions;
        }

        private static Dictionary<string, Instruction[]> BuntingTest(string s)
        {
            Dictionary<string, Instruction[]> result = new Dictionary<string, Instruction[]>();
            var tokens = new Tokenizer().Tokenize(s).ToArray();
            var structures = new BuntingStructurizer().Structurize(tokens).ToArray();

            Console.WriteLine("input: " + s);
            Console.WriteLine("tokens: " + string.Join<Token>(" ", tokens));

            foreach (var template in structures)
            {
                Console.WriteLine("\t\t" + template.Item1);
                var instructions = new Parser().Parse(template.Item2).ToArray();
                Console.WriteLine("structures: " + string.Join<Structure>(" ", template.Item2));
                Console.WriteLine("instructions: " + string.Join<Instruction>(" ", instructions));
                result.Add(template.Item1, instructions);
                Console.WriteLine();
            }

            return result;
        }
    }
}
