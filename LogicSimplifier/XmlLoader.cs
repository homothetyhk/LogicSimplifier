using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace LogicSimplifier
{
    public static class XmlLoader
    {
        public static string[] Settings;
        public static WaypointData waypoints;
        public static LocationData locations;
        public static Dictionary<string, string> macros;
        public static Dictionary<string, string> combos;


        public static void Load()
        {
            try
            {
                XmlDocument settingsDoc = new XmlDocument();
                settingsDoc.Load(Path.Combine(inputDirectory, "settings.xml"));
                Settings = settingsDoc.SelectNodes("randomizer/setting").Cast<XmlNode>().Select(x => x.InnerText).ToArray();

                XmlDocument waypointDoc = new XmlDocument();
                waypointDoc.Load(Path.Combine(inputDirectory, "waypoints.xml"));
                waypoints = new WaypointData(waypointDoc.SelectNodes("randomizer/item").Cast<XmlNode>()
                    .ToDictionary(x => x.Attributes["name"].Value, x => new Waypoint
                    {
                        logic = x.ChildNodes.Cast<XmlNode>().First(c => c.LocalName.Contains("ogic")).InnerText
                    }));

                XmlDocument locationDoc = new XmlDocument();
                locationDoc.Load(Path.Combine(inputDirectory, "locations.xml"));
                locations = new LocationData(locationDoc.SelectNodes("randomizer/item").Cast<XmlNode>()
                    .ToDictionary(x => x.Attributes["name"].Value, x => new LocationDef
                    {
                        logic = x.ChildNodes.Cast<XmlNode>().First(c => c.LocalName.Contains("ogic")).InnerText,
                        pool = x.ChildNodes.Cast<XmlNode>().First(c => c.LocalName == "pool").InnerText,
                    }));

                XmlDocument macroDoc = new XmlDocument();
                macroDoc.Load(Path.Combine(inputDirectory, "macros.xml"));
                macros = macroDoc.SelectNodes("randomizer/macro").Cast<XmlNode>()
                    .ToDictionary(x => x.Attributes["name"].Value, x => x.InnerText);
            }
            catch (FileNotFoundException e)
            {
                LogicSimplifierApp.SendError("Input files expected at " + inputDirectory + " were not found.\n" + e);
                throw e;
            }
            catch (Exception e)
            {
                LogicSimplifierApp.SendError("Malformatted or unusable input files!\n" + e);
                throw e;
            }
        }

        public static void Save(string name, string contents)
        {
            FileInfo file = new FileInfo(Path.Combine(outputDirectory, name));
            if (!file.Directory.Exists) file.Directory.Create();
            File.WriteAllText(file.FullName, contents);
        }


        public static string baseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string inputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Input");
        public static string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        public static string[] filenames = new string[] 
        { 
            "waypoints.xml", 
            "locations.xml", 
            "macros.xml", 
            "combos.xml" 
        };
    }
}
