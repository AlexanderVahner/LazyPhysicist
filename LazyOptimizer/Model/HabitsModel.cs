using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.ESAPI;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace LazyOptimizer.Model
{
    public sealed class HabitsModel
    {
        private readonly App.AppContext context;
        public HabitsModel(App.AppContext context)
        {
            this.context = context;
        }

        public IEnumerable<PlanCachedModel> GetCachedPlans(PlansFilterArgs args)
        {
            var plans = context.PlansContext.GetPlans(args);
            if ((plans?.Count() ?? 0) == 0)
            {
                yield break;
            }
            foreach (var plan in plans)
            {
                yield return new PlanCachedModel(plan, context);
            }
        }
        public void LoadObjectivesIntoCurrentPlan(PlanInfo currentPlan, IPlanCachedModel cachedPlan, bool fillOnlyEmptyStructures)
        {
            if (cachedPlan == null)
            {
                return;
            }
            List<ObjectiveInfo> objectives = GetObjectivesForCurrentPlan(cachedPlan).ToList();
            if (objectives.Count == 0)
            {
                Logger.Write(this, "There are no objectives to add.", LogMessageType.Warning);
                return;
            }

            PlanEdit.LoadObjectives(currentPlan, objectives, fillOnlyEmptyStructures);
            cachedPlan.SelectionFrequency++;
        }
        public void LoadNtoIntoCurrentPlan(IPlanInfo currentPlan, INtoInfo nto)
        {
            PlanEdit.LoadNtoIntoPlan(currentPlan, nto);
        }
        private IEnumerable<ObjectiveInfo> GetObjectivesForCurrentPlan(IPlanCachedModel plan)
        {
            if ((plan?.Structures.Count ?? 0) == 0)
            {
                yield break;
            }
            foreach (IStructureModel s in plan.Structures)
            {
                if (s.CurrentPlanStructure != null && s.CurrentPlanStructure.StructureInfo != null && s.Objectives.Count() > 0)
                {
                    foreach (IObjectiveModel obj in s.Objectives)
                    {
                        if (obj.CachedObjective != null)
                        {
                            yield return GetObjectiveInfo(obj.CachedObjective, s.CurrentPlanStructure.StructureInfo.Structure);
                        }
                    }
                }
            }
        }
        private ObjectiveInfo GetObjectiveInfo(CachedObjective objective, Structure structure)
        {
            ObjectiveInfo result = null;
            if (objective == null)
            {
                Logger.Write(this, "Objective View Model doesn't have an CachedObjective.", LogMessageType.Error);
            }
            else
            {
                result = new ObjectiveInfo()
                {
                    Type = (ObjectiveType)(objective.ObjType ?? 99),
                    Structure = structure,
                    StructureId = objective.StructureId,
                    Priority = objective.Priority ?? .0,
                    Operator = (Operator)objective.Operator,
                    Dose = objective.Dose ?? .0,
                    Volume = objective.Volume ?? .0,
                    ParameterA = objective.ParameterA ?? .0
                };
            }

            return result;
        }
    }
}
