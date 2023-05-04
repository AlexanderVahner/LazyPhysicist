using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class StructureVM : ViewModel<IStructureModel>
    {
        private readonly IStructureModel structureModel;
        private IStructureSuggestionModel planStructureHack;
        public StructureVM(IStructureModel structureModel) : base(structureModel)
        {
            this.structureModel = structureModel;
            StructureSuggestions = structureModel.StructureSuggestions;
            Objectives = new SlaveCollection<IObjectiveModel, ObjectiveVM>(structureModel.Objectives, m => new ObjectiveVM(m), vm => vm.SourceModel);
        }

        public string CachedStructureId => structureModel.CachedStructureId;
        public IStructureSuggestionModel PlanStructure
        {
            get => structureModel.CurrentPlanStructure;
            set
            {
                structureModel.CurrentPlanStructure = value;
                NotifyPropertyChanged(nameof(PlanStructure));
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
    }
}
