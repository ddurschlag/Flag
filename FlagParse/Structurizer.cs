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
        private VisitorState State;

        public Structurizer()
        {
            State = new VisitorState();
            State.Peek.Current = new OuterOutput(State);
        }

        public IEnumerable<Structure> Structurize(IEnumerable<Token> input)
        {
            return input.SelectMany(Structurize);   //Unsafe to inline Structurize, as it will ignore changes
                                                    //to current state!
        }

        public IEnumerable<Structure> Structurize(Token input)
        {
            //This code looks weird, because it is. Why can't we just return the result of visit?
            //The problem is a complex interaction between lazy yield-return methods and
            //the state pattern which, inherently, updates state. The result is that the initial
            //state (OuterOutput in this case) will be used to process ALL input, and quickly fail.
            foreach (var s in State.Peek.Current.Visit(input))
                yield return s;
        }

        [Serializable]
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

        [Serializable]
        private class VisitorFrame
        {
            public StructureFactory<VisitorState, Structure> Current;
            public string Key;
            public List<Structure> Inline = new List<Structure>();
            public string Name;
        }

        private class OuterOutput : StructureFactory<VisitorState, Structure>
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

        private class OuterKey : StructureFactory<VisitorState, Structure>
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

        private class OuterInline : StructureFactory<VisitorState, Structure>
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

        private class OuterName : StructureFactory<VisitorState, Structure>
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

        private class InnerKey : StructureFactory<VisitorState, Structure>
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

        private class InnerInline : StructureFactory<VisitorState, Structure>
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

        private class InnerName : StructureFactory<VisitorState, Structure>
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
