using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace ScriptArgsNameSpace
{
    public sealed class ScriptArgs
    {
        public Patient Patient { get; set; }
        public ExternalPlanSetup Plan { get; set; }
    }
}
