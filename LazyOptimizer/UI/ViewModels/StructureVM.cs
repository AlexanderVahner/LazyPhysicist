using ESAPIInfo.Structures;
using LazyOptimizer.Model;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;
using System.Linq;

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
                /*if (value != null && !Equals(planStructure, value))
                {
                    if (planStructure?.StructureInfo != null)
                    {
                        StructureSuggestions.Insert(1, planStructure); // Insertion into postion 1 because it must be under <none> element
                    }

                    if (value?.StructureInfo != null)
                    {
                        StructureSuggestions.Remove(value);
                    }
                    SetProperty(ref planStructure, value);
                }*/
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
