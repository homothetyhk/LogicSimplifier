using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LogicSimplifier
{
    public partial class LogicSimplifierApp : Form
    {
        public static LogicSimplifierApp instance;

        public LogicSimplifierApp()
        {
            instance = this;
            InitializeComponent();
            XmlLoader.Load();
            BaseLogicCalculator.Setup(XmlLoader.waypoints, XmlLoader.macros);
            SetUpWaypointComboBox();
            SetUpSettingsCheckList();
        }

        private void SetUpSettingsCheckList()
        {
            settingsCheckList.Items.AddRange(XmlLoader.Settings);
        }

        private void SetUpWaypointComboBox()
        {
            waypointComboBox.Items.AddRange(XmlLoader.waypoints.WaypointNames);
            waypointComboBox.SelectedIndex = 0;
        }

        public static void SendError(string s)
        {
            MessageBox.Show(s, "Error", MessageBoxButtons.OK);
        }

        private Dictionary<string, bool> GetSelectedSettings()
        {
            if (!globalSettingsToggle.Checked) return null;
            else return settingsCheckList.Items.Cast<string>().ToDictionary(i => i, i => settingsCheckList.CheckedItems.Contains(i));
        }

        Thread LSThread;
        private void buttonCompute_Click(object sender, EventArgs _)
        {
            if (!(waypointComboBox.SelectedItem is string waypoint) || !XmlLoader.waypoints.IsWaypoint(waypoint))
            {
                SendError("You must select a valid starting waypoint.");
                return;
            }

            buttonCompute.Enabled = false;
            LogicSimplifier ls = new LogicSimplifier(XmlLoader.waypoints, XmlLoader.locations, GetSelectedSettings());
            ls.SolveWaypointsHook += UpdateInfoData;
            ls.SolveLocationsHook += UpdateInfoString;
            ls.SolveGrubsHook += UpdateGrubInfo;
            stopwatch = new Stopwatch();
            stopwatch.Start();
            LSThread = new Thread(() => 
            {
                try
                {
                    ls.SimplifyWaypoints(waypoint);
                    ls.SimplifyLocations();
                    ls.SimplifyGrubs();

                    buttonCompute.BeginInvoke((MethodInvoker)delegate
                    {
                        XmlLoader.Save("waypoints.xml", ls.SerializeLogic(ls.newWaypointLogic));
                        XmlLoader.Save("locations.xml", ls.SerializeLogic(ls.newLocationLogic));
                        XmlLoader.Save("grubs.xml", ls.SerializeLogic(ls.newGrubLogic));
                        MessageBox.Show("Logic simplification successful.");
                        buttonCompute.Enabled = true;
                    });
                }
                catch (Exception e)
                {
                    buttonCompute.BeginInvoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Error during simplification:\n" + e);
                        throw e;
                    });
                }
                buttonCompute.BeginInvoke((MethodInvoker)delegate
                {
                    buttonCompute.Enabled = true;
                });
            });
            LSThread.Start();
        }


        private void globalSettingsToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (globalSettingsToggle.Checked)
            {
                settingsCheckList.Enabled = true;
            }
            else
            {
                settingsCheckList.Enabled = false;
            }
        }

        Stopwatch stopwatch;
        public static (int updateDepth, int updateStack, int relLogic, int absLogic, string lastPoint) info;

        private void UpdateInfoData(AdditiveLogicChain updateChain, int counter, Stack<AdditiveLogicChain> updateStack,
            Dictionary<string, List<AdditiveLogicChain>> relLogic, Dictionary<string, List<AdditiveLogicChain>> absLogic)
        {
            lock ((object)info)
            {
                info = (
                    counter, 
                    updateStack.Count, 
                    relLogic.Aggregate(0, (accum, k1) => k1.Value.Count + accum), 
                    absLogic.Aggregate(0, (accum, k1) => k1.Value.Count + accum), 
                    updateChain.target);
            }
        }

        private void UpdateInfoString(string s)
        {
            lock ((object)info)
            {
                info.updateStack = 0;
                info.lastPoint = s;
            }
        }

        private void UpdateGrubInfo(int i)
        {
            lock ((object)info)
            {
                info.lastPoint = $"{i} grub(s)";
            }
        }

        private void UpdateInfoPanel()
        {
            if (LSThread is null || !LSThread.IsAlive) return;

            labelTimeElapsed.Text = $"Time elapsed: {stopwatch.Elapsed}";
            lock ((object)info)
            {
                labelUpdateDepth.Text = $"Update depth: {info.updateDepth}";
                labelUpdateStack.Text = $"Update stack: {info.updateStack}";
                labelRelLogicStatements.Text = $"Relative logic statements: {info.relLogic}";
                labelAbsLogicStatements.Text = $"Absolute logic statments: {info.absLogic}";
                labelLastWaypoint.Text = $"Most recent: {info.lastPoint ?? string.Empty}";
            }   
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateInfoPanel();
        }
    }
}
