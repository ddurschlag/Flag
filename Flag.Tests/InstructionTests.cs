using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace Flag.Tests
{
    using Parse;
    using Parse.Instructions;
    using Parse.Tokens;
    using Parse.Structures;
    using Flag.Compile;
    using System.IO;
    using System.CodeDom.Compiler;
    using Microsoft.CSharp;
    using System.Reflection;

    [TestFixture]
    public class InstructionTests
    {
        [Test]
        public void Render()
        {
            AssertFlagCompilation(
                "~||~",
                new Token[] { new FlagToken(), new PoleToken(), new PoleToken(), new FlagToken(), new EndToken() },
                new Structure[] { new CommandStructure(null, new Structure[0], null) },
                new Instruction[] { new RenderInstruction() }
            );
        }

        [Test]
        public void Output()
        {
            AssertFlagCompilation(
                "abc",
                new Token[] { new StringToken("abc"), new EndToken() },
                new Structure[] { new OutputStructure("abc") },
                new Instruction[] { new OutputInstruction("abc") }
            );
        }

        [Test]
        public void SimpleBunting()
        {
            var contents = @"List~
~

~Title|~||~|~:
~Items|~||Item~|~

~

~Item~
~  * ~Text|~||~|~
~
";

            var sb = new StringBuilder();

            sb.Append(@"
namespace Testing
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    class Program
    {
        public static string Main()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                NS.CLASS.List(
                new NS.CLASS.ListViewModel
                {
                    Title = ""A"",
                    Items = new List<NS.CLASS.ItemViewModel> {
                            ""B"",
                            ""C""
                    }
                },
                sw
                );
            }

            return sb.ToString();
        }
    }
}
");

            using (var sw = new StringWriter(sb))
            {
                new BuntingCompiler(new Compile.CSharp.TemplateCompiler("NS", "CLASS")).Compile(contents, sw);
            }

            PowerAssert.PAssert.IsTrue(() => CompileAndRun(new string[] { sb.ToString() }) == "\r\n\r\nA:\r\n  * B\r\n  * C\r\n\r\n\r\n");
        }

        private static void AssertFlagCompilation(
            string s,
            IEnumerable<Token> expectedTokens,
            IEnumerable<Structure> expectedStructures,
            IEnumerable<Instruction> expectedInstructions
        )
        {
            var tokens = new Tokenizer().Tokenize(s).ToArray();
            var structures = new Structurizer().Structurize(tokens).ToArray();
            var instructions = new Parser().Parse(structures).ToArray();

            if (expectedTokens != null)
                PowerAssert.PAssert.IsTrue(() => tokens.SequenceEqual(expectedTokens, new TokenComparer()));
            if (expectedStructures != null)
                PowerAssert.PAssert.IsTrue(() => structures.SequenceEqual(expectedStructures, new StructureComparer()));
            if (expectedInstructions != null)
                PowerAssert.PAssert.IsTrue(() => instructions.SequenceEqual(expectedInstructions, new InstructionComparer()));
        }

       private static string CompileAndRun(string[] code)
        {
            CompilerParameters CompilerParams = new CompilerParameters();

            CompilerParams.GenerateInMemory = true;
            CompilerParams.TreatWarningsAsErrors = false;
            CompilerParams.GenerateExecutable = false;
            CompilerParams.CompilerOptions = "/optimize";

            string[] references = { "System.dll" };
            CompilerParams.ReferencedAssemblies.AddRange(references);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);

            if (compile.Errors.HasErrors)
            {
                string text = "Compile error: ";
                foreach (CompilerError ce in compile.Errors)
                {
                    text += "rn" + ce.ToString();
                }
                throw new Exception(text);
            }

            Module module = compile.CompiledAssembly.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
            {
                mt = module.GetType("Testing.Program");
            }

            if (mt != null)
            {
                methInfo = mt.GetMethod("Main");
            }

            if (methInfo != null)
            {
                return (string)methInfo.Invoke(null, null);
            }

            return string.Empty;
        }
    }
}
