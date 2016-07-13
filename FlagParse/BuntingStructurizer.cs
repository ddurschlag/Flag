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
            State.Current = new Header(State);
        }

        public IEnumerable<Tuple<string, IEnumerable<Structure>>> Structurize(IEnumerable<Token> input)
        {
            return input.SelectMany(Structurize);   //Unsafe to inline Structurize, as it will ignore changes
                                                    //to current state!
        }

        public IEnumerable<Tuple<string, IEnumerable<Structure>>> Structurize(Token input)
        {
            return State.Current.Visit(input);
        }

        [Serializable]
        private class VisitorState
        {
            public StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>> Current;
            public string TemplateName;
        }

        private class Header : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public Header(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                State.TemplateName = t.Text;
                State.Current = new StartA(State);
                yield break;
            }
        }

        private class StartA : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public StartA(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Current = new StartB(State);
                yield break;
            }
        }

        private class StartB : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            public StartB(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                State.Current = new Template(State);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                if (string.IsNullOrWhiteSpace(t.Text))
                    yield break;
                else
                    throw new Exception("Only whitespace is allowed between template name delimiting tildes");
            }
        }

        private class Template : StructureFactory<VisitorState, Tuple<string, IEnumerable<Structure>>>
        {
            private TemplateTokenVisitor V = new TemplateTokenVisitor();
            private Structurizer S = new Structurizer();
            private FlagToken PrevFlag = null;
            private StringToken WhiteSpace = null;
            private IEnumerable<Structure> Result = Enumerable.Empty<Structure>();

            public Template(VisitorState s) : base(s)
            {
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(EndToken t)
            {
                if (PrevFlag != null)
                {
                    Process(PrevFlag);
                    PrevFlag = null;
                    if (WhiteSpace != null)
                    {
                        Process(WhiteSpace);
                        WhiteSpace = null;
                    }
                }
                Process(t);
                State.Current = new Header(State);
                yield return Tuple.Create(State.TemplateName, Result);
            }

            private void Process(Token t)
            {
                Result = Result.Concat(S.Structurize(t));
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(PoleToken t)
            {
                if (PrevFlag != null)
                {
                    Process(PrevFlag);
                    PrevFlag = null;
                    if (WhiteSpace != null)
                    {
                        Process(WhiteSpace);
                        WhiteSpace = null;
                    }
                }
                Process(t);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(StringToken t)
            {
                if (PrevFlag != null)
                {
                    if (!string.IsNullOrWhiteSpace(t.Text))
                    {
                        Process(PrevFlag);
                        if ( WhiteSpace != null )
                        {
                            Process(WhiteSpace);
                            WhiteSpace = null;
                        }
                        PrevFlag = null;
                    } else {
                        WhiteSpace = t;
                        yield break;
                    }
                }
                Process(t);
                yield break;
            }

            public override IEnumerable<Tuple<string, IEnumerable<Structure>>> Visit(FlagToken t)
            {
                if (PrevFlag != null)
                {
                    Process(new EndToken());
                    State.Current = new Header(State);
                    yield return Tuple.Create(State.TemplateName, Result);
                }
                else {
                    PrevFlag = t;
                    yield break;
                }
            }
        }


        private class TemplateTokenVisitor : TokenVisitor<IEnumerable<Token>>
        {
            private FlagToken PrevFlag = null;

            public override IEnumerable<Token> Visit(PoleToken t)
            {
                if (PrevFlag != null)
                {
                    yield return PrevFlag;
                    PrevFlag = null;
                }
                yield return t;
            }

            public override IEnumerable<Token> Visit(EndToken t)
            {
                if (PrevFlag != null)
                {
                    yield return PrevFlag;
                    PrevFlag = null;
                }
                yield return t;
            }

            public override IEnumerable<Token> Visit(FlagToken t)
            {
                if (PrevFlag != null)
                    yield return new EndToken();
                else
                    PrevFlag = t;
            }

            public override IEnumerable<Token> Visit(StringToken t)
            {
                if (PrevFlag != null)
                {
                    yield return PrevFlag;
                    PrevFlag = null;
                }
                yield return t;
            }
        }
    }
}
