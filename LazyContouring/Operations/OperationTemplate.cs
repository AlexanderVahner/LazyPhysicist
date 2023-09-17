using LazyContouring.Operations.ContextConditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations
{
    public sealed class OperationTemplate
    {
        public ObservableCollection<OperationNode> RootNodes { get; set; } = new ObservableCollection<OperationNode>();
        public bool IsAutomatic { get; set; }
        public ObservableCollection<ConditionTreeNode> ContextConditionNodes { get; set; } = new ObservableCollection<ConditionTreeNode> { new ConditionGroup() };
    }
}
