using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse
{
    using Tokens;
    using Structures;
    public class Structurizer
    {
        public IEnumerable<Structure> Structurize(IEnumerable<Token> input)
        {
            VisitorState s = new VisitorState();
            s.Peek.Current = new OuterOutput(s);

            foreach (var t in input)
                foreach (var output in s.Peek.Current.Visit(t))
                    yield return output;
        }

        private class VisitorState
        {
            public VisitorState()
            {
                Frames = new Stack<VisitorFrame>();
                Frames.Push(new VisitorFrame());
            }

            public Stack<VisitorFrame> Frames;

            public VisitorFrame Peek { get { return Frames.Peek(); } }
        }

        private class VisitorFrame
        {
            public StructureFactory Current;
            public string Key;
            public List<Structure> Inline = new List<Structure>();
            public string Name;
        }

        private abstract class StructureFactory : TokenVisitor<IEnumerable<Structure>>
        {
            public StructureFactory(VisitorState s)
            {
                State = s;
            }

            protected VisitorState State { get; private set; }

            public override IEnumerable<Structure> Visit(PoleToken t)
            {
                Fail(t);
                yield break;
            }

            public override IEnumerable<Structure> Visit(EndToken t)
            {
                Fail(t);
                yield break;
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                Fail(t);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                Fail(t);
                yield break;
            }

            protected void Fail(Token t)
            {
                Fail("Unexpected token type", t);
            }

            protected void Fail(string message, Token t)
            {
                var error = new Exception(message);
                error.Data.Add("Token", t);
                error.Data.Add("Context", State);
                throw error;
            }
        }

        private class OuterOutput : StructureFactory
        {
            public OuterOutput(VisitorState s) : base(s) { }

            public override IEnumerable<Structure> Visit(EndToken t)
            {
                yield break;
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                State.Peek.Current = new OuterKey(State);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                yield return new OutputStructure(t.Text);
            }
        }

        private class OuterKey : StructureFactory
        {
            public OuterKey(VisitorState s) : base(s) { }
            public override IEnumerable<Structure> Visit(PoleToken t)
            {
                State.Peek.Current = new OuterInline(State);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Key = t.Text;
                yield break;
            }
        }

        private class OuterInline : StructureFactory
        {
            public OuterInline(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Structure> Visit(PoleToken t)
            {
                State.Peek.Current = new OuterName(State);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Inline.Add(new OutputStructure(t.Text));
                yield break;
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                State.Frames.Push(new VisitorFrame());
                State.Peek.Current = new InnerKey(State);
                yield break;
            }
        }

        private class OuterName : StructureFactory
        {
            public OuterName(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                yield return new CommandStructure(State.Peek.Key, State.Peek.Inline, State.Peek.Name);
                State.Frames.Pop();
                State.Frames.Push(new VisitorFrame() { Current = new OuterOutput(State) });
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Name = t.Text;
                yield break;
            }
        }

        private class InnerKey : StructureFactory
        {
            public InnerKey(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Key = t.Text;
                yield break;
            }

            public override IEnumerable<Structure> Visit(PoleToken t)
            {
                State.Peek.Current = new InnerInline(State);
                yield break;
            }
        }

        private class InnerInline : StructureFactory
        {
            public InnerInline(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Structure> Visit(PoleToken t)
            {
                State.Peek.Current = new InnerName(State);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Inline.Add(new OutputStructure(t.Text));
                yield break;
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                State.Frames.Push(new VisitorFrame());
                State.Peek.Current = new InnerKey(State);
                yield break;
            }
        }

        private class InnerName : StructureFactory
        {
            public InnerName(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Structure> Visit(FlagToken t)
            {
                var innerCommand = new CommandStructure(State.Peek.Key, State.Peek.Inline, State.Peek.Name);
                State.Frames.Pop();
                State.Peek.Inline.Add(innerCommand);
                yield break;
            }

            public override IEnumerable<Structure> Visit(StringToken t)
            {
                State.Peek.Name = t.Text;
                yield break;
            }
        }
    }
}
