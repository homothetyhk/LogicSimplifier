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

        public bool DependsOn(string waypoint) => waypoints.Contains(waypoint);

        public bool IsContainedIn(AdditiveLogicChain chain)
        {
            return target == chain.target && misc.All(s => chain.misc.Contains(s)) && waypoints.All(w => chain.waypoints.Contains(w));
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
