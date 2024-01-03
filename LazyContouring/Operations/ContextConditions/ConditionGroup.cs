using ScriptArgsNameSpace;
using System.Collections.ObjectModel;
using System.Linq;

namespace LazyContouring.Operations.ContextConditions
{
    public enum ConditionGroupType { Or, And }
    public sealed class ConditionGroup : ConditionNode
    {
        public ConditionGroup()
        {
            Children = new ObservableCollection<ConditionNode>();
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
