using ESAPIInfo.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIInfo.Plan
{
    public sealed class PlanInfo : IPlanInfo
    {
        public static int MaxLengthOfStructureId = 3;
        public static readonly PlanSetupApprovalStatus[] EditablePlanStatuses = { PlanSetupApprovalStatus.UnApproved, PlanSetupApprovalStatus.Rejected };
        public static readonly PlanSetupApprovalStatus[] TreatedPlanStatuses = { PlanSetupApprovalStatus.TreatmentApproved, PlanSetupApprovalStatus.CompletedEarly, PlanSetupApprovalStatus.Completed };
        public static string GetStructuresPseudoHash(ExternalPlanSetup plan)
        {
            StringBuilder targetsString = new StringBuilder();
            StringBuilder oarsString = new StringBuilder();
            if (plan?.StructureSet != null && plan.StructureSet.Structures.Count() > 0)
            {
                foreach (Structure structure in plan.StructureSet.Structures.OrderBy(s => s.Id))
                {
                    if (StructureInfo.IsTarget(structure.Id))
                    {
                        targetsString.Append(structure.Id.ToUpper());
                        targetsString.Append("/");
                    }
                    else
                    {
                        oarsString.Append((structure.Id.Length > MaxLengthOfStructureId ? structure.Id.ToUpper().Remove(MaxLengthOfStructureId) : structure.Id.ToUpper()));
                        oarsString.Append("/");
                    }
                }
            }
            return targetsString.Append(oarsString.ToString()).ToString();
        }

        private ExternalPlanSetup plan;
        private DateTime creationDate = default;
        private string creatorId = "";
        private string technique = "";
        private string machineId = "";
        public PlanInfo() { }
        public PlanInfo(ExternalPlanSetup plan)
        {
            Plan = plan;
        }
        public ExternalPlanSetup Plan
        {
            get => plan;
            set
            {
                plan = value;
                nto = null;
                structures = null;
                creationDate = Plan?.ApprovalHistory.OrderBy(ah => ah.ApprovalDateTime).FirstOrDefault(ah => ah.ApprovalStatus == PlanSetupApprovalStatus.UnApproved).ApprovalDateTime ?? default;
                creatorId = Plan?.ApprovalHistory.OrderBy(ah => ah.ApprovalDateTime).FirstOrDefault(ah => ah.ApprovalStatus == PlanSetupApprovalStatus.UnApproved).UserId ?? "";
                technique = "";
                machineId = "";
            }
        }
        public bool IsAssigned => plan != null;
        public string PlanId => Plan?.Id ?? "";
        public Patient Patient => Plan?.Course?.Patient;
        public string PatientId => Plan?.Course?.Patient?.Id ?? "";
        public string CourseId => Plan?.Course?.Id ?? "";
        public DateTime CreationDate => creationDate;
        public string CreatorId => creatorId;

        public PlanSetupApprovalStatus ApprovalStatus => Plan?.ApprovalStatus ?? PlanSetupApprovalStatus.Unknown;
        public double SingleDose => Plan?.DosePerFraction.Dose ?? .0;
        public int FractionsCount => Plan?.NumberOfFractions ?? 0;
        public int ObjectivesCount => Plan?.OptimizationSetup?.Objectives.Count() ?? 0;
        public string StructuresPseudoHash => GetStructuresPseudoHash(Plan);
        public bool IsReadyForOptimizerLoad => EditablePlanStatuses.Contains(ApprovalStatus) && MachineId != "" && (Structures?.Count() ?? 0) > 0;
        public bool StructureHasObjectives(Structure structure) => (Plan?.OptimizationSetup.Objectives.FirstOrDefault(o => o.Structure == structure)) != null;
        public string Technique
        {
            get
            {
                if (technique == "")
                {
                    technique = Plan?.Beams.FirstOrDefault(b => !b.IsSetupField)?.Technique.Id ?? "";
                }
                return technique;
            }
        }
        public string MachineId
        {
            get
            {
                if (machineId == "")
                {
                    machineId = Plan?.Beams.FirstOrDefault(b => !b.IsSetupField)?.TreatmentUnit.Id ?? "";
                }
                return machineId;
            }
        }
        private List<StructureInfo> structures;
        public List<StructureInfo> Structures
        {
            get
            {
                if (structures == null)
                {
                    structures = new List<StructureInfo>();
                    if (Plan?.StructureSet != null)
                    {
                        foreach (Structure s in Plan.StructureSet.Structures)
                        {
                            structures.Add(new StructureInfo(s));
                        }
                    }
                }
                return structures;
            }
        }

        private NtoInfo nto;
        public NtoInfo Nto
        {
            get
            {
                if (nto == null)
                {
                    OptimizationNormalTissueParameter parameter = (OptimizationNormalTissueParameter)Plan.OptimizationSetup.Parameters.FirstOrDefault(p => p.GetType() == typeof(OptimizationNormalTissueParameter));
                    nto = new NtoInfo(parameter);
                }
                return nto;
            }
        }
    }
}
