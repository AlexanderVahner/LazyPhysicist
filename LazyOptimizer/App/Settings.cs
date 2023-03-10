using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class Settings
    {
        public string UserPath { get; set; } = @"%USERPROFILE%\Documents\LazyOptimizer";
        public string SqliteDBName { get; set; } = "data.db";
        public string PlansCacheAppPath { get; set; } = @"D:\vahner\projects\LazyPhysicist\PlansCache\bin\debug\PlansCache.exe";
        public int PlansSelectLimit { get; set; } = 10;
    }
}
