﻿using LazyPhysicist.Common;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIInfo.Plan
{
    public interface IObjectiveInfo
    {
        ObjectiveType Type { get; set; }
        Structure Structure { get; set; }
        string StructureId { get; set; }
        double Priority { get; set; }
        Operator Operator { get; set; }
        double Dose { get; set; }
        double Volume { get; set; }
        double ParameterA { get; set; }
    }
}