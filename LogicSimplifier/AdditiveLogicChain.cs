using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier
{
    public struct AdditiveLogicChain
    {
        public string target;
        public string[] waypoints;
        public string[] misc;
        public bool literal => waypoints.Length == 0;

        public static AdditiveLogicChain Empty => new AdditiveLogicChain
        {
            target = string.Empty,
            waypoints = new string[0],
            misc = new string[0]
        };

        public static AdditiveLogicChain Plus(AdditiveLogicChain chain1, AdditiveLogicChain chain2)
        {
            return new AdditiveLogicChain
            {
                target = chain1.target,
                waypoints = chain1.waypoints.Union(chain2.waypoints).ToArray(),
                misc = chain1.misc.Union(chain2.misc).ToArray()
            };
        }

        public static IEnumerable<AdditiveLogicChain> Plus(IEnumerable<AdditiveLogicChain> chains1, IEnumerable<AdditiveLogicChain> chains2)
        {
            HashSet<int> hashes = new HashSet<int>();

            bool checkHash(AdditiveLogicChain c1, AdditiveLogicChain c2)
            {
                int hash = c1.misc.Union(c2.misc).Aggregate(0, (accum, t) => accum + t.GetHashCode());
                if (hashes.Contains(hash)) return false;
                else
                {
                    hashes.Add(hash);
                    return true;
                }
            }


            return
                from c1 in chains1
                from c2 in chains2
                where checkHash(c1, c2)
                select Plus(c1, c2);
        }

        public bool DependsOn(string waypoint) => waypoints.Contains(waypoint);

        // target-sensitive
        public bool IsContainedInT(AdditiveLogicChain chain)
        {
            return target == chain.target && misc.All(s => chain.misc.Contains(s)) && waypoints.All(w => chain.waypoints.Contains(w));
        }

        // ignores target
        public bool IsContainedIn(AdditiveLogicChain chain)
        {
            return misc.All(s => chain.misc.Contains(s)) && waypoints.All(w => chain.waypoints.Contains(w));
        }

        public bool MiscEqual(AdditiveLogicChain chain)
        {
            if (chain.GetHashCode() == GetHashCode())
            {
                return misc.Intersect(chain.misc).Count() == misc.Length;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return misc.Aggregate(0, (accum, t) => accum + t.GetHashCode());
        }


        public AdditiveLogicChain Literalize(AdditiveLogicChain chain)
        {
            string[] reducedWaypoints = waypoints.Except(new string[] { chain.target }).Union(chain.waypoints).ToArray();
            string[] oldMisc = misc;
            string _target = target;

            return new AdditiveLogicChain
            {
                target = _target,
                waypoints = reducedWaypoints,
                misc = oldMisc.Union(chain.misc).ToArray()
            };
        }

        public AdditiveLogicChain[] Literalize(string waypoint, string[][] logic)
        {
            string[] reducedWaypoints = waypoints.Except(new string[] { waypoint }).ToArray();
            string[] oldMisc = misc;
            string _target = target;

            return logic.Select(c =>
            {
                return new AdditiveLogicChain
                {
                    target = _target,
                    waypoints = reducedWaypoints,
                    misc = oldMisc.Union(c).ToArray()
                };
            }).ToArray();
        }

        public AdditiveLogicChain Literalize(string waypoint)
        {
            this.waypoints = waypoints.Except(new string[] { waypoint }).ToArray();
            return this;
        }

        public string GetLogicString()
        {
            string s1 = string.Join(" + ", waypoints);
            string s2 = string.Join(" + ", misc);
            if (string.IsNullOrEmpty(s1)) return s2;
            if (string.IsNullOrEmpty(s2)) return s1;
            return s1 + " + " + s2;
        }
    }
}
