using ESAPIInfo.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIInfo.Plan
{
    public class PlanInfo
    {
        public static int MaxLengthOfStructureId = 3;
        public static readonly PlanSetupApprovalStatus[] EditablePlanStatuses = { PlanSetupApprovalStatus.UnApproved, PlanSetupApprovalStatus.Rejected };
        public static readonly PlanSetupApprovalStatus[] TreatedPlanStatuses = { PlanSetupApprovalStatus.TreatmentApproved, PlanSetupApprovalStatus.CompletedEarly, PlanSetupApprovalStatus.Completed };
        public static string GetStructuresString(ExternalPlanSetup plan)
        {
            string targetsString = "";
            string oarsString = "";
            if (plan?.StructureSet != null && plan.StructureSet.Structures.Count() > 0)
            {
                foreach (Structure structure in plan.StructureSet.Structures.OrderBy(s => s.Id))
                {
                    if (StructureInfo.SkipStructureDicomTypes.Contains(structure.DicomType.ToUpper()))
                    {
                        continue;
                    }

                    if (StructureInfo.IsTarget(structure))
                    {
                        targetsString += structure.Id.ToUpper() + "/";
                    }
                    else
                    {
                        oarsString += structure.Id.Length > MaxLengthOfStructureId ? structure.Id.ToUpper().Remove(MaxLengthOfStructureId) : structure.Id.ToUpper() + "/";
                    }
                }
            }
            return targetsString + oarsString;
        }

        public void UploadNto(NtoInfo nto)
        {

        }

        private ExternalPlanSetup plan;
        public ExternalPlanSetup Plan
        {
            get => plan;
            set
            {
                plan = value;
                nto = null;
            }
        }
        public IEnumerable<Structure> Structures => Plan?.StructureSet?.Structures;
        public string PlanId => Plan?.Id ?? "";
        public string PatientId => Plan?.Course?.Patient?.Id ?? "";
        public string CourseId => Plan?.Course?.Id ?? "";
        public string CreatorId => Plan?.ApprovalHistory.ToList().OrderBy(ah => ah.ApprovalDateTime).FirstOrDefault(ah => ah.ApprovalStatus == PlanSetupApprovalStatus.PlanningApproved).UserId ?? "";
        public PlanSetupApprovalStatus ApprovalStatus => Plan?.ApprovalStatus ?? PlanSetupApprovalStatus.Unknown;
        public double SingleDose => Plan?.DosePerFraction.Dose ?? .0;
        public int FractionsCount => Plan?.NumberOfFractions ?? 0;
        public string Technique => Plan?.Beams.FirstOrDefault(b => !b.IsSetupField)?.Technique.Id ?? "";
        public string MachineId => Plan?.Beams.FirstOrDefault(b => !b.IsSetupField)?.TreatmentUnit.Id ?? "";
        public int ObjectivesCount => Plan?.OptimizationSetup?.Objectives.Count() ?? 0;
        public string StructuresString => GetStructuresString(Plan);

        private NtoInfo nto;
        public NtoInfo Nto
        {
            get
            {
                if (nto == null)
                {
                    OptimizationNormalTissueParameter parameter = (OptimizationNormalTissueParameter)Plan.OptimizationSetup.Parameters.FirstOrDefault(p => p.GetType() == typeof(OptimizationNormalTissueParameter));
                    if (parameter != null)
                    {
                        nto = new NtoInfo(parameter);
                    }
                }
                return nto;
            }
        }
        
        public bool IsReadyForOptimizerLoad => EditablePlanStatuses.Contains(ApprovalStatus) && MachineId != "" && (Structures?.Count() ?? 0) > 0;
        public bool StructureHasObjectives(Structure structure) => Plan?.OptimizationSetup.Objectives.FirstOrDefault(o => o.Structure == structure) == null ? false : true;
    }
}
