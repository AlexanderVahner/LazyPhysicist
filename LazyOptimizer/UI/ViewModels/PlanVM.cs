using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LazyOptimizer.UI.ViewModels
{
    public partial class PlanVM : ViewModel
    {
        private readonly App.AppContext context;
        private readonly IPlanBaseModel planBaseModel;
        private readonly IPlanCachedModel planCachedModel;
        private readonly IPlanMergedModel mergedPlanModel;
        private ObservableCollection<StructureVM> structures;
        private ObservableCollection<StructureInfo> structureSuggestions;
        private NtoVM ntoVM;
        public PlanVM(IPlanBaseModel planModel, App.AppContext context)
        {
            if (planModel == null || context?.CurrentPlan == null)
            {
                Logger.Write(this, "Can't create a PlanVM - Plan Model or Context is NULL", LogMessageType.Error);
                return;
            }
            this.context = context;
            planBaseModel = planModel;
            if (planModel is IPlanCachedModel pm)
            {
                planCachedModel = pm;
            }
            else
            {
                mergedPlanModel = planModel as IPlanMergedModel;
            }
        }
        public string PlanName => planBaseModel.PlanTitle;
        public string CreationDate => planCachedModel?.CreationDate.ToString("g") ?? "";
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
