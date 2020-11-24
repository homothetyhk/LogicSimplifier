using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LogicSimplifier
{
    public struct Waypoint
    {
        public string logic;
    }

    public class WaypointData
    {
        private Dictionary<string, Waypoint> _waypoints;
        public string[] WaypointNames;

        public WaypointData(Dictionary<string, Waypoint> waypoints)
        {
            _waypoints = waypoints;
            WaypointNames = waypoints.Keys.ToArray();
        }

        public Waypoint GetWaypoint(string name)
        {
            return _waypoints[name];
        }

        public bool IsWaypoint(string name)
        {
            return _waypoints.ContainsKey(name);
        }
    }
}
