using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations
{
    public enum TemplateGroupType { Or, And }
    public sealed class ConditionGroup : ConditionTreeNode
    {
        public ObservableCollection<ConditionTreeNode> Children { get; set; } = new ObservableCollection<ConditionTreeNode>();
        public TemplateGroupType Type { get; set; } = TemplateGroupType.And;
    }
}
