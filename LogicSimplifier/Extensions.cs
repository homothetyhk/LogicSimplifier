using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicSimplifier
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> NondeterministicUnion<T>(this IEnumerable<IEnumerable<T>> l1, IEnumerable<IEnumerable<T>> l2)
        {
            return
            from a in l1
            from b in l2
            select a.Union(b);
        }
    }
}
