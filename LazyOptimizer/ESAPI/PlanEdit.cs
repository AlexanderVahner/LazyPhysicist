using ESAPIInfo.Plan;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.Types;

namespace LazyOptimizer.ESAPI
{
    internal class PlanEdit
    {
        private const int MAX_OBJECTIVES_COUNT_FOR_ADDING = 200;

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
                    if (loadedObjectivesCount >= MAX_OBJECTIVES_COUNT_FOR_ADDING)
                    {
                        Logger.Write(plan, $"Too many objectives (>{MAX_OBJECTIVES_COUNT_FOR_ADDING}). Stopped.", LogMessageType.Error);
                        break;
                    }

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
                Logger.Write(plan, "Can't load the objective. Structure is null", LogMessageType.Error);
                return;
            }
            if (objective.Structure.IsEmpty)
            {
                Logger.Write(plan, $"Can't load the objective. Structure \"{objective.Structure.Id}\" is empty", LogMessageType.Error);
                return;
            }

            DoseValue.DoseUnit doseUnit = plan.Plan.DosePerFraction.Unit;
            DoseValue objectiveDose = new DoseValue(objective.Dose * (doseUnit == DoseValue.DoseUnit.cGy ? 100 : 1), doseUnit);

            switch (objective.Type)
            {
                case ObjectiveType.Point:
                    plan.Plan.OptimizationSetup.AddPointObjective(
                        objective.Structure, 
                        (OptimizationObjectiveOperator)(int)objective.Operator,
                        objectiveDose, 
                        objective.Volume, 
                        objective.Priority);
                    break;
                case ObjectiveType.Mean:
                    plan.Plan.OptimizationSetup.AddMeanDoseObjective(
                        objective.Structure,
                        objectiveDose, 
                        objective.Priority);
                    break;
                case ObjectiveType.EUD:
                    plan.Plan.OptimizationSetup.AddEUDObjective(
                        objective.Structure, 
                        (OptimizationObjectiveOperator)(int)objective.Operator,
                        objectiveDose, 
                        objective.ParameterA, 
                        objective.Priority);
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
                var objectives = plan.Plan.OptimizationSetup.Objectives.ToList();
                int i = objectives.Count;

                plan.Plan.Course.Patient.BeginModifications();
                objectives.ForEach(o => plan.Plan.OptimizationSetup.RemoveObjective(o));

                Logger.Write(plan, $"Objectives removed: {i}", LogMessageType.Info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Source, ex.Message, LogMessageType.Error);
            }

        }
    }
}
