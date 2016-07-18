using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse
{
    using Tokens;
    using Structures;
    public class BuntingStructurizer
    {
        private VisitorState State;

        public BuntingStructurizer()
        {
            State = new VisitorState();
            State.Peek.Current = new Header(State);
        }

        public IEnumerable<Tuple<string, IEnumerable<Structure>>> Structurize(IEnumerable<Token> input)
        {
            return input.SelectMany(Structurize);   //Unsafe to inline Structurize, as it will ignore changes
                                                    //to current state!
        }

        public IEnumerable<Tuple<string, IEnumerable<Structure>>> Structurize(Token input)
        {
            return State.Peek.Current.Visit(input);
        }

        [Serializable]
        private class VisitorState
        {
            public VisitorState()
            {
                Frames = new Stack<VisitorFrame>();
                Frames.Push(new VisitorFrame());
            }

            public string TemplateName;
            public List<Structure> TemplateContent = new List<Structure>();

            public Stack<VisitorFrame> Frames;

            public VisitorFrame Peek { get { return Frames.Peek(); } }
        }

        [Serializable]
        private class VisitorFrame
        {
            public StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>> Current;
            public string Key;
            public List<Structure> Inline = new List<Structure>();
            public string Name;
        }

        private class Header : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public Header(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.TemplateName = t.Text;
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                if ( State.TemplateName == null )
                {
                    throw new Exception("Template without name");
                }
                State.Peek.Current = new TemplateStart(State);
                yield break;
            }
        }

        private class TemplateStart : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public TemplateStart(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Peek.Current = new OuterOutput(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                if (!string.IsNullOrWhiteSpace(t.Text))
                    throw new Exception("Only whitespace is allowed amidst template dividers");
                yield break;
            }
        }

        private class OuterOutput : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public OuterOutput(VisitorState s) : base(s) { }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(EndToken t)
            {
                yield return Tuple.Create(State.TemplateName, (IEnumerable<Structure>)State.TemplateContent);
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Peek.Current = new OuterKey(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.TemplateContent.Add(new OutputStructure(t.Text));
                yield break;
            }
        }

        private class OuterKey : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public OuterKey(VisitorState s) : base(s) { }
            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(PoleToken t)
            {
                State.Peek.Current = new OuterInline(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Key = t.Text;
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                if (!string.IsNullOrWhiteSpace(State.Peek.Key))
                    throw new Exception("Only whitespace is allowed amidst template dividers");
                yield return Tuple.Create(State.TemplateName, (IEnumerable<Structure>)State.TemplateContent);
                State.TemplateContent = new List<Structure>();
                State.TemplateName = null;
                State.Frames.Pop();
                State.Frames.Push(new VisitorFrame() { Current = new Header(State) });
            }
        }

        private class OuterInline : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public OuterInline(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(PoleToken t)
            {
                State.Peek.Current = new OuterName(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Inline.Add(new OutputStructure(t.Text));
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Frames.Push(new VisitorFrame());
                State.Peek.Current = new InnerKey(State);
                yield break;
            }
        }

        private class OuterName : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public OuterName(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.TemplateContent.Add( new CommandStructure(State.Peek.Key, State.Peek.Inline, State.Peek.Name));
                State.Frames.Pop();
                State.Frames.Push(new VisitorFrame() { Current = new OuterOutput(State) });
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Name = t.Text;
                yield break;
            }
        }

        private class InnerKey : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public InnerKey(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Key = t.Text;
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(PoleToken t)
            {
                State.Peek.Current = new InnerInline(State);
                yield break;
            }
        }

        private class InnerInline : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public InnerInline(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(PoleToken t)
            {
                State.Peek.Current = new InnerName(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Inline.Add(new OutputStructure(t.Text));
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Frames.Push(new VisitorFrame());
                State.Peek.Current = new InnerKey(State);
                yield break;
            }
        }

        private class InnerName : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public InnerName(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                var innerCommand = new CommandStructure(State.Peek.Key, State.Peek.Inline, State.Peek.Name);
                State.Frames.Pop();
                State.Peek.Inline.Add(innerCommand);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.Peek.Name = t.Text;
                yield break;
            }
        }
    }
}
