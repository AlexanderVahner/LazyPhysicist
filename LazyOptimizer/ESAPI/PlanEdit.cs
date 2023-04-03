using ESAPIInfo.Plan;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.ESAPI
{
    internal class PlanEdit
    {
        public static void LoadNtoIntoPlan(PlanInfo plan, NtoInfo nto)
        {
            if (nto != null && plan != null)
            {
                plan.Plan.Course.Patient.BeginModifications();
                if (nto.IsAutomatic)
                {
                    plan.Plan.OptimizationSetup.AddAutomaticNormalTissueObjective(nto.Priority);
                }
                else
                {
                    plan.Plan.OptimizationSetup.AddNormalTissueObjective(nto.Priority, nto.DistanceFromTargetBorderInMM, nto.StartDosePercentage, nto.EndDosePercentage, nto.FallOff);
                }
            }
        }

        private static void LoadObjective(PlanInfo plan, ObjectiveInfo objective)
        {
            if (plan.Plan == null)
            {
                Logger.Write(plan, "Can't load the objective. The Plan is null", LogMessageType.Error);
            }
            else
            {
                if (!plan.IsReadyForOptimizerLoad)
                {
                    Logger.Write(plan, "Can't load the objective. The Plan is not unapproved, or it doesn't have beams", LogMessageType.Error);
                }
                else
                {
                    if (objective.Structure == null)
                    {
                        Logger.Write(plan, "Can't load the objective. Structure is not defined", LogMessageType.Error);
                    }
                    else
                    {

                        switch (objective.Type)
                        {
                            case ObjectiveType.Point:
                                plan.Plan.OptimizationSetup.AddPointObjective(objective.Structure, (OptimizationObjectiveOperator)(int)objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Volume, objective.Priority);
                                break;
                            case ObjectiveType.Mean:
                                plan.Plan.OptimizationSetup.AddMeanDoseObjective(objective.Structure, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.Priority);
                                break;
                            case ObjectiveType.EUD:
                                plan.Plan.OptimizationSetup.AddEUDObjective(objective.Structure, (OptimizationObjectiveOperator)(int)objective.Operator, new DoseValue(objective.Dose, DoseValue.DoseUnit.Gy), objective.ParameterA, objective.Priority);
                                break;
                            case ObjectiveType.Unknown:
                                Logger.Write(plan, "Can't load the objective. Type is unknown.", LogMessageType.Error);
                                break;
                        }
                    }
                }
            }
        }
        public static void LoadObjectives(PlanInfo plan, IEnumerable<ObjectiveInfo> objectives, bool onlyEmptyStructures = false)
        {
            if (objectives != null)
            {
                plan.Plan.Course.Patient.BeginModifications();
                foreach (ObjectiveInfo objective in objectives)
                {
                    if (onlyEmptyStructures && plan.StructureHasObjectives(objective.Structure))
                    {
                        continue;
                    }
                    LoadObjective(plan, objective);
                }
                Logger.Write(plan, "Objectives added.", LogMessageType.Info);
            }
            else
            {
                Logger.Write(plan, "Can't load objectives. Collection is null.", LogMessageType.Error);
            }
        }
    }
}
