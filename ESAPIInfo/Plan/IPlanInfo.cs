using ESAPIInfo.Structures;
using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;

namespace ESAPIInfo.Plan
{
    public interface IPlanInfo
    {
        bool StructureHasObjectives(Structure structure);
        ExternalPlanSetup Plan { get; }
        bool IsAssigned { get; }
        string PlanId { get; }
        Patient Patient { get; }
        string PatientId { get; }
        string CourseId { get; }
        DateTime CreationDate { get; }
        string CreatorId { get; }
        string TargetId { get; }
        PlanSetupApprovalStatus ApprovalStatus { get; }
        double SingleDose { get; }
        int FractionsCount { get; }
        int ObjectivesCount { get; }
        string StructuresPseudoHash { get; }
        bool IsReadyForOptimizerLoad { get; }
        string Technique { get; }
        string MachineId { get; }
        List<StructureInfo> Structures { get; }
        NtoInfo Nto { get; }
    }
}
