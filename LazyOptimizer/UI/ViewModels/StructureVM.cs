using ESAPIInfo.Plan;
using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class StructureVM : ViewModel<IStructureModel>
    {
        private readonly IPlanInfo currentPlan;
        private readonly IStructureModel structureModel;
        private IStructureSuggestionModel planStructureHack;
        public StructureVM(IPlanInfo currentPlan, IStructureModel structureModel) : base(structureModel)
        {
            this.currentPlan = currentPlan;
            this.structureModel = structureModel;
            StructureSuggestions = structureModel.StructureSuggestions;
            Objectives = new SlaveCollection<IObjectiveModel, ObjectiveVM>(structureModel.Objectives, m => new ObjectiveVM(m), vm => vm.SourceModel);
            NotifyPropertyChanged(nameof(Color));
            NotifyPropertyChanged(nameof(Brush));
        }

        public string CachedStructureId => structureModel.CachedStructureId;
        public IStructureSuggestionModel PlanStructure
        {
            get => structureModel.CurrentPlanStructure;
            set
            {
                structureModel.CurrentPlanStructure = value;
                NotifyPropertyChanged(nameof(PlanStructure));
                NotifyPropertyChanged(nameof(Color));
                NotifyPropertyChanged(nameof(Brush));
                NotifyPropertyChanged(nameof(PlanTargetAttentionVisible));
            }
        }

        // Buffer for WPF combobox. Because a problem in changing ItemsSource (Removing a selected element)
        public IStructureSuggestionModel PlanStructureHack
        {
            get => planStructureHack;
            set
            {
                if (!Equals(planStructureHack, value))
                {
                    SetProperty(ref planStructureHack, value);
                    if (value != null)
                    {
                        PlanStructure = value;
                    }
                }
            }
        }
        public ObservableCollection<IStructureSuggestionModel> StructureSuggestions { get; }
        public SlaveCollection<IObjectiveModel, ObjectiveVM> Objectives { get; }
        public bool IsTarget => StructureInfo.IsTarget(CachedStructureId);
        public Color Color { get => PlanStructure?.StructureInfo?.Color ?? Color.FromRgb(200, 200, 200); }
        public SolidColorBrush Brush { get => new SolidColorBrush(Color); }
        public Visibility PlanTargetAttentionVisible { get => 
                IsTarget && currentPlan.TargetId != (PlanStructure?.Id ?? "") 
                ? Visibility.Visible : Visibility.Collapsed; }
    }
}
