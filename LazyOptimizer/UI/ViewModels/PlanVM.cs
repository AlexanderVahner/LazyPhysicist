using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyOptimizerDataService.DBModel;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class PlanVM : ViewModel<IPlanBaseModel>
    {
        private readonly IPlanCachedModel planCachedModel;
        private readonly IPlanMergedModel planMergedModel;
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
                planMergedModel = planModel as IPlanMergedModel;
            }
        }
        
        public string PlanTitle => SourceModel.PlanTitle;
        public string CreationDate => planCachedModel?.CreationDate.ToString("g") ?? "";
        public INtoInfo Nto => SourceModel.NtoInfo;
        public Action<IPlanCachedModel> AddToMergedPlan { get; set; }
        public MetaCommand Merge => new MetaCommand(
            o => {
                AddToMergedPlan?.Invoke(planCachedModel);
                NotifyPropertyChanged(nameof(ElementVisibility));
                NotifyPropertyChanged(nameof(SelectionFrequency));
            },
            o => planCachedModel != null
        );
        public string Description
        {
            get => SourceModel.Description;
            set => SetProperty((v) => { SourceModel.Description = v; }, value);
        }
        public bool IsDescriptionReadOnly => planCachedModel == null;
        public Visibility MergeLinkVisibility => planCachedModel != null ? Visibility.Visible : Visibility.Collapsed;
        public long SelectionFrequency
        {
            get => SourceModel.SelectionFrequency;
            set => SetProperty((v) => { SourceModel.SelectionFrequency = v; }, value);
        }
        public string SelectionFrequencyBackground
        {
            get
            {
                if (planMergedModel != null)
                {
                    return "#FF333333";
                }

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
        public Visibility ElementVisibility => (planMergedModel == null || (planMergedModel?.MergedPlansCount ?? 0) > 0) ? Visibility.Visible : Visibility.Collapsed;
    }
}
