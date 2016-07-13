using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Instructions
{
    public class InstructionSequenceComparer : IEqualityComparer<IEnumerable<Instruction>>
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
                        return a.Key == b.Key && new InstructionSequenceComparer().Equals(a.Instructions, b.Instructions);
                    }

                    protected override int GetHashCode(CallInlineInstruction obj)
                    {
                        return obj.Key.GetHashCode() ^ new InstructionSequenceComparer().GetHashCode(obj.Instructions);
                    }
                }

                private class LoopInlineComparer : InstructionComparer<LoopInlineInstruction>
                {
                    protected override bool Equals(LoopInlineInstruction a, LoopInlineInstruction b)
                    {
                        return new InstructionSequenceComparer().Equals(a.Instructions, b.Instructions);
                    }

                    protected override int GetHashCode(LoopInlineInstruction obj)
                    {
                        return new InstructionSequenceComparer().GetHashCode(obj.Instructions);
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
                return new InstructionComparerFactory().Visit(x).Equals(x, y);
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
}
