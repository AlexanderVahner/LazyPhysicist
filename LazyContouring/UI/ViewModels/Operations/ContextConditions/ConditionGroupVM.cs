using LazyContouring.Operations.ContextConditions;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ConditionGroupVM : Notifier
    {
        private readonly ConditionGroup conditionGroup;

        public ConditionGroupVM(ConditionGroup conditionGroup)
        {
            this.conditionGroup = conditionGroup;
        }

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
    }
}
