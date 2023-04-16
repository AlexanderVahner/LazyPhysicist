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
    public sealed class HabitsModel : Notifier
    {
        private readonly App.AppContext context;

        public HabitsModel(App.AppContext context)
        {
            this.context = context;
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

            foreach (var plan in plans)
            {
                PlanModels.Add(new PlanCachedModel(plan, context));
            }
            Logger.Write(this, $"You have {plans.Count()} matched plan" + (plans.Count() == 1 ? "." : "s."));
        }
        public ObservableCollection<IPlanBaseModel> PlanModels { get; } = new ObservableCollection<IPlanBaseModel> { };
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
    }
}
