using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazyContouring.UI.ViewModels.Operations.ContextConditions
{
    public sealed class ConditionNodeVM
    {
        private readonly ConditionTreeNode node;

        public ConditionNodeVM(ConditionTreeNode node)
        {
            this.node = node;
        }
        public UIElement UIElement { get; set; }
    }
}
