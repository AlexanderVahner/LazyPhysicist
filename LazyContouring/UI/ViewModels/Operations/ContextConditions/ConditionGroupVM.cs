using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ConditionGroupVM : ConditionNodeVM
    {
        private readonly ConditionGroup conditionGroup;
        private ConditionNodeVM selectedNodeVM;

        public ConditionGroupVM(ConditionGroup conditionGroup) : base(conditionGroup)
        {
            this.conditionGroup = conditionGroup;
        }

        public MetaCommand AddStructureConditionCommand => new MetaCommand(
            o => conditionGroup.Children.Add(new StructureCondition())
        );

        public MetaCommand AddDiagnosisConditionCommand => new MetaCommand(
            o => conditionGroup.Children.Add(new DiagnosisCondition())
        );

        public MetaCommand AddImageConditionCommand => new MetaCommand(
            o => conditionGroup.Children.Add(new ImageCondition())
        );

        public MetaCommand RemoveConditionCommand => new MetaCommand(
            o => conditionGroup.Children.Remove(SelectedNodeVM.Node),
            o => SelectedNodeVM?.Node != null
        );

        public bool AndChecked
        {
            get => conditionGroup.GroupType == ConditionGroupType.And;
            set
            {
                conditionGroup.GroupType = value ? ConditionGroupType.And : ConditionGroupType.Or;
                NotifyPropertyChanged(nameof(AndChecked));
                NotifyPropertyChanged(nameof(OrChecked));
            }
        }
        public bool OrChecked
        {
            get => conditionGroup.GroupType == ConditionGroupType.Or;
            set
            {
                conditionGroup.GroupType = value ? ConditionGroupType.Or : ConditionGroupType.And;
                NotifyPropertyChanged(nameof(AndChecked));
                NotifyPropertyChanged(nameof(OrChecked));
            }
        }

        public ConditionNodeVM SelectedNodeVM
        {
            get => selectedNodeVM;
            set => SetProperty(ref selectedNodeVM, value);
        }
    }
}
