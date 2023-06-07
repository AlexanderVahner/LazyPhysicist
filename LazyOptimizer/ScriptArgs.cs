using System.Windows;
using VMS.TPS.Common.Model.API;

namespace ScriptArgsNameSpace
{
    public class ScriptArgs
    {
        public User CurrentUser { get; set; }
        public ExternalPlanSetup Plan { get; set; }
        public Window Window { get; set; }
    }
}
