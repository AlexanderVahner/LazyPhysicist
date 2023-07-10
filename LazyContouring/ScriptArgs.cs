using System.Windows;
using VMS.TPS.Common.Model.API;

namespace ScriptArgsNameSpace
{
    public class ScriptArgs
    {
        public User CurrentUser { get; set; }
        public Patient Patient { get; set; }
        public StructureSet StructureSet { get; set; }
        public Window Window { get; set; }
    }
}
