using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer
{
    public class ScriptArgs
    {
        public User CurrentUser { get; set; }
        public ExternalPlanSetup Plan { get; set; }
        public Window Window { get; set; }
    }
}
