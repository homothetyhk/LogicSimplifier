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

        WaypointData WData;
        LocationData LData;

        public delegate void SolveWaypointsHookHandler(AdditiveLogicChain updateChain, int counter, Stack<AdditiveLogicChain> updateStack,
            Dictionary<string, List<AdditiveLogicChain>> relLogic, Dictionary<string, List<AdditiveLogicChain>> absLogic);
        public SolveWaypointsHookHandler SolveWaypointsHook;

        public delegate void SolveLocationsHookHandler(string loc);
        public SolveLocationsHookHandler SolveLocationsHook;

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
                newAbsLogic = newAbsLogic.Where(l => absLogic[l.target].All(r => !r.IsContainedIn(l))).ToList();

                // monotonicity check -- new relative logic should not be a superset of any existing statements
                newRelLogic = newRelLogic.Where(l => relLogic[l.target].All(r => !r.IsContainedIn(l))).ToList();

                // monotonicity check -- remove any old absolute logic statements that are supersets of new statements, and add new statements
                foreach (var absChain in newAbsLogic)
                {
                    List<AdditiveLogicChain> temp = absLogic[absChain.target].Where(r => !absChain.IsContainedIn(r)).ToList();
                    temp.Add(absChain);
                    absLogic[absChain.target] = temp;
                    updates.Push(absChain);
                }

                // monotonicity check -- remove any old relative logic statements that are supersets of new statements, and add new statements
                foreach (var relChain in newRelLogic)
                {
                    List<AdditiveLogicChain> temp = relLogic[relChain.target].Where(r => !relChain.IsContainedIn(r)).ToList();
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

        public List<AdditiveLogicChain> RemoveSupersets(List<AdditiveLogicChain> chains)
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

                        if (chains[i].IsContainedIn(chains[j]))
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
