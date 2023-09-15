using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations.ContextConditions
{
    public enum ConditionGroupType { Or, And }
    public sealed class ConditionGroup : ConditionTreeNode
    {
        public ConditionGroup()
        {
            Children = new ObservableCollection<ConditionTreeNode>();
        }

        protected override bool CheckNodeDefinition(ScriptArgs args)
        {
            if (Children.Count == 0)
            {
                return true;
            }

            if (GroupType == ConditionGroupType.Or)
            {
                return Children.FirstOrDefault(c => c.CheckNode(args)) != null;
            }

            // "And" type
            return Children.FirstOrDefault(c => !c.CheckNode(args)) == null;
        }

        public ConditionGroupType GroupType { get; set; } = ConditionGroupType.And;
        
    }
}
