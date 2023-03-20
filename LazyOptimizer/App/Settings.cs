using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class Settings : Notifier
    {
        private string userPath = @"%APPDATA%\LazyOptimizer";
        private string sqliteDBName = "data.db";
        private string plansCacheAppPath = @"..\..\..\PlansCache\bin\debug\PlansCache.exe";
        private bool plansCacheVerboseMode = true;
        private bool plansCacheRecheckAllPatients = false;

        private int plansSelectLimit = 10;
        private bool matchMachine = false;
        private bool matchTechnique = false;
        private bool loadNto = false;

        private bool debugMode = false;


        public string UserPath { get => userPath; set => SetProperty(ref userPath, value); }
        public string SqliteDBName { get => sqliteDBName; set => SetProperty(ref sqliteDBName, value); }
        public string PlansCacheAppPath { get => plansCacheAppPath; set => SetProperty(ref plansCacheAppPath, value); }
        public bool PlansCacheVerboseMode { get => plansCacheVerboseMode; set => SetProperty(ref plansCacheVerboseMode, value); }
        public bool PlansCacheRecheckAllPatients { get => plansCacheRecheckAllPatients; set => SetProperty(ref plansCacheRecheckAllPatients, value); }


        public int PlansSelectLimit { get => plansSelectLimit; set => SetProperty(ref plansSelectLimit, value); }
        public bool MatchMachine { get => matchMachine; set => SetProperty(ref matchMachine, value); }
        public bool MatchTechnique { get => matchTechnique; set => SetProperty(ref matchTechnique, value); }
        public bool LoadNto { get => loadNto; set => SetProperty(ref loadNto, value); }

        public bool DebugMode { get => debugMode; set => SetProperty(ref debugMode, value); }


    }
}
