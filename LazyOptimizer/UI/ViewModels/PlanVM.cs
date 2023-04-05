using ESAPIInfo.Structures;
using LazyOptimizerDataService.DBModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LazyOptimizer.UI.ViewModels
{
    /// <summary>
    /// ViewModel for PlanElement.xaml
    /// This class has a part in "PlanVM_private" file
    /// </summary>
    public partial class PlanVM : ViewModel
    {
        public PlanVM(App.AppContext context, CachedPlan cachedPlan)
        {
            this.context = context;
            this.cachedPlan = cachedPlan;
        }
        public CachedPlan CachedPlan
        {
            get => cachedPlan;
            set => SetProperty(ref cachedPlan, value);
        }
        public string PlanName => $"({CachedPlan?.PatientId}).[{CachedPlan?.CourseId}].{CachedPlan?.PlanId}";
        public string CreationDate => CachedPlan?.CreationDate.ToString("g") ?? "";
        public string StructuresString => CachedPlan?.StructuresString;
        public List<CachedObjective> ObjectivesCache => GetCachedObjectives();
        public ObservableCollection<StructureVM> Structures => GetStructureVMs();
        public ObservableCollection<StructureInfo> StructureSuggestions => structureSuggestions ?? (structureSuggestions = new ObservableCollection<StructureInfo>());
        public NtoVM NtoVM => GetNtoVM();
        public string Description
        {
            get => CachedPlan?.Description;
            set
            {
                SetProperty((v) => { if (CachedPlan != null) CachedPlan.Description = v; }, value);
                context.PlansContext.UpdatePlan(cachedPlan);
            }
        }
        public long? SelectionFrequency
        {
            get => CachedPlan?.SelectionFrequency;
            set
            {
                SetProperty((v) => { if (CachedPlan != null) CachedPlan.SelectionFrequency = v; }, value);
                context.PlansContext.UpdatePlan(CachedPlan);
            }
        }
        public string SelectionFrequencyBackground
        {
            get
            {
                string color = "#FF4646FF";
                if (SelectionFrequency > 5)
                    color = "#FFE83C03";
                else if (SelectionFrequency > 2)
                    color = "#FFD0B13E";
                else if (SelectionFrequency > 1)
                    color = "#FFCED672";
                else if (SelectionFrequency > 0)
                    color = "#FF3E8337";
                return color;
            }
        }

    }
}
