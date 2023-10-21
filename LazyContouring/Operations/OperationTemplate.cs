using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations
{
    public sealed class OperationTemplate : Notifier
    {
        public OperationTemplate()
        {
            RootConditionGroup = new ConditionGroup();
        }

        private string name = "New template";
        private bool isAutomatic = false;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public ObservableCollection<OperationNode> RootNodes { get; set; } = new ObservableCollection<OperationNode>();
        public bool IsAutomatic { get => isAutomatic; set => SetProperty(ref isAutomatic, value); }
        public ConditionGroup RootConditionGroup { get; set; }
        public ObservableCollection<ConditionTreeNode> ChildrenConditionNodes { get; set; } = new ObservableCollection<ConditionTreeNode>();
    }
}
