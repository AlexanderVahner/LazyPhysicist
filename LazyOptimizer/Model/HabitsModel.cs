using ESAPIInfo.Plan;
using LazyOptimizer.ESAPI;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.Model
{
    public sealed class HabitsModel : Notifier
    {
        private readonly App.AppContext context;
        private readonly PlanInteractions planInteractions;
        private readonly IPlanMergedModel planMergedModel;

        public HabitsModel(App.AppContext context)
        {
            this.context = context;

            planInteractions = new PlanInteractions();
            planMergedModel = new PlanMergedModel(context.CurrentPlan, planInteractions);
            planInteractions.PlanMergedModel = planMergedModel;

            UpdatePlans(context.PlansFilterArgs);
            context.PlansFilterArgs.UpdateRequest += (s, args) => UpdatePlans(args);
        }

        public void UpdatePlans(PlansFilterArgs args)
        {
            PlanModels.Clear();
            var plans = context.PlansContext.GetPlans(args);
            if ((plans?.Count() ?? 0) == 0)
            {
                Logger.Write(this, "Seems like you don't have matched plans. Maybe you need to recheck them?", LogMessageType.Warning);
                return;
            }

            PlanModels.Add(planMergedModel);
            foreach (var plan in plans)
            {
                PlanModels.Add(new PlanCachedModel(plan, planInteractions, context));
            }
            Logger.Write(this, $"You have {plans.Count()} matched plan" + (plans.Count() == 1 ? "." : "s."));
        }

        public void FindStructureInOtherPlans(IPlanBaseModel plan, IStructureSuggestionModel structure)
        {
            if (structure == null || plan == null)
            {
                return;
            }

            var objectives = context.PlansContext.GetObjectivesByStructrureId(structure.Id, context.PlansFilterArgs);
            if ((objectives?.Count ?? 0) == 0)
            {
                Logger.Write(this, $@"Sorry, ""{structure.Id}"" was not found in other similar plans.", LogMessageType.Warning);
                return;
            }

            var newStructure = plan.AddStructure(structure.Id, structure);
            foreach (var objective in objectives)
            {
                newStructure.AddObjective(objective);
            }
        }

        public void LoadObjectivesIntoCurrentPlan(IPlanBaseModel planModel, bool fillOnlyEmptyStructures)
        {
            if (planModel == null)
            {
                return;
            }
            List<IObjectiveInfo> objectives = planModel.GetObjectiveInfos().ToList();
            if (objectives.Count == 0)
            {
                Logger.Write(this, "There are no objectives to add.", LogMessageType.Warning);
                return;
            }

            PlanEdit.LoadObjectivesIntoPlan(context.CurrentPlan, objectives, fillOnlyEmptyStructures);

            if (planModel is IPlanCachedModel pcm)
            {
                pcm.SelectionFrequency++;
            }
        }

        public void LoadNtoIntoCurrentPlan(INtoInfo nto)
        {
            PlanEdit.LoadNtoIntoPlan(context.CurrentPlan, nto);
        }

        public ObservableCollection<IPlanBaseModel> PlanModels { get; } = new ObservableCollection<IPlanBaseModel> { };
        public PlanInteractions PlanInteractions => planInteractions;
    }
}
