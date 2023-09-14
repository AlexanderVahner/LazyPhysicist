using ScriptArgsNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations.ContextConditions
{
    public abstract class ContextCondition : ConditionTreeNode
    {
        public bool Meets(ScriptArgs args)
        {
            return ShouldBe == CheckCondition(args);
        }

        protected abstract bool CheckCondition(ScriptArgs args);
        public bool ShouldBe { get; protected set; } = true;
    }
}
