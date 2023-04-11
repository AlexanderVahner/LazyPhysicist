using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class PlanVM : ViewModel
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
                Logger.Write(this, "Can't create a PlanVM - Plan Model or Context is NUL L", LogMessageType.Error);
                return;
            }
            this.context = context;
            planBaseModel = planModel;

            // Plan Model Type recognition
            if (planModel is IPlanCachedModel pm)
            {
                planCachedModel = pm;
            }
            else
            {
                mergedPlanModel = planModel as IPlanMergedModel;
            }
        }
        private ObservableCollection<StructureVM> GetStructureVMs()
        {
            if (structures == null)
            {
                structures = new ObservableCollection<StructureVM>();
                UpdateStructures();
            }
            return structures;
        }
        private void UpdateStructures()
        {
            structures.Clear();
            foreach (IStructureModel s in planBaseModel.Structures)
            {
                structures.Add(new StructureVM(s));
            }
        }
        private void UpdateStructureSuggestions()
        {
            foreach (IStructureInfo structure in context.CurrentPlan.Structures.Where(s =>  planBaseModel.Structures.Select(s => { s.CurrentPlanStructure }))
        }
        private NtoVM GetNtoVM()
        {
            if (ntoVM == null)
            {
                UpdateNto();
            }
            return ntoVM;
        }
        private void UpdateNto()
        {
            ntoVM = new NtoVM()
            {
                Nto = planBaseModel.NtoInfo
            };

            NotifyPropertyChanged(nameof(ntoVM));
        }
        public string PlanName => planBaseModel.PlanTitle;
        public string CreationDate => planCachedModel?.CreationDate.ToString("g") ?? "";
        public ObservableCollection<StructureVM> Structures => GetStructureVMs();
        public ObservableCollection<StructureInfo> StructureSuggestions => structureSuggestions ?? (structureSuggestions = new ObservableCollection<StructureInfo>());
        public NtoVM NtoVM => GetNtoVM();
        public string Description
        {
            get => planCachedModel?.Description ?? "";
            set
            {
                if (planCachedModel != null)
                {
                    SetProperty(v => { planCachedModel.Description = v; }, value);
                }
                
            }
        }
        public long? SelectionFrequency
        {
            get => planCachedModel?.SelectionFrequency ?? 0;
            set
            {
                if (planCachedModel != null)
                {
                    SetProperty((v) => { planCachedModel.SelectionFrequency = v; }, value);
                }
            }
        }
        public bool DescriptionVisible => planCachedModel != null;
        public bool SelectionFrequencyVisible => planCachedModel != null;
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
