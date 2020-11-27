using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier
{
    public class SimpleLogicChain
    {
        public readonly HashSet<string> reqs;

        public static SimpleLogicChain Empty => new SimpleLogicChain(new string[0]);

        public SimpleLogicChain(IEnumerable<string> rs)
        {
            reqs = new HashSet<string>(rs);
        }

        public SimpleLogicChain(AdditiveLogicChain chain)
        {
            reqs = new HashSet<string>(chain.misc);
        }

        public bool IsContainedIn(SimpleLogicChain chain)
        {
            return reqs.IsSubsetOf(chain.reqs);
        }

        public bool IsEqual(SimpleLogicChain chain)
        {
            return reqs.SetEquals(chain.reqs);
        }

        public static SimpleLogicChain Plus(SimpleLogicChain c1, SimpleLogicChain c2)
        {
            return new SimpleLogicChain(c1.reqs.Union(c2.reqs));
        }

        public static void OrPlusWith(HashSet<SimpleLogicChain> chains, SimpleLogicChain chain)
        {
            SimpleLogicChain[] translate = chains.Select(c => Plus(c, chain)).ToArray();
            chains.UnionWith(translate);
        }

        public static void OrPlusWith(HashSet<SimpleLogicChain> chains, HashSet<SimpleLogicChain> pchains)
        {
            pchains.ExceptWith(chains);
            foreach (var chain in pchains) OrPlusWith(chains, chain);
        }


        public static HashSet<SimpleLogicChain> Plus(IEnumerable<SimpleLogicChain> c1s, IEnumerable<SimpleLogicChain> c2s)
        {
            return new HashSet<SimpleLogicChain>(
                from c1 in c1s 
                from c2 in c2s 
                select Plus(c1, c2)
                );
        }

        public static HashSet<SimpleLogicChain> Span(List<SimpleLogicChain> chains)
        {
            HashSet<SimpleLogicChain> span = new HashSet<SimpleLogicChain>((int)Math.Pow(2, 16));
            span.Add(Empty);

            lock ((object)LogicSimplifierApp.info)
            {
                LogicSimplifierApp.info.updateStack = chains.Count;
            }

            for (int i = 0; i < chains.Count; i++)
            {
                lock ((object)LogicSimplifierApp.info)
                {
                    LogicSimplifierApp.info.updateDepth = i;
                    LogicSimplifierApp.info.lastPoint = $"Grubs (span {span.Count})";
                }
                var translate = span.Select(c => Plus(c, chains[i])).ToArray();
                span.UnionWith(translate);
            }
            return span;
        }

        public static List<SimpleLogicChain> RemoveSupersets(IEnumerable<SimpleLogicChain> _chains)
        {
            List<SimpleLogicChain> chains = _chains.ToList();
            do
            {
                int? dupChain = null;
                for (int i = 0; i < chains.Count; i++)
                {
                    for (int j = 0; j < chains.Count; j++)
                    {
                        if (i == j) continue;

                        if (chains[i].IsContainedIn(chains[j]))
                        {
                            dupChain = j;
                            break;
                        }
                    }
                    if (dupChain is null) continue;
                    else break;
                }

                if (dupChain is int dChain)
                {
                    chains.RemoveAt(dChain);
                    continue;
                }
                else break;
            }
            while (true);

            return chains;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleLogicChain chain && reqs.SetEquals(chain.reqs);
        }

        public override int GetHashCode()
        {
            return reqs.Aggregate(0, (accum, s) => s.GetHashCode() + accum);
        }
    }
}
