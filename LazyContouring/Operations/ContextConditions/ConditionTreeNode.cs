using LazyPhysicist.Common;
using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LazyContouring.Operations.ContextConditions
{
    [XmlInclude(typeof(ConditionGroup))]
    [XmlInclude(typeof(ContextCondition))]
    public abstract class ConditionTreeNode : Notifier
    {
        public bool CheckNode(ScriptArgs args)
        {
            return CheckNodeDefinition(args);
        }

        protected abstract bool CheckNodeDefinition(ScriptArgs args);

        public ObservableCollection<ConditionTreeNode> Children { get; set; }
    }
}
