using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.UI.ViewModels
{
    /// <summary>
    /// ViewModel for HabitsPage.xaml
    /// This class has a part in "HabitsVM_private" file
    /// </summary>
    public partial class HabitsVM : ViewModel
    {
        private readonly HabitsModel habitsModel;
        private readonly App.AppContext context;
        private PlanVM selectedPlanVM;
        public HabitsVM(HabitsModel habitsModel, App.AppContext context)
        {
            this.habitsModel = habitsModel;
            this.context = context;

            UpdatePlans(context.PlansFilterArgs);
            context.PlansFilterArgs.UpdateRequest += (s, args) =>
            {
                UpdatePlans(args);
            };
            NotifyPropertyChanged(nameof(LoadNto));
            NotifyPropertyChanged(nameof(PrioritySetter));
        }
        private void UpdatePlans(PlansFilterArgs args)
        {
            Plans.Clear();
            var plans = habitsModel.GetCachedPlans(args);
            if ((plans?.Count() ?? 0) == 0)
            {
                Logger.Write(this, "Seems like you don't have matched plans. Maybe you need to recheck them?", LogMessageType.Warning);
                return;
            }
            foreach (var plan in plans)
            {
                PlanVM planVM = new PlanVM(context, plan.CachedPlan);
                Plans.Add(planVM);
            }

            Logger.Write(this, $"You have {plans.Count()} matched plan" + (plans.Count() == 1 ? "." : "s."));
        }
        public PlanVM SelectedPlan
        {
            get => selectedPlanVM;
            set => SetProperty((v) => SetSelectedPlan(v), value);
        }
        public bool LoadNto
        {
            get => context?.Settings?.LoadNto ?? false;
            set => SetProperty(v => { if (context?.Settings?.LoadNto != null) context.Settings.LoadNto = v; }, value);
        }
        public string PrioritySetter
        {
            get => context?.Settings?.DefaultPrioritySetValue ?? "";
            set
            {
                if (value == "" || (double.TryParse(value, out double dv) && dv <= 1000))
                {
                    SetProperty(v => { if (context?.Settings?.DefaultPrioritySetValue != null) context.Settings.DefaultPrioritySetValue = v; }, value);
                }
            }
        }
        public MetaCommand LoadIntoPlan => new MetaCommand(
            o => FillCurrentPlan(),
            o => SelectedPlan != null && SelectedPlan.Structures.FirstOrDefault(s => s.APIStructure?.Structure != null) != null
        );
        public MetaCommand SetOarsPriority => new MetaCommand(
            priorityString => SetPriorityForOars(priorityString as string),
            o => SelectedPlan != null && SelectedPlan.Structures.Count > 0
        );
        public ObservableCollection<PlanVM> Plans { get; set; } = new ObservableCollection<PlanVM>();
        public ObservableCollection<StructureVM> Structures { get; } = new ObservableCollection<StructureVM>();
        public ObservableCollection<IStructureInfo> UnusedStructures { get; } = new ObservableCollection<IStructureInfo>();
    }
}
