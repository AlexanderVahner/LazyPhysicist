using ESAPIInfo.Plan;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.ESAPI
{
    internal class PlanEdit
    {
        public static void LoadNtoIntoPlan(IPlanInfo plan, INtoInfo nto)
        {
            if (nto != null && plan != null)
            {
                try
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
                    Logger.Write(plan, "NTO added.", LogMessageType.Info);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Source, ex.Message, LogMessageType.Error);
                }
            }
        }

        public static void LoadObjectivesIntoPlan(IPlanInfo plan, IEnumerable<IObjectiveInfo> objectives, bool onlyEmptyStructures = false)
        {
            int loadedObjectivesCount = 0;
            if (objectives == null)
            {
                Logger.Write(plan, "Can't load objectives. Collection is null.", LogMessageType.Error);
                return;
            }
            try
            {
                plan.Plan.Course.Patient.BeginModifications();
                foreach (IObjectiveInfo objective in objectives)
                {
                    if (onlyEmptyStructures && plan.StructureHasObjectives(objective.Structure))
                    {
                        continue;
                    }
                    LoadObjective(plan, objective);
                    loadedObjectivesCount++;
                }
                Logger.Write(plan, String.Format("{0} Objective{1} added.", loadedObjectivesCount, loadedObjectivesCount == 1 ? "" : "s"), LogMessageType.Info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Source, ex.Message, LogMessageType.Error);
            }

        }

        private static void LoadObjective(IPlanInfo plan, IObjectiveInfo objective)
        {
            if (plan.Plan == null)
            {
                Logger.Write(plan, "Can't load the objective. The Plan is null", LogMessageType.Error);
                return;
            }
            if (!plan.IsReadyForOptimizerLoad)
            {
                Logger.Write(plan, "Can't load the objective. The Plan is not unapproved, or it doesn't have beams", LogMessageType.Error);
                return;
            }
            if (objective.Structure == null)
            {
                Logger.Write(plan, "Can't load the objective. Structure is not defined", LogMessageType.Error);
                return;
            }

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

        public static void ClearObjectives(IPlanInfo plan)
        {
            if (plan?.Plan == null)
            {
                Logger.Write(plan, "Plan is null", LogMessageType.Error);
                return;
            }

            try
            {
                int i = 0;
                plan.Plan.Course.Patient.BeginModifications();
                foreach (var objective in plan.Plan.OptimizationSetup.Objectives)
                {
                    plan.Plan.OptimizationSetup.RemoveObjective(objective);
                    ++i;
                }

                Logger.Write(plan, $"Removed objectives count: {i}", LogMessageType.Error);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Source, ex.Message, LogMessageType.Error);
            }

        }
    }
}
