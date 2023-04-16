using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class PlanVM : ViewModel<IPlanBaseModel>
    {
        private readonly IPlanCachedModel planCachedModel;
        private readonly IPlanMergedModel mergedPlanModel;
        public PlanVM(IPlanBaseModel planModel) : base(planModel) 
        {
            if (planModel == null)
            {
                Logger.Write(this, "Can't create a PlanVM - Plan Model is NULL", LogMessageType.Error);
                return;
            }

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
        public string PlanName => SourceModel.PlanTitle;
        public string CreationDate => planCachedModel?.CreationDate.ToString("g") ?? "";
        public INtoInfo Nto => SourceModel.NtoInfo;
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
