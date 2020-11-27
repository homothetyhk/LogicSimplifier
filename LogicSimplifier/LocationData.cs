using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier
{
    public struct LocationDef
    {
        public string logic;
        public string pool;
    }

    public class LocationData
    {
        Dictionary<string, LocationDef> _items;
        public string[] LocationNames;

        public LocationData(Dictionary<string, LocationDef> items)
        {
            _items = items;
            LocationNames = items.Keys.ToArray();
        }

        public LocationDef GetLocationDef(string name)
        {
            return _items[name];
        }

        public IEnumerable<string> Filter(Func<LocationDef, bool> func)
        {
            return _items.Keys.Where(i => func(_items[i]));
        }
    }
}
