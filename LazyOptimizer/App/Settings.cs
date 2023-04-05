using Common;
using LazyPhysicist.Common;
using System;
using System.IO;

namespace LazyOptimizer.App
{
    public class Settings : Notifier
    {
        private static string userPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\LazyOptimizer");
        private const string settingsFileName = "settings.xml";
        private static string settingsPath = $"{userPath}\\{settingsFileName}";
        private const string sqliteDBName = "data.db";
        public static Settings ReadSettings()
        {
            Settings settings = null;

            if (!FileSystem.CheckPathOrCreate(userPath))
            {
                Logger.Write(null, $"Can't make user path \"{userPath}\".", LogMessageType.Error);
            }
            else
            {
                settingsPath = $"{userPath}\\{settingsFileName}";
                if (File.Exists(settingsPath))
                {
                    Xml.ReadXmlToObject(settingsPath, ref settings);
                }
                else
                {
                    settings = new Settings();
                }
            }

            return settings;
        }


        private string plansCacheAppPath = @"PlansCache.exe";
        private bool plansCacheVerboseMode = true;
        private bool plansCacheRecheckAllPatients = false;

        private int plansSelectLimit = 10;
        private bool matchMachine = true;
        private bool matchTechnique = true;
        private bool loadNto = true;
        private string defaultPrioritySetValue = "30";
        private string lastPriorityValue = "-1";

        private bool debugMode = false;

        public void Save()
        {
            Xml.WriteXmlFromObject(settingsPath, this);
        }

        public string UserPath { get => userPath; set => SetProperty(ref userPath, value); }
        public string SqliteDbPath => $"{userPath}\\{sqliteDBName}";
        public string PlansCacheAppPath { get => plansCacheAppPath; set => SetProperty(ref plansCacheAppPath, value); }
        public bool PlansCacheVerboseMode { get => plansCacheVerboseMode; set => SetProperty(ref plansCacheVerboseMode, value); }
        public bool PlansCacheRecheckAllPatients { get => plansCacheRecheckAllPatients; set => SetProperty(ref plansCacheRecheckAllPatients, value); }


        public int PlansSelectLimit { get => plansSelectLimit; set => SetProperty(ref plansSelectLimit, value); }
        public bool MatchMachine { get => matchMachine; set => SetProperty(ref matchMachine, value); }
        public bool MatchTechnique { get => matchTechnique; set => SetProperty(ref matchTechnique, value); }
        public bool LoadNto { get => loadNto; set => SetProperty(ref loadNto, value); }
        public string DefaultPrioritySetValue { get => defaultPrioritySetValue; set => SetProperty(ref defaultPrioritySetValue, value); }
        public string LastPriorityValue { get => lastPriorityValue; set => SetProperty(ref lastPriorityValue, value); }

        public bool DebugMode { get => debugMode; set => SetProperty(ref debugMode, value); }


    }
}
