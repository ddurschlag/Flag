using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flag.Parse
{
    public static class SequenceComparer
    {
        public static IEqualityComparer<IEnumerable<T>> Of<T>(IEqualityComparer<T> innerComparer)
        {
            return new SequenceComparer<T>(innerComparer);
        }
    }

    public class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public SequenceComparer(IEqualityComparer<T> innerComparer)
        {
            InnerComparer = innerComparer;
        }

        private IEqualityComparer<T> InnerComparer;

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            return x.Zip(y, InnerComparer.Equals).All(b => b);
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            return obj.Select(i => InnerComparer.GetHashCode(i)).Aggregate((a, b) =>
                // Burrowed from System.Tuple, that was stolen from System.Web.Util.HashCodeCombiner
                ((a << 5) + a) ^ b
            );
        }
    }
}
