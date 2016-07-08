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

            //Test("abc");
            //Test("a~||~b");
            //Test(@"\\\|\~");
            //Test(@"a~||t~b");
            //Test(@"a~|t|~b");
            //Test(@"a~k||t~b");
            //Test(@"a~k|t|~b");

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

            var sb = new StringBuilder();

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

            private class TemplateComparer : IEqualityComparer<IEnumerable<Instruction>>
            {
                private class InstructionComparer : IEqualityComparer<Instruction>
                {
                    private class InstructionComparerFactory : InstructionVisitor<IEqualityComparer<Instruction>>
                    {
                        private abstract class InstructionComparer<T> : IEqualityComparer<Instruction>
                        where T : Instruction
                        {
                            protected abstract bool Equals(T a, T b);
                            protected abstract int GetHashCode(T obj);

                            public bool Equals(Instruction x, Instruction y)
                            {
                                return x is T && y is T && Equals((T)x, (T)y);
                            }

                            public int GetHashCode(Instruction obj)
                            {
                                if (obj is T)
                                    return GetHashCode((T)obj) ^ typeof(T).GetHashCode();
                                throw new Exception("Bad type in GetHashCode");
                            }
                        }
                        private class LoopComparer : InstructionComparer<LoopInstruction>
                        {
                            protected override bool Equals(LoopInstruction a, LoopInstruction b)
                            {
                                return a.Name == b.Name;
                            }

                            protected override int GetHashCode(LoopInstruction obj)
                            {
                                return obj.Name.GetHashCode();
                            }
                        }

                        private class CallComparer : InstructionComparer<CallInstruction>
                        {
                            protected override bool Equals(CallInstruction a, CallInstruction b)
                            {
                                return a.Key == b.Key && a.Name == b.Name;
                            }

                            protected override int GetHashCode(CallInstruction obj)
                            {
                                return obj.Key.GetHashCode() ^ obj.Name.GetHashCode();
                            }
                        }

                        private class CallInlineComparer : InstructionComparer<CallInlineInstruction>
                        {
                            protected override bool Equals(CallInlineInstruction a, CallInlineInstruction b)
                            {
                                return a.Key == b.Key && new TemplateComparer().Equals(a.Instructions, b.Instructions);
                            }

                            protected override int GetHashCode(CallInlineInstruction obj)
                            {
                                return obj.Key.GetHashCode() ^ new TemplateComparer().GetHashCode(obj.Instructions);
                            }
                        }

                        private class LoopInlineComparer : InstructionComparer<LoopInlineInstruction>
                        {
                            protected override bool Equals(LoopInlineInstruction a, LoopInlineInstruction b)
                            {
                                return new TemplateComparer().Equals(a.Instructions, b.Instructions);
                            }

                            protected override int GetHashCode(LoopInlineInstruction obj)
                            {
                                return new TemplateComparer().GetHashCode(obj.Instructions);
                            }
                        }

                        private class RenderComparer : InstructionComparer<RenderInstruction>
                        {
                            protected override bool Equals(RenderInstruction a, RenderInstruction b)
                            {
                                return true;
                            }

                            protected override int GetHashCode(RenderInstruction obj)
                            {
                                return 0;
                            }
                        }

                        private class OutputComparer : InstructionComparer<OutputInstruction>
                        {
                            protected override bool Equals(OutputInstruction a, OutputInstruction b)
                            {
                                return a.Text == b.Text;
                            }

                            protected override int GetHashCode(OutputInstruction obj)
                            {
                                return obj.Text.GetHashCode();
                            }
                        }

                        public override IEqualityComparer<Instruction> Visit(LoopInstruction i)
                        {
                            return new LoopComparer();
                        }

                        public override IEqualityComparer<Instruction> Visit(CallInstruction i)
                        {
                            return new CallComparer();
                        }

                        public override IEqualityComparer<Instruction> Visit(CallInlineInstruction i)
                        {
                            return new CallInlineComparer();
                        }

                        public override IEqualityComparer<Instruction> Visit(LoopInlineInstruction i)
                        {
                            return new LoopInlineComparer();
                        }

                        public override IEqualityComparer<Instruction> Visit(RenderInstruction i)
                        {
                            return new RenderComparer();
                        }

                        public override IEqualityComparer<Instruction> Visit(OutputInstruction i)
                        {
                            return new OutputComparer();
                        }
                    }

                    public bool Equals(Instruction x, Instruction y)
                    {
                        return new InstructionComparerFactory().Visit(x).Equals(x,y);
                    }

                    public int GetHashCode(Instruction obj)
                    {
                        return new InstructionComparerFactory().Visit(obj).GetHashCode(obj);
                    }
                }

                public bool Equals(IEnumerable<Instruction> x, IEnumerable<Instruction> y)
                {
                    var c = new InstructionComparer();
                    return x.Zip(y, c.Equals).All(b => b);
                }

                public int GetHashCode(IEnumerable<Instruction> obj)
                {
                    var c = new InstructionComparer();
                    return obj.Select(i => c.GetHashCode(i)).Aggregate((a, b) => a ^ b);
                }
            }

            private List<Instruction[]> Sequences = new List<Instruction[]>();
            public IEnumerable<Tuple<Instruction[], int>> Counts
            {
                get { return Sequences.GroupBy(s => s, new TemplateComparer()).Select(g => Tuple.Create(g.Key.ToArray(), g.Count())); }
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
