using ESAPIInfo.Structures;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                        oarsString += structure.Id.Length > MaxLengthOfStructureId ? structure.Id.ToUpper().Remove(MaxLengthOfStructureId) : structure.Id.ToUpper();
                    }
                }
            }
            return targetsString + oarsString;
        }
        public PlanInfo()
        {

        }
        public PlanInfo(ExternalPlanSetup plan)
        {
            Plan = plan;
        }
        public void LoadNtoIntoPlan(NtoInfo nto)
        {
            Patient.BeginModifications();
            if (nto != null && Plan != null)
            {
                if (nto.IsAutomatic)
                {
                    Plan.OptimizationSetup.AddAutomaticNormalTissueObjective(nto.Priority);
                }
                else
                {
                    Plan.OptimizationSetup.AddNormalTissueObjective(nto.Priority, nto.DistanceFromTargetBorderInMM, nto.StartDosePercentage, nto.EndDosePercentage, nto.FallOff);
                }
            }
        }

        private void LoadObjective(ObjectiveInfo objective)
        {
            if (Plan == null)
            {
                Logger.Write(this, "Can't load the objective. The Plan is null", LogMessageType.Error);
            }
            else
            {
                if (!IsReadyForOptimizerLoad)
                {
                    Logger.Write(this, "Can't load the objective. The Plan is not unapproved, or it doesn't have beams", LogMessageType.Error);
                }
                else
                {
                    if (objective.Structure == null)
                    {
                        Logger.Write(this, "Can't load the objective. Structure is not defined", LogMessageType.Error);
                    }
                    else
                    {
                        switch (objective.Type)
                        {
                            case ObjectiveType.Point:
                                Plan.OptimizationSetup.AddPointObjective(objective.Structure, objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Volume, objective.Priority);
                                break;
                            case ObjectiveType.Mean:
                                Plan.OptimizationSetup.AddMeanDoseObjective(objective.Structure, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Priority);
                                break;
                            case ObjectiveType.EUD:
                                Plan.OptimizationSetup.AddEUDObjective(objective.Structure, objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.ParameterA, objective.Priority);
                                break;
                            case ObjectiveType.Unknown:
                                Logger.Write(this, "Can't load the objective. Type is unknown.", LogMessageType.Error);
                                break;
                        }
                    }
                }
            }
        }
        public void LoadObjectives(IEnumerable<ObjectiveInfo> objectives, bool onlyEmptyStructures = false)
        {
            if (objectives != null)
            {
                Patient.BeginModifications();
                foreach (ObjectiveInfo objective in objectives)
                {
                    if (onlyEmptyStructures && StructureHasObjectives(objective.Structure))
                    {
                        continue;
                    }
                    LoadObjective(objective);
                }
                Logger.Write(this, "Objectives added.", LogMessageType.Info);
            }
            else
            {
                Logger.Write(this, "Can't load objectives. Collection is null.", LogMessageType.Error);
            }
        }
        

        private ExternalPlanSetup plan;

        public ExternalPlanSetup Plan
        {
            get => plan;
            set
            {
                plan = value;
                nto = null;
                structures = null;
            }
        }
        //public IEnumerable<Structure> Structures => Plan?.StructureSet?.Structures;

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
        public string PlanId => Plan?.Id ?? "";
        public Patient Patient => Plan?.Course?.Patient;
        public string PatientId => Plan?.Course?.Patient?.Id ?? "";
        public string CourseId => Plan?.Course?.Id ?? "";
        public string CreatorId => Plan?.ApprovalHistory.ToList().OrderBy(ah => ah.ApprovalDateTime).FirstOrDefault(ah => ah.ApprovalStatus == PlanSetupApprovalStatus.UnApproved).UserId ?? "";
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
        public bool StructureHasObjectives(Structure structure) => (Plan?.OptimizationSetup.Objectives.FirstOrDefault(o => o.Structure == structure)) != null;
    }
}
