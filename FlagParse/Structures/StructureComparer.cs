using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse.Structures
{
    public class StructureComparer : IEqualityComparer<Structure>
    {
        public bool Equals(Structure x, Structure y)
        {
            return new StructureComparerFactory().Visit(x).Equals(x, y);
        }

        public int GetHashCode(Structure obj)
        {
            return new StructureComparerFactory().Visit(obj).GetHashCode(obj);
        }

        private class StructureComparerFactory : StructureVisitor<IEqualityComparer<Structure>>
        {
            public override IEqualityComparer<Structure> Visit(CommandStructure s)
            {
                return new CommandComparer();
            }

            public override IEqualityComparer<Structure> Visit(OutputStructure s)
            {
                return new OutputComparer();
            }

            private class OutputComparer : StructureComparer<OutputStructure>
            {
                protected override bool Equals(OutputStructure a, OutputStructure b)
                {
                    return a.Text == b.Text;
                }

                protected override int GetHashCode(OutputStructure obj)
                {
                    return obj.Text.GetHashCode();
                }
            }

            private class CommandComparer : StructureComparer<CommandStructure>
            {
                protected override bool Equals(CommandStructure a, CommandStructure b)
                {
                    return a.Key == b.Key && a.Name == b.Name && SequenceComparer.Of(new StructureComparer()).Equals(a.Inline, b.Inline);
                }

                protected override int GetHashCode(CommandStructure obj)
                {
                    return obj.Key.GetHashCode() ^ obj.Name.GetHashCode() ^ SequenceComparer.Of(new StructureComparer()).GetHashCode(obj.Inline);
                }
            }

            private abstract class StructureComparer<T> : IEqualityComparer<Structure>
            where T : Structure
            {
                protected abstract bool Equals(T a, T b);
                protected abstract int GetHashCode(T obj);

                public bool Equals(Structure x, Structure y)
                {
                    return x is T && y is T && Equals((T)x, (T)y);
                }

                public int GetHashCode(Structure obj)
                {
                    if (obj is T)
                        return GetHashCode((T)obj) ^ typeof(T).GetHashCode();
                    throw new Exception("Bad type in GetHashCode");
                }
            }
        }
    }
}
