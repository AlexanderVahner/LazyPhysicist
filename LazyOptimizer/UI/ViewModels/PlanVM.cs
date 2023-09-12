using ESAPIInfo.Plan;
using LazyOptimizer.Model;
using LazyPhysicist.Common;
using System.Windows;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class PlanVM : ViewModel<IPlanBaseModel>
    {
        private readonly IPlanCachedModel planCachedModel;
        private readonly IPlanMergedModel planMergedModel;
        private bool canMerge;
        private Visibility mergeLinkVisibility;
        private Visibility elementVisibility;
        private Visibility starVisibility;

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

            planModel.PropertyChanged += (s, e) =>
            {
                NotifyPropertyChanged(e.PropertyName);
                if (e.PropertyName == nameof(SelectionFrequency))
                {
                    NotifyPropertyChanged(nameof(SelectionFrequencyBackground));
                }
            };
            SetMergeFeatureVisibility();
            StarVisibility = planCachedModel != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetMergeFeatureVisibility()
        {
            MergeLinkVisibility = planCachedModel != null && CanMerge ? Visibility.Visible : Visibility.Collapsed;
            ElementVisibility = planCachedModel != null || (planMergedModel != null && CanMerge) ? Visibility.Visible : Visibility.Collapsed;
        }

        public MetaCommand Merge => new MetaCommand(
            o => SourceModel.AddToMerged(),
            o => planCachedModel != null
        );

        public MetaCommand ToggleStarred => new MetaCommand(
            o => IsStarred = !IsStarred,
            o => planCachedModel != null
        );

        public string PlanTitle => SourceModel.PlanTitle;
        public string CreationDate => planCachedModel?.CreationDate.ToString("g") ?? "";
        public INtoInfo Nto => SourceModel.NtoInfo;
        public string Description { get => SourceModel.Description; set => SetProperty((v) => { SourceModel.Description = v; }, value); }
        public bool IsDescriptionReadOnly => planCachedModel == null;
        public long SelectionFrequency { get => SourceModel.SelectionFrequency; set => SetProperty((v) => { SourceModel.SelectionFrequency = v; }, value); }
        public bool IsStarred
        {
            get => planCachedModel?.IsStarred ?? false;
            set => SetProperty((v) =>
            {
                if (planCachedModel != null)
                {
                    planCachedModel.IsStarred = v;
                    NotifyPropertyChanged(nameof(StarImageSource));
                }

            }, value);
        }
        public Visibility MergeLinkVisibility { get => mergeLinkVisibility; set => SetProperty(ref mergeLinkVisibility, value); }
        public Visibility ElementVisibility { get => elementVisibility; set => SetProperty(ref elementVisibility, value); }
        public Visibility StarVisibility { get => starVisibility; set => SetProperty(ref starVisibility, value); }
        public bool CanMerge
        {
            get => canMerge;
            set
            {
                SetProperty(ref canMerge, value);
                SetMergeFeatureVisibility();
            }
        }
        public string StarImageSource
        {
            get
            {
                return IsStarred ? "/LazyOptimizer.esapi;component/UI/Views/starred.png" : "/LazyOptimizer.esapi;component/UI/Views/unstarred.png";
            }
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
    }
}
