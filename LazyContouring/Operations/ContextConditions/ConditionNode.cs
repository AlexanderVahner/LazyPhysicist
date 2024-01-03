using LazyPhysicist.Common;
using ScriptArgsNameSpace;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace LazyContouring.Operations.ContextConditions
{
    [XmlInclude(typeof(ConditionGroup))]
    [XmlInclude(typeof(ContextCondition))]
    public abstract class ConditionNode : Notifier
    {
        public bool CheckNode(ScriptArgs args)
        {
            return CheckNodeDefinition(args);
        }

        protected abstract bool CheckNodeDefinition(ScriptArgs args);

        public ObservableCollection<ConditionNode> Children { get; set; }
    }
}
