using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;

namespace LazyContouring.Operations
{
    public sealed class OperationTemplate : Notifier
    {
        public OperationTemplate() { }

        private string name = "New template";
        private bool isAutomatic = false;

        public override string ToString()
        {
            return Name;
        }

        public string Name { get => name; set => SetProperty(ref name, value); }
        public ObservableCollection<OperationNode> OperationNodes { get; set; } = new ObservableCollection<OperationNode>();
        public bool IsAutomatic { get => isAutomatic; set => SetProperty(ref isAutomatic, value); }
        public ObservableCollection<ConditionNode> ConditionNodes { get; set; } = new ObservableCollection<ConditionNode>();
    }
}
