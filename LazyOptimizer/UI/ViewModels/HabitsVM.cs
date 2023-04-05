using ESAPIInfo.Structures;
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
        public HabitsVM(App.AppContext context)
        {
            InitializeModel(context);
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
        public ObservableCollection<StructureInfo> UnusedStructures { get; } = new ObservableCollection<StructureInfo>();
    }
}
