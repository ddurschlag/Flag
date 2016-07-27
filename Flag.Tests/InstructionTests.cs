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

    [TestFixture]
    public class InstructionTests
    {
        [Test]
        public void Render()
        {
            AssertCompilation(
                "~||~",
                new Token[] { new FlagToken(), new PoleToken(), new PoleToken(), new FlagToken(), new EndToken() },
                new Structure[] { new CommandStructure(null, new Structure[0], null) },
                new Instruction[] { new RenderInstruction() }
            );
        }

        [Test]
        public void Output()
        {
            AssertCompilation(
                "abc",
                new Token[] { new StringToken("abc"), new EndToken() },
                new Structure[] { new OutputStructure("abc") },
                new Instruction[] { new OutputInstruction("abc") }
            );
        }


        private static void AssertCompilation(
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
    }
}
