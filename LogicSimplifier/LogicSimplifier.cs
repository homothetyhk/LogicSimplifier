using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogicSimplifier
{
    public class LogicSimplifier
    {
        Dictionary<string, AdditiveLogicChain[]> origWaypointLogic;
        public Dictionary<string, AdditiveLogicChain[]> newWaypointLogic;
        Dictionary<string, AdditiveLogicChain[]> origLocationLogic;
        public Dictionary<string, AdditiveLogicChain[]> newLocationLogic;
        public Dictionary<string, AdditiveLogicChain[]> newGrubLogic;


        WaypointData WData;
        LocationData LData;

        public delegate void SolveWaypointsHookHandler(AdditiveLogicChain updateChain, int counter, Stack<AdditiveLogicChain> updateStack,
            Dictionary<string, List<AdditiveLogicChain>> relLogic, Dictionary<string, List<AdditiveLogicChain>> absLogic);
        public SolveWaypointsHookHandler SolveWaypointsHook;

        public delegate void SolveLocationsHookHandler(string loc);
        public SolveLocationsHookHandler SolveLocationsHook;

        public delegate void SolveGrubsHookHandler(int counter);
        public SolveGrubsHookHandler SolveGrubsHook;

        public LogicSimplifier(WaypointData wData, LocationData lData, Dictionary<string, bool> settings = null)
        {
            WData = wData;
            LData = lData;
            origWaypointLogic = WData.WaypointNames.ToDictionary(w => w,
                w => SubstituteSettings(ChainsFromWaypoint(w), settings ?? new Dictionary<string, bool>()));
            origLocationLogic = LData.LocationNames.ToDictionary(l => l,
                l => SubstituteSettings(ChainsFromLocation(l), settings ?? new Dictionary<string, bool>()));
        }


        public void SimplifyWaypoints(string startPoint)
        {
            Dictionary<string, List<AdditiveLogicChain>> relLogic = origWaypointLogic.ToDictionary(c => c.Key, c => c.Value.ToList());
            Dictionary<string, List<AdditiveLogicChain>> absLogic = WData.WaypointNames.ToDictionary(c => c, c => new List<AdditiveLogicChain>());
            int counter = 0;

            Stack<AdditiveLogicChain> updates = new Stack<AdditiveLogicChain>();

            AdditiveLogicChain startChain = new AdditiveLogicChain
            {
                target = startPoint,
                misc = new string[0],
                waypoints = new string[0]
            };

            updates.Push(startChain);
            absLogic[startPoint].Add(startChain);

            while (updates.Any())
            {
                counter++;
                var upChain = updates.Pop();
                SolveWaypointsHook.Invoke(upChain, counter, updates, relLogic, absLogic);


                IEnumerable<AdditiveLogicChain> updatedLogic = relLogic.Values.SelectMany(l => l).Where(l => l.DependsOn(upChain.target))
                    .Select(l => l.Literalize(upChain));
                List<AdditiveLogicChain> newRelLogic = updatedLogic.Where(l => !l.literal).ToList();
                List<AdditiveLogicChain> newAbsLogic = updatedLogic.Where(l => l.literal).ToList();

                // monotonicity check -- no new absolute logic statement should be a superset of another new statement
                newAbsLogic = RemoveSupersets(newAbsLogic);
                
                // monotonicity check -- no new relative logic statement should be a superset of another new statement
                newRelLogic = RemoveSupersets(newRelLogic);

                // monotonicity check -- new absolute logic should not be a superset of any existing statements
                newAbsLogic = newAbsLogic.Where(l => absLogic[l.target].All(r => !r.IsContainedInT(l))).ToList();

                // monotonicity check -- new relative logic should not be a superset of any existing statements
                newRelLogic = newRelLogic.Where(l => relLogic[l.target].All(r => !r.IsContainedInT(l))).ToList();

                // monotonicity check -- remove any old absolute logic statements that are supersets of new statements, and add new statements
                foreach (var absChain in newAbsLogic)
                {
                    List<AdditiveLogicChain> temp = absLogic[absChain.target].Where(r => !absChain.IsContainedInT(r)).ToList();
                    temp.Add(absChain);
                    absLogic[absChain.target] = temp;
                    updates.Push(absChain);
                }

                // monotonicity check -- remove any old relative logic statements that are supersets of new statements, and add new statements
                foreach (var relChain in newRelLogic)
                {
                    List<AdditiveLogicChain> temp = relLogic[relChain.target].Where(r => !relChain.IsContainedInT(r)).ToList();
                    temp.Add(relChain);
                    relLogic[relChain.target] = temp;
                    //updates.Push(relChain);
                }
                
            }

            newWaypointLogic = absLogic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public void SimplifyLocations()
        {
            newLocationLogic = origLocationLogic.ToDictionary(kvp => { SolveLocationsHook.Invoke(kvp.Key); return kvp.Key; },
                kvp => RemoveSupersets(kvp.Value.SelectMany(SubstituteWaypoints).ToList()).ToArray());
        }

        public void SimplifyGrubs()
        {
            string[] msjp = new string[]
            {
                "Shop",
                "Dreamer",
                "Skill",
                "Charm",
                "Key",
                "Mask",
                "Vessel",
                "Notch",
                "Ore",
                "Geo",
                "Relic",
                "Egg",
                "Stag"
            };

            Dictionary<string, SimpleLogicChain[]> oldGrubLogic = newLocationLogic
                .Where(kvp => msjp.Contains(LData.GetLocationDef(kvp.Key).pool))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(c => new SimpleLogicChain(c)).ToArray());
            this.newGrubLogic = new Dictionary<string, AdditiveLogicChain[]>();

            Dictionary<SimpleLogicChain, int> grubCounts = new Dictionary<SimpleLogicChain, int>();

            int GrubCount(SimpleLogicChain chain)
            {
                if (grubCounts.TryGetValue(chain, out int count))
                {
                    return count;
                }
                else
                {
                    count = oldGrubLogic.Count(kvp => kvp.Value.Any(c => c.IsContainedIn(chain)));
                    grubCounts[chain] = count;
                    return count;
                }
            }

            bool GrubContained(SimpleLogicChain subChain, SimpleLogicChain superChain)
            {
                return subChain.IsContainedIn(superChain) && GrubCount(subChain) == GrubCount(superChain);
            }

            List<SimpleLogicChain> RemoveGrubSupersets(List<SimpleLogicChain> chains)
            {
                chains = chains.ToList();
                do
                {
                    int? dupChain = null;
                    for (int i = 0; i < chains.Count; i++)
                    {
                        for (int j = 0; j < chains.Count; j++)
                        {
                            if (i == j) continue;

                            if (GrubContained(chains[i], chains[j]))
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

            List<SimpleLogicChain> primitives = RemoveGrubSupersets(oldGrubLogic.Values.SelectMany(s => s).ToList());
            //HashSet<SimpleLogicChain> runningGrubLogic = new HashSet<SimpleLogicChain>();
            //runningGrubLogic.Add(SimpleLogicChain.Empty);

            primitives.Add(SimpleLogicChain.Empty);
            Queue<HashSet<SimpleLogicChain>> iterate = new Queue<HashSet<SimpleLogicChain>>
                (primitives.Select(p => new HashSet<SimpleLogicChain> { p }));

            while (iterate.Count > 1)
            {
                var p1 = iterate.Dequeue();
                var p2 = iterate.Dequeue();
                SimpleLogicChain.OrPlusWith(p1, p2);
                p1 = new HashSet<SimpleLogicChain>(
                        p1
                        .GroupBy(c => GrubCount(c))
                        .SelectMany(g => SimpleLogicChain.RemoveSupersets(g)));
                iterate.Enqueue(p1);

                lock ((object)LogicSimplifierApp.info)
                {
                    LogicSimplifierApp.info.updateDepth = iterate.Count;
                    LogicSimplifierApp.info.updateStack = primitives.Count;
                    LogicSimplifierApp.info.lastPoint = $"Grubs logic: {iterate.Aggregate(0, (accum, u) => accum + u.Count)}";
                }
            }
            HashSet<SimpleLogicChain> runningGrubLogic = iterate.Dequeue();

            /*
            for (int i = 0; i < primitives.Count; i++)
            {
                if (!runningGrubLogic.Contains(primitives[i]))
                {
                    SimpleLogicChain.OrPlusWith(runningGrubLogic, primitives[i]);
                    runningGrubLogic = new HashSet<SimpleLogicChain>(
                        runningGrubLogic
                        .GroupBy(c => GrubCount(c))
                        .SelectMany(g => SimpleLogicChain.RemoveSupersets(g)));
                }

                lock ((object)LogicSimplifierApp.info)
                {
                    LogicSimplifierApp.info.updateDepth = i+1;
                    LogicSimplifierApp.info.updateStack = primitives.Count;
                    LogicSimplifierApp.info.lastPoint = $"Grubs logic: {runningGrubLogic.Count}";
                }
            }
            */

            List<(int, List<SimpleLogicChain>)> sortedChains = runningGrubLogic
                .GroupBy(c => GrubCount(c))
                .Select(g => (g.Key, g.ToList()))
                .OrderBy(g => -g.Key).ToList();

            List<SimpleLogicChain> accumChains = new List<SimpleLogicChain>();
            List<(int, List<SimpleLogicChain>)> aggregatedChains = new List<(int, List<SimpleLogicChain>)>();

            foreach (var pair in sortedChains)
            {
                accumChains.AddRange(pair.Item2);
                accumChains = SimpleLogicChain.RemoveSupersets(accumChains);
                aggregatedChains.Add((pair.Item1, accumChains.ToList()));
            }

            aggregatedChains.Reverse();
            newGrubLogic = aggregatedChains.ToDictionary(p => p.Item1.ToString(), p =>
            {
                return p.Item2.Select(c => new AdditiveLogicChain
                {
                    target = p.Item1.ToString(),
                    waypoints = new string[0],
                    misc = c.reqs.ToArray()
                }).ToArray();
            });

            {
                string s = string.Join("\n", newGrubLogic.Select(kvp => (kvp.Value.Length, kvp.Key)).OrderBy(p => p)
                    .Select(p => $"{p.Item2}, {p.Item1}"));
                LogicSimplifierApp.SendError(s);
            }
        }

        public List<AdditiveLogicChain> RemoveSupersets(List<AdditiveLogicChain> chains, bool considerTargets = true)
        {
            chains = chains.ToList();
            do
            {
                AdditiveLogicChain? dupChain = null;
                for (int i = 0; i < chains.Count; i++)
                {
                    for (int j = 0; j < chains.Count; j++)
                    {
                        if (i == j) continue;

                        if (considerTargets ? chains[i].IsContainedInT(chains[j]) : chains[i].IsContainedIn(chains[j]))
                        {
                            dupChain = chains[j];
                            break;
                        }
                    }
                    if (dupChain is null) continue;
                    else break;
                }

                if (dupChain is AdditiveLogicChain dChain)
                {
                    chains.Remove(dChain);
                    continue;
                }
                else break;
            }
            while (true);

            return chains;
        }

        public List<AdditiveLogicChain> RemoveMiscDuplicates(List<AdditiveLogicChain> chains)
        {
            chains = chains.ToList();
            do
            {
                AdditiveLogicChain? dupChain = null;
                for (int i = 0; i < chains.Count; i++)
                {
                    for (int j = 0; j < chains.Count; j++)
                    {
                        if (i == j) continue;

                        if (chains[i].MiscEqual(chains[j]))
                        {
                            dupChain = chains[j];
                            break;
                        }
                    }
                    if (dupChain is null) continue;
                    else break;
                }

                if (dupChain is AdditiveLogicChain dChain)
                {
                    chains.Remove(dChain);
                    continue;
                }
                else break;
            }
            while (true);

            return chains;
        }


        public AdditiveLogicChain[] SubstituteSettings(AdditiveLogicChain[] chains, Dictionary<string, bool> settings)
        {
            List<AdditiveLogicChain> builder = new List<AdditiveLogicChain>();
            foreach (var chain in chains)
            {
                if (chain.misc.All(s => !settings.TryGetValue(s, out bool val) || val))
                {
                    builder.Add(new AdditiveLogicChain
                    {
                        target = chain.target,
                        waypoints = chain.waypoints,
                        misc = chain.misc.Except(settings.Keys).ToArray()
                    });
                }
            }

            return builder.ToArray();
        }

        public AdditiveLogicChain[] ChainsFromWaypoint(string name)
        {
            return BaseLogicCalculator.ParseToAdditiveChains(name, WData.GetWaypoint(name).logic);
        }

        public AdditiveLogicChain[] ChainsFromLocation(string name)
        {
            return BaseLogicCalculator.ParseToAdditiveChains(name, LData.GetLocationDef(name).logic);
        }

        public AdditiveLogicChain[] SubstituteWaypoints(AdditiveLogicChain chain)
        {
            IEnumerable<IEnumerable<string>> miscReqs = new string[][] { chain.misc };

            foreach (string waypoint in chain.waypoints)
            {
                miscReqs = miscReqs.NondeterministicUnion(newWaypointLogic[waypoint].Select(c => (IEnumerable<string>)c.misc));
            }

            return miscReqs.Select(m => new AdditiveLogicChain
            {
                target = chain.target,
                waypoints = new string[0],
                misc = m.ToArray()
            }).ToArray();
        }

        public string PrintLogic(AdditiveLogicChain[] chains)
        {
            return "\n" + string.Join("\n| ", chains.Select(c => c.GetLogicString()).ToArray()) + "\n";
        }

        public string SerializeLogic(Dictionary<string, AdditiveLogicChain[]> logic)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true });
            xw.WriteStartDocument();
            xw.WriteStartElement("randomizer");
            foreach (var kvp in logic)
            {
                xw.WriteStartElement("item");
                xw.WriteAttributeString("name", kvp.Key);
                xw.WriteElementString("logic", PrintLogic(kvp.Value));
                xw.WriteEndElement();
            }
            xw.WriteEndDocument();
            xw.Flush();
            return sb.ToString();
        }
    }
}
